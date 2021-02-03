using System;
using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	public class ColorItemList : ListViewer
	{

		public ColorItem Items;

		public ColorItemList( Rect Bounds, ScrollBar AScrollBar, ColorItem AItems):base( Bounds, 1, null, AScrollBar)
		{
			EventMask |= EventMasks.evBroadcast;
			Items = AItems;
			int I = 0;
			while( AItems != null)
			{
				AItems = AItems.Next;
				I++;
			}
			Range = I;
		}

		public override string GetText( int Item, int MaxLen)
		{
			ColorItem CurItem = Items;
			while( Item > 0)
			{
				CurItem = CurItem.Next;
				Item --;
			}
			return CurItem.Name;
		}
	}
}
