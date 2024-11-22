using Koga.Paint.Recognizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public abstract class PaintTool
    {
        internal TouchPaintingView OwnerView { get; private set; }

        internal PaintTool(TouchPaintingView ownerView)
        {
            OwnerView = ownerView;
        }

        internal abstract void TouchActionUpdate(TouchActionEventArgs args);
    }
}
