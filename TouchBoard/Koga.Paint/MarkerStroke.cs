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
        SKPaint _StrokePaint = new SKPaint()
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

        public MarkerStroke(List<SKPoint> points, SKColor color, float width) : base(StrokeTypes.Marker, points, color, width)
        {
        }

        internal override void Draw(SKCanvas canvas, bool lastSegmentOnly)
        {
            _StrokePaint.Color = Color;
            _StrokePaint.StrokeWidth = Width;

            if (lastSegmentOnly)
            {
                _StrokePaint.IsAntialias = false;
                canvas.DrawPath(LastPathSegment, _StrokePaint);
            }
            else
            {
                canvas.DrawPath(Path, _StrokePaint);
            } 
        }
    }
}
