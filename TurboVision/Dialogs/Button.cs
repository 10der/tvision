using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for Button.
	/// </summary>
	public class Button : View
	{

        private static uint[] CButton = { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0E, 0x0E, 0x0F };
		public int Command;
		private ButtonFlags flags;
		private bool amDefault;
		public string Title;

		public enum ButtonFlags
		{
			Normal    = 0x00,
			Default   = 0x01,
			LeftJust  = 0x02,
			Broadcast = 0x04,
			GrabFocus = 0x08,
		}

		public Button( Rect Bounds, string ATitle, int ACommand, ButtonFlags AFlags):base( Bounds)
		{
			Options |= ( OptionFlags.ofFirstClick | OptionFlags.ofSelectable | OptionFlags.ofPreProcess | OptionFlags.ofPostProcess);
			EventMask |= EventMasks.evBroadcast;
			if( !CommandEnabled( ACommand))
				State |= StateFlags.Disabled;
			Flags = AFlags;
			if( (Flags & ButtonFlags.Default) != 0)
				AmDefault = true;
			else
				AmDefault = false;
			Title = ATitle;
			Command = ACommand;
		}

		public ButtonFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}
		public bool AmDefault
		{
			get
			{
				return amDefault;
			}
			set
			{
				amDefault = value;
			}
		}

        public override uint[] GetPalette()
		{
			return CButton;
		}

		public override void Draw()
		{
			DrawState( false);
		}

		public void DrawState( bool Down)
		{
			uint CButton;
            uint CShadow;
			char Ch;
			int I, S, Y, T;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			Ch = '\x00';
			if( (State & StateFlags.Disabled) !=0 )
				CButton = GetColor(0x0404);
			else
			{
				CButton = GetColor(0x0501);
				if( (State & StateFlags.Active) !=0 )
					if( (State & StateFlags.Selected) != 0)
						CButton = GetColor(0x0703);
					else
						if( AmDefault)
						CButton = GetColor(0x0602);
			}
			CShadow = GetColor(8);
			S = (int)(Size.X - 1);
			T = (int)(( Size.Y / 2) - 1);
			for( Y = 0; Y < Size.Y - 1; Y++)
			{
				B.FillChar( (char)' ', CButton, (int)Size.X);
				B.drawBuffer[0].Attribute = CShadow;
				if( Down)
				{
					B.drawBuffer[1].Attribute = CShadow;
					Ch = ' ';
					I = 2;
				}
				else
				{
					B.drawBuffer[S].Attribute = CShadow;
					if( ShowMarkers)
						Ch = ' ';
					else
					{
						if( Y == 0)
							B.drawBuffer[S].AsciiChar = ldBlockBottom;
						else
							B.drawBuffer[S].AsciiChar = ldBlockFull;
						Ch = ldBlockTop;
					}
					I = 1;
				}
				if( (Y == T) && ( Title != ""))
					DrawTitle( B, ref I, ref S, CButton, Down);
				if( ShowMarkers && (!Down) )
				{
					B.drawBuffer[1].AsciiChar = '[';
					B.drawBuffer[S - 1].AsciiChar = ']';
				}
				WriteLine( 0, Y, (int)Size.X, 1, B);
			}
			B.FillChar( ' ', (byte)CShadow, 2, 0);
            B.FillChar(Ch, (byte)CShadow, S - 1, 2);
			WriteLine( 0, (int)(Size.Y - 1), (int)(Size.X), 1, B);
		}

		internal void DrawTitle( DrawBuffer B, ref int I, ref int S, uint CButton, bool Down)
		{
			int L, SCOff;
			if( (Flags & ButtonFlags.LeftJust) != 0)
				L = 1;
			else
			{
				L = ( S -  CTitleLen() - 1) / 2;
				if( L < 1)
					L = 1;
				B.FillCStr( Title, CButton, I + L);
				if( ShowMarkers & !Down)
				{
					if( (State & StateFlags.Selected) != 0)
						SCOff = 0;
					else
						if( AmDefault)
						SCOff = 2;
					else
						SCOff = 4;
					B.drawBuffer[0].AsciiChar = SpecialChars[SCOff];
					B.drawBuffer[S].AsciiChar = SpecialChars[SCOff + 1];
				}
			}
		}

		public int CTitleLen()
		{
			int j = 0;
			foreach( char c in Title)
				if( c != '~')
					j ++;
			return j;
		}

		public virtual void Press()
		{
			Event E = new Event();
			Message( Owner, Event.Broadcast, cmRecordHistory, null);
			if( (Flags & ButtonFlags.Broadcast) != 0)
				Message( Owner, Event.Broadcast, Command, this);
			else
			{
				E.What = Event.evCommand;
				E.Command = Command;
				E.InfoPtr = this;
				PutEvent( E);
			}
		}

		internal char HotKey( string S)
		{
			int P;
			char Result = '\x00';
			if( S == "")
				return Result;
			P = S.IndexOf( '~');
			if( P != -1)
				Result = char.ToUpper(S[P + 1]);
			return Result;
		}

		public override void HandleEvent( ref Event Event)
		{
			Point Mouse;
			bool Down;
			char C;

			Rect ClickRect = GetExtent();
			ClickRect.A.X ++;
			ClickRect.B.X --;
			ClickRect.B.Y --;
			if( Event.What == Event.MouseDown)
			{
				Mouse = MakeLocal( Event.Where);
				if( ! ClickRect.Contains( Mouse))
					ClearEvent( ref Event);
			}
			if( (Flags & ButtonFlags.GrabFocus) != 0)
				base.HandleEvent( ref Event);
			switch( Event.What)
			{
				case Event.MouseDown :
				{
					if( (State & StateFlags.Disabled) == 0)
					{
						ClickRect.B.X ++;
						Down = false;
						do
						{
							Mouse = MakeLocal( Event.Where);
							if( Down != ClickRect.Contains( Mouse))
							{
								Down = !Down;
								DrawState( Down);
							}
						}while( MouseEvent( ref Event, Event.MouseMove));
						if( Down)
						{
							Press();
							DrawState( false);
						}
					}
					ClearEvent( ref Event);
				}
					break;
				case Event.KeyDown :
					if( Title != "")
					{
						C = HotKey( Title);
                        if ((Event.KeyCode == (KeyboardKeys)Drivers.GetAltCode(C)) ||
                            (( Owner.Phase == Phases.phPostProcess) && ( C != '\x00') &&
							( char.ToUpper((char)Event.CharCode) == C)) ||
							( ((State & StateFlags.Focused) != 0) && ( Event.CharCode == ' ')))
						{
							Press();
							ClearEvent( ref Event);
						}
					}
					break;
				case Event.Broadcast :
				switch( Event.Command)
				{
					case cmDefault :
						if( AmDefault && ( (State & StateFlags.Disabled) == 0))
						{
							Press();
							ClearEvent( ref Event);
						}
						break;
					case cmGrabDefault :
					case cmReleaseDefault :
						if( (Flags & ButtonFlags.Default) != 0)
						{
							AmDefault = Event.Command == cmReleaseDefault;
							DrawView();
						}
						break;
					case cmCommandSetChanged :
					{
						SetState( StateFlags.Disabled, ! CommandEnabled( Command));
						DrawView();
					}
						break;
				}
					break;
			}
		}

		public void MakeDefault( bool Enable)
		{
			int C;
			if( (Flags & ButtonFlags.Default) == 0)
			{
				if( Enable)
					C = cmGrabDefault;
				else
					C = cmReleaseDefault;
				Message( Owner, Event.Broadcast, C, this);
				AmDefault = Enable;
				DrawView();
			}
		}

		public override void SetState(StateFlags AState, bool Enable)
		{
			base.SetState (AState, Enable);
			if( (AState & ( StateFlags.Selected | StateFlags.Active)) != 0)
				DrawView();
			if( (AState & StateFlags.Focused) != 0)
				MakeDefault( Enable);
		}
	}

    public class OkButton : Button
    {
        public OkButton( Rect R):base( R, "O~k~", Button.cmOk, Button.ButtonFlags.Default)
        {
        }
    }

    public class CancelButton : Button
    {
        public CancelButton( Rect R):base( R, "~C~ancel", Button.cmCancel, 0)
        {
        }
    }
}
