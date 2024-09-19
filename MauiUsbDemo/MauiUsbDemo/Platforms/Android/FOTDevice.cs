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

        private Android.Hardware.Usb.UsbDevice? _FOTUsbDevice;
        private UsbDeviceConnection? _FOTUsbConnection;
        private UsbInterface? _FOTBulkInterface;
        private Thread? _BulkReadThread;
        private bool _StopThread = false;
        private bool _Connected;
        public bool Connected { get => _Connected; private set => _Connected = value; }

        internal static readonly string ACTION_USB_PERMISSION = "KOGATOUCH.FlatOpticalTouch.usb.device";
        private static FOTUsbActionReciver UsbActionReceiver = new FOTUsbActionReciver();
        private static bool _Inited = false;
        private static Android.App.Activity? _Activity;
        private static UsbManager? _UsbManager;

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
                    System.Diagnostics.Debug.WriteLine($"HasPermission:{device.ToString()}");
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
        { }

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
            if (_Activity == null || _UsbManager == null)
            {
                return false;
            }

            PendingIntent? mPermissionIntent = PendingIntent.GetBroadcast(_Activity, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
            _UsbManager.RequestPermission(device, mPermissionIntent);

            return true;
        }

        private static bool RegisterBoardcast()
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

        private FOTDevice(UsbDevice device)
        {
            System.Diagnostics.Debug.Assert(device != null);

            _FOTUsbDevice = device;

            UsbDeviceConnection? udc = _UsbManager?.OpenDevice(device);
            if (udc != null)
            {
                _FOTUsbConnection = udc;
                _FOTBulkInterface = _FOTUsbDevice.GetInterface(0);
                _BulkReadThread = new Thread(new ThreadStart(BulkReadProc));
                _BulkReadThread.Start();
                while (!_BulkReadThread.IsAlive) { }
                Connected = true;
            }
        }


        
        public bool Connect()
        {
            if (Connected)
                return true;

            Android.Hardware.Usb.UsbDevice? usbDevice = FindFOTUsbDevice();
            if (usbDevice != null)
            {
                Android.App.Activity? act = Platform.CurrentActivity;
                if (act != null)
                {
                    UsbManager? um = (UsbManager?)act.GetSystemService(Android.Content.Context.UsbService);
                    if (um != null && um.DeviceList != null)
                    {
                        PendingIntent? mPermissionIntent = PendingIntent.GetBroadcast(act, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Mutable);
                        IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);

                        if (!um.HasPermission(usbDevice))
                        {
                            um.RequestPermission(usbDevice, mPermissionIntent);
                        }
                        else
                        {
                            UsbDeviceConnection? udc = um.OpenDevice(usbDevice);
                            if (udc != null)
                            {
                                _FOTUsbDevice = usbDevice;
                                _FOTUsbConnection = udc;
                                _FOTBulkInterface = usbDevice.GetInterface(0);
                                _BulkReadThread = new Thread(new ThreadStart(BulkReadProc));
                                //_BulkReadThread.IsBackground = true;
                                _BulkReadThread.Start();
                                while (!_BulkReadThread.IsAlive) { }
                                Connected = true;

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private const int ReadBuffLength = 16384;
        private void BulkReadProc()
        {
            _StopThread = false;

            if(_FOTUsbConnection.ClaimInterface(_FOTBulkInterface,true))
            {
                byte[] buffer = new byte[ReadBuffLength];
                MemoryStream ms = new MemoryStream();

                int cnt = 0;
                while (!_StopThread)
                {
                    int readCnt = _FOTUsbConnection.BulkTransfer(_FOTBulkInterface.GetEndpoint(0), buffer, ReadBuffLength, 10);
                    if (readCnt < 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Read error {readCnt}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Read {cnt++},{readCnt}");
                        //if (ms.Position == 0)
                        //{
                        //    IntPtr headPtr = Marshal.AllocHGlobal(Marshal.SizeOf<FOT_Bulk_RawImage_Head>());
                        //    Marshal.Copy(buffer, 0, headPtr, Marshal.SizeOf<FOT_Bulk_RawImage_Head>());
                        //    FOT_Bulk_RawImage_Head head = Marshal.PtrToStructure<FOT_Bulk_RawImage_Head>(headPtr);
                        //    if (head.IsValidHead())
                        //    {
                        //        ms.Write(buffer, 0, readCnt);
                        //    }
                        //    else
                        //    {
                        //        continue;
                        //    }
                        //}
                        //else if (ms.Length<FOT_Bulk_RawImage_Packet.FOT_BULK_RAWIMAGE_PACKET_SIZE)
                        //{
                        //    ms.Write(buffer,0,readCnt);
                        //    if(ms.Length==FOT_Bulk_RawImage_Packet.FOT_BULK_RAWIMAGE_PACKET_SIZE)
                        //    {
                        //        byte[] dataBuffer = ms.GetBuffer();
                        //        FOT_Bulk_RawImage rawImage = new FOT_Bulk_RawImage();
                        //        int headSize = Marshal.SizeOf<FOT_Bulk_RawImage_Head>();
                        //        Array.Copy(dataBuffer, headSize, rawImage.Image0, 0, rawImage.Image0.Length);
                        //        Array.Copy(dataBuffer, headSize + rawImage.Image0.Length, rawImage.Image1, 0, rawImage.Image1.Length);

                        //        Debug.WriteLine("Raw image data received.");

                        //        ms.Position = 0;
                        //    }
                        //}
                    }
                }

                ms.Close();
            }

            _StopThread = true;
        }

        public void Disconnect()
        {
            if(!Connected) return;

            _StopThread = true;
            _BulkReadThread?.Join();
            _FOTUsbConnection?.ReleaseInterface(_FOTBulkInterface);
            _FOTUsbConnection?.Close();
            _FOTUsbConnection?.Dispose();
            _FOTBulkInterface?.Dispose();
            _FOTUsbDevice?.Dispose();
            _FOTUsbDevice = null;
            _FOTBulkInterface = null;
            _FOTUsbConnection = null;
            _BulkReadThread = null;

            Connected = false;
        }
    }

    internal class FOTUsbActionReciver : BroadcastReceiver
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
                    if(UsbManager.ActionUsbDeviceAttached.Equals(action))
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                        {
                            device = intent.GetParcelableExtra(UsbManager.ExtraDevice, Java.Lang.Class.FromType(typeof(Android.Hardware.Usb.UsbDevice))) as Android.Hardware.Usb.UsbDevice;
                        }
                        else 
                        {
                            device = intent.GetParcelableExtra(UsbManager.ExtraDevice) as Android.Hardware.Usb.UsbDevice;
                        }

                        if (device != null)
                        {
                            if (device.VendorId == FOTDevice.VENDOR_ID && device.ProductId == FOTDevice.PRODUCT_ID)
                            {
                                UsbManager? um = (UsbManager?)context?.GetSystemService(Android.Content.Context.UsbService);
                                if (!um.HasPermission(device))
                                {
                                    um.RequestPermission(device, PendingIntent.GetBroadcast(context, 0, new Intent(FOTDevice.ACTION_USB_PERMISSION), PendingIntentFlags.Immutable));
                                }
                            }
                        }
                    }

                    if(FOTDevice.ACTION_USB_PERMISSION.Equals(action))
                    {
                        device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                        if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                        {
                            if (device != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Granted:{device.ToString()}");
                            }
                        }
                    }
                }
            }
        }
    }
}
