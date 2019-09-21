using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfPointHook
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HiLoWord
    {
        [FieldOffset(0)]
        public uint Number;

        [FieldOffset(0)]
        public ushort Low;

        [FieldOffset(2)]
        public ushort High;

        public HiLoWord(uint number) : this()
        {
            Number = number;
        }

        public static implicit operator uint(HiLoWord val)
        {
            return val.Number;
        }

        public static implicit operator HiLoWord(uint val)
        {
            return new HiLoWord(val);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTER_INFO
    {
        public PointerInputType pointerType;
        public uint pointerId;
        public uint frameId;
        public PointerFlags pointerFlags;
        public IntPtr sourceDevice;
        public IntPtr hwndTarget;
        public POINT ptPixelLocation;
        public POINT ptHimetricLocation;
        public POINT ptPixelLocationRaw;
        public POINT ptHimetricLocationRaw;
        public uint dwTime;
        public uint historyCount;
        public int InputData;
        public uint dwKeyStates;
        public UInt64 PerformanceCount;
        public PointerButtonChangeType ButtonChangeType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTER_PEN_INFO
    {
        public POINTER_INFO pointerInfo;
        public PenFlags penFlags;
        public PenMask penMask;
        public uint pressure;
        public uint rotation;
        public int tiltX;
        public int tiltY;
    }

    public enum PenFlags
    {
        NONE = 0x00000000, // Default
        BARREL = 0x00000001, // The barrel button is pressed
        INVERTED = 0x00000002, // The pen is inverted
        ERASER = 0x00000004 // The eraser button is pressed
    }

    public enum PenMask
    {
        NONE = 0x00000000, // Default - none of the optional fields are valid
        PRESSURE = 0x00000001, // The pressure field is valid
        ROTATION = 0x00000002, // The rotation field is valid
        TILT_X = 0x00000004, // The tiltX field is valid
        TILT_Y = 0x00000008 // The tiltY field is valid
    }

    public enum PointerFlags
    {
        /// <summary>
        /// Default
        /// </summary>
        NONE = 0x00000000,
        /// <summary>
        /// Indicates the arrival of a new pointer
        /// </summary>
        NEW = 0x00000001,
        /// <summary>
        /// Indicates that this pointer continues to exist. When this flag is not set, it indicates the pointer has left detection range. 
        /// This flag is typically not set only when a hovering pointer leaves detection range (PointerFlag.UPDATE is set) or when a pointer in contact with a window surface leaves detection range (PointerFlag.UP is set). 
        /// </summary>
        INRANGE = 0x00000002,
        /// <summary>
        /// Indicates that this pointer is in contact with the digitizer surface. When this flag is not set, it indicates a hovering pointer.
        /// </summary>
        INCONTACT = 0x00000004,
        /// <summary>
        /// Indicates a primary action, analogous to a mouse left button down.
        ///A touch pointer has this flag set when it is in contact with the digitizer surface.
        ///A pen pointer has this flag set when it is in contact with the digitizer surface with no buttons pressed.
        ///A mouse pointer has this flag set when the mouse left button is down.
        /// </summary>
        FIRSTBUTTON = 0x00000010,
        /// <summary>
        /// Indicates a secondary action, analogous to a mouse right button down.
        /// A touch pointer does not use this flag.
        /// A pen pointer has this flag set when it is in contact with the digitizer surface with the pen barrel button pressed.
        /// A mouse pointer has this flag set when the mouse right button is down.
        /// </summary>
        SECONDBUTTON = 0x00000020,
        /// <summary>
        /// Indicates a secondary action, analogous to a mouse right button down. 
        /// A touch pointer does not use this flag. 
        /// A pen pointer does not use this flag. 
        /// A mouse pointer has this flag set when the mouse middle button is down.
        /// </summary>
        THIRDBUTTON = 0x00000040,
        /// <summary>
        /// Indicates actions of one or more buttons beyond those listed above, dependent on the pointer type. Applications that wish to respond to these actions must retrieve information specific to the pointer type to determine which buttons are pressed. For example, an application can determine the buttons states of a pen by calling GetPointerPenInfo and examining the flags that specify button states.
        /// </summary>
        OTHERBUTTON = 0x00000080,
        /// <summary>
        /// Indicates that this pointer has been designated as primary. A primary pointer may perform actions beyond those available to non-primary pointers. For example, when a primary pointer makes contact with a window’s surface, it may provide the window an opportunity to activate by sending it a WM_POINTERACTIVATE message.
        /// </summary>
        PRIMARY = 0x00000100,
        /// <summary>
        /// Confidence is a suggestion from the source device about whether the pointer represents an intended or accidental interaction, which is especially relevant for PT_TOUCH pointers where an accidental interaction (such as with the palm of the hand) can trigger input. The presence of this flag indicates that the source device has high confidence that this input is part of an intended interaction.
        /// </summary>
        CONFIDENCE = 0x00000200,
        /// <summary>
        /// Indicates that the pointer is departing in an abnormal manner, such as when the system receives invalid input for the pointer or when a device with active pointers departs abruptly. If the application receiving the input is in a position to do so, it should treat the interaction as not completed and reverse any effects of the concerned pointer.
        /// </summary>
        CANCELLED = 0x00000400,
        /// <summary>
        /// Indicates that this pointer just transitioned to a “down” state; that is, it made contact with the window surface.
        /// </summary>
        DOWN = 0x00010000,
        /// <summary>
        /// Indicates that this information provides a simple update that does not include pointer state changes.
        /// </summary>
        UPDATE = 0x00020000,
        /// <summary>
        /// Indicates that this pointer just transitioned to an “up” state; that is, it broke contact with the window surface.
        /// </summary>
        UP = 0x00040000,
        /// <summary>
        /// Indicates input associated with a pointer wheel. For mouse pointers, this is equivalent to the action of the mouse scroll wheel (WM_MOUSEWHEEL).
        /// </summary>
        WHEEL = 0x00080000,
        /// <summary>
        /// Indicates input associated with a pointer h-wheel. For mouse pointers, this is equivalent to the action of the mouse horizontal scroll wheel (WM_MOUSEHWHEEL).
        /// </summary>
        HWHEEL = 0x00100000
    }


    public enum PointerButtonChangeType
    {
        POINTER_CHANGE_NONE,
        POINTER_CHANGE_FIRSTBUTTON_DOWN,
        POINTER_CHANGE_FIRSTBUTTON_UP,
        POINTER_CHANGE_SECONDBUTTON_DOWN,
        POINTER_CHANGE_SECONDBUTTON_UP,
        POINTER_CHANGE_THIRDBUTTON_DOWN,
        POINTER_CHANGE_THIRDBUTTON_UP,
        POINTER_CHANGE_FOURTHBUTTON_DOWN,
        POINTER_CHANGE_FOURTHBUTTON_UP,
        POINTER_CHANGE_FIFTHBUTTON_DOWN,
        POINTER_CHANGE_FIFTHBUTTON_UP
    }
    public enum PointerInputType
    {
        /// <summary>
        /// Generic pointer type. This type never appears in pointer messages or pointer data. Some data query functions allow the caller to restrict the query to specific pointer type. The PT_POINTER type can be used in these functions to specify that the query is to include pointers of all types
        /// </summary>
        POINTER = 0x00000001,
        /// <summary>
        /// Touch pointer type.
        /// </summary>
        TOUCH = 0x00000002,
        /// <summary>
        /// Pen pointer type.
        /// </summary>
        PEN = 0x00000003,
        /// <summary>
        /// Mouse pointer type
        /// </summary>
        MOUSE = 0x00000004
    };

    public static class API
    {
        public const int WM_NCPOINTERUPDATE = 0x0241;
        public const int WM_NCPOINTERDOWN = 0x0242;
        public const int WM_NCPOINTERUP = 0x0243;
        public const int WM_POINTERUPDATE = 0x0245; //581
        public const int WM_POINTERDOWN = 0x0246;
        public const int WM_POINTERUP = 0x0247; //583
        public const int WM_POINTERENTER = 0x0249; //585
        public const int WM_POINTERLEAVE = 0x024A; //586
        public const int WM_POINTERACTIVATE = 0x024B;
        public const int WM_POINTERCAPTURECHANGED = 0x024C;
        public const int WM_TOUCHHITTESTING = 0x024D;
        public const int WM_POINTERWHEEL = 0x024E;
        public const int WM_POINTERHWHEEL = 0x024F;
        public const int DM_POINTERHITTEST = 0x0250;

        public static uint GET_POINTERID_WPARAM(uint wParam) { return ((HiLoWord)wParam).Low; }
        public static uint GET_X_LPARAM(uint lParam) { return ((HiLoWord)lParam).Low; }
        public static uint GET_Y_LPARAM(uint lParam) { return ((HiLoWord)lParam).High; }

        [DllImport("User32")]
        public static extern bool RegisterPointerInputTarget(IntPtr hwnd, PointerInputType pointerType);

        [DllImport("User32")]
        public static extern bool UnregisterPointerInputTarget(IntPtr hwnd, PointerInputType pointerType);

        [DllImport("User32")]
        public static extern bool GetPointerType(uint pointerId,  ref PointerInputType pointerType);

        [DllImport("User32")]
        public static extern bool GetPointerPenInfo(uint pointerId, [In,Out]ref POINTER_PEN_INFO penInfo);
    }
}
