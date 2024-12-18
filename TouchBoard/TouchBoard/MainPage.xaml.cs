﻿using CommunityToolkit.Maui.Views;
using SkiaSharp;
using System.Diagnostics;

#if ANDROID
using TouchBoard.Platforms.Android;
#endif

namespace TouchBoard
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            SetFullScreen();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetFullScreen();
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.Green);
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }

        private void btnClear_Clicked(object sender, EventArgs e)
        {
            paintControl.Clear();
        }

        private void btnTool_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if(button!=null)
            {
                if(button.CommandParameter.Equals("笔"))
                {
                    StrokeSettingsPopup ssp = new StrokeSettingsPopup();
                    paintControl.PaintToolManager.SetCurrentTool("Koga.PaintTool.StrokePaintTool");
                    ssp.ViewModel.Selection = (int)paintControl.PaintToolManager.CurrentTool.Parameters["StrokeType"];
                    ssp.ViewModel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Selection")
                        {
                            paintControl.PaintToolManager.CurrentTool.Parameters["StrokeType"] = (Koga.Paint.StrokeType)ssp.ViewModel.Selection;
                        }
                    };
                    ssp.Anchor = button;
                    this.ShowPopup(ssp);
                }
            }

        }

        private void flowStroke_StrokeChanged(object sender, Koga.Paint.StrokeType e)
        {
            paintControl.PaintToolManager.CurrentTool.Parameters["StrokeType"] = e;
        }


        private void SetFullScreen()
        {
#if ANDROID
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            activity?.SetFullScreen();
#endif
        }
    }

}
