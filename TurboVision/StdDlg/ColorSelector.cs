using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	public enum ColorSel
	{
		csBackground,
		csForeground,
	}

	public class ColorSelector : View
	{

		public byte Color;
		public ColorSel SelType;

		public static bool cBackgroundBlink = false;

		public ColorSelector( Rect Bounds, ColorSel ASelType):base( Bounds)
		{
			Options |= OptionFlags.ofSelectable | OptionFlags.ofFirstClick | OptionFlags.ofFramed;
			EventMask |= EventMasks.evBroadcast;
			SelType = ASelType;
			Color = 0;
		}

		public override void Draw()
		{
			byte C;

			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			B.FillChar((char)' ', 0x70, (int)Size.X);
			for( int i = 0; i <= Size.Y; i++)
			{
				if( i < 4)
					for( int j = 0; j <= 3; j++)
					{
						C = (byte)(i * 4 + j);
						B.FillChar( (char)ldBlockFull, C, 3, j*3);
						if( C == Color)
						{
							B.drawBuffer[ j*3 + 1].AsciiChar = '\x08';
							if( C == 0)
								B.drawBuffer[ j*3 + 1].Attribute = '\x70';
						}
					}
				WriteLine(0, i, (int)Size.X, 1, B);
			}
		}

	}
}
