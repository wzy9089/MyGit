using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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
}
