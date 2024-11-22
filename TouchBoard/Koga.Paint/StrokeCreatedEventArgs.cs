using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class StrokeCreatedEventArgs:EventArgs
    {
        public Stroke Stroke { get; private set; }
        public StrokeCreatedEventArgs(Stroke stroke)
        {
            Stroke = stroke;
        }
    }
}
