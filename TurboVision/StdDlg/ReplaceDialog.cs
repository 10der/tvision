using System;
using TurboVision.Dialogs;
using TurboVision.Objects;

namespace TurboVision.StdDlg
{
	public class ReplaceDialog : Dialog
	{
		public ReplaceDialog():base( new Rect( 0, 0, 40, 16), "Replace")
		{
		}

		public SItem CreateOptions()
		{
			return
				Cluster.NewSItem("~C~ase sensitive",
				Cluster.NewSItem("~W~hole words only",
				Cluster.NewSItem("~P~rompt on replace",
				Cluster.NewSItem("~R~eplace all",
				null))));
		}
	}
}
