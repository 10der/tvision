using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class CalcDisplay : View
	{
		public enum CalcState
		{
			csFirst, csValid, csError,
		}

		public string Number = "";
		public char Sign;
		public CalcState Status;
		public char Operator;
		public float Operand;

		public const int cmCalcButton = 100;

		internal void Error()
		{
			Status = CalcState.csError;
			Number = "Error";
			Sign = ' ';
		}

		internal void SetDisplay( float R)
		{
			string S;
			System.Globalization.CultureInfo SaveCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			try
			{
				S = string.Format("{0,10:G}", R);
			}
			catch
			{
				S = "";
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = SaveCulture;
			}
			if( S[0] != '-')
				Sign = ' ';
			else
			{
				S = S.Remove( 0, 1);
				Sign = '-';
			}
			if( S.Length > ( 15 + 1 + 10))
				Error();
			else
			{
				// tender

                //while( S.EndsWith("0"))
                //    S = S.Substring( 0, S.Length - 1);

                //if( S.EndsWith("."))
                //    S = S.Substring( 0, S.Length - 1);

				Number = S;
			}
		}

		internal void GetDisplay( out float R)
		{
			System.Globalization.CultureInfo SaveCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			try
			{
				R = float.Parse( Sign + Number);
			}
			catch
			{
				R = 0;
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = SaveCulture;
			}
		}

		internal void CheckFirst()
		{
			if( Status == CalcState.csFirst)
			{
				Status = CalcState.csValid;
				Number = "0";
				Sign = ' ';
			}
		}

		public void CalcKey( char Key)
		{
			float R;

			Key = char.ToUpper( Key);
			if( (Status == CalcState.csError) && ( Key != 'C'))
				Key = ' ';
			switch( Key)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					CheckFirst();
					if( Number == "0")
						Number = "";
					Number = Number + Key;
					break;
				case '.':
					CheckFirst();
					if( Number.IndexOf('.') == -1)
						Number = Number + '.';
					break;
				case '\x08':
				case '\x1B':
					CheckFirst();
					if( Number.Length == 1)
						Number = "0";
					else
						Number = Number.Substring( 0, Number.Length - 1);
					break;
				case '_':
				case '\xF1':
					CheckFirst();
					if( Sign == ' ')
						Sign = '-';
					else
						Sign = ' ';
					break;
				case '+':
				case '-':
				case '*':
				case '/':
				case '=':
				case '%':
				case '\x0D':
				{
					if( Status == CalcState.csValid)
					{
						Status = CalcState.csFirst;
						GetDisplay( out R);
						if( Key == '%')
							switch( Operator)
							{
								case '+':
								case '-':
									R = Operand * R / 100;
									break;
								case '*':
								case '/':
									R = R / 100;
									break;
							}
						switch( Operator)
						{
							case '+': SetDisplay( Operand + R);
								break;
							case '-': SetDisplay( Operand - R);
								break;
							case '*': SetDisplay( Operand * R);
								break;
							case '/': if ( R == 0)
										  Error();
										else
										  SetDisplay( Operand / R);
								break;
						}
					}
					Operator = Key;
					GetDisplay( out Operand);
				}
					break;
				case 'C':
					Clear();
					break;
			}
			DrawView();
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			switch( Event.What)
			{
				case Event.KeyDown :
					CalcKey( (char)Event.CharCode);
					ClearEvent( ref Event);
					break;
				case Event.Broadcast :
					if( Event.Command == cmCalcButton)
					{
						CalcKey( ( Event.InfoPtr as Button).Title[0]);
						ClearEvent( ref Event);
					}
					break;
			}
		}


		public CalcDisplay( Rect Bounds):base( Bounds)
		{
			Options |= OptionFlags.ofSelectable;
			EventMask = ( EventMasks.evKeyDown | EventMasks.evBroadcast);
			Clear();
		}
		
		public override void Draw()
		{
			byte Color;
			int I;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			Color = (byte)GetColor(1);
			I = (int)( Size.X - Number.Length - 2);
			B.FillChar( ' ', Color, (int)Size.X);
			B.FillChar( Sign, Color, 1, I);
			B.FillStr( Number, Color, I + 1);
			WriteBuf( 0, 0, (int)Size.X, 1, B);
		}

		public void Clear()
		{
			Status = CalcState.csFirst;
			Number = "0";
			Sign = ' ';
			Operator = '=';
		}

        public override uint[] GetPalette()
		{
            return new uint[] { 0x13 };
		}
	}
}
