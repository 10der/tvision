using System;

namespace TurboVision.Objects
{
    [Serializable]
	public struct Point
	{
		public int X;
		public int Y;

		public static Point operator - ( Point aPoint, Point dPoint)
		{
			aPoint.X -= dPoint.X;
			aPoint.Y -= dPoint.Y;
			return aPoint;
		}

		public static Point operator + ( Point aPoint, Point dPoint)
		{
			aPoint.X += dPoint.X;
			aPoint.Y += dPoint.Y;
			return aPoint;
		}

		public Point( int AX, int AY)
		{
			X = AX;
			Y = AY;
		}
	}

    [Serializable]
	public class Rect
	{
		public Point A;
		public Point B;

        public Rect()
        {
        }

		public Rect( int XA, int YA, int XB, int YB)
		{
			A.X = XA;
			A.Y = YA;
			B.X = XB;
			B.Y = YB;
		}

		public bool Empty()
		{
			return ( A.X >= B.X || A.Y >= B.Y);
		}

		public bool Equals( Rect R)
		{
			return ( A.X == R.A.X && A.Y == R.A.Y && B.X == R.B.X && B.Y == R.B.Y);
		}

		public bool Contains( Point P)
		{
			return ( (P.X >= A.X) && (P.X < B.X) && (P.Y >= A.Y) && (P.Y < B.Y));
		}

		public void Copy( Rect R)
		{
			A = R.A;
			B = R.B;
		}

		public void Union( Rect R)
		{
			if ( R.A.X < A.X)
				A.X = R.A.X;
			if( R.A.Y < A.Y)
				A.Y = R.A.Y;
			if( R.B.X > B.X)
				B.X = R.B.X;
			if( R.B.Y > B.Y)
				B.Y = R.B.Y;
		}

		public static void CheckEmpty( Rect R)
		{
			if( R.A.X >= R.B.X || R.A.Y >= R.B.Y)
			{
				R.A.X = 0;
				R.B.X = 0;
				R.A.Y = 0;
				R.B.Y = 0;
			}
		}

		public void Intersect( Rect R)
		{
			if( R.A.X > A.X)
				A.X = R.A.X;
			if( R.A.Y > A.Y)
				A.Y = R.A.Y;
			if( R.B.X < B.X)
				B.X = R.B.X;
			if( R.B.Y < B.Y)
				B.Y = R.B.Y;
			CheckEmpty( this);
		}

		public void Move( int ADX, int ADY)
		{
			A.X += ADX;
			A.Y += ADY;
			B.X += ADX;
			B.Y += ADY;
		}

		public void Grow( int ADX, int ADY)
		{
			A.X -= ADX;
			A.Y -= ADY;
			B.X += ADX;
			B.Y += ADY;
			CheckEmpty( this);
		}

        public static Rect operator +(Rect aRect, Rect dRect)
        {
            return new Rect(
                aRect.A.X + dRect.A.X,
                aRect.A.Y + dRect.A.Y,
                aRect.B.X + dRect.B.X,
                aRect.B.Y + dRect.B.Y);
        }

        public override string ToString()
		{
			return string.Format( "R.A.X : {0}\nR.A.Y : {1}\nR.B.X : {2}\nR.B.Y : {3}", A.X, A.Y, B.X, B.Y);
		}
	}
}
