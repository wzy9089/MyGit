using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    public sealed class RawDataHelper
    {
        public static SKBitmap? CreateSKBitmapFromRawImageData(int imgWidth,int imgHeight, byte[] data)
        {
            if (data == null || data.Length != imgHeight * imgWidth)
                return null;

            SKBitmap bmp = new SKBitmap(imgWidth, imgHeight, SKColorType.Gray8, SKAlphaType.Opaque);

            nint bmpPtr = bmp.GetPixels();
            Marshal.Copy(data, 0, bmpPtr, data.Length);

            return bmp;
        }

    }
}
