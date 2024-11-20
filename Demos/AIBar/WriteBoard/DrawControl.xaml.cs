using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using System.Numerics;
using Microsoft.UI;
using Microsoft.Graphics.Canvas.Geometry;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WriteBoard
{
    public sealed partial class DrawControl : UserControl
    {
        CanvasBitmap _CanvasBmp;
        public DrawControl()
        {
            this.InitializeComponent();
        }

        private void inkCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawCircle(new Vector2(200, 200), 30, new CanvasSolidColorBrush(args.DrawingSession, Colors.AliceBlue));

            //if (_CanvasBmp != null)
            //{
            //    FrostedGlassEffect effect = new()
            //    {
            //        Source = _CanvasBmp,
            //        BlurAmount = 12,
            //        NoiseAmount = 0.1
            //    };

            //    args.DrawingSession.DrawImage(effect,new Rect(0,0,this.ActualSize.X,this.ActualSize.Y),_CanvasBmp.Bounds);
            //}
            //args.DrawingSession.Blend = CanvasBlend.Add;
            if (_EE != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pt = new Vector2(10, i * 100);
                    args.DrawingSession.DrawImage(_GBE, pt);
                }
            }

        }

        private void inkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

        }

        private void inkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void inkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private void strokeCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawCircle(new Vector2(200, 200), 40, new CanvasSolidColorBrush(args.DrawingSession, Colors.Red));
            //args.DrawingSession.DrawImage()
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.strokeCanvas.RemoveFromVisualTree();
            this.inkCanvas.RemoveFromVisualTree();
            this.inkCanvas = null;
            this.strokeCanvas = null;
        }

        TurbulenceEffect _TE;
        ColorMatrixEffect _CME;
        ExposureEffect _EE;
        GrayscaleEffect _GE;
        GaussianBlurEffect _GBE;

        private void inkCanvas_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(inkCanvas_CreateResourcesAsync(sender).AsAsyncAction());

            //sender.Invalidate();
        }

        private async Task inkCanvas_CreateResourcesAsync(CanvasControl sender)
        {
            _TE = new()
            {
                Frequency = new Vector2(1f, 1f),
                Noise = TurbulenceEffectNoise.Turbulence,
                Size = new Vector2(100,100)
            };

            _CME = new()
            {
                ColorMatrix = new Matrix5x4(1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                Source = _TE
            };

            _GE = new()
            {
                Source = _TE
            };

            _EE = new()
            {
                Exposure = 2,
                Source = _GE
            };

            _GBE = new()
            {
                BlurAmount = 0.1f,
                Source = _EE
            };

            _CanvasBmp = await CanvasBitmap.LoadAsync(sender, @"D:\ÕÕÆ¬\101MSDCF\DSC00002.jpg", 96);
        }
    
    }
}
