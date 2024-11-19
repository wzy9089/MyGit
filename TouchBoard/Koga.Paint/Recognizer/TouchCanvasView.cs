﻿using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint.Recognizer
{
    public class TouchCanvasView:SKCanvasView
    {
        private TouchRecognizer _recognizer;
        public event EventHandler<TouchActionEventArgs>? TouchAction;

        public TouchCanvasView()
        {
            this.HandlerChanged += TouchCanvasView_HandlerChanged;
            this.HandlerChanging += TouchCanvasView_HandlerChanging;
        }

        private void TouchCanvasView_HandlerChanging(object? sender, HandlerChangingEventArgs e)
        {
            if (e.OldHandler != null)
            {
                _recognizer.TouchAction -= OnTouchAction;
            }
        }

        private void TouchCanvasView_HandlerChanged(object? sender, EventArgs e)
        {
            var handler = this.Handler;
            if (handler != null)
            {
                _recognizer = new TouchRecognizer(this);
                _recognizer.TouchAction += OnTouchAction;
            }
        }

        protected virtual void OnTouchAction(object? sender, TouchActionEventArgs e)
        {
            TouchAction?.Invoke(this, e);
        }
    }
}
