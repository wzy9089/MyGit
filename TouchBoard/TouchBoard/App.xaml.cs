#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace TouchBoard
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage());
            window.MaximumWidth = double.PositiveInfinity;
            window.MaximumHeight = double.PositiveInfinity;
            window.MinimumWidth = 0;
            window.MinimumHeight = 0;

#if WINDOWS

            window.Created+=(s, e) =>
            {
                var mauiwin = s as Window;
                if (mauiwin != null)
                {
                    var win = mauiwin.Handler.PlatformView as Microsoft.UI.Xaml.Window;
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(win);
                    var winid = Win32Interop.GetWindowIdFromWindow(hwnd);
                    var appwin = AppWindow.GetFromWindowId(winid);
                    if (appwin != null)
                    {
                        appwin.SetPresenter(AppWindowPresenterKind.FullScreen);
                    }
                }
            };

#endif
            return window;
        }
    }
}