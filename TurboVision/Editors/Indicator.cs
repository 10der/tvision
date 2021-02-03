using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Editors
{
	public class Indicator : View
	{

        private static uint[] CIndicator = { 0x02, 0x03 };

		public bool Modified;
		public Point Location;

		public Indicator( Rect Bounds):base( Bounds)
		{
            GrowMode = GrowModes.gfGrowLoY | GrowModes.gfGrowHiY;
        }

        public override uint[] GetPalette()
		{
			return CIndicator;
		}

		public override void Draw()
		{

			byte Color;
			char Frame;
			long[] L = new long[2];
			string S;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			if( (State & StateFlags.Dragging) == 0)
			{
				Color = (byte)GetColor(1);
				Frame = ldDblHorizontalBar;
			}
			else
			{
				Color = (byte)GetColor(2);
				Frame = ldHorizontalBar;
			}
			B.FillChar( (char)Frame, Color, (int)Size.X);
			if( Modified)
				B.drawBuffer[0].AsciiChar = (char)ldModified;
			L[0] = Location.Y + 1;
			L[1] = Location.X + 1;
			S = string.Format("{0:G}:{1:G}", L[0], L[1]);
			B.FillStr( S, Color, 8 - S.IndexOf(':',0) + 1);
			WriteBuf(0, 0, (int)Size.X, 1, B);
		}

		public void SetValue( Point ALocation, bool AModified)
		{
			if( (Location.X != ALocation.X) || ( Location.Y != ALocation.Y) || ( Modified != AModified))
			{
				Location = ALocation;
				Modified = AModified;
				DrawView();
			}
		}

        public override void SetState(View.StateFlags AState, bool Enable)
        {
            base.SetState( AState, Enable);
            if (AState == StateFlags.Dragging)
                DrawView();
        }
    }
}
