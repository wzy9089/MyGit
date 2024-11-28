using SkiaSharp;

namespace Koga.Paint;

public partial class PaintControl : ContentView
{

    public StrokeTypes CurrentStrokeTool { get => paintView.CurrentStrokeTool; set => paintView.CurrentStrokeTool = value; }

	public PaintControl()
	{
		InitializeComponent();
	}

    private void backgroundLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		var canvas = e.Surface.Canvas;
        canvas.Clear(new SKColor(40, 55, 50));
    }

    private async void paintView_StrokeCreated(object sender, StrokeCreatedEventArgs e)
    {
        await pageView.AddStroke(e.Stroke);
        paintView.InvalidateSurface();
    }

    public void Clear()
    {
        pageView.ClearStrokes();
    }
}