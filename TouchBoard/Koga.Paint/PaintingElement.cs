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
        internal abstract void Draw(SKCanvas canvas, bool lastSegmentOnly);
    }
}
