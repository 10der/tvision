using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	public class ClickTester : StaticText
	{

        private static uint[] CClickTester = { 0x07, 0x08 };
		public bool Clicked;

		public ClickTester( Rect Bounds, string AText):base( Bounds, AText)
		{
			Clicked = false;
		}

        public override uint[] GetPalette()
		{
			return CClickTester;
		}

		public override void Draw()
		{

			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			byte C;

			if( Clicked)
				C = (byte)GetColor(2);
			else
				C = (byte)GetColor(1);

			B.FillChar( (char)' ', C, (int)Size.X);
			B.FillCStr( Text, C, 0);
			WriteLine( 0, 0, (int)Size.X, 1, B);
		}
	}
}
