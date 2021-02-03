#region Using directives

using System;
#if FRAMEWORK20
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Text;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

#endregion

namespace TurboVision.Editors
{
    public class Editor : View
    {

        public static uint[] CEditor = { 0x06, 0x07 };

        public const int cmCharLeft    = 500;
        public const int cmCharRight   = 501;
        public const int cmWordLeft    = 502;
        public const int cmWordRight   = 503;
        public const int cmLineStart   = 504;
        public const int cmLineEnd     = 505;
        public const int cmLineUp      = 506;
        public const int cmLineDown    = 507;
        public const int cmPageUp      = 508;
        public const int cmPageDown    = 509;
        public const int cmTextStart   = 510;
        public const int cmTextEnd     = 511;
        public const int cmNewLine     = 512;
        public const int cmBackSpace   = 513;
        public const int cmDelChar     = 514;
        public const int cmDelWord     = 515;
        public const int cmDelStart    = 516;
        public const int cmDelEnd      = 517;
        public const int cmDelLine     = 518;
        public const int cmInsMode     = 519;
        public const int cmStartSelect = 520;
        public const int cmHideSelect  = 521;
        public const int cmIndentMode  = 522;
        public const int cmUpdateTitle = 523;

        public const byte ufUpdate = 0x01;
        public const byte ufLine   = 0x02;
        public const byte ufView   = 0x04;

        public const int edOutOfMemory   = 0;
        public const int edReadError     = 1;
        public const int edWriteError    = 2;
        public const int edCreateError   = 3;
        public const int edSaveModify    = 4;
        public const int edSaveUntitled  = 5;
        public const int edSaveAs        = 6;
        public const int edFind          = 7;
        public const int edSearchFailed  = 8;
        public const int edReplace       = 9;
        public const int edReplacePrompt = 10;

        public const int cmFind = 82;
        public const int cmReplace = 83;
        public const int cmSearchAgain = 84;

        public ScrollBar HScrollBar = null;
        public ScrollBar VScrollBar = null;
        public Indicator Indicator = null;

        public int BufSize = 0;
        public int BufLen = 0;
        public int CurPtr = 0;
        public int DelCount = 0;
        public int GapLen = 0;
        public int SelStart = 0;
        public int SelEnd = 0;
        public Point CurPos = new Point(0, 0);
        public Point Delta = new Point(0, 0);
        public Point Limit = new Point(0, 0);
        public int DrawLine = 0;
        public int DrawPtr = 0;
        public int InsCount = 0;
        public bool Modified = false;
        public bool IsClipBoard = false;
        public int TabSize = 0;
        public bool CanUndo = false;
        public bool Selecting = false;
        public bool Overwrite = false;

        public const int MaxLineLength = 4096;

        protected char[] buffer = null;
        public bool IsValid = true;
        public delegate int EditorDialog(int dialog, params object[] info);
        public EditorDialog editorDialog;
        public byte updateFlags = 0;
        public bool AutoIndent = false;

        private byte LockCount = 0;
        private int KeyState = 0;
        static Editor ClipBoard = null;

        private const int smExtend = 0x01;
        private const int smDouble = 0x02;

        public const int efCaseSensitive   = 0x0001;
        public const int efWholeWordsOnly  = 0x0002;
        public const int efPromptOnReplace = 0x0004;
        public const int efReplaceAll      = 0x0008;
        public const int efDoReplace       = 0x0010;
        public const int efBackupFiles     = 0x0100;
        public const uint sfSearchFailed    = 0x0FFFFFFF;


        public static Editor Clipboard = null;

        public Editor( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, Indicator AIndicator, int ABufSize)
            :base( Bounds)
        {
            GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
            Options |= OptionFlags.ofSelectable;
            EventMask = (EventMasks)(EventMasks.evMouseDown | EventMasks.evKeyDown | EventMasks.evCommand | EventMasks.evBroadcast);
            ShowCursor();
            HScrollBar = AHScrollBar;
            VScrollBar = AVScrollBar;
            Indicator = AIndicator;
            BufSize = ABufSize;
            CanUndo = true;
            InitBuffer();
            if (buffer != null)
                IsValid = true;
            else
            {
                editorDialog(edOutOfMemory, null);
                BufSize = 0;
            }
            TabSize = 8;
            SetBufLen(0);
        }

        public virtual bool SetBufSize(int NewSize)
        {
            return NewSize <= BufSize;
        }

        public void Update( byte AFlags)
        {
            updateFlags |= AFlags;
            if( LockCount == 0)
                DoUpdate();
        }

