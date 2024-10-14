using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOGA.FOT.MAUI
{
    public class RawImageView:SKCanvasView
    {
        private SKBitmap? _RawImage;
        public SKBitmap? RawImage
        {
            get
            {
                return _RawImage;
            }
            set
            {
                if (_RawImage != value)
                {
                    _RawImage = value;
                    this.InvalidateSurface();
                }
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            if (RawImage == null)
            {
                canvas.Clear();
                return;
            }
            else
            {
                //canvas.DrawPoints(SKPointMode.)
                //SKBitmap bmp = RawImage.Resize(e.Info.Size, SKFilterQuality.Medium);
                canvas.DrawBitmap(RawImage, e.Info.Rect);
            }

            //base.OnPaintSurface(e);
        }
    }
}
