using System;
using TurboVision.Objects;
using System.Runtime.InteropServices;

namespace TurboVision.App
{
    public class Application : Program
	{
		public const int cmNew           = 30;
		public const int cmOpen          = 31;
		public const int cmSave          = 32;
		public const int cmSaveAs        = 33;
		public const int cmSaveAll       = 34;
		public const int cmSaveDone      = 35;
		public const int cmChangeDir     = 36;
		public const int cmDosShell      = 37;
		public const int cmCloseAll      = 38;
		public const int cmDelete        = 39;
		public const int cmEdit          = 40;
		public const int cmAbout         = 41;
		public const int cmDesktopLoad   = 42;
		public const int cmDesktopStore  = 43;
		public const int cmNewDesktop    = 44;
		public const int cmNewMenuBar    = 45;
		public const int cmNewStatusLine = 46;
		public const int cmNewVideo      = 47;

		static Application()
		{
			Drivers.InitMemory();
			Drivers.InitVideo();
			Drivers.SetCrtData();
            Drivers.InitEvents();
		}

		public Application():base()
		{
		}

		public Application( string Title):base( Title)
		{
		}

		public void Cascade()
		{
			Rect R = GetTileRect();
			if( Desktop != null)
				Desktop.Cascade(R);
		}

		public void Tile()
		{
			Rect R = GetTileRect();
			if( Desktop != null)
				Desktop.Tile(R);
		}

		public void DosShell()
		{
		}

		public override void HandleEvent( ref Event Event)
		{
			base.HandleEvent( ref Event);
			switch( Event.What)
			{
				case Event.evCommand :
				switch( Event.Command)
				{
					case cmTile :
						Tile();
						break;
					case cmCascade :
						Cascade();
						break;
					case cmDosShell :
						DosShell();
						break;
					default :
						return;
				}
					ClearEvent( ref Event);
					break;
            }
		}

		public virtual Rect GetTileRect()
		{
			return Desktop.GetExtent();
		}
	}
}
