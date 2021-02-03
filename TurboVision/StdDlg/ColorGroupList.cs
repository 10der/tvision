using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.StdDlg
{

	public class ColorItem
	{
		public string Name;
		public byte Index;
		public ColorItem Next; 

		public ColorItem( string AName, byte AIndex, ColorItem ANext)
		{
			Name = AName;
			Index = AIndex;
			Next = ANext;
		}

		public static ColorItem DesktopColorItems( ColorItem Next)
		{
			return new ColorItem( "Color", 1, Next);
		}

		public static ColorItem MenuColorItems( ColorItem Next)
		{
			return 
			    new ColorItem("Normal",            2,
				new ColorItem("Disabled",          3,
				new ColorItem("Shortcut",          4,
				new ColorItem("Selected",          5,
				new ColorItem("Selected disabled", 6,
				new ColorItem("Shortcut selected", 7,
				Next))))));
		}

		public static ColorItem DialogColorItems( DialogPalettes Palette, ColorItem Next)
		{
			byte[] COffset = new byte[3]{64, 96, 32};
			byte Offset;
			Offset = COffset[(int)Palette];
			return
				new ColorItem("Frame/background",  (byte)(Offset + 1),
				new ColorItem("Frame icons",       (byte)(Offset + 2),
				new ColorItem("Scroll bar page",   (byte)(Offset + 3),
				new ColorItem("Scroll bar icons",  (byte)(Offset + 4),
				new ColorItem("Static text",       (byte)(Offset + 5),

				new ColorItem("Label normal",      (byte)(Offset + 6),
				new ColorItem("Label selected",    (byte)(Offset + 7),
				new ColorItem("Label shortcut",    (byte)(Offset + 8),

				new ColorItem("Button normal",     (byte)(Offset + 9),
				new ColorItem("Button default",    (byte)(Offset + 10),
				new ColorItem("Button selected",   (byte)(Offset + 11),
				new ColorItem("Button disabled",   (byte)(Offset + 12),
				new ColorItem("Button shortcut",   (byte)(Offset + 13),
				new ColorItem("Button shadow",     (byte)(Offset + 14),

				new ColorItem("Cluster normal",    (byte)(Offset + 15),
				new ColorItem("Cluster selected",  (byte)(Offset + 16),
				new ColorItem("Cluster shortcut",  (byte)(Offset + 17),

				new ColorItem("Input normal",      (byte)(Offset + 18),
				new ColorItem("Input selected",    (byte)(Offset + 19),
				new ColorItem("Input arrow",       (byte)(Offset + 20),

				new ColorItem("History button",    (byte)(Offset + 21),
				new ColorItem("History sides",     (byte)(Offset + 22),
				new ColorItem("History bar page",  (byte)(Offset + 23),
				new ColorItem("History bar icons", (byte)(Offset + 24),

				new ColorItem("List normal",       (byte)(Offset + 25),
				new ColorItem("List focused",      (byte)(Offset + 26),
				new ColorItem("List selected",     (byte)(Offset + 27),
				new ColorItem("List divider",      (byte)(Offset + 28),

				new ColorItem("Information pane",  (byte)(Offset + 29),
				Next)))))))))))))))))))))))))))));
		}

		public static ColorItem WindowColorItems( WindowPalettes Palette,
			ColorItem Next)
		{
			byte[] COffset = new byte[3]{8, 16, 24};
			byte Offset;
			Offset = COffset[ (int)Palette];
			return
			    new ColorItem("Frame passive",     (byte)(Offset + 0),
				new ColorItem("Frame active",      (byte)(Offset + 1),
				new ColorItem("Frame icons",       (byte)(Offset + 2),
				new ColorItem("Scroll bar page",   (byte)(Offset + 3),
				new ColorItem("Scroll bar icons",  (byte)(Offset + 4),
				new ColorItem("Normal text",       (byte)(Offset + 5),
				Next))))));
		}
	}

	public class ColorGroup
	{
		public string Name;
		public byte Index;
		public ColorItem Items;
		public ColorGroup Next;
		public ColorGroup( string AName, ColorItem AItems, byte AIndex, ColorGroup ANext)
		{
			Name = AName;
			Index = AIndex;
			Next = ANext;
			Items = AItems;
		}

		public ColorGroup( string AName, ColorItem AItems, ColorGroup ANext)
		{
			Name = AName;
			Index = 0;
			Next = ANext;
			Items = AItems;
		}
	}

	public class ColorGroupList : ListViewer
	{

		public ColorGroup Groups;

		public ColorGroupList( Rect Bounds, ScrollBar AScrollBar, ColorGroup AGroups):base( Bounds, 1, null, AScrollBar)
		{
			Groups = AGroups;
			int I = 0;
			while( AGroups != null)
			{
				AGroups = AGroups.Next;
				I++;
			}
			Range = I;
		}

		public override string GetText( int Item, int MaxLen)
		{
			ColorGroup CurGroup = Groups;
			while( Item > 0)
			{
				CurGroup = CurGroup.Next;
				Item --;
			}
			return CurGroup.Name;
		}
	}
}
