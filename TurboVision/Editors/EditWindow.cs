#region Using directives

using System;
#if FRAMEWORK20
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Text;
using TurboVision;
using TurboVision.Objects;
using TurboVision.Views;

#endregion

namespace TurboVision.Editors
{
    public class EditWindow : Window
    {

        public const int cmUpdateTitle = 523;

        public FileEditor Editor;

        public EditWindow( Rect Bounds, string FileName, int ANumber)
            :base( Bounds, "", ANumber)
        {
            Options |= OptionFlags.ofTileable;
            Rect R = new Rect(18, Size.Y - 1, Size.X - 2, Size.Y);
            ScrollBar HScrollBar = new ScrollBar(R);
            HScrollBar.Hide();
            Insert(HScrollBar);
            R = new Rect(Size.X - 1, 1, Size.X, Size.Y - 1);
            ScrollBar VScrollBar = new ScrollBar(R);
            VScrollBar.Hide();
            Insert(VScrollBar);
            R = new Rect(2, Size.Y - 1, 16, Size.Y);
            Indicator Indicator = new Indicator(R);
            Indicator.Hide();
            Insert(Indicator);
            R = GetExtent();
            R.Grow(-1, -1);
            Editor = new FileEditor(
                R, HScrollBar, VScrollBar, Indicator, FileName);
            Insert(Editor);
        }

        public override string GetTitle(int MaxSize)
        {
            if (Editor.IsClipBoard)
                return "ClipBoard";
            else
                if (Editor.FileName == "")
                return "Untitled";
            else
                return Editor.FileName;
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent(ref Event);
            if ((Event.What == Event.Broadcast) && (Event.Command == cmUpdateTitle))
            {
                Frame.DrawView();
                ClearEvent(ref Event);
            }
        }

        public override void SizeLimits(out Point Min, out Point Max)
        {
            base.SizeLimits( out Min, out Max);
            Min.X = 23;
        }
    }
}
