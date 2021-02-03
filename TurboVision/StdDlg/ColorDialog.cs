using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{
    public class ColorDialog : Dialog
    {
        public string Pal;
        public ColorGroupList Groups;
        public ColorSelector ForSel;
        public ColorSelector BakSel;
        public MonoSelector MonoSel;
        public Label ForLabel;
        public Label BakLabel;
        public Label MonoLabel;

        public ColorDialog(string APalette, ColorGroup AGroups)
            : base(new Rect(0, 0, 61, 18), "Colors")
        {
            Options |= OptionFlags.ofCentered;
            Pal = APalette;

            Rect R = new Rect(18, 3, 19, 14);
            View P = new ScrollBar(R);
            Insert(P);
            R = new Rect(3, 3, 18, 14);
            Groups = new ColorGroupList(R, (ScrollBar)P, AGroups);
            Insert(Groups);
            R = new Rect(2, 2, 8, 3);
            View V = new Label(R, "~G~roup", Groups);
            Insert(V);

            R = new Rect(41, 3, 42, 14);
            P = new ScrollBar(R);
            Insert(P);
            R = new Rect(21, 3, 41, 14);
            P = new ColorItemList(R, (ScrollBar)P, AGroups.Items);
            Insert(P);
            R = new Rect(20, 2, 25, 3);
            V = new Label(R, "~I~tem", P);
            Insert(V);

            R = new Rect(45, 3, 57, 7);
            ForSel = new ColorSelector(R, ColorSel.csForeground);
            Insert(ForSel);
            R.A.Y--; R.B.Y = R.A.Y + 1;
            ForLabel = new Label(R, "~F~oreground", ForSel);
            Insert(ForLabel);

            R.A.Y += 7; R.B.Y += 10;
            if (ColorSelector.cBackgroundBlink)
                R.B.Y -= 2;
            BakSel = new ColorSelector(R, ColorSel.csBackground);
            Insert(BakSel);
            R.A.Y--; R.B.Y = R.A.Y + 1;
            BakLabel = new Label(R, "~B~ackground", BakSel);
            Insert(BakLabel);

            R.A += new Point(-1, 6);
            R.B += new Point(1, 7);
            if (ColorSelector.cBackgroundBlink)
            {
                R.A.Y -= 2;
                R.B.Y -= 2;
            }
            ColorDisplay Display = new ColorDisplay(R, "Text");
            Insert(Display);

            R = new Rect(44, 3, 59, 8);
            MonoSel = new MonoSelector(R);
            MonoSel.Hide();
            Insert(MonoSel);
            R = new Rect(43, 2, 49, 3);
            (MonoLabel = new Label(R, "~C~olor", MonoSel)).Hide();
            Insert(MonoLabel);

            if ((AGroups != null) && (AGroups.Items != null))
                if (AGroups.Items.Index < Pal.Length)
                    Display.SetColor((byte)Pal[AGroups.Items.Index - 1]);
                else
                    Display.SetColor(0);

            R = new Rect(16, 15, 26, 17);
            ((P = new OkButton(R)) as Button).MakeDefault(true);
            Insert(P);
            R = new Rect(28, 15, 38, 17);
            ((P = new CancelButton(R)) as Button).MakeDefault(false);
            Insert(P);
            SelectNext(false);
        }

        public override void SetData(object Rec)
        {
            /*
            Pal = "";
            UInt32[] obj = (UInt32[])Rec[0];
            for (int i = 0; i < obj.Length; i++)
            {
                //Pal += System.Text.Encoding.Default.GetString(new byte[] { Convert.ToByte(obj[i]) });
                Pal += ' ';
            }
            if (string.IsNullOrEmpty(Pal)) { }
            */
            //Pal = new string((char[])Rec[0]);
        }
    }
}
