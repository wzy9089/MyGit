using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPointHook
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();           
        }

        IntPtr hwnd;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.inkCanvas1.DefaultDrawingAttributes.Color = Colors.Red;

            WindowInteropHelper helper = new WindowInteropHelper(this);
            hwnd = helper.Handle;
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));

            var ret = API.RegisterPointerInputTarget(hwnd, PointerInputType.PEN);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            bool ret;
            uint pointId;
            PointerInputType inputType=PointerInputType.PEN;
            POINTER_PEN_INFO penInfo = new POINTER_PEN_INFO();

            switch(msg)
            {
                case API.WM_POINTERENTER:
                case API.WM_POINTERUPDATE:
                case API.WM_POINTERDOWN:
                case API.WM_POINTERUP:
                case API.WM_POINTERLEAVE:
                    pointId = API.GET_POINTERID_WPARAM((uint)wParam.ToInt32());
                    ret = API.GetPointerType(pointId, ref inputType);
                    if(!ret)
                    {
                        break;
                    }

                    if(inputType==PointerInputType.PEN)
                    {
                        ret = API.GetPointerPenInfo(pointId, ref penInfo);
                        if(!ret)
                        {
                            break;
                        }

                        ProcessPenPoint(msg, penInfo,
                            API.GET_X_LPARAM((uint)lParam.ToInt32()),
                            API.GET_Y_LPARAM((uint)lParam.ToInt32()));
                    }
                    handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        //StylusPointCollection pointCollection = new StylusPointCollection();
        Stroke stroke;
        List<Point> points;

        bool isDown = false;
        private void ProcessPenPoint(int msg, POINTER_PEN_INFO penInfo,uint x,uint y)
        {
            Point pt = this.PointFromScreen(new Point(x, y));
            float pressure = penInfo.pressure;
            pressure/= 1024;
            StylusPoint sp = new StylusPoint(pt.X, pt.Y, pressure);
            switch(msg)
            {
                case API.WM_POINTERENTER:
                    if((penInfo.penFlags & PenFlags.ERASER)>0
                        || (penInfo.penFlags & PenFlags.INVERTED)>0)
                    {
                        inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    }
                    else
                    {
                        inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
                    }
                    break;
                case API.WM_POINTERDOWN:
                    isDown = true;
                    if (inkCanvas1.EditingMode == InkCanvasEditingMode.Ink)
                    {
                        stroke = new Stroke(new StylusPointCollection(new StylusPoint[] { sp }), inkCanvas1.DefaultDrawingAttributes);
                        inkCanvas1.Strokes.Add(stroke);
                    }
                    else if(inkCanvas1.EditingMode==InkCanvasEditingMode.EraseByStroke)
                    {
                        points = new List<Point>();
                        points.Add(pt);
                    }
                    break;
                case API.WM_POINTERUPDATE:
                    if (isDown)
                    {
                        if (inkCanvas1.EditingMode == InkCanvasEditingMode.Ink)
                        {
                            stroke.StylusPoints.Add(sp);
                        }
                        else if (inkCanvas1.EditingMode == InkCanvasEditingMode.EraseByStroke)
                        {
                            points.Add(pt);
                            StrokeCollection sc = inkCanvas1.Strokes.HitTest(points, inkCanvas1.EraserShape);
                            inkCanvas1.Strokes.Remove(sc);
                        }
                    }
                    break;
                case API.WM_POINTERUP:
                    isDown = false;
                    if (inkCanvas1.EditingMode == InkCanvasEditingMode.Ink)
                    {
                        stroke.StylusPoints.Add(sp);
                    }
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var ret = API.UnregisterPointerInputTarget(hwnd, PointerInputType.PEN);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
