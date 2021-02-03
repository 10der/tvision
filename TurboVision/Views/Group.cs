using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TurboVision.Objects;
using TurboVision.StdDlg;

namespace TurboVision.Views
{

    public enum Phases
    {
        phPhocused,
        phPreProcess,
        phPostProcess,
    }

    public enum SelectMode
    {
        NormalSelect, EnterSelect, LeaveSelect,
    }

    /// <summary>
    /// Summary description for Group.
    /// </summary>
    [Serializable]
    public class Group : View
    {
        public int EndState;
        public byte LockFlag;
        public Rect Clip;
        public View Current;
        public Phases Phase;
        public char[] Buffer = null;

        public View Last = null;

        public const int FocusedEvents = Event.Keyboard | Event.evCommand;
        public const int PositionalEvents = Event.Mouse;

        public Group(Rect Bounds)
            : base(Bounds)
        {
            Options |= (OptionFlags.ofSelectable | OptionFlags.ofBuffered);
            Clip = GetExtent();
            EventMask = EventMasks.evAll;
        }

        public View First
        {
            get
            {
                if (Last == null)
                    return null;
                else
                    return Last.Next;
            }
        }

        public void Lock()
        {
            if ((Buffer != null) || (LockFlag != 0))
                LockFlag++;
        }

        public int IndexOf(View P)
        {
            View temp;
            int index = 0;
            if (Last != null)
            {
                temp = Last;
                do
                {
                    index++;
                    temp = temp.Next;
                } while ((temp != P) && (temp != Last));
                if (temp != P)
                    index = 0;
            }
            return index;
        }

        public void InsertView(View P, View Target)
        {
            P.Owner = this;
            if (Target != null)
            {
                Target = Target.Prev;
                P.Next = Target.Next;
                Target.Next = P;
            }
            else
            {
                if (Last == null)
                    P.Next = P;
                else
                {
                    P.Next = Last.Next;
                    Last.Next = P;
                }
                Last = P;
            }
        }

        public void RemoveView(View P)
        {
            View cur;

            if (Last != null)
            {
                cur = Last;
                while (true)
                {
                    if (P == cur.Next)
                    {
                        cur.Next = P.Next;
                        if (Last == P)
                        {
                            if (cur.Next == P)
                                Last = null;
                            else
                                Last = cur;
                        }
                        break;
                    }
                    if (cur.Next == Last)
                        break;
                    cur = cur.Next;
                }
            }
        }

        public View FindNext(bool Forwards)
        {
            View Result = null;
            View P;
            if (Current != null)
            {
                P = Current;
                do
                {
                    if (Forwards)
                        P = P.Next;
                    else
                        P = P.Prev;
                } while (!((((P.State & (StateFlags.Visible | StateFlags.Disabled)) == StateFlags.Visible) &&
                    ((P.Options & OptionFlags.ofSelectable) != 0)) | (P == Current)));
                if (P != Current)
                    Result = P;
            }
            return Result;
        }

        public override void ResetCursor()
        {
            if (Current != null)
                Current.ResetCursor();
        }

        public void Unlock()
        {
            if (LockFlag != 0)
            {
                LockFlag--;
                if (LockFlag == 0)
                    DrawView();
            }
        }

        public void DrawSubViews(View P, View Bottom)
        {
            if (P != null)
                while (P != Bottom)
                {
                    P.DrawView();
                    P = P.NextView();
                }
        }

        public void Redraw()
        {
            DrawSubViews(First, null);
        }

        public View At(int Index)
        {
            View Temp = Last;
            while (Index > 0)
            {
                Index--;
                Temp = Temp.Next;
            }
            return Temp;
        }

        public delegate void ForEachProc(View V, params object[] o);

        public void ForEach(ForEachProc P, params object[] o)
        {

            View Temp = null;

            if (Last != null)
            {
                Temp = Last;
                do
                {
                    Temp = Temp.Next;
                    P(Temp, o);
                } while (Temp != Last);
            }
        }

        public void HandleEach(HandleEachProc P, ref Event E)
        {

            View Temp = null;

            if (Last != null)
            {
                Temp = Last;
                do
                {
                    Temp = Temp.Next;
                    P(Temp, ref E);
                } while (Temp != Last);
            }
        }

        internal void DoAwaken(View P, params object[] o)
        {
            P.Awaken();
        }

        public override void Awaken()
        {
            ForEach(DoAwaken);
        }

