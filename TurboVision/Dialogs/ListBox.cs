using System;
using System.Collections;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for ListBox.
	/// </summary>
	public class ListBox : ListViewer
	{

		public ListBox( Rect Bounds, int ANumCols, ScrollBar AScrollBar):base( Bounds, ANumCols, null, AScrollBar)
		{
			List = null;
		}

		public System.Collections.ArrayList CreateCollection()
		{
			return new System.Collections.ArrayList();
		}

        public override object GetData()
        {
            return base.GetData();
        }

		public override string GetText( int Item, int MaxLen)
		{
			return List[Item].ToString();
		}

		public virtual void NewList( ArrayList AList)
		{
			if( List != null)
				List = null;
			List = AList;
            if (List == null)
                SetRange(0);
            else
                SetRange(List.Count);
            if( Range > 0)
				FocusItem(0);
			DrawView();
		}
    }
}
