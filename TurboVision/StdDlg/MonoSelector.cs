using System;
using TurboVision.Objects;
using TurboVision.Dialogs;

namespace TurboVision.StdDlg
{
	/// <summary>
	/// Summary description for MonoSelector.
	/// </summary>
	public class MonoSelector : Cluster
	{
		private byte[] MonoColors;

		public MonoSelector( Rect Bounds):base( Bounds,
			NewSItem("Normal",
			NewSItem("Highlight",
			NewSItem("Underline",
			NewSItem("Inverse", null)))))
		{
			MonoColors = new byte[5];
			MonoColors[0] = 0x07;
			MonoColors[1] = 0x0F;
			MonoColors[2] = 0x01;
			MonoColors[3] = 0x70;
			MonoColors[4] = 0x09;
		}

		public override bool Mark( int Item)
		{
			return MonoColors[Item] == value;
		}

		public SItem CreateOptions()
		{
			return
				NewSItem("Normal",
				NewSItem("Highlight",
				NewSItem("Underline",
				NewSItem("Inverse",
				null))));
		}

		public override void Draw()
		{
			string Button = " ( ) ";
			DrawBox( Button, '\x07');
		}
	}
}