        public override uint GetHelpCtx()
        {
            uint H = hcNoContext;
            if (Current != null)
                return Current.GetHelpCtx();
            if (H == hcNoContext)
                return base.GetHelpCtx();
            return hcNoContext;
        }

        public delegate bool FirstThatProc(View P);

        public View FirstThat(FirstThatProc P)
        {
            View Temp;
            bool Found;

            if (Last == null)
                return null;
            else
            {
                Temp = Last;
                do
                {
                    Temp = Temp.Next;
                    Found = P(Temp);
                } while (!Found && (Temp != Last));
                if (Found)
                    return Temp;
                else
                    return null;
            }
        }

        internal StateFlags _AState;
        internal OptionFlags _AOptions;

        internal bool Matches(View P)
        {
            return (((P.State & _AState) == _AState) &&
                ((P.Options & _AOptions) == _AOptions));
        }

        public View FirstMatch(StateFlags AState, OptionFlags AOptions)
        {
            _AState = AState;
            _AOptions = AOptions;
            return FirstThat(new FirstThatProc(Matches));
        }

        public static bool ContainsMouse(View P, Point Where)
        {
            return (((P.State & StateFlags.Visible) != 0) &&
                P.MouseInView(Where));
        }

        public int _Command = 0;

        public bool IsInvalid(View P)
        {
            return !P.Valid(_Command);
        }

        public bool InvalidExists()
        {
            return FirstThat(new FirstThatProc(IsInvalid)) != null;
        }

        public override bool Valid(int Command)
        {
            bool Result = true;
            _Command = Command;
            if (Command == cmReleasedFocus)
            {
                if ((Current != null) && ((Current.Options & OptionFlags.ofSelectable) != 0))
                    return Current.Valid(Command);
            }
            else
            {
                _Command = Command;
                Result = !InvalidExists();
            }
            return Result;
        }

        internal Point _D;

        internal void DoCalcChange(View P, params object[] o)
        {
            Rect R = P.CalcBounds(ref _D);
            P.ChangeBounds(R);
        }

        public override void ChangeBounds(Rect Bounds)
        {
            _D.X = Bounds.B.X - Bounds.A.X - Size.X;
            _D.Y = Bounds.B.Y - Bounds.A.Y - Size.Y;
            if ((_D.X == 0) && (_D.Y == 0))
            {
                SetBounds(Bounds);
                DrawView();
            }
            else
            {
                SetBounds(Bounds);
                Clip = GetExtent();
                Lock();
                ForEach(new ForEachProc(DoCalcChange));
                Unlock();
            }
        }

        internal void SelectView(View P, bool Enable)
        {
            if (P != null)
                P.SetState(StateFlags.Selected, Enable);
        }

        internal void FocusView(View P, bool Enable)
        {
            if (((State & StateFlags.Focused) != 0) && (P != null))
                P.SetState(StateFlags.Focused, Enable);
        }

        public void SetCurrent(View P, SelectMode Mode)
        {
            if ((Current != P))
            {
                Lock();
                FocusView(Current, false);
                if (Mode != SelectMode.EnterSelect)
                    SelectView(Current, false);
                if (Mode != SelectMode.LeaveSelect)
                    SelectView(P, true);
                FocusView(P, true);
                Current = P;
                Unlock();
            }
        }

        public void ResetCurrent()
        {
            SetCurrent(FirstMatch(StateFlags.Visible, OptionFlags.ofSelectable), SelectMode.NormalSelect);
        }

        public override void EndModal(int Command)
        {
            if ((State & StateFlags.Modal) != 0)
                EndState = Command;
            else
                base.EndModal(Command);
        }

        public void DoHandleEvent(View P, ref Event E)
        {
            if ((P == null) || (((P.State & StateFlags.Disabled) != 0) && ((E.What & (PositionalEvents | FocusedEvents))) != 0))
                return;
            switch (Phase)
            {
                case Phases.phPreProcess:
                    if ((P.Options & OptionFlags.ofPreProcess) == 0)
                        return;
                    break;
                case Phases.phPostProcess:
                    if ((P.Options & OptionFlags.ofPostProcess) == 0)
                        return;
                    break;
            }
            if ((E.What & (int)P.EventMask) != 0)
                P.HandleEvent(ref E);
        }

        public delegate void HandleEachProc(View P, ref Event E);
        public delegate bool FirstThatContainsMouseProc(View P, Point Where);

