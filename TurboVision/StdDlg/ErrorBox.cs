#region Using directives

using System;
#if FRAMEWORK20
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Text;
using TurboVision.Dialogs;
using TurboVision.App;
using TurboVision.Objects;
using TurboVision.Views;

#endregion

namespace TurboVision.StdDlg
{
    public class ErrorBox : Dialog
    {

        private string Details = "";

        public ErrorBox(string title, string errorString, string details)
            : base(new Rect(0, 0, 60, 21), "Error")
        {
            Title = title;
            Details = details;
            Palette = WindowPalettes.wpBlueWindow;
            Options |= OptionFlags.ofCentered;
            Flags |= WindowFlags.wfGrow | WindowFlags.wfZoom;
            StaticText st = new StaticText(
                new Rect(1, 1, 59, 4), errorString);
            st.GrowMode |= GrowModes.gfGrowHiX;
            Insert(st);
            Rect R = GetExtent();
            R.A.X = R.B.X - 2;
            R.B.X--;
            R.A.Y = 5;
            R.B.Y = 16;
            ScrollBar VScrollBar = new ScrollBar(R);
            Insert(VScrollBar);
            R.A.X = 1;
            R.B.X = 58;
            R.A.Y = 16;
            R.B.Y = 17;
            ScrollBar HScrollBar = new ScrollBar(R);
            Insert(HScrollBar);
            TextScroller ts = new TextScroller(
                new Rect(1, 5, 58, 16), HScrollBar, VScrollBar, details);
            ts.Options |= OptionFlags.ofFramed;
            Insert(ts);
            CancelButton cb = new CancelButton(
                new Rect(0, 18, 12, 20));
            cb.MakeDefault(true);
            cb.GrowMode |= GrowModes.gfGrowLoY;
            cb.GrowMode |= GrowModes.gfGrowHiY;
            cb.GrowMode |= GrowModes.gfGrowHiX;
            cb.Options = OptionFlags.ofCenterX;
            Insert(cb);
        }

        public static void Show(string title, string errorString, string details)
        {
            Application.Desktop.ExecView(
                new ErrorBox(title, '\x0003' + errorString, details));
        }


        public override void SizeLimits(out Point Min, out Point Max)
        {
            Min = new Point(60, 21);
            Max = Application.Desktop.GetExtent().B;
        }

        public override void HandleEvent(ref Event Event)
        {
            // tender !!! clipboard
            base.HandleEvent(ref Event);
            if (Event.What == Event.KeyDown)
                if (Event.KeyCode == (KeyboardKeys)0x9200)
                {
//#if FRAMEWORK20
//                    System.Windows.Forms.Clipboard.SetText(Details);
//#else
//                    System.Windows.Forms.Clipboard.SetDataObject(Details, true);
//#endif
                }
        }
    }
}
