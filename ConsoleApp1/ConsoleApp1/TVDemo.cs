using System;
using TurboVision;
using TurboVision.App;
using TurboVision.Dialogs;
using TurboVision.Editors;
using TurboVision.FileDialogs;
using TurboVision.Gadgets;
using TurboVision.Menus;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

namespace ConsoleApp1
{
    public class TVDemo : Application
    {

        private const string CHelpColor = "\x37\x3F\x3A\x13\x13\x30\x3E\x1E";
        private const string CHelpBlackWhite = "\x07\x0F\x07\x70\x70\x07\x0F\x70";
        private const string CHelpMonochrome = "\x07\x0F\x07\x70\x70\x07\x0F\x70";

        public const uint hcAsciiTable = 6;
        public const uint hcCalculator = 4;
        public const uint hcCalendar = 5;
        public const uint hcCancelBtn = 30;
        public const uint hcEdit = 14;
        public const uint hcFCChDirDBox = 32;
        public const uint hcFile = 13;
        public const uint hcFind = 17;
        public const uint hcFOFileOpenDBox = 26;
        public const uint hcFOFiles = 28;
        public const uint hcFOName = 27;
        public const uint hcFOOpenBtn = 29;
        public const uint hcOCColorsDBox = 34;
        public const uint hcOColors = 23;
        public const uint hcOMMouseDBox = 33;
        public const uint hcOMouse = 22;
        public const uint hcOpenBtn = 31;
        public const uint hcOptions = 21;
        public const uint hcORestoreDesktop = 25;
        public const uint hcOSaveDesktop = 24;
        public const uint hcPrevious = 65318;
        public const uint hcPuzzle = 3;
        public const uint hcReplace = 18;
        public const uint hcSAbout = 8;
        public const uint hcSAsciiTable = 11;
        public const uint hcSCalculator = 12;
        public const uint hcSCalendar = 10;
        public const uint hcSearch = 16;
        public const uint hcSearchAgain = 19;
        public const uint hcShowClip = 15;
        public const uint hcSPuzzle = 9;
        public const uint hcSystem = 7;
        public const uint hcViewer = 2;
        public const uint hcWindows = 20;

        public const int cmPuzzle = 1003;
        public const int cmCalendar = 1004;
        public const int cmAsciiTab = 1005;
        public const int cmCalculator = 1006;
        public const int cmMouse = 1007;
        public const int cmColors = 1008;
        public const int cmSaveDesktop = 1010;
        public const int cmRetrieveDesktop = 1011;
        public const int cmShowClip = 1012;

        public ClockView Clock = null;
        public HeapView Heap = null;

        public static EditWindow ClipWindow = null;

        public TVDemo(params string[] args)
            : base()
        {
            Rect R = GetExtent();
            R.A.X = R.B.X - 9;
            R.B.Y = R.A.Y + 1;

            Clock = new ClockView(R);
            Insert(Clock);

            R = GetExtent();
            R.B.X--;
            R.A.X = R.B.X - 9; R.A.Y = R.B.Y - 1;

            Heap = new HeapView(R);
            Insert(Heap);

            DisableCommands(cmSave, cmSaveAs, cmCut, cmCopy, cmPaste, cmClear,
                cmUndo, Editor.cmFind, Editor.cmReplace, Editor.cmSearchAgain, cmCloseAll);

            if (ClipWindow != null)
            {
                Editor.Clipboard = ClipWindow.Editor;
                Editor.Clipboard.CanUndo = false;
            }

            if (args != null)
            {
                foreach (string s in args)
                {
                    string FileName = s;
                    if (FileName[FileName.Length - 1] == '\\')
                        FileName = FileName + "*.*";
                    if ((FileName.IndexOf('?') == -1) && (FileName.IndexOf('*') == -1))
                        OpenEditor(System.IO.Path.GetFullPath(FileName), true);
                    else
                        FileOpen(FileName);
                }
            }
        }

