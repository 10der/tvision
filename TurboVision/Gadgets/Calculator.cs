using System;
using TurboVision.Dialogs;
using TurboVision.Objects;

namespace TurboVision.Gadgets
{
	public class Calculator : Dialog
	{
		public Calculator():base( new Rect( 5, 3, 29, 18), "Calculator")
		{
			const int cmCalcButton = 100;
			string KeyChar = "\x43\x1B\x25\xF1\x37\x38\x39\x2F\x34\x35\x36\x2A\x31\x32\x33\x2D\x30\x2E\x3D\x2B";

			Options |= OptionFlags.ofFirstClick;

			Rect R = GetBounds();

			for( int i = 0; i< 20; i++)
			{
				R.A.X = (i % 4) * 5 + 2;
				R.A.Y = (i / 4) * 2 + 4;
				R.B.X = R.A.X + 5;
				R.B.Y = R.A.Y + 2;
				Button P = new Button(R, new string(KeyChar[i], 1), cmCalcButton, Button.ButtonFlags.Normal | Button.ButtonFlags.Broadcast);
				P.Options &= ~( OptionFlags.ofSelectable);
				Insert( P);
			}
			Insert( 
				new CalcDisplay( new Rect(2, 2, 21, 3)));
		}
	}
}
