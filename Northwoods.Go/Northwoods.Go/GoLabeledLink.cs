using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This class provides a standard implementation of <see cref="T:Northwoods.Go.IGoLink" />
	/// as a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" /> with several other decoration objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" /> property is the actual <see cref="T:Northwoods.Go.GoLink" />
	/// that this group uses to implement the link.  If you want to specify
	/// any arrow or highlighting or stroke point information, you should do
	/// so on the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" /> rather than on this class.  However,
	/// for your convenience, many of the <see cref="T:Northwoods.Go.GoLink" /> properties are
	/// available here, delegated to the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />.
	/// </para>
	/// <para>
	/// The labels, <see cref="P:Northwoods.Go.GoLabeledLink.MidLabel" />, <see cref="P:Northwoods.Go.GoLabeledLink.FromLabel" />, and <see cref="P:Northwoods.Go.GoLabeledLink.ToLabel" />,
	/// are normally either null or instances of <see cref="T:Northwoods.Go.GoText" />.  However,
	/// they can be other objects, including ports or groups of objects.
	/// Nevertheless, GoLabeledLink does not support link labels that are links.
	/// Nor may this link's ports be or be part of any of its own labels.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoLabeledLink : GoGroup, IGoLink, IGoGraphPart, IGoIdentifiablePart, IGoRoutable
	{
		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedLink = 1311;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedFromLabel = 1312;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedMidLabel = 1313;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToLabel = 1314;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedFromLabelCentered = 1315;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedMidLabelCentered = 1316;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToLabelCentered = 1317;

		private const int flagFromLabelCentered = 16777216;

		private const int flagMidLabelCentered = 33554432;

		private const int flagToLabelCentered = 67108864;

		private const float DEFAULT_ARROW_LENGTH = 10f;

		private const float DEFAULT_ARROW_SHAFT_LENGTH = 8f;

		private const float DEFAULT_ARROW_WIDTH = 8f;

		private const bool DEFAULT_ARROW_FILLED = true;

		private GoLink myRealLink;

		private GoObject myFromLabel;

		private GoObject myMidLabel;

		private GoObject myToLabel;

		/// <summary>
		/// Returns itself as a <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.
		/// </summary>
		/// <remarks>
		/// This property cannot be set.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.GoObject" />
		[Description("Returns itself as a GoObject.")]
		public GoObject GoObject
		{
			get
			{
				return this;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets an integer value associated with this port.
		/// </summary>
		/// <value>
		/// The initial value is zero.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />
		[DefaultValue(0)]
		[Description("An integer value associated with this port.")]
		public virtual int UserFlags
		{
			get
			{
				return myRealLink.UserFlags;
			}
			set
			{
				myRealLink.UserFlags = value;
			}
		}

		/// <summary>
		/// Gets or sets an object associated with this port.
		/// </summary>
		/// <value>
		/// The initial value is null.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserObject" />
		[DefaultValue(null)]
		[Description("An object associated with this port.")]
		public virtual object UserObject
		{
			get
			{
				return myRealLink.UserObject;
			}
			set
			{
				myRealLink.UserObject = value;
			}
		}

		/// <summary>
		/// Gets or sets the port that the link is coming from.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromPort" />
		[DefaultValue(null)]
		[Description("The port that the link is coming from.")]
		public virtual IGoPort FromPort
		{
			get
			{
				return myRealLink.FromPort;
			}
			set
			{
				myRealLink.FromPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the port that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// <seealso cref="P:Northwoods.Go.GoLabeledLink.FromPort" />
		[DefaultValue(null)]
		[Description("The port that the link is going to.")]
		public virtual IGoPort ToPort
		{
			get
			{
				return myRealLink.ToPort;
			}
			set
			{
				myRealLink.ToPort = value;
			}
		}

		/// <summary>
		/// Gets the node that the link is coming from.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromNode" />
		/// <seealso cref="P:Northwoods.Go.GoLabeledLink.ToNode" />
		[Description("The node that the link is coming from.")]
		public virtual IGoNode FromNode => myRealLink.FromNode;

		/// <summary>
		/// Gets the node that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToNode" />
		/// <seealso cref="P:Northwoods.Go.GoLabeledLink.FromNode" />
		[Description("The node that the link is going to.")]
		public virtual IGoNode ToNode => myRealLink.ToNode;

		/// <summary>
		/// Let the user see and manipulate the real link as if it were the selected object.
		/// </summary>
		public override GoObject SelectionObject => RealLink;

		/// <summary>
		/// Giving this node a shadow really means giving the icon a shadow.
		/// </summary>
		public override bool Shadowed
		{
			get
			{
				return SelectionObject.Shadowed;
			}
			set
			{
				SelectionObject.Shadowed = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoLink" /> object in this group.
		/// </summary>
		/// <remarks>
		/// <c>GoLabeledLink</c> delegates more of the <see cref="T:Northwoods.Go.IGoLink" /> interface
		/// to the value of this property.
		/// The real link also gets the selection handles when this labeled link is selected.
		/// Setting this property removes any previous <c>RealLink</c> value, and then adds the
		/// new value to this group.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLink.AbstractLink" />
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The GoLink object in this group.")]
		public virtual GoLink RealLink
		{
			get
			{
				return myRealLink;
			}
			set
			{
				GoLink goLink = myRealLink;
				if (goLink != value)
				{
					if (goLink != null)
					{
						goLink.AbstractLink = goLink;
						base.Remove(goLink);
					}
					myRealLink = value;
					if (value != null)
					{
						base.Add(value);
						value.AbstractLink = this;
					}
					Changed(1311, 0, goLink, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the label object associated with the source end of the link.
		/// </summary>
		/// <remarks>
		/// Setting this property removes any previous FromLabel value, and then adds the
		/// new value to this group.
		/// The label is positioned by <see cref="M:Northwoods.Go.GoLabeledLink.LayoutChildren(Northwoods.Go.GoObject)" />.
		/// </remarks>
		[Category("Labels")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The label object associated with the source end of the link.")]
		public virtual GoObject FromLabel
		{
			get
			{
				return myFromLabel;
			}
			set
			{
				GoObject goObject = myFromLabel;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					base.Remove(goObject);
				}
				myFromLabel = value;
				if (value != null)
				{
					base.Add(value);
					if (value == MidLabel)
					{
						myMidLabel = null;
						Changed(1313, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
					else if (value == ToLabel)
					{
						myToLabel = null;
						Changed(1314, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
				}
				Changed(1312, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets or sets whether the label at the start of the link is positioned on the stroke
		/// rather than to the side of the stroke.
		/// </summary>
		/// <value>
		/// By default this value is false;
		/// </value>
		[Category("Labels")]
		[DefaultValue(false)]
		[Description("Whether the label at the start (or source end) of the link is positioned on top of the stroke")]
		public virtual bool FromLabelCentered
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
					Changed(1315, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(FromLabel);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the label object associated with the middle of the link.
		/// </summary>
		/// <remarks>
		/// Setting this property removes any previous MidLabel value, and then adds the
		/// new value to this group.
		/// The label is positioned by <see cref="M:Northwoods.Go.GoLabeledLink.LayoutChildren(Northwoods.Go.GoObject)" />.
		/// </remarks>
		[Category("Labels")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The label object associated with the middle of the link.")]
		public virtual GoObject MidLabel
		{
			get
			{
				return myMidLabel;
			}
			set
			{
				GoObject goObject = myMidLabel;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					base.Remove(goObject);
				}
				myMidLabel = value;
				if (value != null)
				{
					base.Add(value);
					if (value == FromLabel)
					{
						myFromLabel = null;
						Changed(1312, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
					else if (value == ToLabel)
					{
						myToLabel = null;
						Changed(1314, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
				}
				Changed(1313, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets or sets whether the label at the middle of the link is positioned on the stroke
		/// rather than to the side of the stroke.
		/// </summary>
		/// <value>
		/// By default this value is false;
		/// </value>
		[Category("Labels")]
		[DefaultValue(false)]
		[Description("Whether the label at the middle of the link is positioned on top of the stroke")]
		public virtual bool MidLabelCentered
		{
			get
			{
				return (base.InternalFlags & 0x2000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x2000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 33554432;
					}
					else
					{
						base.InternalFlags &= -33554433;
					}
					Changed(1316, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(MidLabel);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the label object associated with the destination end of the link.
		/// </summary>
		/// <remarks>
		/// Setting this property removes any previous ToLabel value, and then adds the
		/// new value to this group.
		/// The label is positioned by <see cref="M:Northwoods.Go.GoLabeledLink.LayoutChildren(Northwoods.Go.GoObject)" />.
		/// </remarks>
		[Category("Labels")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The label object associated with the destination end of the link.")]
		public virtual GoObject ToLabel
		{
			get
			{
				return myToLabel;
			}
			set
			{
				GoObject goObject = myToLabel;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					base.Remove(goObject);
				}
				myToLabel = value;
				if (value != null)
				{
					base.Add(value);
					if (value == MidLabel)
					{
						myMidLabel = null;
						Changed(1313, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
					else if (value == FromLabel)
					{
						myFromLabel = null;
						Changed(1312, 0, value, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
				}
				Changed(1314, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets or sets whether the label at the end of the link is positioned on the stroke
		/// rather than to the side of the stroke.
		/// </summary>
		/// <value>
		/// By default this value is false;
		/// </value>
		[Category("Labels")]
		[DefaultValue(false)]
		[Description("Whether the label at the destination end of the link is positioned on top of the stroke")]
		public virtual bool ToLabelCentered
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x4000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 67108864;
					}
					else
					{
						base.InternalFlags &= -67108865;
					}
					Changed(1317, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(ToLabel);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.ToolTipText" /> property.
		/// </summary>
		/// <remarks>
		/// If you want to generate the tooltip text string dynamically,
		/// override <see cref="M:Northwoods.Go.GoLabeledLink.GetToolTip(Northwoods.Go.GoView)" />.
		/// </remarks>
		[Description("A string to be displayed in a tooltip.")]
		public string ToolTipText
		{
			get
			{
				return RealLink.ToolTipText;
			}
			set
			{
				RealLink.ToolTipText = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoShape.Pen" /> property.
		/// </summary>
		[Browsable(false)]
		public Pen Pen
		{
			get
			{
				return RealLink.Pen;
			}
			set
			{
				RealLink.Pen = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoShape.PenColor" /> property.
		/// </summary>
		[Category("Appearance")]
		[Description("The color of the pen used to draw the stroke.")]
		public Color PenColor
		{
			get
			{
				return RealLink.PenColor;
			}
			set
			{
				RealLink.PenColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoShape.PenWidth" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to draw the stroke.")]
		public float PenWidth
		{
			get
			{
				return RealLink.PenWidth;
			}
			set
			{
				RealLink.PenWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.HighlightPenColor" /> property.
		/// </summary>
		[Category("Appearance")]
		[Description("The color of the pen used to highlight the stroke.")]
		public Color HighlightPenColor
		{
			get
			{
				return RealLink.HighlightPenColor;
			}
			set
			{
				RealLink.HighlightPenColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.HighlightPenWidth" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to highlight the stroke.")]
		public float HighlightPenWidth
		{
			get
			{
				return RealLink.HighlightPenWidth;
			}
			set
			{
				RealLink.HighlightPenWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoShape.Brush" /> property.
		/// </summary>
		[Browsable(false)]
		public Brush Brush
		{
			get
			{
				return RealLink.Brush;
			}
			set
			{
				RealLink.Brush = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoShape.Brush" /> property.
		/// </summary>
		[Category("Appearance")]
		[Description("The brush used to fill any arrowhead.")]
		public Color BrushColor
		{
			get
			{
				return RealLink.BrushColor;
			}
			set
			{
				RealLink.BrushColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an Orthogonal link tries to avoid crossing over any nodes.")]
		public bool AvoidsNodes
		{
			get
			{
				return RealLink.AvoidsNodes;
			}
			set
			{
				RealLink.AvoidsNodes = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("The Orthogonal property of the RealLink.")]
		public bool Orthogonal
		{
			get
			{
				return RealLink.Orthogonal;
			}
			set
			{
				RealLink.Orthogonal = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.Relinkable" /> property.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("The Relinkable property of the RealLink.")]
		public bool Relinkable
		{
			get
			{
				return RealLink.Relinkable;
			}
			set
			{
				RealLink.Relinkable = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <c>Resizable</c> property.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("The Resizable property of the RealLink.")]
		public override bool Resizable
		{
			get
			{
				return RealLink.Resizable;
			}
			set
			{
				RealLink.Resizable = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <c>Reshapable</c> property.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("The Reshapable property of the RealLink.")]
		public override bool Reshapable
		{
			get
			{
				return RealLink.Reshapable;
			}
			set
			{
				RealLink.Reshapable = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.Style" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(GoStrokeStyle.Line)]
		[Description("The Style property of the RealLink.")]
		public GoStrokeStyle Style
		{
			get
			{
				return RealLink.Style;
			}
			set
			{
				RealLink.Style = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrow" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an arrow is drawn at the end of this stroke.")]
		public bool ToArrow
		{
			get
			{
				return RealLink.ToArrow;
			}
			set
			{
				RealLink.ToArrow = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrowStyle" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(GoStrokeArrowheadStyle.Polygon)]
		[Description("The general shape of the arrowhead at the end of this stroke.")]
		public GoStrokeArrowheadStyle ToArrowStyle
		{
			get
			{
				return RealLink.ToArrowStyle;
			}
			set
			{
				RealLink.ToArrowStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The length of the arrow at the end of this stroke, along the shaft from the end point to the widest point.")]
		public float ToArrowLength
		{
			get
			{
				return RealLink.ToArrowLength;
			}
			set
			{
				RealLink.ToArrowLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The length of the arrow along the shaft at the end of this stroke.")]
		public float ToArrowShaftLength
		{
			get
			{
				return RealLink.ToArrowShaftLength;
			}
			set
			{
				RealLink.ToArrowShaftLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The width of the arrowhead at the widest point.")]
		public float ToArrowWidth
		{
			get
			{
				return RealLink.ToArrowWidth;
			}
			set
			{
				RealLink.ToArrowWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.ToArrowFilled" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the arrowhead is filled with the stroke's brush")]
		public bool ToArrowFilled
		{
			get
			{
				return RealLink.ToArrowFilled;
			}
			set
			{
				RealLink.ToArrowFilled = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrow" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an arrow is drawn at the start of this stroke.")]
		public bool FromArrow
		{
			get
			{
				return RealLink.FromArrow;
			}
			set
			{
				RealLink.FromArrow = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrowStyle" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(GoStrokeArrowheadStyle.Polygon)]
		[Description("The general shape of the arrowhead at the start of this stroke.")]
		public GoStrokeArrowheadStyle FromArrowStyle
		{
			get
			{
				return RealLink.FromArrowStyle;
			}
			set
			{
				RealLink.FromArrowStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The length of the arrowhead at the start of this stroke, along the shaft from the end point to the widest point.")]
		public float FromArrowLength
		{
			get
			{
				return RealLink.FromArrowLength;
			}
			set
			{
				RealLink.FromArrowLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The length of the arrow along the shaft at the start of this stroke.")]
		public float FromArrowShaftLength
		{
			get
			{
				return RealLink.FromArrowShaftLength;
			}
			set
			{
				RealLink.FromArrowShaftLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The width at its widest point of the arrowhead at the start of this stroke.")]
		public float FromArrowWidth
		{
			get
			{
				return RealLink.FromArrowWidth;
			}
			set
			{
				RealLink.FromArrowWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.FromArrowFilled" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the arrowhead is filled with the stroke's brush")]
		public bool FromArrowFilled
		{
			get
			{
				return RealLink.FromArrowFilled;
			}
			set
			{
				RealLink.FromArrowFilled = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.Curviness" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("How rounded corners are for strokes of style RoundedLine")]
		public float Curviness
		{
			get
			{
				return RealLink.Curviness;
			}
			set
			{
				RealLink.Curviness = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.Highlight" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether a highlight is shown along the path of this stroke.")]
		public bool Highlight
		{
			get
			{
				return RealLink.Highlight;
			}
			set
			{
				RealLink.Highlight = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.HighlightPen" /> property.
		/// </summary>
		[Browsable(false)]
		public Pen HighlightPen
		{
			get
			{
				return RealLink.HighlightPen;
			}
			set
			{
				RealLink.HighlightPen = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" /> property.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the highlight is shown when this stroke becomes selected.")]
		public bool HighlightWhenSelected
		{
			get
			{
				return RealLink.HighlightWhenSelected;
			}
			set
			{
				RealLink.HighlightWhenSelected = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" /> property.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(GoLinkAdjustingStyle.Calculate)]
		[Description("How CalculateStroke behaves.")]
		public GoLinkAdjustingStyle AdjustingStyle
		{
			get
			{
				return RealLink.AdjustingStyle;
			}
			set
			{
				RealLink.AdjustingStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLink.PartID" /> property.
		/// </summary>
		[Category("Ownership")]
		[Description("The unique ID of this part in its document.")]
		public virtual int PartID
		{
			get
			{
				if (RealLink == null)
				{
					return -1;
				}
				return RealLink.PartID;
			}
			set
			{
				if (RealLink != null)
				{
					RealLink.PartID = value;
				}
			}
		}

		/// <summary>
		/// The constructor produces a link with no labels that is connected to no ports,
		/// and is not movable by the user.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoLabeledLink.CreateRealLink" />, makes sure the resulting link is not
		/// selectable, and assigns it to <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />.
		/// </remarks>
		public GoLabeledLink()
		{
			base.InternalFlags &= -5;
			GoLink goLink = CreateRealLink();
			if (goLink != null)
			{
				goLink.Selectable = false;
				RealLink = goLink;
			}
		}

		/// <summary>
		/// Create a GoLink to act as the real link for this group that is a labeled link.
		/// </summary>
		/// <returns>
		/// By default this allocates a new <see cref="T:Northwoods.Go.GoLink" />.
		/// </returns>
		/// <remarks>
		/// You may wish to override this in order to customize the initial appearance
		/// of the link or to substitute a subclass of <see cref="T:Northwoods.Go.GoLink" /> for
		/// different behavior.
		/// </remarks>
		public virtual GoLink CreateRealLink()
		{
			return new GoLink();
		}

		/// <summary>
		/// Copying a labeled link is just a matter of copying the real link
		/// and the three label objects.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoLabeledLink obj = (GoLabeledLink)newgroup;
			obj.myRealLink = (GoLink)env[myRealLink];
			obj.myFromLabel = (GoObject)env[myFromLabel];
			obj.myMidLabel = (GoObject)env[myMidLabel];
			obj.myToLabel = (GoObject)env[myToLabel];
		}

		/// <summary>
		/// Make sure we remove any references to child objects that are removed from this group.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == RealLink)
			{
				RealLink = null;
				return result;
			}
			if (obj == FromLabel)
			{
				FromLabel = null;
				return result;
			}
			if (obj == MidLabel)
			{
				MidLabel = null;
				return result;
			}
			if (obj == ToLabel)
			{
				ToLabel = null;
			}
			return result;
		}

		/// <summary>
		/// Avoid repositioning the labels when a labeled link is moved.
		/// </summary>
		/// <param name="old"></param>
		protected override void MoveChildren(RectangleF old)
		{
			bool initializing = base.Initializing;
			base.Initializing = true;
			base.MoveChildren(old);
			base.Initializing = initializing;
		}

		/// <summary>
		/// Return the port at the other end of this link from the given port.
		/// </summary>
		/// <param name="p"></param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoPort" />, that may be null if the other end of the link is
		/// not connected, or that may return the same <paramref name="p" /> if both
		/// ends of the link are connected to the same port.
		/// </returns>
		public IGoPort GetOtherPort(IGoPort p)
		{
			return GoLink.GetOtherPort(this, p);
		}

		/// <summary>
		/// Return the node at the other end of this link from the given node.
		/// </summary>
		/// <param name="n"></param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoNode" />, that may be null if the other end of the
		/// link is not connected or whose port may not be part of a node,
		/// or that may return the same <paramref name="n" /> if both ends of
		/// the link are connected to the same node, even if at different ports.
		/// </returns>
		public IGoNode GetOtherNode(IGoNode n)
		{
			return GoLink.GetOtherNode(this, n);
		}

		/// <summary>
		/// Remove this link from its layer.
		/// </summary>
		public virtual void Unlink()
		{
			base.Layer?.Remove(this);
		}

		/// <summary>
		/// Let links get notifications of changes to either port so labels
		/// can be moved and the real link's stroke can be recalculated.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// Most of the parameters are the same as for <see cref="P:Northwoods.Go.GoLabeledLink.GoObject" />.<c>Changed</c>.
		/// This just calls <see cref="M:Northwoods.Go.GoLink.OnPortChanged(Northwoods.Go.IGoPort,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> on the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />,
		/// and then calls <see cref="M:Northwoods.Go.GoLabeledLink.LayoutChildren(Northwoods.Go.GoObject)" />.
		/// </remarks>
		public virtual void OnPortChanged(IGoPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (RealLink != null)
			{
				RealLink.OnPortChanged(port, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			}
			LayoutChildren(port?.GoObject);
		}

		/// <summary>
		/// Respect the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoStroke.PickMargin" />.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="selectableOnly"></param>
		/// <returns></returns>
		public override GoObject Pick(PointF p, bool selectableOnly)
		{
			RectangleF a = Bounds;
			float num = 0f;
			if (RealLink != null)
			{
				num = Math.Max(0f, RealLink.PickMargin);
			}
			GoObject.InflateRect(ref a, num, num);
			if (!GoObject.ContainsRect(a, p))
			{
				return null;
			}
			if (!CanView())
			{
				return null;
			}
			foreach (GoObject backward in base.Backwards)
			{
				GoObject goObject = backward.Pick(p, selectableOnly);
				if (goObject != null)
				{
					return goObject;
				}
			}
			if (PickableBackground)
			{
				if (!selectableOnly)
				{
					return this;
				}
				if (CanSelect())
				{
					return this;
				}
				for (GoObject parent = base.Parent; parent != null; parent = parent.Parent)
				{
					if (parent.CanSelect())
					{
						return parent;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Position each of the labels.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// This method calls <see cref="M:Northwoods.Go.GoLabeledLink.PositionEndLabel(Northwoods.Go.GoObject,System.Boolean,System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF)" /> for the
		/// <see cref="P:Northwoods.Go.GoLabeledLink.FromLabel" /> and <see cref="P:Northwoods.Go.GoLabeledLink.ToLabel" />, and calls
		/// <see cref="M:Northwoods.Go.GoLabeledLink.LayoutMidLabel(Northwoods.Go.GoObject)" /> which similarly calls
		/// <see cref="M:Northwoods.Go.GoLabeledLink.PositionMidLabel(Northwoods.Go.GoObject,System.Drawing.PointF,System.Drawing.PointF)" /> for the <see cref="P:Northwoods.Go.GoLabeledLink.MidLabel" />.
		/// If there are less than two points in the stroke of the
		/// <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />, this method does nothing.
		/// When <c>Initializing</c> is true, this method does nothing.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoLink realLink = RealLink;
			if (realLink == null)
			{
				return;
			}
			int pointsCount = realLink.PointsCount;
			if (pointsCount < 2)
			{
				return;
			}
			GoObject fromLabel = FromLabel;
			if (fromLabel != null)
			{
				PointF point = realLink.GetPoint(0);
				PointF point2 = realLink.GetPoint(1);
				if (pointsCount == 2)
				{
					PositionEndLabel(fromLabel, atEnd: false, point, point, point2);
				}
				else
				{
					PositionEndLabel(fromLabel, atEnd: false, point, point2, realLink.GetPoint(2));
				}
			}
			LayoutMidLabel(childchanged);
			fromLabel = ToLabel;
			checked
			{
				if (fromLabel != null)
				{
					PointF point3 = realLink.GetPoint(pointsCount - 1);
					PointF point4 = realLink.GetPoint(pointsCount - 2);
					if (pointsCount == 2)
					{
						PositionEndLabel(fromLabel, atEnd: true, point3, point3, point4);
					}
					else
					{
						PositionEndLabel(fromLabel, atEnd: true, point3, point4, realLink.GetPoint(pointsCount - 3));
					}
				}
			}
		}

		/// <summary>
		/// Move a <see cref="P:Northwoods.Go.GoLabeledLink.FromLabel" /> or <see cref="P:Northwoods.Go.GoLabeledLink.ToLabel" /> to be near the link's corresponding end point.
		/// </summary>
		/// <param name="lab">The label object.</param>
		/// <param name="atEnd">Whether the <paramref name="lab" /> is the <see cref="P:Northwoods.Go.GoLabeledLink.ToLabel" />.</param>
		/// <param name="a">The port link point.</param>
		/// <param name="b">The inner point of the end segment.</param>
		/// <param name="c">The inner end point of the second line segment.</param>
		/// <remarks>
		/// Note that if there is no end segment, <paramref name="a" /> and <paramref name="b" /> will be the same <c>PointF</c>.
		/// </remarks>
		protected virtual void PositionEndLabel(GoObject lab, bool atEnd, PointF a, PointF b, PointF c)
		{
			if ((!atEnd && FromLabelCentered) || (atEnd && ToLabelCentered))
			{
				if ((IsApprox(a.X, b.X) && IsApprox(a.Y, b.Y)) || (!IsApprox(a.X, b.X) && !IsApprox(a.Y, b.Y)))
				{
					if (a.X < c.X && Math.Abs(c.Y - a.Y) < c.X - a.X)
					{
						lab.SetSpotLocation(256, a);
					}
					else if (a.X > c.X && Math.Abs(c.Y - a.Y) < a.X - c.X)
					{
						lab.SetSpotLocation(64, a);
					}
					else if (a.Y < c.Y)
					{
						lab.SetSpotLocation(32, a);
					}
					else
					{
						lab.SetSpotLocation(128, a);
					}
				}
				else if (IsApprox(a.X, b.X))
				{
					if (a.Y < b.Y)
					{
						lab.SetSpotLocation(32, a);
					}
					else
					{
						lab.SetSpotLocation(128, a);
					}
				}
				else if (IsApprox(a.Y, b.Y))
				{
					if (a.X < b.X)
					{
						lab.SetSpotLocation(256, a);
					}
					else
					{
						lab.SetSpotLocation(64, a);
					}
				}
				else if (a.X < b.X)
				{
					if (a.Y < b.Y)
					{
						lab.SetSpotLocation(2, a);
					}
					else
					{
						lab.SetSpotLocation(16, a);
					}
				}
				else if (a.Y < b.Y)
				{
					lab.SetSpotLocation(8, a);
				}
				else
				{
					lab.SetSpotLocation(4, a);
				}
				return;
			}
			float num = 2f;
			if ((IsApprox(a.X, b.X) && IsApprox(a.Y, b.Y)) || (!IsApprox(a.X, b.X) && !IsApprox(a.Y, b.Y)))
			{
				if (!atEnd && FromArrow)
				{
					num += FromArrowLength;
				}
				else if (atEnd && ToArrow)
				{
					num += ToArrowLength;
				}
				if (a.X < c.X && Math.Abs(c.Y - a.Y) < c.X - a.X)
				{
					a.X += num;
					if (a.Y < c.Y)
					{
						lab.SetSpotLocation(2, a);
					}
					else
					{
						lab.SetSpotLocation(16, a);
					}
				}
				else if (a.X > c.X && Math.Abs(c.Y - a.Y) < a.X - c.X)
				{
					a.X -= num;
					if (a.Y < c.Y)
					{
						lab.SetSpotLocation(4, a);
					}
					else
					{
						lab.SetSpotLocation(8, a);
					}
				}
				else if (a.Y < c.Y)
				{
					a.Y += num;
					if (a.X < c.X)
					{
						lab.SetSpotLocation(2, a);
					}
					else
					{
						lab.SetSpotLocation(4, a);
					}
				}
				else
				{
					a.Y -= num;
					if (a.X < c.X)
					{
						lab.SetSpotLocation(16, a);
					}
					else
					{
						lab.SetSpotLocation(8, a);
					}
				}
			}
			else if (a.X < b.X)
			{
				a.X += num;
				if (b.Y <= c.Y)
				{
					lab.SetSpotLocation(16, a);
				}
				else
				{
					lab.SetSpotLocation(2, a);
				}
			}
			else if (a.X > b.X)
			{
				a.X -= num;
				if (b.Y <= c.Y)
				{
					lab.SetSpotLocation(8, a);
				}
				else
				{
					lab.SetSpotLocation(4, a);
				}
			}
			else if (a.Y < b.Y)
			{
				a.Y += num;
				if (b.X <= c.X)
				{
					lab.SetSpotLocation(4, a);
				}
				else
				{
					lab.SetSpotLocation(2, a);
				}
			}
			else if (a.Y > b.Y)
			{
				a.Y -= num;
				if (b.X <= c.X)
				{
					lab.SetSpotLocation(8, a);
				}
				else
				{
					lab.SetSpotLocation(16, a);
				}
			}
		}

		/// <summary>
		/// Decide which segment should get the middle label,
		/// and call <see cref="M:Northwoods.Go.GoLabeledLink.PositionMidLabel(Northwoods.Go.GoObject,System.Drawing.PointF,System.Drawing.PointF)" /> to perform that positioning.
		/// </summary>
		/// <param name="childchanged">passed on from <see cref="M:Northwoods.Go.GoLabeledLink.LayoutChildren(Northwoods.Go.GoObject)" /></param>
		/// <remarks>
		/// If there are an odd number of segments in the link's stroke, this calls
		/// <see cref="M:Northwoods.Go.GoLabeledLink.PositionMidLabel(Northwoods.Go.GoObject,System.Drawing.PointF,System.Drawing.PointF)" /> with the endpoints of the middle segment.
		/// Otherwise with an even number of segments, it uses the longer of the
		/// two middle segments.
		/// </remarks>
		protected virtual void LayoutMidLabel(GoObject childchanged)
		{
			GoObject midLabel = MidLabel;
			if (midLabel == null)
			{
				return;
			}
			GoLink realLink = RealLink;
			int pointsCount = realLink.PointsCount;
			if (pointsCount < 2)
			{
				return;
			}
			checked
			{
				if (realLink.Style == GoStrokeStyle.Bezier && pointsCount < 7)
				{
					PointF point = realLink.GetPoint(0);
					PointF point2 = realLink.GetPoint(1);
					PointF point3 = realLink.GetPoint(pointsCount - 2);
					PointF point4 = realLink.GetPoint(pointsCount - 1);
					GoStroke.BezierMidPoint(point, point2, point3, point4, out PointF v, out PointF w);
					PositionMidLabel(midLabel, v, w);
					return;
				}
				int num = unchecked(pointsCount / 2);
				if (unchecked(pointsCount % 2) == 0)
				{
					PointF point5 = realLink.GetPoint(num - 1);
					PointF point6 = realLink.GetPoint(num);
					PositionMidLabel(midLabel, point5, point6);
					return;
				}
				PointF point7 = realLink.GetPoint(num - 1);
				PointF point8 = realLink.GetPoint(num);
				PointF point9 = realLink.GetPoint(num + 1);
				float num2 = point8.X - point7.X;
				float num3 = point8.Y - point7.Y;
				float num4 = point9.X - point8.X;
				float num5 = point9.Y - point8.Y;
				if (num2 * num2 + num3 * num3 >= num4 * num4 + num5 * num5)
				{
					PositionMidLabel(midLabel, point7, point8);
				}
				else
				{
					PositionMidLabel(midLabel, point8, point9);
				}
			}
		}

		/// <summary>
		/// Move the MidLabel to an appropriate location near the middle of the link.
		/// </summary>
		/// <param name="lab">The label object.</param>
		/// <param name="a">The start point of the middle segment of the link.</param>
		/// <param name="b">The end point of the middle segment of the link.</param>
		protected virtual void PositionMidLabel(GoObject lab, PointF a, PointF b)
		{
			PointF newp = new PointF((a.X + b.X) / 2f, (a.Y + b.Y) / 2f);
			int spot = 1;
			if (!MidLabelCentered)
			{
				spot = ((a.X < b.X) ? (IsApprox(a.Y, b.Y) ? 128 : ((!(a.Y < b.Y)) ? 8 : 16)) : (IsApprox(a.Y, b.Y) ? 32 : ((!(a.Y < b.Y)) ? 4 : 2)));
			}
			lab.SetSpotLocation(spot, newp);
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override string FindName(GoObject child)
		{
			if (child == RealLink)
			{
				return "RealLink";
			}
			if (child == FromLabel)
			{
				return "FromLabel";
			}
			if (child == MidLabel)
			{
				return "MidLabel";
			}
			if (child == ToLabel)
			{
				return "ToLabel";
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
			if (name == "RealLink")
			{
				return RealLink;
			}
			if (name == "FromLabel")
			{
				return FromLabel;
			}
			if (name == "MidLabel")
			{
				return MidLabel;
			}
			if (name == "ToLabel")
			{
				return ToLabel;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1311:
				RealLink = (GoLink)e.GetValue(undo);
				break;
			case 1312:
				FromLabel = (GoObject)e.GetValue(undo);
				break;
			case 1313:
				MidLabel = (GoObject)e.GetValue(undo);
				break;
			case 1314:
				ToLabel = (GoObject)e.GetValue(undo);
				break;
			case 1315:
				FromLabelCentered = (bool)e.GetValue(undo);
				break;
			case 1316:
				MidLabelCentered = (bool)e.GetValue(undo);
				break;
			case 1317:
				ToLabelCentered = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}

		/// <summary>
		/// This just calls the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="M:Northwoods.Go.GoLink.CalculateRoute" /> method.
		/// </summary>
		public void CalculateRoute()
		{
			RealLink.CalculateRoute();
		}

		/// <summary>
		/// Request the recalculation of the stroke for the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />.
		/// </summary>
		/// <remarks>
		/// If this is part of a <see cref="T:Northwoods.Go.GoDocument" /> whose <see cref="P:Northwoods.Go.GoDocument.SuspendsRouting" />
		/// property is true, this calls <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.UpdateRoute(Northwoods.Go.IGoRoutable)" /> in order to delay
		/// the call to <see cref="M:Northwoods.Go.GoLabeledLink.CalculateRoute" />.
		/// If there is no <see cref="T:Northwoods.Go.GoDocument" />, this just calls <see cref="M:Northwoods.Go.GoLabeledLink.CalculateRoute" /> immediately.
		/// </remarks>
		public void UpdateRoute()
		{
			GoDocument document = base.Document;
			if (document != null)
			{
				document.UpdateRoute(this);
			}
			else
			{
				CalculateRoute();
			}
		}

		/// <summary>
		/// This just calls the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> method.
		/// </summary>
		public void CalculateStroke()
		{
			RealLink.CalculateStroke();
		}

		/// <summary>
		/// Return a string to be displayed in a tooltip, or null for none.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// By default this returns the <see cref="P:Northwoods.Go.GoLabeledLink.RealLink" />'s <see cref="P:Northwoods.Go.GoLabeledLink.ToolTipText" />.
		/// Override this method if you want dynamically computed tooltips.
		/// </returns>
		public override string GetToolTip(GoView view)
		{
			return ToolTipText;
		}
	}
}
