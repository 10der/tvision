using System;
using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.Views;

namespace TurboVision.History
{

	public class HistoryViewer : ListViewer
	{

        public int HistoryId;

        private static uint[] CHistoryViewer = { 0x06, 0x06, 0x07, 0x06, 0x06 };

		public HistoryViewer( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, int AHistoryId):base( Bounds, 1, AHScrollBar, AVScrollBar)
		{
			HistoryId = AHistoryId;
			SetRange( History.HistoryCount( AHistoryId));
			if( Range > 1)
				FocusItem(1);
			HScrollBar.SetRange( 1, HistoryWidth() - Size.X + 3);
		}

        public override uint[] GetPalette()
		{
			return CHistoryViewer;
		}

		public override string GetText( int Item, int MaxLen)
		{
			return History.HistoryStr( HistoryId, Item);
		}

        public override void HandleEvent(ref Event Event)
        {
            if (((Event.What == Event.MouseDown) && Event.Double) ||
                ((Event.What == Event.KeyDown) && (Event.KeyCode == KeyboardKeys.Enter)))
            {
                EndModal(cmOk);
                ClearEvent(ref Event);
            }
            else
                if (((Event.What == Event.KeyDown) && (Event.KeyCode == KeyboardKeys.Esc)) ||
                ((Event.What == Event.evCommand) && (Event.Command == cmCancel)))
            {
                EndModal(cmCancel);
                ClearEvent(ref Event);
            }
        }

        public int HistoryWidth()
        {
            int Width, T, Count;
            Width = 0;
            Count = History.HistoryCount(HistoryId);
            for (int i = 0; i < Count; i++)
            {
                T = History.HistoryStr(HistoryId, i).Length;
                if (T > Width)
                    Width = T;
            }
            return Width;
        }
    }
}
