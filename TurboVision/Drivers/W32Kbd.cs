using System;
using System.Runtime.InteropServices;

namespace TurboVision
{
    public struct SysKeyEvent
    {
        public uint skeKeyCode;
        public byte skeShiftState
        {
            get
            {
                byte Result = 0;
                if (wKeyEvent.Alt)
                    Result = 0x0008;
                if (wKeyEvent.Ctrl)
                    Result |= 0x0004;
                if (wKeyEvent.Shift)
                    Result |= 0x0003;
                if ((wKeyEvent.dwControlKeyState & (int)ButtonStates.SCROLLLOCK_ON) != 0)
                    Result |= 0x0010;
                if ((wKeyEvent.dwControlKeyState & (int)ButtonStates.NUMLOCK_ON) != 0)
                    Result |= 0x0020;
                if ((wKeyEvent.dwControlKeyState & (int)ButtonStates.CAPSLOCK_ON) != 0)
                    Result |= 0x0040;
                return Result;
            }
        }
        public KEY_EVENT_RECORD wKeyEvent;
    }

    public struct SysMouseEvent
    {
        public int smeTime;
        public SysPoint smePos;
        public byte smeButtons;
    }

    public static class W32Kbd
    {

        public static int SysMouCount = 0;
        public static int SysKeyCount = 0;
        public static SysMouseEvent[] SysMouQue = new SysMouseEvent[16];
        public static SysKeyEvent[] SysKeyQue = new SysKeyEvent[16];
        public static int DoubleDelay = 8;
        public static bool MouseReverse = false;
        public static byte SysPlatform = 0;
            
        public static bool MouseEvents = false;

        public static long SysSysMouseCount()
        {
            return Windows.GetTickCount();
        }

        public static byte SysShiftState = 0;

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CONTROL = 17;
        public const byte VK_MENU = 18;

        public const byte VK_NUMPAD0 = 96;
        public const byte VK_NUMPAD1 = 97;
        public const byte VK_NUMPAD2 = 98;
        public const byte VK_NUMPAD3 = 99;
        public const byte VK_NUMPAD4 = 100;
        public const byte VK_NUMPAD5 = 101;
        public const byte VK_NUMPAD6 = 102;
        public const byte VK_NUMPAD7 = 103;
        public const byte VK_NUMPAD8 = 104;
        public const byte VK_NUMPAD9 = 105;

