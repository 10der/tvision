using System;
using System.IO;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.StdDlg;

namespace TurboVision.FileDialogs
{
	public class FileInputLine : InputLine
	{
		public FileInputLine( Rect Bounds, int AMaxLen):base(Bounds, AMaxLen)
		{
			EventMask |= EventMasks.evBroadcast;
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			if( (Event.What == Event.Broadcast) && 
				( Event.Command == StdDialog.cmFileFocused) &&
				( (State & StateFlags.Selected) == 0))
			{
				if( ( (new System.IO.FileInfo( Event.InfoPtr.ToString())).Attributes & FileAttributes.Directory) != 0 )
					Data = Event.InfoPtr.ToString();
				else
					Data = System.IO.Path.GetFileName(Event.InfoPtr.ToString());
				DrawView();
			}
		}
	}
}
