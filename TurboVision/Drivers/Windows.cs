using System;
using System.Runtime.InteropServices;

namespace TurboVision
{
    public enum StdHandles : int
    {
        STD_OUTPUT_HANDLE = -11,
        STD_INPUT_HANDLE = -10
    }

    [Flags]
    public enum EventTypes
    {
        KEY_EVENT = 1,
        MOUSE_EVENT = 2,
        WINDOW_BUFFER_SIZE_EVENT = 4,
        MENU_EVENT = 8,
        FOCUS_EVENT = 16
    }

    [Flags]
    public enum ButtonStates : byte
    {
        RIGHT_ALT_PRESSED = 0x01,	//{ the right alt key is pressed. }
        LEFT_ALT_PRESSED = 0x02,	//{ the left alt key is pressed. }
        RIGHT_CTRL_PRESSED = 0x04,  //{ the right ctrl key is pressed. }
        LEFT_CTRL_PRESSED = 0x08,	//{ the left ctrl key is pressed. }
        SHIFT_PRESSED = 0x10,		//{ the shift key is pressed. }
        NUMLOCK_ON = 0x20,		    //{ the numlock light is on. }
        SCROLLLOCK_ON = 0x40,		//{ the scrolllock light is on. }
        CAPSLOCK_ON = 0x80,		    //{ the capslock light is on. }
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KEY_EVENT_RECORD
    {
        [FieldOffset(0)]
        public bool bKeyDown;
        [FieldOffset(4)]
        public System.UInt16 wRepeatCount;
        [FieldOffset(6)]
        public System.UInt16 wVirtualKeyCode;
        [FieldOffset(8)]
        public System.UInt16 wVirtualScanCode;
        [FieldOffset(10)]
        public char UnicodeChar;
        [FieldOffset(12)]
        public System.Int32 dwControlKeyState;
        [FieldOffset(10)]
        public byte AsciiChar;

        public bool Ctrl
        {
            get
            {
                return (dwControlKeyState & ((int)(ButtonStates.LEFT_CTRL_PRESSED | ButtonStates.RIGHT_CTRL_PRESSED))) != 0;
            }
        }

        public bool Alt
        {
            get
            {
                return (dwControlKeyState & ((int)(ButtonStates.LEFT_ALT_PRESSED | ButtonStates.RIGHT_ALT_PRESSED))) != 0;
            }
        }

        public bool Shift
        {
            get
            {
                return (dwControlKeyState & (int)ButtonStates.SHIFT_PRESSED) != 0;
            }
        }

        public byte CtrlAltShift
        {
            get
            {
                return (byte)(((Ctrl ? 1 : 0) * 4) + ((Alt ? 1 : 0) * 2) + (Shift ? 1 : 0));
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSE_EVENT_RECORD
    {
        [FieldOffset(0)]
        public COORD dwMousePosition;
        [FieldOffset(4)]
        public System.Int32 dwButtonState;
        [FieldOffset(8)]
        public System.Int32 dwControlKeyState;
        [FieldOffset(12)]
        public System.Int32 dwEventFlags;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct FOCUS_EVENT_RECORD
    {
        [FieldOffset(0)]
        public bool bSetFocus;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WINDOW_BUFFER_SIZE_RECORD
    {
        [FieldOffset(0)]
        public COORD dwSize;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MENU_EVENT_RECORD
    {
        [FieldOffset(0)]
        uint dwCommandId;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_RECORD
    {
        [FieldOffset(0)]
        public System.UInt16 EventType;
        [FieldOffset(2)]
        public System.UInt16 Reserved;
        [FieldOffset(4)]
        public KEY_EVENT_RECORD KeyEvent;
        [FieldOffset(4)]
        public MOUSE_EVENT_RECORD MouseEvent;
        [FieldOffset(4)]
        public FOCUS_EVENT_RECORD FocusEvent;
        [FieldOffset(4)]
        public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
        [FieldOffset(4)]
        public MENU_EVENT_RECORD MenuEvent;
        public override string ToString()
        {
            string S = "";
            switch (EventType)
            {
                case 1: S = "KeyEvent\n";
                    if (KeyEvent.bKeyDown)
                        S += "KeyDown\n";
                    else
                        S += "KeyUp\n";
                    S += "Repeat count : " + KeyEvent.wRepeatCount.ToString() + "\n";
                    S += "Virtual Key Code : " + KeyEvent.wVirtualKeyCode.ToString() + "\n";
                    S += "Virtual Scan Code : " + KeyEvent.wVirtualScanCode.ToString() + "\n";
                    S += "Unicode Char : " + KeyEvent.UnicodeChar;
                    break;
                case 2: S = "MouseEvent\n";
                    S += "X :" + MouseEvent.dwMousePosition.X.ToString() + "\n";
                    S += "Y :" + MouseEvent.dwMousePosition.Y.ToString() + "\n";
                    break;
                case 4: S = "WindowBufferSizeEvent\n";
                    break;
                case 8: S = "MenuEvent\n";
                    break;
                case 16: S = "FocusEvent\n";
                    if (FocusEvent.bSetFocus)
                        S += "Focus\n";
                    else
                        S += "Release\n";
                    break;
            }
            return S;
        }

    }

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    //public struct CHAR_INFO
    //{
    //    public char WideChar;
    //    public ushort Attributes;

    //    public CHAR_INFO(char chr, ConsoleColor foreColor, ConsoleColor backColor)
    //    {
    //        WideChar = chr;
    //        Attributes = (ushort)((uint)foreColor | ((uint)backColor << 4));
    //    }
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct SMALL_RECT
    //{
    //    public short Left;
    //    public short Top;
    //    public short Right;
    //    public short Bottom;

    //    public SMALL_RECT(int left, int top, int width, int height)
    //    {
    //        Left = (short)left;
    //        Top = (short)top;
    //        Right = (short)(left + width);
    //        Bottom = (short)(top + height);
    //    }
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct COORD
    //{
    //    public short X;
    //    public short Y;

    //    public COORD(int x, int y)
    //    {
    //        this.X = (short)x;
    //        this.Y = (short)y;
    //    }
    //}

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CHAR_INFO
    {
        public char WideChar;
        public ushort Attributes;

        public CHAR_INFO(char chr, ConsoleColor foreColor, ConsoleColor backColor)
        {
            WideChar = chr;
            Attributes = (ushort)((uint)foreColor | ((uint)backColor << 4));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
        public short Width;

        public SMALL_RECT(int left, int top, int width, int height)
        {
            Left = (short)left;
            Top = (short)top;
            Right = (short)(left + width);
            Bottom = (short)(top + height);
            Width = (short)(width);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;

        public COORD(int x, int y)
        {
            this.X = (short)x;
            this.Y = (short)y;
        }
    }

    public struct CONSOLE_SCREEN_BUFFER_INFO
    {
        public COORD dwSize;
        public COORD dwCursorPosition;
        public uint wAttributes;
        public SMALL_RECT srWindow;
        public COORD dwMaximumWindowSize;
    }

    public struct CONSOLE_CURSOR_INFO
    {
        public int Size;
        public bool Visible;
    }

    [System.Security.SuppressUnmanagedCodeSecurity, System.Security.SecurityCritical]
    static class Windows
    {
        const string kernelDll = "kernel32.dll";
        const string userDll = "user32.dll";

        #region Kernel Functions
        [DllImport(
           kernelDll,
           EntryPoint = "GetNumberOfConsoleInputEvents",
           CallingConvention = CallingConvention.StdCall,
           SetLastError = true)]
        private static extern bool _GetNumberOfConsoleInputEvents(int hConsoleInput, ref System.UInt32 lpNumberOfEvents);

        public static uint GetTickCount()
        {
            return (uint) Environment.TickCount & Int32.MaxValue;
        }

        [DllImport(
           kernelDll,
           EntryPoint = "AllocConsole",
           CallingConvention = CallingConvention.StdCall)]
        public static extern bool AllocConsole();

        [DllImport(
           kernelDll,
           EntryPoint = "GetStdHandle",
             SetLastError = true,
             CallingConvention = CallingConvention.StdCall)]
        public static extern System.Int32 GetStdHandle(System.Int32 nStdHandle);

        [DllImport(
            kernelDll,
            EntryPoint = "GetConsoleScreenBufferInfo",
            SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetConsoleScreenBufferInfo(
            int hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        public static bool GetConsoleScreenBufferInfo(
            out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo)
        {
            return GetConsoleScreenBufferInfo(
                GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), out lpConsoleScreenBufferInfo);
        }

        [DllImport(
            kernelDll,
            EntryPoint = "GetConsoleCursorInfo",
            SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetConsoleCursorInfo(
            int hConsoleOutput, out CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

        [DllImport(
            kernelDll,
            EntryPoint = "SetConsoleCursorPosition",
            SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConsoleCursorPosition(int hConsoleOutput, COORD dwCursorPosition);

        [DllImport(
           kernelDll,
           EntryPoint = "SetConsoleCursorInfo",
           SetLastError = true,
           CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConsoleCursorInfo(int hConsoleOutput, ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

        [DllImport(
            kernelDll,
            EntryPoint = "SetConsoleMode",
            SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConsoleMode(int hConsole, uint wCodePageId);

        [DllImport(
           kernelDll,
           EntryPoint = "GetConsoleMode",
           SetLastError = true,
           CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetConsoleMode(int hConsole, ref uint wCodePageId);

        [DllImport(
            kernelDll,
            EntryPoint = "SetConsoleTitleW",
            SetLastError = true,
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode)]
        public static extern bool SetConsoleTitle(string Title);

        [DllImport(kernelDll, EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead
            );

        [DllImport(kernelDll, SetLastError = true, EntryPoint = "ReadConsoleOutputW", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleOutput(
            IntPtr hConsoleOutput,
            [Out] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpReadRegion
            );

        [DllImport(kernelDll, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpWriteRegion
            );

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        public static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region User Functions

        [DllImport(userDll)]
        public static extern int GetKeyNameText(
            int lParam, ref char[] KeyName, int Size);

        #endregion

        public static bool GetNumberOfConsoleInputEvents(System.Int32 hConsoleInput, ref System.UInt32 lpNumberOfEvents)
        {
            return _GetNumberOfConsoleInputEvents(hConsoleInput, ref lpNumberOfEvents);
        }
    }
}
