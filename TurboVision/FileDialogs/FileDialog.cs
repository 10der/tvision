using System;
using TurboVision.Dialogs;
using TurboVision.FileDialogs;
using TurboVision.History;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

namespace TurboVision.FileDialogs
{
	/// <summary>
	/// Summary description for FileDialog.
	/// </summary>
	public class FileDialog : Dialog
	{
		public int cmFileClear = 802;
		public int cmFileInit = 803;

		public string Directory;
		public string WildCard;
		public FileList FileList;
		public FileInputLine FileName;

		public FileDialog( string AWildCard, string ATitle, string InputName, StdDlg.StdDialogOptions AOptions, byte HistoryId)
			:base( new Rect(15, 1, 64, 20), ATitle)
		{
			Options |=  OptionFlags.ofCentered;
			WildCard = AWildCard;
			Rect R = new Rect( 3, 3, 31, 4);
			FileName = new FileInputLine( R, 79);
            FileName.GrowMode = GrowModes.gfGrowHiX;
			FileName.Data = WildCard;
			Insert( FileName);
			R = new Rect( 2, 2, 3 + Drivers.CStrLen( InputName), 3);
			View Control = new Label( R, InputName, FileName);
			Insert( Control);
			R = new Rect( 31, 3, 34, 4);
			Control = new History.History( R, FileName, HistoryId);
            Control.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
			Insert( Control);
			R = new Rect( 3, 14, 34, 15);
			Control = new ScrollBar( R);
			Insert( Control);
			R = new Rect( 3, 6, 34, 14);
			FileList = new FileList( R, (ScrollBar)Control);
            FileList.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;

			Insert( FileList);
			R = new Rect( 2, 5, 8, 6);
			Control = new Label( R, "~F~iles", FileList);
			Insert( Control);

			R = new Rect( 35, 3, 46, 5);
			Button.ButtonFlags Opt = Button.ButtonFlags.Default;
			if( ( AOptions & StdDialogOptions.fdOpenButton) != 0)
			{
				View V = new Button( R, "~O~pen", StdDialog.cmFileOpen, Opt);
                V.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
				Insert( V);
				Opt = Button.ButtonFlags.Normal;
				R.A.Y += 3;
				R.B.Y += 3;
			}
			if( ( AOptions & StdDialogOptions.fdReplaceButton) != 0)
			{
				View V = new Button( R, "~R~eplace", StdDialog.cmFileReplace, Opt);
                V.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
				Insert( V);
				Opt = Button.ButtonFlags.Normal;
				R.A.Y += 3;
				R.B.Y += 3;
			}
			if( ( AOptions & StdDialogOptions.fdClearButton) != 0)
			{
				View V = new Button( R, "~C~lear", StdDialog.cmFileClear, Opt);
                V.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
				Insert( V);
				R.A.Y += 3;
				R.B.Y += 3;
			}
			View VV = new Button( R, "Cancel", cmCancel, Opt);
            VV.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
			Insert( VV);
			R.A.Y += 3;
			R.B.Y += 3;
			if( ( AOptions & StdDialogOptions.fdHelpButton) != 0)
			{
				View V = new Button( R, "Help", cmHelp, Button.ButtonFlags.Normal);
                V.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoX;
				Insert( V);
				R.A.Y += 3;
				R.B.Y += 3;
			}
			R = new Rect( 1, 16, 48, 18);
			Control = new FileInfoPane( R);
            Control.GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowLoY | GrowModes.gfGrowHiY;
			Insert( Control);

			SelectNext( false);

			if( (AOptions & StdDialogOptions.fdNoLoadDir) == 0)
				ReadDirectory();
		}

		public void ReadDirectory()
		{
			Directory = System.IO.Directory.GetCurrentDirectory();
			FileList.ReadDirectory( Directory, WildCard);
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			if( Event.What == Event.evCommand)
				switch( Event.Command)
				{
					case StdDialog.cmFileOpen :
					case StdDialog.cmFileReplace :
					case StdDialog.cmFileClear :
						EndModal( Event.Command);
						ClearEvent( ref Event);
						break;
				}
		}

		public string GetFileName()
		{
			string S = FileName.Data;
			return S;
		}

		public bool IsWild( string Name)
		{
			return Name.IndexOfAny( new char[2]{'*', '?'}) != -1;
		}

		public override bool Valid( int Command)
		{
			if( Command == 0)
				return true;
			bool Valid = false;
			if( base.Valid( Command))
			{
				if( ( Command != cmCancel) && ( Command != cmFileClear))
				{
					if( IsWild( FileName.Data))
					{
						System.IO.DirectoryInfo di = new System.IO.DirectoryInfo( Directory);
						if( di.Exists)
						{
							WildCard = FileName.Data;
							if( Command != cmFileInit)
								FileList.Select();
							FileList.ReadDirectory( Directory, WildCard);
						}
					}
					else
					{
						string pp = System.IO.Path.Combine( Directory, FileName.Data);
						System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(pp);
						if( ( di.Attributes & System.IO.FileAttributes.Directory) != 0)
						{
							if( di.Exists)
							{
								Directory = GetFileName();
								if( Command != cmFileInit)
									FileList.Select();
								FileList.ReadDirectory( Directory, WildCard);
							}
						}
						else if( System.IO.File.Exists( Directory + @"\" + FileName.Data))
						{
							Valid = true;
						}
						else
						{
							MsgBox.MessageBox("Invalid file name.");
							Valid = false;
						}
					}
				}
				else 
				{
					Valid = true;
				}
			}
			return Valid;
		}
        }
}
