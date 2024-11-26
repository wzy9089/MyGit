using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class BrushStroke : Stroke
    {
        SKPaint _StrokePaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Solid, 1),
        };

        static readonly float MAX_SPEED = 40;

        public BrushStroke(SKColor color,float width) : base(StrokeTypes.Brush, new List<SKPoint>(), color, width)
        {
        }

        public BrushStroke(List<SKPoint> points, SKColor color, float width) : base(StrokeTypes.Brush, points, color, width)
        {
        }

        internal override void BuildPath(List<SKPoint> points, SKPath path)
        {
            path.Reset();
            path.AddPath(StrokeBuilder.CreateBrushStroke(points, Width, MAX_SPEED));
        }

        float _LastStrokeWidth = 0.5f;
        internal override void StrokeStart(SKPoint point)
        {
            if (Points.Count > 0)
                Points.Clear();

            Points.Add(point);
            Path.Reset();
            LastPathSegment.Reset();

            Path.AddCircle(point.X, point.Y, 0.5f, SKPathDirection.Clockwise);
            LastPathSegment.AddCircle(point.X, point.Y, 0.5f, SKPathDirection.Clockwise);
            _LastStrokeWidth = 0.5f;
        }

        internal override void StrokeAdd(SKPoint point, bool isNewSegment)
        {
            if (point.Equals(Points.Last()))
                return;

            Points.Add(point);

            if (isNewSegment)
            {
                _LastPathSegment.Reset();
            }

            SKPoint p0 = Points[Points.Count - 2];
            SKPoint p1 = Points[Points.Count - 1];

            float w = StrokeBuilder.ComputeStrokeWidthBySpeed(p0, p1, Width, MAX_SPEED);

            StrokeBuilder.MakeBrushStrokeSegment(p0, _LastStrokeWidth, p1, w, _Path);
            StrokeBuilder.MakeBrushStrokeSegment(p0, _LastStrokeWidth, p1, w, _LastPathSegment);

            _LastStrokeWidth = w;
        }

        internal override void StrokeEnd()
        {
            if(Points.Count==0) return; 

            BuildPath(Points, Path);
        }

        internal override void Draw(SKCanvas canvas, bool lastSegmentOnly)
        {
            _StrokePaint.Color = Color;
            
            if (lastSegmentOnly)
            {
                canvas.DrawPath(LastPathSegment, _StrokePaint);
            }
            else
            {
                canvas.DrawPath(Path, _StrokePaint);
            }
        }
    }
}
