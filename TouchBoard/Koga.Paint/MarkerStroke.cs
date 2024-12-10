using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class MarkerStroke : Stroke
    {
        SKPaint strokePaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            //IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Solid, 1),
        };

        public MarkerStroke() : this(new List<SKPoint>(), SKColors.Red, 7)
        {
        }

        public MarkerStroke(SKColor color, float width) : this(new List<SKPoint>(), color, width)
        {
        }

        public MarkerStroke(List<SKPoint> points, SKColor color, float width) : base(StrokeType.Marker, points, color, width)
        {
        }

        internal override void Draw(SKCanvas canvas, bool lastSegmentOnly)
        {
            strokePaint.Color = Color;
            strokePaint.StrokeWidth = Width;

            if (lastSegmentOnly)
            {
                strokePaint.IsAntialias = false;
                canvas.DrawPath(LastPathSegment, strokePaint);
            }
            else
            {
                canvas.DrawPath(Path, strokePaint);
            } 
        }
    }
}
