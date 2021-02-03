using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.App
{
	/// <summary>
	/// Summary description for Background.
	/// </summary>
	public class Background : View
	{

		private const uint CBackground = 1;

		private char pattern;

		public Background( Rect Bounds, char APattern):base( Bounds)
		{
			Pattern = APattern;
			GrowMode = ( GrowModes.gfGrowHiX | GrowModes.gfGrowHiY);
		}

		public char Pattern
		{
			get
			{
				return pattern;
			}
			set
			{
				pattern = value;
			}
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
            B.FillChar((char)pattern, (byte)GetColor(1), (int)Size.X);
			WriteLine( 0, 0, (int)Size.X, (int)Size.Y, B);
		}
	}

    public class ClearBackground : Background
    {

        public ClearBackground( Rect R):base(R, '\x0000')
        {
        }

        public override void Draw()
        {
            DrawBuffer B = new DrawBuffer(Size.X * Size.Y);
            B.FillBuf(Program.SaveConsoleBuffer, (uint)(Size.X * Size.Y), 0);
            WriteBuf(0, 0, Size.X, Size.Y, B);
        }
    }
}
