using System;
using TurboVision.Objects;
using TurboVision.Validators;
using TurboVision.Views;
using TurboVision.StdDlg;

namespace TurboVision.Dialogs
{
    /// <summary>
    /// Summary description for InputLine.
    /// </summary>
    public class InputLine : View
    {

        private static uint[] CInputLine = { 0x13, 0x13, 0x14, 0x15 };

        private int maxLen;
        private int curPos;
        private int firstPos;
        private int selStart;
        private int selEnd;

        public string Data = "";

        public Validator Validator;

        public InputLine()
            : base(new Rect(0, 0, 30, 1))
        {
        }

        public InputLine(Rect Bounds, int AMaxLen)
            : base(Bounds)
        {
            State |= StateFlags.CursorVis;
            Options |= (OptionFlags.ofSelectable | OptionFlags.ofFirstClick | OptionFlags.ofVersion20);
            Data = "";
            MaxLen = AMaxLen;
        }

        public void SelectAll(bool Enable)
        {
            curPos = 0;
            firstPos = 0;
            selStart = 0;
            if (Enable)
                SelEnd = Data.Length;
            else
                SelEnd = 0;
        }

        public int MaxLen
        {
            get
            {
                return maxLen;
            }
            set
            {
                maxLen = value;
            }
        }

        public int CurPos
        {
            get
            {
                return curPos;
            }
            set
            {
                curPos = value;
            }
        }

        public int FirstPos
        {
            get
            {
                return firstPos;
            }
            set
            {
                firstPos = value;
            }
        }
        public int SelStart
        {
            get
            {
                return selStart;
            }
            set
            {
                selStart = value;
            }
        }
        public int SelEnd
        {
            get
            {
                return selEnd;
            }
            set
            {
                selEnd = value;
            }
        }

        public override uint[] GetPalette()
        {
            return CInputLine;
        }

        public bool CanScroll(int Delta)
        {
            if (Delta < 0)
                return FirstPos > 0;
            else
                if (Delta > 0)
                    return (Data.Length - FirstPos + 2) > Size.X;
                else
                    return false;
        }

        public override void Draw()
        {
            uint Color;
            int L, R;
            DrawBuffer B = new DrawBuffer(Size.X * Size.Y);

            if ((State & StateFlags.Focused) == 0)
                Color = GetColor(1);
            else
                Color = GetColor(2);
            B.FillChar((char)' ', Color, (int)Size.X);
            B.FillStr((Data + new string(' ', (int)Size.X)).Substring(FirstPos, (int)(Size.X - 2)), Color, 1);
            if (CanScroll(1))
                B.FillChar(ldRightScroll, GetColor(4), 1, (int)Size.X - 1);
            if ((State & StateFlags.Focused) != 0)
            {
                if (CanScroll(-1))
                    B.FillChar(ldLeftScroll, GetColor(4), 1);
                L = SelStart - FirstPos;
                R = SelEnd - FirstPos;
                if (L < 0)
                    L = 0;
                if (R > (Size.X - 2))
                    R = (int)Size.X - 2;
                if (L < R)
                    B.FillChar('\x00', GetColor(3), R - L, L + 1);
            }
            WriteLine(0, 0, (int)Size.X, (int)Size.Y, B);
            SetCursor(CurPos - FirstPos + 1, 0);
        }

        public override object GetData()
        {
            return Data;
        }

        public override void SetData(object Rec)
        {
            Data = (string)Rec;
        }

        public override uint DataSize()
        {
            return 1;
        }

        internal int MouseDelta(ref Event Event)
        {
            Point Mouse;
            Mouse = MakeLocal(Event.Where);
            if (Mouse.X <= 0)
                return -1;
            else if (Mouse.X >= (Size.X - 1))
                return 1;
            else
                return 0;
        }

        internal int MousePos(ref Event Event)
        {
            int Pos;
            Point Mouse;
            Mouse = MakeLocal(Event.Where);
            if (Mouse.X < 1)
                Mouse.X = 1;
            Pos = Mouse.X + FirstPos - 1;
            if (Pos < 0)
                Pos = 0;
            if (Pos > Data.Length)
                Pos = Data.Length;
            return Pos;
        }

        internal void AdjustSelectBlock(ref int Anchor)
        {
            if (CurPos < Anchor)
            {
                SelStart = CurPos;
                SelEnd = Anchor;
            }
            else
            {
                SelStart = Anchor;
                SelEnd = CurPos;
            }
        }

        internal void SaveState(ref string OldData, ref int OldCurPos, ref int OldFirstPos, ref int OldSelStart, ref int OldSelEnd, ref bool WasAppending)
        {
            if (Validator != null)
            {
                OldData = Data;
                OldCurPos = CurPos;
                OldFirstPos = FirstPos;
                OldSelStart = SelStart;
                OldSelEnd = SelEnd;
                WasAppending = Data.Length == CurPos;
            }
        }

        internal bool CheckValid(bool NoAutoFill)
        {

            if (Validator != null)
            {
                return false;
            }
            else
                return true;
        }

        internal void DeleteSelect()
        {
            if (SelStart != SelEnd)
            {
                Data = Data.Remove(SelStart, SelEnd - SelStart);
                CurPos = SelStart;
            }
        }

