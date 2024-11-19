using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Microsoft.Maui.Platform;
using SkiaSharp.Views.Maui.Controls;
using static Android.Views.MotionEvent;
using View = Android.Views.View;

namespace Koga.Paint.Recognizer
{
    public partial class TouchRecognizer
    {
        View? _view;
        Func<double, double> fromPixels;
        int[] twoIntArray = new int[2];
        private Point _oldscreenPointerCoords;

        public partial void Initialize(SKCanvasView view)
        {
            _view = view.Handler?.PlatformView as View;
            if (_view != null)
            {
                fromPixels = _view.Context.FromPixels;
                _view.Touch += OnTouch;
            }
        }
        private void OnTouch(object? sender, View.TouchEventArgs e)
        {
            var mve = e.Event;
            if(mve.Source != InputSourceType.Touchscreen)
            {
                return;
            }

            var senderView = sender as Android.Views.View;
            var pointerIndex = mve.ActionIndex;
            var id = mve.GetPointerId(pointerIndex);
            //senderView.GetLocationOnScreen(twoIntArray);
            //var screenPointerCoords = new Point(twoIntArray[0] + mve.GetX(pointerIndex),
            //                                      twoIntArray[1] + mve.GetY(pointerIndex));
            var screenPointerCoords = new Point(mve.GetX(pointerIndex),
                                                  mve.GetY(pointerIndex));
            var touchSize = new Size(mve.GetTouchMajor(pointerIndex), mve.GetTouchMinor(pointerIndex));
            var size = mve.GetSize(pointerIndex);

            switch (mve?.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    if(senderView != null)
                        senderView.RequestPointerCapture();

                    InvokeTouchActionEvent(id, TouchActionTypes.Pressed, screenPointerCoords, touchSize, size, true);
                    break;

                case MotionEventActions.Move:
                    var hisCount = mve.HistorySize;
                    var ptCount = mve.PointerCount;

                    if (hisCount > 0)
                    {
                        for (pointerIndex = 0; pointerIndex < ptCount; pointerIndex++)
                        {
                            id = mve.GetPointerId(pointerIndex);
                            List<TouchPointer> hisPoints = new List<TouchPointer>();
                            for (var pos = 0; pos < hisCount; pos++)
                            {
                                screenPointerCoords = new Point(mve.GetHistoricalX(pointerIndex, pos),
                                                                  mve.GetHistoricalY(pointerIndex, pos));
                                touchSize = new Size(mve.GetHistoricalTouchMajor(pointerIndex, pos), mve.GetHistoricalTouchMinor(pointerIndex, pos));
                                hisPoints.Add(new TouchPointer
                                {
                                    PointerId = (uint)id,
                                    Position = screenPointerCoords,
                                    Size = touchSize
                                });
                            }

                            screenPointerCoords = new Point(mve.GetX(pointerIndex),
                                                              mve.GetY(pointerIndex));
                            touchSize = new Size(mve.GetTouchMajor(pointerIndex), mve.GetTouchMinor(pointerIndex));
                            hisPoints.Add(new TouchPointer
                            {
                                PointerId = (uint)id,
                                Position = screenPointerCoords,
                                Size = touchSize
                            });

                            TouchAction?.Invoke(this, new TouchActionEventArgs(TouchActionTypes.Moved, hisPoints));
                        }
                    }
                    else
                    {
                        for (pointerIndex = 0; pointerIndex < mve.PointerCount; pointerIndex++)
                        {
                            id = mve.GetPointerId(pointerIndex);
                            //senderView.GetLocationOnScreen(twoIntArray);

                            //screenPointerCoords = new Point(twoIntArray[0] + mve.GetX(pointerIndex),
                            //                                twoIntArray[1] + mve.GetY(pointerIndex));
                            screenPointerCoords = new Point(mve.GetX(pointerIndex),
                                                                  mve.GetY(pointerIndex));

                            touchSize = new Size(mve.GetTouchMajor(pointerIndex), mve.GetTouchMinor(pointerIndex));

                            if (_oldscreenPointerCoords == default
                                                        || screenPointerCoords != _oldscreenPointerCoords)
                            {
                                _oldscreenPointerCoords = screenPointerCoords;
                                InvokeTouchActionEvent(id, TouchActionTypes.Moved, screenPointerCoords, touchSize, size, true);
                            }
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    InvokeTouchActionEvent(id, TouchActionTypes.Released, screenPointerCoords, touchSize, size, false);

                    if(senderView != null)
                        senderView.ReleasePointerCapture();
                    break;
                case MotionEventActions.Cancel:
                    InvokeTouchActionEvent(id, TouchActionTypes.Cancelled, screenPointerCoords, touchSize, size, false);

                    if (senderView != null)
                        senderView.ReleasePointerCapture();
                    break;
            }

            e.Handled=true;
        }

        void InvokeTouchActionEvent(int id, TouchActionTypes actionType, Point pointerLocation,Size pointerSize, float size, bool isInContact)
        {
            //_view.GetLocationOnScreen(twoIntArray);
            //double x = pointerLocation.X - twoIntArray[0];
            //double y = pointerLocation.Y - twoIntArray[1];
            //Point point = new Point(fromPixels(x), fromPixels(y));
            TouchPointer touchPointer = new TouchPointer
            {
                PointerId = (uint)id,
                Position = pointerLocation,
                Size = pointerSize
            };

            Debug.WriteLine($"{id},{actionType},{pointerLocation},{pointerSize},{size}");
            TouchAction?.Invoke(this, new TouchActionEventArgs(actionType,touchPointer));
        }


        public partial void Dispose()
        { 
            if(_view != null)
                _view.Touch -= OnTouch;
        }
    }
}
