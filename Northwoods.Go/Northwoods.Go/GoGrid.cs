using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object that draws a set of dots, crosses, or lines in a rectangular pattern,
	/// and that can affect how objects are moved, copied, and resized.
	/// </summary>
	/// <remarks>
	/// A <c>GoGrid</c> is not a table containing cells of objects.
	/// This class just draws lines in a regular manner within the bounds of
	/// this object according to a frequency specified by the <see cref="P:Northwoods.Go.GoGrid.CellSize" />.
	/// The appearance of the lines is governed by the <see cref="P:Northwoods.Go.GoGrid.Style" />
	/// and <see cref="P:Northwoods.Go.GoGrid.LineWidth" /> and <see cref="P:Northwoods.Go.GoGrid.LineColor" />.
	/// You can also specify a regular pattern of colors by specifying <see cref="P:Northwoods.Go.GoGrid.CellColors" />.
	/// Furthermore a grid can have a semi-infinite or infinite extent by
	/// setting <see cref="P:Northwoods.Go.GoGrid.UnboundedSpots" />.
	/// This class also implements the <see cref="T:Northwoods.Go.IGoDragSnapper" /> interface,
	/// which affects how various view methods and tools move, copy, and resize
	/// objects.
	/// </remarks>
	[Serializable]
	public class GoGrid : GoRectangle, IGoDragSnapper
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.Style" /> property.
		/// </summary>
		public const int ChangedStyle = 1801;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.Origin" /> property.
		/// </summary>
		public const int ChangedOrigin = 1802;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.OriginRelative" /> property.
		/// </summary>
		public const int ChangedOriginRelative = 1803;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.CellSize" /> property.
		/// </summary>
		public const int ChangedCellSize = 1804;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.LineColor" /> property.
		/// </summary>
		public const int ChangedLineColor = 1805;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.LineWidth" /> property.
		/// </summary>
		public const int ChangedLineWidth = 1806;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.LineDashStyle" /> property.
		/// </summary>
		public const int ChangedLineDashStyle = 1807;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.SnapDrag" /> property.
		/// </summary>
		public const int ChangedSnapDrag = 1808;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.SnapResize" /> property.
		/// </summary>
		public const int ChangedSnapResize = 1809;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.CellColors" /> property.
		/// </summary>
		public const int ChangedCellColors = 1810;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.UnboundedSpots" /> property.
		/// </summary>
		public const int ChangedUnboundedSpots = 1811;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.SnapDragWhole" /> property.
		/// </summary>
		public const int ChangedSnapDragWhole = 1812;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.SnapOpaque" /> property.
		/// </summary>
		public const int ChangedSnapOpaque = 1814;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.SnapCellSpot" /> property.
		/// </summary>
		public const int ChangedSnapCellSpot = 1815;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.MajorLineColor" /> property.
		/// </summary>
		public const int ChangedMajorLineColor = 1816;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.MajorLineWidth" /> property.
		/// </summary>
		public const int ChangedMajorLineWidth = 1817;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.MajorLineDashStyle" /> property.
		/// </summary>
		public const int ChangedMajorLineDashStyle = 1818;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.MajorLineFrequency" /> property.
		/// </summary>
		public const int ChangedMajorLineFrequency = 1819;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.LineDashPattern" /> property.
		/// </summary>
		public const int ChangedLineDashPattern = 1820;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.MajorLineDashPattern" /> property.
		/// </summary>
		public const int ChangedMajorLineDashPattern = 1821;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGrid.PaintMinorScale" /> property.
		/// </summary>
		public const int ChangedPaintMinorScale = 1822;

		private const int flagOriginRelative = 1048576;

		private const int flagSnapOpaque = 2097152;

		private const int flagSnapDragWhole = 4194304;

		internal static readonly Color[,] DefaultCellColors = new Color[0, 0];

		internal static readonly float[] DefaultLineDashPattern = new float[2]
		{
			4f,
			4f
		};

		internal static readonly float[] DefaultMajorLineDashPattern = new float[2]
		{
			4f,
			4f
		};

		private GoViewGridStyle myStyle;

		private PointF myOrigin;

		private SizeF myCellSize = new SizeF(50f, 50f);

		private int myUnboundedSpots;

		private Color myLineColor = Color.LightGray;

		private Color myMajorLineColor = Color.Gray;

		private Size myMajorLineFrequency;

		private Color[,] myCellColors = DefaultCellColors;

		private float myLineWidth;

		private DashStyle myLineDashStyle;

		private float[] myLineDashPattern = DefaultLineDashPattern;

		private float myMajorLineWidth;

		private DashStyle myMajorLineDashStyle;

		private float[] myMajorLineDashPattern = DefaultMajorLineDashPattern;

		private GoViewSnapStyle mySnapDrag;

		private GoViewSnapStyle mySnapResize;

		private int mySnapCellSpot = 2;

		private float myPaintMinorScale;

		/// <summary>
		/// Gets a <c>RectangleF</c> that simulates the bounds of extent of this grid,
		/// using large numbers for the semi-infinite or infinite positions and/or sizes.
		/// </summary>
		/// <value>
		/// when <see cref="P:Northwoods.Go.GoGrid.IsUnbounded" /> is false, this is the same as <see cref="P:Northwoods.Go.GoObject.Bounds" />;
		/// otherwise this inflates the bounds by large values according to the value of <see cref="P:Northwoods.Go.GoGrid.UnboundedSpots" />.
		/// </value>
		[Browsable(false)]
		public RectangleF Extent => ComputeInfiniteBounds(Bounds);

		/// <summary>
		/// Gets or sets the style of the grid.
		/// </summary>
		/// <value>
		/// This <see cref="T:Northwoods.Go.GoViewGridStyle" /> value defaults to <see cref="F:Northwoods.Go.GoViewGridStyle.None" />.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Origin" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.CellSize" />
		/// <seealso cref="T:System.Drawing.Color" />
		[Category("Grid")]
		[DefaultValue(GoViewGridStyle.None)]
		[Description("The appearance style of the grid.")]
		public virtual GoViewGridStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				GoViewGridStyle goViewGridStyle = myStyle;
				if (goViewGridStyle != value)
				{
					myStyle = value;
					Changed(1801, (int)goViewGridStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the origin for the grid.
		/// </summary>
		/// <value>
		/// This <c>PointF</c> value is a document coordinate point.
		/// The default value is (0, 0).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.OriginRelative" />
		[Category("Grid")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The origin for the grid.")]
		public virtual PointF Origin
		{
			get
			{
				PointF result = myOrigin;
				if (OriginRelative)
				{
					result.X += base.Left;
					result.Y += base.Top;
				}
				return result;
			}
			set
			{
				PointF pointF = value;
				if (OriginRelative)
				{
					pointF.X -= base.Left;
					pointF.Y -= base.Top;
				}
				PointF pointF2 = myOrigin;
				if (pointF2 != pointF)
				{
					myOrigin = pointF;
					Changed(1802, 0, null, GoObject.MakeRect(pointF2), 0, null, GoObject.MakeRect(pointF));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the value of <see cref="P:Northwoods.Go.GoGrid.Origin" /> moves along with this grid's Position.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		[Category("Grid")]
		[DefaultValue(true)]
		[Description("Whether the Origin moves along with the GoGrid's Position")]
		public virtual bool OriginRelative
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
					Changed(1803, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of each cell in the grid.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value describes the size of each cell in document coordinates.
		/// The <c>Width</c> and <c>Height</c> must be positive.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		[Category("Grid")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The size of each cell in the grid.")]
		public virtual SizeF CellSize
		{
			get
			{
				return myCellSize;
			}
			set
			{
				SizeF sizeF = myCellSize;
				if (sizeF != value)
				{
					if (value.Width <= 0f || value.Height <= 0f)
					{
						throw new ArgumentException("New SizeF value for GoGrid.CellSize must have positive dimensions");
					}
					myCellSize = value;
					Changed(1804, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the directions in which this grid is effectively unlimited in size and extent.
		/// </summary>
		/// <value>
		/// This must be some bitwise OR of the standard eight side-or-corner spots:
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="F:Northwoods.Go.GoObject.TopLeft" />, <see cref="F:Northwoods.Go.GoObject.MiddleTop" />, <see cref="F:Northwoods.Go.GoObject.TopRight" />,
		/// <see cref="F:Northwoods.Go.GoObject.MiddleLeft" />, <see cref="F:Northwoods.Go.GoObject.MiddleRight" />,
		/// <see cref="F:Northwoods.Go.GoObject.BottomLeft" />, <see cref="F:Northwoods.Go.GoObject.MiddleBottom" />, <see cref="F:Northwoods.Go.GoObject.BottomRight" />.
		/// The default value is <see cref="F:Northwoods.Go.GoObject.NoSpot" />; that is, this grid object is completely finite,
		/// like the typical <see cref="T:Northwoods.Go.GoObject" />.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoGrid.IsUnbounded" />
		/// <seealso cref="M:Northwoods.Go.GoGrid.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoGrid.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />
		[Category("Grid")]
		[DefaultValue(0)]
		[Description("The directions in which the grid is infinite, a bitwise-OR of the standard eight GoObject spots")]
		public virtual int UnboundedSpots
		{
			get
			{
				return myUnboundedSpots;
			}
			set
			{
				int num = myUnboundedSpots;
				if (num != value)
				{
					myUnboundedSpots = value;
					Changed(1811, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (base.Parent != null)
					{
						base.Parent.InvalidatePaintBounds();
					}
					if (base.Document != null)
					{
						base.Document.InvalidateViews();
					}
					else if (base.View != null)
					{
						base.View.UpdateView();
					}
				}
			}
		}

		/// <summary>
		/// This is true when <see cref="P:Northwoods.Go.GoGrid.UnboundedSpots" /> includes at least one of the standard
		/// eight spots at the sides or corners.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoGrid.UnboundedSpots" />
		[Category("Grid")]
		public bool IsUnbounded => (UnboundedSpots & 0x1FE) != 0;

		/// <summary>
		/// Gets or sets the color used in drawing grid markings.
		/// </summary>
		/// <value>
		/// The color defaults to a light gray.
		/// </value>
		/// <remarks>
		/// You may prefer a darker color for a Dot grid style,
		/// or a lighter color for a Line grid style.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		[Category("Grid")]
		[Description("The color used in drawing the grid lines.")]
		public virtual Color LineColor
		{
			get
			{
				return myLineColor;
			}
			set
			{
				Color color = myLineColor;
				if (color != value)
				{
					myLineColor = value;
					Changed(1805, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color used in drawing grid markings.
		/// </summary>
		/// <value>
		/// The color defaults to a gray.
		/// </value>
		/// <remarks>
		/// You may prefer a darker color for a Dot grid style,
		/// or a lighter color for a Line grid style.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		[Category("Grid")]
		[Description("The color used in drawing the grid lines.")]
		public virtual Color MajorLineColor
		{
			get
			{
				return myMajorLineColor;
			}
			set
			{
				Color color = myMajorLineColor;
				if (color != value)
				{
					myMajorLineColor = value;
					Changed(1816, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of grid markings.
		/// </summary>
		/// <value>
		/// The width defaults to 0.
		/// </value>
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The width of the pen used in drawing the grid lines.")]
		public virtual float LineWidth
		{
			get
			{
				return myLineWidth;
			}
			set
			{
				float num = myLineWidth;
				if (num != value)
				{
					myLineWidth = value;
					Changed(1806, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the dash style for pens used in drawing the grid lines.
		/// </summary>
		/// <value>
		/// The dash style defaults to solid.
		/// </value>
		/// <remarks>
		/// This property is only used when the <see cref="P:Northwoods.Go.GoGrid.Style" /> is
		/// <see cref="F:Northwoods.Go.GoViewGridStyle.Line" />, <see cref="F:Northwoods.Go.GoViewGridStyle.HorizontalLine" />, or
		/// <see cref="F:Northwoods.Go.GoViewGridStyle.VerticalLine" />.
		/// </remarks>
		[Category("Grid")]
		[DefaultValue(DashStyle.Solid)]
		[Description("The pen dash style used in drawing the grid lines.")]
		public virtual DashStyle LineDashStyle
		{
			get
			{
				return myLineDashStyle;
			}
			set
			{
				DashStyle dashStyle = myLineDashStyle;
				if (dashStyle != value)
				{
					myLineDashStyle = value;
					Changed(1807, (int)dashStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dash pattern drawn by grid lines,
		/// when <see cref="P:Northwoods.Go.GoGrid.LineDashStyle" /> is <c>DashStyle.Custom</c>.
		/// </summary>
		/// <value>
		/// An array of singles floats describing the length of each (alternating)
		/// drawn and empty section, repeated to form the whole dashed line.
		/// Do not modify the array returned by this property;
		/// to change the pattern, you must set this property to a newly allocated array.
		/// </value>
		[Category("Grid")]
		[Description("The pattern of dashes used in drawing the grid lines, when the LineDashStyle is DashStyle.Custom")]
		public virtual float[] LineDashPattern
		{
			get
			{
				return myLineDashPattern;
			}
			set
			{
				float[] array = myLineDashPattern;
				if (value != null && value.Length >= 2 && !array.Equals(value))
				{
					myLineDashPattern = (float[])value.Clone();
					Changed(1820, 0, array, GoObject.NullRect, 0, value.Clone(), GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of grid markings.
		/// </summary>
		/// <value>
		/// The width defaults to 0.
		/// </value>
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The width of the pen used in drawing the grid lines.")]
		public virtual float MajorLineWidth
		{
			get
			{
				return myMajorLineWidth;
			}
			set
			{
				float num = myMajorLineWidth;
				if (num != value)
				{
					myMajorLineWidth = value;
					Changed(1817, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the dash style for pens used in drawing the grid lines.
		/// </summary>
		/// <value>
		/// The dash style defaults to solid.
		/// </value>
		/// <remarks>
		/// This property is only used when the <see cref="P:Northwoods.Go.GoGrid.Style" /> is
		/// <see cref="F:Northwoods.Go.GoViewGridStyle.Line" />, <see cref="F:Northwoods.Go.GoViewGridStyle.HorizontalLine" />, or
		/// <see cref="F:Northwoods.Go.GoViewGridStyle.VerticalLine" />.
		/// </remarks>
		[Category("Grid")]
		[DefaultValue(DashStyle.Solid)]
		[Description("The pen dash style used in drawing the grid lines.")]
		public virtual DashStyle MajorLineDashStyle
		{
			get
			{
				return myMajorLineDashStyle;
			}
			set
			{
				DashStyle dashStyle = myMajorLineDashStyle;
				if (dashStyle != value)
				{
					myMajorLineDashStyle = value;
					Changed(1818, (int)dashStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dash pattern drawn by grid lines,
		/// when <see cref="P:Northwoods.Go.GoGrid.MajorLineDashStyle" /> is <c>DashStyle.Custom</c>.
		/// </summary>
		/// <value>
		/// An array of singles floats describing the length of each (alternating)
		/// drawn and empty section, repeated to form the whole dashed line.
		/// Do not modify the array returned by this property;
		/// to change the pattern, you must set this property to a newly allocated array.
		/// </value>
		[Category("Grid")]
		[Description("The pattern of dashes used in drawing the grid lines, when the MajorLineDashStyle is DashStyle.Custom")]
		public virtual float[] MajorLineDashPattern
		{
			get
			{
				return myMajorLineDashPattern;
			}
			set
			{
				float[] array = myMajorLineDashPattern;
				if (value != null && value.Length >= 2 && !array.Equals(value))
				{
					myMajorLineDashPattern = (float[])value.Clone();
					Changed(1821, 0, array, GoObject.NullRect, 0, value.Clone(), GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets how often grid lines should be drawn as major lines.
		/// </summary>
		/// <value>
		/// The Width indicates how often vertical lines should be drawn using
		/// <see cref="P:Northwoods.Go.GoGrid.MajorLineColor" /> and MajorLineWidth and MajorLineDashStyle.
		/// The Height indicates the same thing for horizontal lines.
		/// A value of zero means never; one means always; two means every other one, etc.
		/// The initial value is 0x0, so major lines are never drawn.
		/// </value>
		/// <remarks>
		/// Typically this will have the same values as the bounds of the <see cref="P:Northwoods.Go.GoGrid.CellColors" />
		/// array, or integral multiples of each other, if both are in use.
		/// </remarks>
		[Category("Grid")]
		[Description("How often major lines should be drawn; zero means never.")]
		public virtual Size MajorLineFrequency
		{
			get
			{
				return myMajorLineFrequency;
			}
			set
			{
				Size size = myMajorLineFrequency;
				if (size != value)
				{
					myMajorLineFrequency = value;
					Changed(1819, 0, size, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the scale at which the Minor Grid markings are painted.
		/// </summary>
		/// <value>
		/// This value defaults to <c>0.0f</c>.
		/// </value>
		/// <remarks>
		/// Painting thousands of marks for grids can become a performance
		/// issue when zoomed to a small scale.  Setting this value will allow
		/// the minor grid marks to disappear below that scale value.  Major
		/// grid marks will remain.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.PaintGreekScale" />
		/// <seealso cref="P:Northwoods.Go.GoView.PaintNothingScale" />
		[Category("Appearance")]
		[DefaultValue(0f)]
		[Description("The scale at which greeked objects paint something simple.")]
		public virtual float PaintMinorScale
		{
			get
			{
				return myPaintMinorScale;
			}
			set
			{
				float num = myPaintMinorScale;
				if (num != value)
				{
					myPaintMinorScale = value;
					Changed(1822, 0, num, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets an array of colors to be used in drawing grid cell backgrounds.
		/// </summary>
		/// <value>
		/// A two-dimensional array of <c>Color</c>s.
		/// You must not modify the array that is returned by this property.
		/// To change any cell colors you must set this property, either by
		/// allocating a new array or by cloning the current value, modifying the copy,
		/// and then setting this property.
		/// When set the grid will keep a copy of the array.
		/// Initially the array has zero-by-zero bounds, so there is no cell color.
		/// </value>
		/// <remarks>
		/// This array is used by <see cref="M:Northwoods.Go.GoGrid.FillGrid(System.Drawing.Graphics,Northwoods.Go.GoView,System.Drawing.RectangleF)" />.
		/// Typically this array's bounds will have the same values as <see cref="P:Northwoods.Go.GoGrid.MajorLineFrequency" />,
		/// or integral multiples of each other, if both are in use.
		/// </remarks>
		[Category("Grid")]
		[Description("The colors used in drawing the cell backgrounds.")]
		public virtual Color[,] CellColors
		{
			get
			{
				return myCellColors;
			}
			set
			{
				Color[,] array = myCellColors;
				if (value != null && !array.Equals(value))
				{
					myCellColors = (Color[,])value.Clone();
					Changed(1810, 0, array, GoObject.NullRect, 0, myCellColors.Clone(), GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the interactive dragging behavior for positioning objects.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoViewSnapStyle.None" />, so moving and copying
		/// objects is very smooth, to any point.
		/// </value>
		/// <remarks>
		/// This is used by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.SnapResize" />
		[Category("Grid")]
		[DefaultValue(GoViewSnapStyle.None)]
		[Description("The interactive dragging behavior for positioning objects.")]
		public virtual GoViewSnapStyle SnapDrag
		{
			get
			{
				return mySnapDrag;
			}
			set
			{
				GoViewSnapStyle goViewSnapStyle = mySnapDrag;
				if (goViewSnapStyle != value)
				{
					mySnapDrag = value;
					Changed(1808, (int)goViewSnapStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the interactive resizing behavior for resizing objects.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoViewSnapStyle.None" />, so resizing
		/// objects is very smooth, to any size.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoGrid.Style" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.SnapDrag" />
		[Category("Grid")]
		[DefaultValue(GoViewSnapStyle.None)]
		[Description("The interactive resizing behavior for resizing objects.")]
		public virtual GoViewSnapStyle SnapResize
		{
			get
			{
				return mySnapResize;
			}
			set
			{
				GoViewSnapStyle goViewSnapStyle = mySnapResize;
				if (goViewSnapStyle != value)
				{
					mySnapResize = value;
					Changed(1809, (int)goViewSnapStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />
		/// should try to fit the whole object being dragged within the grid.
		/// </summary>
		[Category("Grid")]
		[DefaultValue(true)]
		[Description("Whether GoView.SnapPoint should try to fit the whole object within the grid.")]
		public virtual bool SnapDragWhole
		{
			get
			{
				return (base.InternalFlags & 0x400000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x400000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 4194304;
					}
					else
					{
						base.InternalFlags &= -4194305;
					}
					Changed(1812, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />
		/// should look at any grid that might be behind this one.
		/// </summary>
		[Category("Grid")]
		[DefaultValue(true)]
		[Description("Whether GoView.SnapPoint should look at any grid that might be behind this one.")]
		public virtual bool SnapOpaque
		{
			get
			{
				return (base.InternalFlags & 0x200000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x200000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 2097152;
					}
					else
					{
						base.InternalFlags &= -2097153;
					}
					Changed(1814, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the spot in the cell that is returned by <see cref="M:Northwoods.Go.GoGrid.FindNearestGridPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <value>The default value is <see cref="T:Northwoods.Go.GoObject" />.<see cref="F:Northwoods.Go.GoObject.TopLeft" /></value>
		/// <remarks>
		/// The most common values are <see cref="F:Northwoods.Go.GoObject.TopLeft" /> and <see cref="F:Northwoods.Go.GoObject.Middle" />,
		/// corresponding to the most common spots used to control the <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Location" />.
		/// The values <see cref="F:Northwoods.Go.GoObject.MiddleLeft" /> and <see cref="F:Northwoods.Go.GoObject.MiddleTop" /> are also useful.
		/// Values such as <see cref="F:Northwoods.Go.GoObject.BottomRight" /> are effectively the same as <see cref="F:Northwoods.Go.GoObject.TopLeft" />
		/// in the adjacent cell,
		/// just as <see cref="F:Northwoods.Go.GoObject.MiddleRight" /> is the same as <see cref="F:Northwoods.Go.GoObject.MiddleLeft" />
		/// in a different adjacent cell.
		/// </remarks>
		[Category("Grid")]
		[DefaultValue(2)]
		[Description("Which cell spot should be returned by GoGrid.FindNearestGridPoint.")]
		public virtual int SnapCellSpot
		{
			get
			{
				return mySnapCellSpot;
			}
			set
			{
				int num = mySnapCellSpot;
				if (num != value)
				{
					mySnapCellSpot = value;
					Changed(1815, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The default grid of size 100x100 does not draw anything, because it has
		/// no <see cref="P:Northwoods.Go.GoShape.Pen" /> or <see cref="P:Northwoods.Go.GoShape.Brush" /> and the
		/// <see cref="P:Northwoods.Go.GoGrid.Style" /> is <see cref="F:Northwoods.Go.GoViewGridStyle.None" />.
		/// </summary>
		public GoGrid()
		{
			base.InternalFlags |= 7340032;
			base.Size = new SizeF(100f, 100f);
			Pen = null;
			Brush = null;
		}

		/// <summary>
		/// Draw the grid, if the <see cref="P:Northwoods.Go.GoGrid.Style" /> is not <see cref="F:Northwoods.Go.GoViewGridStyle.None" />.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// Note that if <see cref="P:Northwoods.Go.GoGrid.UnboundedSpots" /> specifies any corner or side spots,
		/// <see cref="P:Northwoods.Go.GoGrid.IsUnbounded" /> will be true, and this grid will paint arbitrarily
		/// far in the direction(s) specified.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			RectangleF clipBounds = g.ClipBounds;
			RectangleF infRect = ComputeInfiniteBounds(Bounds);
			RectangleF a = ComputeFiniteBounds(infRect, view.DocExtent);
			Brush brush = Brush;
			if (Shadowed && brush != null)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				float width = shadowOffset.Width;
				float height = shadowOffset.Height;
				if (shadowOffset.Width != 0f || shadowOffset.Height != 0f)
				{
					float x = a.X;
					float y = a.Y;
					float num = a.X + a.Width;
					float num2 = a.Y + a.Height;
					PointF pointF = new PointF(x, y);
					PointF pointF2 = new PointF(num, y);
					PointF pointF3 = new PointF(num, num2);
					PointF pointF4 = new PointF(x, num2);
					PointF pointF5 = new PointF(x + width, y + height);
					PointF pointF6 = new PointF(num + width, y + height);
					PointF pointF7 = new PointF(num + width, num2 + height);
					PointF pointF8 = new PointF(x + width, num2 + height);
					Brush shadowBrush = GetShadowBrush(view);
					if (width > 0f)
					{
						if (height > 0f)
						{
							PointF[] array = view.AllocTempPointArray(6);
							array[0] = pointF6;
							array[1] = pointF7;
							array[2] = pointF8;
							array[3] = new PointF(x + width, num2);
							array[4] = pointF3;
							array[5] = new PointF(num, y + height);
							GoShape.DrawPolygon(g, view, null, shadowBrush, array);
							view.FreeTempPointArray(array);
						}
						else if (height < 0f)
						{
							PointF[] array2 = view.AllocTempPointArray(6);
							array2[0] = pointF5;
							array2[1] = pointF6;
							array2[2] = pointF7;
							array2[3] = new PointF(num, num2 + height);
							array2[4] = pointF2;
							array2[5] = new PointF(x + width, y);
							GoShape.DrawPolygon(g, view, null, shadowBrush, array2);
							view.FreeTempPointArray(array2);
						}
						else
						{
							GoShape.DrawRectangle(g, view, null, shadowBrush, pointF2.X, pointF2.Y, width, a.Height);
						}
					}
					else if (width < 0f)
					{
						if (height > 0f)
						{
							PointF[] array3 = view.AllocTempPointArray(6);
							array3[0] = pointF7;
							array3[1] = pointF8;
							array3[2] = pointF5;
							array3[3] = new PointF(x, y + height);
							array3[4] = pointF4;
							array3[5] = new PointF(num + width, num2);
							GoShape.DrawPolygon(g, view, null, shadowBrush, array3);
							view.FreeTempPointArray(array3);
						}
						else if (height < 0f)
						{
							PointF[] array4 = view.AllocTempPointArray(6);
							array4[0] = pointF8;
							array4[1] = pointF5;
							array4[2] = pointF6;
							array4[3] = new PointF(num + width, y);
							array4[4] = pointF;
							array4[5] = new PointF(x, num2 + height);
							GoShape.DrawPolygon(g, view, null, shadowBrush, array4);
							view.FreeTempPointArray(array4);
						}
						else
						{
							GoShape.DrawRectangle(g, view, null, shadowBrush, pointF5.X, pointF5.Y, 0f - width, a.Height);
						}
					}
					else if (height > 0f)
					{
						GoShape.DrawRectangle(g, view, null, shadowBrush, pointF4.X, pointF4.Y, a.Width, height);
					}
					else if (height < 0f)
					{
						GoShape.DrawRectangle(g, view, null, shadowBrush, pointF5.X, pointF5.Y, a.Width, 0f - height);
					}
				}
			}
			if (brush != null)
			{
				GoShape.DrawRectangle(g, view, null, brush, a.X, a.Y, a.Width, a.Height);
			}
			RectangleF rectangleF = GoObject.IntersectionRect(a, clipBounds);
			if (rectangleF.Width > 0f && rectangleF.Height > 0f)
			{
				Region clip = g.Clip;
				g.IntersectClip(rectangleF);
				FillGrid(g, view, rectangleF);
				switch (Style)
				{
				case GoViewGridStyle.Line:
				case GoViewGridStyle.HorizontalLine:
				case GoViewGridStyle.VerticalLine:
					DrawGridLines(g, view, rectangleF);
					break;
				case GoViewGridStyle.Dot:
					DrawGridDots(g, view, rectangleF);
					break;
				case GoViewGridStyle.Cross:
					DrawGridCrosses(g, view, new SizeF(6f, 6f), rectangleF);
					break;
				}
				g.Clip = clip;
			}
			if (Pen != null)
			{
				GoShape.DrawRectangle(g, view, Pen, null, a.X, a.Y, a.Width, a.Height);
			}
		}

		/// <summary>
		/// If this grid is the <see cref="P:Northwoods.Go.GoSheet.Paper" /> of a <see cref="T:Northwoods.Go.GoSheet" />,
		/// let it paint the sheet's shadow.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public override Brush GetShadowBrush(GoView view)
		{
			GoSheet goSheet = base.Parent as GoSheet;
			if (goSheet != null && goSheet.Paper == this)
			{
				return goSheet.GetShadowBrush(view);
			}
			return base.GetShadowBrush(view);
		}

		private RectangleF ComputeFiniteBounds(RectangleF infRect, RectangleF viewRect)
		{
			RectangleF a = viewRect;
			GoObject.InflateRect(ref a, 100f, 100f);
			return GoObject.IntersectionRect(infRect, a);
		}

		private RectangleF ComputeInfiniteBounds(RectangleF rect)
		{
			if (!IsUnbounded)
			{
				return rect;
			}
			float num = 9.999E+07f;
			int unboundedSpots = UnboundedSpots;
			if ((unboundedSpots & 0x112) != 0)
			{
				rect.X = rect.X + rect.Width - num;
				rect.Width = num;
			}
			if ((unboundedSpots & 0x26) != 0)
			{
				rect.Y = rect.Y + rect.Height - num;
				rect.Height = num;
			}
			if ((unboundedSpots & 0x4C) != 0)
			{
				rect.Width += num;
			}
			if ((unboundedSpots & 0x98) != 0)
			{
				rect.Height += num;
			}
			return rect;
		}

		/// <summary>
		/// The paint bounds are effectively unbounded if <see cref="P:Northwoods.Go.GoGrid.IsUnbounded" /> is true.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns>
		/// If <see cref="P:Northwoods.Go.GoGrid.IsUnbounded" /> is true,
		/// a <c>RectangleF</c> with a very large <c>Width</c> and/or <c>Height</c>;
		/// otherwise the standard expanded rectangle as for any <see cref="T:Northwoods.Go.GoShape" />
		/// accounting for any pen and/or shadow.
		/// </returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			RectangleF rectangleF = ComputeInfiniteBounds(rect);
			RectangleF a = rectangleF;
			if (view != null)
			{
				a = ComputeFiniteBounds(rectangleF, view.DocExtent);
			}
			float num = Math.Max(PenWidth, 1f);
			GoObject.InflateRect(ref a, num, num);
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (shadowOffset.Width < 0f)
				{
					a.X += shadowOffset.Width;
					a.Width -= shadowOffset.Width;
				}
				else
				{
					a.Width += shadowOffset.Width;
				}
				if (shadowOffset.Height < 0f)
				{
					a.Y += shadowOffset.Height;
					a.Height -= shadowOffset.Height;
				}
				else
				{
					a.Height += shadowOffset.Height;
				}
			}
			return a;
		}

		/// <summary>
		/// Fill the cells of the grid according to the colors given by <see cref="P:Northwoods.Go.GoGrid.CellColors" />.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="clipRect">
		/// the area in document coordinates that needs to be painted;
		/// this is used to optimize the drawing by not trying to draw anything that is
		/// clearly outside of this clipping rectangle
		/// </param>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoGrid.CellColors" /> has either zero width or zero height,
		/// this draws no cell colors.
		/// A color that is <c>Color.Empty</c> draws nothing for that cell.
		/// </remarks>
		protected virtual void FillGrid(Graphics g, GoView view, RectangleF clipRect)
		{
			Color[,] cellColors = CellColors;
			int num;
			int num2;
			float width;
			float height;
			PointF pointF;
			PointF pointF2;
			checked
			{
				num = cellColors.GetUpperBound(0) + 1;
				num2 = cellColors.GetUpperBound(1) + 1;
				if (num <= 0 || num2 <= 0 || (num == 1 && num2 == 1 && cellColors[0, 0] == Color.Empty))
				{
					return;
				}
				width = CellSize.Width;
				height = CellSize.Height;
				float x = clipRect.X - width;
				float y = clipRect.Y - height;
				float x2 = clipRect.X + clipRect.Width + width;
				float y2 = clipRect.Y + clipRect.Height + height;
				pointF = FindNearestInfiniteGridPoint(new PointF(x, y), 0f, 0f);
				pointF2 = FindNearestInfiniteGridPoint(new PointF(x2, y2), 0f, 0f);
			}
			int num3 = checked((int)Math.Floor((pointF.X - Origin.X) / width)) % num;
			int num4 = checked((int)Math.Floor((pointF.Y - Origin.Y) / height)) % num2;
			checked
			{
				if (num3 < 0)
				{
					num3 += num;
				}
				if (num4 < 0)
				{
					num4 += num2;
				}
				int num5 = num3;
				SolidBrush solidBrush = new SolidBrush(Color.White);
				for (float num6 = pointF.Y; num6 < pointF2.Y; num6 += height)
				{
					for (float num7 = pointF.X; num7 < pointF2.X; num7 += width)
					{
						Color color = cellColors[num3, num4];
						if (color != Color.Empty)
						{
							solidBrush.Color = color;
							GoShape.DrawRectangle(g, view, null, solidBrush, num7, num6, width, height);
						}
						num3++;
						if (num3 >= num)
						{
							num3 = 0;
						}
					}
					num3 = num5;
					num4++;
					if (num4 >= num2)
					{
						num4 = 0;
					}
				}
				solidBrush.Dispose();
			}
		}

		/// <summary>
		/// Draw continuous lines for the grids.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="clipRect">
		/// the area in document coordinates that needs to be painted;
		/// this is used to optimize the drawing by not trying to draw anything that is
		/// clearly outside of this clipping rectangle
		/// </param>
		protected virtual void DrawGridLines(Graphics g, GoView view, RectangleF clipRect)
		{
			float width = CellSize.Width;
			float height = CellSize.Height;
			Color lineColor = LineColor;
			Color majorLineColor = MajorLineColor;
			Pen pen = new Pen(lineColor, LineWidth);
			if (LineDashStyle == DashStyle.Custom)
			{
				pen.DashPattern = LineDashPattern;
			}
			pen.DashStyle = LineDashStyle;
			Pen pen2 = new Pen(majorLineColor, MajorLineWidth);
			if (MajorLineDashStyle == DashStyle.Custom)
			{
				pen2.DashPattern = MajorLineDashPattern;
			}
			pen2.DashStyle = MajorLineDashStyle;
			if (pen.DashStyle != 0 || pen2.DashStyle != 0)
			{
				clipRect = GoObject.UnionRect(clipRect, view.DocPosition);
			}
			float x = clipRect.X - width;
			float y = clipRect.Y - height;
			float x2 = clipRect.X + clipRect.Width + width;
			float y2 = clipRect.Y + clipRect.Height + height;
			PointF pointF = FindNearestInfiniteGridPoint(new PointF(x, y), 0f, 0f);
			PointF pointF2 = FindNearestInfiniteGridPoint(new PointF(x2, y2), 0f, 0f);
			if (Style != GoViewGridStyle.HorizontalLine)
			{
				int width2 = MajorLineFrequency.Width;
				if (width2 > 0)
				{
					int num = checked((int)Math.Floor((pointF.X - Origin.X) / width)) % width2;
					checked
					{
						if (num < 0)
						{
							num += width2;
						}
						for (float num2 = pointF.X; num2 < pointF2.X; num2 += width)
						{
							Pen pen3 = pen;
							if (num == 0)
							{
								pen3 = pen2;
							}
							if (view.DocScale > myPaintMinorScale || num == 0)
							{
								GoShape.DrawLine(g, view, pen3, num2, pointF.Y, num2, pointF2.Y);
							}
							num++;
							if (num >= width2)
							{
								num = 0;
							}
						}
					}
				}
				else if (view.DocScale > myPaintMinorScale)
				{
					for (float num3 = pointF.X; num3 < pointF2.X; num3 += width)
					{
						GoShape.DrawLine(g, view, pen, num3, pointF.Y, num3, pointF2.Y);
					}
				}
			}
			if (Style != GoViewGridStyle.VerticalLine)
			{
				int height2 = MajorLineFrequency.Height;
				if (height2 > 0)
				{
					int num4 = checked((int)Math.Floor((pointF.Y - Origin.Y) / height)) % height2;
					checked
					{
						if (num4 < 0)
						{
							num4 += height2;
						}
						for (float num5 = pointF.Y; num5 < pointF2.Y; num5 += height)
						{
							Pen pen4 = pen;
							if (num4 == 0)
							{
								pen4 = pen2;
							}
							if (view.DocScale > myPaintMinorScale || num4 == 0)
							{
								GoShape.DrawLine(g, view, pen4, pointF.X, num5, pointF2.X, num5);
							}
							num4++;
							if (num4 >= height2)
							{
								num4 = 0;
							}
						}
					}
				}
				else if (view.DocScale > myPaintMinorScale)
				{
					for (float num6 = pointF.Y; num6 < pointF2.Y; num6 += height)
					{
						GoShape.DrawLine(g, view, pen, pointF.X, num6, pointF2.X, num6);
					}
				}
			}
			pen.Dispose();
			pen2.Dispose();
		}

		/// <summary>
		/// Draw small dots at the grid points.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="clipRect">
		/// the area in document coordinates that needs to be painted;
		/// this is used to optimize the drawing by not trying to draw anything that is
		/// clearly outside of this clipping rectangle
		/// </param>
		protected virtual void DrawGridDots(Graphics g, GoView view, RectangleF clipRect)
		{
			float width = CellSize.Width;
			float height = CellSize.Height;
			Color lineColor = LineColor;
			Color majorLineColor = MajorLineColor;
			float num = LineWidth;
			Pen pen = new Pen(lineColor, num);
			Pen pen2 = new Pen(majorLineColor, MajorLineWidth);
			float x = clipRect.X - width;
			float y = clipRect.Y - height;
			float x2 = clipRect.X + clipRect.Width + width;
			float y2 = clipRect.Y + clipRect.Height + height;
			PointF pointF = FindNearestInfiniteGridPoint(new PointF(x, y), 0f, 0f);
			PointF pointF2 = FindNearestInfiniteGridPoint(new PointF(x2, y2), 0f, 0f);
			if (num <= 0f && view != null)
			{
				num = 1f / view.DocScale;
			}
			int width2 = MajorLineFrequency.Width;
			int height2 = MajorLineFrequency.Height;
			if (width2 > 0 && height2 > 0)
			{
				int num2 = checked((int)Math.Floor((pointF.X - Origin.X) / width)) % width2;
				int num3 = checked((int)Math.Floor((pointF.Y - Origin.Y) / height)) % height2;
				checked
				{
					if (num2 < 0)
					{
						num2 += width2;
					}
					if (num3 < 0)
					{
						num3 += height2;
					}
					int num4 = num3;
					for (float num5 = pointF.X; num5 < pointF2.X; num5 += width)
					{
						for (float num6 = pointF.Y; num6 < pointF2.Y; num6 += height)
						{
							Pen pen3 = pen;
							if (num2 == 0 && num3 == 0)
							{
								pen3 = pen2;
							}
							if (view.DocScale > myPaintMinorScale || (num2 == 0 && num3 == 0))
							{
								GoShape.DrawLine(g, view, pen3, num5 - num / 2f, num6, num5 + num / 2f, num6);
							}
							num3++;
							if (num3 >= height2)
							{
								num3 = 0;
							}
						}
						num3 = num4;
						num2++;
						if (num2 >= width2)
						{
							num2 = 0;
						}
					}
				}
			}
			else if (view.DocScale > myPaintMinorScale)
			{
				for (float num7 = pointF.X; num7 < pointF2.X; num7 += width)
				{
					for (float num8 = pointF.Y; num8 < pointF2.Y; num8 += height)
					{
						GoShape.DrawLine(g, view, pen, num7 - num / 2f, num8, num7 + num / 2f, num8);
					}
				}
			}
			pen.Dispose();
			pen2.Dispose();
		}

		/// <summary>
		/// Draw small crosses at the grid points.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="cross">how long the lines of each cross should be</param>
		/// <param name="clipRect">
		/// the area in document coordinates that needs to be painted;
		/// this is used to optimize the drawing by not trying to draw anything that is
		/// clearly outside of this clipping rectangle
		/// </param>
		protected virtual void DrawGridCrosses(Graphics g, GoView view, SizeF cross, RectangleF clipRect)
		{
			float width = CellSize.Width;
			float height = CellSize.Height;
			Color lineColor = LineColor;
			Color majorLineColor = MajorLineColor;
			Pen pen = new Pen(lineColor, LineWidth);
			Pen pen2 = new Pen(majorLineColor, MajorLineWidth);
			float x = clipRect.X - width;
			float y = clipRect.Y - height;
			float x2 = clipRect.X + clipRect.Width + width;
			float y2 = clipRect.Y + clipRect.Height + height;
			PointF pointF = FindNearestInfiniteGridPoint(new PointF(x, y), 0f, 0f);
			PointF pointF2 = FindNearestInfiniteGridPoint(new PointF(x2, y2), 0f, 0f);
			int width2 = MajorLineFrequency.Width;
			int height2 = MajorLineFrequency.Height;
			if (width2 > 0 && height2 > 0)
			{
				int num = checked((int)Math.Floor((pointF.X - Origin.X) / width)) % width2;
				int num2 = checked((int)Math.Floor((pointF.Y - Origin.Y) / height)) % height2;
				checked
				{
					if (num < 0)
					{
						num += width2;
					}
					if (num2 < 0)
					{
						num2 += height2;
					}
					int num3 = num2;
					for (float num4 = pointF.X; num4 < pointF2.X; num4 += width)
					{
						for (float num5 = pointF.Y; num5 < pointF2.Y; num5 += height)
						{
							Pen pen3 = pen;
							if (num == 0 && num2 == 0)
							{
								pen3 = pen2;
							}
							bool flag = view.DocScale > myPaintMinorScale || (num == 0 && num2 == 0);
							unchecked
							{
								if (cross.Height > 0f && flag)
								{
									GoShape.DrawLine(g, view, pen3, num4, num5 - cross.Height / 2f, num4, num5 + cross.Height / 2f);
								}
								if (cross.Width > 0f && flag)
								{
									GoShape.DrawLine(g, view, pen3, num4 - cross.Width / 2f, num5, num4 + cross.Width / 2f, num5);
								}
							}
							num2++;
							if (num2 >= height2)
							{
								num2 = 0;
							}
						}
						num2 = num3;
						num++;
						if (num >= width2)
						{
							num = 0;
						}
					}
				}
			}
			else if (view.DocScale > myPaintMinorScale)
			{
				for (float num6 = pointF.X; num6 < pointF2.X; num6 += width)
				{
					for (float num7 = pointF.Y; num7 < pointF2.Y; num7 += height)
					{
						if (cross.Height > 0f)
						{
							GoShape.DrawLine(g, view, pen, num6, num7 - cross.Height / 2f, num6, num7 + cross.Height / 2f);
						}
						if (cross.Width > 0f)
						{
							GoShape.DrawLine(g, view, pen, num6 - cross.Width / 2f, num7, num6 + cross.Width / 2f, num7);
						}
					}
				}
			}
			pen.Dispose();
			pen2.Dispose();
		}

		/// <summary>
		/// This predicate is called by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /> to
		/// decide whether this grid should take part.
		/// </summary>
		/// <param name="p">a <c>PointF</c> in document coordinates</param>
		/// <param name="obj">the object being moved or resized (may be null/nothing)</param>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> in which the snapping is occuring; may be null/nothing</param>
		/// <returns>true if <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />
		/// should call <see cref="T:Northwoods.Go.GoGrid" />.<see cref="M:Northwoods.Go.GoGrid.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /></returns>
		/// <remarks>
		/// This considers the <see cref="P:Northwoods.Go.GoGrid.SnapDrag" /> property
		/// (or the <see cref="P:Northwoods.Go.GoGrid.SnapResize" /> property if resizing).
		/// If <paramref name="view" /> is not null and the current <see cref="P:Northwoods.Go.GoView.Tool" />
		/// is either <see cref="T:Northwoods.Go.GoToolResizing" /> or <see cref="T:Northwoods.Go.GoToolDragging" />,
		/// this method checks whether the <see cref="P:Northwoods.Go.GoView.LastInput" /> point is within the grid;
		/// if the view is null or another type of tool is currently in use,
		/// it checks whether the given point <paramref name="p" /> is within the grid.
		/// This predicate is false if <see cref="M:Northwoods.Go.GoObject.CanView" /> returns false,
		/// but this predicate is not affected by the value of <see cref="P:Northwoods.Go.GoGrid.Style" />
		/// or any other properties that control this grid's appearance.
		/// To avoid confusion in some common cases,
		/// this predicate is also false if the <paramref name="obj" /> is this
		/// grid itself, or if this grid <see cref="M:Northwoods.Go.GoObject.IsChildOf(Northwoods.Go.GoObject)" /> the
		/// <paramref name="obj" />, or if this grid is part of the view's
		/// <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		public virtual bool CanSnapPoint(PointF p, GoObject obj, GoView view)
		{
			if (!CanView())
			{
				return false;
			}
			if (view != null)
			{
				if (obj == this || IsChildOf(obj))
				{
					return false;
				}
				switch ((!(view.Tool is GoToolResizing)) ? SnapDrag : SnapResize)
				{
				case GoViewSnapStyle.None:
					return false;
				case GoViewSnapStyle.After:
					if (view.LastInput.InputState != GoInputState.Finish)
					{
						return false;
					}
					break;
				}
				if (view.Selection.Contains(this))
				{
					return false;
				}
			}
			RectangleF a = ComputeInfiniteBounds(Bounds);
			if (view != null && (view.Tool is GoToolResizing || view.Tool is GoToolDragging))
			{
				return GoObject.ContainsRect(a, view.LastInput.DocPoint);
			}
			return GoObject.ContainsRect(a, p);
		}

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /> to
		/// find the point to which an object should be moved or resized.
		/// </summary>
		/// <param name="p">a <c>PointF</c> in document coordinates</param>
		/// <param name="obj">the object being moved or resized (may be null/nothing)</param>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> in which the snapping is occuring; may be null/nothing</param>
		/// <returns>by default, just the result of calling <see cref="T:Northwoods.Go.GoGrid" />.<see cref="M:Northwoods.Go.GoGrid.FindNearestGridPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /></returns>
		public virtual PointF SnapPoint(PointF p, GoObject obj, GoView view)
		{
			if (obj != null && view != null && SnapDragWhole && view.Tool is GoToolDragging)
			{
				return FindNearestGridPoint(p, obj);
			}
			return FindNearestGridPoint(p, null);
		}

		/// <summary>
		/// Find the nearest grid point to a given point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> in document coordinates.</param>
		/// <param name="obj">the object being dragged if <see cref="P:Northwoods.Go.GoGrid.SnapDragWhole" /> is true;
		/// may be null/nothing</param>
		/// <returns>A <c>PointF</c> point in document coordinates,
		/// adjusted according to value of <see cref="P:Northwoods.Go.GoGrid.SnapCellSpot" />.
		/// When <paramref name="obj" /> is not null, the returned point is
		/// also adjusted to try to include the whole object within the grid.
		/// If that object cannot fit in the grid because it is too wide and/or
		/// too tall, the <see cref="P:Northwoods.Go.GoObject.Location" /> of the object,
		/// if along one of the four edges of the object, determines which side(s)
		/// stay within the bounds of the grid.
		/// </returns>
		public virtual PointF FindNearestGridPoint(PointF p, GoObject obj)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (obj != null)
			{
				PointF location = obj.Location;
				num = Math.Max(0f, location.X - obj.Left);
				num2 = Math.Max(0f, obj.Right - location.X);
				num3 = Math.Max(0f, location.Y - obj.Top);
				num4 = Math.Max(0f, obj.Bottom - location.Y);
			}
			SizeF cellSize = CellSize;
			PointF rectangleSpotLocation = GetRectangleSpotLocation(new RectangleF(0f, 0f, cellSize.Width, cellSize.Height), SnapCellSpot);
			PointF result = FindNearestInfiniteGridPoint(p, rectangleSpotLocation.X, rectangleSpotLocation.Y);
			RectangleF rectangleF = ComputeInfiniteBounds(Bounds);
			checked
			{
				if (result.X - num < rectangleF.Left)
				{
					result.X += cellSize.Width * (float)(int)Math.Ceiling((rectangleF.Left - (result.X - num)) / cellSize.Width);
				}
				if (result.X + num2 > rectangleF.Right)
				{
					result.X -= cellSize.Width * (float)(int)Math.Ceiling((result.X + num2 - rectangleF.Right) / cellSize.Width);
				}
				if (num2 > 0f && result.X - num < rectangleF.Left)
				{
					result.X += cellSize.Width * (float)(int)Math.Ceiling((rectangleF.Left - (result.X - num)) / cellSize.Width);
				}
				if (result.Y - num3 < rectangleF.Top)
				{
					result.Y += cellSize.Height * (float)(int)Math.Ceiling((rectangleF.Top - (result.Y - num3)) / cellSize.Height);
				}
				if (result.Y + num4 > rectangleF.Bottom)
				{
					result.Y -= cellSize.Height * (float)(int)Math.Ceiling((result.Y + num4 - rectangleF.Bottom) / cellSize.Height);
				}
				if (num4 > 0f && result.Y - num3 < rectangleF.Top)
				{
					result.Y += cellSize.Height * (float)(int)Math.Ceiling((rectangleF.Top - (result.Y - num3)) / cellSize.Height);
				}
				return result;
			}
		}

		private PointF FindNearestInfiniteGridPoint(PointF p, float offx, float offy)
		{
			float x = p.X;
			float y = p.Y;
			PointF origin = Origin;
			float num = origin.X + offx;
			float num2 = origin.Y + offy;
			SizeF cellSize = CellSize;
			float width = cellSize.Width;
			float height = cellSize.Height;
			float num3 = (float)Math.Floor((x - num) / width) * width + num;
			float num4 = (float)Math.Floor((y - num2) / height) * height + num2;
			float x2 = num3;
			if (num3 + width - x < width / 2f)
			{
				x2 = num3 + width;
			}
			float y2 = num4;
			if (num4 + height - y < height / 2f)
			{
				y2 = num4 + height;
			}
			return new PointF(x2, y2);
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1801:
				Style = (GoViewGridStyle)e.GetInt(undo);
				break;
			case 1802:
				Origin = e.GetPoint(undo);
				break;
			case 1803:
				OriginRelative = (bool)e.GetValue(undo);
				break;
			case 1804:
				CellSize = e.GetSize(undo);
				break;
			case 1805:
				LineColor = (Color)e.GetValue(undo);
				break;
			case 1816:
				MajorLineColor = (Color)e.GetValue(undo);
				break;
			case 1806:
				LineWidth = e.GetFloat(undo);
				break;
			case 1807:
				LineDashStyle = (DashStyle)e.GetInt(undo);
				break;
			case 1820:
				LineDashPattern = (float[])e.GetValue(undo);
				break;
			case 1817:
				MajorLineWidth = e.GetFloat(undo);
				break;
			case 1818:
				MajorLineDashStyle = (DashStyle)e.GetInt(undo);
				break;
			case 1821:
				MajorLineDashPattern = (float[])e.GetValue(undo);
				break;
			case 1819:
				MajorLineFrequency = (Size)e.GetValue(undo);
				break;
			case 1808:
				SnapDrag = (GoViewSnapStyle)e.GetInt(undo);
				break;
			case 1809:
				SnapResize = (GoViewSnapStyle)e.GetInt(undo);
				break;
			case 1810:
				CellColors = (Color[,])e.GetValue(undo);
				break;
			case 1811:
				UnboundedSpots = e.GetInt(undo);
				break;
			case 1814:
				SnapOpaque = (bool)e.GetValue(undo);
				break;
			case 1815:
				SnapCellSpot = e.GetInt(undo);
				break;
			case 1812:
				SnapDragWhole = (bool)e.GetValue(undo);
				break;
			case 1822:
				PaintMinorScale = (float)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
