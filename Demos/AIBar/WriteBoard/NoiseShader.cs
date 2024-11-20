using ComputeSharp;
using ComputeSharp.D2D1;

[D2DInputCount(0)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader40)]
[AutoConstructor]
public readonly partial struct NoiseShader : ID2D1PixelShader
{
    private readonly float _amount;

    /// <inheritdoc/>
    public float4 Execute()
    {
        // Get the current pixel coordinate (in pixels)
        int2 position = (int2)D2D.GetScenePosition().XY;

        // Compute a random value in the [0, 1] range for each target pixel. This line just
        // calculates a hash from the current position and maps it into the [0, 1] range.
        // This effectively provides a "random looking" value for each pixel.
        float hash = Hlsl.Frac(Hlsl.Sin(Hlsl.Dot(position, new float2(41, 289))) * 45758.5453f);

        // Map the random value in the [0, amount] range, to control the strength of the noise
        float alpha = Hlsl.Lerp(0, _amount, hash);

        // Return a white pixel with the random value modulating the opacity
        return new(1, 1, 1, alpha);
    }
}