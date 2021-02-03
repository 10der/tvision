using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Editors
{
	public class Memo : Editor
	{

        private static uint[] CMemo = { 0x1a, 0x1b };

		public Memo( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, Indicator AIndicator, int ABufSize):base( Bounds, AHScrollBar, AVScrollBar, AIndicator, ABufSize)
		{
		}

        public override uint[] GetPalette()
		{
			return CMemo;
		}
	}
}
