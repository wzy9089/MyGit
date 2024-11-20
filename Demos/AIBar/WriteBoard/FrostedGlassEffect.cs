using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

public sealed class FrostedGlassEffect : CanvasEffect
{
    private static readonly EffectNode<GaussianBlurEffect> BlurNode = new();
    private static readonly EffectNode<PixelShaderEffect<NoiseShader>> NoiseNode = new();

    private ICanvasImage? _source;
    private double _blurAmount;
    private double _noiseAmount;

    public ICanvasImage? Source
    {
        get => _source;
        set => SetAndInvalidateEffectGraph(ref _source, value);
    }

    public double BlurAmount
    {
        get => _blurAmount;
        set => SetAndInvalidateEffectGraph(ref _blurAmount, value);
    }

    public double NoiseAmount
    {
        get => _noiseAmount;
        set => SetAndInvalidateEffectGraph(ref _noiseAmount, value);
    }

    /// <inheritdoc/>
    protected override void BuildEffectGraph(EffectGraph effectGraph)
    {
        // Create the effect graph as follows:
        //
        // ┌────────┐   ┌──────┐
        // │ source ├──►│ blur ├─────┐
        // └────────┘   └──────┘     ▼
        //                       ┌───────┐   ┌────────┐
        //                       │ blend ├──►│ output │
        //                       └───────┘   └────────┘
        //    ┌───────┐              ▲   
        //    │ noise ├──────────────┘
        //    └───────┘
        //
        GaussianBlurEffect gaussianBlurEffect = new();
        BlendEffect blendEffect = new() { Mode = BlendEffectMode.Overlay };
        PixelShaderEffect<NoiseShader> noiseEffect = new();
        PremultiplyEffect premultiplyEffect = new();

        // Connect the effect graph
        premultiplyEffect.Source = noiseEffect;
        blendEffect.Background = gaussianBlurEffect;
        blendEffect.Foreground = premultiplyEffect;

        // Register all effects. For those that need to be referenced later (ie. the ones with
        // properties that can change), we use a node as a key, so we can perform lookup on
        // them later. For others, we register them anonymously. This allows the effect
        // to autommatically and correctly handle disposal for all effects in the graph.
        effectGraph.RegisterNode(BlurNode, gaussianBlurEffect);
        effectGraph.RegisterNode(NoiseNode, noiseEffect);
        effectGraph.RegisterNode(premultiplyEffect);
        effectGraph.RegisterOutputNode(blendEffect);
    }

    /// <inheritdoc/>
    protected override void ConfigureEffectGraph(EffectGraph effectGraph)
    {
        // Set the effect source
        effectGraph.GetNode(BlurNode).Source = Source;

        // Configure the blur amount
        effectGraph.GetNode(BlurNode).BlurAmount = (float)BlurAmount;

        // Set the constant buffer of the shader
        effectGraph.GetNode(NoiseNode).ConstantBuffer = new NoiseShader((float)NoiseAmount);
    }
}