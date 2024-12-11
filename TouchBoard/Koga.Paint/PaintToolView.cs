using Koga.Paint.Recognizer;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class PaintToolView: TouchCanvasView
    {
        public PaintTool? PaintTool { get; set; }
        public PaintToolView()
        {
        }

        protected override void OnTouchAction(object? sender, TouchActionEventArgs e)
        {
            if (PaintTool == null)
            {
                return;
            }

            PaintTool.TouchActionUpdate(e);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            if (PaintTool == null)
            {
                return;
            }

            PaintTool.Draw(e.Surface.Canvas);
        }
    }
}
