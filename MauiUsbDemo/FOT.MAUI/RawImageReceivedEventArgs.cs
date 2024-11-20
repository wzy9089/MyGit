using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{

    public class RawImageReceivedEventArgs : EventArgs
    {
        public FOT_Bulk_RawImage RawImage { get; }

        public RawImageReceivedEventArgs(FOT_Bulk_RawImage rawImage) => RawImage = rawImage;
    }

}