        internal View FirstThatContainsMouse(FirstThatContainsMouseProc P, Point Where)
        {
            View Temp;
            bool Found;

            if (Last == null)
                return null;
            else
            {
                Temp = Last;
                do
                {
                    Temp = Temp.Next;
                    Found = P(Temp, Where);
                } while (!Found && (Temp != Last));
                if (Found)
                    return Temp;
                else
                    return null;
            }
        }

        public override void HandleEvent(ref Event Event)
        {
            try
            {
                base.HandleEvent(ref Event);
                if ((Event.What & FocusedEvents) != 0)
                {
                    Phase = Phases.phPreProcess;
                    HandleEach(new HandleEachProc(DoHandleEvent), ref Event);
                    Phase = Phases.phPhocused;
                    DoHandleEvent(Current, ref Event);
                    Phase = Phases.phPostProcess;
                    HandleEach(new HandleEachProc(DoHandleEvent), ref Event);
                }
                else
                {
                    Phase = Phases.phPhocused;
                    if ((Event.What & PositionalEvents) != 0)
                        DoHandleEvent(FirstThatContainsMouse(
                            new FirstThatContainsMouseProc(ContainsMouse), Event.Where), ref Event);
                    else
                        HandleEach(new HandleEachProc(DoHandleEvent), ref Event);
                }
            }
            catch (Exception Ex)
            {
                ErrorBox.Show("Exception : " + Ex.GetType().ToString(), Ex.Message, Ex.StackTrace);
                throw;
            }
        }

        public void Delete(View P)
        {
            StateFlags SaveState = State;
            P.Hide();
            RemoveView(P);
            P.Owner = null;
            P.Next = null;
            if ((SaveState & StateFlags.Visible) != 0)
                P.Show();
        }

        public override void Done()
        {
            View P;
            View T;
            Hide();
            P = Last;
            if (P != null)
            {
                do
                {
                    P.Hide();
                    P = P.Prev;
                } while (P != Last);
                do
                {
                    T = P.Prev;
                    P.Done();
                    P = T;
                } while (Last != null);
            }
            base.Done();
        }

        public override void SetData(object Rec)
        {
            List<object> R = new List<object>();
            if (Rec.GetType().IsPrimitive || Rec.GetType().Equals(typeof(string)))
            {
            }
            else
            {
                var properties = Rec.GetType().GetProperties();
                foreach (var property in properties)
                {
                    try
                    {
                        object value = property.GetValue(Rec, null);
                        R.Add(value);
                    }
                    catch
                    {
                    }
                }
            }

            int I = 0;
            View V;
            if (Last != null)
            {
                V = Last;
                do
                {
                    if (V.DataSize() != 0)
                    {
                        if (R.Count != 0)
                        {
                            if (I <= R.Count)
                                V.SetData(R[I]);
                        }
                        else
                        {
                            if (Rec.GetType().IsPrimitive || Rec.GetType().Equals(typeof(string)))
                            {
                                V.SetData(Rec);
                            }
                        }
                        I++;
                    }
                    V = V.Prev;
                } while (V != Last);
            }
        }

        public T GetData<T>()
        {
            var Instance = Activator.CreateInstance<T>();
            var properties = Instance.GetType().GetProperties();

            List<PropertyInfo> R = new List<PropertyInfo>();
            foreach (PropertyInfo property in properties)
            {
                R.Add(property);
            }

            int I = 0;
            View V;
            if (Last != null)
            {
                V = Last;
                do
                {
                    if (V.DataSize() != 0)
                    {
                        object obj = V.GetData();
                        if (obj != null)
                        {
                            R[I].SetValue(Instance, obj, null);
                        }
                        I++;
                    }
                    V = V.Prev;
                } while (V != Last);
            }

            return Instance;
        }

        public override object GetData()
        {
            ArrayList R = new ArrayList();
            int I = 0;
            View V;
            if (Last != null)
            {
                V = Last;
                do
                {
                    if (V.DataSize() != 0)
                    {
                        object obj = V.GetData();
                        if (obj != null)
                        {
                            //if (obj.GetType().IsPrimitive || obj.GetType().Equals(typeof(string)))
                            //{
                            //    return obj;
                            //}
                            // R[I].SetValue(Instance, obj, null);
                        }
                        R.Add(obj);
                        I++;
                    }
                    V = V.Prev;
                } while (V != Last);
            }

            return R.ToArray();
        }

        uint _T = 0;

        internal void AddSubViewDataSize(View V, params object[] o)
        {
            _T += V.DataSize();
        }