        public void SetBufLen(int Length)
        {
            BufLen = Length;
            GapLen = BufSize - Length;
            SelStart = 0;
            SelEnd = 0;
            CurPtr = 0;
            CurPos.X = 0; CurPos.Y = 0;
            Delta.X = 0; Delta.Y = 0;
            Limit.X = MaxLineLength;
            Limit.Y = (int)CountLines(GapLen, BufLen) + 1;
            DrawLine = 0;
            DrawPtr = 0;
            DelCount = 0;
            InsCount = 0;
            Modified = false;
            Update(ufView);
        }

        private int CountLines(long StartPos, long Count)
        {
            long p;
            int lines;
            p = StartPos;
            lines = 0;
            while (Count > 0)
            {
                if ((buffer[p] == 0x0D) || (buffer[p] == 0x0A))
                {
                    lines++;
                    if ((buffer[p] == 0x0D) && (buffer[p + 1] == 0x0A))
                    {
                        p++;
                        Count--;
                        if (Count == 0)
                            break;
                    }
                }
                p++;
                Count--;
            }
            return lines;
        }

        public void DoUpdate()
        {
            if (updateFlags != 0)
            {
                SetCursor(CurPos.X - Delta.X, CurPos.Y - Delta.Y);
                if ((updateFlags & ufView) != 0)
                    DrawView();
                else if ((updateFlags & ufLine) != 0)
                    DrawLines(CurPos.Y - Delta.Y, 1, LineStart(CurPtr));
                if (HScrollBar != null)
                    HScrollBar.SetParams(Delta.X, 0, Limit.X - Size.X, Size.X / 2, 1);
                if (VScrollBar != null)
                    VScrollBar.SetParams(Delta.Y, 0, Limit.Y - Size.Y, Size.Y - 1, 1);
                if (Indicator != null)
                    Indicator.SetValue(CurPos, Modified);
                if ((State & StateFlags.Active) != 0)
                    UpdateCommands();
                updateFlags = 0;
            }
        }

        public virtual void UpdateCommands()
        {
            SetCmdState(cmUndo, (DelCount != 0) || (InsCount != 0));
            if (!IsClipBoard)
            {
                SetCmdState(cmCut, HasSelection());
                SetCmdState(cmCopy, HasSelection());
                SetCmdState(cmPaste, (ClipBoard != null) && (ClipBoard.HasSelection()));
            }
            SetCmdState(cmClear, HasSelection());
            SetCmdState(cmFind, true);
            SetCmdState(cmReplace, true);
            SetCmdState(cmSearchAgain, true);
        }

        public void SetCmdState(int Command, bool Enable)
        {
            int[] S = new int[1]{Command};
            if (Enable && ((State & StateFlags.Active) != 0))
                EnableCommands(S);
            else
                DisableCommands(S);
        }

        public bool HasSelection()
        {
            return SelStart != SelEnd;
        }

        public void FormatLine(DrawBuffer DrawBuf, int LinePtr, int Width, ushort Colors)
        {
            int OutCnt = 0;
            int OutPtr = 0;
            int idxPos = LinePtr;
            ushort attr = (ushort)((Colors & 0x00FF));
            if (FormatUntil((int)SelStart, DrawBuf, Width, ref OutCnt, ref OutPtr, ref idxPos, attr))
                return;
            attr = (ushort)((Colors & 0xFF00) >> 8);
            if (FormatUntil((int)CurPtr, DrawBuf, Width, ref OutCnt, ref OutPtr, ref idxPos, attr))
                return;
            idxPos += (int)GapLen;
            if (FormatUntil((int)(SelEnd + GapLen), DrawBuf, Width, ref OutCnt, ref OutPtr, ref idxPos, attr))
                return;
            attr = (ushort)((Colors & 0x00FF));
            if (FormatUntil((int)BufSize, DrawBuf, Width, ref OutCnt, ref OutPtr, ref idxPos, attr))
                return;
        }

        private bool FormatUntil(int EndPos, DrawBuffer DrawBuf, int Width, ref int OutCnt, ref int OutPtr, ref int idxPos, ushort attr)
        {
            // FormatUntil

            int P;
            bool FormatUntil = false;
            P = idxPos;
            int Cnt = OutCnt;
            int Ptr = OutPtr;
            FillSpace(Width - OutCnt, ref Cnt, ref Ptr, attr, DrawBuf);
            while (EndPos > idxPos)
            {
                if (OutCnt > Width)
                    return FormatUntil;
                switch (buffer[P])
                {
                    case '\x0009':
                        FillSpace(TabSize - (OutCnt % TabSize), ref OutCnt, ref OutPtr, attr, DrawBuf);
                        break;
                    case '\x000D':
                    case '\x000A':
                        FillSpace(Width - OutCnt, ref OutCnt, ref OutPtr, attr, DrawBuf);
                        FormatUntil = true;
                        return FormatUntil;
                    default:
                        OutCnt++;
                        DrawBuf.drawBuffer[OutPtr].AsciiChar = buffer[P];
                        DrawBuf.drawBuffer[OutPtr].Attribute = attr;
                        OutPtr++;
                        break;
                }
                P++;
                idxPos++;
            }
            return FormatUntil;
        }