        public override uint[] GetPalette()
        {
            uint[] CNewColor = CAppColor;
            uint[] CNewBlackWhite = CAppBlackWhite;
            uint[] CNewMonochrome = CAppMonochrome;
            uint[][] P = new uint[][] { CNewColor, CNewBlackWhite, CNewMonochrome };
            return P[(int)AppPalette];
        }

        public bool IsTileable(View P)
        {
            return (((P.Options & OptionFlags.ofTileable) != 0) && ((P.State & StateFlags.Visible) != 0));
        }

        public override void Idle()
        {
            base.Idle();
            Clock.Update();
            Heap.Update();
            if (Desktop.FirstThat(new FirstThatProc(IsTileable)) != null)
                this.EnableCommands(new int[] { cmTile, cmCascade });
            else
                this.DisableCommands(new int[] { cmTile, cmCascade });
        }

        public void FileOpen(string WildCard)
        {
            string FileName = "*.*";
            Dialog V = new FileDialog(WildCard, "Open a file", "~N~ame", StdDialogOptions.fdOpenButton | StdDialogOptions.fdHelpButton, 100);
            if (ExecuteDialog(V, FileName) != cmCancel)
                OpenEditor(FileName, true);
        }

        public EditWindow OpenEditor(string FileName, bool Visible)
        {
            Rect R = Desktop.GetExtent();
            EditWindow V = new EditWindow(R, FileName, wnNoNumber);
            View P = Application.ValidView(V);
            if (!Visible)
                P.Hide();
            Desktop.Insert(V);
            return (EditWindow)P;
        }

        public void FileNew()
        {
            OpenEditor("", true);
        }

        public void ShowClip()
        {
            ClipWindow.Select();
            ClipWindow.Show();
        }

        public void ChangeDir()
        {
            // tender
            // MsgBox.MessageBox(ConsoleApp3.Resource1.TestLocalizedString + " " + string.Format("{0:c}", 2345.56f));
        }

        public void About()
        {
            Rect R = new Rect(0, 0, 40, 11);
            Dialog D = new Dialog(R, "About");
            D.Options |= OptionFlags.ofCentered;
            R.Grow(-1, -1);
            R.B.Y -= 3;
            View V = new StaticText(R,
                "\x0D\x03" + "Turbo Vision Demo\x0D" +
                "\x0D\x03" + "Copyright (c) 1992\x0D" +
                "\x0D\x03" + "Borland International");
            D.Insert(V);
            R = new Rect(15, 8, 25, 10);
            V = new Button(R, "O~k~", cmOk, Button.ButtonFlags.Default);
            D.Insert(V);
            if (ValidView(D) != null)
            {
                Desktop.ExecView(D);
                D.Done();
            }
        }

        public void Puzzle()
        {
            PuzzleWindow P = new PuzzleWindow();
            P.HelpCtx = hcPuzzle;
            InsertWindow(P);
        }

        public void Calendar()
        {
            CalendarWindow C = new CalendarWindow();
            C.HelpCtx = hcCalendar;
            InsertWindow(C);
        }

        public void AsciiTab()
        {
            AsciiChart P = new AsciiChart();
            P.HelpCtx = hcAsciiTable;
            InsertWindow(P);
        }

        public void Calculator()
        {
            Calculator P = new Calculator();
            P.HelpCtx = hcCalculator;
            InsertWindow(P);
        }

        public void Colors()
        {
            ColorDialog D = new ColorDialog("",
                new ColorGroup("Desktop", ColorItem.DesktopColorItems(null),
                new ColorGroup("Menus", ColorItem.MenuColorItems(null),
                new ColorGroup("Dialogs/Calc", ColorItem.DialogColorItems(DialogPalettes.GrayDialog, null),
                new ColorGroup("Editor/Puzzle", ColorItem.WindowColorItems(WindowPalettes.wpBlueWindow, null),
                new ColorGroup("Ascii table", ColorItem.WindowColorItems(WindowPalettes.wpGrayWindow, null),
                new ColorGroup("Calendar",
                    ColorItem.WindowColorItems(WindowPalettes.wpCyanWindow,
                    new ColorItem("Current day", 22, null)),
                null)))))));
            D.HelpCtx = hcOCColorsDBox;

            object pal = Application.GetPalette();

            if (ExecuteDialog(D, pal) != cmCancel)
            {
                Redraw();
            }
        }

