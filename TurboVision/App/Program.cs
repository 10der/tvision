using System;
using System.Runtime.InteropServices;
using TurboVision.Objects;
using TurboVision.Menus;
using TurboVision.Dialogs;
using TurboVision.StdDlg;
using TurboVision.Views;
using System.Text;

namespace TurboVision.App
{
    public class Program : Group
    {
        public interface IEventListener
        {
            Event GetEvent();
        }

        public class EventListenersStack : System.Collections.Stack
        {
            public Event GetEvent()
            {
                Event E = new Event();
                E.What = Event.Nothing;
                foreach (object o in this)
                {
                    E = (o as IEventListener).GetEvent();
                    if (E.What != Event.Nothing)
                        return E;
                }
                return E;
            }
        }

        public EventListenersStack EventListeners = new EventListenersStack();

        public enum ScreenModes
		{
			smBW80        = 0x0002,
			smCO80        = 0x0003,
			smMono        = 0x0007,
			smNonStandard = 0x00FF,
			smFont8x8     = 0x0100,
		}

		public enum AppColors
		{
			apColor			= 0,
			apBlackWhite	= 1,
			apMonochrome	= 2,
		}

		public static AppColors AppPalette  = AppColors.apColor;
		public static Desktop Desktop = null;
		public static MenuView MenuBar = null;
		public static StatusLine StatusLine = null;
		public static Program Application =  null;
		private static Event Pending = new Event();

        internal static CHAR_INFO[] SaveConsoleBuffer;

        public static uint[] CAppColor =
        {   0x71, 0x70, 0x78, 0x74, 0x20, 0x28, 0x24, 0x17, 0x1F, 0x1A, 0x31, 0x31, 0x1E, 0x71, 0x1F,
            0x37, 0x3F, 0x3A, 0x13, 0x13, 0x3E, 0x21, 0x3F, 0x70, 0x7F, 0x7A, 0x13, 0x13, 0x70, 0x7F, 0x7E,
            0x70, 0x7F, 0x7A, 0x13, 0x13, 0x70, 0x70, 0x7F, 0x7E, 0x20, 0x2B, 0x2F, 0x78, 0x2E, 0x70, 0x30,
            0x3F, 0x3E, 0x1F, 0x2F, 0x1A, 0x20, 0x72, 0x31, 0x31, 0x30, 0x2F, 0x3E, 0x31, 0x13, 0x38, 0x00,
            0x17, 0x1F, 0x1A, 0x71, 0x71, 0x1E, 0x17, 0x1F, 0x1E, 0x20, 0x2B, 0x2F, 0x78, 0x2E, 0x10, 0x30,
            0x3F, 0x3E, 0x70, 0x2F, 0x7A, 0x20, 0x12, 0x31, 0x31, 0x30, 0x2F, 0x3E, 0x31, 0x13, 0x38, 0x00,
            0x37, 0x3F, 0x3A, 0x13, 0x13, 0x3E, 0x30, 0x3F, 0x3E, 0x20, 0x2B, 0x2F, 0x78, 0x2E, 0x30, 0x70,
            0x7F, 0x7E, 0x1F, 0x2F, 0x1A, 0x20, 0x32, 0x31, 0x71, 0x70, 0x2F, 0x7E, 0x71, 0x13, 0x38, 0x00};

        public static uint[] CAppBlackWhite =
        {   0x70, 0x70, 0x78, 0x7F, 0x07, 0x07, 0x0F, 0x07, 0x0F, 0x07, 0x70, 0x70, 0x07, 0x70, 0x0F,
            0x07, 0x0F, 0x07, 0x70, 0x70, 0x07, 0x70, 0x0F, 0x70, 0x7F, 0x7F, 0x70, 0x07, 0x70, 0x07, 0x0F,
            0x70, 0x7F, 0x7F, 0x70, 0x07, 0x70, 0x70, 0x7F, 0x7F, 0x07, 0x0F, 0x0F, 0x78, 0x0F, 0x78, 0x07,
            0x0F, 0x0F, 0x0F, 0x70, 0x0F, 0x07, 0x70, 0x70, 0x70, 0x07, 0x70, 0x0F, 0x07, 0x07, 0x78, 0x00,
            0x07, 0x0F, 0x0F, 0x07, 0x70, 0x07, 0x07, 0x0F, 0x0F, 0x70, 0x78, 0x7F, 0x08, 0x7F, 0x08, 0x70,
            0x7F, 0x7F, 0x7F, 0x0F, 0x70, 0x70, 0x07, 0x70, 0x70, 0x70, 0x07, 0x7F, 0x70, 0x07, 0x78, 0x00,
            0x70, 0x7F, 0x7F, 0x70, 0x07, 0x70, 0x70, 0x7F, 0x7F, 0x07, 0x0F, 0x0F, 0x78, 0x0F, 0x78, 0x07,
            0x0F, 0x0F, 0x0F, 0x70, 0x0F, 0x07, 0x70, 0x70, 0x70, 0x07, 0x70, 0x0F, 0x07, 0x07, 0x78, 0x00};