        private void FillSpace(int i, ref int OutCnt, ref int OutPtr, ushort attr, DrawBuffer DrawBuf)
        {
            OutCnt += i;
            while (i > 0)
            {
                DrawBuf.drawBuffer[OutPtr].AsciiChar = ' ';
                DrawBuf.drawBuffer[OutPtr].Attribute = attr;
                OutPtr++;
                i--;
            }
        }

        public int LineStart(int P)
        {
            int i = 0;
            int start = 0;
            int pc = 0;
            int LineStart;

            if (P > CurPtr)
            {
                start = (int)GapLen;
                pc = start;
                i = (int)(P - CurPtr);
                pc--;
                while (i > 0)
                {
                    if ((buffer[pc] == '\x000A') || (buffer[pc] == '\x000D'))
                        break;
                    pc--;
                    i--;
                }
            }
            else
                i = 0;
            if (i == 0)
            {
                start = 0;
                i = P;
                pc = start + P;
                pc--;
                while (i > 0)
                {
                    try
                    {
                        if ((buffer[pc] == '\x000A') || (buffer[pc] == '\x000D'))
                            break;
                    }
                    catch (Exception Ex)
                    {
                        ErrorBox.Show(Ex.GetType().ToString(), Ex.Message, pc.ToString()+"\n"+Ex.StackTrace);
                    }
                    pc--;
                    i--;
                }
                if (i == 0)
                {
                    LineStart = 0;
                    return LineStart;
                }
            }
            LineStart = pc - start + 1;
            return LineStart;
        }

        public int NextLine(int P)
        {
            return NextChar(LineEnd(P));
        }

        public int NextChar(int P)
        {
            int pc = 0;
            if (P != BufLen)
            {
                P++;
                if (P != BufLen)
                {
                    pc = 0;
                    if (P >= CurPtr)
                        pc += (int)GapLen;
                    pc += P - 1;
                    if ((buffer[pc] == '\x000D') && (buffer[pc + 1] == '\x000A'))
                        P++;
                }
            }
            return P;
        }

        public int LineEnd(int P)
        {
            int start;
            int i;
            int pc;
            int LineEnd = 0;

            if (P < CurPtr)
            {
                i = (int)CurPtr - P;
                pc = P;
                while (i > 0)
                {
                    if ((buffer[pc] == '\x000D') || (buffer[pc] == '\x000A'))
                    {
                        LineEnd = pc;
                        return LineEnd;
                    }
                    pc++;
                    i--;
                }
                start = (int)CurPtr;
            }
            else
                start = P;
            i = (int)BufLen - start;
            pc = (int)(GapLen + start);
            while (i > 0)
            {
                if ((buffer[pc] == '\x000D') || (buffer[pc] == '\x000A'))
                {
                    LineEnd = pc - (int)GapLen;
                    return LineEnd;
                }
                pc++;
                i--;
            }
            LineEnd = pc - (int)GapLen;
            return LineEnd;
        }

        public virtual void InitBuffer()
        {
            buffer = new char[BufSize];
        }

        public char BufChar(int P)
        {
            if (P > CurPtr)
                P += (int)GapLen;
            return buffer[P];
        }

        public int BufPtr(int P)
        {
            if (P > CurPtr)
                return P + (int)GapLen;
            else
                return P;
        }

        public override void ChangeBounds(Rect R)
        {
            SetBounds( R);
            Delta.X = Math.Max(0, Math.Min(Delta.X, Limit.X - Size.X));
            Delta.Y = Math.Max(0, Math.Min(Delta.Y, Limit.Y - Size.Y));
            Update(ufView);
        }

        public virtual void ConvertEvent(ref Event Event)
        {
            if (Event.What == Event.KeyDown)
            {
                if (((Drivers.GetShiftState() & 0x03) != 0) &&
                    (Event.ScanCode >= 0x47) && (Event.ScanCode <= 0x51))
                    Event.CharCode = 0;
                KeyboardKeys Key = Event.KeyCode;
                if (KeyState != 0)
                {
                    if ((((int)Key & 0xFF) >= 0x01) & (((int)Key & 0xFF) <= 0x1A))
                        Key += 0x40;
                    if ((((int)Key & 0xFF) >= 0x61) & (((int)Key & 0x7A) <= 0x1A))
                        Key -= 0x20;
                }
                Key = (KeyboardKeys)ScanKeyMap(KeyMap[KeyState], (uint)Key);
                KeyState = 0;
                if (Key != 0)
                    if ((((int)Key & 0xFF00) >> 8) == 0xFF)
                    {
                        KeyState = ((int)Key & 0xFF);
                        ClearEvent(ref Event);
                    }
                    else
                    {
                        Event.What = Event.evCommand;
                        Event.Command = (int)Key;
                    }
            }
        }

