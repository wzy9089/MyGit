using SkiaSharp;

namespace Koga.Paint;

public partial class PaintControl : ContentView
{

    public StrokeTypes CurrentStrokeTool { get => paintView.CurrentStrokeTool; set => paintView.CurrentStrokeTool = value; }
    public Painting Painting
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

                paintingView.InvalidateSurface();
            }
        }
    }

    private void Painting_PaintingChanged(object? sender, PaintintChangedEventArgs e)
    {
        paintingView.InvalidateSurface();
    }

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
        await paintingView.AddStroke(e.Stroke);
        paintView.InvalidateSurface();
    }

    public void Clear()
    {
        paintingView.ClearStrokes();
    }
}