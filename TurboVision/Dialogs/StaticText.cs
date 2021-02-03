using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for StaticText.
	/// </summary>
	public class StaticText : View
	{

        private static uint[] CStaticText = { 0x06 };
		public string text = "";

        public StaticText( Rect Bounds, string AText):base( Bounds)
		{
			Text = AText;
		}

		public override uint[] GetPalette()
		{
			return CStaticText;
		}

		public string Text
		{
			get
			{
				if( text != null)
					return text;
				else
					return "";
			}
			set
			{
				text = value;
				DrawView();
			}
		}

		public override void Draw()
		{
			uint Color;
			bool Center;
			int I, J, L, P, Y;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			string S;

			Color = GetColor(1);
			S = Text;
			L = S.Length - 1;

			P = 0;
			Y = 0;

			Center = false;

			while( Y < Size.Y)
			{
				B.FillChar( ' ', Color, Size.X);
				if( P <= L)
				{
					if( S[P] == 0x03)
					{
						Center = true;
						P++;
					}
					I = P;
					do
					{
						J = P;
						while( (P <= L) && (S[P] == ' '))
							P++;
						while( (P <= L) && (S[P] != ' ') && (S[P] != 0x0D))
							P++;

					}while( !((P > L) || ( P >= (I + Size.X)) || ( S[P] == 0x0D)));
					if( P > I + Size.X)
						if( J > I)
							P = J;
					else
							P = I + Size.X;
					if( Center)
						J = ( Size.X - P + I)/2;
					else
						J = 0;
					B.FillBuf( S.Substring(I), Color, (byte)(P - I), J);
					while( (P <= L) && ( S[P] == ' '))
						P++;
					if( (P <= L) && (S[P] == 0x0D))
					{
						Center = false;
						P++;
						if( (P <= L) && ( S[P] == 0x0A))
							P ++;
					}
				}
				WriteLine(0, Y, Size.X, 1, B);
				Y ++;
			}
		}
	}
}
