using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.App;

namespace TurboVision.StdDlg.DataBase
{
	public class LoginDialog : Dialog
	{
		protected InputLine NameLine = null;
        protected PasswordInput PwdLine = null;
        protected Label NameLabel = null;
        protected Label PwdLabel = null;

	        public string UserId
		{
			get
			{
				return NameLine.Data;
			}
			set
			{
				NameLine.Data = value;
                if (NameLine.Data.Trim() != "")
                    PwdLine.Focus();
            }
		}

		public string Password
		{
			get
			{
				return PwdLine.Data;
			}
		}
		
		public LoginDialog():base( new Rect( 0, 0, 30, 10), "Login")
		{
			Options |= OptionFlags.ofCentered;
			NameLine = new InputLine( new Rect( 2, 2, 28, 3), 128);
			Insert( NameLine);
			PwdLine = new PasswordInput( new Rect( 2, 5, 28, 6), 128);
			Insert( PwdLine);
			NameLabel = new Label( new Rect( 2, 1, 7, 2), "~N~ame", NameLine);
			Insert( NameLabel);
			PwdLabel = new Label( new Rect( 2, 4, 11, 5), "~P~assword", PwdLine);
			Insert( PwdLabel);
			OkButton OkButton = new OkButton( new Rect( 1, 7, 14, 9));
            OkButton.GrowMode = GrowModes.gfGrowHiY | GrowModes.gfGrowLoY;
			Insert( OkButton);
            CancelButton CancelButton = new CancelButton(new Rect(15, 7, 29, 9));
            CancelButton.GrowMode = GrowModes.gfGrowHiY | GrowModes.gfGrowLoY;
            Insert( CancelButton);
            OkButton.Select();
        }

        public static bool Show(ref string uid, ref string pwd, System.Type loginDialogType)
        {
            LoginDialog dlg = (LoginDialog)loginDialogType.GetConstructor(new System.Type[] { }).Invoke(new object[] { });
            dlg.NameLine.Data = uid;
            dlg.PwdLine.Data = pwd;
            {
                if (Application.Desktop.ExecView(dlg) == cmOk)
                {
                    uid = dlg.UserId;
                    pwd = dlg.Password;
                    dlg.Done();
                    return true;
                }
                else
                {
                    dlg.Done();
                    return false;
                }
            }
        }

        public static bool Show(ref string uid, ref string pwd)
        {
            return Show(ref uid, ref pwd, typeof(LoginDialog));
        }
	}
}