using SkiaSharp;
using System.Diagnostics;

namespace TouchBoard
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.Green);
        }

        private void canvasView_TouchAction(object sender, Koga.Paint.Recognizer.TouchActionEventArgs e)
        {
            
        }

        private void glview_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs e)
        {
            var canvas= e.Surface.Canvas;
            canvas.Clear(SKColors.DarkGray);
        }
    }

}
