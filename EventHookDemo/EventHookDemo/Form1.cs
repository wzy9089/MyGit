using System.Diagnostics;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.WindowsAndMessaging;
namespace EventHookDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _WinEventProc = new WINEVENTPROC(EventProc);
        }

        const uint EVENT_OBJECT_FOCUS = 0x8005;
        const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        const uint WINEVENT_OUTOFCONTEXT = 0x0000;  // Events are ASYNC
        const uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread
        const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
        const uint WINEVENT_INCONTEXT = 0x0004;  // Events are SYNC, this causes your dll to be injected into every process

        WINEVENTPROC _WinEventProc;
        HWINEVENTHOOK eventHookHandle;
        private void Form1_Load(object sender, EventArgs e)
        {
            eventHookHandle = PInvoke.SetWinEventHook(0, 0xff, HMODULE.Null, _WinEventProc, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            Debug.WriteLine(eventHookHandle.Value);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(!eventHookHandle.IsNull)
            {
                PInvoke.UnhookWinEvent(eventHookHandle);
            }
        }

        internal unsafe void EventProc(Windows.Win32.UI.Accessibility.HWINEVENTHOOK hWinEventHook, uint @event, Windows.Win32.Foundation.HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            switch(@event)
            {
                case EVENT_SYSTEM_FOREGROUND:
                case EVENT_SYSTEM_MOVESIZEEND:
                    HWND hw = hwnd;
                    if (hw.Value == this.Handle)
                    {
                        break;
                    }
                    WINDOWINFO info = new WINDOWINFO();
                    RECT rect;
                    char[] chars = new char[512];
                    fixed (char* cstr = chars)
                    {
                        PWSTR str = new PWSTR(cstr);

                        PInvoke.GetWindowInfo(hw, ref info);
                        PInvoke.GetClientRect(hw, out rect);
                        PInvoke.GetWindowText(hw, str, 256);
                        
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Format("Caption:{0}", str));
                        sb.AppendLine(string.Format("WindowRect:X={0},Y={1},Width={2},Height={3}", info.rcWindow.X, info.rcWindow.Y, info.rcWindow.Width, info.rcWindow.Height));

                        this.textBox1.Text = sb.ToString();

                        this.Size = new Size(info.rcWindow.Width, info.rcWindow.Height - 40);
                        this.Location = new Point(info.rcWindow.X, info.rcWindow.Y + 40);
                        this.Focus();
                    }
                    break;
            }
        }

    }
}
