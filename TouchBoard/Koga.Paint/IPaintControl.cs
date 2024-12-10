using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public interface IPaintControl
    {
        public Painting Painting { get;set; }
        internal void UpdateRealtimeView();
        internal void UpdatePaintingView();
    }
}