        public static uint[] FirstKeys = new uint[]{
                39,
                1,  cmWordLeft,     3,  cmPageDown,
                4,  cmCharRight,    5,  cmLineUp,
                6,  cmWordRight,    7,  cmDelChar,
                8,  cmBackSpace,    11, 0xFF02,
                12, cmSearchAgain,  13, cmNewLine,
                15, cmIndentMode,   17, 0xFF01,
                18, cmPageUp,       19, cmCharLeft,
                20, cmDelWord,      21, 0,
                22, cmInsMode,      24, cmLineDown,
                25, cmDelLine,      (uint)KeyboardKeys.Left,  cmCharLeft,
                (uint)KeyboardKeys.Right     , cmCharRight,  (uint)KeyboardKeys.CtrlLeft, cmWordLeft,
                (uint)KeyboardKeys.CtrlRight , cmWordRight,  (uint)KeyboardKeys.Home,     cmLineStart,
                (uint)KeyboardKeys.End       , cmLineEnd,    (uint)KeyboardKeys.Up,       cmLineUp,
                (uint)KeyboardKeys.Down      , cmLineDown,   (uint)KeyboardKeys.PageUp,     cmPageUp,
                (uint)KeyboardKeys.PageDown      , cmPageDown,   (uint)KeyboardKeys.CtrlPageUp, cmTextStart,
                (uint)KeyboardKeys.CtrlPageDown  , cmTextEnd,    (uint)KeyboardKeys.Ins,      cmInsMode,
                (uint)KeyboardKeys.Del       , cmDelChar,    (uint)KeyboardKeys.ShiftIns, cmPaste,
                (uint)KeyboardKeys.ShiftDel  , cmCut,        (uint)KeyboardKeys.CtrlIns,  cmCopy,
                (uint)KeyboardKeys.CtrlDel   , cmClear,      (uint)KeyboardKeys.AltBack,  cmUndo
        };

        public static uint[] QuickKeys = new uint[]{
            8,
                (uint)'A', cmReplace,   (uint)'C', cmTextEnd,
                (uint)'D', cmLineEnd,   (uint)'F', cmFind,
                (uint)'H', cmDelStart,  (uint)'R', cmTextStart,
                (uint)'S', cmLineStart, (uint)'Y', cmDelEnd
        };

        public static uint[] BlockKeys = new uint[]{
            5,
            (uint)'B', cmStartSelect,   (uint)'C', cmPaste,
            (uint)'H', cmHideSelect,    (uint)'K', cmCopy,
            (uint)'Y', cmCut
        };

        public uint[][] KeyMap = new uint[][] { FirstKeys, QuickKeys, BlockKeys };

        private uint ScanKeyMap(uint[] KeyMap, uint KeyCode)
        {
            uint Key;
            for (int i = 1; i < KeyMap[0]; i++)
            {
                Key = KeyMap[i * 2 - 1];
                if (((Key & 0xFF) == (KeyCode & 0xFF)) && ((Key & 0xFF00) == 0) || ((Key & 0xFF00) == (KeyCode & 0xFF00)))
                {
                    return KeyMap[i * 2];
                }
            }
            return 0;
        }

        public bool CursorVisible()
        {
            return (CurPos.Y >= Delta.Y) && (CurPos.Y < (Delta.Y + Size.Y));
        }

        public void DeleteSelect()
        {
            InsertText(null, 0, false);
        }

        public bool InsertText(char[] Text, int Length, bool SelectText)
        {
            return InsertBuffer(ref Text, 0, Length, CanUndo, SelectText);
        }

