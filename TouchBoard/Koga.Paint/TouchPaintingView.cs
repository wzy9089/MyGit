using Koga.Paint.Recognizer;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class TouchPaintingView: TouchCanvasView
    {
        protected new event EventHandler<TouchActionEventArgs>? TouchAction;

        internal event EventHandler<StrokeCreatedEventArgs>? StrokeCreated;

        private Dictionary<uint, Stroke> _Strokes = new Dictionary<uint, Stroke>();

        private bool _NeedClear = true;

        public TouchPaintingView()
        {
            //this.PaintSurface += TouchPaintingView_PaintSurface;
        }

        protected override async void OnTouchAction(object? sender, TouchActionEventArgs e)
        {
            switch(e.ActionType)
            {
                case TouchActionTypes.Pressed:
                    if(!_Strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        _Strokes.Add(e.Pointer.PointerId, new ChalkStroke(SKColors.White, 8));
                        _Strokes[e.Pointer.PointerId].StrokeStart(ToDipPoint(e.Pointer.Position));
                    }
                    break;
                case TouchActionTypes.Moved:
                    if (_Strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        var pointer = e.HistoricalPointers.First();
                        _Strokes[e.Pointer.PointerId].StrokeAdd(ToDipPoint(pointer.Position), true);
                        for (int i=1;i<e.HistoricalPointers.Count;i++)
                        {
                            pointer = e.HistoricalPointers[i];
                            _Strokes[e.Pointer.PointerId].StrokeAdd(ToDipPoint(pointer.Position), false);
                        }

                        _NeedClear = false;
                        InvalidateSurface();
                    }
                    break;

                case TouchActionTypes.Cancelled:
                case TouchActionTypes.Released:
                    if(_Strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        var stroke = _Strokes[e.Pointer.PointerId];
                        stroke.StrokeEnd();

                        OnStrokeCreated(new StrokeCreatedEventArgs(stroke));

                        _Strokes.Remove(e.Pointer.PointerId);
                        _NeedClear = true;
                        InvalidateSurface();
                    }
                    break;
            }
        }

        private void OnStrokeCreated(StrokeCreatedEventArgs strokeCreatedEventArgs)
        {
            StrokeCreated?.Invoke(this, strokeCreatedEventArgs);
        }

        SKPoint ToDipPoint(Point point)
        {
            float x = (float)point.X;
            float y = (float)point.Y;

#if WINDOWS
            float scale = (float)DeviceDisplay.MainDisplayInfo.Density;
            x = x * scale;
            y = y * scale;
#endif

            return new SKPoint(x, y);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            if (_NeedClear)
            {
                e.Surface.Canvas.Clear();

                foreach (var stroke in _Strokes.Values)
                {
                    stroke.Draw(e.Surface.Canvas, false);
                }
            }
            else
            {
                foreach (var stroke in _Strokes.Values)
                {
                    stroke.Draw(e.Surface.Canvas, true);
                }
            }
        }
    }
}
