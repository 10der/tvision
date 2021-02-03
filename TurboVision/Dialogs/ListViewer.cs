using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for ListViewer.
	/// </summary>
	public abstract class ListViewer : View
	{

        public static uint[] CListViewer = { 0x1a, 0x1a, 0x1b, 0x1c, 0x1d };

		private int numCols;
		private int topItem;

		public int Focused;
		public new int Range;

		public ScrollBar HScrollBar;
		public ScrollBar VScrollBar;

        public onFocusItem ItemFocused;

		public System.Collections.ArrayList List = null;

		public ListViewer( Rect Bounds, int ANumCols, ScrollBar AHScrollBar, ScrollBar AVScrollBar):base( Bounds)
		{
			int ArStep, PgStep;
			Options |= ( OptionFlags.ofFirstClick | OptionFlags.ofSelectable);
			EventMask |= EventMasks.evBroadcast;
			Range = 0;
			NumCols = ANumCols;
			Focused = 0;
			if( AVScrollBar != null)
			{
				if( NumCols == 1)
				{
					PgStep = Size.Y - 1;
					ArStep = 1;
				}
				else
				{
					PgStep = Size.Y * NumCols;
					ArStep = Size.Y;
				}
				AVScrollBar.SetStep( PgStep, ArStep);
			}
			if( AHScrollBar != null)
				AHScrollBar.SetStep( Size.X / NumCols, 1);
			HScrollBar = AHScrollBar;
			VScrollBar = AVScrollBar;
		}

		public int NumCols
		{
			get
			{
				return numCols;
			}
			set
			{
				numCols = value;
			}
		}

		public int TopItem
		{
			get
			{
				return topItem;
			}
			set
			{
				topItem = value;
			}
		}

        public override uint[] GetPalette()
		{
			return CListViewer;
		}

		public override void Draw()
		{
			int Item;
            uint NormalColor;
            uint SelectedColor;
            uint FocusedColor;
            uint Color;
			int ColWidth, CurCol, Indent;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			string Text;
			byte SCOff;

			FocusedColor = 0;
			if( (State & ( StateFlags.Selected | StateFlags.Active)) == ( StateFlags.Selected | StateFlags.Active))
			{
				NormalColor = GetColor(1);
				FocusedColor = GetColor(3);
				SelectedColor = GetColor(4);
			}
			else
			{
				NormalColor = GetColor(2);
				SelectedColor = GetColor(4);
			}
			if( HScrollBar != null)
				Indent = HScrollBar.Value;
			else
				Indent = 0;
			ColWidth = (int)( Size.X / NumCols + 1);
			for( int i = 0; i < Size.Y; i++)
			{
				for( int j = 0; j < NumCols; j++)
				{
					Item = (int)(j * Size.Y + i + TopItem);
					CurCol = j * ColWidth;
					if( ((State & ( StateFlags.Selected | StateFlags.Active)) == ( StateFlags.Selected | StateFlags.Active)) &&
						( Focused == Item) && ( Range > 0))
					{
						Color = FocusedColor;
						SetCursor( CurCol + 1, i);
						SCOff = 0;
					}
					else
						if( ( Item < Range) && IsSelected( Item))
					{
						Color = SelectedColor;
						SCOff = 2;
					}
					else
					{
						Color = NormalColor;
						SCOff = 4;
					}
					B.FillChar( ' ', Color, ColWidth, CurCol);
					if( Item < Range)
					{
						Text = GetText( Item, ColWidth + Indent);
						Text = (Text + new string(' ', ColWidth)).Substring( Indent, ColWidth);
						B.FillStr( Text, Color, (int)CurCol + 1);
						if( ShowMarkers)
						{
							B.drawBuffer[CurCol].AsciiChar = SpecialChars[SCOff];
							B.drawBuffer[CurCol + ColWidth - 2].AsciiChar = SpecialChars[SCOff + 1];
						}
					}
					B.FillChar( ldVerticalBar, GetColor(5), 1, (int)CurCol + ColWidth - 1);
				}
				WriteLine( 0, i, (int)Size.X, 1, B);
			}
		}

		public virtual bool IsSelected( int Item)
		{
			return (Item == Focused);
		}

		public virtual string GetText( int Item, int MaxLen)
		{
			return List[Item].ToString();
		}

		public void SetRange( int ARange)
		{
			Range = ARange;
			if( VScrollBar != null)
			{
				if( Focused > ARange)
					Focused = 0;
				VScrollBar.SetParams( Focused, 0, ARange - 1, VScrollBar.PgStep, VScrollBar.ArStep);
			}
		}

        public delegate void onFocusItem(ListViewer sender, object Item);

		public virtual void FocusItem( int Item)
		{
			Focused = Item;
			if( VScrollBar != null)
				VScrollBar.SetValue( Item);
			if( Item < TopItem)
				if( NumCols == 1)
					TopItem = Item;
			else
					TopItem = Item - (Item % Size.Y);
			else
				if( Item >= (TopItem + (Size.Y * NumCols)))
				if( NumCols == 1)
					TopItem = Item - Size.Y + 1;
			else
					TopItem = Item - (Item % Size.Y) - ( Size.Y * (NumCols - 1));
        if (ItemFocused != null)
            ItemFocused(this, this.List[Item]);
		}

		public virtual void FocusItemNum( int Item)
		{
			if( Item < 0)
				Item = 0;
			else
				if( (Item >= Range) && ( Range > 0))
				Item = Range - 1;
			if( Range != 0)
				FocusItem( Item);
		}

		public virtual void SelectItem( int Item)
		{
			Message( Owner, Event.Broadcast, cmListItemSelected, this);
		}

		public override void HandleEvent(ref Event Event)
		{
			const byte MouseAutosToSkip = 4;

			Point Mouse;
			int ColWidth;
			int OldItem, NewItem;
			uint Count;

			base.HandleEvent (ref Event);
			if( Event.What == Event.MouseDown)
			{
				ColWidth = (Size.X / NumCols) + 1;
				OldItem = Focused;
				Mouse = MakeLocal( Event.Where);
				if( MouseInView( Event.Where))
					NewItem = Mouse.Y + ( Size.Y * ( Mouse.X / ColWidth)) + TopItem;
				else
					NewItem = OldItem;
				Count = 0;
				do
				{
					if( NewItem != OldItem)
					{
						FocusItemNum( NewItem);
						DrawView();
					}
					OldItem = NewItem;
					Mouse = MakeLocal( Event.Where);
					if( MouseInView( Event.Where))
						NewItem = Mouse.Y + ( Size.Y * ( Mouse.X / ColWidth)) + TopItem;
					else
					{
						if( NumCols == 1)
						{
							if( Event.What == Event.MouseAuto)
								Count++;
							if( Count == MouseAutosToSkip)
							{
								Count = 0;
								if( Mouse.Y < 0)
									NewItem = Focused - 1;
								else
									if( Mouse.Y >= Size.Y )
									NewItem = Focused + 1;
							}
						}
						else
						{
							if( Event.What == Event.MouseAuto)
								Count++;
							if( Count == MouseAutosToSkip)
							{
								Count = 0;
								if( Mouse.X < 0)
									NewItem = Focused - Size.Y;
								else
									if( Mouse.X >= Size.X)
									NewItem = Focused + Size.Y;
								else
									if( Mouse.Y < 0)
									NewItem = Focused - ( Focused % Size.Y);
								else if( Mouse.Y > Size.Y)
									NewItem = Focused - ( Focused % Size.Y) + Size.Y - 1;
							}
						}
					}
				}while( MouseEvent( ref Event, Event.MouseMove | Event.MouseAuto));
				FocusItemNum( NewItem);
				DrawView();
				if( (Event.Double) & ( Range > Focused))
					SelectItem( Focused);
				ClearEvent( ref Event);
			}
			else
				if( Event.What == Event.KeyDown)
			{
				if( (Event.CharCode == (byte)' ') && ( Focused < Range))
				{
					SelectItem( Focused);
					NewItem = Focused;
				}
				else
                    switch (Drivers.CtrlToArrow(Event.KeyCode))
                    {
						case KeyboardKeys.Up :
							NewItem = Focused - 1;
							break;
						case KeyboardKeys.Down :
							NewItem = Focused + 1;
							break;
						case KeyboardKeys.Right :
							if( NumCols > 1)
								NewItem = Focused + Size.Y;
							else
								return;
							break;
						case KeyboardKeys.Left :
							if( NumCols > 1)
								NewItem = Focused - Size.Y;
							else
								return;
							break;
						case KeyboardKeys.PageDown :
							NewItem = Focused + ( Size.Y * NumCols);
							break;
						case KeyboardKeys.PageUp :
							NewItem = Focused - ( Size.Y * NumCols);
							break;
						case KeyboardKeys.Home :
							NewItem = TopItem;
							break;
						case KeyboardKeys.End :
							NewItem = TopItem + ( Size.Y * NumCols) - 1;
							break;
						case KeyboardKeys.CtrlPageDown :
							NewItem = Range - 1;
							break;
						case KeyboardKeys.CtrlPageUp :
							NewItem = 0;
							break;
						default :
							return;
					}
				FocusItemNum( NewItem);
				DrawView();
				ClearEvent( ref Event);
			}
			else
				if( Event.What == Event.Broadcast)
				if( (Options & OptionFlags.ofSelectable) != 0)
					if( (Event.Command == cmScrollBarClicked) && ( (Event.InfoPtr == HScrollBar) || ( Event.InfoPtr == VScrollBar)))
						Select();
					else
						if( Event.Command == cmScrollBarChanged)
					{
						FocusItemNum( VScrollBar.Value);
						DrawView();
					}
			else
						if( HScrollBar == Event.InfoPtr)
						DrawView();
		}

		internal void ShowSBar( ScrollBar SBar)
		{
			if( SBar != null)
				if( GetState( StateFlags.Active) && GetState( StateFlags.Visible))
					SBar.Show();
			else
					SBar.Hide();
		}
		
		public override void SetState(StateFlags AState, bool Enable)
		{
			base.SetState (AState, Enable);
			if( (AState & ( StateFlags.Selected | StateFlags.Active | StateFlags.Visible)) != 0)
			{
				ShowSBar( HScrollBar);
				ShowSBar( VScrollBar);
				DrawView();
			}
		}

	}
}