        public bool InsertBuffer(ref char[] P, int Offset, int Length, bool AllowUndo, bool SelectText)
        {
            int SelLen, DelLen, SelLines, Lines;
            int NewSize;

            bool InsertBuffer = true;

            Selecting = false;
            SelLen = SelEnd - SelStart;
            if ((SelLen == 0) && (Length == 0))
                return InsertBuffer;
            DelLen = 0;
            if (AllowUndo)
                if (CurPtr == SelStart)
                    DelLen = SelLen;
                else
                    if (SelLen > InsCount)
                    DelLen = SelLen - InsCount;
            NewSize = BufLen + DelCount - SelLen + DelLen + Length;
            if( NewSize > ( BufLen + DelCount))
                if (!SetBufSize(NewSize))
                {
                    editorDialog(edOutOfMemory, null);
                    InsertBuffer = false;
                    SelEnd = SelStart;
                    return InsertBuffer;
                }
            SelLines = CountLines(BufPtr((int)SelStart), SelLen);
            if (CurPtr == SelEnd)
            {
                if (AllowUndo)
                {
                    if (DelLen > 0)
                        Array.Copy(buffer, SelStart, buffer, CurPtr + GapLen - DelCount - DelLen, DelLen);
                    InsCount -= SelLen - DelLen;
                }
                CurPtr = SelStart;
                CurPos.Y -= (int)SelLines;
            }
            if (Delta.Y > CurPos.Y)
            {
                Delta.Y -= (int)SelLines;
                if (Delta.Y < CurPos.Y)
                    Delta.Y = CurPos.Y;
            }
            if (Length > 0)
                Array.Copy(P, Offset, buffer, CurPtr, Length);
            Lines = CountLines(CurPtr, Length);
            CurPtr += Length;
            CurPos.Y += (int)Lines;
            DrawLine = CurPos.Y;
            DrawPtr = LineStart((int)CurPtr);
            CurPos.X = CharPos(DrawPtr, CurPtr);
            if (!SelectText)
                SelStart = CurPtr;
            SelEnd = CurPtr;
            BufLen += Length - SelLen;
            GapLen -= Length - SelLen;
            if (AllowUndo)
            {
                DelCount += DelLen;
                InsCount += Length;
            }
            Limit.Y += (int)(Lines - SelLines);
            Delta.Y = Math.Max(0, Math.Min(Delta.Y, Limit.Y - Size.Y));
            if (!IsClipBoard)
                Modified = true;
            SetBufSize(BufLen + DelCount);
            if ((SelLines == 0) && (Lines == 0))
                Update(ufLine);
            else
                Update(ufView);
            return InsertBuffer;
        }

        public virtual void DoneBuffer()
        {
            if (buffer != null)
                buffer = null;
        }

        public override void Draw()
        {
            if (DrawLine != Delta.Y)
            {
                DrawPtr = LineMove((int)DrawPtr, Delta.Y - DrawLine);
                DrawLine = Delta.Y;
            }
            DrawLines(0, Size.Y, (int)DrawPtr);
        }

        public int LineMove(int P, int Count)
        {
            int Pos;
            int I;
            int LineMove;

            I = P;
            P = LineStart(P);
            Pos = CharPos(P, I);
            while (Count != 0)
            {
                I = P;
                if (Count < 0)
                {
                    P = PrevLine(P);
                    Count++;
                }
                else
                {
                    P = NextLine(P);
                    Count--;
                }
            }
            if (P != I)
                P = CharPtr(P, Pos);
            LineMove = P;
            return LineMove;
        }

        public void DrawLines(int Y, int Count, int LinePtr)
        {
            uint Color;
            DrawBuffer B = new DrawBuffer(MaxLineLength);
            Color = GetColor(0x0201);
            while (Count > 0)
            {
                FormatLine(B, (int)LinePtr, Delta.X + Size.X, (ushort)Color);
                WriteBuf(0, Y, Size.X, 1, B, Delta.X);
                LinePtr = NextLine(LinePtr);
                Y++;
                Count--;
            }
        }

        private int CharPos(int P, int Target)
        {
            int Pos = 0;
            while (P < Target)
            {
                if (BufChar(P) == '\x0009')
                    Pos = Pos | 7;
                Pos++;
                P++;
            }
            return Pos;
        }

        private int PrevLine(int P)
        {
            return LineStart(PrevChar(P));
        }

        private int CharPtr(int P, int Target)
        {
            int Pos = 0;
            while ((Pos < Target) && (P < BufLen) && (BufChar(P) != '\x000D'))
            {
                if (BufChar(P) == '\x0009')
                    Pos = Pos | 7;
                Pos++;
                P++;
            }
            if (Pos > Target)
                P--;
            return P;
        }

        private int PrevChar(int P)
        {
            int pc;
            if (P != 0)
            {
                P--;
                if (P != 0)
                {
                    pc = 0;
                    if (P >= CurPtr)
                        pc += (int)GapLen;
                    pc += P - 1;
                    if ((buffer[pc] == '\x000D') && (buffer[pc + 1] == '\x000A'))
                        P--;
                }
            }
            return P;
        }


        public override uint[] GetPalette()
        {
            return CEditor;
        }

