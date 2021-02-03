using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Menus
{
    [Serializable]
	public class MenuBar : MenuView
	{

		public MenuBar( Rect Bounds, Menu AMenu):base( Bounds)
		{
			GrowMode = GrowModes.gfGrowHiX;
			Menu = AMenu;
			Options |= OptionFlags.ofPreProcess;
		}

		public override void Draw()
		{
			int X, L;
			uint CNormal, CSelect, CNormDisabled, CSelDisabled, Color;
			MenuItem P;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			CNormal = GetColor(0x0301);
			CSelect = GetColor(0x0604);
			CNormDisabled = GetColor(0x0202);
			CSelDisabled = GetColor(0x0505);
            B.FillChar(' ', CNormal, (byte)Size.X);
			if( Menu != null)
			{
				X = 1;
				P = Menu.Items;
				while( P != null)
				{
					if( P.Name != "")
					{
						L = P.CNameLen();
						if( (X + L) < Size.X)
						{
							if( P.Disabled)
								if( P == Current)
									Color = CSelect;
							else
									Color = CNormDisabled;
							else
								if( P == Current)
								Color = CSelect;
							else
								Color = CNormal;
                        B.FillChar(' ', Color, 1, X);
                        B.FillCStr(P.Name, Color, X + 1);
                        B.FillChar(' ', Color, 1, X + L + 1);
						}
						X += L + 2;
					}
					P = P.Next;
				}
			}
			WriteBuf(0, 0 , (byte)Size.X, 1, B);
		}

		public override Rect GetItemRect( MenuItem Item)
		{
			MenuItem P;
			Rect R = new Rect( 1, 0, 1, 1);
			P = Menu.Items;
			while( true)
			{
				R.A.X = R.B.X;
				if( P.Name != "")
					R.B.X += (P.CNameLen() + 2);
				if( P == Item)
					return R;
				P = P.Next;
			}
		}
	}
}
