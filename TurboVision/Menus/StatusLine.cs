using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Menus
{
    [Serializable]
	public class StatusItem
	{
		public StatusItem Next;
		public string Text;
		public KeyboardKeys KeyCode;
		public int Command;

		public int CTextLen()
		{
			int j = 0;
			foreach( char c in Text)
				if( c != '~')
					j ++;
			return j;
		}
	}

    [Serializable]
	public class StatusDef
	{
		public StatusDef Next;
		public uint Min;
		public uint Max;
		public StatusItem Items;
	}

    [Serializable]
	public class StatusLine : View
	{
        public static uint[] CStatusLine = { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

		public StatusItem Items = null;
		public StatusDef Defs = null;
		
		public StatusLine( Rect Bounds, StatusDef ADefs):base(Bounds)
		{
			Options |= OptionFlags.ofPreProcess;
			EventMask |= EventMasks.evBroadcast;
			GrowMode = GrowModes.gfGrowLoY | GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
			Defs = ADefs;
			FindItems();
		}

        public override uint[] GetPalette()
		{
			return CStatusLine;
		}

		public static StatusDef NewStatusDef( uint min, uint max, StatusItem items, StatusDef next)
		{
            return new StatusDef()
            {
                Next = next,
                Min = min,
                Max = max,
                Items = items
            };
		}

		public static StatusItem NewStatusKey( string text, KeyboardKeys keyCode, int command, StatusItem next)
		{
            return new StatusItem()
            {
                Text = text,
                KeyCode = keyCode,
                Command = command,
                Next = next
            };
		}

		public override void Draw()
		{
			DrawSelect(null);
		}


		public void DrawSelect( StatusItem Selected)
		{
			DrawBuffer B  = new DrawBuffer( Size.X * Size.Y);
			int I, L;
			StatusItem T;
			uint CSelect, CNormal, CSelDisabled, CNormDisabled;
			uint Color;
			string HintBuf;

			CNormal = GetColor(0x0301);
			CSelect = GetColor(0x0604);
			CNormDisabled = GetColor(0x0202);
			CSelDisabled = GetColor(0x0505);

			B.FillChar( ' ', CNormal, (int)Size.X);
			T = Items;
			I = 0;
			while ( T != null)
			{
				if( T.Text != "")
				{
					L = T.CTextLen();
					if( ((I + L) - 1) < Size.X)
					{
						if( CommandEnabled( T.Command))
							if( T == Selected)
								Color = CSelect;
							else
								Color = CNormal;
						else
							if( T == Selected)
							Color = CSelDisabled;
						else
							Color = CNormDisabled;
						B.FillCStr( T.Text, Color, I);
						B.FillChar( ' ', CNormal, 1, (int)I + L);
					}
					I += ( L + 1);
				}
				T = T.Next;
			}
			if( I < (Size.X - 2))
			{
				HintBuf = Hint( HelpCtx);
				if( HintBuf != "")
				{
					B.FillChar( ldVerticalBar, CNormal, 1, I);
					I += 2;
					if( (I + HintBuf.Length) > Size.X)
						HintBuf = HintBuf.Substring(0, (int)Size.X);
					B.FillStr( HintBuf, CNormal, I);
				}
			}
			WriteLine(0, 0, (int)Size.X, 1, B);
		}

		public virtual string Hint( uint AHelpCtx)
		{
			return "";
		}

		public void FindItems()
		{
			StatusDef P = Defs;
			while( ( P != null) && (( HelpCtx < P.Min) && ( HelpCtx > P.Max)))
				P = P.Next;
			if( P == null)
				Items = null;
			else
				Items = P.Items;
		}

		public void Update()
		{

		}

		internal StatusItem ItemMouseIsIn( Point Mouse)
		{
			uint I, K;
			StatusItem T;
			StatusItem Result = null;
			if( Mouse.Y != 0)
				return Result;
			I = 0;
			T = Items;
			while( T != null)
			{
				if( T.Text != "")
				{
					K = (uint)(I + T.CTextLen() + 1);
					if( (Mouse.X >= I) && ( Mouse.X < K))
					{
						Result = T;
						return Result;
					}
					I = K;
				}
				T = T.Next;
			}
			return Result;
		}

		public override void HandleEvent( ref Event Event)
		{

			StatusItem T = null;
			Point Mouse;

			base.HandleEvent( ref Event);
			switch( Event.What)
			{
				case Event.MouseDown :
					T = null;
					do
					{
						Mouse = MakeLocal( Event.Where);
						if( T != ItemMouseIsIn( Mouse))
						{
							T = ItemMouseIsIn( Mouse);
							DrawSelect( T);
						}
					}while( MouseEvent( ref Event, Event.MouseMove));
					if( (T != null) && ( CommandEnabled( T.Command)))
					{
						Event.What = Event.evCommand;
						Event.Command = T.Command;
						Event.InfoPtr = null;
						
						PutEvent( Event);
					}
					ClearEvent( ref Event);
					DrawView();
					break;
				case Event.KeyDown:
					T = Items;
					while( T != null)
					{
                        if ((Event.KeyCode == T.KeyCode) && (CommandEnabled(T.Command)))
                        {
							Event.What = Event.evCommand;
							Event.Command = T.Command;
							Event.InfoPtr = null;
							return;
						}
						T = T.Next;
					}
					break;
				case Event.Broadcast :
					if( Event.Command == cmCommandSetChanged)
						DrawView();
					break;
			}
		}
	}
}
