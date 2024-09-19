namespace MauiUsbDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        //FOTDevice _fotDevice;
        public MainPage()
        {
            InitializeComponent();

            FOTDevice.Init();
            //_fotDevice = new FOTDevice();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            //_fotDevice.Connect();
            //count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void DisconnectBtn_Clicked(object sender, EventArgs e)
        {
            //_fotDevice.Disconnect();
        }
    }

}
