using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A simple node with a <see cref="P:Northwoods.Go.GoBasicNode.Shape" />, a single <see cref="P:Northwoods.Go.GoBasicNode.Port" /> and an optional text <see cref="P:Northwoods.Go.GoBasicNode.Label" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// By default the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> is a <see cref="T:Northwoods.Go.GoEllipse" />.
	/// You can easily replace that shape with a <see cref="T:Northwoods.Go.GoRectangle" /> or other <see cref="T:Northwoods.Go.GoShape" />.
	/// If you call the <see cref="M:Northwoods.Go.GoBasicNode.#ctor(Northwoods.Go.GoFigure)" /> constructor, the shape will be a <see cref="T:Northwoods.Go.GoDrawing" />
	/// initialized to have that <see cref="T:Northwoods.Go.GoFigure" />.
	/// </para>
	/// <para>
	/// The position and size of the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> relative to the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />
	/// is determined by the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> property.
	/// When the spot is <see cref="F:Northwoods.Go.GoObject.Middle" />, the shape is automatically
	/// resized to fit the label's text (if <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> is true)
	/// plus the <see cref="P:Northwoods.Go.GoBasicNode.MiddleLabelMargin" />,
	/// and the port is hidden and resized to be the same size as the shape itself.
	/// </para>
	/// <para>
	/// When <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> is false, the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> is sized to
	/// fit inside the bounds of the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />, minus the margins.
	/// The label (a <see cref="T:Northwoods.Go.GoText" />) is automatically wrapped and clipped.
	/// </para>
	/// <para>
	/// Since the default value for <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> is <c>GoObject.MiddleTop</c>,
	/// you will need to explicitly set the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> to <c>GoObject.Middle</c>
	/// to get this appearance and behavior.  Otherwise the default appearance is to have the
	/// <see cref="P:Northwoods.Go.GoBasicNode.Label" /> be positioned outside of the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />, with the
	/// <see cref="P:Northwoods.Go.GoBasicNode.Port" /> small and positioned in the middle of the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />.
	/// </para>
	/// <para>
	/// Unless you set the <see cref="P:Northwoods.Go.GoBasicNode.Text" /> or the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> property,
	/// there will be no <see cref="P:Northwoods.Go.GoBasicNode.Label" />.
	/// However, the <see cref="M:Northwoods.Go.GoBasicNode.#ctor(Northwoods.Go.GoFigure)" /> constructor does create a <see cref="P:Northwoods.Go.GoBasicNode.Label" />,
	/// in addition to having the label be in the middle of the shape.
	/// </para>
	/// <para>
	/// When the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> is an instance of <see cref="T:Northwoods.Go.GoDrawing" />,
	/// you can get and set the <see cref="P:Northwoods.Go.GoBasicNode.Figure" /> property, if you want to dynamically
	/// change the appearance of the node.  More generally, you can modify that <see cref="T:Northwoods.Go.GoDrawing" />
	/// or replace the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> with another kind of <see cref="T:Northwoods.Go.GoShape" />.
	/// </para>
	/// <para>
	/// Setting the <see cref="P:Northwoods.Go.GoNode.Location" />, <see cref="P:Northwoods.Go.GoNode.Resizable" />,
	/// <see cref="P:Northwoods.Go.GoNode.Reshapable" /> and <see cref="P:Northwoods.Go.GoNode.Shadowed" /> properties
	/// actually set the same properties on the node's
	/// <see cref="P:Northwoods.Go.GoBasicNode.SelectionObject" />, which is the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />.
	/// </para>
	/// </remarks>
	/// <example>
	/// Typical usage might be something like:
	/// <code>
	/// GoBasicNode node1 = new GoBasicNode();
	/// node1.LabelSpot = GoObject.Middle;
	/// node1.Text = "basic node 1";
	/// node1.Shape.BrushColor = Color.LightGreen;
	/// node1.Location = new PointF(75, 50);
	/// goView1.Document.Add(node1);
	///
	/// GoBasicNode node2 = new GoBasicNode(GoFigure.CreateRequest);
	/// node2.Text = "basic node 2";
	/// node2.Shape.BrushColor = Color.LightYellow;
	/// node2.Location = new PointF(200, 50);
	/// goView1.Document.Add(node2);
	///
	/// GoLink link = new GoLink();
	/// link.ToArrow = true;
	/// link.FromPort = node1.Port;
	/// link.ToPort = node2.Port;
	/// goView1.Document.Add(link);
	/// </code>
	/// </example>
	[Serializable]
	public class GoBasicNode : GoNode
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> property.
		/// </summary>
		public const int ChangedLabelSpot = 2101;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> property.
		/// </summary>
		public const int ChangedShape = 2102;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2103;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.Port" /> property.
		/// </summary>
		public const int ChangedPort = 2104;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.MiddleLabelMargin" /> property.
		/// </summary>
		public const int ChangedMiddleLabelMargin = 2105;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> property.
		/// </summary>
		public const int ChangedAutoResizes = 2106;

		private const int flagAutoResizes = 16777216;

		private static readonly SizeF DefaultPortSize = new SizeF(7f, 7f);

		private static readonly SizeF DefaultShapeMargin = new SizeF(7f, 7f);

		private GoShape myShape;

		private GoText myLabel;

		private GoPort myPort;

		private int myLabelSpot = 32;

		private SizeF myMiddleLabelMargin = new SizeF(20f, 10f);

		/// <summary>
		/// The user appears to select the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />, not the node as a whole.
		/// </summary>
		public override GoObject SelectionObject
		{
			get
			{
				GoObject shape = Shape;
				if (shape != null)
				{
					return shape;
				}
				return this;
			}
		}

		/// <summary>
		/// Gets or sets the spot at which the <see cref="P:Northwoods.Go.GoBasicNode.Label" />, if any, should be positioned
		/// relative to the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />.
		/// </summary>
		/// <remarks>
		/// This calls the virtual method <see cref="M:Northwoods.Go.GoBasicNode.OnLabelSpotChanged(System.Int32)" /> to determine
		/// how to change all of the parts of the node appropriately.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(32)]
		[Description("The spot at which any label is positioned relative to the shape")]
		public virtual int LabelSpot
		{
			get
			{
				return myLabelSpot;
			}
			set
			{
				int num = myLabelSpot;
				if (num != value)
				{
					myLabelSpot = value;
					Changed(2101, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						OnLabelSpotChanged(num);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the how much larger the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> should be on each side of
		/// the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> when the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> is <see cref="F:Northwoods.Go.GoObject.Middle" />.
		/// </summary>
		/// <value>
		/// The default value is a width of 20 and a height of 10, which is adequate for an ellipse
		/// around short text strings.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin of the shape around the label, when the LabelSpot is Middle")]
		public virtual SizeF MiddleLabelMargin
		{
			get
			{
				return myMiddleLabelMargin;
			}
			set
			{
				SizeF sizeF = myMiddleLabelMargin;
				if (sizeF != value)
				{
					myMiddleLabelMargin = value;
					Changed(2105, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the size of the background is changed as the text label's size changes,
		/// when the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> is <c>GoObject.Middle</c>.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// Setting this property will call <see cref="M:Northwoods.Go.GoBasicNode.OnAutoResizesChanged(System.Boolean)" /> with the old value
		/// to determine how to change the properties of the <see cref="T:Northwoods.Go.GoText" /> label.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the background changes size as the text changes")]
		public virtual bool AutoResizes
		{
			get
			{
				return (base.InternalFlags & 0x1000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x1000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 16777216;
					}
					else
					{
						base.InternalFlags &= -16777217;
					}
					Changed(2106, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						OnAutoResizesChanged(flag);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the shape's <see cref="P:Northwoods.Go.GoShape.Pen" /> property.
		/// </summary>
		[Browsable(false)]
		public Pen Pen
		{
			get
			{
				return Shape.Pen;
			}
			set
			{
				Shape.Pen = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />'s <see cref="P:Northwoods.Go.GoShape.Brush" /> property.
		/// </summary>
		[Browsable(false)]
		public Brush Brush
		{
			get
			{
				return Shape.Brush;
			}
			set
			{
				Shape.Brush = value;
			}
		}

		/// <summary>
		/// The text is of course just the <see cref="P:Northwoods.Go.GoBasicNode.Label" />'s text.
		/// </summary>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoBasicNode.Label" />, the value is an empty string.
		/// Setting this property will create and position the label, if needed.
		/// Setting it to a null value will remove the text label from this node.
		/// </remarks>
		public override string Text
		{
			get
			{
				GoText label = Label;
				if (label != null)
				{
					return label.Text;
				}
				return "";
			}
			set
			{
				if (value == null)
				{
					Remove(myLabel);
				}
				else if (Label == null)
				{
					Label = CreateLabel(value);
				}
				else
				{
					Label.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDrawing" />.<see cref="P:Northwoods.Go.GoDrawing.Figure" />
		/// of the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> if the shape is a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		/// <value>
		/// This returns <see cref="T:Northwoods.Go.GoFigure" />.<see cref="F:Northwoods.Go.GoFigure.None" />
		/// in the typical case where the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> is a <see cref="T:Northwoods.Go.GoEllipse" />
		/// or other non-<see cref="T:Northwoods.Go.GoDrawing" /> shape.
		/// Setting this property has no effect if the <see cref="P:Northwoods.Go.GoBasicNode.Shape" /> is not a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoFigure.None)]
		[Description("The GoFigure defining the shape of the Shape, if it is a GoDrawing")]
		public virtual GoFigure Figure
		{
			get
			{
				return (Shape as GoDrawing)?.Figure ?? GoFigure.None;
			}
			set
			{
				GoDrawing goDrawing = Shape as GoDrawing;
				if (goDrawing != null)
				{
					goDrawing.Figure = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoShape" />, the background for this node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the background shape.
		/// </value>
		/// <remarks>
		/// Instead of setting the shape after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoBasicNode.CreateShape(Northwoods.Go.GoPort)" /> method.
		/// The new shape will have its Center location,
		/// Selectable, Resizable, Reshapable, ResizesRealtime, and Shadowed
		/// properties copied from the old shape.
		/// Setting this property will also set the <see cref="P:Northwoods.Go.GoBasicNode.Port" />'s
		/// <see cref="P:Northwoods.Go.GoPort.PortObject" /> to be the new shape.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape
		{
			get
			{
				return myShape;
			}
			set
			{
				GoShape goShape = myShape;
				if (goShape != value)
				{
					CopyPropertiesFromSelectionObject(goShape, value);
					if (goShape != null)
					{
						Remove(goShape);
					}
					myShape = value;
					if (value != null)
					{
						InsertBefore(null, value);
					}
					Changed(2102, 0, goShape, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && Port != null && Port.PortObject == goShape)
					{
						Port.PortObject = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoText" />, the label for this node (or null if there is no label).
		/// </summary>
		/// <value>
		/// Initially this value may be null.  You can set the <see cref="P:Northwoods.Go.GoBasicNode.Text" /> value
		/// to a non-null string in order to create the label.
		/// The new value may be null, to simply remove the label.
		/// If non-null, the new text object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// If you need to use your own subclass of <see cref="T:Northwoods.Go.GoText" />,
		/// instead of setting the label after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoBasicNode.CreateLabel(System.String)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public override GoText Label
		{
			get
			{
				return myLabel;
			}
			set
			{
				GoText goText = myLabel;
				if (goText != value)
				{
					if (goText != null)
					{
						Remove(goText);
					}
					myLabel = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2103, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoPort" /> for this node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Setting this property to a new port will also set that port's
		/// <see cref="P:Northwoods.Go.GoPort.PortObject" /> to be this node's <see cref="P:Northwoods.Go.GoBasicNode.Shape" />,
		/// if it didn't already have a <see cref="P:Northwoods.Go.GoPort.PortObject" />.
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoBasicNode.CreatePort" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort Port
		{
			get
			{
				return myPort;
			}
			set
			{
				GoPort goPort = myPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2104, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && value != null && value.PortObject == null)
					{
						value.PortObject = Shape;
					}
				}
			}
		}

		/// <summary>
		/// Create a GoBasicNode with just a port and a shape centered behind the port,
		/// but no label.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoBasicNode.CreatePort" /> and <see cref="M:Northwoods.Go.GoBasicNode.CreateShape(Northwoods.Go.GoPort)" />
		/// to get initial values for <see cref="P:Northwoods.Go.GoBasicNode.Port" /> and <see cref="P:Northwoods.Go.GoBasicNode.Shape" />.
		/// </remarks>
		public GoBasicNode()
		{
			base.InternalFlags |= 16908288;
			myPort = CreatePort();
			myShape = CreateShape(myPort);
			Add(myShape);
			Add(myPort);
			if (myPort != null)
			{
				myPort.PortObject = myShape;
			}
			base.PropertiesDelegatedToSelectionObject = true;
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create a GoBasicNode with a port and a label and a <see cref="T:Northwoods.Go.GoDrawing" /> shape,
		/// initialized to have the figure <paramref name="fig" /> and a Middle <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" />.
		/// </summary>
		/// <param name="fig">a <see cref="T:Northwoods.Go.GoFigure" /> enumeration value</param>
		/// <remarks>
		/// This constructor basically does the following, but more efficiently:
		/// <pre><code>
		/// GoBasicNode n = new GoBasicNode();
		/// n.LabelSpot = GoObject.Middle;
		/// n.Text = "";
		/// n.Shape = new GoDrawing(fig);
		/// </code></pre>
		/// </remarks>
		public GoBasicNode(GoFigure fig)
		{
			base.InternalFlags |= 16908288;
			myLabelSpot = 1;
			myShape = new GoDrawing(fig);
			myShape.Selectable = false;
			myShape.Resizable = false;
			myShape.Reshapable = false;
			myShape.Brush = GoShape.Brushes_White;
			myPort = CreatePort();
			myPort.Style = GoPortStyle.None;
			myLabel = CreateLabel("");
			Add(myShape);
			Add(myPort);
			Add(myLabel);
			if (myPort != null)
			{
				myPort.PortObject = myShape;
			}
			base.PropertiesDelegatedToSelectionObject = true;
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoPort" />.
		/// </summary>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoPort" /> that is a shape with no particular
		/// link spot for links either coming into or going out of the port
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///   protected virtual GoPort CreatePort() {
		///     GoPort p = new GoPort();
		///     p.Style = GoPortStyle.Ellipse;  // black circle/ellipse
		///     // use custom link spots for both links coming in and going out
		///     p.FromSpot = GoObject.NoSpot;
		///     p.ToSpot = GoObject.NoSpot;
		///     p.Size = new SizeF(7, 7);
		///     return p;
		///   }
		/// </code>
		/// </example>
		protected virtual GoPort CreatePort()
		{
			return new GoPort
			{
				Style = GoPortStyle.Ellipse,
				FromSpot = 0,
				ToSpot = 0,
				Size = DefaultPortSize
			};
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoShape" />.
		/// </summary>
		/// <param name="p">the <see cref="T:Northwoods.Go.GoPort" /> created for this node by <see cref="M:Northwoods.Go.GoBasicNode.CreatePort" /></param>
		/// <returns>
		/// By default this returns a <see cref="T:Northwoods.Go.GoEllipse" /> that is somewhat larger than the port <paramref name="p" />.
		/// </returns>
		/// <remarks>
		/// By default the ellipse uses a white brush--set the <see cref="P:Northwoods.Go.GoBasicNode.Pen" />
		/// and <see cref="P:Northwoods.Go.GoBasicNode.Brush" /> properties to change the appearance.
		/// The shape is not itself selectable, resizable, or reshapable.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoShape CreateShape(GoPort p) {
		///    // create the bigger circle/ellipse around and behind the port
		///    GoShape e = new GoEllipse();
		///    SizeF psize = p.Size;
		///    e.Size = new SizeF(psize.Width + 2*7, psize.Height + 2*7);
		///    e.Selectable = false;
		///    e.Resizable = false;
		///    e.Reshapable = false;
		///    e.Brush = GoShape.Brushes_White;
		///    return e;
		///  }
		/// </code>
		/// </example>
		protected virtual GoShape CreateShape(GoPort p)
		{
			GoEllipse goEllipse = new GoEllipse();
			SizeF size = p.Size;
			goEllipse.Size = new SizeF(size.Width + 2f * DefaultShapeMargin.Width, size.Height + 2f * DefaultShapeMargin.Height);
			goEllipse.Selectable = false;
			goEllipse.Resizable = false;
			goEllipse.Reshapable = false;
			goEllipse.Brush = GoShape.Brushes_White;
			return goEllipse;
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoText" /> as the label.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>
		/// a non-selectable, non-editable, non-rescaling <see cref="T:Northwoods.Go.GoText" />
		/// displaying <paramref name="name" />
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel(String name) {
		///    // create a label with a transparent background that is centered
		///    GoText l = new GoText();
		///    l.Text = name;
		///    l.Selectable = false;
		///    l.Alignment = SpotOpposite(this.LabelSpot);
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel(string name)
		{
			return new GoText
			{
				Text = name,
				Selectable = false,
				Alignment = SpotOpposite(LabelSpot)
			};
		}

		/// <summary>
		/// Make copies of the shape, label and port.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoBasicNode obj = (GoBasicNode)newgroup;
			obj.myShape = (GoShape)env[myShape];
			obj.myPort = (GoPort)env[myPort];
			obj.myLabel = (GoText)env[myLabel];
		}

		/// <summary>
		/// If any part is removed from this group,
		/// be sure to remove any references in local fields.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myShape)
			{
				myShape = null;
				return result;
			}
			if (obj == myLabel)
			{
				myLabel = null;
				return result;
			}
			if (obj == myPort)
			{
				myPort = null;
			}
			return result;
		}

		/// <summary>
		/// The port is centered in the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />; the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> is positioned according
		/// to <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> relative to the shape.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// When the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> is <see cref="F:Northwoods.Go.GoObject.Middle" />
		/// and <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> is true,
		/// we automatically resize the shape to be somewhat larger than the label,
		/// and we size the port to be the same size as the shape.
		/// If <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> is false, the <see cref="P:Northwoods.Go.GoBasicNode.Label" /> is
		/// resized to fit within the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />'s bounds minus the margins.
		/// Also the <see cref="P:Northwoods.Go.GoBasicNode.Label" />'s <see cref="T:Northwoods.Go.GoText" />.<see cref="P:Northwoods.Go.GoText.WrappingWidth" />
		/// is set to the same value as the label's width.
		/// When the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> is not <see cref="F:Northwoods.Go.GoObject.Middle" />,
		/// the label is just positioned relative to the <see cref="P:Northwoods.Go.GoBasicNode.Shape" />
		/// according to the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" />.
		/// This method does nothing if there is no <see cref="P:Northwoods.Go.GoBasicNode.Shape" />.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoShape shape = Shape;
			if (shape == null)
			{
				return;
			}
			GoText label = Label;
			if (label != null)
			{
				if (LabelSpot == 1)
				{
					PointF center = shape.Center;
					SizeF middleLabelMargin = MiddleLabelMargin;
					if (AutoResizes)
					{
						float num = label.Width + middleLabelMargin.Width;
						float num2 = label.Height + middleLabelMargin.Height;
						shape.Bounds = new RectangleF(center.X - num / 2f, center.Y - num2 / 2f, num, num2);
					}
					else
					{
						float num3 = Math.Max(shape.Width - (middleLabelMargin.Width + middleLabelMargin.Width), 0f);
						float num4 = Math.Max(shape.Height - (middleLabelMargin.Height + middleLabelMargin.Height), 0f);
						label.Width = num3;
						label.WrappingWidth = num3;
						label.UpdateSize();
						float num5 = Math.Min(label.Height, num4);
						float x = shape.Left + middleLabelMargin.Width;
						float y = shape.Top + middleLabelMargin.Height + (num4 - num5) / 2f;
						label.Bounds = new RectangleF(x, y, num3, num5);
					}
					label.Center = center;
					if (Port != null)
					{
						Port.Bounds = shape.Bounds;
					}
				}
				else
				{
					label.SetSpotLocation(SpotOpposite(LabelSpot), shape, LabelSpot);
				}
			}
			if (Port != null)
			{
				Port.SetSpotLocation(1, shape, 1);
			}
		}

		/// <summary>
		/// Determine how to change the whole node when the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> changes.
		/// </summary>
		/// <param name="old">the former <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> value</param>
		/// <remarks>
		/// By default, setting the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> to <see cref="F:Northwoods.Go.GoObject.Middle" />
		/// will make the shape big enough to hold the text and the port the same size as
		/// the shape, and will make the node not <see cref="P:Northwoods.Go.GoObject.Resizable" />.
		/// It also changes the port's <see cref="P:Northwoods.Go.GoPort.Style" />
		/// to <see cref="F:Northwoods.Go.GoPortStyle.None" />, so that the port does not obscure the label.
		/// Changing the <see cref="P:Northwoods.Go.GoBasicNode.LabelSpot" /> from <see cref="F:Northwoods.Go.GoObject.Middle" /> to some
		/// other value will reset the shape and port sizes to their initial, default values.
		/// </remarks>
		public virtual void OnLabelSpotChanged(int old)
		{
			if (Port != null)
			{
				if (LabelSpot == 1)
				{
					Port.Style = GoPortStyle.None;
					Resizable = false;
				}
				else if (old == 1)
				{
					Port.Style = GoPortStyle.Ellipse;
					RectangleF bounds = new RectangleF(Shape.Center.X - DefaultPortSize.Width / 2f, Shape.Center.Y - DefaultPortSize.Height / 2f, DefaultPortSize.Width, DefaultPortSize.Height);
					RectangleF bounds2 = new RectangleF(Shape.Center.X - bounds.Width / 2f - DefaultShapeMargin.Width, Shape.Center.Y - bounds.Height / 2f - DefaultShapeMargin.Height, bounds.Width + 2f * DefaultShapeMargin.Width, bounds.Height + 2f * DefaultShapeMargin.Height);
					Shape.Bounds = bounds2;
					Port.Bounds = bounds;
				}
			}
			if (Label != null)
			{
				Label.Alignment = SpotOpposite(LabelSpot);
			}
			LayoutChildren(Label);
		}

		/// <summary>
		/// This method is called when the value of <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> changes.
		/// </summary>
		/// <remarks>
		/// This just changes the <see cref="P:Northwoods.Go.GoBasicNode.Label" />'s <see cref="P:Northwoods.Go.GoText.Wrapping" />
		/// and <see cref="P:Northwoods.Go.GoText.Clipping" /> properties to be true when the
		/// <see cref="P:Northwoods.Go.GoBasicNode.AutoResizes" /> property is false, and vice-versa.
		/// </remarks>
		public virtual void OnAutoResizesChanged(bool old)
		{
			GoText label = Label;
			if (label != null)
			{
				label.Wrapping = old;
				label.Clipping = old;
			}
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override string FindName(GoObject child)
		{
			if (child == Shape)
			{
				return "Shape";
			}
			if (child == Label)
			{
				return "Label";
			}
			if (child == Port)
			{
				return "Port";
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
			if (name == "Shape")
			{
				return Shape;
			}
			if (name == "Label")
			{
				return Label;
			}
			if (name == "Port")
			{
				return Port;
			}
			return base.FindChild(name);
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
			case 2101:
				LabelSpot = e.GetInt(undo);
				break;
			case 2102:
				Shape = (GoShape)e.GetValue(undo);
				break;
			case 2103:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2104:
				Port = (GoPort)e.GetValue(undo);
				break;
			case 2105:
				MiddleLabelMargin = e.GetSize(undo);
				break;
			case 2106:
				AutoResizes = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
