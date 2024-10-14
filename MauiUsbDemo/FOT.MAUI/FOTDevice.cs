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

    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct FOT_Bulk_RawImage_Head
    {
        public const UInt32 PACKET_PREFIX = 0xEFFE0110;
        public const UInt32 RAWIMAGE_PAYLOAD_SIZE = 0x0001C7A0;
        //public const UInt32 RAWIMAGE_PAYLOAD_SIZE = 0x0001C9F0;

        public UInt32 Prefix;       
        public UInt32 Length;
        public UInt32 Reserve0;
        public UInt32 Reserve1;

        public bool IsValidHead()
        {
            return (Prefix == PACKET_PREFIX && Length == RAWIMAGE_PAYLOAD_SIZE);
        }
    }

    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct FOT_Bulk_RawImage
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 58320)]  //1080*54
        public byte[] Image0 = new byte[58320];

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 58320)]   //1080*54
        public byte[] Image1 = new byte[58320];

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 592)]
        public byte[] Reserve = new byte[592];

        public FOT_Bulk_RawImage()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FOT_Bulk_RawImage_Packet
    {
        public const int PACK_ALIGN_SIZE= 512;
        public const int FOT_BULK_RAWIMAGE_PACKET_SIZE = PACK_ALIGN_SIZE * 229;

        public FOT_Bulk_RawImage_Head Head;
        public FOT_Bulk_RawImage Data;
    }

    public class RawImageReceivedEventArgs:EventArgs
    {
        public FOT_Bulk_RawImage RawImage { get; }

        public RawImageReceivedEventArgs(FOT_Bulk_RawImage rawImage) => RawImage = rawImage;
    }

}
