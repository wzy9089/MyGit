#if ANDROID
using Android.Accounts;
using Android.Views;
using AndroidX.Core.View;
#endif
using Microsoft.Maui.Platform;

#if WINDOWS
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
#endif

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

        static SKColor backgroundColor = new SKColor(10, 30, 10);
        static int eraseSize = 150;

        bool isErasing = false;

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.LightSlateGray,
            StrokeWidth = 3,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        SKPaint inprogressPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 3,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        SKPaint erasePaing = new SKPaint()
        {
            Style=SKPaintStyle.Stroke,
            Color = backgroundColor,
            StrokeWidth = eraseSize,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        public MainPage()
        {
            InitializeComponent();

        }

        private void paintingLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear();

            foreach (SKPath path in inProgressPaths.Values)
            {
                if(isErasing)
                {
                    canvas.DrawPath(path, erasePaing);
                }
                else
                {
                    canvas.DrawPath(path, inprogressPaint);
                }
            }

            //Debug.WriteLine("Painting Layer Painted");
        }

        private void presentLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            //Debug.WriteLine("Present Layer Painted");
        }

        private void bcakgroundLayer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(backgroundColor);
            //Debug.WriteLine("Background Layer Painted");
        }

        private void OnClear(object sender, EventArgs e)
        {
            var menu = sender as MenuItem;
            if(menu.Text == "Clear")
            {
                completedPaths.Clear();
                presentLayer.InvalidateSurface();
            }
            else if (menu.Text == "Clear All")
            {
                foreach (var id in inProgressPaths.Keys)
                {
                    Debug.WriteLine($"Clearing:{id}");
                }
                
                inProgressPaths.Clear();
                completedPaths.Clear();
                paintingLayer.InvalidateSurface();
                presentLayer.InvalidateSurface();
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            App.Current?.CloseWindow(App.Current.MainPage.GetParentWindow());
        }

        private void paintingLayer_TouchAction(object sender, Recognizer.TouchActionEventArgs e)
        {
            var pt = e.Pointer;
            var pts = e.HistoricalPointers;
            switch (e.Type)
            {
                case Recognizer.TouchActionTypes.Pressed:
                    if (!inProgressPaths.ContainsKey(pt.PointerId))
                    {
                        inProgressPaths[pt.PointerId] = new SKPath();
                        inProgressPaths[pt.PointerId].MoveTo(new SKPoint((float)pt.Position.X, (float)pt.Position.Y));
                    }

                    break;
                case Recognizer.TouchActionTypes.Moved:
                    if (inProgressPaths.ContainsKey(pt.PointerId))
                    {
                        foreach (var vpt in pts)
                        {
                            inProgressPaths[vpt.PointerId].LineTo(new SKPoint((float)vpt.Position.X, (float)vpt.Position.Y));
                        }
                    }

                    paintingLayer.InvalidateSurface();

                    break;

                case Recognizer.TouchActionTypes.Exited:
                case Recognizer.TouchActionTypes.Released:
                    if (inProgressPaths.ContainsKey(pt.PointerId))
                    {
                        completedPaths.Add(inProgressPaths[pt.PointerId]);
                        inProgressPaths.Remove(pt.PointerId);
                        paintingLayer.InvalidateSurface();
                        presentLayer.InvalidateSurface();
                    }

                    break;

            }
        }
    }

}
