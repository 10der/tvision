using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class Report : View
	{

		public const int AsciiTableCommandBase = 910;
		public const int cmCharacterFocused = 0;

		public long ASCIIChar;

		public Report( Rect Bounds):base( Bounds)
		{
		}

		public override void Draw()
		{
			string TempStr = string.Format( " Char: {0} Decimal: {1,3:G}  Hex: {1,2:X}  ", (char)ASCIIChar, ASCIIChar);
			WriteStr( 0, 0, TempStr, 6);
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			if( Event.What == Event.Broadcast)
				if( Event.Command == ( AsciiTableCommandBase | cmCharacterFocused))
				{
					ASCIIChar = (int)Event.InfoPtr;
					DrawView();
				}
		}

	}
}
