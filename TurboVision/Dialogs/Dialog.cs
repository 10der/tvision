using System;
using System.Xml.Serialization;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	public enum DialogPalettes
	{
		BlueDialog = 0,
		CyanDialog = 1,
		GrayDialog = 2,
	}

    [Serializable]
	public class Dialog : Window
	{

        private static uint[] CGrayDialog = 
        {   0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e,
			0x2f, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d,
			0x3e, 0x3f};
        private static uint[] CBlueDialog =
        {   0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 
            0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 
            0x5e, 0x5f };
        private static uint[] CCyanDialog =
        {   0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 
            0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 
            0x7e, 0x7f};

        public Dialog():base( new Rect(0, 0, 50, 20), "")
        {
            Initialize();
        }

        public Dialog( Rect Bounds, string ATitle):base( Bounds, ATitle, 0)
		{
            Initialize();
		}

        protected void Initialize()
        {
            Options |= OptionFlags.ofVersion20;
            GrowMode = 0;
            Flags = WindowFlags.wfMove | WindowFlags.wfClose;
            Palette = (WindowPalettes)DialogPalettes.GrayDialog;
        }

        public override uint[] GetPalette()
		{
            uint[][] P = new uint[3][] { CBlueDialog, CCyanDialog, CGrayDialog };
			return P[(int)Palette];
		}

		public override void SizeLimits(out Point Min, out Point Max)
		{
			base.SizeLimits (out Min, out Max);
			if( Owner != null)
			{
				Max.X = Owner.Size.X;
				Max.Y = Owner.Size.Y;
			}
			else
			{
				Max.X = int.MaxValue;
				Max.Y = int.MaxValue;
			}
		}

		public override bool Valid(int Command)
		{
			if( Command == cmCancel)
				return true;
			else
				return base.Valid( Command);
		}

		public override void HandleEvent( ref Event Event)
		{
			base.HandleEvent( ref Event);
			switch( Event.What)
			{
				case Event.KeyDown :
                    switch (Event.KeyCode)
                    {
					case KeyboardKeys.Esc :
						Event.What = Event.evCommand;
						Event.Command = cmCancel;
						Event.InfoPtr = null;
						PutEvent( Event);
						ClearEvent( ref Event);
						break;
					case KeyboardKeys.Enter :
						Event.What = Event.Broadcast;
						Event.Command = cmDefault;
						Event.InfoPtr = null;
						PutEvent( Event);
						ClearEvent( ref Event);
						break;
				}
					break;
				case Event.evCommand :
				switch( Event.Command)
				{
					case cmOk :
					case cmCancel :
					case cmYes :
					case cmNo :
						if( (State & StateFlags.Modal) != 0)
						{
							EndModal( Event.Command);
							ClearEvent( ref Event);
						}
						break;
				}
					break;
			}
		}

	}
}
