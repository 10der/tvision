using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Help
{

	public class HelpFile : Objects.Object
	{
	}

	public class HelpWindow : Window
	{

        private static uint[] CHelpWindow = { 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87 };

		public HelpWindow( HelpFile HFile, uint Context)
			:base( new Rect(0, 0, 50, 18), "Help", wnNoNumber)
		{
		}

        public override uint[] GetPalette()
		{
			return CHelpWindow;
		}
	}
}
