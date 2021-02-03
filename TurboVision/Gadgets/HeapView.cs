using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	/// <summary>
	/// Summary description for HeapView.
	/// </summary>
	public class HeapView : View
	{
		private long OldMem;
		private DateTime LastTime = DateTime.Now;
		public byte Refresh = 1;

		public HeapView( Rect Bounds):base( Bounds)
		{
			OldMem = 0;
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			OldMem = GC.GetTotalMemory(false);
			string S = string.Format( "{0:G"+Size.X.ToString()+"}", OldMem);
			byte C = (byte)GetColor(2);
			B.FillChar( (char)' ', C, (int)Size.X);
			B.FillStr( S, C, 0);
			WriteLine( 0, 0, (int)Size.X, 1, B);
		}

		public virtual void Update()
		{
			if ( Math.Abs((LastTime - DateTime.Now).Seconds) > 0)
			{
				LastTime = DateTime.Now;
				OldMem = GC.GetTotalMemory(false);
				DrawView();
			}
		}
	}
}
