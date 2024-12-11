using Koga.Paint.Recognizer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public abstract class PaintTool
    {
        public abstract string ToolID { get; }

        internal event EventHandler<PaintToolEventArgs>? PaintStarted;
        internal event EventHandler<PaintToolEventArgs>? PaintFinished;

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        internal IPaintControl Owner { get; private set; }

        internal PaintTool(IPaintControl owner)
        {
            Owner = owner;
        }

        internal virtual void OnPaintStarted(PaintToolEventArgs args)
        {
            PaintStarted?.Invoke(this, args);
        }

        internal virtual void OnPaintFinished(PaintToolEventArgs args)
        {
            PaintFinished?.Invoke(this, args);
        }

        internal abstract void TouchActionUpdate(TouchActionEventArgs args);

        internal abstract void Draw(SKCanvas canvas);

        internal static SKPoint ToDipPoint(Point point)
        {
            float x = (float)point.X;
            float y = (float)point.Y;

#if WINDOWS
            float scale = (float)DeviceDisplay.MainDisplayInfo.Density;
            x = x * scale;
            y = y * scale;
#endif

            return new SKPoint(x, y);
        }

    }
}