        public override void HandleEvent(ref Event Event)
        {
            Point Mouse;
            Point D;
            bool CenterCursor = false;
            byte selectMode = 0;

            base.HandleEvent( ref Event);
            ConvertEvent(ref Event);
            CenterCursor = !CursorVisible();
            selectMode = 0;
            if (Selecting || ((Drivers.GetShiftState() & 0x03) != 0))
                selectMode = smExtend;
            switch (Event.What)
            {
                case Event.MouseDown :
                    if (Event.Double)
                        selectMode |= smDouble;
                    do
                    {
                        Lock();
                        if (Event.What == Event.MouseAuto)
                        {
                            Mouse = MakeLocal(Event.Where);
                            D = Delta;
                            if (Mouse.X < 0)
                                D.X--;
                            if (Mouse.X >= Size.X)
                                D.X++;
                            if (Mouse.Y < 0)
                                D.Y--;
                            if (Mouse.Y >= Size.Y)
                                D.Y++;
                            ScrollTo(D.X, D.Y);
                        }
                        SetCurPtr(GetMousePtr(Event.Where), selectMode);
                        selectMode |= smExtend;
                        UnLock();
                    } while (!MouseEvent(ref Event, Event.MouseMove | Event.MouseAuto));
                    break;
                case Event.KeyDown :
                    if ((Event.KeyEvent.UnicodeChar == '\x0009') || (Event.KeyEvent.UnicodeChar >= '\x0020'/* && Event.KeyEvent.UnicodeChar <= '\x00FF'*/))
                    {
                        Lock();
                        if (Overwrite && (!HasSelection()))
                            if (CurPtr != LineEnd((int)CurPtr))
                                SelEnd = NextChar((int)CurPtr);
                        char c = (char)Event.KeyEvent.UnicodeChar;
                        InsertText(new char[] { Event.KeyEvent.UnicodeChar }, 1, false);
                        TrackCursor(CenterCursor);
                        UnLock();
                    }
                    else
                        return;
                    break;
                case Event.evCommand :
                    switch (Event.Command)
                    {
                        case cmFind :
//                            Find();
//                            break;
//                        case cmReplace :
//                            Replace();
//                            break;
//                        case cmSearchAgain :
//                            SearchAgain();
//                            break;
                        default :
                            Lock();
                            switch (Event.Command)
                            {
                                case cmCut :
                                    ClipCut();
                                    break;
                                case cmCopy :
                                    ClipCopy();
                                    break;
                                case cmPaste :
                                    ClipPaste();
                                    break;
                                case cmUndo :
                                    Undo();
                                    break;
                                case cmClear :
                                    DeleteSelect();
                                    break;
                                case cmCharLeft :
                                    SetCurPtr(PrevChar((int)CurPtr), selectMode);
                                    break;
                                case cmCharRight :
                                    SetCurPtr(NextChar((int)CurPtr), selectMode);
                                    break;
                                case cmWordLeft :
                                    SetCurPtr(PrevWord((int)CurPtr), selectMode);
                                    break;
                                case cmWordRight :
                                    SetCurPtr(NextWord((int)CurPtr), selectMode);
                                    break;
                                case cmLineStart :
                                    SetCurPtr(LineStart((int)CurPtr), selectMode);
                                    break;
                                case cmLineEnd :
                                    SetCurPtr(LineEnd((int)CurPtr), selectMode);
                                    break;
                                case cmLineUp :
                                    SetCurPtr(LineMove((int)CurPtr, -1), selectMode);
                                    break;
                                case cmLineDown :
                                    SetCurPtr(LineMove((int)CurPtr, 1), selectMode);
                                    break;
                                case cmPageUp:
                                    SetCurPtr(LineMove((int)CurPtr, -(Size.Y - 1)), selectMode);
                                    break;
                                case cmPageDown :
                                    SetCurPtr(LineMove((int)CurPtr, Size.Y - 1), selectMode);
                                    break;
                                case cmTextStart :
                                    SetCurPtr(0, selectMode);
                                    break;
                                case cmTextEnd :
                                    SetCurPtr((int)BufLen, selectMode);
                                    break;
                                case cmNewLine :
                                    NewLine();
                                    break;
                                case cmBackSpace :
                                    DeleteRange(PrevChar((int)CurPtr), (int)CurPtr, true);
                                    break;
                                case cmDelChar :
                                    DeleteRange((int)CurPtr, (int)NextChar((int)CurPtr), true);
                                    break;
                                case cmDelWord :
                                    DeleteRange((int)CurPtr, NextWord((int)CurPtr), false);
                                    break;
                                case cmDelStart :
                                    DeleteRange((int)LineStart((int)CurPtr), (int)CurPtr, false);
                                    break;
                                case cmDelEnd :
                                    DeleteRange((int)CurPtr, (int)LineEnd((int)CurPtr), false);
                                    break;
                                case cmDelLine :
                                    DeleteRange((int)LineStart((int)CurPtr), NextLine((int)CurPtr), false);
                                    break;
                                case cmInsMode :
                                    ToggleInsMode();
                                    break;
                                case cmStartSelect :
                                    StartSelect();
                                    break;
                                case cmHideSelect :
                                    HideSelect();
                                    break;
                                case cmIndentMode :
                                    AutoIndent = !AutoIndent;
                                    break;
                                default :
                                    UnLock();
                                    return;
                            }
                            TrackCursor(CenterCursor);
                            UnLock();
                            break;
                    }
                    break;
                case Event.Broadcast :
                    switch (Event.Command)
                    {
                        case cmScrollBarChanged :
                            if ((Event.InfoPtr == HScrollBar) || (Event.InfoPtr == VScrollBar))
                            {
                                CheckScrollBar(Event, HScrollBar, ref Delta.X);
                                CheckScrollBar(Event, VScrollBar, ref Delta.Y);
                            }
                            else
                                return;
                            break;
                        default :
                            return;
                    }
                    break;
                default :
                    return;
            }
            ClearEvent(ref Event);
        }

