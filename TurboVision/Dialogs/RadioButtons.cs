using System;
using TurboVision.Objects;
using TurboVision.Dialogs;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for RadioButtons.
	/// </summary>
	public class RadioButtons : Cluster
	{
		public RadioButtons( Rect Bounds, SItem AStrings):base( Bounds, AStrings)
		{
		}

		public override bool Mark( int Item)
		{
			return Item == value;
		}

		public override void Draw()
		{
			const string Button = " ( ) ";
			DrawMultiBox( Button , "\x32\x7");
		}
	}
}
