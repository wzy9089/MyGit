using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;
using Nefarius.Utilities.DeviceManagement;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace KOGA.FOT.MAUI
{
    public partial class FOTDevice
    {
        static readonly Guid FOT_WINUSB_GUID = new Guid("{661db43b-90fe-45b1-a815-2e32bd85e0b3}");
        static readonly string FOT_BULK_DEV_SELECT = UsbDevice.GetDeviceSelector(VENDOR_ID, PRODUCT_ID, FOT_WINUSB_GUID);
        static readonly string FOT_FEATURE_DEV_SELECT = HidDevice.GetDeviceSelector(0xFFF4, 0x33, VENDOR_ID, PRODUCT_ID);

        static string? _FOTDeviceId;
        static UsbDevice? _FOTDevice;
        static HidDevice? _FeatureDevice;
        static bool _Inited = false;
        static bool _IsConnected = false;
        static DeviceWatcher? _Watcher;
        static Thread? _BulkReadThread;
        static bool _StopBulkReadThread = true;

        static object _Lock = new object(); 

        public static partial bool Init()
        {
            if (_Inited) return true;

            _Watcher = DeviceInformation.CreateWatcher(FOT_BULK_DEV_SELECT);
            _Watcher.Added += _Watcher_Added;
            _Watcher.Removed += _Watcher_Removed;
            DeviceInformationCollection devInfos = DeviceInformation.FindAllAsync(FOT_BULK_DEV_SELECT).AsTask().Result;

            var devInfo = devInfos.FirstOrDefault();
            if (devInfo != null)
            {
                ConnectToDevice(devInfo.Id);
            }
            
            _Watcher.Start();

            _Inited = true;
            return true;
        }

        public static partial void Uninit()
        {
            if (!_Inited) return;

            DisconnectToDevice();

            if (_Watcher != null)
            {
                _Watcher.Stop();
                _Watcher.Added -= _Watcher_Added;
                _Watcher.Removed -= _Watcher_Removed;
            }

            if (_FOTDevice != null)
            {
                _FOTDevice.Dispose();
                _FOTDevice = null;
            }

            _FOTDeviceId = null;

            _Inited = false;
        }

        private static void _Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (args.Id.Equals(_FOTDeviceId)
                && _FOTDevice != null)
            {
                DisconnectToDevice();
            }
        }

        private static void _Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if(_FOTDevice == null)
            {
                ConnectToDevice(args.Id);
            }
        }

        private const int BulkReadBuffLength = 32768;

        private static void BulkReadProc()
        {
            if (_FOTDevice != null)
            {
                _StopBulkReadThread = false;
                UsbBulkInPipe pipe = _FOTDevice.DefaultInterface.BulkInPipes[0];
                Windows.Storage.Streams.Buffer buff = new Windows.Storage.Streams.Buffer(BulkReadBuffLength);
                MemoryStream ms = new MemoryStream();

                while (!_StopBulkReadThread)
                {
                    try
                    {
                        var readTask = pipe.InputStream.ReadAsync(buff, BulkReadBuffLength, InputStreamOptions.Partial).AsTask();
                        if (!readTask.IsFaulted)
                        {
                            readTask.Wait(100);
                            if (readTask.IsCompleted)
                            {
                                var data = readTask.Result;
                                if ((data != null))
                                {
                                    ms.Write(data.ToArray());

                                    if (data.Length < BulkReadBuffLength)
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
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }
            }
        }

        private static void ConnectToDevice(string devId)
        {
            lock (_Lock)
            {
                if (_IsConnected) return;

                _FOTDeviceId = devId;
                _FOTDevice = UsbDevice.FromIdAsync(devId).AsTask().Result;

                var infos = DeviceInformation.FindAllAsync(FOT_FEATURE_DEV_SELECT).AsTask().Result;
                
                PnPDevice bulkDevice = PnPDevice.GetDeviceByInterfaceId(devId);
                foreach (var info in infos)
                {
                    PnPDevice featureDevice = PnPDevice.GetDeviceByInterfaceId(info.Id);
                    if(bulkDevice.Parent.Equals(featureDevice.Parent.Parent))
                    {
                        _FeatureDevice = HidDevice.FromIdAsync(info.Id, Windows.Storage.FileAccessMode.ReadWrite).AsTask().Result;
                    }
                }

                _BulkReadThread = new Thread(BulkReadProc);
                _BulkReadThread.IsBackground = true;
                _BulkReadThread.Start();
                while (!_BulkReadThread.IsAlive) { }

                _IsConnected = true;

                OnConnected();
            }
        }

        private static void DisconnectToDevice()
        {
            lock (_Lock)
            {
                if (!_IsConnected) return;

                _StopBulkReadThread = true;
                if (_BulkReadThread.IsAlive)
                {
                    _BulkReadThread?.Join();
                }
                _BulkReadThread = null;

                _FOTDevice?.Dispose();
                _FOTDevice = null;

                _FeatureDevice?.Dispose();
                _FeatureDevice = null;

                _FOTDeviceId = null;

                _IsConnected = false;

                OnDisconnected();
            }
        }

        private static UsbDevice? FindFOTUsbDevice()
        {
            return null;
        }

        public static partial int GetFeature(byte reportId, byte[] data, int length, int timeout)
        {
            if (_FeatureDevice == null)
                return -1;

            var rpt = _FeatureDevice.GetFeatureReportAsync(reportId).AsTask().Result;
            rpt.Data.CopyTo(data);

            return (int)rpt.Data.Length;
        }

        public static partial int SetFeature(byte reportId, byte[] data, int length, int timeout)
        { 
            if(_FeatureDevice == null)
                return -1;

            HidFeatureReport rpt = _FeatureDevice.CreateFeatureReport(reportId);
            rpt.Data = data.AsBuffer();
            var ret = _FeatureDevice.SendFeatureReportAsync(rpt).AsTask().Result;

            return (int)ret; 
        }

    }
}
