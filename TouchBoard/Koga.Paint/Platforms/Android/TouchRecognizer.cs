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
        //Func<double, double> fromPixels;
        int[] twoIntArray = new int[2];

        public partial void Initialize(Microsoft.Maui.Controls.View view)
        {
            _view = view.Handler?.PlatformView as View;
            if (_view != null)
            {
                //fromPixels = _view.Context.FromPixels;
                _view.Touch += OnTouch;
            }
        }
        private void OnTouch(object? sender, View.TouchEventArgs e)
        {
            var mve = e.Event;
            if(mve == null)
                return;

            var senderView = sender as Android.Views.View;
            var pointerIndex = mve.ActionIndex;
            var id = mve.GetPointerId(pointerIndex);

            var screenPointerCoords = new Point(mve.GetX(pointerIndex),
                                                  mve.GetY(pointerIndex));
            
            Size touchSize = new Size(mve.GetTouchMajor(pointerIndex), mve.GetTouchMinor(pointerIndex));

            float pressure = mve.GetPressure(pointerIndex);

            switch (mve?.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    if(senderView != null)
                        senderView.RequestPointerCapture();

                    InvokeTouchActionEvent(ToTouchDeviceType(mve.Source), id, TouchActionTypes.Pressed, screenPointerCoords, touchSize, pressure, true);
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
                                pressure = mve.GetHistoricalPressure(pointerIndex, pos);
                                hisPoints.Add(new TouchPointer
                                {
                                    PointerId = (uint)id,
                                    Position = screenPointerCoords,
                                    Size = touchSize,
                                    Pressure = pressure
                                });
                            }

                            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(mve.Source), TouchActionTypes.Moved, hisPoints));
                        }
                    }
                    else
                    {
                        for (pointerIndex = 0; pointerIndex < mve.PointerCount; pointerIndex++)
                        {
                            id = mve.GetPointerId(pointerIndex);

                            screenPointerCoords = new Point(mve.GetX(pointerIndex),
                                                                  mve.GetY(pointerIndex));

                            touchSize = new Size(mve.GetTouchMajor(pointerIndex), mve.GetTouchMinor(pointerIndex));
                            pressure = mve.GetPressure(pointerIndex);

                            TouchAction?.Invoke(this, new TouchActionEventArgs(ToTouchDeviceType(mve.Source), TouchActionTypes.Moved, new TouchPointer
                            {
                                PointerId = (uint)id,
                                Position = screenPointerCoords,
                                Size = touchSize,
                                Pressure = pressure
                            }));
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    InvokeTouchActionEvent(ToTouchDeviceType(mve.Source), id, TouchActionTypes.Released, screenPointerCoords, touchSize, pressure, false);

                    if(senderView != null)
                        senderView.ReleasePointerCapture();
                    break;
                case MotionEventActions.Cancel:
                    InvokeTouchActionEvent(ToTouchDeviceType(mve.Source), id, TouchActionTypes.Cancelled, screenPointerCoords, touchSize, pressure, false);

                    if (senderView != null)
                        senderView.ReleasePointerCapture();
                    break;
            }

            e.Handled=true;
        }

        void InvokeTouchActionEvent(TouchDeviceType deviceType, int id, TouchActionTypes actionType, Point pointerLocation,Size pointerSize, float pressure, bool isInContact)
        {
            TouchPointer touchPointer = new TouchPointer
            {
                PointerId = (uint)id,
                Position = pointerLocation,
                Size = pointerSize,
                Pressure = pressure
            };

            Debug.WriteLine($"{id},{actionType},{pointerLocation},{pointerSize},{pressure}");
            TouchAction?.Invoke(this, new TouchActionEventArgs(deviceType, actionType,touchPointer));
        }

        private TouchDeviceType ToTouchDeviceType(InputSourceType sourceType)
        {
            return sourceType switch
            {
                InputSourceType.Mouse => TouchDeviceType.Mouse,
                InputSourceType.Stylus => TouchDeviceType.Pen,
                InputSourceType.Touchscreen => TouchDeviceType.Touch,
                InputSourceType.Touchpad => TouchDeviceType.TouchPad,
                _ => TouchDeviceType.Unknown
            };
        }

        public partial void Dispose()
        { 
            if(_view != null)
                _view.Touch -= OnTouch;
        }
    }
}
