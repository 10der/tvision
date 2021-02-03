using System;
using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.Views;

namespace TurboVision.FileDialogs
{
	
	public class DirEntry
	{
		public string DisplayText;
		public string Directory;
	}
	
	public class DirListBox : ListBox
	{

		public string Dir = "";
		public uint Cur;

		public DirListBox( Rect Bounds, ScrollBar AScrollBar):base( Bounds, 1, AScrollBar)
		{
			Dir = "";
		}

		public override string GetText(int Item, int MaxLen)
		{
			return (List[Item] as DirEntry).DisplayText;
		}

		public override bool IsSelected( int Item)
		{
			return Item == Cur;
		}
	}
}
