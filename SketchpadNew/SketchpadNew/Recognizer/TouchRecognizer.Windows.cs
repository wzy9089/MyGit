using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchpadNew.Recognizer
{
    public partial class TouchRecognizer
    {
        FrameworkElement? _view;
        public partial void Initialize(SKCanvasView view)
        {
            _view = view.Handler?.PlatformView as FrameworkElement;
            if (_view != null)
            {
                _view.PointerPressed += OnPointerPressed;
                _view.PointerMoved += OnPointerMoved;
                _view.PointerReleased += OnPointerReleased;
                _view.PointerCanceled += OnPointerCanceled;
                _view.PointerExited += OnPointerExited;
                _view.PointerEntered += OnPointerEntered;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerEntered:{e.Pointer.PointerId}");
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerExited:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(TouchActionTypes.Exited, pt));
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerCancele:{e.Pointer.PointerId}");
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerReleased:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(TouchActionTypes.Released, pt));
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerMoved:{e.Pointer.PointerId}");
            var pts = GetHistoryTouchPointers(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(TouchActionTypes.Moved, pts));
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                return;

            Debug.WriteLine($"OnPointerPressed:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(TouchActionTypes.Pressed, pt));
        }

        private TouchPointer GetTouchPointer(object sender, PointerRoutedEventArgs e)
        {
            var tpt = e.GetCurrentPoint(sender as UIElement);
            return new TouchPointer
            {
                PointerId = tpt.PointerId,
                Position = new Point(tpt.Position.X, tpt.Position.Y),
                Size = new Size(tpt.Properties.ContactRect.Width, tpt.Properties.ContactRect.Height)
            };
        }

        private List<TouchPointer> GetHistoryTouchPointers(object sender, PointerRoutedEventArgs e)
        {
            var hpts = e.GetIntermediatePoints(sender as UIElement);
            
            List<TouchPointer> historicalPointers = new List<TouchPointer>();
            foreach (var hpt in hpts)
            {
                historicalPointers.Insert(0, new TouchPointer
                {
                    PointerId = hpt.PointerId,
                    Position = new Point(hpt.Position.X, hpt.Position.Y),
                    Size = new Size(hpt.Properties.ContactRect.Width, hpt.Properties.ContactRect.Height)
                });
            }

            return historicalPointers;
        }


        public partial void Dispose()
        {
            if (_view != null)
            {
                _view.PointerPressed -= OnPointerPressed;
                _view.PointerMoved -= OnPointerMoved;
                _view.PointerReleased -= OnPointerReleased;
                _view.PointerCanceled -= OnPointerCanceled;
                _view.PointerExited -= OnPointerExited;
                _view.PointerEntered -= OnPointerEntered;

                _view = null;
            }
        }
    }
}
