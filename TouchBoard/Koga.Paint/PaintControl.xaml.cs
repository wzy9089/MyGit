using SkiaSharp;
using Koga.Paint.Recognizer;

namespace Koga.Paint;

public partial class PaintControl : ContentView,IPaintControl
{

    //public StrokeType CurrentStrokeTool { get => paintToolView.CurrentStrokeTool; set => paintToolView.CurrentStrokeTool = value; }

    private PaintingView paintingView;
    private PaintToolView paintToolView;
    //private PaintToolManager paintToolManager;

    public PaintToolManager PaintToolManager { get; private set; }

    public Painting Painting { get => paintingView.Painting; set => paintingView.Painting = value; }


    public PaintControl()
	{
		InitializeComponent();

        paintingView = new PaintingView();
        paintToolView = new PaintToolView();

        gridLayout.Add(paintingView);
        gridLayout.Add(paintToolView);

        PaintToolManager = new PaintToolManager(this);
        PaintToolManager.CurrentToolChanged += PaintToolManager_CurrentToolChanged;

        paintToolView.PaintTool = PaintToolManager.CurrentTool;
    }

    private void PaintToolManager_CurrentToolChanged(object? sender, EventArgs e)
    {
        paintToolView.PaintTool = PaintToolManager.CurrentTool;
    }

    private void backgroundLayer_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		var canvas = e.Surface.Canvas;
        canvas.Clear(new SKColor(40, 55, 50));
    }

    public void Clear()
    {
        Painting.Clear();
    }

    public void UpdateRealtimeView()
    {
        paintToolView.InvalidateSurface();
    }

    public void UpdatePaintingView()
    {
        paintingView.InvalidateSurface();
    }

    void IPaintControl.OnPaintStarted(object? sender, PaintToolEventArgs args)
    {
    }

    void IPaintControl.OnPaintFinished(object? sender, PaintToolEventArgs args)
    {
        Painting.Add(args.NewElement);
    }
}