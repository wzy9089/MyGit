using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FOT_Bulk_RawImage_Packet
    {
        public const int PACK_ALIGN_SIZE = 512;
        public const int FOT_BULK_RAWIMAGE_PACKET_SIZE = PACK_ALIGN_SIZE * 229;

        public FOT_Bulk_RawImage_Head Head;
        public FOT_Bulk_RawImage Data;
    }

}
