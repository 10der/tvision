using System;
using TurboVision.Dialogs;
using TurboVision.App;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	[Flags]
	public enum MessageBoxFlags
	{
		/// <summary>
		/// Display a Warning box
		/// </summary>
		mfWarning      = 0x0000,
		/// <summary>
		/// Display a Error box
		/// </summary>
		mfError        = 0x0001,
		/// <summary>
		/// Display an Information Box
		/// </summary>
		mfInformation  = 0x0002,
		/// <summary>
		/// Display a Confirmation Box
		/// </summary>
		mfConfirmation = 0x0003,

		/// <summary>
		/// Put a Yes button into the dialog
		/// </summary>
		mfYesButton    = 0x0100,
		/// <summary>
		/// Put a No button into the dialog
		/// </summary>
		mfNoButton     = 0x0200,
		/// <summary>
		/// Put an OK button into the dialog
		/// </summary>
		mfOKButton     = 0x0400,
		/// <summary>
		/// Put a Cancel button into the dialog
		/// </summary>
		mfCancelButton = 0x0800,

		/// <summary>
		/// Insert message box into application
		/// </summary>
		mfInsertInApp  = 0x0080,
			
		/// <summary>
		/// Insert message box into application
		/// </summary>
		mfCentered  = 0x0080,

		/// <summary>
		/// Standard Yes, No, Cancel dialog
		/// </summary>
		mfYesNoCancel  = mfYesButton + mfNoButton + mfCancelButton,
	}

	public class MsgBox
	{
		public MsgBox()
		{
		}

        public static int MessageBox(string Msg)
        {
			return MessageBox( Msg, MessageBoxFlags.mfOKButton | MessageBoxFlags.mfCancelButton, new object[0]);
		}

        public static int MessageBox(string Msg, MessageBoxFlags AOptions)
        {
            return MessageBox(Msg, AOptions, new object[0]);
        }

        public static int MessageBox(string Msg, MessageBoxFlags AOptions, params object[] Params)
        {
			Rect R = new Rect( 0, 0, 40, 12);
			if( (AOptions & MessageBoxFlags.mfInsertInApp) == 0)
				R.Move( (Program.Desktop.Size.X - R.B.X) / 2, ( Program.Desktop.Size.Y - R.B.Y) / 2);
			else
				R.Move( (Program.Application.Size.X - R.B.X) / 2, ( Program.Application.Size.Y - R.B.Y) / 2);
			return MessageBoxRect( R, Msg, AOptions, Params);
		}

        public static int MessageBoxRect(Rect R, string Msg, MessageBoxFlags AOptions, params object[] Params)
		{
			string[] ButtonName = new string[4]{"~Y~es", "~N~o", "O~K~", "Cancel"};
			int[] Commands = new int[4]{ View.cmYes, View.cmNo, View.cmOk, View.cmCancel};
			string[] Titles = new string[4]{"Warning","Error","Information","Confirm"};

			int X, ButtonCount;
			Dialog Dialog;
			View Control;
			View[] ButtonList = new View[4];
			string S;

			Dialog = new Dialog( R, Titles[(int)AOptions & 0x03]);
			R = new Rect( 3, 2, Dialog.Size.X - 2, Dialog.Size.Y - 3);
			S = string.Format( Msg, Params);
			Control = new StaticText( R, S);
			Dialog.Insert( Control);
			X = -2;
			ButtonCount = 0;
			for( int i = 0; i < 4; i++)
				if( ( (int)AOptions & ( 0x0100 << i)) != 0)
				{
					R = new Rect( 0, 0, 12, 2);
					Control = new Button( R, ButtonName[i], Commands[i], Button.ButtonFlags.Normal);
					X += Control.Size.X + 2;
					ButtonList[ButtonCount] = Control;
					ButtonCount++;
				}
			X = ( Dialog.Size.X - X) >> 1;
			for( int i = 0; i < ButtonCount; i++)
			{
				Control = ButtonList[i];
				Dialog.Insert( Control);
				Control.MoveTo( X, Dialog.Size.Y - 3);
				X += Control.Size.X + 2;
			}
			Dialog.SelectNext( false);
			if( (AOptions & MessageBoxFlags.mfCentered) != 0)
				Dialog.Options |= View.OptionFlags.ofCentered;
			if( (AOptions & MessageBoxFlags.mfInsertInApp) == 0)
				return Program.Desktop.ExecView( Dialog);
			else
				return Program.Application.ExecView( Dialog);
		}
	}
}
