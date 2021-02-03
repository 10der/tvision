using System;
using System.IO;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

namespace TurboVision.FileDialogs
{
	public class FileInfoPane : View
	{

        private static uint[] CInfoPane = { 0x1E };

		public FileInfo S = new FileInfo(".");

		public FileInfoPane( Rect Bounds):base( Bounds)
		{
			EventMask |= EventMasks.evBroadcast;
		}

        public override uint[] GetPalette()
		{
			return CInfoPane;
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			string D;
			string M;
			bool PM;
			byte Color;
			DateTime Time;
			string Path;
			string FmtId;
			object[] Params = new object[8];
			string Str;

			const string sDirectoryLine = " {0,-12} {1,12} {2,-20}";
			const string sFileLine = " {0,-12} {1,12:G} {2,-20}";
			string[] Month = new string[12];

			Month[0] = "Jan";
			Month[1] = "Feb";
			Month[2] = "Mar";
			Month[3] = "Apr";
			Month[4] = "May";
			Month[5] = "Jun";
			Month[6] = "Jul";
			Month[7] = "Aug";
			Month[8] = "Sep";
			Month[9] = "Oct";
			Month[10] = "Nov";
			Month[11] = "Dec";

			D = "";

			Path = (Owner as FileDialog).Directory + (Owner as FileDialog).WildCard;
			Color = (byte)GetColor(0x01);
			B.FillChar( (char)' ', Color, (int)Size.X);
			B.FillStr( Path, Color, 1);
			WriteLine( 0, 0, (int)Size.X, 1, B);
			B.FillChar( (char)' ', Color, (int)Size.X);

			if( ( S.Attributes & FileAttributes.Directory) != 0)
			{
				FmtId = sDirectoryLine;
				D = "Directory";
			}
			else
			{
				FmtId = sFileLine;
				Params[0] = S.Length;
			}
			Time = S.CreationTime;
			M = Month[Time.Month - 1];
			Params[2] = M;
			Params[3] = Time.Day;
			Params[4] = Time.Year;
			PM = ( Time.Hour >= 12);
			Time = new DateTime( Time.Year, Time.Month, Time.Day, Time.Hour % 12, Time.Minute, Time.Second);
			if( Time.Hour == 0)
				Time = new DateTime( Time.Year, Time.Month, Time.Day, 0, Time.Minute, Time.Second);
			Params[5] = Time.Hour;
			Params[6] = Time.Minute;
			if( PM )
				Params[7] = (byte)'p';
			else
				Params[7] = (byte)'a';
			if( (S.Attributes & FileAttributes.Directory) != 0)
				Str = string.Format( FmtId, 
					(S.Name + "            ").Substring(0, 12), D, Time.ToString("dd.MM.yyyy hh:mm:ss"));
			else
				Str = string.Format( FmtId, 
					(S.Name + "            ").Substring(0, 12), S.Length, Time.ToString("dd.MM.yyyy hh:mm:ss"));
			B.FillStr( Str, Color, 0);
			WriteLine( 0, 1, (int)Size.X, 1, B);
			B.FillChar( ' ', Color, (int)Size.X, 0);
			WriteLine( 0, 2, (int)Size.X, (int)(Size.Y - 2), B);
		}

		public void SetFileInfo( string fileName)
		{
			if( fileName.IndexOfAny( Path.GetInvalidFileNameChars()) == -1)
				if( fileName.IndexOfAny( new char[2]{'*','?'}) == -1)
				S = new FileInfo( fileName);
			else
					S = new FileInfo(".");
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			if( (Event.What == Event.Broadcast) &&
				( Event.Command == StdDialog.cmFileFocused))
			{
				S = new System.IO.FileInfo( Event.InfoPtr.ToString());
				DrawView();
			}
		}

	}
}
