using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nefarius.Drivers.WinUSB;

namespace KOGA.FOT.MAUI
{
    public partial class FOTDevice
    {
        public static partial bool Init()
        {
            return false;
        }

        public static partial void Uninit() { }

        public static partial int GetFeature(byte reportId, byte[] data, int length, int timeout)
        { return -1; }

        public static partial int SetFeature(byte reportId, byte[] data, int length, int timeout)
        { return -1; }

    }
}