        public override uint DataSize()
        {
            ForEach(new ForEachProc(AddSubViewDataSize), null);
            return _T;
        }

        public void DoSetState(View P, params object[] o)
        {
            P.SetState((StateFlags)o[0], (bool)o[1]);
        }

        public void DoExpose(View P, params object[] o)
        {
            if ((State & StateFlags.Visible) != 0)
                P.SetState(StateFlags.Exposed, (bool)o[0]);
        }

        public override void SetState(StateFlags AState, bool Enable)
        {
            base.SetState(AState, Enable);
            switch (AState)
            {
                case StateFlags.Active:
                case StateFlags.Dragging:
                    Lock();
                    ForEach(new ForEachProc(DoSetState), AState, Enable);
                    Unlock();
                    break;
                case StateFlags.Focused:
                    if (Current != null)
                        Current.SetState(StateFlags.Focused, Enable);
                    break;
                case StateFlags.Exposed:
                    ForEach(new ForEachProc(DoExpose), Enable);
                    break;
            }
        }

        public void InsertBefore(View P, View Target)
        {
            StateFlags SaveState;

            if ((P != null) && (P.Owner == null) && ((Target == null) || (Target.Owner == this)))
            {
                if ((P.Options & OptionFlags.ofCenterX) != 0)
                    P.Origin.X = (Size.X - P.Size.X) / 2;
                if ((P.Options & OptionFlags.ofCenterY) != 0)
                    P.Origin.Y = (Size.Y - P.Size.Y) / 2;
                SaveState = P.State;
                P.Hide();
                InsertView(P, Target);
                if ((SaveState & StateFlags.Visible) != 0)
                    P.Show();
                if ((State & StateFlags.Active) != 0)
                    P.SetState(StateFlags.Active, true);
            }
        }

        public virtual void Insert(View P)
        {
            InsertBefore(P, First);
        }

        public virtual void EventError(Event Event)
        {
            if (Owner != null)
                Owner.EventError(Event);
        }

        public override int Execute()
        {
            do
            {
                EndState = 0;
                do
                {
                    try
                    {
                        Event E = GetEvent();
                        HandleEvent(ref E);
                        if (E.What != Event.Nothing)
                            EventError(E);
                    }
                    catch (Exception Ex)
                    {
                        ErrorBox.Show("Exception : " + Ex.GetType().ToString(), Ex.Message, Ex.StackTrace);
                    }
                } while (EndState == 0);
            } while (!Valid(EndState));
            return EndState;
        }

        public int ExecView(View P)
        {
            OptionFlags SaveOptions;
            Group SaveOwner;
            View SaveTopView;
            View SaveCurrent;
            int Result;
            int[] Commands;

            if (P != null)
            {
                SaveOptions = P.Options;
                SaveOwner = P.Owner;
                SaveTopView = TheTopView;
                SaveCurrent = Current;
                Commands = GetCommands();
                TheTopView = P;
                P.Options &= ~OptionFlags.ofSelectable;
                P.SetState(StateFlags.Modal, true);
                SetCurrent(P, SelectMode.EnterSelect);
                if (SaveOwner == null)
                    Insert(P);
                Result = P.Execute();
                if (SaveOwner == null)
                    Delete(P);
                SetCurrent(SaveCurrent, SelectMode.LeaveSelect);
                P.SetState(StateFlags.Modal, false);
                P.Options = SaveOptions;
                TheTopView = SaveTopView;
                SetCommands(Commands);
                return Result;
            }
            else
                return cmCancel;
        }

        public bool FocusNext(bool Forwards)
        {
            bool Result;
            View P = FindNext(Forwards);
            Result = true;
            if (P != null)
                return P.Focus();
            return Result;
        }

        public override void Draw()
        {
            if (Buffer == null)
            {
                Buffer = new char[
                    ScreenManager.ScreenWidth * ScreenManager.ScreenHeight * 2];
                if (Buffer != null)
                {
                    LockFlag++;
                    Redraw();
                    LockFlag--;
                }
            }

            if (Buffer != null)
                WriteBuf(0, 0, Size.X, Size.Y, Buffer);
            else
            {
                Clip = GetClipRect();
                Redraw();
                Clip = GetExtent();
            }
        }

        public void SelectNext(bool Forwards)
        {
            View P = FindNext(Forwards);
            if (P != null)
                P.Select();
        }

        /// <summary>
        /// Envelope for SelectNext( true) - forwards
        /// </summary>
        public void SelectNext()
        {
            SelectNext(true);
        }
    }
}
