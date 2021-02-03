using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class CalendarView : View
	{

		public int Year = DateTime.Now.Year;
		public int Month = DateTime.Now.Month;
		public uint Days = 1;

		public uint CurYear;
		public uint CurMonth;
		public uint CurDay;

		public byte[] DaysInMonth = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        public string[] MonthStr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        public string DayString = string.Format(
        "{0,-2} {1,-2} {2,-2} {3,-2} {4,-2} {5,-2} {6,-2}",
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[0],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[1],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[2],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[3],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[4],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[5],
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[6]);


		public CalendarView( Rect Bounds):base( Bounds)
		{
			Options |= OptionFlags.ofSelectable;
			EventMask |= EventMasks.evMouseAuto;
			Year = DateTime.Now.Year;
			Month = DateTime.Now.Month;
			DrawView();
		}

		public override void Draw()
		{
			const int Width = 20;
			int DayOf, CurDays;
			string S;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			uint Color, BoldColor;

			Color = (byte)GetColor(6);
			BoldColor = (byte)GetColor(7);
			DayOf = (int)(new DateTime( (int)Year, (int)Month, 1)).DayOfWeek;
			Days = (byte)(DaysInMonth[(int)(Month - 1)]);
			if (((Year % 4) == 0) && ( Month == 2))
				Days++;
			S = string.Format("{0:0000}", Year);
			B.FillChar( ' ', Color, Width);
            B.FillCStr(string.Format("{0,-10} {1} \x1E \x1F", MonthStr[Month - 1], S), Color, 0);
			WriteLine( 0, 0, Width, 1, B);
			B.FillChar( ' ', Color, Width);
            B.FillStr( DayString, Color, 0);
			WriteLine(0, 1, Width, 1, B);
			CurDays = 1 - DayOf;
			for( int i = 1; i <= 6; i++)
			{
				for( int j = 0; j <= 6; j++)
				{
					if( (CurDays < 1) || ( CurDays > Days))
						B.FillStr( "   ", Color, j * 3);
					else
						if( (Year == CurYear) && ( Month == CurMonth) && ( CurDays == CurDay))
						B.FillStr( string.Format("{0:00}", CurDays), BoldColor, j*3);
					else
						B.FillStr( string.Format("{0:00}", CurDays), Color, j*3);
					CurDays++;
				}
				WriteLine( 0, i + 1, Width, 1, B);
			}
		}

		public override void HandleEvent(ref Event Event)
		{
			Point Point;

			base.HandleEvent (ref Event);
			if( (State & StateFlags.Selected) != 0)
			{
				if( (Event.What & ( Event.MouseDown | Event.MouseAuto)) != 0)
				{
					Point = MakeLocal( Event.Where);
					if( (Point.X == 16) && ( Point.Y == 0))
					{
						Month ++;
						if( Month > 12)
						{
							Year ++;
							Month = 1;
						}
						DrawView();
					}
					if( (Point.X == 18) && ( Point.Y == 0) )
					{
						Month --;
						if( Month < 1)
						{
							Year --;
							Month = 12;
						}
						DrawView();
					}
				}
				else
					if( Event.What == Event.KeyDown)
				{
					if( (((int)Event.KeyCode & 0xFF) == (byte)'+') || ( Event.KeyCode == KeyboardKeys.Down))
					{
						Month++;
						if( Month > 12)
						{
							Year ++;
							Month = 1;
						}
					}
					if( (((int)Event.KeyCode & 0xFF) == (byte)'-') || ( Event.KeyCode == KeyboardKeys.Up))
					{
						Month --;
						if( Month < 1)
						{
							Year --;
							Month = 12;
						}
					}
					DrawView();
				}
			}
		}
	}
}
