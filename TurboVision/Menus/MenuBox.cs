using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Menus
{
	public class MenuBox : MenuView
	{

		internal static Rect CalcBoxRect( Rect Bounds, Menu AMenu)
		{
			int L, H, W;
			MenuItem P;
			Rect R = new Rect();

			W = 10;
			H = 2;

			if( AMenu != null)
			{
				P = AMenu.Items;
				while ( P != null)
				{
					if( P.Name != "")
					{
						L = P.CNameLen() + 6;
						if( P.Command == 0)
							L += 3;
						else
							if( P.Param != "")
							L += (P.CParamLen() + 2);
						if( L > W)
							W = L;
					}
					H++;
					P = P.Next;
				}
			}
			R.Copy( Bounds);
			if( (R.A.X + W) < R.B.X)
				R.B.X = R.A.X + W;
			else
				R.A.X = R.A.X - W;
			if( (R.A.Y + H) < R.B.Y)
				R.B.Y = R.A.Y + H;
			else
				R.A.Y = R.B.Y - H;
			return R;
		}

		public MenuBox( Rect Bounds, Menu AMenu, MenuView AParentMenu):base( CalcBoxRect( Bounds, AMenu))
		{
			State |= StateFlags.Shadow;
			Options |=OptionFlags.ofPreProcess;
			Menu = AMenu;
			ParentMenu = AParentMenu;
		}

		public override Rect GetItemRect(MenuItem Item)
		{
			int Y = 1;
			MenuItem P = Menu.Items;
			while( P != Item)
			{
				Y++;
				P = P.Next;
			}
			return new Rect(2, Y, Size.X - 2, Y + 1);
		}

		internal void FrameLine(int N, DrawBuffer B, uint CNormal, uint Color)
		{
			B.FillBuf( ldMenuFrameChars.Substring( N, 2), CNormal, 2);
			B.FillChar( (char)ldMenuFrameChars[N + 2], Color, (int)(Size.X - 4), 2);
			B.FillBuf( ldMenuFrameChars.Substring( N + 3, 2), CNormal, 2, (int)(Size.X - 2));
		}

		internal void DrawLine( ref int Y, DrawBuffer B)
		{
			WriteBuf( 0, Y, (int)Size.X, 1, B);
			Y++;
		}

		public override void Draw()
		{
			uint CNormal, CSelect, CNormDisabled, CSelDisabled, Color;
			int Y;
			MenuItem P;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			CNormal = GetColor(0x0301);
			CSelect = GetColor(0x0604);
			CNormDisabled = GetColor(0x0202);
			CSelDisabled = GetColor(0x0505);

			Y = 0;
			Color = CNormal;
			FrameLine(0, B, CNormal, Color);
			DrawLine( ref Y, B);
			if( Menu != null)
			{
				P = Menu.Items;
				while( P != null)
				{
					Color = CNormal;
					if( P.Name == "")
						FrameLine(15, B, (byte)CNormal, (byte)Color);
					else
					{
						if( P.Disabled)
							if( P == Current)
								Color = CSelDisabled;
						else
								Color = CNormDisabled;
						else
							if( P == Current)
							Color = CSelect;
						FrameLine(10, B, CNormal, Color);
						B.FillCStr( P.Name, Color, 3);
						if( P.Command == 0)
                            B.FillChar(ldSubMenuArrow, Color, 1, (int)Size.X - 4);
						else
							if( P.Param != "")
                                B.FillStr(P.Param, Color, (int)Size.X - 3 - P.Param.Length);
					}
					DrawLine( ref Y, B);
					P = P.Next;
				}
			}
			Color = CNormal;
			FrameLine(5, B, CNormal, Color);
			DrawLine( ref Y, B);
		}
	}
}
