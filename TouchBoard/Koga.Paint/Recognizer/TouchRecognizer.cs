using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint.Recognizer
{
    public partial class TouchRecognizer:IDisposable
    {
        public event EventHandler<TouchActionEventArgs>? TouchAction;

        public TouchRecognizer(SKCanvasView view)
        {
            Initialize(view);
        }

        public partial void Initialize(SKCanvasView view);

        public partial void Dispose();
    }
}
