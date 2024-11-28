using SkiaSharp;
using System.Diagnostics;
using SkiaSharp.Views.Maui.Controls;

#if ANDROID
using Android.Graphics;
#endif

namespace Test
{
    public partial class MainPage : ContentPage
    {
#if WINDOWS
        SKCanvasView canvasView;// = new SKCanvasView();
#elif ANDROID
        SKGLView canvasView;    // = new SKGLView();
#endif
        public MainPage()
        {
            InitializeComponent();

            Init();

        }

        private void Init()
        {
#if WINDOWS
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += CanvasView_PaintSurface;
            viewContent.Content = canvasView;
#elif ANDROID
            canvasView = new SKGLView();
            canvasView.PaintSurface += CanvasView_GLPaintSurface; ;
            viewContent.Content = canvasView;
#endif
        }

        private void CanvasView_GLPaintSurface(object? sender, SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs e)
        {
            PaintSurface(e.Surface, e.Info);
        }

        private void CanvasView_PaintSurface(object? sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface, e.Info);
        }

        private void PaintSurface(SKSurface surface, SKImageInfo info)
        {
            var canvas = surface.Canvas;
            if(_StartTest)
            {
                Random rand = new Random();

                SKBitmap bmp = CreateBitmap(info.Width, info.Height);

                Stopwatch sw = Stopwatch.StartNew();

                for(int i=0; i < 1000; i++)
                {
                    SKColor color = new SKColor((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
                    canvas.Clear(color);
                    //canvas.DrawBitmap(bmp, 0, 0);
                    canvas.Flush();
                }

                sw.Stop();
                watch.Stop();
                using (var dpaint = new SKPaint())
                {
                    dpaint.IsAntialias = true;
                    dpaint.Color = SKColors.Black;
                    dpaint.IsStroke = false;

                    canvas.DrawText($"Time: {sw.ElapsedMilliseconds} ms, total: {watch.ElapsedMilliseconds} ms", 10f, 100f,new SKFont(SKTypeface.Default,48), dpaint);
                }
            }


            // 原始形状路径
            SKPath shapePath = new SKPath();
            //shapePath.AddCircle(0, 0, 10); // 一个圆形作为例子
            shapePath.AddOval(new SKRect(0, 0, 5, 15));

            // 目标曲线路径
            SKPath curvePath = new SKPath();
            curvePath.MoveTo(10, 100);
            //curvePath.CubicTo(60, 20, 240, 280, 400, 100);
            curvePath.CubicTo(600, 20, 840, 280, 1000, 100);

            // 创建变换矩阵
            SKMatrix matrix = SKMatrix.CreateScale(0.5f, 0.5f); // 拉伸
                                                                // 或者创建自定义扭曲
                                                                // matrix = SKMatrix.MakeSkew(0.3f, 0.1f);

            // 应用变换
            SKPathEffect pathEffect = SKPathEffect.Create1DPath(shapePath,1,0,SKPath1DPathEffectStyle.Rotate);//SKPathEffect.Create2DPath(matrix, shapePath);
            //SKPathEffect pathEffect = SKPathEffect.Create2DPath;//SKPathEffect.Create2DPath(matrix, shapePath);


            // 绘制变形的路径
            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                PathEffect = pathEffect,
                Color = SKColors.White,
                //Style = SKPaintStyle.Stroke
            };

            // 在 Canvas 上绘制
            canvas.DrawPath(curvePath, paint);

            _StartTest = false;
        }

        private SKBitmap CreateBitmap(int width, int height)
        {
            var bitmap = new SKBitmap(width, height);

            using( SKCanvas canvas = new SKCanvas(bitmap))
            {
                for (int i = 0; i < 1000; i++)
                {
                    canvas.Clear(SKColors.Blue);
                    canvas.Flush();
                }
            }
            //var random = new Random();

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        // 随机生成点，模拟粉笔颗粒效果
            //        var alpha = random.Next(120, 255);
            //        var color = new SKColor(255, 255, 255, (byte)alpha); // 白色点
            //        bitmap.SetPixel(x, y, color);
            //    }
            //}

            return bitmap;
        }

        bool _StartTest = false;
        Stopwatch watch = new Stopwatch();
        private void btnTest_Clicked(object sender, EventArgs e)
        {
            _StartTest = true;
            watch.Restart();
            canvasView.InvalidateSurface();
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }
    }

}
