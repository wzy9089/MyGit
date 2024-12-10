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

        public TouchDeviceType DeviceType { get; internal set; }

        internal TouchActionEventArgs(TouchDeviceType deviceType, TouchActionTypes type, List<TouchPointer> historicalPointers)
        {
            Debug.Assert(historicalPointers != null && historicalPointers.Count > 0);

            DeviceType = deviceType;
            ActionType = type;
            HistoricalPointers = historicalPointers;
            Pointer = historicalPointers.Last();
        }

        internal TouchActionEventArgs(TouchDeviceType deviceType, TouchActionTypes type, TouchPointer pointer)
        {
            DeviceType = deviceType;
            ActionType = type;
            Pointer = pointer;
            HistoricalPointers = new List<TouchPointer> { pointer };
        }
    }
}
