using System;
using TurboVision.Objects;
using TurboVision.App;
using TurboVision.Views;

namespace TurboVision.Menus
{
    [Serializable]
	public class Menu
	{
		protected MenuItem items;
		protected MenuItem @default;
		public virtual MenuItem Items
		{
			get
			{
				return items;
			}
			set
			{
				items = value;
				@default = Items;
			}
		}

		public MenuItem Default 
		{
			get
			{
				if ( @default != null)
					return @default;
				else
					return Items;
			}
			set
			{
				@default = value;
			}
		}

		internal Menu()
		{
		}
	}
		
    [Serializable]
	public class MenuItem
	{
        public MenuItem Next { get; set; }
        public string Name { get; set; }
        public int Command { get; set; }
        public bool Disabled { get; set; }
        public KeyboardKeys KeyCode { get; set; }
        public uint HelpCtx { get; set; }
        public string Param { get; set; }
		public Menu SubMenu;

		public int CNameLen()
		{
			int j = 0;
			foreach( char c in Name)
				if( c != '~')
					j ++;
			return j;
		}

		public int CParamLen()
		{
			int j = 0;
			foreach( char c in Param)
				if( c != '~')
					j ++;
			return j;
		}
	}

    [Serializable]
	public class MenuView : View
	{

		public const uint hcNew = 0xFF01;
		public const uint hcOpen = 0xFF02;
		public const uint hcSave = 0xFF03;
		public const uint hcSaveAs = 0xFF04;
		public const uint hcSaveAll = 0xFF05;
		public const uint hcChangeDir = 0xFF06;
		public const uint hcDosShell = 0xFF07;
		public const uint hcExit = 0xFF08;

		public const uint hcTile         = 0xFF20;
		public const uint hcCascade      = 0xFF21;
		public const uint hcCloseAll     = 0xFF22;
		public const uint hcResize       = 0xFF23;
		public const uint hcZoom         = 0xFF24;
		public const uint hcNext         = 0xFF25;
		public const uint hcPrev         = 0xFF26;
		public const uint hcClose        = 0xFF27;

		public const int cmNew = 30;
		public const int cmOpen = 31;
		public const int cmSave = 32;
		public const int cmSaveAs = 33;
		public const int cmSaveAll = 34;
		public const int cmChangeDir = 35;
		public const int cmDosShell = 36;
		public const int cmCloseAll  = 37;

		public MenuItem Items = null;
		public MenuItem Current = null;
		public Menu Menu  = null;
		public MenuView ParentMenu = null;

