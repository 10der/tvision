using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class ClockView : View
	{

		public string TimeStr = "";
		public byte Refresh = 1;
		private DateTime LastTime = DateTime.Now;

		public ClockView( Rect Bounds):base( Bounds)
		{
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			uint C = GetColor(2);
            B.FillChar((char)' ', (byte)C, (int)Size.X);
            B.FillStr(TimeStr, (byte)C, 0);
			WriteLine(0, 0, (int)Size.X, 1, B);
		}

		public virtual void Update()
		{
			if ( Math.Abs((LastTime - DateTime.Now).Seconds) >= Refresh)
			{
				LastTime = DateTime.Now;
				TimeStr = DateTime.Now.ToString( "HH:mm:ss");
				DrawView();
			}
		}
	}
}
