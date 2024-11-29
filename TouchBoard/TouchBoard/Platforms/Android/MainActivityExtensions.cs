using Android.App;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchBoard.Platforms.Android
{
    public static class MainActivityExtensions
    {
        [Obsolete]
        public static void SetFullScreen(this Activity activity)
        {
            var window = activity.Window;
            if (window != null)
            {
                window.AddFlags(WindowManagerFlags.Fullscreen);
                window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
                window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.ImmersiveSticky);
            }
        }
    }
}
