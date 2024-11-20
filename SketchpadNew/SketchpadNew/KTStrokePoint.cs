using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchpadNew
{
    public struct KTStrokePoint
    {
        public Point Position { get;private set; }
        public float Size { get; private set; }

        public KTStrokePoint(Point position, float size)
        {
            Position = position;
            Size = size;
        }
    }
}
