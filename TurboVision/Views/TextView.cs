using System;
using System.IO;
using TurboVision.Objects;
using System.Collections.Specialized;

namespace TurboVision.Views
{
	public class TextScroller : Scroller
	{
		private StringCollection content = new StringCollection();

		private string text = "";

        private int tabWidth = 8;
        public int TabWidth
        {
            get
            {
                return tabWidth;
            }
            set
            {
                tabWidth = value;
            }
        }

        public string Text		
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				StringReader sr = new StringReader( text);
				int XLimit = 0;
				content.Clear();
                string s = "";
                while( ( s = sr.ReadLine()) != null)
				{
					content.Add( s);
					if( XLimit < s.Length)
						XLimit =  s.Length;
				}

				SetLimit( XLimit, content.Count);
				Delta.X = 0;
				Delta.Y = 0;
				DrawView();

			}
		}
		
		public TextScroller( Rect R, ScrollBar HScrollBar, ScrollBar VScrollBar, string DisplayText):base(  R, HScrollBar, VScrollBar)
		{
			GrowMode |= GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
			Text = DisplayText;
		}

		public override void Draw()
		{
            for (int i = 0; i < Size.Y; i++)
            {
                if (i < content.Count)
                {
                    string DisplayString = content[i + Delta.Y];
                    System.Text.StringBuilder EncodedString = new System.Text.StringBuilder();
                    int joffset = 0;
                    for (int j = 0; j < DisplayString.Length; j++)
                    {
                        char c = DisplayString[j];
                        if (c != '\x0009')
                        {
                            EncodedString.Append( c);
                            joffset++;
                        }
                        else
                        {
                            EncodedString.Append( new string(' ', tabWidth - (joffset % tabWidth)));
                            joffset += tabWidth - (joffset % tabWidth);
                        }
                    }
                    WriteStr(0, i, (EncodedString.Append( new string(' ', Limit.X + Size.X)).ToString(Delta.X, Size.X)), 1);
                }
                else
                    WriteStr(0, i, new string(' ', Size.X), 1);
            }
        }

		public override void HandleEvent( ref Event E)
		{
			base.HandleEvent( ref E);
			if( E.What == Event.KeyDown)
			{
                int OldDeltaX = Delta.X;
                int OldDeltaY = Delta.Y;
                switch (E.KeyCode)
                {
					case KeyboardKeys.Down :
						ScrollTo( Delta.X, Delta.Y + 1);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.Up :
						ScrollTo( Delta.X, Delta.Y - 1);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.PageDown :
						ScrollTo( Delta.X, Delta.Y + Size.Y);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.PageUp :
                        ScrollTo(Delta.X, Delta.Y - Size.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.CtrlPageDown:
						ScrollTo( Delta.X, content.Count);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.CtrlPageUp :
						ScrollTo( Delta.X, 0);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.Right :
						ScrollTo( Delta.X + HScrollBar.ArStep, Delta.Y);
                        ClearEvent(ref E);
                        break;
					case KeyboardKeys.Left :
						ScrollTo( Delta.X - HScrollBar.ArStep, Delta.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.Home:
                        ScrollTo(0, Delta.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.CtrlLeft:
                        ScrollTo(Delta.X - HScrollBar.PgStep, Delta.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.CtrlRight:
                        ScrollTo(Delta.X + HScrollBar.PgStep, Delta.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.End:
                        ScrollTo(HScrollBar.Max, Delta.Y);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.CtrlEnd:
                        ScrollTo(HScrollBar.Max, VScrollBar.Max);
                        ClearEvent(ref E);
                        break;
                    case KeyboardKeys.CtrlHome:
                        ScrollTo(0, 0);
                        ClearEvent(ref E);
                        break;
                }
                if ((Delta.X != OldDeltaX) || (Delta.Y != OldDeltaY))
                    ScrollDraw();
            }
        }
	}

    public class TextViewer : Window
    {

        private TextScroller ts = null;

        public TextViewer(Rect R, string Title, string TextToView)
            : base(R, Title)
        {
            R = GetExtent();
            R.A.Y = R.B.Y - 1;
            R.B.X -= 2;
            R.A.X++;
            ScrollBar HScrollBar = new ScrollBar( R);
            Insert( HScrollBar);
            R = GetExtent();
            R.A.X = R.B.X - 1;
            R.A.Y++;
            R.B.Y--;
            ScrollBar VScrollBar = new ScrollBar( R);
            Insert( VScrollBar);
            R = GetExtent();
            R.Grow( -1, -1);
            ts = new TextScroller(R, HScrollBar, VScrollBar, TextToView);
            Insert(ts);
        }

        public string Text
        {
            get
            {
                return ts.Text;
            }
            set
            {
                ts.Text = value;
            }
        }
    }
}