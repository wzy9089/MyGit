using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal interface IPaintControl
    {
        public Painting Painting { get;set; }
        public void UpdateRealtimeView();
        public void UpdatePaintingView();

        public void OnPaintStarted(object? sender, PaintToolEventArgs args);

        public void OnPaintFinished(object? sender, PaintToolEventArgs args);
    }
}
