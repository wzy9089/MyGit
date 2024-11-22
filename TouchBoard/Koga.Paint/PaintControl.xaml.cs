using SkiaSharp;

namespace Koga.Paint;

public partial class PaintControl : ContentView
{
	public PaintControl()
	{
		InitializeComponent();
	}

    private void backgroundLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.Green);
    }

    private async void paintView_StrokeCreated(object sender, StrokeCreatedEventArgs e)
    {
        await pageView.AddStroke(e.Stroke);
    }

    public void Clear()
    {
        pageView.ClearStrokes();
    }
}