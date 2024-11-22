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

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }

        private void btnClear_Clicked(object sender, EventArgs e)
        {
            paintControl.Clear();
        }
    }

}
