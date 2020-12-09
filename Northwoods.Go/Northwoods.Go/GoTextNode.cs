using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A node displaying a string with a background and four ports, one at the
	/// middle of each side.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you don't need all four ports, you can remove ones you don't want by
	/// setting the corresponding property to null.
	/// </para>
	/// <para>
	/// You can change the background from a light-gray rectangle by setting
	/// the <see cref="P:Northwoods.Go.GoTextNode.Background" /> property to be some other object.  In order for
	/// the text to fit nicely within the shape, you may need to set the
	/// <see cref="P:Northwoods.Go.GoTextNode.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoTextNode.BottomRightMargin" /> properties
	/// to larger values.  This is particularly true if the new shape is a
	/// <see cref="T:Northwoods.Go.GoEllipse" /> or a polygon that has a small rectangular area in it.
	/// </para>
	/// <para>
	/// When <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> is false, the <see cref="P:Northwoods.Go.GoTextNode.Label" /> is sized to
	/// fit inside the bounds of the <see cref="P:Northwoods.Go.GoTextNode.Background" />, minus the margins.
	/// The label (a <see cref="T:Northwoods.Go.GoText" />) is automatically wrapped and clipped.
	/// </para>
	/// <para>
	/// To make it easier to initialize or modify the shape's brush and/or pen,
	/// the <see cref="P:Northwoods.Go.GoTextNode.Shape" /> property casts the <see cref="P:Northwoods.Go.GoTextNode.Background" /> to a
	/// <see cref="T:Northwoods.Go.GoShape" />.  If you make the background a group of various
	/// kind of objects, you may want to override the <see cref="P:Northwoods.Go.GoTextNode.Shape" /> property
	/// to return the particular shape inside the group that you consider to be
	/// the most prominent.
	/// </para>
	/// <para>
	/// The text string is normally multiline and not editable, but you can change
	/// those and other properties by setting the <see cref="P:Northwoods.Go.GoTextNode.Label" />'s properties.
	/// </para>
	/// <para>
	/// Setting the <see cref="P:Northwoods.Go.GoTextNode.Shadowed" /> property for this node actually sets that
	/// property on the <see cref="P:Northwoods.Go.GoTextNode.Background" /> object.
	/// Although the <see cref="P:Northwoods.Go.GoTextNode.Background" /> object is normally a <see cref="T:Northwoods.Go.GoRectangle" />,
	/// it could be any kind of <see cref="T:Northwoods.Go.GoObject" />.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoTextNode : GoNode
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2801;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.Background" /> property.
		/// </summary>
		public const int ChangedBackground = 2802;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.TopPort" /> property.
		/// </summary>
		public const int ChangedTopPort = 2803;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.RightPort" /> property.
		/// </summary>
		public const int ChangedRightPort = 2804;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.BottomPort" /> property.
		/// </summary>
		public const int ChangedBottomPort = 2805;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.LeftPort" /> property.
		/// </summary>
		public const int ChangedLeftPort = 2806;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 2807;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 2808;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> property.
		/// </summary>
		public const int ChangedAutoResizes = 2809;

		private const int flagAutoResizes = 16777216;

		private GoText myLabel;

		private GoObject myBack;

		private GoPort myTopPort;

		private GoPort myRightPort;

		private GoPort myBottomPort;

		private GoPort myLeftPort;

		private SizeF myTopLeftMargin = new SizeF(4f, 2f);

		private SizeF myBottomRightMargin = new SizeF(4f, 2f);

		/// <summary>
		/// Giving this node a shadow really means giving the background a shadow.
		/// </summary>
		public override bool Shadowed
		{
			get
			{
				return Background?.Shadowed ?? base.Shadowed;
			}
			set
			{
				GoObject background = Background;
				if (background != null)
				{
					background.Shadowed = value;
				}
				else
				{
					base.Shadowed = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the top and left sides of the text label
		/// with the top and left sides of the background.
		/// </summary>
		/// <remarks>
		/// If you change the background to be a different shape by overriding
		/// <see cref="M:Northwoods.Go.GoTextNode.CreateBackground" />, you will probably want to set this margin
		/// and <see cref="P:Northwoods.Go.GoTextNode.BottomRightMargin" /> appropriately so that the text fits.
		/// </remarks>
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
				if (sizeF != value)
				{
					myTopLeftMargin = value;
					Changed(2807, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the bottom and right sides of the text label
		/// with the bottom and right sides of the background.
		/// </summary>
		/// <remarks>
		/// If you change the background to be a different shape by overriding
		/// <see cref="M:Northwoods.Go.GoTextNode.CreateBackground" />, you will probably want to set this margin
		/// and <see cref="P:Northwoods.Go.GoTextNode.TopLeftMargin" /> appropriately so that the text fits.
		/// </remarks>
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
				if (sizeF != value)
				{
					myBottomRightMargin = value;
					Changed(2808, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the size of the background is changed as the text label's size changes.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
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
					Changed(2809, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						OnAutoResizesChanged(flag);
					}
				}
			}
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> is false, this returns the <see cref="P:Northwoods.Go.GoTextNode.Background" />
		/// object; otherwise this just returns the node itself.
		/// </summary>
		public override GoObject SelectionObject
		{
			get
			{
				GoObject background = Background;
				if (background != null && !AutoResizes)
				{
					return background;
				}
				return this;
			}
		}

		/// <summary>
		/// Display a string and participate in standard textual node searches and editing.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the label.
		/// If non-null, the new text object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the label after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreateLabel" /> method.
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
					Changed(2801, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the background object.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the background object.
		/// If non-null, the new object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the background after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreateBackground" /> method.
		/// The new background object will have its
		/// Selectable and Shadowed properties copied from the old background object.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject Background
		{
			get
			{
				return myBack;
			}
			set
			{
				GoObject goObject = myBack;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					if (value != null)
					{
						value.Selectable = goObject.Selectable;
						value.Shadowed = goObject.Shadowed;
					}
					Remove(goObject);
				}
				myBack = value;
				if (value != null)
				{
					InsertBefore(null, value);
				}
				Changed(2802, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets this node's background shape, assuming it is a <see cref="T:Northwoods.Go.GoShape" />, as it usually is.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape => Background as GoShape;

		/// <summary>
		/// This convenience property exposes the <c>Pen</c> of the <see cref="P:Northwoods.Go.GoTextNode.Shape" />.
		/// </summary>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoTextNode.Background" /> object is null or is not a <see cref="T:Northwoods.Go.GoShape" />
		/// the value will be null and setting the value has no effect.
		/// </remarks>
		[Browsable(false)]
		public Pen Pen
		{
			get
			{
				return Shape?.Pen;
			}
			set
			{
				GoShape shape = Shape;
				if (shape != null)
				{
					shape.Pen = value;
				}
			}
		}

		/// <summary>
		/// This convenience property exposes the <c>Brush</c> of the <see cref="P:Northwoods.Go.GoTextNode.Shape" />.
		/// </summary>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoTextNode.Background" /> object is null or is not a <see cref="T:Northwoods.Go.GoShape" />
		/// the value will be null and setting the value has no effect.
		/// </remarks>
		[Browsable(false)]
		public Brush Brush
		{
			get
			{
				return Shape?.Brush;
			}
			set
			{
				GoShape shape = Shape;
				if (shape != null)
				{
					shape.Brush = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDrawing" />.<see cref="P:Northwoods.Go.GoDrawing.Figure" />
		/// of the <see cref="P:Northwoods.Go.GoTextNode.Background" /> if the background is a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		/// <value>
		/// This returns <see cref="T:Northwoods.Go.GoFigure" />.<see cref="F:Northwoods.Go.GoFigure.None" />
		/// in the typical case where the <see cref="P:Northwoods.Go.GoTextNode.Shape" /> is a <see cref="T:Northwoods.Go.GoRectangle" />
		/// or other non-<see cref="T:Northwoods.Go.GoDrawing" /> shape.
		/// Setting this property has no effect if the <see cref="P:Northwoods.Go.GoTextNode.Shape" /> is not a <see cref="T:Northwoods.Go.GoDrawing" />.
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
		/// Gets or sets the port at the top of the node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreatePort(System.Int32)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort TopPort
		{
			get
			{
				return myTopPort;
			}
			set
			{
				GoPort goPort = myTopPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myTopPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2803, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the port at the right side of the node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreatePort(System.Int32)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort RightPort
		{
			get
			{
				return myRightPort;
			}
			set
			{
				GoPort goPort = myRightPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myRightPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2804, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the port at the bottom of the node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreatePort(System.Int32)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort BottomPort
		{
			get
			{
				return myBottomPort;
			}
			set
			{
				GoPort goPort = myBottomPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myBottomPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2805, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the port at the left of the node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoTextNode.CreatePort(System.Int32)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort LeftPort
		{
			get
			{
				return myLeftPort;
			}
			set
			{
				GoPort goPort = myLeftPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myLeftPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2806, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Create a GoTextNode displaying an empty text string with a rectangular
		/// background and a port at each side.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoTextNode.CreateBackground" />, <see cref="M:Northwoods.Go.GoTextNode.CreateLabel" />,
		/// and <see cref="M:Northwoods.Go.GoTextNode.CreatePort(System.Int32)" /> for each of the four <c>Middle...</c> spots,
		/// to get initial values for <see cref="P:Northwoods.Go.GoTextNode.Background" />, <see cref="P:Northwoods.Go.GoTextNode.Label" />,
		/// <see cref="P:Northwoods.Go.GoTextNode.TopPort" />, <see cref="P:Northwoods.Go.GoTextNode.RightPort" />, <see cref="P:Northwoods.Go.GoTextNode.BottomPort" />,
		/// and <see cref="P:Northwoods.Go.GoTextNode.LeftPort" />.
		/// </remarks>
		public GoTextNode()
		{
			base.InternalFlags &= -17;
			base.InternalFlags |= 16908288;
			myBack = CreateBackground();
			Add(myBack);
			myLabel = CreateLabel();
			Add(myLabel);
			myTopPort = CreatePort(32);
			Add(myTopPort);
			myRightPort = CreatePort(64);
			Add(myRightPort);
			myBottomPort = CreatePort(128);
			Add(myBottomPort);
			myLeftPort = CreatePort(256);
			Add(myLeftPort);
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create a GoTextNode with four ports and a <see cref="T:Northwoods.Go.GoDrawing" /> background,
		/// initialized to have the figure <paramref name="fig" />.
		/// </summary>
		/// <param name="fig">a <see cref="T:Northwoods.Go.GoFigure" /> enumeration value</param>
		/// <remarks>
		/// This constructor basically does the following, but more efficiently:
		/// <pre><code>
		/// GoTextNode n = new GoTextNode();
		/// n.Background = new GoDrawing(fig);
		/// </code></pre>
		/// </remarks>
		public GoTextNode(GoFigure fig)
		{
			base.InternalFlags &= -17;
			base.InternalFlags |= 16908288;
			myBack = new GoDrawing(fig)
			{
				Selectable = false,
				Resizable = false,
				Reshapable = false,
				Brush = GoShape.Brushes_LightGray
			};
			Add(myBack);
			myLabel = CreateLabel();
			Add(myLabel);
			myTopPort = CreatePort(32);
			Add(myTopPort);
			myRightPort = CreatePort(64);
			Add(myRightPort);
			myBottomPort = CreatePort(128);
			Add(myBottomPort);
			myLeftPort = CreatePort(256);
			Add(myLeftPort);
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create and initialize an object that serves as the background for the text.
		/// </summary>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoRectangle" /> with a light gray <see cref="P:Northwoods.Go.GoShape.Brush" />
		/// </returns>
		/// <remarks>
		/// You may wish to override this to use a different kind of background object,
		/// such as <see cref="T:Northwoods.Go.GoRoundedRectangle" />.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateBackground() {
		///    GoRectangle r = new GoRectangle();
		///    r.Selectable = false;
		///    r.Resizable = false;
		///    r.Reshapable = false;
		///    r.Brush = Brushes.LightGray;
		///    return r;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateBackground()
		{
			return new GoRectangle
			{
				Selectable = false,
				Resizable = false,
				Reshapable = false,
				Brush = GoShape.Brushes_LightGray
			};
		}

		/// <summary>
		/// Create and initialize the text object.
		/// </summary>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoText" /> object, normally supporting multiple lines but
		/// that is not resizable or editable.
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel() {
		///    GoText l = new GoText();
		///    l.Selectable = false;
		///    l.Multiline = true;
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel()
		{
			return new GoText
			{
				Selectable = false,
				Multiline = true
			};
		}

		/// <summary>
		/// Create and initialize a port for a side of this node.
		/// </summary>
		/// <param name="spot"></param>
		/// <returns>
		/// a small <see cref="T:Northwoods.Go.GoPort" /> whose <see cref="P:Northwoods.Go.GoPort.Style" /> is
		/// <see cref="F:Northwoods.Go.GoPortStyle.None" /> supporting both source and
		/// destination links
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoPort CreatePort(int spot) {
		///    GoPort p = new GoPort();
		///    p.Style = GoPortStyle.None;
		///    p.Size = new SizeF(4, 4);
		///    p.FromSpot = spot;
		///    p.ToSpot = spot;
		///    return p;
		///  }
		/// </code>
		/// </example>
		protected virtual GoPort CreatePort(int spot)
		{
			return new GoPort
			{
				Style = GoPortStyle.None,
				Size = new SizeF(4f, 4f),
				FromSpot = spot,
				ToSpot = spot
			};
		}

		/// <summary>
		/// Copy the background, text label, and the four ports.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		/// <remarks>
		/// Remember to override this to copy any objects you add to this class.
		/// </remarks>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoTextNode obj = (GoTextNode)newgroup;
			obj.myBack = (GoObject)env[myBack];
			obj.myLabel = (GoText)env[myLabel];
			obj.myTopPort = (GoPort)env[myTopPort];
			obj.myRightPort = (GoPort)env[myRightPort];
			obj.myBottomPort = (GoPort)env[myBottomPort];
			obj.myLeftPort = (GoPort)env[myLeftPort];
		}

		/// <summary>
		/// If any part is removed from this group,
		/// be sure to remove any references in local fields.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myBack)
			{
				myBack = null;
				return result;
			}
			if (obj == myLabel)
			{
				myLabel = null;
				return result;
			}
			if (obj == myTopPort)
			{
				myTopPort = null;
				return result;
			}
			if (obj == myRightPort)
			{
				myRightPort = null;
				return result;
			}
			if (obj == myBottomPort)
			{
				myBottomPort = null;
				return result;
			}
			if (obj == myLeftPort)
			{
				myLeftPort = null;
			}
			return result;
		}

		/// <summary>
		/// Size the background to fit the text, and position the ports at the edges
		/// of the background object.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// This method uses the <see cref="P:Northwoods.Go.GoTextNode.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoTextNode.BottomRightMargin" />
		/// properties to decide how much bigger the background should be than the text label.
		/// This method does nothing if there is no <see cref="P:Northwoods.Go.GoTextNode.Label" />.
		/// If <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> is false, the <see cref="P:Northwoods.Go.GoTextNode.Background" /> object is
		/// not resized, but the <see cref="P:Northwoods.Go.GoTextNode.Label" />'s bounds and
		/// <see cref="P:Northwoods.Go.GoText.WrappingWidth" /> are updated according to how much room is
		/// left inside the <see cref="P:Northwoods.Go.GoTextNode.Background" /> after subtracting the margins.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoText label = Label;
			if (label == null)
			{
				return;
			}
			GoObject goObject = Background;
			if (goObject != null)
			{
				SizeF topLeftMargin = TopLeftMargin;
				SizeF bottomRightMargin = BottomRightMargin;
				if (AutoResizes)
				{
					goObject.Bounds = new RectangleF(label.Left - topLeftMargin.Width, label.Top - topLeftMargin.Height, label.Width + topLeftMargin.Width + bottomRightMargin.Width, label.Height + topLeftMargin.Height + bottomRightMargin.Height);
				}
				else
				{
					float num = Math.Max(goObject.Width - (topLeftMargin.Width + bottomRightMargin.Width), 0f);
					float num2 = Math.Max(goObject.Height - (topLeftMargin.Height + bottomRightMargin.Height), 0f);
					label.Width = num;
					label.WrappingWidth = num;
					label.UpdateSize();
					float num3 = Math.Min(label.Height, num2);
					float x = goObject.Left + topLeftMargin.Width;
					float y = goObject.Top + topLeftMargin.Height + (num2 - num3) / 2f;
					label.Bounds = new RectangleF(x, y, num, num3);
				}
			}
			if (goObject == null && AutoResizes)
			{
				goObject = label;
			}
			if (goObject != null)
			{
				if (TopPort != null)
				{
					TopPort.SetSpotLocation(32, goObject, 32);
				}
				if (RightPort != null)
				{
					RightPort.SetSpotLocation(64, goObject, 64);
				}
				if (BottomPort != null)
				{
					BottomPort.SetSpotLocation(128, goObject, 128);
				}
				if (LeftPort != null)
				{
					LeftPort.SetSpotLocation(256, goObject, 256);
				}
			}
		}

		/// <summary>
		/// This method is called when the value of <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> changes.
		/// </summary>
		/// <remarks>
		/// This just changes the <see cref="P:Northwoods.Go.GoTextNode.Label" />'s <see cref="P:Northwoods.Go.GoText.Wrapping" />
		/// and <see cref="P:Northwoods.Go.GoText.Clipping" /> properties to be true when the
		/// <see cref="P:Northwoods.Go.GoTextNode.AutoResizes" /> property is false, and vice-versa.
		/// </remarks>
		public virtual void OnAutoResizesChanged(bool old)
		{
			GoText label = Label;
			if (label != null)
			{
				label.Wrapping = old;
				label.Clipping = old;
				base.PropertiesDelegatedToSelectionObject = old;
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
			if (child == Label)
			{
				return "Label";
			}
			if (child == Background)
			{
				return "Background";
			}
			if (child == TopPort)
			{
				return "TopPort";
			}
			if (child == RightPort)
			{
				return "RightPort";
			}
			if (child == BottomPort)
			{
				return "BottomPort";
			}
			if (child == LeftPort)
			{
				return "LeftPort";
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
			if (name == "Label")
			{
				return Label;
			}
			if (name == "Background")
			{
				return Background;
			}
			if (name == "TopPort")
			{
				return TopPort;
			}
			if (name == "RightPort")
			{
				return RightPort;
			}
			if (name == "BottomPort")
			{
				return BottomPort;
			}
			if (name == "LeftPort")
			{
				return LeftPort;
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
			case 2801:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2802:
				Background = (GoObject)e.GetValue(undo);
				break;
			case 2803:
				TopPort = (GoPort)e.GetValue(undo);
				break;
			case 2804:
				RightPort = (GoPort)e.GetValue(undo);
				break;
			case 2805:
				BottomPort = (GoPort)e.GetValue(undo);
				break;
			case 2806:
				LeftPort = (GoPort)e.GetValue(undo);
				break;
			case 2807:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 2808:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 2809:
				AutoResizes = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
