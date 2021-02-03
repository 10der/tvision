using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for Label.
	/// </summary>
	public class Label : StaticText
	{

        private static uint[] CLabel = { 0x07, 0x08, 0x09, 0x09 };

		public View Link = null;
		private bool light;

		public Label( Rect Bounds, string AText, View ALink):base( Bounds, AText)
		{
			Link = ALink;
			Options |= (OptionFlags.ofPreProcess | OptionFlags.ofPostProcess);
			EventMask |= EventMasks.evBroadcast;
		}

		public bool Light
		{
			get
			{
				return light;
			}
			set
			{
				light = value;
			}
		}

        public override uint[] GetPalette()
		{
			return CLabel;
		}

		public override void Draw()
		{
			uint Color;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			byte SCOff;

			if( Light)
			{
				Color = GetColor(0x0402);
				SCOff = 0;
			}
			else
			{
				Color = GetColor(0x0301);
				SCOff = 4;
			}
            B.FillChar(' ', Color, (int)Size.X);
			if( Text != "")
				B.FillCStr( Text, Color, 0);
			if( ShowMarkers)
				B.drawBuffer[0].AsciiChar = (char)SpecialChars[SCOff];
			WriteLine(0, 0, (int)Size.X, 1, B);
		}

		public override void SetData(object Rec)
		{
			base.SetData (Rec);
		}

        internal void FocusLink( ref Event E)
        {
            if ((Link != null) && ((Link.Options & OptionFlags.ofSelectable) != 0))
                Link.Focus();
            ClearEvent(ref E);
        }

        internal char HotKey(string S)
        {
            int P;
            char HotKey = '\x0000';
            if (S == "")
                return HotKey;
            P = S.IndexOf('~');
            if (P != -1)
                HotKey = char.ToUpper(S[P + 1]);    
            return HotKey;
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent( ref Event);
            if (Event.What == Event.MouseDown)
                FocusLink( ref Event);
            else
                if (Event.What == Event.KeyDown)
            {
                char C = HotKey(Text);
                if ((Drivers.GetAltCode(C) == (char)Event.KeyCode) ||
                ((C != '\x0000') && (Owner.Phase == Phases.phPostProcess) && (char.ToUpper((char)Event.CharCode) == C)))
                    FocusLink( ref Event);
            }
            else
            {
                    if( Event.What == Event.Broadcast)
                        if (((Event.Command == cmReceivedFocus) || (Event.Command == cmReleasedFocus)) && ( Link != null))
                        {
                            Light = (Link.State & StateFlags.Focused) != 0;
                            DrawView();
                        }
                }
        }

    }
}
