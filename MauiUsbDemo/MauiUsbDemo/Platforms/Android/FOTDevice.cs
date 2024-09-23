using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Google.Android.Material.Color.Utilities;
using Java.Nio;
using Kotlin.Reflect;
using Microsoft.Maui.Platform;

namespace MauiUsbDemo
{
    public partial class FOTDevice
    {
        internal static readonly string ACTION_USB_PERMISSION = "KOGATOUCH.FOTDevice.USB_PERMISSION";
        private static FOTUsbActionReciver UsbActionReceiver = new FOTUsbActionReciver();
        private static bool _Inited = false;
        private static Android.App.Activity? _Activity;
        private static UsbManager? _UsbManager;
        private static UsbDevice? _UsbDevice;
        private static UsbInterface? _UsbBulkInterface;
        private static UsbDeviceConnection? _UsbDeviceConnection;
        private static Thread? _BulkReadThread;
        private static bool _StopBulkReadThread = true;

        private static bool _IsConnected;
        public static bool IsConnected { get => _IsConnected; private set => _IsConnected = value; }

        public static partial bool Init()
        {
            if (_Inited)
            {
                return true;
            }

            Android.App.Activity? act = Platform.CurrentActivity;

            if (act == null)
            {
                return false;
            }

            _Activity = act;

            UsbManager? um = (UsbManager?)_Activity.GetSystemService(Android.Content.Context.UsbService);
            if (um == null)
            {
                return false;
            }

            _UsbManager = um;

            RegisterBoardcast();

            UsbDevice? device = FindFOTUsbDevice();
            if (device != null)
            {
                if (_UsbManager.HasPermission(device))
                {
                    ConnectToDevice(device);
                }
                else
                {
                    RequestPermission(device);
                }
            }

            _Inited = true;
            return true;
        }


        public static partial void Uninit()
        {
            if( !_Inited)
            { return; }

            DisconnectToDevice();

            UnregisterBoardcast();

            _UsbManager = null;
            _Activity = null;

            _Inited = false;
        }

        private const int ReadBuffLength = 16384;
        private static void BulkReadProc()
        {
            _StopBulkReadThread = false;

            if (_UsbDeviceConnection.ClaimInterface(_UsbBulkInterface, true))
            {
                byte[] buffer = new byte[ReadBuffLength];
                MemoryStream ms = new MemoryStream();

                while (!_StopBulkReadThread)
                {
                    int readCnt = _UsbDeviceConnection.BulkTransfer(_UsbBulkInterface.GetEndpoint(0), buffer, ReadBuffLength, 10);
                    if (readCnt < 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Read error {readCnt}");
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine($"Read {cnt++},{readCnt}");
                        ms.Write(buffer, 0, readCnt);

                        if (readCnt < ReadBuffLength)
                        {
                            if (ms.Length == FOT_Bulk_RawImage_Packet.FOT_BULK_RAWIMAGE_PACKET_SIZE)
                            {
                                ms.Seek(0, SeekOrigin.Begin);
                                using (BinaryReader br = new BinaryReader(ms))
                                {
                                    FOT_Bulk_RawImage_Head head = new FOT_Bulk_RawImage_Head();
                                    head.Prefix = br.ReadUInt32();
                                    head.Length = br.ReadUInt32();

                                    if (head.IsValidHead())
                                    {
                                        FOT_Bulk_RawImage rawImg = new FOT_Bulk_RawImage();
                                        br.Read(rawImg.Image0);
                                        br.Read(rawImg.Image1);

                                        OnRawImageReceived(new RawImageReceivedEventArgs(rawImg));
                                    }
                                }

                                ms = new MemoryStream();
                            }
                            else
                            {
                                ms.Close();
                                ms.Dispose();
                                ms = new MemoryStream();
                            }
                        }

                        if (ms.Length > FOT_Bulk_RawImage_Packet.FOT_BULK_RAWIMAGE_PACKET_SIZE)
                        {
                            ms.Close();
                            ms.Dispose();
                            ms = new MemoryStream();
                        }
                    }
                }

                try
                {
                    ms.Close();
                    ms.Dispose();
                }
                catch { }
            }

            _UsbDeviceConnection.ReleaseInterface(_UsbBulkInterface);
            _StopBulkReadThread = true;
        }

