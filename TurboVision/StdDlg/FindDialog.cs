using System;
using TurboVision.Dialogs;
using TurboVision.Objects;

namespace TurboVision.StdDlg
{
	public class FindDialog : Dialog
	{
		public FindDialog():base( new Rect(0, 0, 38, 12), "Find")
		{
		}

		public SItem CreateOptions()
		{
			return Cluster.NewSItem( "~C~ase sensitive", 
				Cluster.NewSItem( "~W~hole words only",
				null));

		}
	}
}
