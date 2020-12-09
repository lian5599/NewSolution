using System;
using System.Drawing;

namespace Northwoods.Go
{
	[Serializable]
	internal sealed class GoPositionArray
	{
		private const int OCCUPIED = 0;

		internal const int VERT = 1;

		internal const int HORIZ = 2;

		private const int MASK = 3;

		private const int SHIFT = 2;

		private const int START = 4;

		private const int STEP = 4;

		private const int MAX = 2147483644;

		private const int UNOCCUPIED = int.MaxValue;

		internal const int StartDistance = 1;

		internal const int StepDistance = 1;

		internal const int MaxDistance = 536870911;

		private const float RIGHT = 0f;

		private const float DOWN = 90f;

		private const float LEFT = 180f;

		private const float UP = 270f;

		private bool myInvalid = true;

		private bool myAbort;

		private float myMinX = 1f;

		private float myMinY = 1f;

		private float myMaxX = -1f;

		private float myMaxY = -1f;

		private float myCellX = 8f;

		private float myCellY = 8f;

		private int[,] myArray;

		private int myUpperBoundX;

		private int myUpperBoundY;

		private bool myWholeDocument;

		private float mySmallMargin = 22f;

		private float myLargeMargin = 111f;

		internal bool Invalid
		{
			get
			{
				return myInvalid;
			}
			set
			{
				myInvalid = value;
			}
		}

		internal bool Abort
		{
			get
			{
				return myAbort;
			}
			set
			{
				myAbort = value;
			}
		}

		internal RectangleF Bounds => new RectangleF(myMinX, myMinY, myMaxX - myMinX, myMaxY - myMinY);

		internal SizeF CellSize
		{
			get
			{
				return new SizeF(myCellX, myCellY);
			}
			set
			{
				if (value.Width > 0f && value.Height > 0f && (value.Width != myCellX || value.Height != myCellY))
				{
					myCellX = value.Width;
					myCellY = value.Height;
					Initialize(new RectangleF(myMinX, myMinY, myMaxX - myMinX, myMaxY - myMinY));
				}
			}
		}

		internal bool WholeDocument
		{
			get
			{
				return myWholeDocument;
			}
			set
			{
				myWholeDocument = value;
			}
		}

		internal float SmallMargin
		{
			get
			{
				return mySmallMargin;
			}
			set
			{
				mySmallMargin = value;
			}
		}

		internal float LargeMargin
		{
			get
			{
				return myLargeMargin;
			}
			set
			{
				myLargeMargin = value;
			}
		}

		internal GoPositionArray()
		{
		}

		internal void Initialize(RectangleF rect)
		{
			checked
			{
				if (!(rect.Width <= 0f) && !(rect.Height <= 0f))
				{
					float x = rect.X;
					float y = rect.Y;
					float num = rect.X + rect.Width;
					float num2 = rect.Y + rect.Height;
					myMinX = (float)Math.Floor((x - myCellX) / myCellX) * myCellX;
					myMinY = (float)Math.Floor((y - myCellY) / myCellY) * myCellY;
					myMaxX = (float)Math.Ceiling((num + 2f * myCellX) / myCellX) * myCellX;
					myMaxY = (float)Math.Ceiling((num2 + 2f * myCellY) / myCellY) * myCellY;
					int num3 = 1 + (int)Math.Ceiling((myMaxX - myMinX) / myCellX);
					int num4 = 1 + (int)Math.Ceiling((myMaxY - myMinY) / myCellY);
					if (myArray == null || myUpperBoundX < num3 - 1 || myUpperBoundY < num4 - 1)
					{
						myArray = new int[num3, num4];
						myUpperBoundX = num3 - 1;
						myUpperBoundY = num4 - 1;
					}
					SetAll(int.MaxValue);
				}
			}
		}

		private bool InBounds(float x, float y)
		{
			if (myMinX <= x && x <= myMaxX && myMinY <= y)
			{
				return y <= myMaxY;
			}
			return false;
		}

		internal int GetDist(float x, float y)
		{
			if (!InBounds(x, y))
			{
				return 0;
			}
			x -= myMinX;
			x /= myCellX;
			y -= myMinY;
			y /= myCellY;
			checked
			{
				int num = (int)x;
				int num2 = (int)y;
				return myArray[num, num2] >> 2;
			}
		}

		internal void SetOccupied(float x, float y)
		{
			checked
			{
				if (InBounds(x, y))
				{
					x -= myMinX;
					x /= myCellX;
					y -= myMinY;
					y /= myCellY;
					int num = (int)x;
					int num2 = (int)y;
					myArray[num, num2] = 0;
				}
			}
		}

