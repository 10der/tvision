using System;
using System.Runtime.InteropServices;
using TurboVision.Objects;

namespace TurboVision
{

    public enum CursorType
    {
        Hidden = 0,
        Block = 1,
        Underline = 2,
    }

    public struct SysPoint
    {
        public Int16 X;
        public Int16 Y;
    }

    public struct WIN32_CELL
    {
        public UInt16 Ch;
        public UInt16 Attr;
    }

    public enum KeyboardKeys
    {
        Esc = 0x011B, AltSpace = 0x0200, CtrlIns = 0x9200,
        ShiftIns = 0x5200, CtrlDel = 0x0600, ShiftDel = 0x0700,
        Back = 0x0E08, CtrlBack = 0x0E7F, ShiftTab = 0x0F00,
        Tab = 0x0F09, AltQ = 0x1000, AltW = 0x1100,
        AltE = 0x1200, AltR = 0x1300, AltT = 0x1400,
        AltY = 0x1500, AltU = 0x1600, AltI = 0x1700,
        AltO = 0x1800, AltP = 0x1900, CtrlEnter = 0x1C0A,
        Enter = 0x1C0D, AltA = 0x1E00, AltS = 0x1F00,
        AltD = 0x2000, AltF = 0x2100, AltG = 0x2200,
        AltH = 0x2300, AltJ = 0x2400, AltK = 0x2500,
        AltL = 0x2600, AltZ = 0x2C00, AltX = 0x2D00,
        AltC = 0x2E00, AltV = 0x2F00, AltB = 0x3000,
        AltN = 0x3100, AltM = 0x3200, F1 = 0x3B00,
        F2 = 0x3C00, F3 = 0x3D00, F4 = 0x3E00,
        F5 = 0x3F00, F6 = 0x4000, F7 = 0x4100,
        F8 = 0x4200, F9 = 0x4300, F10 = 0x4400,
        Home = 0x4700, Up = 0x4800, PageUp = 0x4900,
        GrayMinus = 0x4A2D, Left = 0x4B00, Right = 0x4D00,
        GrayPlus = 0x4E2B, End = 0x4F00, Down = 0x5000,
        PageDown = 0x5100, Ins = 0x5200, Del = 0x5300,
        ShiftF1 = 0x5400, ShiftF2 = 0x5500, ShiftF3 = 0x5600,
        ShiftF4 = 0x5700, ShiftF5 = 0x5800, ShiftF6 = 0x5900,
        ShiftF7 = 0x5A00, ShiftF8 = 0x5B00, ShiftF9 = 0x5C00,
        ShiftF10 = 0x5D00, CtrlF1 = 0x5E00, CtrlF2 = 0x5F00,
        CtrlF3 = 0x6000, CtrlF4 = 0x6100, CtrlF5 = 0x6200,
        CtrlF6 = 0x6300, CtrlF7 = 0x6400, CtrlF8 = 0x6500,
        CtrlF9 = 0x6600, CtrlF10 = 0x6700, AltF1 = 0x6800,
        AltF2 = 0x6900, AltF3 = 0x6A00, AltF4 = 0x6B00,
        AltF5 = 0x6C00, AltF6 = 0x6D00, AltF7 = 0x6E00,
        AltF8 = 0x6F00, AltF9 = 0x7000, AltF10 = 0x7100,
        CtrlPrinttScreen = 0x7200, CtrlLeft = 0x7300,
        CtrlRight = 0x7400, CtrlEnd = 0x7500, CtrlPageDown = 0x7600,
        CtrlHome = 0x7700, Alt1 = 0x7800, Alt2 = 0x7900,
        Alt3 = 0x7A00, Alt4 = 0x7B00, Alt5 = 0x7C00,
        Alt6 = 0x7D00, Alt7 = 0x7E00, Alt8 = 0x7F00,
        Alt9 = 0x8000, Alt0 = 0x8100, AltMinus = 0x8200,
        AltEqual = 0x8300, CtrlPageUp = 0x8400, AltBack = 0x0E00,
        NoKey = 0x0000, AltShiftBack = 0x0900,
    }

    public static class ScreenManager
    {
        public const ushort MaxViewWidth = 255;
        public static byte ScreenWidth;
        public static uint ScreenMode;
        public static byte ScreenHeight;
        public static bool HiResScreen;
        public static uint StartupMode = 0xFFFF;

