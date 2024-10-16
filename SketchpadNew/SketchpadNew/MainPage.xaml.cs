using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;

namespace SketchpadNew
{
    public partial class MainPage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.LightSlateGray,
            StrokeWidth = 3,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        public MainPage()
        {
            InitializeComponent();
        }

        private void canvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear();
            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }
        }

        private void paintView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(new SKColor(10, 30, 10));

            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }
        }

        private void OnClear(object sender, EventArgs e)
        {
            completedPaths.Clear();
            paintView.InvalidateSurface();
        }

        private void OnExit(object sender, EventArgs e)
        {
            App.Current?.CloseWindow(App.Current.MainPage.GetParentWindow());
        }

        private void PointerGestureRecognizer_PointerEntered(object sender, Microsoft.Maui.Controls.PointerEventArgs e)
        {
            if (e.PlatformArgs?.PointerRoutedEventArgs.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Touch)
                return;

            var ppt = e.PlatformArgs?.PointerRoutedEventArgs.GetCurrentPoint(canvasView.Handler?.PlatformView as UIElement);
            //var pts = e.PlatformArgs?.PointerRoutedEventArgs.GetIntermediatePoints(canvasView.Handler?.PlatformView as UIElement);

            if (ppt != null)
            {
                if (!inProgressPaths.ContainsKey(ppt.PointerId))
                {
                    inProgressPaths[ppt.PointerId] = new SKPath();
                    inProgressPaths[ppt.PointerId].MoveTo(new SKPoint((float)ppt.Position.X, (float)ppt.Position.Y));
                }
                //Debug.WriteLine($"Enter:{pts.Count}");
            }
        }

        private void PointerGestureRecognizer_PointerMoved(object sender, Microsoft.Maui.Controls.PointerEventArgs e)
        {
            if (e.PlatformArgs?.PointerRoutedEventArgs.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Touch)
                return;

            var ppt = e.PlatformArgs?.PointerRoutedEventArgs.GetCurrentPoint(canvasView.Handler?.PlatformView as UIElement);
            var pts = e.PlatformArgs?.PointerRoutedEventArgs.GetIntermediatePoints(canvasView.Handler?.PlatformView as UIElement);

            if (ppt != null && pts != null)
            {
                if (inProgressPaths.ContainsKey(ppt.PointerId))
                {
                    foreach (var pt in pts)
                    {
                        inProgressPaths[pt.PointerId].LineTo(new SKPoint((float)pt.Position.X, (float)pt.Position.Y));
                    }
                }
                //Debug.WriteLine($"Move:{pts.Count}");
            }

            canvasView.InvalidateSurface();
        }

        private void PointerGestureRecognizer_PointerExited(object sender, Microsoft.Maui.Controls.PointerEventArgs e)
        {
            if (e.PlatformArgs?.PointerRoutedEventArgs.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Touch)
                return;

            var pt = e.PlatformArgs?.PointerRoutedEventArgs.GetCurrentPoint(null);
            //var pts = e.PlatformArgs?.PointerRoutedEventArgs.GetIntermediatePoints(null);

            if (pt != null)
            {
                if (inProgressPaths.ContainsKey(pt.PointerId))
                {
                    completedPaths.Add(inProgressPaths[pt.PointerId]);
                    inProgressPaths.Remove(pt.PointerId);
                    paintView.InvalidateSurface();
                }

                //Debug.WriteLine($"Exit:{pts.Count}");
            }

            canvasView.InvalidateSurface();
        }
    }

}
