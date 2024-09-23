using System.Diagnostics;

namespace MauiUsbDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
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
            if (MainThread.IsMainThread)
            {
                
            }
            else
            {
            }
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
    }

}
