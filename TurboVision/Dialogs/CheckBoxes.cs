using System;
using TurboVision.Objects;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for CheckBoxes.
	/// </summary>
	public class CheckBoxes : Cluster
	{

        private CheckBoxesToggle checkBoxesToggle = null;

        public CheckBoxesToggle CheckBoxesToggle
        {
            get { return checkBoxesToggle; }
            set { checkBoxesToggle = value; }
        }

		public CheckBoxes( Rect Bounds, SItem AStrings):base( Bounds, AStrings)
		{
		}

		public override bool Mark( int Item)
		{
			return (value & ( 1 << Item )) != 0;
		}

		public override void Draw()
		{
			const string Button = " [ ] ";
			DrawMultiBox( Button, " X");
		}


        public override void Press(int Item)
        {
            Value = Value ^ ( 1 << Item);
            if (checkBoxesToggle != null)
                checkBoxesToggle(this, Item);
        }
    }

    public delegate void CheckBoxesToggle(object sender, int item);
}