        public static uint[] CAppMonochrome =
        {   0x70, 0x07, 0x07, 0x0F, 0x70, 0x70, 0x70, 0x07, 0x0F, 0x07, 0x70, 0x70, 0x07, 0x70, 0x00,
            0x07, 0x0F, 0x07, 0x70, 0x70, 0x07, 0x70, 0x00, 0x70, 0x70, 0x70, 0x07, 0x07, 0x70, 0x07, 0x00,
            0x70, 0x70, 0x70, 0x07, 0x07, 0x70, 0x70, 0x70, 0x0F, 0x07, 0x07, 0x0F, 0x70, 0x0F, 0x70, 0x07,
            0x0F, 0x0F, 0x07, 0x70, 0x07, 0x07, 0x70, 0x07, 0x07, 0x07, 0x70, 0x0F, 0x07, 0x07, 0x70, 0x00,
            0x70, 0x70, 0x70, 0x07, 0x07, 0x70, 0x70, 0x70, 0x0F, 0x07, 0x07, 0x0F, 0x70, 0x0F, 0x70, 0x07,
            0x0F, 0x0F, 0x07, 0x70, 0x07, 0x07, 0x70, 0x07, 0x07, 0x07, 0x70, 0x0F, 0x07, 0x07, 0x70, 0x00,
            0x70, 0x70, 0x70, 0x07, 0x07, 0x70, 0x70, 0x70, 0x0F, 0x07, 0x07, 0x0F, 0x70, 0x0F, 0x70, 0x07,
            0x0F, 0x0F, 0x07, 0x70, 0x07, 0x07, 0x70, 0x07, 0x07, 0x07, 0x70, 0x0F, 0x07, 0x07, 0x70, 0x00};

		static Rect StartupSize = new Rect();

		static Program()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			InitScreen();
			StartupSize = new Rect( 0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
		}

		private void Initialization()
		{
			Application = this;
			State = StateFlags.Visible | 
				StateFlags.Selected | 
				StateFlags.Focused | 
				StateFlags.Modal | 
				StateFlags.Exposed;
			Options = 0;
			this.Buffer = ScreenManager.ScreenBuffer;
			Drivers.SetCursorType( CursorType.Hidden);
			InitDesktop();
			InitStatusLine();
			InitMenuBar();
			if( Desktop != null)
				Insert( Desktop);
			if( StatusLine != null)
				Insert( StatusLine);
			if( MenuBar != null)
				Insert( MenuBar);
		}

		public Program():base( StartupSize)
		{
			Initialization();
		}

		public Program( string Title):base( StartupSize)
		{
			Initialization();
            Windows.SetConsoleTitle(Title);
		}

		public virtual void InitDesktop()
		{
			Rect R = GetExtent();
			R.A.Y ++;
			R.B.Y --;
			Desktop = new Desktop(R);
		}

		public virtual void InitMenuBar()
		{
			Rect R = GetExtent();
			R.B.Y = R.A.Y + 1;
			MenuBar = new MenuBar(R, null);
		}

		public virtual void InitStatusLine()
		{
			Rect R = GetExtent();
			R.A.Y = R.B.Y - 1;
			StatusLine = new StatusLine( R,
				StatusLine.NewStatusDef( 0, 0xFFFF,
					StatusLine.NewStatusKey("~Alt-X~ Exit", KeyboardKeys.AltX, cmQuit,
					StdStatusKeys(null)), null));
		}