        public static System.UInt16 TranslateKeyCode(byte KeyCode, byte ScanCode, byte charCode, int ShiftState)
        {
            byte[] CtrlTable = { 119, 141, 132, 142, 115, 143, 116, 144, 117, 145, 118, 146, 147 };
            byte[] AltTable = { 151, 152, 153, 74, 155, 76, 157, 78, 159, 160, 161, 162, 163 };
            const byte CTRL_PRESSED = (byte)(ButtonStates.RIGHT_CTRL_PRESSED | ButtonStates.LEFT_CTRL_PRESSED);
            const byte ALT_PRESSED = (byte)(ButtonStates.LEFT_ALT_PRESSED | ButtonStates.RIGHT_ALT_PRESSED);

            if ((charCode != 0) && (ShiftState & (byte)ButtonStates.LEFT_ALT_PRESSED) == 0)
            {
                if ((charCode == 9) && ((ShiftState & (byte)ButtonStates.SHIFT_PRESSED) != 0))
                {
                    return 15 << 8;
                }
                else
                    if (charCode != 0xE0)
                    {
                        if ((ShiftState & 0x0100) != 0)
                            ScanCode = 0xE0;
                        if (((ShiftState & (byte)ButtonStates.RIGHT_ALT_PRESSED) != 0) && (charCode == 0xF0))
                            charCode = 0x00;
                        return (ushort)((ScanCode << 8) | charCode);
                    }
            }

            UInt16 Result = 0;
            switch (ScanCode)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 118) << 8);
                    else
                        Result = (ushort)((ScanCode << 8) + charCode);
                    break;
                case 28:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)(ScanCode << 8);
                    break;
                case 14:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)(ScanCode << 8);
                    break;
                case 53:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = 0xA400;
                    else
                        if ((ShiftState & CTRL_PRESSED) != 0)
                            Result = 0x9500;
                        else
                            Result = (ushort)(ScanCode << 8);
                    break;
                case 55:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = 0x3700;
                    else
                        if ((ShiftState & CTRL_PRESSED) != 0)
                            Result = 0x9600;
                        else
                            Result = (UInt16)(ScanCode << 8);
                    break;
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 45) << 8);
                    else if ((ShiftState & CTRL_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 35) << 8);
                    else if ((ShiftState & (byte)ButtonStates.SHIFT_PRESSED) > 0)
                        Result = (ushort)((ScanCode + 25) << 8);
                    else
                        Result = (ushort)(ScanCode << 8);
                    break;
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                    if ((ShiftState & CTRL_PRESSED) != 0)
                        Result = (ushort)(CtrlTable[ScanCode - 71] << 8);
                    else if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)(AltTable[ScanCode - 71] << 8);
                    else
                        Result = (ushort)(ScanCode << 8);
                    break;
                case 87:
                case 88:
                    if ((ShiftState & ALT_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 52) << 8);
                    else if ((ShiftState & CTRL_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 50) << 8);
                    else if ((ShiftState & (byte)ButtonStates.SHIFT_PRESSED) != 0)
                        Result = (ushort)((ScanCode + 48) << 8);
                    else
                        Result = (ushort)((ScanCode + 46) << 8);
                    break;
            }
            return Result;
        }

        internal struct AltNumericKey
        {
            public uint VK;
            public byte Value;
            public AltNumericKey(uint AVK, byte AValue)
            {
                VK = AVK;
                Value = AValue;
            }
        }

        public static void KbdUpdateEventQueues()
        {

            AltNumericKey[] AltNumericKeys =
				{
					new AltNumericKey( 0x9B00, 4),
					new AltNumericKey( 0x9D00, 6),
					new AltNumericKey( 0x9F00, 1),
					new AltNumericKey( 0xA100, 3),
					new AltNumericKey( 0x9700, 7),
					new AltNumericKey( 0x9900, 9),
					new AltNumericKey( 0x9800, 8),
					new AltNumericKey( 0xA000, 2),
					new AltNumericKey( 0xA200, 0),
					new AltNumericKey( 0x4c00, 5)};

            System.UInt32 EventCount;
            INPUT_RECORD[] InRec = new INPUT_RECORD[1];
            bool FoundAlt;
            byte AltNumeric = 0;

            if (SysKeyCount > SysKeyQue.GetUpperBound(0))
                return;
            do
            {
                EventCount = 0;
                bool x = Windows.GetNumberOfConsoleInputEvents(Windows.GetStdHandle((int)StdHandles.STD_INPUT_HANDLE),
                    ref EventCount);
                if (EventCount == 0)
                {
                    return;
                }
                //Windows.ReadConsoleInput(Windows.GetStdHandle((int)StdHandles.STD_INPUT_HANDLE), ref InRec, out EventCount);
                Windows.ReadConsoleInput(new IntPtr(Windows.GetStdHandle((int)StdHandles.STD_INPUT_HANDLE)), InRec, 1, out EventCount);
                if (EventCount == 0) return;
                switch ((EventTypes)InRec[0].EventType)
                {
                    case EventTypes.KEY_EVENT:
                        if (SysKeyCount <= SysKeyQue.GetUpperBound(0))
                            if (InRec[0].KeyEvent.bKeyDown)
                            {
                                SysKeyQue[SysKeyCount].wKeyEvent = InRec[0].KeyEvent;
                                SysShiftState = SysKeyQue[SysKeyCount].skeShiftState;
                                switch ((int)InRec[0].KeyEvent.wVirtualKeyCode)
                                {
                                    case VK_SHIFT:
                                    case VK_CONTROL:
                                    case VK_MENU:
                                        break;
                                    default:
                                        System.Text.Encoding enc = System.Text.Encoding.GetEncoding(866);
                                        byte[] bb = enc.GetBytes(new char[] { InRec[0].KeyEvent.UnicodeChar });
                                        SysKeyQue[SysKeyCount].skeKeyCode = TranslateKeyCode((byte)InRec[0].KeyEvent.wVirtualKeyCode, (byte)InRec[0].KeyEvent.wVirtualScanCode, /*InRec.KeyEvent.AsciiChar*/ bb[0], InRec[0].KeyEvent.dwControlKeyState);
                                        SysKeyQue[SysKeyCount].wKeyEvent = InRec[0].KeyEvent;
                                        if (SysKeyQue[SysKeyCount].skeKeyCode == 0)
                                            return;
                                        FoundAlt = false;
                                        if (((SysKeyQue[SysKeyCount].skeShiftState & 0x08) == 0x08) &&
                                            ((InRec[0].KeyEvent.dwControlKeyState & 0x100) == 0))
                                            if (SysPlatform == 1)
                                            {
                                                for (int i = 0; i < 10; i++)
                                                    if (SysKeyQue[SysKeyCount].skeKeyCode == AltNumericKeys[i].VK)
                                                    {
                                                        AltNumeric = (byte)((AltNumeric * 10) + AltNumericKeys[i].Value);
                                                        FoundAlt = true;
                                                    }
                                            }
                                            else
                                                if (((int)InRec[0].KeyEvent.wVirtualKeyCode >= VK_NUMPAD0) && ((int)InRec[0].KeyEvent.wVirtualKeyCode <= VK_NUMPAD9))
                                                {
                                                    AltNumeric = (byte)((AltNumeric * 10) + InRec[0].KeyEvent.wVirtualKeyCode - VK_NUMPAD0);
                                                    FoundAlt = true;
                                                }
                                        if (!FoundAlt)
                                        {
                                            SysKeyCount++;
                                            AltNumeric = 0;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if ((int)InRec[0].KeyEvent.wVirtualKeyCode == VK_MENU)
                                    if (AltNumeric != 0)
                                    {
                                        SysKeyQue[SysKeyCount].skeKeyCode = AltNumeric;
                                        AltNumeric = 0;
                                        SysKeyCount++;
                                    }
                            }
                        break;
                    case EventTypes.MOUSE_EVENT:
                        if (SysMouCount <= SysMouQue.GetUpperBound(0))
                        {
                            SysMouQue[SysMouCount].smePos.X = InRec[0].MouseEvent.dwMousePosition.X;
                            SysMouQue[SysMouCount].smePos.Y = InRec[0].MouseEvent.dwMousePosition.Y;
                            SysMouQue[SysMouCount].smeButtons = (byte)InRec[0].MouseEvent.dwButtonState;
                            SysMouQue[SysMouCount].smeTime = (int)SysSysMouseCount();
                            SysMouCount++;
                        }
                        break;
                }
            } while (true && (SysKeyCount <= SysKeyQue.GetUpperBound(0)));
        }
    }
}