        public void Mouse()
        {
            Dialog D = new MouseDialog();
            D.HelpCtx = hcOMMouseDBox;
            int mouse = W32Kbd.MouseReverse ? 1 : 0;
            ExecuteDialog(D, mouse);
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent(ref Event);
            switch (Event.What)
            {
                case Event.evCommand:
                    switch (Event.Command)
                    {
                        case cmOpen:
                            FileOpen("*.*");
                            break;
                        case cmNew:
                            FileNew();
                            break;
                        case cmShowClip:
                            ShowClip();
                            break;
                        case cmChangeDir:
                            ChangeDir();
                            break;
                        case cmAbout:
                            About();
                            break;
                        case cmPuzzle:
                            Puzzle();
                            break;
                        case cmCalendar:
                            Calendar();
                            break;
                        case cmAsciiTab:
                            AsciiTab();
                            break;
                        case cmCalculator:
                            Calculator();
                            break;
                        case cmColors:
                            Colors();
                            break;
                        case cmMouse:
                            Mouse();
                            break;
                        default:
                            return;
                    }
                    ClearEvent(ref Event);
                    break;
            }
        }

        public override void InitStatusLine()
        {
            Rect R = GetExtent();
            R.A.Y = R.B.Y - 1;
            StatusLine = new StatusLine(R,
                StatusLine.NewStatusDef(0, 0xFFFF,
                    StatusLine.NewStatusKey("~Alt-X~ Exit", KeyboardKeys.AltX, cmQuit,
                    StatusLine.NewStatusKey("~F1~ Help", KeyboardKeys.F1, cmHelp,
                    StatusLine.NewStatusKey("~F3~ Open", KeyboardKeys.F3, cmOpen,
                    StatusLine.NewStatusKey("~Alt-F3~ Close", KeyboardKeys.AltF3, cmClose,
                    StatusLine.NewStatusKey("~F5~ Zoom", KeyboardKeys.F5, cmZoom,
                    StatusLine.NewStatusKey("~F10~ Menu", KeyboardKeys.F10, cmMenu,
                    StatusLine.NewStatusKey("", KeyboardKeys.CtrlF5, cmResize,
                    null))))))),
                null));
        }

        public override void InitMenuBar()
        {
            Rect R = GetExtent();
            R.B.Y = R.A.Y + 1;
            MenuBar = new MenuBar(R, MenuView.NewMenu(
        MenuView.NewSubMenu("~■~", hcSystem, MenuView.NewMenu(
            MenuView.NewItem("~A~bout", "", KeyboardKeys.NoKey, cmAbout, hcSAbout,
            MenuView.NewLine(
            MenuView.NewItem("~P~uzzle", "", KeyboardKeys.NoKey, cmPuzzle, hcSPuzzle,
            MenuView.NewItem("Ca~l~endar", "", KeyboardKeys.NoKey, cmCalendar, hcSCalendar,
            MenuView.NewItem("Ascii ~t~able", "", KeyboardKeys.NoKey, cmAsciiTab, hcSAsciiTable,
            MenuView.NewItem("~C~alculator", "", KeyboardKeys.NoKey, cmCalculator, hcCalculator, null))))))),
        MenuView.NewSubMenu("~F~ile", hcFile, MenuView.NewMenu(
            MenuView.StdFileMenuItems(null)),
        MenuView.NewSubMenu("~E~dit", hcEdit, MenuView.NewMenu(
            MenuView.StdEditMenuItems(
            MenuView.NewLine(
            MenuView.NewItem("~S~how clipboard", "", KeyboardKeys.NoKey, cmShowClip, hcShowClip,
            null)))),
        MenuView.NewSubMenu("~S~earch", hcSearch, MenuView.NewMenu(
            MenuView.NewItem("~F~ind...", "", KeyboardKeys.NoKey, Editor.cmFind, hcFind,
            MenuView.NewItem("~R~eplace...", "", KeyboardKeys.NoKey, Editor.cmReplace, hcReplace,
            MenuView.NewItem("~S~earch again", "", KeyboardKeys.NoKey, Editor.cmSearchAgain, hcSearchAgain,
            null)))),
        MenuView.NewSubMenu("~W~indow", hcWindows, MenuView.NewMenu(
            MenuView.StdWindowMenuItems(null)),
        MenuView.NewSubMenu("~O~ptions", hcOptions, MenuView.NewMenu(
            MenuView.NewItem("~M~ouse...", "", KeyboardKeys.NoKey, cmMouse, hcOMouse,
            MenuView.NewItem("~C~olors...", "", KeyboardKeys.NoKey, cmColors, hcOColors,
            MenuView.NewLine(
            MenuView.NewItem("~S~ave desktop", "", KeyboardKeys.NoKey, cmSaveDesktop, hcOSaveDesktop,
            MenuView.NewItem("~R~etrieve desktop", "", KeyboardKeys.NoKey, cmRetrieveDesktop, hcORestoreDesktop, null)))))),
            null))))))));
        }

