using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// A 2-D representation of a Rectangular Prism, with the 
	/// "depth" dimension controlled by an additional GoHandle.
	/// </summary>
	[Serializable]
	public class GoCube : GoShape
	{
		private const int flagReshapableDepth = 1048576;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCube.Depth" /> property.
		/// </summary>
		public const int ChangedDepth = 1491;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCube.Perspective" /> property.
		/// </summary>
		public const int ChangedPerspective = 1492;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCube.ReshapableDepth" /> property.
		/// </summary>
		public const int ChangedReshapableDepth = 1493;

		/// <summary>
		/// A special handle ID for a handle which controls the depth size.
		/// </summary>
		public const int DepthHandleID = 1033;

		private PointF[] myPoints = new PointF[7];

		private SizeF myDepth = new SizeF(10f, 10f);

		private GoPerspective myPerspective;

		/// <summary>
		/// Gets or sets the width and height of the depth dimension.
		/// </summary>
		/// <value>
		/// This defaults to 10x10.
		/// If given a negative value, it uses zero instead.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The offset of the back square from the forward square giving the impression of depth.")]
		public virtual SizeF Depth
		{
			get
			{
				return myDepth;
			}
			set
			{
				SizeF sizeF = myDepth;
				if (value.Width < 0f)
				{
					value.Width = 0f;
				}
				if (value.Height < 0f)
				{
					value.Height = 0f;
				}
				if (sizeF != value)
				{
					myDepth = value;
					ResetPath();
					Changed(1491, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the perspective from which the cube is viewed.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoPerspective.TopLeft" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoPerspective.TopLeft)]
		[Description("The direction the back square is offset from the front square.")]
		public virtual GoPerspective Perspective
		{
			get
			{
				return myPerspective;
			}
			set
			{
				GoPerspective goPerspective = myPerspective;
				if (goPerspective != value)
				{
					myPerspective = value;
					ResetPath();
					Changed(1492, 0, goPerspective, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the depth control handle.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether to add the depth control handle.")]
		public virtual bool ReshapableDepth
		{
			get
			{
				return (base.InternalFlags & 0x100000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x100000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 1048576;
					}
					else
					{
						base.InternalFlags &= -1048577;
					}
					Changed(1493, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a cube with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline, no <see cref="T:System.Drawing.Brush" /> fill.
		/// </summary>
		public GoCube()
		{
			base.InternalFlags |= 1049088;
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			PointF[] points = getPoints();
			graphicsPath.AddLine(points[0], points[1]);
			graphicsPath.AddLine(points[1], points[2]);
			graphicsPath.AddLine(points[2], points[3]);
			graphicsPath.AddLine(points[3], points[0]);
			graphicsPath.AddLine(points[0], points[4]);
			graphicsPath.AddLine(points[4], points[5]);
			graphicsPath.AddLine(points[5], points[1]);
			graphicsPath.AddLine(points[2], points[6]);
			graphicsPath.AddLine(points[6], points[5]);
			graphicsPath.AddLine(points[5], points[1]);
			return graphicsPath;
		}

		private PointF[] getPoints()
		{
			RectangleF bounds = Bounds;
			SizeF depth = Depth;
			if (depth.Width > bounds.Width)
			{
				depth.Width = bounds.Width;
			}
			if (depth.Height > bounds.Height)
			{
				depth.Height = bounds.Height;
			}
			if (Perspective == GoPerspective.TopRight)
			{
				myPoints[0] = new PointF(bounds.X, bounds.Y + depth.Height);
				myPoints[1] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y + depth.Height);
				myPoints[2] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y + bounds.Height);
				myPoints[3] = new PointF(bounds.X, bounds.Y + bounds.Height);
				myPoints[4] = new PointF(bounds.X + depth.Width, bounds.Y);
				myPoints[5] = new PointF(bounds.X + bounds.Width, bounds.Y);
				myPoints[6] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - depth.Height);
			}
			else if (Perspective == GoPerspective.BottomRight)
			{
				myPoints[0] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y);
				myPoints[1] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y + bounds.Height - depth.Height);
				myPoints[2] = new PointF(bounds.X, bounds.Y + bounds.Height - depth.Height);
				myPoints[3] = new PointF(bounds.X, bounds.Y);
				myPoints[4] = new PointF(bounds.X + bounds.Width, bounds.Y + depth.Height);
				myPoints[5] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
				myPoints[6] = new PointF(bounds.X + depth.Width, bounds.Y + bounds.Height);
			}
			else if (Perspective == GoPerspective.TopLeft)
			{
				myPoints[0] = new PointF(bounds.X + depth.Width, bounds.Y + bounds.Height);
				myPoints[1] = new PointF(bounds.X + depth.Width, bounds.Y + depth.Height);
				myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + depth.Height);
				myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
				myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height - depth.Height);
				myPoints[5] = new PointF(bounds.X, bounds.Y);
				myPoints[6] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y);
			}
			else if (Perspective == GoPerspective.BottomLeft)
			{
				myPoints[0] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - depth.Height);
				myPoints[1] = new PointF(bounds.X + depth.Width, bounds.Y + bounds.Height - depth.Height);
				myPoints[2] = new PointF(bounds.X + depth.Width, bounds.Y);
				myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y);
				myPoints[4] = new PointF(bounds.X + bounds.Width - depth.Width, bounds.Y + bounds.Height);
				myPoints[5] = new PointF(bounds.X, bounds.Y + bounds.Height);
				myPoints[6] = new PointF(bounds.X, bounds.Y + depth.Height);
			}
			return myPoints;
		}

		/// <summary>
		/// Considers the shape of the "outer" cube to determine
		/// if a given point is inside.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// Currently this does not take the pen width into account.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			if (!base.ContainsPoint(p))
			{
				return false;
			}
			return GetPath().IsVisible(p);
		}

		/// <summary>
		/// Support either allowing the user to move the depth control handle.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			RectangleF bounds = Bounds;
			SizeF depth = Depth;
			if (whichHandle == 1033 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				if (Perspective == GoPerspective.TopRight)
				{
					if (newPoint.Y > bounds.Y + bounds.Height)
					{
						depth.Height = bounds.Height;
					}
					else if (newPoint.Y < bounds.Y)
					{
						depth.Height = 0f;
					}
					else
					{
						depth.Height = newPoint.Y - bounds.Y;
					}
					if (newPoint.X > bounds.X + bounds.Width)
					{
						depth.Width = 0f;
					}
					else if (newPoint.X < bounds.X)
					{
						depth.Width = bounds.Width;
					}
					else
					{
						depth.Width = bounds.X + bounds.Width - newPoint.X;
					}
				}
				else if (Perspective == GoPerspective.BottomRight)
				{
					if (newPoint.Y < bounds.Y)
					{
						depth.Height = bounds.Height;
					}
					else if (newPoint.Y > bounds.Y + bounds.Height)
					{
						depth.Height = 0f;
					}
					else
					{
						depth.Height = bounds.Y + bounds.Height - newPoint.Y;
					}
					if (newPoint.X < bounds.X)
					{
						depth.Width = bounds.Width;
					}
					else if (newPoint.X > bounds.X + bounds.Width)
					{
						depth.Width = 0f;
					}
					else
					{
						depth.Width = bounds.X + bounds.Width - newPoint.X;
					}
				}
				else if (Perspective == GoPerspective.TopLeft)
				{
					if (newPoint.Y > bounds.Y + bounds.Height)
					{
						depth.Height = bounds.Height;
					}
					else if (newPoint.Y < bounds.Y)
					{
						depth.Height = 0f;
					}
					else
					{
						depth.Height = newPoint.Y - bounds.Y;
					}
					if (newPoint.X > bounds.X + bounds.Width)
					{
						depth.Width = bounds.Width;
					}
					else if (newPoint.X < bounds.X)
					{
						depth.Width = 0f;
					}
					else
					{
						depth.Width = newPoint.X - bounds.X;
					}
				}
				else if (Perspective == GoPerspective.BottomLeft)
				{
					if (newPoint.Y < bounds.Y)
					{
						depth.Height = bounds.Height;
					}
					else if (newPoint.Y > bounds.Y + bounds.Height)
					{
						depth.Height = 0f;
					}
					else
					{
						depth.Height = bounds.Y + bounds.Height - newPoint.Y;
					}
					if (newPoint.X > bounds.X + bounds.Width)
					{
						depth.Width = bounds.Width;
					}
					else if (newPoint.X < bounds.X)
					{
						depth.Width = 0f;
					}
					else
					{
						depth.Width = newPoint.X - bounds.X;
					}
				}
				Depth = depth;
				ResetPath();
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
				ResetPath();
			}
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> and <see cref="P:Northwoods.Go.GoCube.ReshapableDepth" />
		/// are true, this supports a depth control handle.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape() && ReshapableDepth)
			{
				PointF loc = getPoints()[1];
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, loc, 1033, filled: true);
				MakeDiamondResizeHandle(handle, 1);
			}
		}

		/// <summary>
		/// The closest point of the "outer" cube that intersects with a given line
		/// is the closest such point of each of its six line segments.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float shift = PenWidth / 2f;
			PointF[] points = getPoints();
			PointF pointF = GoShape.ExpandPointOnEdge(points[0], bounds, shift);
			PointF pointF2 = GoShape.ExpandPointOnEdge(points[4], bounds, shift);
			PointF pointF3 = GoShape.ExpandPointOnEdge(points[5], bounds, shift);
			PointF pointF4 = GoShape.ExpandPointOnEdge(points[6], bounds, shift);
			PointF pointF5 = GoShape.ExpandPointOnEdge(points[2], bounds, shift);
			PointF pointF6 = GoShape.ExpandPointOnEdge(points[3], bounds, shift);
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF7 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF3, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF4, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF4, pointF5, p1, p2, out result2))
			{
				float num5 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num5 < num)
				{
					num = num5;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF5, pointF6, p1, p2, out result2))
			{
				float num6 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num6 < num)
				{
					num = num6;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF6, pointF, p1, p2, out result2))
			{
				float num7 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num7 < num)
				{
					num = num7;
					pointF7 = result2;
				}
			}
			result = pointF7;
			return num < 1E+21f;
		}

		/// <summary>
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1491:
				Depth = e.GetSize(undo);
				break;
			case 1492:
				Perspective = (GoPerspective)e.GetValue(undo);
				break;
			case 1493:
				ReshapableDepth = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
