using SkiaSharp;

namespace Koga.Paint;

public partial class PaintControl : ContentView
{

    public StrokeType CurrentStrokeTool { get => editView.CurrentStrokeTool; set => editView.CurrentStrokeTool = value; }

    private PaintingView paintingView;
    private TouchPaintingView editView;

    public Painting Painting { get => paintingView.Painting; set => paintingView.Painting = value; }


    public PaintControl()
	{
		InitializeComponent();

        paintingView = new PaintingView();
        editView = new TouchPaintingView();
        editView.StrokeCreated += EditView_StrokeCreated; ;

        gridLayout.Add(paintingView);
        gridLayout.Add(editView);

    }

    private void backgroundLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		var canvas = e.Surface.Canvas;
        canvas.Clear(new SKColor(40, 55, 50));
    }

    private void EditView_StrokeCreated(object? sender, StrokeCreatedEventArgs e)
    {
        Painting.Add(e.Stroke);
    }

    public void Clear()
    {
        Painting.Clear();
    }
}