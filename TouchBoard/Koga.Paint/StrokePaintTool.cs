using Koga.Paint.Recognizer;
using Microsoft.Maui;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal class StrokePaintTool : PaintTool
    {
        private Dictionary<uint, Stroke> strokes = new Dictionary<uint, Stroke>();

        public override string ToolID => "Koga.PaintTool.StrokePaintTool";

        public StrokePaintTool(IPaintControl control) : base(control)
        {
            Parameters["StrokeType"] = StrokeType.Marker;
            Parameters["StrokeWidth"] = 5;
            Parameters["StrokeColor"] = SKColors.Black;
        }

        internal override void TouchActionUpdate(TouchActionEventArgs e)
        {
            StrokeType strokeType;
            if(Parameters.ContainsKey("StrokeType"))
            {
                strokeType = (StrokeType)Parameters["StrokeType"];
            }
            else
            {
                strokeType = StrokeType.Marker;
            }

            switch (e.ActionType)
            {
                case TouchActionTypes.Pressed:
                    if (!strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        var stroke = StrokeFactory.Create(strokeType);

                        strokes.Add(e.Pointer.PointerId, stroke);
                        strokes[e.Pointer.PointerId].StrokeStart(ToDipPoint(e.Pointer.Position));
                    }
                    break;
                case TouchActionTypes.Moved:
                    if (strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        var pointer = e.HistoricalPointers.First();
                        strokes[e.Pointer.PointerId].StrokeAdd(ToDipPoint(pointer.Position), true);
                        for (int i = 1; i < e.HistoricalPointers.Count; i++)
                        {
                            pointer = e.HistoricalPointers[i];
                            strokes[e.Pointer.PointerId].StrokeAdd(ToDipPoint(pointer.Position), false);
                        }

                        bool redraw = strokeType == StrokeType.Highlighter ? true : false;
                        Owner.UpdateRealtimeView();
                    }
                    break;

                case TouchActionTypes.Cancelled:
                case TouchActionTypes.Released:
                    if (strokes.ContainsKey(e.Pointer.PointerId))
                    {
                        var stroke = strokes[e.Pointer.PointerId];
                        stroke.StrokeEnd();

                        OnPaintFinished(new PaintToolEventArgs(stroke));

                        strokes.Remove(e.Pointer.PointerId);
                    }
                    break;
            }
        }

        internal override void Draw(SKCanvas canvas)
        {
            canvas.Clear();
            
            foreach (var stroke in strokes.Values)
            {
                stroke.Draw(canvas, false);
            }
        }
    }
}
