#region Using directives

using System;
#if FRAMEWORK20
using System.Collections.Generic;
#endif
using System.Collections;
using System.Text;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

#endregion

namespace TurboVision.Dialogs
{
    public class DateInput : View
    {
        private static uint[] CDateInput = { 0x13, 0x13, 0x14, 0x15 };

        int Year = DateTime.Now.Year;
        int Month = DateTime.Now.Month;
        int Day = DateTime.Now.Day;

        public override object GetData()
        {
            try
            {
                DateTime Value = new DateTime(Year, Month, Day);
                return Value;
            }
            catch
            {
                MsgBox.MessageBox("\x000D\x0003Некорректный формат даты !", MessageBoxFlags.mfError | MessageBoxFlags.mfCancelButton);
                throw;
            }
        }

        public override void SetData(object Rec)
        {
            /*
            if (Rec[0].GetType() == typeof(DateTime))
            {
                Year = ((DateTime)Rec[0]).Year;
                Month = ((DateTime)Rec[0]).Month;
                Day = ((DateTime)Rec[0]).Day;
            }
            else if (Rec[0].GetType() == typeof(int) ||
                Rec[1].GetType() == typeof(int) ||
                Rec[2].GetType() == typeof(int))
            {
                Year = (int)Rec[0];
                Month = (int)Rec[1];
                Day = (int)Rec[2];
            }
             */
        }

        public DateInput(Rect R)
            : base(R)
        {
            Options |= OptionFlags.ofSelectable;
            BlockCursor();
            SetCursor(1, 0);
        }

        public override void SetState(View.StateFlags AState, bool Enable)
        {
            base.SetState(AState, Enable);
            if ((AState & StateFlags.Selected) != 0)
            {
                SetCursor(1, 0);
                DrawView();
            }
        }

        public override uint[] GetPalette()
        {
            return CDateInput;
        }

        public override void Draw()
        {
            DrawBuffer B = new DrawBuffer(Size.X * Size.Y);
            string DisplayString = string.Format(" {0,2:D2}.{1,2:D2}.{2,4:D4} ", Day, Month, Year);
            B.FillStr(DisplayString, (byte)GetColor(1), 0);
            if ((State & StateFlags.Selected) != 0)
                ShowCursor();
            else
                HideCursor();
            WriteLine(0, 0, (int)Size.X, (int)Size.Y, B);
        }

        private void MoveCursorRight()
        {
            if (Cursor.X < Size.X - 1)
                SetCursor(Cursor.X + 1, Cursor.Y);
            if (Cursor.X == 3 || Cursor.X == 6)
                SetCursor(Cursor.X + 1, Cursor.Y);
        }

        private void MoveCursorLeft()
        {
            if (Cursor.X > 1)
                SetCursor(Cursor.X - 1, Cursor.Y);
            if (Cursor.X == 3 || Cursor.X == 6)
                SetCursor(Cursor.X - 1, Cursor.Y);
        }

        private void SetChar(char c)
        {
            if (Cursor.X == Size.X - 1)
                return;
            StringBuilder sb = new StringBuilder(string.Format(" {0,2:D2}.{1,2:D2}.{2,4:D4} ", Day, Month, Year));
            sb[Cursor.X] = c;
            string s = sb.ToString();
            Day = int.Parse(s.Substring(1, 2));
            Month = int.Parse(s.Substring(4, 2));
            Year = int.Parse(s.Substring(7, 4));
            MoveCursorRight();
            DrawView();
        }

        public override bool Valid(int Command)
        {
            if (Command == cmOk)
                return CheckLeavePossiblity();
            else
                return true;
        }

        protected virtual bool CheckLeavePossiblity()
        {
            try
            {
                DateTime dt = new DateTime(Year, Month, Day);
                return true;
            }
            catch
            {
                MsgBox.MessageBox("\x000D\x0003Некорректный фрмат даты !", MessageBoxFlags.mfError | MessageBoxFlags.mfCancelButton);
                return false;
            }
        }

        private void ClearLeft()
        {
            MoveCursorLeft();
            StringBuilder sb = new StringBuilder(string.Format(" {0,2:D2}.{1,2:D2}.{2,4:D4} ", Day, Month, Year));
            sb[Cursor.X] = '0';
            string s = sb.ToString();
            Day = int.Parse(s.Substring(1, 2));
            Month = int.Parse(s.Substring(4, 2));
            Year = int.Parse(s.Substring(7, 4));
            DrawView();
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent(ref Event);
            if (Event.What == Event.KeyDown)
            {
                switch (Event.KeyCode)
                {
                    case KeyboardKeys.Right:
                        MoveCursorRight();
                        ClearEvent(ref Event);
                        break;
                    case KeyboardKeys.Left:
                        MoveCursorLeft();
                        ClearEvent(ref Event);
                        break;
                    case KeyboardKeys.Tab:
                        if (!CheckLeavePossiblity())
                            ClearEvent(ref Event);
                        break;
                }
            }
            if (Event.What == Event.Keyboard)
            {
                switch (Event.CharCode)
                {
                    case 8:
                        ClearLeft();
                        break;
                    case 48:
                        SetChar('0');
                        ClearEvent(ref Event);
                        break;
                    case 49:
                        SetChar('1');
                        ClearEvent(ref Event);
                        break;
                    case 50:
                        SetChar('2');
                        ClearEvent(ref Event);
                        break;
                    case 51:
                        SetChar('3');
                        ClearEvent(ref Event);
                        break;
                    case 52:
                        SetChar('4');
                        ClearEvent(ref Event);
                        break;
                    case 53:
                        SetChar('5');
                        ClearEvent(ref Event);
                        break;
                    case 54:
                        SetChar('6');
                        ClearEvent(ref Event);
                        break;
                    case 55:
                        SetChar('7');
                        ClearEvent(ref Event);
                        break;
                    case 56:
                        SetChar('8');
                        ClearEvent(ref Event);
                        break;
                    case 57:
                        SetChar('9');
                        ClearEvent(ref Event);
                        break;
                }
            }
        }

    }
}