		internal static byte GetAltChar( int KeyCode)
		{
			byte[] AltCodes1 = { 0x51, 0x57, 0x45, 0x52, 0x54, 0x59, 0x55, 0x49, 0x4F, 0x50,
                                 0x00, 0x00, 0x00, 0x00, 0x41, 0x53, 0x44, 0x46, 0x47, 0x48,
                                 0x4A, 0x4B, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5A, 0x58,
                                 0x43, 0x56, 0x42, 0x4E, 0x4D};
			byte[] AltCodes2 = { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30,
                                 0x2D, 0x3D}; 
			byte Result = 0;
			if( (KeyCode & 0xFF) == 0)
				switch( (KeyCode >> 8) & 0xFF)
				{
					case 0x02 :
                        Result = 240;
						break;
					default :
						if( (((KeyCode >> 8) & 0xFF) >= 0x10) &&
							(((KeyCode >> 8) & 0xFF) <= 0x32))
							Result = AltCodes1[((KeyCode >> 8) & 0xFF) - 0x10];
						else
						if( (((KeyCode >> 8) & 0xFF) >= 0x78) &&
							(((KeyCode >> 8) & 0xFF) <= 0x83))
							Result = AltCodes2[((KeyCode >> 8) & 0xFF) - 0x78];
						break;
				}
			return Result;
		}

        public delegate void ConsoleEventHandler(object sender, INPUT_RECORD ConsoleEvent);
        public event ConsoleEventHandler ConsoleEvent;

        public override void HandleEvent( ref Event Event)
		{
			if( Event.What == Event.KeyDown)
			{
                byte C = (byte)GetAltChar((int)Event.KeyCode);
                byte P = (byte)(C - 0x30);
				if( (C >= (byte)'1') && ( C <= (byte)'9'))
					if ( Message( Desktop, Event.Broadcast, cmSelectWindowNum, P) != null)
						ClearEvent( ref Event);
			}
			base.HandleEvent( ref Event);
            if (Event.What == Event.Console)
                ConsoleEvent( this, (INPUT_RECORD)Event.InfoPtr);
            if( Event.What == Event.evCommand)
				if( Event.Command == cmQuit)
				{
					EndModal( cmQuit);
					ClearEvent( ref Event);
				}
		}

		public virtual void Run()
		{
			try
			{
				Execute();
			}
			catch( Exception Ex)
			{
				ErrorBox.Show( "Exception : " + Ex.GetType().ToString(), Ex.Message, Ex.StackTrace);
			}
		}

        public override uint[] GetPalette()
		{
            uint[][] P = new uint[3][] { CAppColor, CAppBlackWhite, CAppMonochrome };
			return P[(int)AppPalette];
		}

		public static StatusItem StdStatusKeys( StatusItem Next)
		{
			return
				StatusLine.NewStatusKey("", KeyboardKeys.AltX, cmQuit,
				StatusLine.NewStatusKey("", KeyboardKeys.F10, cmMenu,
				StatusLine.NewStatusKey("", KeyboardKeys.AltF3, cmClose,
				StatusLine.NewStatusKey("", KeyboardKeys.F5, cmZoom,
				StatusLine.NewStatusKey("", KeyboardKeys.CtrlF5, cmResize,
				StatusLine.NewStatusKey("", KeyboardKeys.F6, cmNext,
				StatusLine.NewStatusKey("", KeyboardKeys.ShiftF6, cmPrev,
				Next)))))));

		}

		public StatusDef CreateStatusLine()
		{
			return
				StatusLine.NewStatusDef( 0, 0xFFFF,
				StatusLine.NewStatusKey("~Alt-X~ Exit", KeyboardKeys.AltX, cmQuit,
				StdStatusKeys(null)), null);
		}

