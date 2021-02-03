using System;
using System.Collections.Specialized;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{

	public class SItem
	{
		public string Value;
		public SItem Next;
	}

	public class Cluster : View
	{

        private static uint[] CCluster = { 0x10, 0x11, 0x12, 0x12, 0x1F };

		protected long value;
		protected int sel;
		protected System.UInt32 enableMask;

		public StringCollection Strings = new StringCollection();

		public Cluster( Rect Bounds, SItem AStrings):base( Bounds)
		{
			Options |= OptionFlags.ofSelectable | OptionFlags.ofFirstClick | OptionFlags.ofPreProcess |	OptionFlags.ofPostProcess | OptionFlags.ofVersion20;
			int I = 0;
			SItem P = AStrings;
			while( P != null)
			{
				I++;
				P = P.Next;
			}
			Strings = new StringCollection();
			while( AStrings != null)
			{
				P = AStrings;
				Strings.Add( AStrings.Value);
				AStrings = AStrings.Next;
			}
			Value = 0;
			Sel = 0;
			SetCursor( 2, 0);
			ShowCursor();
			EnableMask = 0xFFFFFFFF;
		}

		public virtual bool Mark( int Item)
		{
			return false;
		}

		public virtual byte MultiMark( int Item) 
		{
			if (Mark( Item) == true)
				return 1;
			else
				return 0;
		}

		public long Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}
		public int Sel
		{
			get
			{
				return sel;
			}
			set
			{
				sel = value;
			}
		}

		public System.UInt32 EnableMask
		{
			get
			{
				return enableMask;
			}
			set
			{
				enableMask = value;
			}
		}

        public override uint[] GetPalette()
		{
			return CCluster;
		}

		public static SItem NewSItem( string Str, SItem Next)
		{
			SItem Item = new SItem();
			Item.Value = Str;
			Item.Next = Next;
			return Item;
		}

		public void DrawMultiBox( string Icon, string Marker)
		{
			int Cur, Col;
			uint CNorm, CSel, CDis, Color;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			CNorm = GetColor(0x0301);
			CSel = GetColor(0x0402);
			CDis = GetColor(0x0505);

			for( int i = 0; i <= Size.Y; i++)
			{
				B.FillChar( ' ', (byte)CNorm, Size.X);
				for( int j = 0; j <= ( (Strings.Count / Size.Y) + 1); j++)
				{
					Cur = (int)(j * Size.Y + i);
					if( Cur < Strings.Count)
					{
						Col = Column( Cur);
						if( ((Col + ItemCLen( Strings[Cur]) + 5) < 
							( B.drawBuffer.Length)) && ( Col < Size.X))
						{
							if( ! ButtonState(Cur))
								Color = CDis;
							else
								if( (Cur == Sel) && ( (State & StateFlags.Focused) != 0))
								Color = CSel;
							else
								Color = CNorm;
							B.FillChar( (char)' ', (byte)Color, (int)Size.X - Col, Col);
							B.FillStr( Icon, (byte)Color, Col);
							B.drawBuffer[Col + 2].AsciiChar = (char)Marker[MultiMark(Cur)];
							B.FillCStr( Strings[Cur], (uint)Color, Col + 5);
							if( ShowMarkers && ( ( State & StateFlags.Focused) != 0) && ( Cur == Sel))
							{
								B.drawBuffer[Col].AsciiChar = (char)SpecialChars[0];
								B.drawBuffer[Column( (int)(Cur + Size.Y)) - 1].AsciiChar = (char)SpecialChars[1];
							}
						}
					}
				}
				WriteBuf(0, i, (int)Size.X, 1, B);
			}
			SetCursor( Column( Sel) + 2, Row( Sel));
		}

		public int ItemCLen( string s)
		{
			int j = 0;
			for( int i = 0; i < s.Length; i++)
				if( s[i] != '~')
					j++;
			return j;
		}
		
		public int Column( int Item)
		{
			int Result;
			int Col, Width, L = 0;
			if( Item < Size.Y)
				Result = 0;
			else
			{
				Width = 0;
				Col = -6;
				for( int i = 0; i <= Item; i++)
				{
					if( (i % Size.Y) == 0)
					{
						Col += Width + 6;
						Width = 0;
					}
					if( i < Strings.Count)
						L = ItemCLen( Strings[i]);
					if( L > Width)
						Width = L;
				}
				Result = Col;
			}
			return Result;
		}

		public bool ButtonState( int Item)
		{
			if( Item > 31)
				return false;
			else
				return ( ((1 << Item) & EnableMask) != 0);
		}

		public int Row( int Item)
		{
			return (int)(Item % Size.Y);
		}

		public void DrawBox( string Icon, char Marker)
		{
			DrawMultiBox( Icon, " " + Marker);
		}

		public override uint GetHelpCtx()
		{
			if( HelpCtx == hcNoContext)
				return hcNoContext;
			else
				return (uint)(HelpCtx + Sel);
		}

        public override object GetData()
		{
            return Value;
            // return new object[1] { Value };
        }

        public override uint DataSize()
        {
            return 1;
        }

        internal void MoveSel(int I, int S)
        {
            if (I < Strings.Count)
            {
                Sel = S;
                MovedTo(Sel);
                DrawView();
            }
        }

        public override void HandleEvent(ref Event Event)
        {
            Point Mouse;

            base.HandleEvent( ref Event);
            if ((Options & OptionFlags.ofSelectable) == 0)
                return;
            if (Event.What == Event.MouseDown)
            {
                Mouse = MakeLocal(Event.Where);
                int I = FindSel(Mouse);
                if (I != -1)
                    if (ButtonState(I))
                        Sel = I;
                DrawView();
                do
                {
                    Mouse = MakeLocal(Event.Where);
                    if (FindSel(Mouse) == Sel)
                        ShowCursor();
                    else
                        HideCursor();
                } while (MouseEvent(ref Event, Event.MouseMove));
                ShowCursor();
                Mouse = MakeLocal(Event.Where);
                if ((FindSel(Mouse) == Sel) && (ButtonState(Sel)))
                {
                    Press(Sel);
                    DrawView();
                }
                ClearEvent(ref Event);
            }
            else if (Event.What == Event.KeyDown)
            {
                int S = Sel;
                switch (Drivers.CtrlToArrow(Event.KeyCode))
                {
                    case KeyboardKeys.Up :
                        if( (State & StateFlags.Focused) != 0)
                        {
                            int I = 0;
                            do
                            {
                                I++;
                                S--;
                                if (S < 0)
                                    S = Strings.Count - 1;
                            }while( !(ButtonState( S) || ( I > Strings.Count)));
                            MoveSel( I, S);
                            ClearEvent(ref Event);
                        }
                        break;
                    case KeyboardKeys.Down :
                        if ((State & StateFlags.Focused) != 0)
                        {
                            int I = 0;
                            do
                            {
                                I++;
                                S++;
                                if (S >= Strings.Count)
                                    S = 0;
                            } while (!(ButtonState(S) || (I > Strings.Count)));
                            MoveSel( I, S);
                            ClearEvent(ref Event);
                        }
                        break;
                    case KeyboardKeys.Right :
                        if ((State & StateFlags.Focused) != 0)
                        {
                            int I = 0;
                            do
                            {
                                I ++;
                                S += Size.Y;
                                if( S > Strings.Count)
                                {
                                    S = ( S + 1) % Size.Y;
                                    if( S >= Strings.Count)
                                        S = 0;
                                }
                            } while (!(ButtonState(S) || (I > Strings.Count)));
                            MoveSel( I, S);
                            ClearEvent(ref Event);
                        }
                        break;
                    case KeyboardKeys.Left :
                        if ((State & StateFlags.Focused) != 0)
                        {
                            int I = 0;
                            do
                            {
                                I ++;
                                if( S > 0)
                                {
                                    S -= Size.Y;
                                    if( S < 0)
                                    {
                                        S = (( Strings.Count + Size.Y - 1) / Size.Y) * Size.Y + S - 1;
                                        if( S >= Strings.Count)
                                            S = Strings.Count - 1;
                                    }
                                }
                                else
                                    S = Strings.Count - 1;
                            } while (!(ButtonState(S) || (I > Strings.Count)));
                            MoveSel( I, S);
                            ClearEvent(ref Event);
                        }
                        break;
                    default:
                        {
                            for (int I = 0; I < Strings.Count; I++)
                            {
                                char C = HotKey(Strings[I]);
                                if ((Drivers.GetAltCode(C) == (uint)Event.KeyCode) ||
                                (((Owner.Phase == Phases.phPostProcess) || ((State & StateFlags.Focused) != 0))
                                    && (C != '\x0000') && (char.ToUpper((char)Event.CharCode) == C)))
                                {
                                    if (ButtonState(I))
                                    {
                                        if (Focus())
                                        {
                                            Sel = I;
                                            MovedTo(Sel);
                                            Press(Sel);
                                            DrawView();
                                        }
                                        ClearEvent(ref Event);
                                    }
                                    return;
                                }
                            }
                            if ((Event.CharCode == ' ') && ((State & StateFlags.Focused) != 0) && ButtonState(Sel))
                            {
                                Press(Sel);
                                DrawView();
                                ClearEvent(ref Event);
                            }
                        }
                        break;
                }
            }
        }

        internal static char HotKey( string s)
        {
            int P;
            char HotKey = '\x0000';
            if (s == "")
                return HotKey;
            P = s.IndexOf('~');
            if (P != 0)
                HotKey = char.ToUpper(s[P + 1]);
            return HotKey;
        }

        public int FindSel(Point P)
        {
            int FindSel = -1;
            Rect R = GetExtent();
            if (!R.Contains(P))
                FindSel = -1;
            else
            {
                int I = 0;
                while (P.X >= Column(I + Size.Y))
                    I += Size.Y;
                int S = I + P.Y;
                if (S >= Strings.Count)
                    FindSel = -1;
                else
                    FindSel = S;
            }
            return FindSel;
        }

        public virtual void Press( int Item)
        {
        }

        public virtual void MovedTo(int Item)
        {
        }

        public override void SetState(View.StateFlags AState, bool Enable)
        {
            base.SetState( AState, Enable);
            if (AState == StateFlags.Focused)
                DrawView();
        }

        public override void SetData(object Rec)
        {
            Value = (int)Rec;
        }
    }
}
