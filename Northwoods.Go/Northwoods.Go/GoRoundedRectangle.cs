using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of a rectangle or square, whose corners are quarter
	/// ellipses at the corners of the bounding rectangle.
	/// </summary>
	/// <remarks>
	/// Although by default all four corners are rounded by the value of <see cref="P:Northwoods.Go.GoRoundedRectangle.Corner" />,
	/// you can control which ones are rounded by setting <see cref="P:Northwoods.Go.GoRoundedRectangle.RoundedCornerSpots" />.
	/// For example, if you just want the top corners to be rounded-off, but you want to
	/// keep the bottom corners squared-off:
	/// <pre><code>r.RoundedCornerSpots = GoObject.TopLeft | GoObject.TopRight;</code></pre>
	/// </remarks>
	[Serializable]
	public class GoRoundedRectangle : GoRectangle
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoRoundedRectangle.Corner" /> property.
		/// </summary>
		public const int ChangedCorner = 1421;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoRoundedRectangle.RoundedCornerSpots" /> property.
		/// </summary>
		public const int ChangedRoundedCornerSpots = 1422;

		private SizeF myCorner = new SizeF(10f, 10f);

		private int myRoundedCornerSpots = 30;

		/// <summary>
		/// Gets or sets the maximum radial width and height of each corner.
		/// </summary>
		/// <value>
		/// The default value is 10x10.  Both the width and the height must be
		/// non-negative; only when both are positive will there be arcs at each corner.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum radial width and height of each corner")]
		public virtual SizeF Corner
		{
			get
			{
				return myCorner;
			}
			set
			{
				SizeF sizeF = myCorner;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCorner = value;
					ResetPath();
					Changed(1421, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets which corners are rounded off.
		/// </summary>
		/// <value>
		/// A bitwise combination of the following four <see cref="T:Northwoods.Go.GoObject" /> spots:
		/// <see cref="F:Northwoods.Go.GoObject.TopLeft" />, <see cref="F:Northwoods.Go.GoObject.TopRight" />,
		/// <see cref="F:Northwoods.Go.GoObject.BottomRight" />, <see cref="F:Northwoods.Go.GoObject.BottomLeft" />.
		/// By default this includes all four corners.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(30)]
		[Description("The maximum radial width and height of each corner")]
		public int RoundedCornerSpots
		{
			get
			{
				return myRoundedCornerSpots;
			}
			set
			{
				int num = myRoundedCornerSpots;
				if (num != value)
				{
					myRoundedCornerSpots = value;
					ResetPath();
					Changed(1422, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Paint a possibly shadowed rectangle with rounded corners.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <seealso cref="P:Northwoods.Go.GoRoundedRectangle.Corner" />
		public override void Paint(Graphics g, GoView view)
		{
			PaintPath(g, view, GetPath());
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			MakeRoundedRectangularPath(graphicsPath, Bounds, Corner, RoundedCornerSpots);
			return graphicsPath;
		}

		internal static void MakeRoundedRectangularPath(GraphicsPath path, float offx, float offy, RectangleF rect, SizeF corner)
		{
			MakeRoundedRectangularPath(path, new RectangleF(rect.X + offx, rect.Y + offy, rect.Width, rect.Height), corner, 30);
		}

		internal static void MakeRoundedRectangularPath(GraphicsPath path, RectangleF rect, SizeF corner, int spots)
		{
			if (corner.Width > rect.Width / 2f)
			{
				corner.Width = rect.Width / 2f;
			}
			if (corner.Height > rect.Height / 2f)
			{
				corner.Height = rect.Height / 2f;
			}
			float num = corner.Width * 2f;
			float num2 = corner.Height * 2f;
			if (num < 0.01f || num2 < 0.01f || spots == 0)
			{
				path.AddRectangle(rect);
			}
			else
			{
				float x = rect.X;
				float y = rect.Y;
				float num3 = x + num;
				float num4 = y + num2;
				float num5 = x + rect.Width - num;
				float num6 = y + rect.Height - num2;
				float num7 = x + rect.Width;
				float num8 = y + rect.Height;
				bool flag = (spots & 4) != 0;
				bool flag2 = (spots & 8) != 0;
				bool flag3 = (spots & 0x10) != 0;
				bool flag4 = (spots & 2) != 0;
				if (flag)
				{
					path.AddArc(num5, y, num, num2, 270f, 90f);
				}
				if (!flag || !flag2 || num4 < num6)
				{
					path.AddLine(num7, flag ? num4 : y, num7, flag2 ? num6 : num8);
				}
				if (flag2)
				{
					path.AddArc(num5, num6, num, num2, 0f, 90f);
				}
				if (!flag2 || !flag3 || num3 < num5)
				{
					path.AddLine(flag2 ? num5 : num7, num8, flag3 ? num3 : x, num8);
				}
				if (flag3)
				{
					path.AddArc(x, num6, num, num2, 90f, 90f);
				}
				if (!flag3 || !flag4 || num4 < num6)
				{
					path.AddLine(x, flag3 ? num6 : num8, x, flag4 ? num4 : y);
				}
				if (flag4)
				{
					path.AddArc(x, y, num, num2, 180f, 90f);
				}
			}
			path.CloseAllFigures();
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
			case 1421:
				Corner = e.GetSize(undo);
				break;
			case 1422:
				RoundedCornerSpots = e.GetInt(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