        private void CheckScrollBar(Event Event, ScrollBar B, ref int D)
        {
            ScrollBar P = B;
            if ((Event.InfoPtr == P) && (P.Value != D))
            {
                D = P.Value;
                Update(ufView);
            }
        }

        public void ScrollTo(int X, int Y)
        {
            X = Math.Max(0, Math.Min(X, Limit.X - Size.X));
            Y = Math.Max(0, Math.Min(Y, Limit.Y - Size.Y));
            if ((X != Delta.X) || (Y != Delta.Y))
            {
                Delta.X = X;
                Delta.Y = Y;
                Update(ufView);
            }
        }

        public void SetCurPtr(int P, byte SelectMode)
        {
            int Anchor;
            if ((SelectMode & smExtend) == 0)
                Anchor = P;
            else
                if (CurPtr == SelStart)
                Anchor = SelEnd;
            else
                Anchor = SelStart;
            if (P < Anchor)
            {
                if ((SelectMode & smDouble) != 0)
                {
                    P = PrevLine(NextLine(P));
                    Anchor = NextLine(PrevLine((int)Anchor));
                }
                SetSelect(P, Anchor, true);
            }
            else
            {
                if ((SelectMode & smDouble) != 0)
                {
                    P = NextLine(P);
                    Anchor = PrevLine(NextLine((int)Anchor));
                }
                SetSelect(Anchor, P, false);
            }
        }

        public void SetSelect(int NewStart, int NewEnd, bool CurStart)
        {
            byte Flags;
            int P, L;
            if (CurStart)
                P = NewStart;
            else
                P = NewEnd;
            Flags = ufUpdate;
            if ((NewStart != SelStart) || (NewEnd != SelEnd))
                if ((NewStart != NewEnd) || (SelStart != SelEnd))
                    Flags = ufView;
            if (P != CurPtr)
            {
                if (P > CurPtr)
                {
                    L = P - CurPtr;
                    Array.Copy(buffer, CurPtr + GapLen, buffer, CurPtr, L);
                    CurPos.Y += (int)CountLines(CurPtr, L);
                    CurPtr = P;
                }
                else
                {
                    L = CurPtr - P;
                    CurPtr = P;
                    CurPos.Y -= (int)CountLines(CurPtr, L);
                    Array.Copy(buffer, CurPtr, buffer, CurPtr + GapLen, L);
                }
                DrawLine = CurPos.Y;
                DrawPtr = LineStart((int)P);
                CurPos.X = (int)CharPos((int)DrawPtr, (int)P);
                DelCount = 0;
                InsCount = 0;
                SetBufSize(BufLen);
            }
            SelStart = NewStart;
            SelEnd = NewEnd;
            Update(Flags);
        }

        public void Lock()
        {
            LockCount++;
        }

        public void UnLock()
        {
            if (LockCount > 0)
            {
                LockCount--;
                if (LockCount == 0)
                    DoUpdate();
            }
        }

        public int GetMousePtr(Point Mouse)
        {
            Mouse = MakeLocal(Mouse);
            Mouse.X = Math.Max(0, Math.Min(Mouse.X, Size.X - 1));
            Mouse.Y = Math.Max(0, Math.Min(Mouse.Y, Size.Y - 1));
            return CharPtr(LineMove((int)DrawPtr, Mouse.Y + Delta.Y - DrawLine), Mouse.X + Delta.X);
        }