        private static uint[] CMenuView = { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

		public MenuView( Rect Bounds):base( Bounds)
		{
			EventMask |= EventMasks.evBroadcast;
		}

        public override uint[] GetPalette()
		{
			return CMenuView;
		}

		public static MenuItem NewItem( string name, string param, KeyboardKeys keyCode, int command, 
			uint aHelpCtx, MenuItem next)
		{
			View MenuEnabler = new View( new Rect());
			if( ( name != "") && ( command != 0))
			{
                return new MenuItem()
                {
                    Next = next,
                    Name = name,
                    Command = command,
                    Disabled = false,
                    KeyCode = keyCode,
                    HelpCtx = aHelpCtx,
                    Param = param
                };
			}
			else
				return next;
		}

		public static MenuItem StdEditMenuItems( MenuItem Next)
		{
			return
				NewItem("~U~ndo", "", KeyboardKeys.AltBack, cmUndo, hcUndo,
				NewLine(
				NewItem("Cu~t~", "Shift+Del", KeyboardKeys.ShiftDel, cmCut, hcCut,
				NewItem("~C~opy", "Ctrl+Ins", KeyboardKeys.CtrlIns, cmCopy, hcCopy,
				NewItem("~P~aste", "Shift+Ins", KeyboardKeys.ShiftIns, cmPaste, hcPaste,
				NewItem("C~l~ear", "Ctrl+Del", KeyboardKeys.CtrlDel, cmClear, hcClear,
				Next))))));
		}

		public static MenuItem StdFileMenuItems( MenuItem Next)
		{
			return
				NewItem("~N~ew", "", KeyboardKeys.NoKey, cmNew, hcNew,
				NewItem("~O~pen...", "F3", KeyboardKeys.F3, cmOpen, hcOpen,
				NewItem("~S~ave", "F2", KeyboardKeys.F2, cmSave, hcSave,
				NewItem("S~a~ve as...", "", KeyboardKeys.NoKey, cmSaveAs, hcSaveAs,
				NewItem("Save a~l~l", "", KeyboardKeys.NoKey, cmSaveAll, hcSaveAll,
				NewLine(
				NewItem("~C~hange dir...", "", KeyboardKeys.NoKey, Application.cmChangeDir, hcChangeDir,
				NewItem("~D~OS shell", "", KeyboardKeys.NoKey, cmDosShell, hcDosShell,
				NewItem("E~x~it", "Alt+X", KeyboardKeys.AltX, cmQuit, hcExit,
				Next)))))))));
		}

		public static MenuItem StdWindowMenuItems( MenuItem Next)
		{
			return
				NewItem("~T~ile", "", KeyboardKeys.NoKey, cmTile, hcTile,
				NewItem("C~a~scade", "", KeyboardKeys.NoKey, cmCascade, hcCascade,
				NewItem("Cl~o~se all", "", KeyboardKeys.NoKey, cmCloseAll, hcCloseAll,
				NewLine(
				NewItem("~S~ize/Move","Ctrl+F5", KeyboardKeys.CtrlF5, cmResize, hcResize,
				NewItem("~Z~oom", "F5", KeyboardKeys.F5, cmZoom, hcZoom,
				NewItem("~N~ext", "F6", KeyboardKeys.F6, cmNext, hcNext,
				NewItem("~P~revious", "Shift+F6", KeyboardKeys.ShiftF6, cmPrev, hcPrev,
				NewItem("~C~lose", "Alt+F3", KeyboardKeys.AltF3, cmClose, hcClose,
				Next)))))))));
		}

		public static Menu NewMenu( MenuItem items)
		{
            return new Menu()
            {
                Items = items,
                Default = items,
            };
		}

		public static MenuItem NewSubMenu( string name, uint aHelpCtx, Menu subMenu, MenuItem next)
		{
			if( (name != "") && ( subMenu != null))
			{
                return new MenuItem()
                {
                    Next = next,
                    Name = name,
                    Command = 0,
                    Disabled = false,
                    HelpCtx = aHelpCtx,
                    SubMenu = subMenu,
                };
			}
			else
				return next;
		}

		public static MenuItem NewLine( MenuItem next)
		{
            return new MenuItem()
            {
                Next = next,
                Name = "",
                HelpCtx = hcNoContext,
            };
		}

		public virtual Rect GetItemRect( MenuItem Item)
		{
			return new Rect();
		}

		public override uint GetHelpCtx()
		{
			MenuView C = this;
			while( (C != null) && (( Current == null) || ( Current.HelpCtx == hcNoContext) || ( Current.Name == "")))
				C = C.ParentMenu;
			if( C != null)
				return C.Current.HelpCtx;
			else
				return hcNoContext;
		}

		internal void UpdateMenu( Menu Menu, ref bool CallDraw)
		{
			MenuItem P = Menu.Items;
			bool CommandState;
			while( P != null)
			{
				if( P.Name != "")
					if( P.Command == 0)
						UpdateMenu( P.SubMenu, ref CallDraw);
					else
					{
						CommandState = CommandEnabled( P.Command);
						if( P.Disabled == CommandState)
						{
							P.Disabled = !CommandState;
							CallDraw = true;
						}
					}
				P = P.Next;
			}
		}

		internal void DoSelect( ref Event Event)
		{
			PutEvent( Event);
			Event.Command = Owner.ExecView( this);
			if( (Event.Command != 0) && ( CommandEnabled( Event.Command)))
			{
				Event.What = Event.evCommand;
				PutEvent( Event);
			}
			ClearEvent( ref Event);
		}

		public override void HandleEvent(ref Event Event)
		{

			bool CallDraw;
			MenuItem P;

			if( Menu != null)
				switch( Event.What)
				{
					case Event.MouseDown :
						DoSelect( ref Event);
						break;
					case Event.KeyDown :
						if( ( FindItem( (char)Program.GetAltChar( (int)Event.KeyCode)) != null))
							DoSelect( ref Event);
						else
						{
							P = HotKey( Event.KeyCode);
							if( (P != null) && CommandEnabled( P.Command))
							{
								Event.What = Event.evCommand;
								Event.Command = P.Command;
								Event.InfoPtr = null;
								PutEvent( Event);
								ClearEvent( ref Event);
							}
						}
						break;
					case Event.evCommand :
						if( Event.Command == cmMenu)
							DoSelect( ref Event);
						break;
					case Event.Broadcast :
						if( Event.Command == cmCommandSetChanged)
						{
							CallDraw = false;
							UpdateMenu( Menu, ref CallDraw);
							if( CallDraw)
								DrawView();
                            ClearEvent(ref Event);
                        }
						break;
				}
		}

		public MenuItem FindItem( char Ch)
		{
			MenuItem P;
			int I;
			Ch = char.ToUpper( Ch);
			P = Menu.Items;
			if( P != null)
			{
				if( (P.Name != "") && (!P.Disabled))
				{
					I = P.Name.IndexOf('~');
					if( (I != -1) && (Ch == P.Name[I + 1]))
						return P;
				}
				P = P.Next;
			}
			return null;
		}

		internal MenuItem FindHotKey( MenuItem P, KeyboardKeys KeyCode)
		{

			MenuItem T;

			while( P != null)
			{
				if( P.Name != "")
					if( P.Command == 0)
					{
						T = FindHotKey( P.SubMenu.Items, KeyCode);
						if( T != null)
							return T;
					}
					else
						if( (!P.Disabled) && (P.KeyCode != KeyboardKeys.NoKey) && (P.KeyCode == KeyCode))
						return P;
				P = P.Next;
			}
			return null;
		}

		public MenuItem HotKey( KeyboardKeys KeyCode)
		{
			return FindHotKey( Menu.Items, KeyCode);
		}

		internal enum MenuAction{ DoNothing, DoSelect, DoReturn }
		internal MenuView TopMenu()
		{
			MenuView P = this;
			while( P.ParentMenu != null)
				P = ParentMenu;
			return P;
		}

		internal bool MouseInOwner( Event E)
		{
			Point Mouse;
			Rect R = new Rect();

			bool Result = false;
			if( (ParentMenu != null) && (ParentMenu.Size.Y == 1))
			{
				Mouse = ParentMenu.MakeLocal( E.Where);
				R = ParentMenu.GetItemRect( ParentMenu.Current);
				Result = R.Contains( Mouse);
			}

			return Result;
		}

		internal void TrackMouse( Event E)
		{
			Point Mouse;
			Rect R = new Rect();
			Mouse = MakeLocal( E.Where);
			Current = Menu.Items;
			while( Current != null)
			{
				R = GetItemRect( Current);
				if( R.Contains( Mouse))
					return;
				Current = Current.Next;
			}
		}

		internal bool MouseInMenus( Event E)
		{
			MenuView P = ParentMenu;
			while( (P != null) && (!P.MouseInView( E.Where)))
				P = P.ParentMenu;
			return P != null;
		}

		internal void NextItem()
		{
			Current = Current.Next;
			if( Current == null)
				Current = Menu.Items;
		}

		internal void PrevItem()
		{
			MenuItem P = Current;
			if( P == Menu.Items)
				P = null;
			do
			{
				NextItem();
			}while( P != Current.Next);
		}

		internal void TrackKey( bool FindNext)
		{
			if( Current != null)
				do
				{
					if( FindNext)
						NextItem();
					else
						PrevItem();
				}while( Current.Name == "");
		}

		public override int Execute()
		{
			bool AutoSelect;
			MenuAction Action;
			char Ch;
			MenuItem ItemShown, P;
			MenuView Target;
			Rect R;
			Event E;
			bool MouseActive;
			int Result;

			AutoSelect = false;
			E.InfoPtr = null;
			ItemShown = null;

			Current = Menu.Default;
			MouseActive = false;
			do
			{
				Action = MenuAction.DoNothing;
				E = GetEvent();
				switch( E.What)
				{
					case Event.MouseDown :
						if( (MouseInView( E.Where)) || ( MouseInOwner( E)))
						{
							TrackMouse( E);
							if( Size.Y == 1)
								AutoSelect = true;
						}
						else
							Action = MenuAction.DoReturn;
						break;
					case Event.MouseUp :
					{
						TrackMouse( E);
						if( MouseInOwner( E))
							Current = Menu.Default;
						else
							if( Current != null)
							if( Current.Name != "")
								Action = MenuAction.DoSelect;
							else
								if( MouseActive || MouseInView( E.Where))
								Action = MenuAction.DoReturn;
							else
							{
								Current = Menu.Default;
								if( Current != null)
									Current = Menu.Items;
								Action = MenuAction.DoNothing;
							}
					}
						break;
					case Event.MouseMove :
						if( E.Buttons != 0)
						{
							TrackMouse( E);
							if( (!(MouseInView( E.Where) || MouseInOwner( E))) && MouseInMenus( E))
								Action = MenuAction.DoReturn;
						}
						break;
					case Event.KeyDown :
                        switch (Drivers.CtrlToArrow(E.KeyCode))
                        {
						case KeyboardKeys.Up :
						case KeyboardKeys.Down :
							if( Size.Y != 1)
                                TrackKey(Drivers.CtrlToArrow(E.KeyCode) == KeyboardKeys.Down);
                            else
                                if (E.KeyCode == KeyboardKeys.Down)
                                AutoSelect = true;
							break;
						case KeyboardKeys.Left :
						case KeyboardKeys.Right :
							if( ParentMenu == null)
                                TrackKey(Drivers.CtrlToArrow(E.KeyCode) == KeyboardKeys.Right);
                            else
								Action = MenuAction.DoReturn;
							break;
						case KeyboardKeys.Home :
						case KeyboardKeys.End :
							if( Size.Y != 1)
							{
								Current = Menu.Items;
                                if (E.KeyCode == KeyboardKeys.End)
                                    TrackKey( false);
							}
							break;
						case KeyboardKeys.Enter :
						{
							if ( Size.Y == 1)
								AutoSelect = true;
							Action = MenuAction.DoSelect;
						}
							break;
						case KeyboardKeys.Esc :
						{
							Action = MenuAction.DoReturn;
							if( (ParentMenu == null) || ( ParentMenu.Size.Y != 1))
								ClearEvent( ref E);
						}
							break;
						default :
							Target = this;
                            Ch = (char)(Program.GetAltChar((int)E.KeyCode));
                            if( Ch == '\x00')
								Ch = (char)E.CharCode;
							else
								Target = TopMenu();
							P = Target.FindItem( Ch);
							if( P == null)
							{
                                P = TopMenu().HotKey(E.KeyCode);
                                if( (P != null) && ( CommandEnabled(P.Command)))
								{
									Result = P.Command;
									Action = MenuAction.DoReturn;
								}
							}
							else
								if( Target == this)
							{
								if( Size.Y == 1)
									AutoSelect = true;
								Action = MenuAction.DoSelect;
								Current = P;
							}
							else
								if( (ParentMenu != Target) || ( ParentMenu.Current != P))
								Action = MenuAction.DoReturn;
							break;
					}
						break;
					case Event.evCommand :
						if( E.Command == cmMenu)
						{
							AutoSelect = false;
							if( ParentMenu != null)
								Action = MenuAction.DoReturn;
						}
						else
							Action = MenuAction.DoReturn;
						break;
				}
				if( ItemShown != Current)
				{
					ItemShown = Current;
					DrawView();
				}
				Result = 0;
				if( (Action == MenuAction.DoSelect) || (( Action == MenuAction.DoNothing) && AutoSelect))
					if( Current != null)
						if( Current.Name != "")
							if( Current.Command == 0)
							{
								if( (E.What & ( Event.MouseDown | Event.MouseMove)) != 0)
									PutEvent( E);
								R = GetItemRect( Current);
								R.A.X += Origin.X;
								R.A.Y = R.B.Y + Origin.Y;
								R.B = new Point( Owner.Size.X, Owner.Size.Y);
								if( Size.Y == 1)
									R.A.X --;
								Target = TopMenu().NewSubView( R, Current.SubMenu, this);
								Result = Owner.ExecView( Target);
								Target.Done();
							}
				else
								if( Action == MenuAction.DoSelect)
								Result = Current.Command;
				if( (Result != 0) && CommandEnabled( Result))
				{
					Action = MenuAction.DoReturn;
					ClearEvent( ref E);
				}
				else
					Result = 0;
			}while( Action != MenuAction.DoReturn);
			if( E.What != Event.Nothing)
				if( (ParentMenu != null) || ( E.What == Event.evCommand))
					PutEvent( E);
			if( Current != null)
			{
				Menu.Default = Current;
				Current = null;
				DrawView();
			}
			return Result;
		}

		public virtual MenuView NewSubView( Rect Bounds, Menu AMenu, MenuView AParentMenu)
		{
			return new MenuBox( Bounds, AMenu, AParentMenu);
		}
	}
}
