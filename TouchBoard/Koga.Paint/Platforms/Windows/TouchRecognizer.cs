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

namespace Koga.Paint.Recognizer
{
    public partial class TouchRecognizer
    {
        FrameworkElement? view;
        public partial void Initialize(Microsoft.Maui.Controls.View view)
        {
            this.view = view.Handler?.PlatformView as FrameworkElement;
            if (this.view != null)
            {
                this.view.PointerPressed += OnPointerPressed;
                this.view.PointerMoved += OnPointerMoved;
                this.view.PointerReleased += OnPointerReleased;
                this.view.PointerCanceled += OnPointerCanceled;
                this.view.PointerExited += OnPointerExited;
                this.view.PointerEntered += OnPointerEntered;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine($"OnPointerEntered:{e.Pointer.PointerId}");

            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Entered, pt));
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine($"OnPointerExited:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Exited, pt));
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine($"OnPointerCancele:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Cancelled, pt));

            if (view != null)
            {
                view.ReleasePointerCapture(e.Pointer);
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Debug.WriteLine($"OnPointerReleased:{e.Pointer.PointerId}");
            var pt = GetTouchPointer(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Released, pt));

            if(view != null)
            {
                view.ReleasePointerCapture(e.Pointer);
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //Debug.WriteLine($"OnPointerMoved:{e.Pointer.PointerId}");
            var pts = GetHistoryTouchPointers(sender, e);
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Moved, pts));
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(view!=null)
            {
                view.CapturePointer(e.Pointer);
            }

            var pt = GetTouchPointer(sender, e);
            Debug.WriteLine($"OnPointerPressed:{e.Pointer.PointerId},{pt.Position}");
            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(e.Pointer.PointerDeviceType), TouchActionTypes.Pressed, pt));
        }

        private TouchDeviceType ToTouchDeviceType(PointerDeviceType deviceType)
        {
            return deviceType switch
            {
                PointerDeviceType.Mouse => TouchDeviceType.Mouse,
                PointerDeviceType.Pen => TouchDeviceType.Pen,
                PointerDeviceType.Touch => TouchDeviceType.Touch,
                PointerDeviceType.Touchpad => TouchDeviceType.TouchPad,
                _ => TouchDeviceType.Unknown
            };
        }

        private TouchPointer GetTouchPointer(object sender, PointerRoutedEventArgs e)
        {
            var tpt = e.GetCurrentPoint(sender as UIElement);
            return new TouchPointer
            {
                PointerId = tpt.PointerId,
                Position = new Point(tpt.Position.X, tpt.Position.Y),
                Size = new Size(tpt.Properties.ContactRect.Width, tpt.Properties.ContactRect.Height),
                Pressure = tpt.Properties.Pressure
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
                    Size = new Size(hpt.Properties.ContactRect.Width, hpt.Properties.ContactRect.Height),
                    Pressure = hpt.Properties.Pressure
                });
            }

            return historicalPointers;
        }


        public partial void Dispose()
        {
            if (view != null)
            {
                view.PointerPressed -= OnPointerPressed;
                view.PointerMoved -= OnPointerMoved;
                view.PointerReleased -= OnPointerReleased;
                view.PointerCanceled -= OnPointerCanceled;
                view.PointerExited -= OnPointerExited;
                view.PointerEntered -= OnPointerEntered;

                view = null;
            }
        }
    }
}