        public override void HandleEvent(ref Event Event)
        {

            ConsoleKey[] PadKeys = new ConsoleKey[]{
                ConsoleKey.LeftArrow    ,
                ConsoleKey.RightArrow   ,
                ConsoleKey.UpArrow      ,
                ConsoleKey.DownArrow    ,
                ConsoleKey.Home         ,
                ConsoleKey.End          ,
                ConsoleKey.Delete       ,
                ConsoleKey.Insert       ,
                ConsoleKey.PageUp       ,
                ConsoleKey.PageDown     ,
                ConsoleKey.Backspace};
            int Delta, Anchor = 0;
            bool ExtendBlock = false;
            string OldData = "";
            int OldCurPos = 0, OldFirstPos = 0,
            OldSelStart = 0, OldSelEnd = 0;
            bool WasAppending = false;

            base.HandleEvent(ref Event);
            if ((State & StateFlags.Selected) != 0)
            {
                switch (Event.What)
                {
                    case Event.MouseDown:
                        {
                            Delta = MouseDelta(ref Event);
                            if (CanScroll(Delta))
                            {
                                do
                                {
                                    if (CanScroll(Delta))
                                    {
                                        FirstPos += Delta;
                                        DrawView();
                                    }
                                } while (MouseEvent(ref Event, Event.MouseAuto));
                            }
                            else
                                if (Event.Double)
                                    SelectAll(true);
                                else
                                {
                                    Anchor = MousePos(ref Event);
                                    do
                                    {
                                        if (Event.What == Event.MouseAuto)
                                        {
                                            Delta = MouseDelta(ref Event);
                                            if (CanScroll(Delta))
                                                FirstPos += Delta;
                                        }
                                        CurPos = MousePos(ref Event);
                                        AdjustSelectBlock(ref Anchor);
                                        DrawView();
                                    } while (MouseEvent(ref Event, Event.MouseMove | Event.MouseAuto));
                                }
                            ClearEvent(ref Event);
                        }
                        break;
                    case Event.KeyDown:
                        SaveState(ref OldData, ref OldCurPos, ref OldFirstPos, ref OldSelStart, ref OldSelEnd, ref WasAppending);
                        Event.KeyCode = Drivers.CtrlToArrow(Event.KeyCode);
                        if ((Array.IndexOf(PadKeys, (ConsoleKey)Event.KeyEvent.wVirtualKeyCode) != -1) &&
                            ((Event.KeyEvent.CtrlAltShift ^ 0x01) == 0))
                        {
                            Event.CharCode = 0x00;
                            if (CurPos == SelEnd)
                                Anchor = SelStart;
                            else
                                Anchor = SelEnd;
                            ExtendBlock = true;
                        }
                        else
                            ExtendBlock = false;
                        switch (Event.KeyCode)
                        {
                            case KeyboardKeys.Left:
                                if (CurPos > 0)
                                    CurPos--;
                                break;
                            case KeyboardKeys.Right:
                                if (CurPos < Data.Length)
                                {
                                    CurPos++;
                                    CheckValid(true);
                                }
                                break;
                            case KeyboardKeys.Home:
                                CurPos = 0;
                                break;
                            case KeyboardKeys.End:
                                CurPos = Data.Length;
                                CheckValid(true);
                                break;
                            case KeyboardKeys.Back:
                                if (CurPos > 0)
                                {
                                    Data = Data.Remove(CurPos - 1, 1);
                                    CurPos--;
                                    if (FirstPos > 0)
                                        FirstPos--;
                                    CheckValid(true);
                                }
                                break;
                            case KeyboardKeys.Del:
                                if (SelStart == SelEnd)
                                    if (CurPos < (Data.Length - 1))
                                    {
                                        SelStart = CurPos;
                                        SelEnd = CurPos + 1;
                                    }
                                DeleteSelect();
                                CheckValid(true);
                                break;
                            case KeyboardKeys.Ins:
                                SetState(StateFlags.CursorIns, (State & StateFlags.CursorIns) == 0);
                                break;
                            case KeyboardKeys.CtrlIns:
//                                if (SelStart == SelEnd)
//                                    System.Windows.Forms.Clipboard.SetText(Data);
//                                else
//                                    System.Windows.Forms.Clipboard.SetText(Data.Substring( SelStart, SelEnd - SelStart));
                                break;
                            default:
                                if ((Event.KeyEvent.UnicodeChar >= (byte)' ') /*&& ( Event.CharCode <= 255)*/)
                                {
                                    if ((State & StateFlags.CursorIns) != 0)
                                        Data = Data.Remove(CurPos, 1);
                                    else
                                        DeleteSelect();
                                    if (CheckValid(true))
                                    {
                                        if ((Data.Length - 1) < MaxLen)
                                        {
                                            if (FirstPos > CurPos)
                                                FirstPos = CurPos;
                                            CurPos++;
                                            Data = Data.Insert(CurPos - 1, new string(Event.KeyEvent.UnicodeChar, 1));
                                        }
                                        CheckValid(true);
                                    }
                                }
                                else if (Event.CharCode == 0x19)
                                {
                                    Data = "";
                                    CurPos = 0;
                                }
                                else
                                    return;
                                break;
                        }
                        if (ExtendBlock)
                            AdjustSelectBlock(ref Anchor);
                        else
                        {
                            SelStart = CurPos;
                            SelEnd = CurPos;
                        }
                        if (FirstPos > CurPos)
                            FirstPos = CurPos;
                        int I = CurPos - Size.X + 2;
                        if (FirstPos < I)
                            FirstPos = I;
                        DrawView();
                        ClearEvent(ref Event);
                        break;
                }
            }
        }

        public override void SetState(StateFlags AState, bool Enable)
        {
            base.SetState(AState, Enable);
            if ((AState == StateFlags.Selected) ||
                ((AState == StateFlags.Active) && ((State & StateFlags.Selected) != 0)))
                SelectAll(Enable);
            else
                if (AState == StateFlags.Focused)
                    DrawView();
        }

    }
}