        public void TrackCursor( bool Center)
        {
            if (Center)
                ScrollTo(CurPos.X - Size.X + 1, CurPos.Y - Size.Y / 2);
            else
                ScrollTo(Math.Max(CurPos.X - Size.X + 1, Math.Min(Delta.X, CurPos.X)),
                    Math.Max(CurPos.Y - Size.Y + 1, Math.Min(Delta.Y, CurPos.Y)));

        }

        public override void SetState(View.StateFlags AState, bool Enable)
        {
            base.SetState( AState, Enable);
            switch (AState)
            {
                case StateFlags.Exposed:
                    if (Enable)
                        UnLock();
                    break;
                case StateFlags.Focused :
                    if ((State & StateFlags.Selected) == StateFlags.Selected)
                    {
                        if (HScrollBar != null)
                            HScrollBar.SetState(StateFlags.Visible, Enable);
                        if (VScrollBar != null)
                            VScrollBar.SetState(StateFlags.Visible, Enable);
                        if (Indicator != null)
                            Indicator.SetState(StateFlags.Visible, Enable);
                        UpdateCommands();
                    }
                    break;
                case StateFlags.Selected :
                    if ((State & StateFlags.Selected) == StateFlags.Selected)
                    {
                        if (HScrollBar != null)
                            HScrollBar.SetState(StateFlags.Visible, Enable);
                        if (VScrollBar != null)
                            VScrollBar.SetState(StateFlags.Visible, Enable);
                        if (Indicator != null)
                            Indicator.SetState(StateFlags.Visible, Enable);
                        UpdateCommands();
                    }
                    break;
            }
        }

        public override bool Valid(int Command)
        {
            return IsValid;
        }

        public void StartSelect()
        {
            HideSelect();
            Selecting = true;
        }

        private void HideSelect()
        {
            Selecting = false;
            SetSelect(CurPtr, CurPtr, false);
        }

        private int NextWord(int P)
        {
            while ((P < BufLen) && char.IsLetterOrDigit( BufChar(P)))
                P = NextChar(P);
            while ((P < BufLen) && !char.IsLetterOrDigit(BufChar(P)))
                P = NextChar(P);
            return P;
        }

        private int PrevWord(int P)
        {
            while ((P < BufLen) && char.IsLetterOrDigit(BufChar(P)))
                P = PrevChar(P);
            while ((P < BufLen) && !char.IsLetterOrDigit(BufChar(P)))
                P = PrevChar(P);
            return P;
        }

        private void NewLine()
        {
            int P;
            int I;
            P = LineStart((int)CurPtr);
            I = P;
            while ((I < CurPtr) && ((buffer[I] == ' ') || (buffer[I] == '\x0009')))
                I++;
            InsertText(new char[2] { '\x000D', '\x000A' }, 2, false);
            if (AutoIndent)
                InsertText(new char[1] { buffer[P] }, I - P, false);
        }

        private void DeleteRange(int StartPtr, int EndPtr, bool DelSelect)
        {
            if (HasSelection() && DelSelect)
                DeleteSelect();
            else
            {
                SetSelect(CurPtr, EndPtr, true);
                DeleteSelect();
                SetSelect(StartPtr, CurPtr, false);
                DeleteSelect();
            }
        }

        private void ToggleInsMode()
        {
            Overwrite = !Overwrite;
            SetState(StateFlags.CursorIns, !GetState(StateFlags.CursorIns));
        }

        private void Undo()
        {
            int Length;
            if ((DelCount != 0) || (InsCount != 0))
            {
                SelStart = CurPtr - InsCount;
                SelEnd = CurPtr;
                Length = DelCount;
                DelCount = 0;
                InsCount = 0;
                InsertBuffer( ref buffer, CurPtr + GapLen - Length, Length, true, true);
            }
        }

        private void ClipPaste()
        {
            if ((Clipboard != null) && (Clipboard != this))
                InsertFrom(Clipboard);
        }

        private bool InsertFrom( Editor Editor)
        {
            return
                InsertBuffer(
                ref Editor.buffer, Editor.BufPtr(Editor.SelStart), Editor.SelEnd - Editor.SelStart, CanUndo, IsClipBoard);
        }

        private bool ClipCopy()
        {
            bool ClipCopy = false;
            if ((Clipboard != null) && (Clipboard != this))
            {
                ClipCopy = Clipboard.InsertFrom(this);
                Selecting = false;
                Update(ufUpdate);
            }
            return ClipCopy;
        }

        private void ClipCut()
        {
            if (ClipCopy())
                DeleteSelect();
        }

        public char[] Buffer
        {
            get
            {
                return buffer;
            }
        }
    }
}
