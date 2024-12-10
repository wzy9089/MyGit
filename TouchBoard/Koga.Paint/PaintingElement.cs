using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public abstract class PaintingElement
    {
        public bool IsVisible { get; set; }

        internal bool IsSelected { get; set; }

        public abstract bool HitTest(Point point);

        internal abstract void Draw(SKCanvas canvas, bool lastSegmentOnly);
    }
}