        public static int StrtCurY1;
        public static int StrtCurY2;
        public static bool StrtCurVisible;

        public static CONSOLE_SCREEN_BUFFER_INFO SysBufInfo;

        public static char[] ScreenBuffer = new char[100000];

        public static long CurXPos = -1;
        public static long CurYPos = -1;

        static ScreenManager()
        {
            uint zz = 0;
            Windows.GetConsoleMode(Windows.GetStdHandle((int)StdHandles.STD_INPUT_HANDLE), ref zz);
            Windows.SetConsoleMode(Windows.GetStdHandle((int)StdHandles.STD_INPUT_HANDLE), zz | 0x0010);
        }

        public static bool ReadConsoleOutput(
            ref CHAR_INFO[] lpBuffer, COORD BufferSize, COORD BufferCoord, ref SMALL_RECT ReadRegion)
        {
            return Windows.ReadConsoleOutput((IntPtr)Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), lpBuffer, BufferSize, BufferCoord, ref ReadRegion);

            //unsafe
            //{
            //    fixed (char* ciRef = &lpBuffer[0].WideChar)
            //    {
            //        return Windows.ReadConsoleOutput(
            //            Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE),
            //            ciRef,
            //            BufferSize,
            //            BufferCoord,
            //            ref ReadRegion);
            //    }
            //}
        }

