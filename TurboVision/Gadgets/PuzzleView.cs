using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class PuzzleView : View
	{

		public byte[,] Board = new byte[6,6];
		public uint Moves;
		public bool Solved;

        private static uint[] CPuzzleView = { 0x06, 0x07 };

		public PuzzleView( Rect Bounds):base( Bounds)
		{
			Board[0, 0] = 65;//'A'
			Board[0, 1] = 66;
			Board[0, 2] = 67;
			Board[0, 3] = 68;
			Board[1, 0] = 69;
			Board[1, 1] = 70;
			Board[1, 2] = 71;
			Board[1, 3] = 72;
			Board[2, 0] = 73;
			Board[2, 1] = 74;
			Board[2, 2] = 75;
			Board[2, 3] = 76;
			Board[3, 0] = 77;
			Board[3, 1] = 78;
			Board[3, 2] = 79;
			Board[3, 3] = 32;

			Options |= OptionFlags.ofSelectable;
//			for ( int i = 0; i < Board.GetUpperBound(0); i++)
				//for ( int j = 0; j < Board.GetUpperBound(1); j++)
				//	Board[i, j] = (byte)'?';
			Scramble();
		}

		public void Scramble()
		{
			Moves = 0;
			Solved = false;
			Random R = new Random(4);
		}

        public override uint[] GetPalette()
		{
			return CPuzzleView;
		}

		public override void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			string S1;
			byte[] Color = new byte[2];
			byte ColorBack;

			byte[] Map = new byte[15];
			Map[0] = 0;
			Map[1] = 1;
			Map[2] = 0;
			Map[3] = 1;

			Map[4] = 1;
			Map[5] = 0;
			Map[6] = 1;
			Map[7] = 0;

			Map[8] = 0;
			Map[9] = 1;
			Map[10] = 0;
			Map[11] = 1;

			Map[12] = 1;
			Map[13] = 0;
			Map[14] = 1;

			Color[0] = (byte)GetColor(1);
			Color[1] = (byte)GetColor(2);
			ColorBack = (byte)GetColor(1);

			if( Solved)
				Color[1] = Color[0];
			else
				Color[1] = (byte)GetColor(2);
			for( int i = 0; i < 4; i++)
			{
				B.FillChar( (char)' ', ColorBack, 18);
				if( i == 1)
					B.FillStr( "Move", ColorBack, 13);
				if( i == 2)
				{
					S1 = string.Format("{0:G3}", Moves);
					B.FillStr( S1, ColorBack, 14);
				}
				for( int j = 0; j < 4; j++)
				{
					S1 = " " + (char)Board[ i, j] + " ";
					if( Board[ i, j] == ' ')
						B.FillStr( S1, Color[0], j * 3);
					else
						B.FillStr( S1, Color[Map[Board[i, j]-65]], j * 3);
				}
				WriteLine(0, i, 18, 1, B);
			}
		}
	}
}
