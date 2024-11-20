using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    public partial class FOTDevice
    {
        internal const int VENDOR_ID = 0x2621;
        internal const int PRODUCT_ID = 0x5001;
        internal const string WINUSB_GUID = "{661db43b-90fe-45b1-a815-2e32bd85e0b3}";

        public static event EventHandler<RawImageReceivedEventArgs>? RawImageReceived;
        public static event EventHandler? Connected;
        public static event EventHandler? Disconnected;

        private static void OnConnected()
        {
            Connected?.Invoke(null, EventArgs.Empty);
        }

        private static void OnDisconnected()
        {
            Disconnected?.Invoke(null, EventArgs.Empty);
        }

        private static void OnRawImageReceived(RawImageReceivedEventArgs e)
        {
            RawImageReceived?.Invoke(null, e);
        }

        public static partial bool Init();

        public static partial void Uninit();

        public static partial int GetFeature(byte reportId, byte[] data, int length, int timeout);

        public static partial int SetFeature(byte reportId, byte[] data, int length, int timeout);
    }


}
