using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This group arranges its children vertically or horizontally, aligned within the rectangular bounds,
	/// optionally spaced with lines drawn between the objects, bordered, and with a background.
	/// </summary>
	/// <remarks>
	/// As with any other group, adding a child to this group does not change its selectability.
	/// You will need to decide whether the user can select individual items in the list of
	/// objects.  If an item is selectable, it should have its DragsNode property set to true
	/// and it probably should not be resizable.
	/// </remarks>
	[Serializable]
	public class GoListGroup : GoGroup
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Spacing" /> property.
		/// </summary>
		public const int ChangedSpacing = 2501;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Alignment" /> property.
		/// </summary>
		public const int ChangedAlignment = 2502;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" /> property.
		/// </summary>
		public const int ChangedLinePen = 2503;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" /> property.
		/// </summary>
		public const int ChangedBorderPen = 2504;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Brush" /> property.
		/// </summary>
		public const int ChangedBrush = 2505;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Corner" /> property.
		/// </summary>
		public const int ChangedCorner = 2506;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 2507;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 2508;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 2509;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.TopIndex" /> property.
		/// </summary>
		public const int ChangedTopIndex = 2510;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.MinimumItemSize" /> property.
		/// </summary>
		public const int ChangedMinimumItemSize = 2511;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoListGroup.Clipping" /> property.
		/// </summary>
		public const int ChangedClipping = 2512;

		private const int flagClipping = 4194304;

		private int myTopIndex = -1;

		private Orientation myOrientation = Orientation.Vertical;

		private float mySpacing;

		private int myAlignment = 2;

		private GoShape.GoPenInfo myLinePenInfo;

		private GoShape.GoPenInfo myBorderPenInfo;

		private GoShape.GoBrushInfo myBrushInfo;

		[NonSerialized]
		private GraphicsPath myPath;

		[NonSerialized]
		private Brush myBrush;

		private SizeF myCorner;

		private SizeF myTopLeftMargin = new SizeF(2f, 2f);

		private SizeF myBottomRightMargin = new SizeF(2f, 2f);

		private SizeF myMinimumItemSize = new SizeF(1f, 1f);

		/// <summary>
		/// Whenever the size and/or position is changed, we need to recalculate the
		/// corners.
		/// </summary>
		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				ResetPath();
				base.Bounds = value;
			}
		}

		/// <summary>
		/// The index of the first visible child, or -1 to specify no scrolling.
		/// </summary>
		/// <value>
		/// The default value is -1.
		/// A value greater or equal to the count of items in this list will cause no items to be painted.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property can be incremented or decremented to cause the list to "scroll".
		/// When this value is zero, the first item is positioned and drawn at the top
		/// (or at the left, if <see cref="P:Northwoods.Go.GoListGroup.Orientation" /> is <c>Orientation.Horizontal</c>).
		/// Following items are positioned after, until either there are no more or until
		/// the next item will not fit within the current <c>Bounds</c>.
		/// </para>
		/// <para>
		/// When the value is greater than zero, the items whose index is less than this
		/// value are positioned at the top-left corner of the group and are made not
		/// <c>Visible</c> and not <c>Printable</c>.
		/// </para>
		/// <para>
		/// When the value is -1, all items are assumed to be <c>Visible</c> and <c>Printable</c>,
		/// and the result of <see cref="M:Northwoods.Go.GoListGroup.ComputeBounds" /> will be the size of the group including
		/// all of its items, laid out in a column or row.
		/// </para>
		/// <para>
		/// Although this property is very useful in supporting scrolling, this
		/// group does <b>not</b> provide any scroll buttons or a scroll bar or any other
		/// "controls" to allow the user to "scroll".  Such objects (such as <c>GoButton</c>s)
		/// can be added to a group or node that includes this <c>GoListGroup</c>.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(-1)]
		[Description("The index of the first visible list item")]
		public int TopIndex
		{
			get
			{
				return myTopIndex;
			}
			set
			{
				int num = myTopIndex;
				int num2 = Math.Max(-1, value);
				if (num != num2)
				{
					myTopIndex = num2;
					Changed(2510, num, null, GoObject.NullRect, num2, null, GoObject.NullRect);
					if (num2 < 0)
					{
						using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
						{
							while (goGroupEnumerator.MoveNext())
							{
								GoObject current = goGroupEnumerator.Current;
								current.Visible = true;
								current.Printable = true;
							}
						}
					}
					else
					{
						InvalidateViews();
					}
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum size to be used when resizing.
		/// </summary>
		/// <value>
		/// The width and height must not be negative.  The default value is 1x1.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The minimum Width and Height for each item")]
		public virtual SizeF MinimumItemSize
		{
			get
			{
				return myMinimumItemSize;
			}
			set
			{
				SizeF sizeF = myMinimumItemSize;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myMinimumItemSize = value;
					Changed(2511, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the direction in which the child objects are positioned relative to each other.
		/// </summary>
		/// <value>
		/// The default orientation is <c>Vertical</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Vertical)]
		[Description("How LayoutChildren will position the items.")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				Orientation orientation = myOrientation;
				if (orientation != value)
				{
					myOrientation = value;
					Changed(2509, (int)orientation, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets how much space there should be between items.
		/// </summary>
		/// <value>
		/// This specifies the vertical distance between items in document coordinates.
		/// By default this value is <c>0</c>, so that the items just touch each other.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The additional vertical distance between items.")]
		public virtual float Spacing
		{
			get
			{
				return mySpacing;
			}
			set
			{
				float num = mySpacing;
				if (num != value)
				{
					mySpacing = value;
					Changed(2501, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets where each item should be drawn if it is narrower than the width
		/// of the whole group.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoObject.TopLeft" />, i.e., left-aligned (or
		/// top-aligned if the orientation is horizontal).
		/// </value>
		[Category("Appearance")]
		[DefaultValue(2)]
		[Description("How each item is positioned along the X axis.")]
		public virtual int Alignment
		{
			get
			{
				return myAlignment;
			}
			set
			{
				int num = myAlignment;
				if (num != value)
				{
					myAlignment = value;
					Changed(2502, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Pen</c> used to draw lines separating the items.
		/// </summary>
		/// <value>
		/// You must not modify the pen after you have assigned it.
		/// If this value is null, no lines will be drawn.
		/// By default this value is null.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The pen used to draw lines separating the items.")]
		public virtual Pen LinePen
		{
			get
			{
				if (LinePenInfo != null)
				{
					return LinePenInfo.GetPen();
				}
				return null;
			}
			set
			{
				LinePenInfo = GoShape.GetPenInfo(value);
			}
		}

		internal GoShape.GoPenInfo LinePenInfo
		{
			get
			{
				return myLinePenInfo;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = myLinePenInfo;
				if (goPenInfo != value && (goPenInfo == null || !goPenInfo.Equals(value)))
				{
					myLinePenInfo = value;
					Changed(2503, 0, goPenInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" />.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" />, or <b>Color.Empty</b> if there is no pen.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoListGroup.LinePen" /> to null.
		/// </value>
		[Category("Appearance")]
		[Description("The color of the pen used to draw lines separating the items")]
		public virtual Color LinePenColor
		{
			get
			{
				return LinePenInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoShape.GoPenInfo linePenInfo = LinePenInfo;
				GoShape.GoPenInfo goPenInfo = null;
				if (linePenInfo != null)
				{
					if (linePenInfo.Color == value)
					{
						return;
					}
					if (value != Color.Empty)
					{
						goPenInfo = new GoShape.GoPenInfo(linePenInfo);
						goPenInfo.Color = value;
					}
				}
				else if (value != Color.Empty)
				{
					goPenInfo = new GoShape.GoPenInfo();
					goPenInfo.Width = LinePenWidth;
					goPenInfo.Color = value;
				}
				LinePenInfo = goPenInfo;
			}
		}

		/// <summary>
		/// Gets or sets the width of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" />.
		/// </summary>
		/// <value>
		/// The width of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" />, or 0 if there is no pen.
		/// The default value is zero.
		/// </value>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoListGroup.LinePen" />, setting this property might have no effect.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to draw the outline of the shape.")]
		public virtual float LinePenWidth
		{
			get
			{
				return LinePenInfo?.Width ?? 0f;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = LinePenInfo;
				float num = 0f;
				if (goPenInfo != null)
				{
					num = goPenInfo.Width;
				}
				else
				{
					goPenInfo = GoShape.PenInfo_Black;
				}
				if (num != value)
				{
					GoShape.GoPenInfo goPenInfo2 = new GoShape.GoPenInfo(goPenInfo);
					goPenInfo2.Width = value;
					LinePenInfo = goPenInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the pen used to draw a rectangular outline just inside the
		/// edges of this node.
		/// </summary>
		/// <value>
		/// You must not modify the pen after you have assigned it.
		/// The <c>Pen</c> value may be null, in which case no outline is drawn.
		/// This value defaults to null.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The pen used to draw an outline for this node.")]
		public virtual Pen BorderPen
		{
			get
			{
				if (BorderPenInfo != null)
				{
					return BorderPenInfo.GetPen();
				}
				return null;
			}
			set
			{
				BorderPenInfo = GoShape.GetPenInfo(value);
			}
		}

		internal GoShape.GoPenInfo BorderPenInfo
		{
			get
			{
				return myBorderPenInfo;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = myBorderPenInfo;
				if (goPenInfo != value && (goPenInfo == null || !goPenInfo.Equals(value)))
				{
					myBorderPenInfo = value;
					ResetPath();
					Changed(2504, 0, goPenInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
					InvalidatePaintBounds();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" />.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" />, or <b>Color.Empty</b> if there is no pen.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" /> to null.
		/// </value>
		[Category("Appearance")]
		[Description("The color of the pen used to draw the border of the group")]
		public virtual Color BorderPenColor
		{
			get
			{
				return BorderPenInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoShape.GoPenInfo borderPenInfo = BorderPenInfo;
				GoShape.GoPenInfo goPenInfo = null;
				if (borderPenInfo != null)
				{
					if (borderPenInfo.Color == value)
					{
						return;
					}
					if (value != Color.Empty)
					{
						goPenInfo = new GoShape.GoPenInfo(borderPenInfo);
						goPenInfo.Color = value;
					}
				}
				else if (value != Color.Empty)
				{
					goPenInfo = new GoShape.GoPenInfo();
					goPenInfo.Width = BorderPenWidth;
					goPenInfo.Color = value;
				}
				BorderPenInfo = goPenInfo;
			}
		}

		/// <summary>
		/// Gets or sets the width of the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" />.
		/// </summary>
		/// <value>
		/// The width of the <see cref="P:Northwoods.Go.GoListGroup.BorderPen" />, or 0 if there is no pen.
		/// The default value is zero.
		/// </value>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoListGroup.BorderPen" />, setting this property might have no effect.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to draw the outline of the shape.")]
		public virtual float BorderPenWidth
		{
			get
			{
				return BorderPenInfo?.Width ?? 0f;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = BorderPenInfo;
				float num = 0f;
				if (goPenInfo != null)
				{
					num = goPenInfo.Width;
				}
				else
				{
					goPenInfo = GoShape.PenInfo_Black;
				}
				if (num != value)
				{
					GoShape.GoPenInfo goPenInfo2 = new GoShape.GoPenInfo(goPenInfo);
					goPenInfo2.Width = value;
					BorderPenInfo = goPenInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used to fill this group as a background.
		/// </summary>
		/// <value>
		/// The <c>Brush</c> value may be null, in which case no background is drawn.
		/// This value defaults to null.
		/// </value>
		/// <remarks>
		/// <para>
		/// LinearGradientBrushes are drawn with their origin at the Position of this group.
		/// </para>
		/// <para>
		/// You must not modify the brush after you have assigned it.
		/// It is common to use the predefined brushes that are members of the
		/// <c>Brushes</c> class.
		/// For the simple linear gradient and path gradient effects,
		/// you can just set the <see cref="P:Northwoods.Go.GoListGroup.BrushColor" />, <see cref="P:Northwoods.Go.GoListGroup.BrushForeColor" />,
		/// <see cref="P:Northwoods.Go.GoListGroup.BrushMidColor" />, and <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> properties.
		/// Finally, for the most sophisticated or complex kinds of gradient brushes,
		/// you may need to override this property to return the kind of value you need.
		/// </para>
		/// <para>
		/// When a linear gradient brush or a path gradient brush is drawn very small,
		/// due to a combination of small size and small <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush or not draw anything at all,
		/// both for efficiency as well as to avoid GDI+ errors.
		/// When a path gradient brush is drawn very large,
		/// due to a combination of large size and large <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush, both for efficiency as well as to avoid GDI+ errors.
		/// </para>
		/// <para>
		/// Currently serialization is limited to standard solid, hatch,
		/// and texture brushes and many kinds of simple linear gradient and
		/// path gradient brushes.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The brush used to fill the outline of this GoListGroup.")]
		public virtual Brush Brush
		{
			get
			{
				if (BrushInfo != null)
				{
					if (myBrush == null)
					{
						myBrush = BrushInfo.GetBrush(GetPath(0f, 0f));
					}
					return myBrush;
				}
				return null;
			}
			set
			{
				BrushInfo = GoShape.GetBrushInfo(value, GetPath(0f, 0f));
			}
		}

		internal GoShape.GoBrushInfo BrushInfo
		{
			get
			{
				return myBrushInfo;
			}
			set
			{
				GoShape.GoBrushInfo goBrushInfo = myBrushInfo;
				if (goBrushInfo != value && (goBrushInfo == null || !goBrushInfo.Equals(value)))
				{
					myBrushInfo = value;
					myBrush = null;
					Changed(2505, 0, goBrushInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the main or background color of the brush.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoListGroup.Brush" />, or <b>Color.Empty</b> if there is no brush.
		/// Setting the new value to a non-empty <b>Color</b> when <see cref="P:Northwoods.Go.GoListGroup.Brush" /> is null
		/// will set the <see cref="P:Northwoods.Go.GoListGroup.Brush" /> to a <b>SolidBrush</b> of that new color.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoListGroup.Brush" /> to null if
		/// the old brush was a <b>SolidBrush</b>.
		/// </value>
		/// <remarks>
		/// This refers to the color of a <b>SolidBrush</b>, the background color of
		/// a <b>HatchBrush</b>, the ending color of a <b>LinearGradientBrush</b>, or the
		/// center color of a <b>PathGradientBrush</b>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The background color of a solid, hatch, or gradient brush")]
		public virtual Color BrushColor
		{
			get
			{
				return BrushInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.Color != value)
				{
					if (value == Color.Empty && brushInfo.BrushType == GoShape.GoBrushType.Solid)
					{
						BrushInfo = null;
						return;
					}
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.Color = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.Color = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the foreground color of a brush.
		/// </summary>
		/// <value>
		/// Setting this value will have no effect for brushes that are not
		/// <b>HatchBrush</b>, <b>LinearGradientBrush</b>, or <b>PathGradientBrush</b>.
		/// </value>
		/// <remarks>
		/// This refers to the foreground color of a <b>HatchBrush</b>,
		/// the starting color of a <b>LinearGradientBrush</b>, or the
		/// surrounding color of a <b>PathGradientBrush</b>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The foreground color of a hatch or gradient brush")]
		public virtual Color BrushForeColor
		{
			get
			{
				return BrushInfo?.ForeColor ?? Color.Empty;
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.ForeColor != value)
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.ForeColor = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.ForeColor = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the middle color of a gradient brush.
		/// </summary>
		/// <value>
		/// For a <b>LinearGradientBrush</b> or <b>PathGradientBrush</b>, the value may be <b>Color.Empty</b>
		/// if no middle color is needed.
		/// This defaults to <b>Color.Empty</b>.
		/// Setting this value will have no effect for other kinds of brushes.
		/// </value>
		[Category("Appearance")]
		[Description("The middle color of a linear gradient brush")]
		public virtual Color BrushMidColor
		{
			get
			{
				return BrushInfo?.MidColor ?? Color.Empty;
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.MidColor != value)
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.MidColor = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.MidColor = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the fractional distance between the end colors at which the middle color is drawn.
		/// </summary>
		/// <remarks>
		/// This property is ignored if the value is <b>Single.NaN</b>.
		/// The meaning of this property depends on the value <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> property;
		/// many brush styles ignore this property.
		/// </remarks>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> is a linear gradient or a path gradient
		/// that supports three colors.
		/// Changing the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[Description("The fraction (0-1) of the distance between the end colors at which the middle color is drawn")]
		public virtual float BrushMidFraction
		{
			get
			{
				return BrushInfo?.MidFraction ?? float.NaN;
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				float num = value;
				if (!float.IsNaN(num))
				{
					if (num < 0f)
					{
						num = 0f;
					}
					else if (num > 1f)
					{
						num = 1f;
					}
				}
				if (brushInfo != null && brushInfo.MidFraction != num)
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.MidFraction = num;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && num != float.NaN)
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.MidFraction = num;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the start point for linear gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>PointF</b> whose X and Y values are normally between 0 and 1, and should be between -1 and 2.
		/// An X value of zero corresponds to the left side of the shape; an X value of 1 corresponds to the right side.
		/// A Y value of zero corresponds to the top side of the shape; a Y value of 1 corresponds to the bottom side.
		/// Negative values or values greater than 1 denote points that are outside of the shape.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> is a linear gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The normalized/fractional start point for linear gradients, typically each value from around 0 to 1")]
		public virtual PointF BrushStartPoint
		{
			get
			{
				return BrushInfo?.StartOrFocusScales ?? default(PointF);
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				PointF pointF = value;
				if (pointF.X < -1f)
				{
					pointF.X = -1f;
				}
				else if (pointF.X > 2f)
				{
					pointF.X = 2f;
				}
				if (pointF.Y < -1f)
				{
					pointF.Y = -1f;
				}
				else if (pointF.Y > 2f)
				{
					pointF.Y = 2f;
				}
				if (brushInfo != null && brushInfo.Point != pointF)
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.StartOrFocusScales = pointF;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && pointF != default(PointF))
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.StartOrFocusScales = pointF;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the end point for linear gradients or the focus point for path gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>PointF</b> whose X and Y values are normally between 0 and 1, and should be between -1 and 2.
		/// An X value of zero corresponds to the left side of the shape; an X value of 1 corresponds to the right side.
		/// A Y value of zero corresponds to the top side of the shape; a Y value of 1 corresponds to the bottom side.
		/// Negative values or values greater than 1 denote points that are outside of the shape.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> is a linear gradient or a path gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The normalized/fractional end point for linear gradients or focus point for path gradients, typically value each around from 0 to 1")]
		public virtual PointF BrushPoint
		{
			get
			{
				return BrushInfo?.Point ?? default(PointF);
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				PointF pointF = value;
				if (pointF.X < -1f)
				{
					pointF.X = -1f;
				}
				else if (pointF.X > 2f)
				{
					pointF.X = 2f;
				}
				if (pointF.Y < -1f)
				{
					pointF.Y = -1f;
				}
				else if (pointF.Y > 2f)
				{
					pointF.Y = 2f;
				}
				if (brushInfo != null && brushInfo.Point != pointF)
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.Point = pointF;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && pointF != default(PointF))
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.Point = pointF;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of the focus area for path gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>SizeF</b> whose Width and Height values must be between 0 and 1.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> is a path gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoListGroup.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The fractional size of the focus area of a path gradient, each value from 0 to 1")]
		public virtual SizeF BrushFocusScales
		{
			get
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null)
				{
					return new SizeF(brushInfo.StartOrFocusScales.X, brushInfo.StartOrFocusScales.Y);
				}
				return default(SizeF);
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				SizeF sz = value;
				if (sz.Width < 0f)
				{
					sz.Width = 0f;
				}
				else if (sz.Width > 1f)
				{
					sz.Width = 1f;
				}
				if (sz.Height < 0f)
				{
					sz.Height = 0f;
				}
				else if (sz.Height > 1f)
				{
					sz.Height = 1f;
				}
				if (brushInfo != null && (brushInfo.StartOrFocusScales.X != sz.Width || brushInfo.StartOrFocusScales.Y != sz.Height))
				{
					GoShape.GoBrushInfo goBrushInfo = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo.StartOrFocusScales = new PointF(sz.Width, sz.Height);
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && sz != default(SizeF))
				{
					GoShape.GoBrushInfo goBrushInfo2 = new GoShape.GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.StartOrFocusScales = new PointF(sz.Width, sz.Height);
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the kind of brush used by this shape.
		/// </summary>
		/// <value>
		/// A <see cref="T:Northwoods.Go.GoBrushStyle" />.
		/// The default value depends on the shape type.
		/// However, for most shapes the default is <see cref="F:Northwoods.Go.GoBrushStyle.None" />
		/// because <see cref="P:Northwoods.Go.GoListGroup.Brush" /> is null.
		/// </value>
		/// <remarks>
		/// <para>
		/// Changing this property to a new <see cref="T:Northwoods.Go.GoBrushStyle" /> that is a
		/// gradient will also reset some of the brush properties to
		/// default values established by the corresponding <b>Fill...</b> methods.
		/// In particular, the value of <see cref="P:Northwoods.Go.GoListGroup.BrushMidFraction" />, <see cref="P:Northwoods.Go.GoListGroup.BrushPoint" />,
		/// <see cref="P:Northwoods.Go.GoListGroup.BrushStartPoint" />, and/or <see cref="P:Northwoods.Go.GoListGroup.BrushFocusScales" /> may change.
		/// However, setting this property will not change the <see cref="P:Northwoods.Go.GoListGroup.BrushColor" />,
		/// <see cref="P:Northwoods.Go.GoListGroup.BrushForeColor" /> or <see cref="P:Northwoods.Go.GoListGroup.BrushMidColor" /> properties,
		/// although some or all of those properties might not be used by certain
		/// brush styles.  You will normally want to set this property first, before
		/// setting other <b>Brush...</b> properties.
		/// </para>
		/// <para>
		/// When a linear gradient brush or a path gradient brush is drawn very small,
		/// due to a combination of small size and small <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush or not draw anything at all,
		/// both for efficiency as well as to avoid GDI+ errors.
		/// When a path gradient brush is drawn very large,
		/// due to a combination of large size and large <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush, both for efficiency as well as to avoid GDI+ errors.
		/// </para>
		/// <para>
		/// Caution: using gradient brush styles, particularly path gradients,
		/// can be computationally expensive to paint.  This is especially true
		/// for large shapes.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[Description("The kind of simple brush used by this shape, including some linear and path gradients")]
		public virtual GoBrushStyle BrushStyle
		{
			get
			{
				return BrushInfo?.BrushStyle ?? GoBrushStyle.None;
			}
			set
			{
				GoShape.GoBrushInfo brushInfo = BrushInfo;
				if ((brushInfo == null || brushInfo.BrushStyle != value) && (brushInfo != null || value != 0))
				{
					BrushInfo = GoShape.ModifyBrushStyle(brushInfo, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum radial width and height of each corner.
		/// </summary>
		/// <value>
		/// The default value is 0x0.  Both the width and the height must be
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
					Changed(2506, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the top and left sides of the text label
		/// with the top and left sides of the border.
		/// </summary>
		/// <value>
		/// The width and height must not be negative.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the left side and the top")]
		public virtual SizeF TopLeftMargin
		{
			get
			{
				return myTopLeftMargin;
			}
			set
			{
				SizeF sizeF = myTopLeftMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myTopLeftMargin = value;
					Changed(2507, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the bottom and right sides of the text label
		/// with the bottom and right sides of the border.
		/// </summary>
		/// <value>
		/// The width and height must not be negative.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the right side and the bottom")]
		public virtual SizeF BottomRightMargin
		{
			get
			{
				return myBottomRightMargin;
			}
			set
			{
				SizeF sizeF = myBottomRightMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myBottomRightMargin = value;
					Changed(2508, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the items should be clipped to paint within the rounded corners of the border.
		/// </summary>
		/// <value>The default value is false</value>
		/// <remarks>
		/// Setting this property to true is useful when the <see cref="P:Northwoods.Go.GoListGroup.TopLeftMargin" /> and/or <see cref="P:Northwoods.Go.GoListGroup.BottomRightMargin" />
		/// are small enough relative to the <see cref="P:Northwoods.Go.GoListGroup.Corner" /> to cause the rounded corners of the border to be drawn
		/// across the top item(s) or bottom item(s).
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the child objects should be clipped within the rounded corners of the border")]
		public virtual bool Clipping
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
					Changed(2512, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (base.Parent != null)
					{
						base.Parent.InvalidatePaintBounds();
					}
				}
			}
		}

		/// <summary>
		/// Construct an empty GoListGroup.
		/// </summary>
		public GoListGroup()
		{
			base.InternalFlags &= -17;
		}

		/// <summary>
		/// Perform some internal bookkeeping in addition to copying each of the items.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			GoListGroup goListGroup = (GoListGroup)newgroup;
			if (goListGroup != null)
			{
				goListGroup.myPath = null;
				goListGroup.myBrush = null;
			}
			base.CopyChildren(newgroup, env);
		}

		/// <summary>
		/// The width of this group is just the maximum width of all of the children,
		/// and the height is the sum of the heights of the items, incremented by the
		/// <see cref="P:Northwoods.Go.GoListGroup.Spacing" /> property between the items.
		/// </summary>
		/// <returns>
		/// a <c>RectangleF</c> describing the actual size and position
		/// of the whole group in document coordinates
		/// </returns>
		/// <remarks>
		/// <para>
		/// Of course, if <see cref="P:Northwoods.Go.GoListGroup.Orientation" /> is <c>Horizontal</c>,
		/// the width of this group is the sum of the widths of the items,
		/// incremented by the <see cref="P:Northwoods.Go.GoListGroup.Spacing" /> property between the items,
		/// and the height is just the maximum height of all of the children.
		/// This ignores child objects that are not <see cref="P:Northwoods.Go.GoObject.Visible" />.
		/// </para>
		/// <para>
		/// When <see cref="P:Northwoods.Go.GoListGroup.TopIndex" /> is non-negative, this group supports "scrolling".
		/// The <c>Bounds</c> is not determined by the size of the child objects,
		/// but can be set, possibly resulting in some objects at the start and at
		/// the end of the list being made not <c>Visible</c>.
		/// </para>
		/// </remarks>
		protected override RectangleF ComputeBounds()
		{
			if (TopIndex >= 0)
			{
				if (Bounds == new RectangleF(0f, 0f, 10f, 10f))
				{
					using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
					{
						while (goGroupEnumerator.MoveNext())
						{
							GoObject current = goGroupEnumerator.Current;
							if (current.Visible)
							{
								return current.Bounds;
							}
						}
					}
				}
				return Bounds;
			}
			RectangleF bounds = Bounds;
			SizeF topLeftMargin = TopLeftMargin;
			SizeF bottomRightMargin = BottomRightMargin;
			float num = 0f;
			float num2 = 0f;
			float num3 = Spacing;
			if (LinePenInfo != null)
			{
				num3 = Math.Max(LinePenInfo.Width, num3);
			}
			if (Orientation == Orientation.Vertical)
			{
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current2 = goGroupEnumerator.Current;
						if (current2 != null)
						{
							num = Math.Max(num, current2.Width);
							if (current2.Visible)
							{
								if (num2 > 0f)
								{
									num2 += num3;
								}
								num2 += current2.Height;
							}
						}
					}
				}
			}
			else
			{
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current3 = goGroupEnumerator.Current;
						if (current3 != null)
						{
							num2 = Math.Max(num2, current3.Height);
							if (current3.Visible)
							{
								if (num > 0f)
								{
									num += num3;
								}
								num += current3.Width;
							}
						}
					}
				}
			}
			bounds.Width = num + topLeftMargin.Width + bottomRightMargin.Width;
			bounds.Height = num2 + topLeftMargin.Height + bottomRightMargin.Height;
			return bounds;
		}

		/// <summary>
		/// Never resize any children unless <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true.
		/// </summary>
		/// <param name="old"></param>
		protected override void RescaleChildren(RectangleF old)
		{
			if (AutoRescales)
			{
				base.RescaleChildren(old);
			}
		}

		/// <summary>
		/// Position each child starting at the top, according to the alignment,
		/// leaving space according to the spacing and drawn lines.
		/// </summary>
		/// <param name="childchanged">the child object that was moved or resized</param>
		/// <remarks>
		/// The first child is drawn at the top (smallest Y coordinate); the last
		/// one is at the bottom (largest Y coordinate).
		/// The actual amount of space between items is the larger of the <see cref="P:Northwoods.Go.GoListGroup.Spacing" />
		/// value and the width of the <see cref="P:Northwoods.Go.GoListGroup.LinePen" />.
		/// The actual positioning of each child item is performed by a call
		/// to <see cref="M:Northwoods.Go.GoListGroup.LayoutItem(System.Int32,System.Drawing.RectangleF)" />.
		/// When <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true, this method does nothing.
		/// When <see cref="P:Northwoods.Go.GoListGroup.Orientation" /> is <c>Horizontal</c> instead of <c>Vertical</c>,
		/// the items are positioned from the left towards the right instead of top-down.
		/// This ignores child objects that are not <see cref="P:Northwoods.Go.GoObject.Visible" />.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			ResetPath();
			SizeF topLeftMargin = TopLeftMargin;
			SizeF bottomRightMargin = BottomRightMargin;
			float num = Spacing;
			if (LinePenInfo != null)
			{
				num = Math.Max(LinePenInfo.Width, num);
			}
			int topIndex = TopIndex;
			checked
			{
				if (topIndex >= 0)
				{
					if (Orientation == Orientation.Vertical)
					{
						float num2 = 0f;
						float num3 = base.Height - topLeftMargin.Height - bottomRightMargin.Height;
						for (int i = 0; i < Count; i++)
						{
							GoObject goObject = this[i];
							if (i < topIndex)
							{
								goObject.Visible = false;
								goObject.Printable = false;
								continue;
							}
							if (i != topIndex && num2 + num + goObject.Height > num3)
							{
								goObject.Visible = false;
								goObject.Printable = false;
								continue;
							}
							if (num2 > 0f)
							{
								num2 += num;
							}
							num2 += goObject.Height;
							goObject.Visible = true;
							goObject.Printable = true;
						}
					}
					else
					{
						float num4 = 0f;
						float num5 = base.Width - topLeftMargin.Width - bottomRightMargin.Width;
						for (int j = 0; j < Count; j++)
						{
							GoObject goObject2 = this[j];
							if (j < topIndex)
							{
								goObject2.Visible = false;
								goObject2.Printable = false;
								continue;
							}
							if (j != topIndex && num4 + num + goObject2.Width > num5)
							{
								goObject2.Visible = false;
								goObject2.Printable = false;
								continue;
							}
							if (num4 > 0f)
							{
								num4 += num;
							}
							num4 += goObject2.Width;
							goObject2.Visible = true;
							goObject2.Printable = true;
						}
					}
				}
				float left = base.Left;
				float top = base.Top;
				float num6 = 0f;
				float num7 = 0f;
				if (LinePenInfo != null)
				{
					num = Math.Max(LinePenInfo.Width, num);
				}
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current = goGroupEnumerator.Current;
						if (current != null && current.Visible)
						{
							num6 = Math.Max(num6, current.Width);
							num7 = Math.Max(num7, current.Height);
						}
					}
				}
				float num8 = left + topLeftMargin.Width;
				float num9 = top + topLeftMargin.Height;
				if (Orientation == Orientation.Vertical)
				{
					float num2 = num9;
					for (int k = 0; k < Count; k++)
					{
						float num10 = num2;
						num2 = Math.Max(num2, LayoutItem(k, new RectangleF(num8, num2, num6, num7 - num2)));
						if (num2 > num10)
						{
							num2 += num;
						}
					}
				}
				else
				{
					float num4 = num8;
					for (int l = 0; l < Count; l++)
					{
						float num11 = num4;
						num4 = Math.Max(num4, LayoutItem(l, new RectangleF(num4, num9, num6 - num4, num7)));
						if (num4 > num11)
						{
							num4 += num;
						}
					}
				}
				base.InvalidBounds = true;
			}
		}

		/// <summary>
		/// Position a child item, within a delimiting rectangle.
		/// </summary>
		/// <param name="i">the index in the group of the child object to be positioned</param>
		/// <param name="cell">
		/// a <c>RectangleF</c> delimiting where the item should be placed;
		/// the width and height denote how much room remains in the whole group for all
		/// of the child items to be placed.
		/// </param>
		/// <returns>the maximum Y coordinate so far, including the just positioned item
		/// (or the maximum X coordinate if the orientation is horizontal)</returns>
		public virtual float LayoutItem(int i, RectangleF cell)
		{
			if (Orientation == Orientation.Vertical)
			{
				float num = cell.Y;
				GoObject goObject = this[i];
				if (goObject == null)
				{
					return num;
				}
				if (goObject.Visible)
				{
					PointF position;
					switch (Alignment)
					{
					case 2:
					case 16:
					case 256:
						position = new PointF(cell.X, num);
						break;
					default:
						position = new PointF(cell.X + (cell.Width - goObject.Width) / 2f, num);
						break;
					case 4:
					case 8:
					case 64:
						position = new PointF(cell.X + cell.Width - goObject.Width, num);
						break;
					}
					goObject.Position = position;
					num += goObject.Height;
				}
				else
				{
					goObject.Position = new PointF(cell.X, cell.Y);
				}
				return num;
			}
			float num2 = cell.X;
			GoObject goObject2 = this[i];
			if (goObject2 == null)
			{
				return num2;
			}
			if (goObject2.Visible)
			{
				PointF position2;
				switch (Alignment)
				{
				case 2:
				case 4:
				case 32:
					position2 = new PointF(num2, cell.Y);
					break;
				default:
					position2 = new PointF(num2, cell.Y + (cell.Height - goObject2.Height) / 2f);
					break;
				case 8:
				case 16:
				case 128:
					position2 = new PointF(num2, cell.Y + cell.Height - goObject2.Height);
					break;
				}
				goObject2.Position = position2;
				num2 += goObject2.Width;
			}
			else
			{
				goObject2.Position = new PointF(cell.X, cell.Y);
			}
			return num2;
		}

		/// <summary>
		/// This value is used when resizing this GoListGroup.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Resizing should only be true when <see cref="P:Northwoods.Go.GoListGroup.TopIndex" /> is not negative,
		/// to indicate that scrolling is supported.
		/// </remarks>
		public virtual SizeF ComputeMaximumItemSize()
		{
			float num = 0f;
			float num2 = 0f;
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current = goGroupEnumerator.Current;
					if (current.Width > num)
					{
						num = current.Width;
					}
					if (current.Height > num2)
					{
						num2 = current.Height;
					}
				}
			}
			return new SizeF(num, num2);
		}

		/// <summary>
		/// Interactive resizing is limited to be large enough to hold any item in the list.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <remarks>
		/// Resizing should only occur when <see cref="P:Northwoods.Go.GoListGroup.TopIndex" /> is not negative,
		/// to indicate that scrolling is supported.
		/// This calls <see cref="M:Northwoods.Go.GoListGroup.ComputeMaximumItemSize" /> and uses <see cref="P:Northwoods.Go.GoListGroup.MinimumItemSize" />
		/// to determine the real minimum <c>SizeF</c> to be passed to the base method call.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			SizeF sizeF = ComputeMaximumItemSize();
			SizeF minimumItemSize = MinimumItemSize;
			sizeF.Width = Math.Max(sizeF.Width, minimumItemSize.Width);
			sizeF.Height = Math.Max(sizeF.Height, minimumItemSize.Height);
			SizeF topLeftMargin = TopLeftMargin;
			SizeF bottomRightMargin = BottomRightMargin;
			if (Orientation == Orientation.Vertical)
			{
				min.Height = topLeftMargin.Height + sizeF.Height + bottomRightMargin.Height;
				min.Width = sizeF.Width;
			}
			else
			{
				min.Width = topLeftMargin.Width + sizeF.Width + bottomRightMargin.Width;
				min.Height = sizeF.Height;
			}
			base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
		}

		/// <summary>
		/// In addition to painting all of the children, this method also
		/// draws lines using <see cref="P:Northwoods.Go.GoListGroup.LinePen" /> between each child and
		/// <see cref="P:Northwoods.Go.GoListGroup.BorderPen" /> around them all.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoListGroup.PaintDecoration(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// Painting of the child objects may be clipped to the border if <see cref="P:Northwoods.Go.GoListGroup.Clipping" /> is true.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			if (Clipping && Corner != new SizeF(0f, 0f))
			{
				GraphicsState gstate = g.Save();
				PaintDecoration(g, view);
				base.Paint(g, view);
				g.Restore(gstate);
			}
			else
			{
				PaintDecoration(g, view);
				base.Paint(g, view);
			}
		}

		/// <summary>
		/// Paint a shadow, filled border, and separator lines.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoListGroup.Clipping" /> property is true and if <see cref="P:Northwoods.Go.GoListGroup.Corner" /> is non-zero,
		/// painting of the child items and of the separator lines will be clipped to fit within
		/// the rounded rectangle formed by the border.
		/// </remarks>
		public virtual void PaintDecoration(Graphics g, GoView view)
		{
			Brush brush = Brush;
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				GraphicsPath path = GetPath(shadowOffset.Width, shadowOffset.Height);
				if (brush != null)
				{
					Brush shadowBrush = GetShadowBrush(view);
					GoShape.DrawPath(g, view, null, shadowBrush, path);
				}
				else if (BorderPen != null)
				{
					Pen shadowPen = GetShadowPen(view, GoShape.GetPenWidth(BorderPen));
					GoShape.DrawPath(g, view, shadowPen, null, path);
				}
				DisposePath(path);
			}
			GraphicsPath path2 = GetPath(0f, 0f);
			GoShape.DrawPath(g, view, BorderPen, brush, path2);
			if (Clipping && Corner != new SizeF(0f, 0f))
			{
				Region region = new Region(path2);
				g.IntersectClip(region);
				region.Dispose();
			}
			Pen linePen = LinePen;
			checked
			{
				if (linePen != null)
				{
					float left = base.Left;
					float top = base.Top;
					float width = base.Width;
					float height = base.Height;
					float num = Math.Max(GoShape.GetPenWidth(linePen), Spacing);
					SizeF topLeftMargin = TopLeftMargin;
					if (Orientation == Orientation.Vertical)
					{
						float num2 = top + topLeftMargin.Height;
						float num3 = 0f;
						for (int i = 0; i < Count; i++)
						{
							GoObject goObject = this[i];
							if (goObject != null && goObject.Visible)
							{
								if (num3 > 0f)
								{
									GoShape.DrawLine(g, view, linePen, left, num2 + num3 + num / 2f, left + width, num2 + num3 + num / 2f);
									num3 += num;
								}
								num3 += goObject.Height;
							}
						}
					}
					else
					{
						float num4 = left + topLeftMargin.Width;
						float num5 = 0f;
						for (int j = 0; j < Count; j++)
						{
							GoObject goObject2 = this[j];
							if (goObject2 != null && goObject2.Visible)
							{
								if (num5 > 0f)
								{
									GoShape.DrawLine(g, view, linePen, num4 + num5 + num / 2f, top, num4 + num5 + num / 2f, top + height);
									num5 += num;
								}
								num5 += goObject2.Width;
							}
						}
					}
				}
				DisposePath(path2);
			}
		}

		private GraphicsPath GetPath(float offx, float offy)
		{
			RectangleF bounds = Bounds;
			SizeF corner = Corner;
			if (offx != 0f || offy != 0f)
			{
				GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
				GoRoundedRectangle.MakeRoundedRectangularPath(graphicsPath, offx, offy, bounds, corner);
				return graphicsPath;
			}
			if (myPath == null)
			{
				myPath = new GraphicsPath(FillMode.Winding);
				GoRoundedRectangle.MakeRoundedRectangularPath(myPath, 0f, 0f, bounds, corner);
			}
			return myPath;
		}

		private void DisposePath(GraphicsPath path)
		{
			if (path != myPath)
			{
				path.Dispose();
			}
		}

		private void ResetPath()
		{
			if (myPath != null)
			{
				myPath.Dispose();
				myPath = null;
			}
			if (myBrush != null)
			{
				if (myBrushInfo != null && !myBrushInfo.HasBrush)
				{
					myBrush.Dispose();
				}
				myBrush = null;
			}
		}

		/// <summary>
		/// The expanded paint bounds for a GoListGroup includes any BorderPen width
		/// and any drop shadow.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			rect = base.ExpandPaintBounds(rect, view);
			if (BorderPenInfo != null)
			{
				float num = BorderPenInfo.Width / 2f;
				GoObject.InflateRect(ref rect, num, num);
			}
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (shadowOffset.Width < 0f)
				{
					rect.X += shadowOffset.Width;
					rect.Width -= shadowOffset.Width;
				}
				else
				{
					rect.Width += shadowOffset.Width;
				}
				if (shadowOffset.Height < 0f)
				{
					rect.Y += shadowOffset.Height;
					rect.Height -= shadowOffset.Height;
				}
				else
				{
					rect.Height += shadowOffset.Height;
				}
			}
			return rect;
		}

		/// <summary>
		/// Perform changes to the spacing, alignment, and pen.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1001:
				base.ChangeValue(e, undo);
				ResetPath();
				break;
			case 2510:
				TopIndex = e.GetInt(undo);
				break;
			case 2501:
				Spacing = e.GetFloat(undo);
				break;
			case 2502:
				Alignment = e.GetInt(undo);
				break;
			case 2503:
			{
				object value2 = e.GetValue(undo);
				if (value2 is Pen)
				{
					LinePen = (Pen)value2;
				}
				else if (value2 is GoShape.GoPenInfo)
				{
					LinePenInfo = (GoShape.GoPenInfo)value2;
				}
				break;
			}
			case 2504:
			{
				object value3 = e.GetValue(undo);
				if (value3 is Pen)
				{
					BorderPen = (Pen)value3;
				}
				else if (value3 is GoShape.GoPenInfo)
				{
					BorderPenInfo = (GoShape.GoPenInfo)value3;
				}
				break;
			}
			case 2505:
			{
				object value = e.GetValue(undo);
				if (value is Brush)
				{
					Brush = (Brush)value;
				}
				else if (value is GoShape.GoBrushInfo)
				{
					BrushInfo = (GoShape.GoBrushInfo)value;
				}
				break;
			}
			case 2506:
				Corner = e.GetSize(undo);
				break;
			case 2507:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 2508:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 2509:
				Orientation = (Orientation)e.GetInt(undo);
				break;
			case 2511:
				MinimumItemSize = e.GetSize(undo);
				break;
			case 2512:
				Clipping = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
