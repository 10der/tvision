using System;
using TurboVision.Objects;

namespace TurboVision
{
	public struct Event
	{
		public const int MouseDown = 0x0001;
		public const int MouseUp   = 0x0002;
		public const int MouseMove = 0x0004;
		public const int MouseAuto = 0x0008;
		public const int KeyDown   = 0x0010;
		public const int evCommand = 0x0100;
		public const int Broadcast = 0x0200;

		public const int Nothing   = 0x0000;
		public const int Mouse     = 0x000F;
		public const int Keyboard  = 0x0010;
		//public const int Message   = 0xFF00;
        public const int Console   = 0x0020;
        //public const int MSMQ      = 0x0040;

        public int What;
		public byte Buttons;
		public bool Double;
		public Point Where;
		public object InfoPtr;
		public int InfoLong;
		public uint InfoWord;
		public short InfoInt;
		public byte InfoByte;
		public byte InfoChar;

		public KeyboardKeys KeyCode;

		public int Command;

        public KEY_EVENT_RECORD KeyEvent;

        public byte CharCode
		{
			get
			{
				return (byte)((uint)(KeyCode) & 0xFF);
			}
			set
			{
				uint v = value;
				KeyCode = (KeyboardKeys)( (((uint)KeyCode >> 8) << 8) | ( v & 0xFF));
			}
		}


        public byte ScanCode
        {
            get
            {
                return (byte)(((uint)KeyCode) & 0xFF);
            }
            set
            {
                KeyCode = (KeyboardKeys)(((((uint)KeyCode) >> 8) << 8) | value);
            }
        }

        public override string ToString()
		{
			return string.Format( @"
What : {0};
Buttons : {1};
Double : {2};
Where
	X : {3};
	Y : {4};
InfoPtr {5};
InfoLong {6};
InfoWord {7};
InfoInt {8};
InfoByte {9};
InfoChar {10};",
		What, Buttons, Double, Where.X, Where.Y, InfoPtr, InfoLong, InfoWord, InfoInt, InfoByte, InfoChar);
		}
    }
}