		public static void InitScreen()
		{
			if( (Drivers.ScreenMode & 0xFF) != (byte)ScreenModes.smMono)
			{
				if( (Drivers.ScreenMode & (int)(ScreenModes.smFont8x8)) != 0)
					ShadowSize.X = 1;
				else
					ShadowSize.X = 2;
				ShadowSize.Y = 1;
				ShowMarkers = false;
				if( (Drivers.ScreenMode & 0xFF) == (byte)ScreenModes.smBW80)
					AppPalette = AppColors.apBlackWhite;
				else
					AppPalette = AppColors.apColor;
			}
			else
			{
				ShadowSize.X = 0;
				ShadowSize.Y = 0;
				ShowMarkers = true;
				AppPalette = AppColors.apMonochrome;
			}
            CONSOLE_SCREEN_BUFFER_INFO CSBI = new CONSOLE_SCREEN_BUFFER_INFO();
            Windows.GetConsoleScreenBufferInfo(out CSBI);
            SaveConsoleBuffer = new CHAR_INFO[CSBI.dwSize.X * CSBI.dwSize.Y];
            SMALL_RECT ReadRegion = new SMALL_RECT(0, 0, (short)(CSBI.dwSize.X - 1), (short)(CSBI.dwSize.Y - 1));
            ScreenManager.ReadConsoleOutput(
                ref SaveConsoleBuffer, CSBI.dwSize, new COORD(0, 0), ref ReadRegion);
		}

		public override void PutEvent( Event Event)
		{
			Pending = Event;
		}

		protected override Event GetEvent()
		{
			Event Event = new Event();
			try
			{
				if( Pending.What != Event.Nothing)
				{
					Event = Pending;
					Pending.What = Event.Nothing;
				}
				else
				{
					Event = MouseManager.GetMouseEvent();
					if( Event.What == Event.Nothing)
					{
						Event = Drivers.GetKeyEvent();
                        if (Event.What == Event.Nothing)
                            Event = Drivers.GetConsoleEvent();
                        if (Event.What == Event.Nothing)
                            Event = EventListeners.GetEvent();
                        if( Event.What == Event.Nothing)
                            Idle();
					}
				}
				if( StatusLine != null)
					if ((Event.What & Event.KeyDown) != 0)
						StatusLine.HandleEvent( ref Event);
					else if((Event.What & Event.MouseDown) != 0)
					{
						if( FirstThatContainsMouse( 
							new FirstThatContainsMouseProc( ContainsMouse), Event.Where) == StatusLine)
							StatusLine.HandleEvent( ref Event);
					}
			}
			catch(Exception Ex)
			{
				ErrorBox.Show( "Exception : " + Ex.GetType().ToString(), Ex.Message, Ex.StackTrace);
			}
			return Event;
		}

        public virtual void Idle()
		{
			if( StatusLine != null)
				StatusLine.Update();
            System.Threading.Thread.Sleep(1);
            if( CommandSetChanged)
			{
				Message( this, Event.Broadcast, cmCommandSetChanged, null);
				CommandSetChanged = false;
			}
		}

		public Window InsertWindow( Window P)
		{
			Window Result = null;
			if( ValidView(P) != null)
				if ( CanMoveFocus())
				{
					Desktop.Insert( P);
					Result = P;
				}
			else
					P.Done();
			return Result;
		}

		public bool CanMoveFocus()
		{
			return Desktop.Valid( cmReleasedFocus);
		}

		public View ValidView(View P )
		{
			View Result = null;
			if( P != null)
			{
				if( Drivers.LowMemory())
				{
					P.Done();
					OutOfMemory();
					return Result;
				}
				if( !Valid( cmValid))
				{
					P.Done();
					return Result;
				}
				Result = P;
			}
			return Result;
		}

		public virtual void OutOfMemory()
		{
		}

        public int ExecuteDialog(Dialog P)
        {
            object data = new object();
            return ExecuteDialog(P, data);
        }

		public int ExecuteDialog( Dialog P, object Data)
		{
            int C;
            int Result = cmCancel;
            if (ValidView(P) != null)
            {
                if (Data != null)
                    P.SetData(Data);
                C = Desktop.ExecView(P);
                if ((C != cmCancel) && (Data != null))
                    Data = P.GetData();
                P.Done();
                Result = C;
            }
            return Result;
		}

        public virtual void DoneScreen()
        {
            CONSOLE_SCREEN_BUFFER_INFO CSBI = new CONSOLE_SCREEN_BUFFER_INFO();
            Windows.GetConsoleScreenBufferInfo(out CSBI);
            SMALL_RECT ReadRegion = new SMALL_RECT(0, 0, (short)(CSBI.dwSize.X - 1), (short)(CSBI.dwSize.Y - 1));
            ScreenManager.WriteConsoleOutput(
                SaveConsoleBuffer, CSBI.dwSize, new COORD(0, 0), ref ReadRegion);
        }

		public override void Done()
		{
            DoneScreen();
            base.Done();
		}
	}
}
