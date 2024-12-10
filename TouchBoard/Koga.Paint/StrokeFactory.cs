using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    internal static class StrokeFactory
    {
        public static Stroke Create(StrokeType strokeType)
        {
            switch (strokeType)
            {
                case StrokeType.Marker:
                    return new MarkerStroke();
                case StrokeType.Chalk:
                    return new ChalkStroke();
                case StrokeType.Brush:
                    return new BrushStroke();
                case StrokeType.ChineseBrush:
                    return new BrushStroke();
                case StrokeType.Highlighter:
                    return new HighlighterStroke();
                default:
                    throw new ArgumentException("Invalid stroke type");
            }
        }
    }
}
