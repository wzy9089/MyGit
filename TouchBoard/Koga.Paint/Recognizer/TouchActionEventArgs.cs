using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint.Recognizer
{
    public class TouchActionEventArgs:EventArgs
    {
        public List<TouchPointer> HistoricalPointers { get; private set; }
        public TouchPointer Pointer { get; private set; }

        public TouchActionTypes ActionType { get; private set; }

        internal TouchActionEventArgs(TouchActionTypes type, List<TouchPointer> historicalPointers)
        {
            Debug.Assert(historicalPointers != null && historicalPointers.Count > 0);

            ActionType = type;
            HistoricalPointers = historicalPointers;
            Pointer = historicalPointers.Last();
        }

        internal TouchActionEventArgs(TouchActionTypes type, TouchPointer pointer)
        {
            ActionType = type;
            Pointer = pointer;
            HistoricalPointers = new List<TouchPointer> { pointer };
        }
    }
}
