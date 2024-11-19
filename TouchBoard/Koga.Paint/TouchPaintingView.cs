using Koga.Paint.Recognizer;
using SkiaSharp.Views.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class TouchPaintingView: TouchCanvasView
    {
        protected new event EventHandler<TouchActionEventArgs>? TouchAction;

        protected override void OnTouchAction(object? sender, TouchActionEventArgs e)
        {
            
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            
        }
    }
}
