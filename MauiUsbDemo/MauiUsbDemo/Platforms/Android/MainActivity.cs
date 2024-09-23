using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MauiUsbDemo
{
    [Activity(Theme = "@style/Maui.SplashTheme",Exported =true, MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { "android.hardware.usb.action.USB_DEVICE_ATTACHED" })]
    //[MetaData("android.hardware.usb.action.USB_DEVICE_ATTACHED", Resource ="@xml/device_filter")]
    //[MetaData("android.hardware.usb.host",Value ="true")]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}
