using System;
using TurboVision.Objects;

namespace TurboVision.Dialogs
{
	public class MultiCheckBoxes : Cluster
	{

		private byte selRange;
		private uint flags;

		public string States;

		public MultiCheckBoxes( Rect Bounds, SItem AStrings):base( Bounds, AStrings)
		{
		}

		public override byte MultiMark( int Item)
		{
			return 0;
		}

		public byte SelRange
		{
			get
			{
				return selRange;
			}
			set
			{
				selRange = value;
			}
		}
		public uint Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}

		public override void Draw()
		{
			const string Button = " [ ] ";
			DrawMultiBox( Button, States);
		}

        public override object GetData()
		{
			return base.GetData();
		}
	}
}
