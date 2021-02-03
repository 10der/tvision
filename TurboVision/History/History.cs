using System;
using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.Views;

namespace TurboVision.History
{
	/// <summary>
	/// Summary description for History.
	/// </summary>
    public class History : View
    {

        private static uint[] CHistory = { 0x16, 0x17 };

        public InputLine Link;
        public static HistoryBlock HistMemory = null;
        public int HistoryId;

        public class HistoryBlock
        {
            public int Id;
            public string String;
            public HistoryBlock Next;
        }

        public History(Rect Bounds, InputLine ALink, int AHistoryId):base( Bounds)
        {
            Options |= OptionFlags.ofPostProcess;
            EventMask |= EventMasks.evBroadcast;
            Link = ALink;
            HistoryId = AHistoryId;
        }

        public override uint[] GetPalette()
        {
            return CHistory;
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent(ref Event);
/*            if ((Event.What == Event.evMouseDown) ||
            ((Event.What == Event.evKeyDown) && (Drivers.CtrlToArrow(Event.KeyCode) == Drivers.KbKeys.kbDown) &&
            ((Link.State & StateFlags.sfFocused) != 0)))
            {
                if (!Link.Focus())
                {
                    ClearEvent(ref Event);
                    return;
                }
                RecordHistory(Link.Data);
                Rect R = Link.GetBounds();
                R.A.X--;
                R.B.X++;
                R.B.Y += 7;
                R.A.Y--;
                Rect P = Owner.GetExtent();
                R.Intersect(P);
                R.B.Y--;
                HistoryWindow = InitHistoryWindow(R);
                if (HistoryWindow != null)
                {
                    int C = Owner.ExecView(HistoryWindow);
                    if (C == cmOk)
                    {
                        string Rslt = HistoryWindow.GetSelection();
                        if (Rslt.Length > Link.MaxLen)
                            Rslt.Substring(0, Link.MaxLen);
                        Link.Data = Rslt;
                        Link.SelectAll(true);
                        Link.DrawView();
                    }
                    HistoryWindow.Done();
                }
                ClearEvent(ref Event);
            }
            else
                    if (Event.What == Event.evBroadcast)
                if (((Event.Command == cmReleasedFocus) && (Event.InfoPtr == Link)) ||
                    (Event.Command == cmRecordHistory))
                    RecordHistory(Link.Data);
*/
        }

        public override void Draw()
        {
            DrawBuffer B = new DrawBuffer(Size.X * Size.Y);
            B.FillCStr(ldHistoryDropDown, GetColor(0x0102), 0);
            WriteLine(0, 0, (int)Size.X, (int)Size.Y, B);
        }

        public HistoryWindow InitHistoryWindow(Rect Bounds)
        {
            HistoryWindow P = new HistoryWindow(Bounds, HistoryId);
            P.HelpCtx = Link.HelpCtx;
            return P;
        }

        public void RecordHistory(string S)
        {
            HistoryAdd(HistoryId, S);
        }

        public static void HistoryAdd(int Id, string S)
        {
            HistoryBlock HP = HistMemory;
            if (HistMemory == null)
            {
                HistMemory = new HistoryBlock();
                HistMemory.Id = Id;
                HistMemory.String = S;
            }
            else
            {
                while (HP.Next != null)
                {
                    if ((HP.Id == Id) && (HP.String == S))
                        if (HP.Next != null)
                            HP.Next = HP.Next.Next;
                    if( HP.Next != null)
                        HP = HP.Next;
                }
                if ((HP.Id != Id) || (HP.String != S))
                {
                    HP.Next = new HistoryBlock();
                    HP.Next.Id = Id;
                    HP.Next.String = S;
                }
            }
        }

        public static int HistoryCount(int Id)
        {
            int C = 0;
            HistoryBlock HP = HistMemory;
            while (HP != null)
            {
                if (HP.Id == Id)
                    C++;
                HP = HP.Next;
            }
            return C;
        }

        public static string HistoryStr(int Id, int Count)
        {
            HistoryBlock HP = HistMemory;
            while (Count > 0)
            {
                if (HP.Id == Id)
                    Count--;
                HP = HP.Next;
            }
            if (HP.Id == Id)
                return HP.String;
            else
                return "";
        }
    }
}