        private static void ConnectToDevice(UsbDevice usbDevice)
        {
            lock (_Activity)
            {
                if (!IsConnected)
                {
                    UsbDeviceConnection? udc = _UsbManager.OpenDevice(usbDevice);
                    if(udc ==  null)
                    {
                        return;
                    }

                    _UsbDeviceConnection = udc;
                    _UsbBulkInterface = usbDevice.GetInterface(0);
                    _UsbDevice = usbDevice;
                    
                    _BulkReadThread = new Thread(new ThreadStart(BulkReadProc));
                    _BulkReadThread.IsBackground = true;
                    _BulkReadThread.Start();
                    while (!_BulkReadThread.IsAlive) { }

                    IsConnected = true;

                    OnConnected();
                }
            }
        }

        private static void OnConnected()
        {
            if(Connected!=null)
            {
                Connected(null, EventArgs.Empty);
            }
        }

        private static void DisconnectToDevice()
        {
            lock (_Activity)
            {
                if (IsConnected)
                {
                    IsConnected = false;
                    _StopBulkReadThread = true;

                    _BulkReadThread?.Join();

                    _UsbDeviceConnection?.Dispose();
                    _UsbBulkInterface?.Dispose();
                    _UsbDevice?.Dispose();

                    _UsbBulkInterface = null;
                    _UsbDeviceConnection = null;
                    _UsbDevice = null;
                    _BulkReadThread = null;

                    OnDisconnected();
                }
            }
        }

        private static void OnDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(null, EventArgs.Empty);
            }
        }

        private static Android.Hardware.Usb.UsbDevice? FindFOTUsbDevice()
        {
            if (_UsbManager != null && _UsbManager.DeviceList != null)
            {
                foreach (var device in _UsbManager.DeviceList.Values)
                {
                    if (device.VendorId == VENDOR_ID && device.ProductId == PRODUCT_ID)
                    { return device; }
                }
            }

            return null;
        }

        private static bool RequestPermission(UsbDevice device)
        {
            var act = Platform.CurrentActivity;
            if (act != null)
            {
                UsbManager? um = (UsbManager?)act.GetSystemService(Android.Content.Context.UsbService);

                if (um != null)
                {

                    PendingIntent? mPermissionIntent;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                    {
                        mPermissionIntent = PendingIntent.GetBroadcast(_Activity, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Mutable);
                    }
                    else
                    {
                        mPermissionIntent = PendingIntent.GetBroadcast(_Activity, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
                    }

                    um.RequestPermission(device, mPermissionIntent);

                    return true;
                }
            }

            return false;
        }

        internal static bool RegisterBoardcast()
        {
            if (_Activity == null)
                return false;

            IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
            filter.AddAction(UsbManager.ActionUsbDeviceDetached);
            filter.AddAction(UsbManager.ActionUsbDeviceAttached);
            _Activity.RegisterReceiver(UsbActionReceiver, filter);

            return true;
        }

        private static void UnregisterBoardcast()
        {
            if (_Activity != null)
            {
                _Activity.UnregisterReceiver(UsbActionReceiver);
            }
        }

        private class FOTUsbActionReciver : BroadcastReceiver
        {
            public override void OnReceive(Context? context, Intent? intent)
            {
                if (intent != null)
                {
                    string? action = intent.Action;
                    Android.Hardware.Usb.UsbDevice? device;

                    if (action != null)
                    {
                        System.Diagnostics.Debug.WriteLine(action);
                        if (UsbManager.ActionUsbDeviceAttached.Equals(action))
                        {
                            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                            {
                                device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice, Java.Lang.Class.FromType(typeof(Android.Hardware.Usb.UsbDevice)));
                            }
                            else
                            {
                                device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                            }

                            if (device != null)
                            {
                                if (device.VendorId == FOTDevice.VENDOR_ID && device.ProductId == FOTDevice.PRODUCT_ID)
                                {
                                    UsbManager? um = (UsbManager?)context?.GetSystemService(Android.Content.Context.UsbService);
                                    if (!um.HasPermission(device))
                                    {
                                        RequestPermission(device);
                                    }
                                }
                            }
                        }

                        if(UsbManager.ActionUsbDeviceDetached.Equals(action))
                        {
                            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                            {
                                device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice, Java.Lang.Class.FromType(typeof(Android.Hardware.Usb.UsbDevice)));
                            }
                            else
                            {
                                device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                            }

                            if (device != null && IsConnected)
                            {
                                if (device.Equals(_UsbDevice))
                                {
                                    DisconnectToDevice();
                                }
                            }
                        }

                        if (FOTDevice.ACTION_USB_PERMISSION.Equals(action))
                        {
                            lock (this)
                            {
                                device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                                if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                                {
                                    if (device != null)
                                    {
                                        ConnectToDevice(device);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
