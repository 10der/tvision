using System;
using System.IO;
using System.Collections;
using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.FileDialogs;
using TurboVision.StdDlg;
using TurboVision.Views;

namespace TurboVision.FileDialogs
{

	public class FileList : SortedListBox
	{
        public const int cmFileFocused = 806;
		
		public FileList( Rect Bounds, ScrollBar AScrollBar):base( Bounds, 2, AScrollBar)
		{
		}

		public override string GetText( int Item, int MaxLen)
		{
			return System.IO.Path.GetFileName( List[Item].ToString());
		}

		public virtual void ReadDirectory( string ADirectory, string AWildCard)
		{
			ArrayList FileList = new ArrayList();

			string[] Files = System.IO.Directory.GetFileSystemEntries( ADirectory, AWildCard); 
			foreach( string s in Files)
			{
				System.IO.FileInfo f = new System.IO.FileInfo( s);
				if( (f.Attributes & FileAttributes.Directory) == 0)
					FileList.Add( f);
			}
			Files = System.IO.Directory.GetFileSystemEntries( ADirectory, "*.*"); 
			foreach( string s in Files)
			{
				System.IO.FileInfo f = new System.IO.FileInfo( s);
				if( (f.Attributes & FileAttributes.Directory) != 0)
					FileList.Add( f);
			}
			FileList.Add( new System.IO.FileInfo( ".."));
			NewList( FileList);
			if( FileList.Count > 0)
			{
				Event E = new Event();
				E.What = Event.Broadcast;
				E.Command = cmFileFocused;
				E.InfoPtr = List[0];
				Owner.HandleEvent( ref E);
			}
		}

		public override void HandleEvent(ref Event Event)
		{
			if( (Event.What == Event.MouseDown) && Event.Double)
			{
				Event.What = Event.evCommand;
				Event.Command = cmOk;
				PutEvent( Event);
				ClearEvent( ref Event);
			}
			else
				base.HandleEvent (ref Event);
		}

        public override void SetData(object Rec)
		{
			ReadDirectory( (Owner as FileDialog).Directory, (Owner as FileDialog).WildCard);
		}

		public override void FocusItem(int Item)
		{
			base.FocusItem (Item);
			Message( Owner, Event.Broadcast, StdDialog.cmFileFocused, List[Item]);
		}
	}
}
