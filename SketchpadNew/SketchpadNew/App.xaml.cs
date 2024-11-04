#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT.Interop;
#endif

namespace SketchpadNew
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.MaximumWidth = double.PositiveInfinity;
            window.MaximumHeight = double.PositiveInfinity;
            window.MinimumWidth = 0;
            window.MinimumHeight = 0;

#if WINDOWS
            //设置全屏和隐藏拖动条
            window.Created += (s, e) =>
            {
                var mauiWindow = s as Microsoft.Maui.Controls.Window;
                if (mauiWindow != null)
                {
                    var nativeWindow = mauiWindow.Handler.PlatformView as Microsoft.UI.Xaml.Window;
                    if (nativeWindow != null)
                    {
                        var hwnd = WindowNative.GetWindowHandle(nativeWindow);
                        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                        var appWindow = AppWindow.GetFromWindowId(windowId);

                        // 设置全屏
                        appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

                        //// 隐藏拖动条
                        //var titleBar = appWindow.TitleBar;
                        //titleBar.ExtendsContentIntoTitleBar = true;
                        //titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                        //titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                    }
                }
            };
#endif

            return window;
        }
    }
}
