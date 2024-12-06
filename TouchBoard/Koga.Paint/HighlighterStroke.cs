using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class HighlighterStroke : Stroke
    {
        SKPaint strokePaint;
        SKPath tipPath;

        public HighlighterStroke() : this(new List<SKPoint>(), SKColors.Yellow, 18)
        {
        }

        public HighlighterStroke(SKColor color, float width) : this(new List<SKPoint>(), color, width)
        {
        }

        public HighlighterStroke(List<SKPoint> points, SKColor color, float width) : base(StrokeTypes.Highlighter, points, color, width)
        {
            strokePaint = new SKPaint()
            {
                Color = color.WithAlpha((byte)(color.Alpha/2)),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Lighten,
            };

            tipPath = new SKPath();
            tipPath.AddOval(new SKRect(0, 0, width / 3, width));
            var pathEffect = SKPathEffect.Create1DPath(tipPath, 1, 0, SKPath1DPathEffectStyle.Translate);

            strokePaint.PathEffect = pathEffect;
        }

        internal override void Draw(SKCanvas canvas, bool lastSegmentOnly)
        {
            if (lastSegmentOnly)
            {
                strokePaint.IsAntialias = false;

                canvas.DrawPath(Path, strokePaint);
            }
            else
            {
                strokePaint.IsAntialias = true;
                canvas.DrawPath(Path, strokePaint);
            }
        }
    }
}
