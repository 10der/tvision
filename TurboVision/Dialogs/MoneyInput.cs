#region Using directives

using System;
using TurboVision.Objects;
using TurboVision.Views;

#endregion

namespace TurboVision.Dialogs
{
    public class MoneyInput : View
    {

        private static uint[] CMoneyInput = { 0x13, 0x13, 0x14, 0x15 };
        private int CurrentDecimals = -1;

        public decimal Value = 0;
        private int prec;

        public MoneyInput( Rect R, int Len, int Prec):base( R)
        {
            Options |= OptionFlags.ofSelectable;
            prec = Prec;
            SetCursor(Size.X - 2, 0);
        }

        public override void Draw()
        {
            DrawBuffer B = new DrawBuffer(Size.X * Size.Y);
            string DisplayString = "";
            if (CurrentDecimals == 0)
            {
                DisplayString = string.Format("{0," + (Size.X - 2).ToString() + ":N0}" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator+ " ", Value);
            }
            else if (CurrentDecimals < 0)
                DisplayString = string.Format("{0," + (Size.X - 1).ToString() + ":N0} ", Value);
            else
                DisplayString = string.Format("{0," + (Size.X - 1).ToString() + ":N" + CurrentDecimals.ToString() + "} ", Value);
            if ((State & StateFlags.Selected) != 0)
            {
                B.FillStr(DisplayString, (byte)GetColor(1), 0);
                ShowCursor();
            }
            else
            {
                B.FillStr(DisplayString, (byte)GetColor(1), 0);
                HideCursor();
            }
            WriteLine(0, 0, (int)Size.X, (int)Size.Y, B);
        }


        public override void SetState(View.StateFlags AState, bool Enable)
        {
            base.SetState( AState, Enable);
            if ( (AState & StateFlags.Selected) != 0)
                DrawView();
        }

        public override uint[] GetPalette()
        {
            return CMoneyInput;
        }

        private void SetValue(char c)
        {
            if (c == '\x0008')
            {
                if (CurrentDecimals >= 0)
                    CurrentDecimals--;
                else
                    Value = decimal.Floor( Value / 10);
            }
            else if ( c == '.' && CurrentDecimals < 0)
                CurrentDecimals = 0;
            else if (Value == 0 && c != '.')
                Value = (int.Parse(new string(c, 1)));
            else if (Value > 0 && CurrentDecimals == -1 && c != '.')
                Value = Value * 10 + (int.Parse(new string(c, 1)));
            else if (CurrentDecimals >= 0 && c != '.')
            {
                if (CurrentDecimals < prec)
                {
                    Value = Value + decimal.Parse(new string(c, 1)) / (decimal)Math.Pow(10, CurrentDecimals + 1);
                    CurrentDecimals++;
                }
            }
            DrawView();
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent( ref Event);
            if( Event.What == Event.KeyDown)
            {
                if (Event.KeyCode == KeyboardKeys.Del)
                    Event.CharCode = 8;
                switch( Event.CharCode)
                {
                    case 8 :
                        SetValue('\x0008');
                        ClearEvent(ref Event);
                        break;
                    case 46:
                        SetValue('.');
                        ClearEvent(ref Event);
                        break;
                    case 48:
                        SetValue('0');
                        ClearEvent(ref Event);
                        break;
                    case 49:
                        SetValue('1');
                        ClearEvent(ref Event);
                        break;
                    case 50:
                        SetValue('2');
                        ClearEvent(ref Event);
                        break;
                    case 51:
                        SetValue('3');
                        ClearEvent(ref Event);
                        break;
                    case 52:
                        SetValue('4');
                        ClearEvent(ref Event);
                        break;
                    case 53:
                        SetValue('5');
                        ClearEvent(ref Event);
                        break;
                    case 54:
                        SetValue('6');
                        ClearEvent(ref Event);
                        break;
                    case 55:
                        SetValue('7');
                        ClearEvent(ref Event);
                        break;
                    case 56:
                        SetValue('8');
                        ClearEvent(ref Event);
                        break;
                    case 57:
                        SetValue('9');
                        ClearEvent(ref Event);
                        break;
                }
            }
        }
    }
}
