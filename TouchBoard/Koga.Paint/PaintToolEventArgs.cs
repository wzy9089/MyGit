using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class PaintToolEventArgs:EventArgs
    {
        public PaintingElement NewElement { get; private set; }
        public PaintToolEventArgs(PaintingElement element)
        {
            NewElement = element;
        }
        
    }
}
