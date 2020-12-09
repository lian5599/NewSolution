namespace Northwoods.Go
{
	/// <summary>
	/// An enumeration of all of the kinds of simple brushes that <see cref="T:Northwoods.Go.GoShape" /> supports.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is used by the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> property, in coordination
	/// with the <see cref="P:Northwoods.Go.GoShape.BrushColor" />, <see cref="P:Northwoods.Go.GoShape.BrushForeColor" />,
	/// and <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> properties.
	/// </para>
	/// <para>
	/// Note that diagonal linear gradients always go from one corner spot to the opposite
	/// corner spot, following the aspect ratio of the shape, not at 45 or 135 degree angles.
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
	/// </remarks>
	public enum GoBrushStyle
	{
		/// <summary>
		/// Nothing is painted.
		/// If you do not want <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> to fill the region inside a shape,
		/// it is most efficient to set <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.Brush" /> to null or Nothing.
		/// </summary>
		None = 0,
		/// <summary>
		/// The default brush style, a uniform color using a <b>SolidBrush</b>;
		/// the color that is drawn may be partially or totally transparent.
		/// You can specify the color by setting <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />.
		/// Alternatively, you can set the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.Brush" />
		/// to an instance of a <b>System.Drawing.SolidBrush</b>.
		/// </summary>
		Solid = 1,
		/// <summary>
		/// A simple two color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom.
		/// </summary>
		SimpleGradientVertical = 2,
		/// <summary>
		/// A simple two color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the left
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the right.
		/// </summary>
		SimpleGradientHorizontal = 3,
		/// <summary>
		/// A simple two color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-left
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom-right.
		/// </summary>
		SimpleGradientForwardDiagonal = 4,
		/// <summary>
		/// A simple two color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-right
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom-left.
		/// </summary>
		SimpleGradientBackwardDiagonal = 5,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> at the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		MiddleGradientVertical = 6,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the left,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> at the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the right.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		MiddleGradientHorizontal = 7,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-left,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> at the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom-right.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		MiddleGradientForwardDiagonal = 8,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-right,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> at the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom-left.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		MiddleGradientBackwardDiagonal = 9,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the top,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the bottom.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		SingleEdgeGradientTop = 10,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the left,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the right.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		SingleEdgeGradientLeft = 11,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the right,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the left.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		SingleEdgeGradientRight = 12,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the bottom,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in the middle,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the top.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		SingleEdgeGradientBottom = 13,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the top and the bottom,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the center, and
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in between.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		DoubleEdgeGradientVertical = 14,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the left and the right sides,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the center, and
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in between.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		DoubleEdgeGradientHorizontal = 0xF,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-left and bottom-right corners,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the center, and
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in between.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		DoubleEdgeGradientForwardDiagonal = 0x10,
		/// <summary>
		/// A three color linear gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> at the top-right and bottom-left corners,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the center, and
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in between.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// </summary>
		DoubleEdgeGradientBackwardDiagonal = 17,
		/// <summary>
		/// A two color path gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the edges of the shape,
		/// gradually turning into the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the very middle.
		/// This gradient works best with convex shapes.
		/// This should not be used for very large shapes; in some cases the system will automatically
		/// use a simple solid brush instead.
		/// </summary>
		ShapeSimpleGradient = 18,
		/// <summary>
		/// A three color path gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the edges of the shape,
		/// and with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> and
		/// <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> in most of the middle.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// This gradient works best with convex shapes.
		/// This should not be used for very large shapes; in some cases the system will automatically
		/// use a simple solid brush instead.
		/// </summary>
		ShapeFringeGradient = 19,
		/// <summary>
		/// A two color path gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> in most of the shape,
		/// but with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> near the top-left corner.
		/// This gradient works best with convex shapes.
		/// This should not be used for very large shapes; in some cases the system will automatically
		/// use a simple solid brush instead.
		/// </summary>
		ShapeHighlightGradient = 20,
		/// <summary>
		/// A three color path gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the edges of a rectangle
		/// whose Bounds are the same as the shape,
		/// gradually turning into the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the very middle.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// This has the same effect as <see cref="F:Northwoods.Go.GoBrushStyle.ShapeSimpleGradient" /> when the shape is a rectangle,
		/// but when the shape is not a rectangle, the brush continues to paint a rectangular gradient.
		/// This should not be used for very large shapes; in some cases the system will automatically
		/// use a simple solid brush instead.
		/// </summary>
		RectangleGradient = 21,
		/// <summary>
		/// A three color path gradient,
		/// with the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> along the edges of an ellipse
		/// whose Bounds are the same as the shape,
		/// gradually turning into the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the very middle.
		/// If the <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> is <b>Color.Empty</b>, it uses the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> at the middle.
		/// This has the same effect as <see cref="F:Northwoods.Go.GoBrushStyle.ShapeSimpleGradient" /> when the shape is an ellipse,
		/// but when the shape is not an ellipse, the brush continues to paint an elliptical gradient.
		/// This should not be used for very large shapes; in some cases the system will automatically
		/// use a simple solid brush instead.
		/// </summary>
		EllipseGradient = 22,
		/// <summary>
		/// A pattern of tiled images, using a <b>TextureBrush</b>.
		/// You can set the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.Brush" />
		/// to an instance of a <b>System.Drawing.TextureBrush</b>.
		/// </summary>
		Texture = 253,
		/// <summary>
		/// A <b>LinearGradientBrush</b> not constructed by setting the various <see cref="T:Northwoods.Go.GoShape" />.<b>Brush...</b>
		/// properties or calling the various <see cref="T:Northwoods.Go.GoShape" />.<b>Fill...</b> methods.
		/// Such a brush is not likely to be serializable, and will need to be reconstructed dynamically.
		/// </summary>
		CustomLinearGradient = 254,
		/// <summary>
		/// A <b>PathGradientBrush</b> not constructed by setting the various <see cref="T:Northwoods.Go.GoShape" />.<b>Brush...</b>
		/// properties or calling the various <see cref="T:Northwoods.Go.GoShape" />.<b>Fill...</b> methods.
		/// Such a brush is not likely to be serializable, and will need to be reconstructed dynamically.
		/// </summary>
		CustomPathGradient = 0xFF,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchHorizontal = 0x100,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchVertical = 257,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchForwardDiagonal = 258,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchBackwardDiagonal = 259,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchCross = 260,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDiagonalCross = 261,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent05 = 262,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent10 = 263,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent20 = 264,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent25 = 265,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent30 = 266,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent40 = 267,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent50 = 268,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent60 = 269,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent70 = 270,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent75 = 271,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent80 = 272,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPercent90 = 273,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLightDownwardDiagonal = 274,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLightUpwardDiagonal = 275,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDarkDownwardDiagonal = 276,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDarkUpwardDiagonal = 277,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchWideDownwardDiagonal = 278,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchWideUpwardDiagonal = 279,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLightVertical = 280,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLightHorizontal = 281,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchNarrowVertical = 282,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchNarrowHorizontal = 283,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDarkVertical = 284,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDarkHorizontal = 285,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDashedDownwardDiagonal = 286,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDashedUpwardDiagonal = 287,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDashedHorizontal = 288,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDashedVertical = 289,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchSmallConfetti = 290,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLargeConfetti = 291,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchZigZag = 292,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchWave = 293,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDiagonalBrick = 294,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchHorizontalBrick = 295,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchWeave = 296,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchPlaid = 297,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDivot = 298,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDottedGrid = 299,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchDottedDiamond = 300,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchShingle = 301,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchTrellis = 302,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchSphere = 303,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchSmallGrid = 304,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchSmallCheckerBoard = 305,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchLargeCheckerBoard = 306,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchOutlinedDiamond = 307,
		/// <summary>
		/// A hatch pattern, using a <b>HatchBrush</b>,
		/// that uses the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushForeColor" />.
		/// </summary>
		HatchSolidDiamond = 308
	}
}
