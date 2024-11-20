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
using RadioButton = Microsoft.Maui.Controls.RadioButton;
using DrawLib;

namespace SketchpadNew
{
    public partial class MainPage : ContentPage
    {
        Dictionary<long,KTStroke> inkStrokes = new Dictionary<long, KTStroke>();
        List<KTStroke> completedStrokes = new List<KTStroke>();

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

        SKPaint fill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.LightSlateGray,
            //StrokeWidth = 3,
            //StrokeCap = SKStrokeCap.Round,
            //StrokeJoin = SKStrokeJoin.Round,
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

        SKPaint inprogressFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Red,
            //StrokeWidth = 3,
            //StrokeCap = SKStrokeCap.Round,
            //StrokeJoin = SKStrokeJoin.Round,
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
            canvas.Scale((float)DeviceDisplay.MainDisplayInfo.Density);

            foreach (SKPath path in inProgressPaths.Values)
            {
                if(isErasing)
                {
                    canvas.DrawPath(path, erasePaing);
                }
                else
                {
                    canvas.DrawPath(path, inprogressFill);
                }
            }

            canvas.DrawText($"Max:{_MaxLength},Min:{_MinLength}", new SKPoint(10, 10), paint);

            //Debug.WriteLine("Painting Layer Painted");
        }

        private void presentLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            canvas.Scale((float)DeviceDisplay.Current.MainDisplayInfo.Density);
            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, fill);
            }

            //Debug.WriteLine("Present Layer Painted");
        }

        private void bcakgroundLayer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            //canvas.Clear(backgroundColor);
            canvas.Clear();
            //Debug.WriteLine("Background Layer Painted");
        }

        private void OnClear(object sender, EventArgs e)
        {
            var menu = sender as MenuItem;
            if(menu.Text == "Clear")
            {
                completedPaths.Clear();
                completedStrokes.Clear();
                presentLayer.InvalidateSurface();
            }
            else if (menu.Text == "Clear All")
            {
                foreach (var id in inProgressPaths.Keys)
                {
                    Debug.WriteLine($"Clearing:{id}");
                }
                inkStrokes.Clear();
                completedStrokes.Clear();
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

        float _MaxLength = float.MinValue;
        float _MinLength = float.MaxValue;
        private void paintingLayer_TouchAction(object sender, Recognizer.TouchActionEventArgs e)
        {
            var pt = e.Pointer;
            var pts = e.HistoricalPointers;
            switch (e.Type)
            {
                case Recognizer.TouchActionTypes.Pressed:
                    if(!inkStrokes.ContainsKey(pt.PointerId))
                    {
                        inkStrokes[pt.PointerId] = new KTStroke();
                        inkStrokes[pt.PointerId].Add(new KTStrokePoint(pt.Position, (float)pt.Size.Width));
                    }

                    //if (!inProgressPaths.ContainsKey(pt.PointerId))
                    //{
                    //    inkStrokes[pt.PointerId] = new KTStroke();
                    //    inkStrokes[pt.PointerId].Add(new KTStrokePoint(pt.Position, (float)pt.Size.Width));
                    //    inProgressPaths[pt.PointerId] = new SKPath();
                    //    inProgressPaths[pt.PointerId].MoveTo(new SKPoint((float)pt.Position.X, (float)pt.Position.Y));
                    //}

                    break;
                case Recognizer.TouchActionTypes.Moved:
                    if (inkStrokes.ContainsKey(pt.PointerId))
                    {
                        var stroke = inkStrokes[pt.PointerId];
                        for (int i= 0; i<pts.Count;i++)
                        {
                            var vpt = pts[i];
                            if (vpt.Position != stroke.Last().Position)
                            {
                                SKPoint v = new SKPoint((float)(vpt.Position.X - stroke.Last().Position.X), (float)(vpt.Position.Y - stroke.Last().Position.Y));
                                
                                stroke.Add(new KTStrokePoint(vpt.Position, (float)vpt.Size.Width));

                                if(v.Length > _MaxLength)
                                {
                                    _MaxLength = v.Length;
                                }

                                if(v.Length < _MinLength)
                                {
                                    _MinLength = v.Length;
                                }
                            }
                        }

                        if(stroke.Count>2)
                        {
                            //inProgressPaths[pt.PointerId] = StrokeBuilder.CreateHandwriteStroke(stroke.ToSKPointList(), (float)widthSlider.Value, 1, _StrokeType);
                            inProgressPaths[pt.PointerId] = StrokeBuilder.CreateSoftbrushStroke(stroke.ToSKPointList(), (float)widthSlider.Value, 1);
                            Debug.WriteLine("Path Count:" + inProgressPaths[pt.PointerId].PointCount);
                        }
                    }

                    paintingLayer.InvalidateSurface();

                    break;

                case Recognizer.TouchActionTypes.Exited:
                case Recognizer.TouchActionTypes.Released:
                    if (inkStrokes.ContainsKey(pt.PointerId))
                    {
                        var stroke = inkStrokes[pt.PointerId];
                        completedStrokes.Add(stroke);
                        if(stroke.Count>2)
                        {
                            //completedPaths.Add(StrokeBuilder.CreateHandwriteStroke(stroke.ToSKPointList(), (float)widthSlider.Value, 1, _StrokeType));
                            completedPaths.Add(StrokeBuilder.CreateSoftbrushStroke(stroke.ToSKPointList(), (float)widthSlider.Value, 1));
                        }
                        //completedPaths.Add(inProgressPaths[pt.PointerId]);
                        inProgressPaths.Remove(pt.PointerId);
                        inkStrokes.Remove(pt.PointerId);
                        paintingLayer.InvalidateSurface();
                        presentLayer.InvalidateSurface();
                    }

                    break;

            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        int _StrokeType = 0;
        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio.IsChecked)
            {
                string? content = radio?.Content as string;

                if (content=="效果1")
                {
                    _StrokeType = 0;
                }
                else if (content == "效果2")
                {
                    _StrokeType = 1;
                }
                else if (content == "效果3")
                {
                    _StrokeType = 2;
                }
                else if (content == "效果4")
                {
                    _StrokeType = 3;
                }

                presentLayer.InvalidateSurface();
            }
        }
    }

}
