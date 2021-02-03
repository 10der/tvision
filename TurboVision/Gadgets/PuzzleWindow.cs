using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class PuzzleWindow : Window
	{
		internal static Rect GetPuzzleWindowBounds()
		{
			return new Rect(1, 1, 21, 7);
		}

		public PuzzleWindow():base( GetPuzzleWindowBounds(), "Puzzle", 0)
		{
			Flags &= ~( WindowFlags.wfZoom | WindowFlags.wfGrow);
			GrowMode = 0;
			Rect R = GetExtent();
			R.Grow( -1, -1);
			View V = new PuzzleView( R);
			Insert( V);
		}
	}
}
