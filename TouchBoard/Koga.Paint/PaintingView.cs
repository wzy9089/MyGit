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
        SKPicture? prePicture;

        internal Painting Painting
        {
            get;
            set
            {
                if (field != value)
                {
                    if (field != null)
                    {
                        field.PaintingChanged -= Painting_PaintingChanged;
                    }

                    field = value;
                    field.PaintingChanged += Painting_PaintingChanged;

                    InvalidateSurface();
                }
            }
        }

        public PaintingView()
        {
            Painting = new Painting();
        }

        private async void Painting_PaintingChanged(object? sender, PaintintChangedEventArgs e)
        {
            prePicture = await BuildPicture();
            InvalidateSurface();
        }

        async Task<SKPicture> BuildPicture()
        {
            return await Task.Run(() =>
            {
                SKPictureRecorder recorder = new SKPictureRecorder();
                SKCanvas recordCanvas = recorder.BeginRecording(new SKRect(0, 0, CanvasSize.Width, CanvasSize.Height));
                recordCanvas.Clear();

                lock (Painting)
                {
                    foreach (var element in Painting)
                    {
                        element.Draw(recordCanvas, false);
                    }
                }

                return recorder.EndRecording();
            });
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (prePicture != null)
            {
                canvas.DrawPicture(prePicture);
            }
            else
            {
                canvas.Clear();
            }
        }
    }
}
