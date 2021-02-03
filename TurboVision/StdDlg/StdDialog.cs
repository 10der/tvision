using System;

namespace TurboVision.StdDlg
{
	[Flags]
	public enum StdDialogOptions
	{
		fdOkButton   = 0x0001,
		fdOpenButton = 0x0002,
		fdReplaceButton = 0x0004,
		fdClearButton = 0x0008,
		fdHelpButton = 0x0010,
		fdNoLoadDir = 0x0100,
	}

	public class StdDialog
	{
		public const int cmFileOpen    = 800;   // Returned from TFileDialog when Open pressed }
		public const int cmFileReplace = 801;   // Returned from TFileDialog when Replace pressed }
		public const int cmFileClear   = 802;   // Returned from TFileDialog when Clear pressed }
		public const int cmFileInit    = 803;   // Used by TFileDialog internally }
		public const int cmChangeDir   = 804;   // Used by TChDirDialog internally }
		public const int cmRevert      = 805;   // Used by TChDirDialog internally }

		public const int cmFileFocused = 806;    // A new file was focused in the TFileList }
		public const int cmFileDoubleClicked = 807; // A file was selected in the TFileList }

	}
}
