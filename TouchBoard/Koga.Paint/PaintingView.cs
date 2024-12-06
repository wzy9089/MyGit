using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class PaintingView:SKCanvasView
    {
        List<Stroke> _StrokeList = new List<Stroke>();

        bool _NeedClear = true;

        public async Task AddStroke(Stroke stroke)
        {
            lock (_StrokeList)
            {
                _StrokeList.Add(stroke);
            }

            _Picture = await BuildPicture();

            InvalidateSurface();
        }

        public void ClearStrokes()
        {
            _StrokeList.Clear();
            _Picture = null;
            InvalidateSurface();
        }

        SKPicture? _Picture;

        async Task<SKPicture> BuildPicture()
        {
            return await Task.Run(() =>
            {
                SKPictureRecorder recorder = new SKPictureRecorder();
                SKCanvas recordCanvas = recorder.BeginRecording(new SKRect(0, 0, CanvasSize.Width, CanvasSize.Height));
                recordCanvas.Clear();

                lock (_StrokeList)
                {
                    foreach (var stroke in _StrokeList)
                    {
                        stroke.Draw(recordCanvas, false);
                    }
                }

                return recorder.EndRecording();
            });
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (_Picture != null)
            {
                canvas.DrawPicture(_Picture);
            }
            else
            {
                canvas.Clear();
            }

            //if (_NeedClear)
            //{
            //    canvas.Clear();

            //    foreach (var stroke in _StrokeList)
            //    {
            //        stroke.Draw(canvas, false);
            //    }
            //}
            //else
            //{
            //    _StrokeList.Last().Draw(canvas, false);

            //    _NeedClear = true;
            //}
        }
    }
}
