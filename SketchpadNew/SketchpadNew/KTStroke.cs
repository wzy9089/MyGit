using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchpadNew
{
    public class KTStroke:List<KTStrokePoint>
    {
        public List<SKPoint> ToSKPointList()
        {
            List<SKPoint> points = new List<SKPoint>();
            foreach (var point in this)
            {
                points.Add(new SKPoint((float)point.Position.X, (float)point.Position.Y));
            }
            return points;
        }
    }
}