		internal void SetAll(int v)
		{
			if (myArray == null)
			{
				return;
			}
			checked
			{
				for (int i = 0; i <= myUpperBoundX; i++)
				{
					for (int j = 0; j <= myUpperBoundY; j++)
					{
						myArray[i, j] = v;
					}
				}
			}
		}

		internal void ClearAllUnoccupied()
		{
			if (myArray == null)
			{
				return;
			}
			checked
			{
				for (int i = 0; i <= myUpperBoundX; i++)
				{
					for (int j = 0; j <= myUpperBoundY; j++)
					{
						if (myArray[i, j] >= 4)
						{
							myArray[i, j] |= 2147483644;
						}
					}
				}
			}
		}

		internal bool IsOccupied(float x, float y)
		{
			return GetDist(x, y) == 0;
		}

		internal bool IsUnoccupied(float x, float y, float w, float h)
		{
			if (x > myMaxX)
			{
				return true;
			}
			if (x + w < myMinX)
			{
				return true;
			}
			if (y > myMaxY)
			{
				return true;
			}
			if (y + h < myMinY)
			{
				return true;
			}
			checked
			{
				int num = (int)((x - myMinX) / myCellX);
				int num2 = (int)((y - myMinY) / myCellY);
				int num3 = (int)(Math.Max(0f, w) / myCellX) + 1;
				int num4 = (int)(Math.Max(0f, h) / myCellY) + 1;
				if (num < 0)
				{
					num3 += num;
					num = 0;
				}
				if (num2 < 0)
				{
					num4 += num2;
					num2 = 0;
				}
				if (num3 < 0)
				{
					return true;
				}
				if (num4 < 0)
				{
					return true;
				}
				int num5 = Math.Min(num + num3 - 1, myUpperBoundX);
				int num6 = Math.Min(num2 + num4 - 1, myUpperBoundY);
				for (int i = num; i <= num5; i++)
				{
					for (int j = num2; j <= num6; j++)
					{
						if (myArray[i, j] == 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		private int Ray(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
		{
			int num = myArray[x, y] & -4;
			checked
			{
				if (num >= 4 && num < 2147483644)
				{
					if (vert)
					{
						y += inc;
					}
					else
					{
						x += inc;
					}
					num += 4;
					while (lowx <= x && x <= hix && lowy <= y && y <= hiy)
					{
						int num2 = myArray[x, y];
						if (num >= (num2 & -4))
						{
							break;
						}
						myArray[x, y] = (num | (num2 & 3));
						num += 4;
						if (vert)
						{
							y += inc;
						}
						else
						{
							x += inc;
						}
					}
				}
				if (vert)
				{
					return y;
				}
				return x;
			}
		}

		private void Spread(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
		{
			if (x < lowx || x > hix || y < lowy || y > hiy)
			{
				return;
			}
			int num = Ray(x, y, inc, vert, lowx, hix, lowy, hiy);
			checked
			{
				if (vert)
				{
					if (inc > 0)
					{
						for (int i = y + inc; i < num; i += inc)
						{
							Spread(x, i, 1, !vert, lowx, hix, lowy, hiy);
							Spread(x, i, -1, !vert, lowx, hix, lowy, hiy);
						}
					}
					else
					{
						for (int j = y + inc; j > num; j += inc)
						{
							Spread(x, j, 1, !vert, lowx, hix, lowy, hiy);
							Spread(x, j, -1, !vert, lowx, hix, lowy, hiy);
						}
					}
				}
				else if (inc > 0)
				{
					for (int k = x + inc; k < num; k += inc)
					{
						Spread(k, y, 1, !vert, lowx, hix, lowy, hiy);
						Spread(k, y, -1, !vert, lowx, hix, lowy, hiy);
					}
				}
				else
				{
					for (int l = x + inc; l > num; l += inc)
					{
						Spread(l, y, 1, !vert, lowx, hix, lowy, hiy);
						Spread(l, y, -1, !vert, lowx, hix, lowy, hiy);
					}
				}
			}
		}

		private bool IsBarrier(int v)
		{
			return (v & 3) == 0;
		}

		private int BreakOut(int x1, int y1, int x2, int y2, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
		{
			int num = x1;
			int num2 = y1;
			int v = myArray[num, num2];
			checked
			{
				while (IsBarrier(v) && num > lowx && num < hix && num2 > lowy && num2 < hiy)
				{
					if (vert)
					{
						num2 += inc;
					}
					else
					{
						num += inc;
					}
					v = myArray[num, num2];
					if (Math.Abs(num - x2) <= 1 && Math.Abs(num2 - y2) <= 1)
					{
						Abort = true;
						return 0;
					}
				}
				num = x1;
				num2 = y1;
				v = myArray[num, num2];
				int num3 = 7;
				myArray[num, num2] = num3;
				while (IsBarrier(v) && num > lowx && num < hix && num2 > lowy && num2 < hiy)
				{
					if (vert)
					{
						num2 += inc;
					}
					else
					{
						num += inc;
					}
					v = myArray[num, num2];
					myArray[num, num2] = num3;
					num3 += 4;
				}
				if (vert)
				{
					return num2;
				}
				return num;
			}
		}

		private void BreakIn(int x1, int y1, int x2, int y2, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
		{
			int num = x2;
			int num2 = y2;
			int v = myArray[num, num2];
			checked
			{
				while (IsBarrier(v) && num > lowx && num < hix && num2 > lowy && num2 < hiy)
				{
					if (vert)
					{
						num2 += inc;
					}
					else
					{
						num += inc;
					}
					v = myArray[num, num2];
					if (Math.Abs(num - x1) <= 1 && Math.Abs(num2 - y1) <= 1)
					{
						Abort = true;
						return;
					}
				}
				num = x2;
				num2 = y2;
				v = myArray[num, num2];
				myArray[num, num2] = int.MaxValue;
				while (IsBarrier(v) && num > lowx && num < hix && num2 > lowy && num2 < hiy)
				{
					if (vert)
					{
						num2 += inc;
					}
					else
					{
						num += inc;
					}
					v = myArray[num, num2];
					myArray[num, num2] = int.MaxValue;
				}
			}
		}

		internal void Propagate(PointF p1, float fromDir, PointF p2, float toDir, RectangleF bounds)
		{
			if (myArray == null)
			{
				return;
			}
			Abort = false;
			float x = p1.X;
			float y = p1.Y;
			if (!InBounds(x, y))
			{
				return;
			}
			x -= myMinX;
			x /= myCellX;
			y -= myMinY;
			y /= myCellY;
			float x2 = p2.X;
			float y2 = p2.Y;
			if (!InBounds(x2, y2))
			{
				return;
			}
			x2 -= myMinX;
			x2 /= myCellX;
			y2 -= myMinY;
			y2 /= myCellY;
			if (Math.Abs(x - x2) <= 1f && Math.Abs(y - y2) <= 1f)
			{
				Abort = true;
				return;
			}
			float x3 = bounds.X;
			float y3 = bounds.Y;
			float num = bounds.X + bounds.Width;
			float num2 = bounds.Y + bounds.Height;
			x3 -= myMinX;
			x3 /= myCellX;
			y3 -= myMinY;
			y3 /= myCellY;
			num -= myMinX;
			num /= myCellX;
			num2 -= myMinY;
			num2 /= myCellY;
			checked
			{
				int lowx = Math.Max(0, Math.Min(myUpperBoundX, (int)x3));
				int hix = Math.Min(myUpperBoundX, Math.Max(0, (int)num));
				int lowy = Math.Max(0, Math.Min(myUpperBoundY, (int)y3));
				int hiy = Math.Min(myUpperBoundY, Math.Max(0, (int)num2));
				int num3 = (int)x;
				int num4 = (int)y;
				int x4 = (int)x2;
				int y4 = (int)y2;
				int x5 = num3;
				int y5 = num4;
				int inc = (fromDir == 0f || fromDir == 90f) ? 1 : (-1);
				bool flag = fromDir == 90f || fromDir == 270f;
				if (flag)
				{
					y5 = BreakOut(num3, num4, x4, y4, inc, flag, lowx, hix, lowy, hiy);
				}
				else
				{
					x5 = BreakOut(num3, num4, x4, y4, inc, flag, lowx, hix, lowy, hiy);
				}
				if (!Abort)
				{
					BreakIn(num3, num4, x4, y4, (toDir == 0f || toDir == 90f) ? 1 : (-1), toDir == 90f || toDir == 270f, lowx, hix, lowy, hiy);
					if (!Abort)
					{
						Spread(x5, y5, 1, vert: false, lowx, hix, lowy, hiy);
						Spread(x5, y5, -1, vert: false, lowx, hix, lowy, hiy);
						Spread(x5, y5, 1, vert: true, lowx, hix, lowy, hiy);
						Spread(x5, y5, -1, vert: true, lowx, hix, lowy, hiy);
					}
				}
			}
		}
	}
}
