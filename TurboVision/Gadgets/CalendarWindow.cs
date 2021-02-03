using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class CalendarWindow : Window
	{
		public CalendarWindow():base( new Rect( 1, 1, 23, 11), "Calendar", 0)
		{
			Flags &= ~( WindowFlags.wfGrow | WindowFlags.wfZoom);
			GrowMode = 0;
			Palette = WindowPalettes.wpCyanWindow;

			Rect R = GetExtent();
			R.Grow( -1, -1);
			CalendarView V = new CalendarView( R);
			Insert(V);
		}
	}
}
