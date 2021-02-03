using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.App
{
	/// <summary>
	/// Summary description for Desktop.
	/// </summary>
	public class Desktop : Group
	{

		public Background Background = null;

		private bool tileColumnFirst;

		public Desktop( Rect Bounds):base( Bounds)
		{
			GrowMode = GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
			InitBackground();
			if( Background != null)
				Insert( Background);
		}

		public virtual void InitBackground()
		{
			Background = new Background(GetExtent(), (char)ldDesktopBackground);
		}
		
		public bool TileColumnFirst
		{
			get
			{
				return tileColumnFirst;
			}
			set
			{
				tileColumnFirst = value;
			}
		}

		public virtual void TileError()
		{
		}

		int _CascadeNum = 0;
		View _LastView = null;

		internal void DoCount( View P, params object[] o)
		{
			if( Tileable(P))
			{
				_CascadeNum++;
				_LastView = P;
			}
		}

		internal void DoCascade( View P, params object[] o)
		{
			if( Tileable(P) && ( _CascadeNum > 0))
			{
				((Rect)o[0]).A.X += _CascadeNum;
				((Rect)o[0]).A.Y += _CascadeNum;
				P.Locate( ((Rect)o[0]));
				_CascadeNum--;
			}
		}

		internal bool Tileable( View P)
		{
			return ( ((P.Options & OptionFlags.ofTileable) != 0) &&
				( (State & StateFlags.Visible) != 0));
		}

        public void Cascade(Rect R)
        {
            Point Min, Max;
            _CascadeNum = 0;
            ForEach(new ForEachProc(DoCount), null);
            if (_CascadeNum > 0)
            {
                _LastView.SizeLimits(out Min, out Max);
                if ((Min.X > (R.B.X - R.A.X - _CascadeNum)) ||
                    (Min.Y > (R.B.Y - R.A.Y - _CascadeNum)))
                    TileError();
                else
                {
                    _CascadeNum--;
                    Lock();
                    ForEach(new ForEachProc(DoCascade), R);
                    Unlock();
                }
            }
        }


        private int NumTileable;
		private int TileNum;
		private int NumCols;
		private int NumRows;
		private Rect XR;
		private int LeftOver;

		public static int ISqr( int X)
		{
			int Result = 0;
			do
				Result++;
			while( (Result*Result) < X);
			return Result;
		}

		public static void MostEqualDivisors( int N, ref int X, ref int Y, bool FavorY)
		{
			int i;
			i = ISqr( N);
			if( (N % i) != 0)
				if( ( N % (i +1)) == 0)
					i++;
			if( i < ( N / i))
				i = N / i;
			if( FavorY)
			{
				X = N / i;
				Y = i;
			}
			else
			{
				Y = N / i;
				X = i;
			}
		}

		internal void DoCountTileable( View P, params object[] o)
		{
			if( Tileable(P))
				NumTileable++;
		}

		internal void DoTile( View P, params object[] o)
		{
			if( Tileable(P))
			{
				CalcTileRect( TileNum, ref XR);
				P.Locate( XR);
				TileNum--;
			}
		}

		internal void CalcTileRect( int Pos, ref Rect NR)
		{
			int X, Y, D;
			D = ( NumCols - LeftOver) * NumRows;
			if( Pos < D)
			{
				X = Pos / NumRows;
				Y = Pos % NumRows;
			}
			else
			{
				X = (( Pos - D) / (NumRows + 1)) + ( NumCols - LeftOver);
				Y = ( Pos - D) % ( NumRows + 1);
			}
			NR.A.X = (int)DividerLoc(XR.A.X, XR.B.X, NumCols, X);
			NR.B.X = (int)DividerLoc(XR.A.X, XR.B.X, NumCols, X + 1);
			if( Pos >= D)
			{
				NR.A.Y = (int)DividerLoc( XR.A.Y, XR.B.Y, NumRows + 1, Y);
				NR.B.Y = (int)DividerLoc( XR.A.Y, XR.B.Y, NumRows + 1, Y + 1);
			}
			else
			{
				NR.A.Y = (int)DividerLoc( XR.A.Y, XR.B.Y, NumRows, Y);
				NR.B.Y = (int)DividerLoc( XR.A.Y, XR.B.Y, NumRows, Y + 1);
			}
		}

		internal static long DividerLoc( long Lo, long Hi, long Num, long Pos)
		{
			return (( Hi - Lo) * Pos ) / Num + Lo;
		}

		public void Tile( Rect R)
		{
			XR = R;
			int TileNum;
			NumTileable = 0;
			ForEach( new ForEachProc( DoCountTileable), null);
			if( NumTileable > 0)
			{
				MostEqualDivisors( NumTileable, ref NumCols, ref NumRows, !TileColumnFirst);
				if( (((XR.B.X - XR.A.X) / NumCols) == 0) || (((XR.B.Y - XR.A.Y)/ NumRows) == 0))
					TileError();
				else
				{
					LeftOver = NumTileable % NumCols;
					TileNum = NumTileable - 1;
					Lock();
					ForEach( new ForEachProc( DoTile));
					Unlock();
				}
			}
		}

		public override void HandleEvent(ref Event Event)
		{
			base.HandleEvent (ref Event);
			if( Event.What == Event.evCommand)
			{
				switch( Event.Command)
				{
					case cmNext : FocusNext( false);
						break;
					case cmPrev : 
						if( Valid( cmReleasedFocus))
							Current.PutInFrontOf( Background);
						break;
					default :
						return;
				}
				ClearEvent( ref Event);
			}
		}

	}
}
