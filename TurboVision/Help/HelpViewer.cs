using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Help
{
	public class HelpViewer : Scroller
	{

        private static uint[] CHelpViewer = { 0x06, 0x07, 0x08 };

		public HelpViewer( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, HelpFile HelpFile, uint Context):base( Bounds, AHScrollBar, AVScrollBar)
		{
		}

        public override uint[] GetPalette()
		{
			return CHelpViewer;
		}
	}
}
