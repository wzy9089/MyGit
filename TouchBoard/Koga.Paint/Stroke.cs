using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public abstract class Stroke: PaintingElement
    {
        public StrokeTypes StrokeType { get; private set; }
        public List<SKPoint> Points { get; private set; }
        public SKColor Color { get; set; }
        public float Width { get; set; }

        protected SKPath _Path = new SKPath();
        protected virtual SKPath Path
        {
            get
            {
                return _Path;
            }
        }

        protected SKPath _LastPathSegment = new SKPath();
        protected virtual SKPath LastPathSegment
        {
            get
            {
                return _LastPathSegment;
            }
        }

        protected Stroke(StrokeTypes strokeType, List<SKPoint> points, SKColor color, float width)
        {
            StrokeType = strokeType;
            Color = color;
            Width = width;
            Points = points;

            if(Points.Count > 0)
            {
                BuildPath(Points, Path);
            }
        }

        internal virtual void BuildPath(List<SKPoint> points, SKPath path)
        {
            path.Reset();

            if (points.Count == 0) return;

            if (points.Count == 1)
            {
                path.AddCircle(points[0].X, points[0].Y, Width / 2, SKPathDirection.Clockwise);
            }
            else if (points.Count == 2)
            {
                path.MoveTo(points[0]);
                path.LineTo(points[1]);
            }
            else
            {
                var controls = StrokeBuilder.CalculateCubicControls(points, StrokeBuilder.K);

                if (controls != null)
                {
                    path.MoveTo(controls[0]);

                    for (int i = 1; i < controls.Count; i += 3)
                    {
                        path.CubicTo(controls[i], controls[i + 1], controls[i + 2]);
                    }
                }
            }
        }

        internal virtual void StrokeStart(SKPoint point)
        {
            if(Points.Count > 0) 
                Points.Clear();

            Points.Add(point);
            Path.Reset();
            LastPathSegment.Reset();
        }

        SKPoint _LastCubicControl;
        internal virtual void StrokeAdd(SKPoint point, bool isNewSegment)
        {
            if(point.Equals(Points.Last()))
                return;

            Points.Add(point);

            if(isNewSegment)
            {
                _LastPathSegment.Reset();
            }

            if (Points.Count == 3)
            {
                var ret = StrokeBuilder.CalculateCubicControl(Points[0], Points[1], Points[2], StrokeBuilder.K);

                _Path.MoveTo(Points[0]);
                _Path.CubicTo(Points[0], ret.p1, ret.p2);
                _LastCubicControl = ret.p3;

                _LastPathSegment.MoveTo(Points[0]);
                _LastPathSegment.CubicTo(Points[0], ret.p1, ret.p2);
            }
            else if (Points.Count > 3)
            {
                var ret = StrokeBuilder.CalculateCubicControl(Points[Points.Count - 3], Points[Points.Count - 2], Points[Points.Count - 1], StrokeBuilder.K);
 //               _Path.MoveTo(Points[Points.Count - 3]);
                _Path.CubicTo(_LastCubicControl, ret.p1, ret.p2);
                _LastCubicControl = ret.p3;

                _LastPathSegment.MoveTo(Points[Points.Count - 3]);
                _LastPathSegment.CubicTo(_LastCubicControl, ret.p1, ret.p2);
            }
        }

        internal virtual void StrokeEnd()
        {
            if(Points.Count == 0) return;

            if(Points.Count == 1)
            {
                _Path.AddCircle(Points[0].X, Points[0].Y, Width / 2, SKPathDirection.Clockwise);
            }
            else if (Points.Count == 2)
            {
                _Path.MoveTo(Points[0]);
                _Path.LineTo(Points[1]);
            }
            else
            {
                //_Path.MoveTo(Points[Points.Count - 2]);
                _Path.CubicTo(_LastCubicControl, Points[Points.Count - 1], Points[Points.Count - 1]);
            }
        }

        internal virtual void ClearPoints()
        {
            Points.Clear();
            _Path.Reset();
            _LastPathSegment.Reset();
        }
    }
}
