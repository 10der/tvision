using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class AsciiChart : Window
	{
		public AsciiChart():base( new Rect( 0, 0, 34, 12), "ASCII Chart", wnNoNumber)
		{
			Flags &= ~( WindowFlags.wfGrow | WindowFlags.wfZoom);
			Palette = WindowPalettes.wpGrayWindow;
			Rect R = GetBounds();
			R.Grow( -1, -1);
			R.A.Y = R.B.Y - 1;
			View Control = new Report( R);
			Control.Options |= OptionFlags.ofFramed;
			Control.EventMask |= EventMasks.evBroadcast;
			Insert( Control);
			R = GetExtent();
			R.Grow( -1, -1);
			R.B.Y -=2;
			Control = new Table( R);
			Control.Options |= ( OptionFlags.ofFramed | OptionFlags.ofSelectable);
			Control.BlockCursor();
			Insert( Control);
			Control.Select();
		}
	}
}
