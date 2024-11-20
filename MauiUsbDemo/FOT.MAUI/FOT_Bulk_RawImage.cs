using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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
}
