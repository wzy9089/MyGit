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
        //SKBitmap? skBmp1;

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
                fotImg0.RawImage = RawDataHelper.CreateSKBitmapFromRawImageData(1080, 54, e.RawImage.Image0);
                fotImg1.RawImage = RawDataHelper.CreateSKBitmapFromRawImageData(1080, 54, e.RawImage.Image1);
            });
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
    }

}
