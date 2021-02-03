using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class Table : View
	{
		private const int AsciiTableCommandBase = 910;
		private const int cmCharacterFocused = 0;

		public Table( Rect Bounds):base( Bounds)
		{
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			uint Color = GetColor(6);
			for( int Y = 0; Y <= Size.Y - 1; Y++)
			{
                B.FillChar((char)' ', (byte)Color, (int)Size.X);
				for( int X = 0; X <= Size.X - 1; X++)
                    B.FillChar((char)(32 * Y + X), (byte)Color, 1, X);
				WriteLine(0, Y, (int)Size.X, 1, B);
			}
			ShowCursor();
		}

		internal void CharFocused()
		{
			Message( Owner, Event.Broadcast, AsciiTableCommandBase | cmCharacterFocused, Cursor.X + (32 * Cursor.Y));
		}

		public override void HandleEvent( ref Event Event)
		{
			Point CurrentSpot;

			base.HandleEvent( ref Event);
			if( Event.What == Event.MouseDown)
			{
				do
				{
					CurrentSpot = MakeLocal( Event.Where);
					SetCursor( CurrentSpot.X, CurrentSpot.Y);
					CharFocused();
				}while( MouseEvent( ref Event, Event.MouseMove));
				ClearEvent( ref Event);
			}
			else if( Event.What == Event.KeyDown)
			{
                switch (Event.KeyCode)
                {
					case KeyboardKeys.Home :
						SetCursor( 0, 0);
						break;
					case KeyboardKeys.End :
						SetCursor( Size.X - 1, Size.Y - 1);
						break;
					case KeyboardKeys.Up :
						if ( Cursor.Y > 0)
							SetCursor( Cursor.X, Cursor.Y - 1);
						break;
					case KeyboardKeys.Down :
						if( Cursor.Y < (Size.Y - 1))
							SetCursor( Cursor.X, Cursor.Y + 1);
						break;
					case KeyboardKeys.Left :
						if( Cursor.X > 0)
							SetCursor( Cursor.X - 1, Cursor.Y);
						break;
					case KeyboardKeys.Right :
						if( Cursor.X < ( Size.X - 1))
							SetCursor( Cursor.X + 1, Cursor.Y);
						break;
					default :
						SetCursor( Event.CharCode % 32, Event.CharCode / 32) ;
						break;
				}
				CharFocused();
				ClearEvent( ref Event);
			}
		}
	}
}
