using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.History
{
	public class HistoryWindow : Window
	{

        private static uint[] CHistoryWindow = { 0x13, 0x13, 0x15, 0x18, 0x19, 0x13, 0x14 };

        ListViewer Viewer = null;

        public HistoryWindow( Rect Bounds, int HistoryId):base( Bounds, "", wnNoNumber)
		{
            Flags = WindowFlags.wfClose;
            InitViewer(HistoryId);
        }

        public override uint[] GetPalette()
		{
			return CHistoryWindow;
		}

        public string GetSelection()
        {
            return Viewer.GetText(Viewer.Focused, 255);
        }

        public void InitViewer(int HistoryId)
        {
            Rect R = GetExtent();
            R.Grow(-1, -1);
            Viewer = new HistoryViewer(
                R, StandardScrollBar(sbHorizontal + sbHandleKeyboard),
                StandardScrollBar(sbVertical + sbHandleKeyboard),
                HistoryId);
            Insert(Viewer);
        }
    }
}
