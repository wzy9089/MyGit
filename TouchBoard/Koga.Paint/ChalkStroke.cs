using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class ChalkStroke:Stroke
    {
        SKPaint _StrokePaint;
        SKPaint _StrokePaintFast;

        static Dictionary<SKColor, SKShader> ShakerDict = new Dictionary<SKColor, SKShader>();

        public ChalkStroke():this(new List<SKPoint>(), SKColors.White, 10)
        {
        }

        public ChalkStroke(SKColor color, float width) : this(new List<SKPoint>(), color, width)
        {
        }

        public ChalkStroke(List<SKPoint> points, SKColor color, float width) : base(StrokeTypes.Chalk, points, color, width)
        {
            _StrokePaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Square,
                StrokeWidth = width,
                //BlendMode = SKBlendMode.Overlay,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1),
            };

            _StrokePaint.Shader = GetChalkTextureShader(color);

            _StrokePaintFast = _StrokePaint.Clone();
            //_StrokePaintFast.MaskFilter = null;
            _StrokePaintFast.BlendMode = SKBlendMode.Src;
        }
        internal override void Draw(SKCanvas canvas, bool lastSegmentOnly)
        {

            if (lastSegmentOnly)
            {
                _StrokePaintFast.StrokeWidth = Width;
                canvas.DrawPath(LastPathSegment, _StrokePaintFast);
            }
            else
            {
                _StrokePaint.StrokeWidth = Width;
                canvas.DrawPath(Path, _StrokePaint);
            }
        }

        private SKShader GetChalkTextureShader(SKColor color)
        {
            if (ShakerDict.ContainsKey(color))
            {
                return ShakerDict[color];
            }
            else
            {
                var shader = CreateChalkTextureShader(32, 32, color);
                ShakerDict.Add(color, shader);
                return shader;
            }
        }

        private SKShader CreateChalkTextureShader(int width, int height, SKColor color)
        {
            var bitmap = new SKBitmap(width, height);
            var random = new Random();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 随机生成点，模拟粉笔颗粒效果
                    var alpha = random.Next(120, 255);
                    //var color = new SKColor(255, 255, 255, (byte)alpha); // 白色点
                    bitmap.SetPixel(x, y, new SKColor(color.Red, color.Green, color.Blue, (byte)alpha));
                }
            }

            return SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
        }

    }
}
