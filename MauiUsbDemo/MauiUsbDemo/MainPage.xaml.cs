using SkiaSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;
using KOGA.FOT.MAUI;

namespace MauiUsbDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        SKBitmap? skBmp0;
        SKBitmap? skBmp1;

        //FOTDevice _fotDevice;
        public MainPage()
        {
            InitializeComponent();

            FOTDevice.Connected += FOTDevice_Connected;
            FOTDevice.Disconnected += FOTDevice_Disconnected;
            FOTDevice.RawImageReceived += FOTDevice_RawImageReceived;
        }

        private void FOTDevice_RawImageReceived(object? sender, RawImageReceivedEventArgs e)
        {
            Debug.WriteLine($"FOTDevice_RawImageReceived:{DateTime.Now.Ticks}");
            MainThread.BeginInvokeOnMainThread(() => {
                skBmp0 = CreateSKBitmap(e.RawImage.Image0);
                skBmp1 = CreateSKBitmap(e.RawImage.Image1);
                skImg0.InvalidateSurface();
                skImg1.InvalidateSurface();
            });
        }

        private SKBitmap CreateSKBitmap(byte[] data)
        {
            if (data == null || data.Length != 1080 * 54)
                return null;

            SKBitmap bmp=new SKBitmap(1080,54,SKColorType.Gray8,SKAlphaType.Opaque);

            nint bmpPtr = bmp.GetPixels();
            Marshal.Copy(data, 0, bmpPtr, data.Length);

            return bmp;
        }
        private void FOTDevice_Disconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("FOTDevice_Disconnected");
        }

        private void FOTDevice_Connected(object? sender, EventArgs e)
        {
            Debug.WriteLine("FOTDevice_Connected");
        }

        MemoryStream ms = new MemoryStream();
        private void OnCounterClicked(object sender, EventArgs e)
        {
        }

        private void DisconnectBtn_Clicked(object sender, EventArgs e)
        {
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            FOTDevice.Init();
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            FOTDevice.Uninit();
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {

        }

        private void skImg0_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            if (skBmp0 == null)
            {
                canvas.Clear();
                return;
            }
            else
            {
                SKBitmap bmp = skBmp0.Resize(e.Info.Size, SKFilterQuality.Medium);
                canvas.DrawBitmap(bmp, 0, 0);
            }
        }

        private void skImg1_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            if (skBmp1 == null)
            {
                canvas.Clear();
                return;
            }
            else
            {
                SKBitmap bmp = skBmp1.Resize(e.Info.Size, SKFilterQuality.Medium);
                canvas.DrawBitmap(bmp, 0, 0);
            }
        }
    }

}
