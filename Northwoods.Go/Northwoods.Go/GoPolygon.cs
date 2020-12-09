using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Northwoods.Go
{
	/// <summary>
	/// A shape whose number, style, and location of sides can be specified.
	/// </summary>
	[Serializable]
	public class GoPolygon : GoShape
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int AddedPoint = 1401;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoPolygon.AddedPoint" />.
		/// </summary>
		public const int ChangedAddPoint = 1401;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedPoint = 1402;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoPolygon.RemovedPoint" />.
		/// </summary>
		public const int ChangedRemovePoint = 1402;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ModifiedPoint = 1403;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoPolygon.ModifiedPoint" />.
		/// </summary>
		public const int ChangedModifiedPoint = 1403;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedAllPoints = 1412;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoPolygon.Style" /> property.
		/// </summary>
		public const int ChangedStyle = 1414;

		private GoPolygonStyle myStyle;

		private int myPointsCount;

		private PointF[] myPoints = new PointF[6];

		/// <summary>
		/// Gets or sets the style of curve drawn using this polygon's points.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoPolygonStyle.Line" />, straight lines
		/// between the points.  A value of <see cref="F:Northwoods.Go.GoPolygonStyle.Bezier" />
		/// results in a closed poly-Bezier shape, with a straight line segment
		/// closing the shape between the last and first point if they are
		/// not the same point.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoPolygonStyle.Line)]
		[Description("The kind of line or curve drawn using this polygon's points.")]
		public virtual GoPolygonStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				GoPolygonStyle goPolygonStyle = myStyle;
				if (goPolygonStyle != value)
				{
					myStyle = value;
					ResetPath();
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1414, 0, goPolygonStyle, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the number of points in this polygon.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.GetPoint(System.Int32)" />.
		/// <seealso cref="M:Northwoods.Go.GoPolygon.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoPolygon.AddPoint(System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoPolygon.InsertPoint(System.Int32,System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoPolygon.RemovePoint(System.Int32)" />.
		[Category("Appearance")]
		[Description("The number of points in this polygon.")]
		public virtual int PointsCount => myPointsCount;

		/// <summary>
		/// The constructor produces a polygon that users can resize in real time.
		/// </summary>
		public GoPolygon()
		{
			base.InternalFlags |= 512;
		}

		/// <summary>
		/// Make sure the cloned polygon does not share any data references with the
		/// original polygon.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoPolygon goPolygon = (GoPolygon)base.CopyObject(env);
			if (goPolygon != null)
			{
				goPolygon.myPoints = (PointF[])myPoints.Clone();
			}
			return goPolygon;
		}

		/// <summary>
		/// Add another point to the end of this polygon's curve.
		/// </summary>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <returns>the zero-based index of the point that was added</returns>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.InsertPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoPolygon.RemovePoint(System.Int32)" />
		public virtual int AddPoint(PointF p)
		{
			return InternalInsertPoint(myPointsCount, p);
		}

		/// <summary>
		/// This method is just a convenience overload of <see cref="M:Northwoods.Go.GoPolygon.AddPoint(System.Drawing.PointF)" />.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>the zero-based index of the point that was added</returns>
		public int AddPoint(float x, float y)
		{
			return AddPoint(new PointF(x, y));
		}

		/// <summary>
		/// Add a point at a particular index, thereby increasing the number of points by one.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <remarks>
		/// This method throws an <c>ArgumentException</c> if the index <paramref name="i" />
		/// is less than zero.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.AddPoint(System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoPolygon.RemovePoint(System.Int32)" />
		public virtual void InsertPoint(int i, PointF p)
		{
			InternalInsertPoint(i, p);
		}

		private int InternalInsertPoint(int i, PointF p)
		{
			if (i < 0)
			{
				throw new ArgumentException("GoPolygon.InsertPoint given an invalid index, less than zero");
			}
			if (i > myPointsCount)
			{
				i = myPointsCount;
			}
			ResetPath();
			int num = myPoints.Length;
			checked
			{
				if (myPointsCount >= num)
				{
					PointF[] destinationArray = new PointF[Math.Max(num * 2, myPointsCount + 1)];
					Array.Copy(myPoints, 0, destinationArray, 0, num);
					myPoints = destinationArray;
				}
				if (myPointsCount > i)
				{
					Array.Copy(myPoints, i, myPoints, i + 1, myPointsCount - i);
				}
				myPointsCount++;
				myPoints[i] = p;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1401, i, null, GoObject.MakeRect(p), i, null, GoObject.MakeRect(p));
				return i;
			}
		}

		/// <summary>
		/// Remove the point at a particular index, thereby reducing the number of points by one.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <remarks>
		/// This method does nothing if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.AddPoint(System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoPolygon.InsertPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoPolygon.PointsCount" />
		public virtual void RemovePoint(int i)
		{
			InternalRemovePoint(i);
		}

		private void InternalRemovePoint(int i)
		{
			checked
			{
				if (i >= 0 && i < myPointsCount)
				{
					ResetPath();
					PointF p = myPoints[i];
					if (myPointsCount > i + 1)
					{
						Array.Copy(myPoints, i + 1, myPoints, i, myPointsCount - i - 1);
					}
					myPointsCount--;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1402, i, null, GoObject.MakeRect(p), i, null, GoObject.MakeRect(p));
				}
			}
		}

		/// <summary>
		/// Get the point at a particular index.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <returns>A <c>PointF</c> in document coordinates</returns>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.SetPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoPolygon.PointsCount" />
		public virtual PointF GetPoint(int i)
		{
			if (i >= 0 && i < myPointsCount)
			{
				return myPoints[i];
			}
			throw new ArgumentException("GoPolygon.GetPoint given an invalid index");
		}

		/// <summary>
		/// Replace the point at a particular index;
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.GetPoint(System.Int32)" />
		/// <seealso cref="P:Northwoods.Go.GoPolygon.PointsCount" />
		public virtual void SetPoint(int i, PointF p)
		{
			InternalSetPoint(i, p);
		}

		private void InternalSetPoint(int i, PointF p)
		{
			if (i >= 0 && i < myPointsCount)
			{
				PointF pointF = myPoints[i];
				if (pointF != p)
				{
					ResetPath();
					myPoints[i] = p;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1403, i, null, GoObject.MakeRect(pointF), i, null, GoObject.MakeRect(p));
				}
				return;
			}
			throw new ArgumentException("GoPolygon.SetPoint given an invalid index");
		}

		/// <summary>
		/// Remove all of the points for this polygon.
		/// </summary>
		/// <remarks>
		/// Afterwards, this polygon is not likely to participate usefully in many
		/// operations, such as painting, until more points are added by calling
		/// <see cref="M:Northwoods.Go.GoPolygon.AddPoint(System.Drawing.PointF)" /> or <see cref="M:Northwoods.Go.GoPolygon.SetPoints(System.Drawing.PointF[])" />.
		/// </remarks>
		public virtual void ClearPoints()
		{
			if (PointsCount > 0)
			{
				Changing(1412);
				ResetPath();
				myPointsCount = 0;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1412, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Replace all of the points for this polygon.
		/// </summary>
		/// <param name="points">A <c>PointF</c> array whose points are in document coordinates</param>
		/// <remarks>
		/// Afterwards, <see cref="P:Northwoods.Go.GoPolygon.PointsCount" /> should equal the length of the
		/// <paramref name="points" /> array.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPolygon.CopyPointsArray" />
		public virtual void SetPoints(PointF[] points)
		{
			if (points != null)
			{
				Changing(1412);
				ResetPath();
				int num = points.Length;
				if (num > myPoints.Length)
				{
					myPoints = new PointF[num];
				}
				Array.Copy(points, 0, myPoints, 0, num);
				myPointsCount = num;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1412, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Returns a copy of the array of points in this polygon.
		/// </summary>
		/// <value>
		/// An array of <c>PointF</c> values.
		/// </value>
		[Category("Appearance")]
		[Description("A copy of the array of points in this polygon.")]
		public virtual PointF[] CopyPointsArray()
		{
			PointF[] array = new PointF[myPointsCount];
			Array.Copy(myPoints, 0, array, 0, myPointsCount);
			return array;
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			int pointsCount = PointsCount;
			PointF[] array = new PointF[pointsCount];
			for (int i = 0; i < pointsCount; i = checked(i + 1))
			{
				array[i] = GetPoint(i);
			}
			bool flag = Style == GoPolygonStyle.Bezier;
			if (flag && pointsCount % 3 != 1)
			{
				GoObject.Trace("Polygon has wrong number of points: " + pointsCount.ToString(NumberFormatInfo.InvariantInfo) + "; should have 3n+1 points");
				flag = false;
			}
			if (flag)
			{
				graphicsPath.AddBeziers(array);
			}
			else
			{
				graphicsPath.AddLines(array);
			}
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		/// <summary>
		/// Changing the bounds of a polygon may change all of its points.
		/// </summary>
		/// <param name="old">the earlier bounds, a <c>RectangleF</c> in document coordinates</param>
		/// <remarks>
		/// All of the points are modified to reflect the translation and
		/// scaling of the new bounding rectangle from the old one.
		/// </remarks>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			_ = PointsCount;
			RectangleF bounds = Bounds;
			if (old.Width == bounds.Width && old.Height == bounds.Height)
			{
				float num = bounds.X - old.X;
				float num2 = bounds.Y - old.Y;
				if (num != 0f || num2 != 0f)
				{
					GoStroke.TranslatePoints(myPoints, num, num2);
					base.InvalidBounds = false;
				}
			}
			else
			{
				Changing(1412);
				GoStroke.RescalePoints(myPoints, old, bounds);
				base.InvalidBounds = false;
				Changed(1412, 0, null, old, 0, null, bounds);
			}
		}

		/// <summary>
		/// The bounding rectangle of a polygon is computed as the smallest
		/// rectangle that includes all of its points.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// If there are only zero or one points, the size will be zero.
		/// The computed bounds for Bezier-sided polygons are not necessarily the closest fitting.
		/// </remarks>
		protected override RectangleF ComputeBounds()
		{
			int pointsCount = PointsCount;
			if (pointsCount <= 0)
			{
				PointF position = base.Position;
				return new RectangleF(position.X, position.Y, 0f, 0f);
			}
			PointF point = GetPoint(0);
			float num = point.X;
			float num2 = point.Y;
			float num3 = point.X;
			float num4 = point.Y;
			checked
			{
				if (Style == GoPolygonStyle.Bezier)
				{
					for (int i = 3; i < myPointsCount; i += 3)
					{
						PointF point2 = GetPoint(i - 3);
						PointF point3 = GetPoint(i - 2);
						if (i + 3 >= myPointsCount)
						{
							i = myPointsCount - 1;
						}
						PointF point4 = GetPoint(i - 1);
						PointF point5 = GetPoint(i);
						RectangleF rectangleF = GoStroke.BezierBounds(point2, point3, point4, point5, 0.1f);
						num = Math.Min(num, rectangleF.X);
						num2 = Math.Min(num2, rectangleF.Y);
						num3 = Math.Max(num3, rectangleF.X + rectangleF.Width);
						num4 = Math.Max(num4, rectangleF.Y + rectangleF.Height);
					}
				}
				else
				{
					for (int j = 1; j < pointsCount; j++)
					{
						point = GetPoint(j);
						num = Math.Min(num, point.X);
						num2 = Math.Min(num2, point.Y);
						num3 = Math.Max(num3, point.X);
						num4 = Math.Max(num4, point.Y);
					}
				}
				return new RectangleF(num, num2, num3 - num, num4 - num2);
			}
		}

		/// <summary>
		/// A point is in a polygon only if it is inside its fill area,
		/// even if it has no Brush.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// This currently does not take pen width into account.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			if (!base.ContainsPoint(p))
			{
				return false;
			}
			GraphicsPath path = GetPath();
			bool result = path.IsVisible(p);
			DisposePath(path);
			return result;
		}

		/// <summary>
		/// Return the index of the first point of a segment of this polygon
		/// that is close to a given point.
		/// </summary>
		/// <param name="pnt">A <c>PointF</c> in document coordinates</param>
		/// <param name="pickMargin">
		/// the approximate distance from each line segment that is allowed,
		/// in addition to the width of the Pen; assumed to be non-negative
		/// </param>
		/// <returns>
		/// The zero-based index of the first point of a segment,
		/// or <c>-1</c> if no segment is near <paramref name="pnt" />.
		/// </returns>
		/// <remarks>
		/// This ignores the filled area and only considers the boundary
		/// defined by the polygon points that is drawn by the Pen
		/// (or if there is no Pen, what would be drawn if there were a Pen).
		/// For Bezier style polygons, this returns the index of the first of
		/// each set of points, e.g. 0, 3, 7, ....
		/// </remarks>
		public int GetSegmentNearPoint(PointF pnt, float pickMargin)
		{
			RectangleF bounds = Bounds;
			float num = Math.Max(PenWidth, 0.1f);
			float num2 = Math.Max(pickMargin, 0f);
			num += num2;
			if (pnt.X < bounds.X - num || pnt.X > bounds.X + bounds.Width + num || pnt.Y < bounds.Y - num || pnt.Y > bounds.Y + bounds.Height + num)
			{
				return -1;
			}
			int pointsCount = PointsCount;
			if (pointsCount <= 1)
			{
				return -1;
			}
			num -= num2 / 2f;
			checked
			{
				if (Style == GoPolygonStyle.Bezier && pointsCount >= 4)
				{
					num *= Math.Max(1f, Math.Max(bounds.Width, bounds.Height) / 1000f);
					for (int i = 3; i < pointsCount; i += 3)
					{
						int result = i - 3;
						PointF point = GetPoint(i - 3);
						PointF point2 = GetPoint(i - 2);
						if (i + 3 >= pointsCount)
						{
							i = pointsCount - 1;
						}
						PointF point3 = GetPoint(i - 1);
						PointF point4 = GetPoint(i);
						if (GoStroke.BezierContainsPoint(point, point2, point3, point4, num, pnt))
						{
							return result;
						}
					}
				}
				else
				{
					for (int j = 0; j < pointsCount - 1; j++)
					{
						PointF point5 = GetPoint(j);
						PointF point6 = GetPoint(j + 1);
						if (GoStroke.LineContainsPoint(point5, point6, num, pnt))
						{
							return j;
						}
					}
				}
				PointF point7 = GetPoint(pointsCount - 1);
				PointF point8 = GetPoint(0);
				if (point7 != point8 && GoStroke.LineContainsPoint(point7, point8, num, pnt))
				{
					return pointsCount - 1;
				}
				return -1;
			}
		}

		/// <summary>
		/// The closest intersection point of a polygon with a line is the
		/// closest such point for each of its segments.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		/// <remarks>
		/// This currently does not always take into account any Pen width.
		/// </remarks>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float num = PenWidth / 2f;
			float num2 = 1E+21f;
			PointF pointF = default(PointF);
			checked
			{
				PointF result2;
				if (Style == GoPolygonStyle.Bezier)
				{
					for (int i = 3; i < myPointsCount; i += 3)
					{
						PointF point = GetPoint(i - 3);
						PointF point2 = GetPoint(i - 2);
						if (i + 3 >= myPointsCount)
						{
							i = myPointsCount - 1;
						}
						PointF point3 = GetPoint(i - 1);
						PointF point4 = GetPoint(i);
						if (GoStroke.BezierNearestIntersectionOnLine(point, point2, point3, point4, p1, p2, num, out result2))
						{
							float num3 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num3 < num2)
							{
								num2 = num3;
								pointF = result2;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < PointsCount; j++)
					{
						PointF a = GoShape.ExpandPointOnEdge(GetPoint(j), bounds, num);
						PointF b = GoShape.ExpandPointOnEdge(GetPoint((j + 1 < PointsCount) ? (j + 1) : 0), bounds, num);
						if (GoStroke.NearestIntersectionOnLine(a, b, p1, p2, out result2))
						{
							float num4 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num4 < num2)
							{
								num2 = num4;
								pointF = result2;
							}
						}
					}
				}
				result = pointF;
				return num2 < 1E+21f;
			}
		}

		/// <summary>
		/// When the resize handles are at each point of the polygon, the user's
		/// dragging of a resize handle should just change that point in the curve.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> is true, this method always calls
		/// <see cref="M:Northwoods.Go.GoPolygon.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// Otherwise it only calls <see cref="M:Northwoods.Go.GoPolygon.SetPoint(System.Int32,System.Drawing.PointF)" /> when the <paramref name="evttype" />
		/// is <see cref="F:Northwoods.Go.GoInputState.Finish" /> or <see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (whichHandle >= 8192 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				SetPoint(checked(whichHandle - 8192), newPoint);
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// Display the appropriate selected appearance, normally, resize selection handles
		/// at each point of the polygon.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// If this polygon is resizable and reshapable, we add resize selection
		/// handles at each polygon point, with handle IDs equal to
		/// <see cref="F:Northwoods.Go.GoObject.LastHandle" /> plus the index of the point.
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			if (!(CanResize() && flag) || !(CanReshape() && flag2))
			{
				base.AddSelectionHandles(sel, selectedObj);
				return;
			}
			sel.RemoveHandles(this);
			checked
			{
				int num = PointsCount - 1;
				for (int i = 0; i <= num; i++)
				{
					PointF point = GetPoint(i);
					sel.CreateResizeHandle(this, selectedObj, point, 8192 + i, filled: true);
				}
			}
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1412)
			{
				if (e.IsBeforeChanging)
				{
					PointF[] array2 = (PointF[])(e.OldValue = CopyPointsArray());
				}
			}
			else
			{
				base.CopyOldValueForUndo(e);
			}
		}

		/// <summary>
		/// Copies state to permit a redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyNewValueForRedo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1412)
			{
				if (!e.IsBeforeChanging)
				{
					PointF[] array2 = (PointF[])(e.NewValue = CopyPointsArray());
				}
			}
			else
			{
				base.CopyNewValueForRedo(e);
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1001:
			{
				base.ChangeValue(e, undo);
				RectangleF rect = e.GetRect(!undo);
				RectangleF rect2 = e.GetRect(undo);
				if (rect.Width == rect2.Width && rect.Height == rect2.Height)
				{
					float dx = rect2.X - rect.X;
					float dy = rect2.Y - rect.Y;
					GoStroke.TranslatePoints(myPoints, dx, dy);
				}
				break;
			}
			case 1401:
				if (undo)
				{
					InternalRemovePoint(e.OldInt);
				}
				else
				{
					InternalInsertPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
				}
				break;
			case 1402:
				if (undo)
				{
					InternalInsertPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
				}
				else
				{
					InternalRemovePoint(e.OldInt);
				}
				break;
			case 1403:
				if (undo)
				{
					InternalSetPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
				}
				else
				{
					InternalSetPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
				}
				break;
			case 1412:
			{
				PointF[] points = (PointF[])e.GetValue(undo);
				SetPoints(points);
				break;
			}
			case 1414:
				Style = (GoPolygonStyle)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
