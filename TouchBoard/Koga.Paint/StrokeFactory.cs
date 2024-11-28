using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal static class StrokeFactory
    {
        public static Stroke Create(StrokeTypes strokeType)
        {
            switch (strokeType)
            {
                case StrokeTypes.Marker:
                    return new MarkerStroke();
                case StrokeTypes.Chalk:
                    return new ChalkStroke();
                case StrokeTypes.Brush:
                    return new BrushStroke();
                case StrokeTypes.ChineseBrush:
                    return new BrushStroke();
                case StrokeTypes.Highlighter:
                    return new HighlighterStroke();
                default:
                    throw new ArgumentException("Invalid stroke type");
            }
        }
    }
}
