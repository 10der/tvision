using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	public class MouseDialog : Dialog
	{
		public ScrollBar MouseScrollBar;
		public int OldDelay;

		public MouseDialog():base( new Rect( 0, 0, 34, 12), "Mouse options")
		{
			Rect R;
			View V;
			Options |= OptionFlags.ofCentered;

			R = new Rect(3, 4, 30, 5);
			MouseScrollBar = new ScrollBar(R);
			MouseScrollBar.SetParams(1, 1, 20, 20, 1);
			MouseScrollBar.Options |= OptionFlags.ofSelectable;
			MouseScrollBar.SetValue( W32Kbd.DoubleDelay);
			Insert(MouseScrollBar);
			R = new Rect(2, 2, 21, 3);
			V = new Label(R, "~M~ouse double click", MouseScrollBar);
			Insert( V);

			R = new Rect(3, 3, 30, 4);
			V = new ClickTester(R, "Fast       Medium      Slow");
			Insert( V);

			R = new Rect(3, 6, 30, 7);
			V = new CheckBoxes(R,
					 CheckBoxes.NewSItem("~R~everse mouse buttons", null));
			Insert( V);

			OldDelay = W32Kbd.DoubleDelay;

			R = new Rect(9, 9, 19, 11);
			V = new Button(R, "O~K~", cmOk, Button.ButtonFlags.Default);
			Insert( V);
			R.A.X+=12; R.B.X+=12;
			V = new Button(R, "Cancel", cmCancel, Button.ButtonFlags.Normal);
			Insert( V);

			SelectNext(false);
		}

		public SItem CreateOptions()
		{
			return
				Cluster.NewSItem("~R~everse mouse buttons", null);
		}
	}
}