        public static void SysTVGetCurType(ref int Y1, ref int Y2, ref bool Visible)
        {
            CONSOLE_CURSOR_INFO Info;
            Windows.GetConsoleCursorInfo(Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), out Info);
            Visible = Info.Visible;
            if (Info.Size <= 25)
            {
                Y1 = 6;
                Y2 = 7;
            }
            else
            {
                Y1 = 1;
                Y2 = 7;
            }
        }

        public static uint SysTVGetScrMode(ref SysPoint Size)
        {
            uint result = 0;
            Windows.GetConsoleScreenBufferInfo(Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), out SysBufInfo);
            switch (SysBufInfo.dwSize.Y)
            {
                case 25: result = 0x0003;
                    break;
                case 43:
                case 50: result = 0x0103;
                    break;
                default: result = 0x00ff;
                    break;
            }

            Size.X = SysBufInfo.dwSize.X;
            Size.Y = SysBufInfo.dwSize.Y;
            if (Size.Y > 234)
                Size.Y = 234;
            return result;
        }

        public static char[] SysTVGetScrBuf()
        {
            return ScreenBuffer;
        }

        static CHAR_INFO[] SaveScreen = null;

        public static void SetCrtData()
        {
            int Y1, Y2;
            bool Visible;
            SysPoint SrcSize = new SysPoint();

            ScreenMode = SysTVGetScrMode(ref SrcSize);
            ScreenHeight = (byte)SrcSize.Y;
            ScreenWidth = (byte)SrcSize.X;
            HiResScreen = true;
            SaveScreen = new CHAR_INFO[(ScreenWidth) * (ScreenHeight)];
            Y1 = 0;
            Y2 = 0;
            Visible = true;
            SysTVGetCurType(ref Y1, ref Y2, ref Visible);
        }

        public static void ConsoleOutput(int x, int x2, int y)
        {
            int len;
            SMALL_RECT _to = new SMALL_RECT();
            CHAR_INFO[] cbuf = new CHAR_INFO[MaxViewWidth];
            COORD bsize, from;

            len = x2 - x;
            _to.Left = (short)x;
            _to.Top = (short)y;
            _to.Right = (short)(x + len - 1);
            _to.Bottom = (short)y;

            int offset = (y * ScreenManager.ScreenWidth + x) * 2;

            for (int i = 0; i < len; i++)
            {
                // tender
                if ((i < cbuf.Length) && (i * 2 + offset < ScreenManager.ScreenBuffer.Length))
                {
                    cbuf[i].WideChar = (char)ScreenManager.ScreenBuffer[i * 2 + offset];
                    cbuf[i].Attributes = (byte)ScreenManager.ScreenBuffer[i * 2 + 1 + offset];
                }
            }
            bsize.X = (short)len;
            bsize.Y = 1;
            from.X = 0;
            from.Y = 0;
            WriteConsoleOutput(cbuf, bsize, from, ref _to);
        }

        public static bool WriteConsoleOutput(
            CHAR_INFO[] char_info, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion)
        {
            return Windows.WriteConsoleOutput((IntPtr)Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), char_info, dwBufferSize, dwBufferCoord, ref lpWriteRegion);
        }

        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern IntPtr GetStdHandle(int nStdHandle);

        //[DllImport("kernel32.dll", ExactSpelling = true)]
        //private static extern IntPtr GetConsoleWindow();
        //private static IntPtr ThisConsole = GetConsoleWindow();

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;

        public static void SysTVSetScrMode(int Mode)
        {
            Windows.AllocConsole();

            //SMALL_RECT R;
            //COORD Size;

            //Size.X = 80;
            //Size.Y = 25;

            //if ((Mode & 0x0100) != 0)
            //    Size.Y = 50;

            //Windows.SetConsoleScreenBufferSize(
            //    Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), Size);

            //R.Left = 0;
            //R.Top = 0;
            //R.Right = (short)(Size.X - 1);
            //R.Bottom = (short)(Size.Y - 1);
            //System.Console.SetWindowSize(System.Console.LargestWindowWidth - 5, Console.LargestWindowHeight - 5);
            //System.Console.SetWindowPosition(0, 0);


            int cols = Console.LargestWindowWidth-2;
            int rows = Console.LargestWindowHeight-2;

            Console.SetWindowSize(cols, rows);
            Console.SetBufferSize(cols + 1, rows + 1);
            Console.SetWindowPosition(0, 0);
            Windows.ShowWindow(Windows.ThisConsole, MAXIMIZE);
        }

        public static void InitVideo()
        {
            SysTVGetCurType(ref StrtCurY1, ref StrtCurY2, ref StrtCurVisible);
            SysPoint Dummy = new SysPoint();
            if (StartupMode != 0xFFFF)
                StartupMode = SysTVGetScrMode(ref Dummy);
            if (StartupMode != ScreenMode)
                SysTVSetScrMode((int)ScreenMode);
            SetCrtData();
        }
    }

    public static class MouseManager
    {
        public static byte ButtonCount = 0;
        public static byte DownButtons = 0;
        public static bool LastDouble = false;
        public static byte LastButtons = 0;
        public static long MouseWhereX;
        public static long MouseWhereY;
        public static bool MouseEvents = false;
        public static byte MouseButtons = 0;
        public static bool MouseReverse = false;
        public static uint AutoDelay;
        public static uint AutoTicks;
        public static uint DownTicks;
        public static Point DownWhere;
        public static uint DoubleDelay = 8;
        public static uint RepeatDelay = 8;

        public static long SysTVDetectMouse()
        {
            return 2;
        }

        public static void DetectMouse()
        {
            ButtonCount = (byte)SysTVDetectMouse();
        }

        public static bool SysTVGetMouseEvent(out SysMouseEvent Event)
        {
            Event = new SysMouseEvent();
            W32Kbd.KbdUpdateEventQueues();
            if (W32Kbd.SysMouCount == 0)
                return false;
            else
            {
                W32Kbd.SysMouCount--;
                Event = W32Kbd.SysMouQue[0];
                for (int i = W32Kbd.SysMouCount; i > 0; i--)
                    W32Kbd.SysMouQue[i] = W32Kbd.SysMouQue[i - 1];
                return true;
            }

        }

        internal static void StoreEvent(uint MouWhat, ref Event Event, SysMouseEvent SysMouseEvent)
        {
            LastButtons = MouseButtons;
            MouseWhereX = SysMouseEvent.smePos.X;
            MouseWhereY = SysMouseEvent.smePos.Y;
            Event.What = (int)MouWhat;
            Event.Buttons = MouseButtons;
            Event.Double = LastDouble;
            Event.Where = new Point(SysMouseEvent.smePos.X, SysMouseEvent.smePos.Y);
        }

        public static Event GetMouseEvent()
        {
            byte B;
            uint CurTicks;
            SysMouseEvent SysMouseEvent;
            Event Event = new Event();

            if (!MouseEvents)
            {
                Event.What = Event.Nothing;
            }
            else
            {
                if (!SysTVGetMouseEvent(out SysMouseEvent))
                {
                    MouseButtons = LastButtons;
                    SysMouseEvent.smeTime = (int)W32Kbd.SysSysMouseCount();
                    SysMouseEvent.smePos.X = (short)MouseManager.MouseWhereX;
                    SysMouseEvent.smePos.Y = (short)MouseManager.MouseWhereY;
                }
                else
                {
                    if (MouseReverse)
                    {
                        B = 0;
                        if ((SysMouseEvent.smeButtons & 0x0001) != 0)
                            B += 0x0002;
                        if ((SysMouseEvent.smeButtons & 0x0002) != 0)
                            B += 0x0001;
                        SysMouseEvent.smeButtons = B;
                    }
                    MouseButtons = SysMouseEvent.smeButtons;
                }
                CurTicks = (uint)SysMouseEvent.smeTime / 55;
                if ((LastButtons != 0) && (MouseButtons == 0))
                    StoreEvent(Event.MouseUp, ref Event, SysMouseEvent);
                else
                    if (LastButtons == MouseButtons)
                    {
                        if ((SysMouseEvent.smePos.Y != MouseWhereY) || (SysMouseEvent.smePos.X != MouseWhereX))
                            StoreEvent(Event.MouseMove, ref Event, SysMouseEvent);
                        else
                            if ((MouseButtons != 0) && ((CurTicks - AutoTicks) >= AutoDelay))
                            {
                                AutoTicks = CurTicks;
                                AutoDelay = 1;
                                StoreEvent(Event.MouseAuto, ref Event, SysMouseEvent);
                            }
                            else
                            {
                                StoreEvent(Event.Nothing, ref Event, SysMouseEvent);
                            }
                    }
                    else
                    {
                        LastDouble = false;
                        if ((MouseButtons == DownButtons) && (SysMouseEvent.smePos.Y == DownWhere.Y) && (SysMouseEvent.smePos.X == DownWhere.X)
                            && (CurTicks - DownTicks) < DoubleDelay)
                            LastDouble = true;
                        DownButtons = MouseButtons;
                        DownWhere.Y = SysMouseEvent.smePos.Y;
                        DownWhere.X = SysMouseEvent.smePos.X;
                        DownTicks = CurTicks;
                        AutoTicks = CurTicks;
                        AutoDelay = RepeatDelay;
                        StoreEvent(Event.MouseDown, ref Event, SysMouseEvent);
                    }
            }
            return Event;
        }
    }

    public class Drivers
    {

        static Event SysConsoleEvent = new Event();

        const byte scSpace = 0x39; const byte scIns = 0x52; const byte scDel = 0x53;
        const byte scBack = 0x0E; const byte scUp = 0x48; const byte scDown = 0x50;
        const byte scLeft = 0x4B; const byte scRight = 0x4D; const byte scHome = 0x47;
        const byte scEnd = 0x4F; const byte scPgUp = 0x49; const byte scPgDn = 0x51;
        const byte scCtrlIns = 0x92; const byte scCtrlDel = 0x93; const byte scCtrlUp = 0x8D;
        const byte scCtrlDown = 0x91; const byte kbShift = kbLeftShift + kbRightShift;

        const byte kbRightShift = 0x01;
        const byte kbLeftShift = 0x02;
        const byte kbCtrlShift = 0x04;
        const byte kbAltShift = 0x08;
        const byte kbScrollState = 0x10;
        const byte kbNumState = 0x20;
        const byte kbCapsState = 0x40;
        const byte kbInsState = 0x80;

        public static System.Threading.Thread ConsoleEventThread = null;

        static Drivers()
        {
            ScreenManager.SetCrtData();
            MouseManager.DetectMouse();
        }

        public static void ConsoleOutput(int x, int x2, int y)
        {
            ScreenManager.ConsoleOutput(x, x2, y);
        }

        public static void InitVideo()
        {
            ScreenManager.InitVideo();
        }

        public static void InitMemory()
        {
        }

        public static int MaxViewWidth
        {
            get
            {
                return ScreenManager.MaxViewWidth;
            }
        }

        public static int ScreenWidth
        {
            get
            {
                return ScreenManager.ScreenWidth;
            }
        }

        public static int ScreenHeight
        {
            get
            {
                return ScreenManager.ScreenHeight;
            }
        }

        public static uint ScreenMode
        {
            get
            {
                return ScreenManager.ScreenMode;
            }
        }

        public char this[int index]
        {
            get
            {
                return ScreenManager.ScreenBuffer[index];
            }
            set
            {
                ScreenManager.ScreenBuffer[index] = value;
            }
        }

        public static uint StartupMode
        {
            get
            {
                return ScreenManager.StartupMode;
            }
            set
            {
                ScreenManager.StartupMode = value;
            }
        }

        public static uint ButtonCount
        {
            get
            {
                return MouseManager.ButtonCount;
            }
        }

        public void tryMethod()
        {
            ScreenManager.ConsoleOutput(0, 20, 20);
            Console.ReadLine();
        }

        public char[] SysScrBuf
        {
            get
            {
                return ScreenManager.ScreenBuffer;
            }
            set
            {
                ScreenManager.ScreenBuffer = value;
            }
        }

        public void WriteConsoleLine(short X, short Y, short Len)
        {
            int P;
            int Q;
            CHAR_INFO[] LineBuf = new CHAR_INFO[256];
            SMALL_RECT R = new SMALL_RECT();
            COORD BufPos;
            COORD LineSize;

            LineSize.X = ScreenManager.SysBufInfo.dwSize.X;
            LineSize.Y = 1;
            BufPos.X = 0;
            BufPos.Y = 0;
            R.Left = X;
            R.Top = Y;
            R.Right = (short)(X + Len - 1);
            R.Bottom = Y;

            P = ((Y * ScreenManager.SysBufInfo.dwSize.X) + X) * 2;
            Q = 0;

            while (Len > 0)
            {
                LineBuf[Q].WideChar = (char)SysScrBuf[P];
                P++;
                LineBuf[Q].Attributes = (byte)SysScrBuf[P];
                P++;
                Q++;
                Len--;
            }

            LineBuf[Q].WideChar = 'c';
            ScreenManager.WriteConsoleOutput(LineBuf, LineSize, BufPos, ref R);
        }

        public void SysTVShowBuf(int Pos, int Size)
        {
            short I, X, Y;
            Pos = Pos / 2;
            X = (short)(Pos % ScreenManager.SysBufInfo.dwSize.X);
            Y = (short)(Pos / ScreenManager.SysBufInfo.dwSize.X);
            while (Size > 0)
            {
                I = Math.Min((short)(ScreenManager.SysBufInfo.dwSize.X - X), (short)(Size / 2));
                WriteConsoleLine(X, Y, I);
                Size -= I * 2;
                X = 0;
                Y++;
            }
        }

        public static long CurXPos
        {
            get
            {
                return ScreenManager.CurXPos;
            }
            set
            {
                ScreenManager.CurXPos = value;
            }
        }

        public static long CurYPos
        {
            get
            {
                return ScreenManager.CurYPos;
            }
            set
            {
                ScreenManager.CurYPos = value;
            }
        }

        public static void SetConsoleCursorPosition(long X, long Y)
        {
            Windows.SetConsoleCursorPosition(
                Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), new COORD((short)X, (short)Y));
        }

        public static void SysTVSetCurPos(long X, long Y)
        {
            CurXPos = X;
            CurYPos = Y;
            SetConsoleCursorPosition(CurXPos, CurYPos);
        }

        public void SysTVClrScr()
        {
            int I, BufSize;
            BufSize = ScreenManager.SysBufInfo.dwSize.X * ScreenManager.SysBufInfo.dwSize.Y * 2;
            I = 0;
            while (I < BufSize)
            {
                SysScrBuf[I] = (char)' ';
                I++;
                SysScrBuf[I] = (char)'\x07';
                I++;
            }

            SysTVShowBuf(0, BufSize);
            SysTVSetCurPos(0, 0);
        }

        public void ClearScreen()
        {
            SysTVClrScr();
        }

        public static bool SetConsoleCursorInfo(int hConsoleOutput, CONSOLE_CURSOR_INFO lpConsoleCursorInfo)
        {
            return Windows.SetConsoleCursorInfo(hConsoleOutput, ref lpConsoleCursorInfo);
        }

        public static void SetCursorType(CursorType CursorType)
        {
            int[] CursorTypes = new int[3] { 1, 100, 15 };
            bool[] CursorVisible = new bool[3] { false, true, true };
            CONSOLE_CURSOR_INFO Info;
            Info.Size = CursorTypes[(int)CursorType];
            Info.Visible = CursorVisible[(int)CursorType];
            SetConsoleCursorInfo(Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), Info);
        }

        public void SysTVSetCurType(int Y1, int Y2, bool Show)
        {
            CONSOLE_CURSOR_INFO Info;
            Info.Visible = Show;
            if (Math.Abs(Y1 - Y2) <= 1)
            {
                Info.Size = 15;
            }
            else
            {
                Info.Size = 99;
            }
            SetConsoleCursorInfo(Windows.GetStdHandle((int)StdHandles.STD_OUTPUT_HANDLE), Info);
        }

        public void DoneVideo()
        {
            if (ScreenManager.StartupMode != 0xFFFF &&
                ScreenManager.StartupMode != ScreenManager.ScreenMode)
            {
                ScreenManager.SysTVSetScrMode((int)ScreenManager.StartupMode);
            }
            ClearScreen();
            SysTVSetCurType(ScreenManager.StrtCurY1, ScreenManager.StrtCurY2, ScreenManager.StrtCurVisible);
        }

        public static void SetCrtData()
        {
            ScreenManager.SetCrtData();
        }

        public static void SysTVInitMouse(ref long X, ref long Y)
        {
            X = 0;
            Y = 0;
        }

        public static void InitEvents()
        {
            if (MouseManager.ButtonCount != 0)
            {
                MouseManager.DownButtons = 0;
                MouseManager.LastDouble = false;
                MouseManager.LastButtons = 0;
                SysTVInitMouse(ref MouseManager.MouseWhereX, ref MouseManager.MouseWhereY);
                MouseManager.MouseEvents = true;
            }
        }

        public static KeyboardKeys CtrlToArrow(KeyboardKeys KeyCode)
        {
            int NumCodes = 11;
            string CtrlCodes = "\x13\x4\x5\x18\x1\x6\x7\x16\x12\x3\x8";
            string ArrowCodes = "\x4B00\x4D00\x4800\x5000\x4700\x4F00\x5300\x5200\x4900\x5100\x0E08";

            KeyboardKeys Result = KeyCode;
            for (int i = 0; i < NumCodes; i++)
            {
                if ((((ushort)KeyCode) & 0xFF) == (byte)(CtrlCodes[i]))
                    return (KeyboardKeys)((uint)ArrowCodes[i]);
            }

            return Result;
        }

        public static bool LowMemory()
        {
            return false;
        }

        const string AltCodes1 = "\x51\x57\x45\x52\x54\x59\x55\x49\x4F\x50\x00\x00\x00\x00\x41\x53\x44\x46\x47\x48\x4A\x4B\x4C\x00\x00\x00\x00\x00\x5A\x58\x43\x56\x42\x4E\x4D";
        const string AltCodes2 = "1234567890-=";

        public static uint GetAltCode(char Ch)
        {
            uint Result = 0;
            if (Ch == '\x00')
                return Result;
            Ch = char.ToUpper(Ch);
            if (Ch == '\xF0')
                return 0x0200;
            for (int i = 0x10; i <= 0x32; i++)
                if (AltCodes1[i - 0x10] == Ch)
                    return (uint)(i << 8);
            for (int i = 0x78; i <= 0x83; i++)
                if (AltCodes2[i - 0x78] == Ch)
                    return (uint)(i << 8);
            return Result;
        }

        public static byte GetShiftState()
        {
            return W32Kbd.SysShiftState;
        }

        public static int CStrLen(string s)
        {
            int j = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] != '~')
                    j++;
            return j;
        }

        public static bool SysTVGetKeyEvent(ref SysKeyEvent Event)
        {
            W32Kbd.KbdUpdateEventQueues();
            if (W32Kbd.SysKeyCount == 0)
                return false;
            else
            {
                W32Kbd.SysKeyCount--;
                Event = W32Kbd.SysKeyQue[0];
                for (int i = W32Kbd.SysKeyCount; i > 0; i--)
                    W32Kbd.SysKeyQue[i] = W32Kbd.SysKeyQue[i - 1];
                return true;
            }
        }

        public const int KeyDownMask = Event.KeyDown;
        public struct KeyTransEntry
        {
            public byte Scan;
            public byte Shift;
            public KeyboardKeys Code;
            public KeyTransEntry(byte AScan, byte AShift, KeyboardKeys ACode)
            {
                Scan = AScan;
                Shift = AShift;
                Code = ACode;
            }
        }

        public static Event GetConsoleEvent()
        {
            Event E = new Event();
            E.What = SysConsoleEvent.What;
            E.InfoPtr = SysConsoleEvent.InfoPtr;
            SysConsoleEvent.What = Event.Nothing;
            return E;
        }

        public static Event GetKeyEvent()
        {
            KeyTransEntry[] KeyTranslateTable = 
				{
                    new KeyTransEntry( scSpace, 0x08, KeyboardKeys.AltSpace),
					new KeyTransEntry( scIns, 0x04, KeyboardKeys.CtrlIns),
					new KeyTransEntry( scCtrlIns, 0x04, KeyboardKeys.CtrlIns),
					new KeyTransEntry( scIns, 0x01, KeyboardKeys.ShiftIns),
					new KeyTransEntry( scIns, 0x02, KeyboardKeys.ShiftIns),
					new KeyTransEntry( scIns, 0x03, KeyboardKeys.ShiftIns),
					new KeyTransEntry( scDel, 0x04, KeyboardKeys.CtrlDel),
					new KeyTransEntry( scCtrlDel, 0x04, KeyboardKeys.CtrlDel),
					new KeyTransEntry( scDel, 0x01, KeyboardKeys.ShiftDel),
					new KeyTransEntry( scDel, 0x02, KeyboardKeys.ShiftDel),
					new KeyTransEntry( scDel, 0x03, KeyboardKeys.ShiftDel),
					new KeyTransEntry( scBack, 0x09, KeyboardKeys.AltShiftBack),
					new KeyTransEntry( scBack, 0x0A, KeyboardKeys.AltShiftBack),
					new KeyTransEntry( scBack, 0x0B, KeyboardKeys.AltShiftBack),
					new KeyTransEntry( scBack, 0x08, KeyboardKeys.AltBack)
				};

            SysKeyEvent SysKeyEvent = new SysKeyEvent();
            Event Event = new Event();
            if (!SysTVGetKeyEvent(ref SysKeyEvent))
                Event.What = Event.Nothing;
            else
            {
                Event.What = KeyDownMask;
                Event.KeyCode = (KeyboardKeys)SysKeyEvent.skeKeyCode;
                Event.KeyEvent = SysKeyEvent.wKeyEvent;
                for (int i = KeyTranslateTable.GetLowerBound(0); i <= KeyTranslateTable.GetUpperBound(0); i++)
                    if ((KeyTranslateTable[i].Scan == Event.ScanCode) &&
                        (KeyTranslateTable[i].Shift & SysKeyEvent.skeShiftState) == KeyTranslateTable[i].Shift)
                    {
                        Event.KeyCode = KeyTranslateTable[i].Code;
                        break;
                    }
                if (Event.CharCode == 0xE0)
                    switch (Event.ScanCode)
                    {
                        case scUp:
                        case scDown:
                        case scLeft:
                        case scRight:
                        case scIns:
                        case scDel:
                        case scHome:
                        case scEnd:
                        case scPgUp:
                        case scPgDn:
                        case ((int)KeyboardKeys.CtrlHome >> 8) & 0xFF:
                        case ((int)KeyboardKeys.CtrlEnd >> 8) & 0xFF:
                        case ((int)KeyboardKeys.CtrlPageUp >> 8) & 0xFF:
                        case ((int)KeyboardKeys.CtrlPageDown >> 8) & 0xFF:
                        case ((int)KeyboardKeys.CtrlLeft >> 8) & 0xFF:
                        case ((int)KeyboardKeys.CtrlRight >> 8) & 0xFF:
                        case scCtrlUp:
                        case scCtrlDown:
                            Event.CharCode = 0;
                            break;
                    }
                if (Event.KeyCode == (KeyboardKeys)0xE00D)
                    Event.KeyCode = KeyboardKeys.Enter;
                Event.InfoPtr = null;
            }
            return Event;
        }
    }
}
