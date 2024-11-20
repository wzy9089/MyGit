using SkiaSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;
using KOGA.FOT.MAUI;

namespace MauiUsbDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            FOTDevice.Connected += FOTDevice_Connected;
            FOTDevice.Disconnected += FOTDevice_Disconnected;
            FOTDevice.RawImageReceived += FOTDevice_RawImageReceived;
        }

        static DateTime _LastTime = DateTime.Now;
        int _FrameCnt = 0;

        private void FOTDevice_RawImageReceived(object? sender, RawImageReceivedEventArgs e)
        {
            TimeSpan span = DateTime.Now - _LastTime;
            Debug.WriteLine($"FOTDevice_RawImageReceived:{span.TotalMilliseconds}");

            _FrameCnt++;
            if (_FrameCnt > 3)
            {
                _FrameCnt = 0;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    fotImg0.RawImage = RawDataHelper.CreateSKBitmapFromRawImageData(1080, 54, e.RawImage.Image0);
                    fotImg1.RawImage = RawDataHelper.CreateSKBitmapFromRawImageData(1080, 54, e.RawImage.Image1);
                });
            }

            _LastTime = DateTime.Now;
        }

        private void FOTDevice_Disconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("FOTDevice_Disconnected");
        }

        private void FOTDevice_Connected(object? sender, EventArgs e)
        {
            Debug.WriteLine("FOTDevice_Connected");
        }


        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            FOTDevice.Init();
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            FOTDevice.Uninit();
        }

        private void CounterBtn_Clicked(object sender, EventArgs e)
        {
            byte[] cmdData = new byte[]
            {
                0x18,0x01,0x1b,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            };
            int ret = FOTDevice.SetFeature(0x18, cmdData, cmdData.Length, 10);
            Debug.WriteLine($"SetFeature retuen {ret}");
            Thread.Sleep(10);
            ret = FOTDevice.GetFeature(0x18, cmdData, cmdData.Length, 10);
            Debug.WriteLine($"GetFeature retuen {ret}");
        }
    }

}
