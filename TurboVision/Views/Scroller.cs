using System;
using TurboVision.Objects;

namespace TurboVision.Views
{
	/// <summary>
	/// Summary description for Scroller.
	/// </summary>
	public class Scroller : View
	{

        private static uint[] CScroller = { 0x06, 0x07 };

		public Point Limit;
		public Point Delta;
		public byte DrawLock;
		public bool DrawFlag;
		public ScrollBar HScrollBar;
		public ScrollBar VScrollBar;

		public Scroller( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar):base( Bounds)
		{
			Options |= OptionFlags.ofSelectable;
			EventMask |= EventMasks.evBroadcast;
			HScrollBar = AHScrollBar;
			VScrollBar = AVScrollBar;
		}

		public override void ChangeBounds( Rect Bounds)
		{
			SetBounds( Bounds);
			DrawLock++;
			SetLimit( Limit.X, Limit.Y);
			DrawLock --;
			DrawFlag = false;
			DrawView();
		}

		public void CheckDraw()
		{
			if( (DrawLock == 0) && DrawFlag)
			{
				DrawFlag = false;
				DrawView();
			}
		}

		public override void HandleEvent( ref Event Event)
		{
			base.HandleEvent( ref Event);
			if( (Event.What == Event.Broadcast) && ( Event.Command == cmScrollBarChanged) &&
				((Event.InfoPtr == HScrollBar) || (Event.InfoPtr == VScrollBar)))
				ScrollDraw();
		}

		public virtual void ScrollDraw()
		{
			Point D;
			if( HScrollBar != null)
				D.X = HScrollBar.Value;
			else
				D.X = 0;
			if( VScrollBar != null)
				D.Y = VScrollBar.Value;
			else
				D.Y = 0;
			if( (D.X != Delta.X) || ( D.Y != Delta.Y))
			{
				SetCursor( Cursor.X + Delta.X - D.X, Cursor.Y + Delta.Y - D.Y);
				Delta = D;
				if( DrawLock != 0)
					DrawFlag = true;
				else
					DrawView();
			}
		}

		public void ScrollTo( int X, int Y)
		{
			DrawLock++;
			if( HScrollBar != null)
				HScrollBar.SetValue( X);
			if( VScrollBar != null)
				VScrollBar.SetValue( Y);
			DrawLock --;
			CheckDraw();
		}

		public virtual void SetLimit( int X, int Y)
		{
			Limit.X = X;
			Limit.Y = Y;
			DrawLock ++;
			if( HScrollBar != null)
				HScrollBar.SetParams( HScrollBar.Value, 0, X - Size.X, Size.X - 1, HScrollBar.ArStep);
			if( VScrollBar != null)
				VScrollBar.SetParams( VScrollBar.Value, 0, Y - Size.Y, Size.Y - 1, VScrollBar.ArStep);
			DrawLock--;
			CheckDraw();
		}

        public override uint[] GetPalette()
		{
			return CScroller;
		}

		internal void ShowSBar( ScrollBar SBar)
		{
			if( SBar != null)
				if( GetState(  StateFlags.Active | StateFlags.Selected))
					SBar.Show();
			else
					SBar.Hide();
		}

		public override void SetState( StateFlags AState, bool Enable)
		{
			base.SetState( AState, Enable);
			if( (AState & ( StateFlags.Active | StateFlags.Selected)) != 0)
			{
				ShowSBar( HScrollBar);
				ShowSBar( VScrollBar);
			}
		}
	}
}
