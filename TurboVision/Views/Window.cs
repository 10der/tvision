using System;
using System.Runtime.InteropServices;
using TurboVision.Objects;

namespace TurboVision.Views
{
	public enum WindowPalettes
	{
		wpBlueWindow = 0,
		wpCyanWindow = 1,
		wpGrayWindow = 2,
	}

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Window : Group
	{

        public static uint[] CBlueWindow =
        {   0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x40, 0x41, 0x42, 0x43, 
            0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 
            0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 
            0x5c, 0x5d, 0x5e, 0x5f };
        public static uint[] CCyanWindow =
        {   0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x60, 0x61, 0x62, 0x63, 
            0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 
            0x7c, 0x7d, 0x7e, 0x7f};
        public static uint[] CGrayWindow =
        {   0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x20, 0x21, 0x22, 0x23, 
            0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 
            0x3c, 0x3d, 0x3e, 0x3f};

        public const int sbHorizontal = 0x0000;
        public const int sbVertical = 0x0001;
        public const int sbHandleKeyboard = 0x0002;

        private static Point minWinSize;

        private WindowFlags flags;
		private int number;
		public WindowPalettes Palette;
		public Frame Frame;
		private string title = "";

		public Rect ZoomRect;

		static Window()
		{
			minWinSize.X = 16;
			minWinSize.Y = 6;
		}

        public Window():base(
            new Rect( 10, 10, 50, 30))
        {
            State |= StateFlags.Shadow;
            Options |= (OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
            GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
            Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
            Title = "";
            Number = wnNoNumber;
            Palette = WindowPalettes.wpBlueWindow;
            InitFrame();
            if (Frame != null)
                Insert(Frame);
            ZoomRect = GetBounds();
        }

		public Window( Rect Bounds, string ATitle):base( Bounds)
		{
			State |= StateFlags.Shadow;
			Options |= ( OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
			GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
			Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
			Title = ATitle;
			Number = wnNoNumber;
			Palette = WindowPalettes.wpBlueWindow;
			InitFrame();
			if( Frame != null)
				Insert( Frame);
			ZoomRect = GetBounds();
		}

		public Window( Rect Bounds, string ATitle, int ANumber):base( Bounds)
		{
			State |= StateFlags.Shadow;
			Options |= ( OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
			GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
			Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
			Title = ATitle;
			Number = ANumber;
			Palette = WindowPalettes.wpBlueWindow;
			InitFrame();
			if( Frame != null)
				Insert( Frame);
			ZoomRect = GetBounds();
		}

		public virtual void InitFrame()
		{
			Rect R = GetExtent();
			Frame = new Frame( R);
		}

		public virtual string GetTitle( int MaxSize)
		{
			if( title != "")
				return title;
			else
				return "";
		}

		public WindowFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if( value == null)
					title = "";
				else
					title = value;
			}
		}

		public Point MinWinSize
		{
			get
			{
				return minWinSize;
			}
		}

        public override uint[] GetPalette()
		{
            uint[][] P = new uint[3][] { CBlueWindow, CCyanWindow, CGrayWindow };
             return P[(int)Palette];
        }

        public override void SizeLimits(out Point Min, out Point Max)
		{
			base.SizeLimits (out Min, out Max);
			Min.X = MinWinSize.X;
			Min.Y = MinWinSize.Y;
		}

		public virtual void Zoom()
		{
			Point Min, Max;
			SizeLimits( out Min, out Max);
			if( (Size.X != Max.X) || ( Size.Y != Max.Y))
			{
				ZoomRect = GetBounds();
				Rect R = new Rect(0, 0, Max.X, Max.Y);
				Locate( R);
			}
			else
				Locate( ZoomRect);
		}

		public override void HandleEvent(ref Event Event)
		{
			Point Min, Max;
			Rect Limits;

			base.HandleEvent (ref Event);
			if( Event.What == Event.evCommand)
				switch( Event.Command)
				{
					case cmResize :
						if( (Flags & ( WindowFlags.wfMove | WindowFlags.wfGrow)) != 0)
						{
							Limits = Owner.GetExtent();
							SizeLimits( out Min, out Max);
							DragView( Event, (DragModes)(DragMode | (DragModes)( Flags & ( WindowFlags.wfMove | WindowFlags.wfGrow))),
								Limits, Min, Max);
							ClearEvent( ref Event);
						}
						break;
					case cmClose :
						if( ((Flags & WindowFlags.wfClose) != 0) &&
							((Event.InfoPtr == null) || ( Event.InfoPtr == this)))
						{
							ClearEvent( ref Event);
							if( (State & StateFlags.Modal) == 0)
								Close();
							else
							{
								Event.What = Event.evCommand;
								Event.Command = cmCancel;
								Event.InfoPtr = null;
								PutEvent( Event);
								ClearEvent( ref Event);
							}
						}
						break;
					case cmZoom :
						if( ((Flags & WindowFlags.wfZoom) != 0) &&
							(( Event.InfoPtr == null) || ( Event.InfoPtr == this)))
						{
							Zoom();
							ClearEvent( ref Event);
						}
						break;
				}
			else
				if( Event.What == Event.KeyDown)
                switch (Event.KeyCode)
                {
					case KeyboardKeys.Tab :
					{
						FocusNext( false);
						ClearEvent( ref Event);
					}
						break;
					case KeyboardKeys.ShiftTab :
					{
						FocusNext( true);
						ClearEvent( ref Event);
					}
						break;
				}
			else
				if( (Event.What == Event.Broadcast) &&
				( Event.Command == cmSelectWindowNum) &&
				( Event.InfoInt == Number) && 
				( (Options & OptionFlags.ofSelectable) != 0))
			{
				Select();
				ClearEvent( ref Event);
			}
        }

		public virtual void Close()
		{
			if( Valid( cmClose))
				Free();
		}

		public override void SetState( StateFlags AState, bool Enable)
		{
			base.SetState( AState, Enable);
			if( AState == StateFlags.Selected)
				SetState( StateFlags.Active, Enable);
			if( (AState == StateFlags.Selected) || ( (AState == StateFlags.Exposed) && ( (State & StateFlags.Selected) != 0)))
			{
			}
		}

        public ScrollBar StandardScrollBar(int AOptions)
        {
            Rect R = GetExtent();
            if ((AOptions & sbVertical) == 0)
                R = new Rect( R.A.X + 2, R.B.Y - 1, R.B.X - 2, R.B.Y);
            else
                R = new Rect( R.B.X - 1, R.A.Y + 1, R.B.X, R.B.Y - 1);
            ScrollBar S = new ScrollBar( R);
            Insert( S);
            if( (AOptions & sbHandleKeyboard) != 0)
                S.Options |= OptionFlags.ofPostProcess;
            return S;
        }
    }
}