        public static int DoEditDialog(int Dialog, object Info)
        {
            Rect R;
            Point T;
            Dialog D;
            switch (Dialog)
            {
                case Editor.edOutOfMemory:
                    return MsgBox.MessageBox("Not enough memory for this operation.",
                        MessageBoxFlags.mfError | MessageBoxFlags.mfOKButton, null);
                case Editor.edReadError:
                    return MsgBox.MessageBox("Error reading file {0}.",
                        MessageBoxFlags.mfError | MessageBoxFlags.mfOKButton, Info);
                case Editor.edWriteError:
                    return MsgBox.MessageBox("Error writing file {0}.",
                        MessageBoxFlags.mfError | MessageBoxFlags.mfOKButton, Info);
                case Editor.edCreateError:
                    return MsgBox.MessageBox("Error creating file {0}.",
                        MessageBoxFlags.mfError | MessageBoxFlags.mfOKButton, Info);
                case Editor.edSaveModify:
                    return MsgBox.MessageBox("{0} has been modified. Save?",
                        MessageBoxFlags.mfInformation | MessageBoxFlags.mfYesNoCancel, Info);
                case Editor.edSaveUntitled:
                    return MsgBox.MessageBox("Save untitled file?",
                        MessageBoxFlags.mfInformation | MessageBoxFlags.mfYesNoCancel);
                case Editor.edSaveAs:
                    D = new FileDialog("*.*",
                        "Save file as", "~N~ame", StdDialogOptions.fdOkButton, 101);
                    return Application.ExecuteDialog(D);
                case Editor.edFind:
                    return Application.ExecuteDialog(CreateFindDialog(), Info);
                case Editor.edSearchFailed:
                    return MsgBox.MessageBox("Search string not found.",
                        MessageBoxFlags.mfError | MessageBoxFlags.mfOKButton);
                //case Editor.edReplace :
                //	return Application.ExecuteDialog( Editor.CreateReplaceDialog(), Info);
                case Editor.edReplacePrompt:
                    R = new Rect(0, 1, 40, 8);
                    R.Move((Desktop.Size.X - R.B.X) / 2, 0);
                    T = Desktop.MakeGlobal(R.B);
                    T.Y++;
                    if (((Point)Info).Y <= T.Y)
                        R.Move(0, Desktop.Size.Y - R.B.Y - 2);
                    return MsgBox.MessageBoxRect(R, "Replace this occurence ?",
                        MessageBoxFlags.mfYesNoCancel | MessageBoxFlags.mfInformation);
                default: return 0;
            }
        }

        private static Dialog CreateFindDialog()
        {
            throw new NotImplementedException();
        }
    }
}
