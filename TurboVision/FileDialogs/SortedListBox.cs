using System;
using System.Collections;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.FileDialogs
{
	public class SortedListBox :ListBox
	{

		public uint SearchPos;
		public byte ShiftState;

		public SortedListBox( Rect Bounds, int ANumCols, ScrollBar AScrollBar):base( Bounds, ANumCols, AScrollBar)
		{
			SearchPos = 0;
			ShowCursor();
			SetCursor( 1, 0);
		}

		public override void NewList( ArrayList AList)
		{
			base.NewList( AList);
			SearchPos = 0;
		}

		internal bool Equal( string S1, string S2, uint Count, int OldPos)
		{
			bool Result = false;
			if( (S1.Length < Count) || ( S2.Length < Count))
				return Result;
			for( int i = 1; i <= Count; i++)
				if( char.ToUpper(S1[i]) != char.ToUpper( S2[i]))
					return Result;
			return true;
		}
	}
}
