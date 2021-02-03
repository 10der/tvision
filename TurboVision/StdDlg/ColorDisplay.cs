using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
	public class ColorDisplay : View
	{

		public const int cmColorForegroundChanged = 71;
		public const int cmColorBackgroundChanged = 72;
		public const int cmColorSet               = 73;
		public const int cmNewColorItem           = 74;
		public const int cmNewColorIndex          = 75;
		public const int cmSaveColorIndex         = 76;

		public string Text;
		public byte Color;

		public ColorDisplay( Rect Bounds, string AText):base( Bounds)
		{
			EventMask |= EventMasks.evBroadcast;
			Text = AText;
			Color = 0;
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			byte C = Color;
			if( C == 0)
				C = ErrorAttr;
			for( int i = 0; i <= (Size.X / Text.Length); i++)
				B.FillStr( Text, C, i * Text.Length);
			WriteLine(0, 0, (int)Size.X, (int)Size.Y, B);
		}

		public virtual void SetColor( byte AColor)
		{
			Color = AColor;
			Message( Owner, Event.Broadcast, cmColorSet, Color);
		}
	}
}
