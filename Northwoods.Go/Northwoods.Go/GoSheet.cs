using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace Northwoods.Go
{
	/// <summary>
	/// This class is an object that represents a sheet of paper, normally only used
	/// as a background <see cref="P:Northwoods.Go.GoView.Sheet" /> in a <see cref="T:Northwoods.Go.GoView" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A sheet also supports a <see cref="P:Northwoods.Go.GoSheet.Grid" /> and an optional <see cref="P:Northwoods.Go.GoSheet.BackgroundImage" />
	/// that you can set.  It also can paint the margins to let the user know where the
	/// margins are.
	/// </para>
	/// <para>
	/// For WinForms, you will need to set the margins appropriately, given
	/// the current page settings for your application.
	/// Given a <c>PageSettings</c> object you can call <c>UpdateBounds</c>
	/// to adjust the sizes of the <see cref="P:Northwoods.Go.GoSheet.Paper" /> and the margins.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoSheet : GoGroup
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 3101;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 3102;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.BackgroundImageSpot" /> property.
		/// </summary>
		public const int ChangedBackgroundImageSpot = 3103;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.ShowsMargins" /> property.
		/// </summary>
		public const int ChangedShowsMargins = 3104;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.MarginColor" /> property.
		/// </summary>
		public const int ChangedMarginColor = 3105;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.Paper" /> property.
		/// </summary>
		public const int ChangedPaper = 3110;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.BackgroundImage" /> property.
		/// </summary>
		public const int ChangedBackgroundImage = 3111;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSheet.Grid" /> property.
		/// </summary>
		public const int ChangedGrid = 3112;

		internal static readonly Color DefaultMarginColor = Color.FromArgb(64, Color.LightGray);

		private GoRectangle myPaper;

		private GoImage myBackgroundImage;

		private GoGrid myGrid;

		private GoRectangle myLeft;

		private GoRectangle myRight;

		private GoRectangle myTop;

		private GoRectangle myBottom;

		private SizeF myTopLeftMargin = new SizeF(50f, 50f);

		private SizeF myBottomRightMargin = new SizeF(50f, 50f);

		private int myBackgroundImageSpot = 1;

		private bool myShowsMargins = true;

		private Color myMarginColor = DefaultMarginColor;

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoRectangle" /> object that provides
		/// the sheet of paper appearance.
		/// </summary>
		public virtual GoRectangle Paper
		{
			get
			{
				return myPaper;
			}
			set
			{
				GoRectangle goRectangle = myPaper;
				if (goRectangle != value)
				{
					if (goRectangle != null)
					{
						Remove(goRectangle);
					}
					myPaper = value;
					if (value != null)
					{
						InsertBefore(null, value);
					}
					Changed(3110, 0, goRectangle, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoImage" /> that decorates the paper.
		/// </summary>
		/// <value>Initially this is null</value>
		/// <remarks>
		/// If you set this property, you may want to initialize the <see cref="T:Northwoods.Go.GoImage" />
		/// with not only the Name/Index/Image, but also by setting <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> to false.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoSheet.BackgroundImageSpot" />
		public virtual GoImage BackgroundImage
		{
			get
			{
				return myBackgroundImage;
			}
			set
			{
				GoImage goImage = myBackgroundImage;
				if (goImage == value)
				{
					return;
				}
				if (goImage != null)
				{
					Remove(goImage);
				}
				myBackgroundImage = value;
				if (value != null)
				{
					if (Paper != null)
					{
						InsertAfter(Paper, value);
					}
					else
					{
						InsertBefore(null, value);
					}
				}
				Changed(3111, 0, goImage, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets or sets the spot at which the <see cref="P:Northwoods.Go.GoSheet.BackgroundImage" /> is
		/// located by <see cref="M:Northwoods.Go.GoSheet.LayoutChildren(Northwoods.Go.GoObject)" />, relative to the <see cref="P:Northwoods.Go.GoSheet.MarginBounds" />.
		/// </summary>
		/// <value>The default value is <c>GoObject.Middle</c></value>
		/// <seealso cref="P:Northwoods.Go.GoSheet.BackgroundImage" />
		[Category("Appearance")]
		[DefaultValue(1)]
		[Description("The spot at which the BackgroundImage is located relative to the MarginBounds")]
		public virtual int BackgroundImageSpot
		{
			get
			{
				return myBackgroundImageSpot;
			}
			set
			{
				int num = myBackgroundImageSpot;
				if (num != value)
				{
					myBackgroundImageSpot = value;
					Changed(3103, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoGrid" /> that provides both
		/// the sheet of paper appearance and the grid appearance and behavior.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The GoGrid that covers this sheet")]
		public virtual GoGrid Grid
		{
			get
			{
				return myGrid;
			}
			set
			{
				GoGrid goGrid = myGrid;
				if (goGrid == value)
				{
					return;
				}
				if (goGrid != null)
				{
					Remove(goGrid);
				}
				myGrid = value;
				if (value != null)
				{
					GoObject goObject = BackgroundImage;
					if (goObject == null)
					{
						goObject = Paper;
					}
					if (goObject != null)
					{
						InsertAfter(goObject, value);
					}
					else
					{
						InsertBefore(null, value);
					}
				}
				Changed(3112, 0, goGrid, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets the bounding rectangle of the area inside the margins.
		/// </summary>
		/// <value>
		/// This just returns the <see cref="P:Northwoods.Go.GoSheet.Paper" />'s bounds after the margins are subtracted all around.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		[Category("Appearance")]
		[Description("The bounds of the rectangle inside the margins")]
		public virtual RectangleF MarginBounds
		{
			get
			{
				GoObject goObject = Paper;
				if (goObject == null)
				{
					goObject = this;
				}
				SizeF topLeftMargin = TopLeftMargin;
				SizeF bottomRightMargin = BottomRightMargin;
				if (topLeftMargin.Width + bottomRightMargin.Width > goObject.Width)
				{
					topLeftMargin.Width = goObject.Width / 2f;
					bottomRightMargin.Width = goObject.Width / 2f;
				}
				if (topLeftMargin.Height + bottomRightMargin.Height > goObject.Height)
				{
					topLeftMargin.Height = goObject.Height / 2f;
					bottomRightMargin.Height = goObject.Height / 2f;
				}
				return new RectangleF(goObject.Left + Math.Min(topLeftMargin.Width, goObject.Width / 2f), goObject.Top + Math.Min(topLeftMargin.Height, goObject.Height / 2f), Math.Max(0f, goObject.Width - topLeftMargin.Width - bottomRightMargin.Width), Math.Max(0f, goObject.Height - topLeftMargin.Height - bottomRightMargin.Height));
			}
		}

		/// <summary>
		/// Gets or sets the size of the margins along the left side and top.
		/// </summary>
		/// <value>
		/// The default value is 50x50.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.ShowsMargins" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.MarginColor" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margins along the left side and top")]
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
					Changed(3101, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of the margins along the right side and bottom.
		/// </summary>
		/// <value>
		/// The default value is 50x50.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.ShowsMargins" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.MarginColor" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margins along the right side and the bottom")]
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
					Changed(3102, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this sheet should shade the marginal area with <see cref="P:Northwoods.Go.GoSheet.MarginColor" />.
		/// </summary>
		/// <value>
		/// By default this is true.
		/// </value>
		/// <remarks>
		/// If you maintain the margin properties to match the user's current printer
		/// page settings, as you can easily do by calling <see cref="M:Northwoods.Go.GoSheet.UpdateBounds(System.Drawing.Printing.PageSettings,System.Single)" />,
		/// then the user will be able to see whether any objects might be clipped
		/// when printed.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.MarginColor" />
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether this paints the marginal areas with the MarginColor")]
		public virtual bool ShowsMargins
		{
			get
			{
				return myShowsMargins;
			}
			set
			{
				bool flag = myShowsMargins;
				if (flag == value)
				{
					return;
				}
				myShowsMargins = value;
				Changed(3104, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					if (myLeft != null)
					{
						myLeft.Visible = value;
						myLeft.Printable = value;
					}
					if (myRight != null)
					{
						myRight.Visible = value;
						myRight.Printable = value;
					}
					if (myTop != null)
					{
						myTop.Visible = value;
						myTop.Printable = value;
					}
					if (myBottom != null)
					{
						myBottom.Visible = value;
						myBottom.Printable = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the color used to shade the margins when <see cref="P:Northwoods.Go.GoSheet.ShowsMargins" /> is true.
		/// </summary>
		/// <value>
		/// This defaults to a mostly transparent LightGray.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoSheet.ShowsMargins" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />
		/// <seealso cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		[Category("Appearance")]
		[Description("The color for the margins, including alpha transparency")]
		public virtual Color MarginColor
		{
			get
			{
				return myMarginColor;
			}
			set
			{
				Color color = myMarginColor;
				if (!(color != value))
				{
					return;
				}
				myMarginColor = value;
				Changed(3105, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					if (myLeft != null)
					{
						myLeft.BrushColor = myMarginColor;
					}
					if (myRight != null)
					{
						myRight.BrushColor = myMarginColor;
					}
					if (myTop != null)
					{
						myTop.BrushColor = myMarginColor;
					}
					if (myBottom != null)
					{
						myBottom.BrushColor = myMarginColor;
					}
				}
			}
		}

		/// <summary>
		/// Constructs a sheet by calling <see cref="M:Northwoods.Go.GoSheet.CreatePaper" />,
		/// <see cref="M:Northwoods.Go.GoSheet.CreateBackgroundImage" />, and <see cref="M:Northwoods.Go.GoSheet.CreateGrid" />.
		/// </summary>
		public GoSheet()
		{
			base.Initializing = true;
			myPaper = CreatePaper();
			Add(myPaper);
			myBackgroundImage = CreateBackgroundImage();
			Add(myBackgroundImage);
			myGrid = CreateGrid();
			Add(myGrid);
			myLeft = MakeBoundary();
			Add(myLeft);
			myRight = MakeBoundary();
			Add(myRight);
			myTop = MakeBoundary();
			Add(myTop);
			myBottom = MakeBoundary();
			Add(myBottom);
			base.Initializing = false;
			LayoutChildren(null);
		}

		private GoRectangle MakeBoundary()
		{
			return new GoRectangle
			{
				Selectable = false,
				AutoRescales = false,
				BrushColor = MarginColor,
				Pen = null
			};
		}

		/// <summary>
		/// Create the <see cref="T:Northwoods.Go.GoRectangle" /> that acts as the sheet's <see cref="P:Northwoods.Go.GoSheet.Paper" />.
		/// </summary>
		/// <returns>a large white shadowed <see cref="T:Northwoods.Go.GoGrid" /> that appears and acts as a simple rectangle</returns>
		/// <remarks>
		/// The default implementation of this method does:
		/// <pre><code>
		///   protected virtual GoRectangle CreatePaper() {
		///    GoGrid grid = new GoGrid();
		///    grid.Selectable = false;
		///    grid.UnboundedSpots = GoObject.NoSpot;
		///    grid.Brush = Brushes.White;
		///    grid.Pen = Pens.Black;
		///    grid.Shadowed = true;
		///    grid.Style = GoViewGridStyle.None;
		///    return grid;
		///  }
		/// </code></pre>
		/// By default this object does not appear or behave as a normal grid, but
		/// just as a simple rectangle.  The <see cref="P:Northwoods.Go.GoSheet.Grid" /> property returns the
		/// grid that is normally manipulated to show any grid lines or to support snapping
		/// behavior.  However, this paper object is normally responsible for drawing any
		/// shadow that this sheet defines.
		/// </remarks>
		protected virtual GoRectangle CreatePaper()
		{
			return new GoGrid
			{
				Selectable = false,
				UnboundedSpots = 0,
				Brush = GoShape.Brushes_White,
				Pen = GoShape.Pens_Black,
				Shadowed = true,
				Style = GoViewGridStyle.None
			};
		}

		/// <summary>
		/// The default sheet has no background <see cref="T:Northwoods.Go.GoImage" />.
		/// </summary>
		/// <returns>This returns null</returns>
		/// <remarks>
		/// You can set the <see cref="P:Northwoods.Go.GoSheet.BackgroundImage" /> to an
		/// initialized <see cref="T:Northwoods.Go.GoImage" /> if you want to display
		/// an image, or you can override this method.
		/// </remarks>
		protected virtual GoImage CreateBackgroundImage()
		{
			return null;
		}

		/// <summary>
		/// Create a <see cref="T:Northwoods.Go.GoGrid" /> object.
		/// </summary>
		/// <returns>the grid that is the primary object in this group</returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///   protected virtual GoGrid CreateGrid() {
		///     GoGrid grid = new GoGrid();
		///     grid.Selectable = false;
		///     grid.CellSize = new SizeF(25, 25);
		///     grid.OriginRelative = false;
		///     grid.LineDashStyle = DashStyle.Custom;
		///     grid.MajorLineColor = Color.LightGray;
		///     grid.MajorLineFrequency = new Size(4, 4);
		///     grid.Style = GoViewGridStyle.None;
		///     return grid;
		///   }
		/// </code>
		/// </example>
		protected virtual GoGrid CreateGrid()
		{
			return new GoGrid
			{
				Selectable = false,
				CellSize = new SizeF(25f, 25f),
				OriginRelative = false,
				LineDashStyle = DashStyle.Custom,
				MajorLineColor = Color.LightGray,
				MajorLineFrequency = new Size(4, 4),
				Style = GoViewGridStyle.None
			};
		}

		/// <summary>
		/// Update the internal reference fields when this object is copied.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoSheet obj = (GoSheet)newgroup;
			obj.myPaper = (GoRectangle)env[myPaper];
			obj.myBackgroundImage = (GoImage)env[myBackgroundImage];
			obj.myGrid = (GoGrid)env[myGrid];
			obj.myLeft = (GoRectangle)env[myLeft];
			obj.myRight = (GoRectangle)env[myRight];
			obj.myTop = (GoRectangle)env[myTop];
			obj.myBottom = (GoRectangle)env[myBottom];
		}

		/// <summary>
		/// Update the internal reference fields when a child object is removed.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myPaper)
			{
				myPaper = null;
				return result;
			}
			if (obj == myBackgroundImage)
			{
				myBackgroundImage = null;
				return result;
			}
			if (obj == myGrid)
			{
				myGrid = null;
				return result;
			}
			if (obj == myLeft)
			{
				myLeft = null;
				return result;
			}
			if (obj == myRight)
			{
				myRight = null;
				return result;
			}
			if (obj == myTop)
			{
				myTop = null;
				return result;
			}
			if (obj == myBottom)
			{
				myBottom = null;
			}
			return result;
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override string FindName(GoObject child)
		{
			if (child == Paper)
			{
				return "Paper";
			}
			if (child == BackgroundImage)
			{
				return "BackgroundImage";
			}
			if (child == Grid)
			{
				return "Grid";
			}
			return base.FindName(child);
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override GoObject FindChild(string name)
		{
			if (name == "Paper")
			{
				return Paper;
			}
			if (name == "BackgroundImage")
			{
				return BackgroundImage;
			}
			if (name == "Grid")
			{
				return Grid;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// The sheet has a black shadow.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <remarks>
		/// The shadow is not painted by this group but by the <see cref="P:Northwoods.Go.GoSheet.Paper" />.
		/// </remarks>
		public override Brush GetShadowBrush(GoView view)
		{
			return GoShape.Brushes_Black;
		}

		/// <summary>
		/// Call this method to update the bounds and the margins of this sheet
		/// when you want it to match the information in a <c>PageSettings</c> object.
		/// </summary>
		/// <param name="ps"></param>
		/// <param name="viewscale">typically this should be the value of <see cref="P:Northwoods.Go.GoView.PrintScale" /></param>
		/// <remarks>
		/// This updates the <see cref="P:Northwoods.Go.GoObject.Bounds" />, <see cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />,
		/// and <see cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />.
		/// If <c>PageSettings.Landscape</c> is true, the PaperSize.Width is used for the <see cref="P:Northwoods.Go.GoSheet.Paper" /> height,
		/// and the PaperSize.Height is used for the page width, after accounting for the <paramref name="viewscale" />.
		/// It does not update any <see cref="T:Northwoods.Go.GoGrid" /> properties.
		/// Note well that this method may raise exceptions when examining the
		/// <c>PageSettings</c> <paramref name="ps" /> and other printer structures,
		/// for example when there is no printer installed.
		/// </remarks>
		public virtual void UpdateBounds(PageSettings ps, float viewscale)
		{
			PaperSize paperSize = ps.PaperSize;
			float num = (float)paperSize.Width / viewscale;
			float num2 = (float)paperSize.Height / viewscale;
			Margins margins = ps.Margins;
			float width = (float)margins.Left / viewscale;
			float height = (float)margins.Top / viewscale;
			float width2 = (float)margins.Right / viewscale;
			float height2 = (float)margins.Bottom / viewscale;
			RectangleF bounds = ps.Landscape ? new RectangleF(0f, 0f, num2, num) : new RectangleF(0f, 0f, num, num2);
			if (Paper != null)
			{
				Paper.Bounds = bounds;
			}
			else
			{
				Bounds = bounds;
			}
			TopLeftMargin = new SizeF(width, height);
			BottomRightMargin = new SizeF(width2, height2);
		}

		/// <summary>
		/// The paint bounds for a sheet depend on the paint bounds of the
		/// <see cref="T:Northwoods.Go.GoGrid" /> child objects, which may be large and
		/// depend on the size of the view.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			InvalidatePaintBounds();
			return base.ExpandPaintBounds(rect, view);
		}

		/// <summary>
		/// This updates any internal structures needed to keep the marginal areas
		/// up-to-date with the <see cref="P:Northwoods.Go.GoSheet.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		/// properties.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// This does nothing if <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true.
		/// If there is a <see cref="P:Northwoods.Go.GoSheet.BackgroundImage" />, it is positioned relative to the <see cref="P:Northwoods.Go.GoSheet.MarginBounds" />
		/// according to the <see cref="P:Northwoods.Go.GoSheet.BackgroundImageSpot" />.
		/// If there is a <see cref="P:Northwoods.Go.GoSheet.Grid" />, it gets the same Bounds as the <see cref="P:Northwoods.Go.GoSheet.Paper" />.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (!base.Initializing)
			{
				base.LayoutChildren(childchanged);
				SizeF topLeftMargin = TopLeftMargin;
				SizeF bottomRightMargin = BottomRightMargin;
				RectangleF marginBounds = MarginBounds;
				if (BackgroundImage != null)
				{
					BackgroundImage.SetSpotLocation(BackgroundImageSpot, GetRectangleSpotLocation(marginBounds, BackgroundImageSpot));
				}
				GoObject goObject = Paper;
				if (goObject == null)
				{
					goObject = this;
				}
				if (Grid != null)
				{
					Grid.Bounds = goObject.Bounds;
				}
				if (topLeftMargin.Width + bottomRightMargin.Width > goObject.Width)
				{
					topLeftMargin.Width = goObject.Width / 2f;
					bottomRightMargin.Width = goObject.Width / 2f;
				}
				if (topLeftMargin.Height + bottomRightMargin.Height > goObject.Height)
				{
					topLeftMargin.Height = goObject.Height / 2f;
					bottomRightMargin.Height = goObject.Height / 2f;
				}
				if (myLeft != null)
				{
					myLeft.Bounds = new RectangleF(marginBounds.X - topLeftMargin.Width, marginBounds.Y, topLeftMargin.Width, marginBounds.Height);
				}
				if (myRight != null)
				{
					myRight.Bounds = new RectangleF(marginBounds.X + marginBounds.Width, marginBounds.Y, bottomRightMargin.Width, marginBounds.Height);
				}
				if (myTop != null)
				{
					myTop.Bounds = new RectangleF(marginBounds.X - topLeftMargin.Width, marginBounds.Y - topLeftMargin.Height, marginBounds.Width + topLeftMargin.Width + bottomRightMargin.Width, topLeftMargin.Height);
				}
				if (myBottom != null)
				{
					myBottom.Bounds = new RectangleF(marginBounds.X - topLeftMargin.Width, marginBounds.Y + marginBounds.Height, marginBounds.Width + topLeftMargin.Width + bottomRightMargin.Width, bottomRightMargin.Height);
				}
			}
		}

		/// <summary>
		/// Implement undo and redo support.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 3101:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 3102:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 3103:
				BackgroundImageSpot = e.GetInt(undo);
				break;
			case 3104:
				ShowsMargins = (bool)e.GetValue(undo);
				break;
			case 3105:
				MarginColor = (Color)e.GetValue(undo);
				break;
			case 3110:
				Paper = (GoGrid)e.GetValue(undo);
				break;
			case 3111:
				BackgroundImage = (GoImage)e.GetValue(undo);
				break;
			case 3112:
				Grid = (GoGrid)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
