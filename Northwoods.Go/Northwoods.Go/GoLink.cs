using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This class provides a standard implementation of <see cref="T:Northwoods.Go.IGoLink" />
	/// as a <see cref="P:Northwoods.Go.GoLink.GoObject" />.
	/// </summary>
	/// <remarks>
	/// A GoLink is a <see cref="T:Northwoods.Go.GoStroke" /> that connects two <see cref="T:Northwoods.Go.GoPort" />s.
	/// Make a link by constructing a link, setting both the <see ref="FromPort" />
	/// and the <see cref="P:Northwoods.Go.GoLink.ToPort" />, setting any other properties you want on the link,
	/// and adding the link to your <see cref="T:Northwoods.Go.GoDocument" />.
	/// Setting the two port properties will automatically add the link to each port's
	/// collection of links.
	/// Remove a link by calling <see cref="M:Northwoods.Go.GoLink.Unlink" />.
	/// That will automatically disconnect the link from both of its ports.
	/// <para>
	/// A link does not own either port to which it is connected  -- ports are
	/// normally owned by nodes.
	/// Ports hold references to the links that are connected to them -- they do
	/// not own the links.
	/// </para>
	/// <para>
	/// The appearance of a link is determined by the stroke <see cref="P:Northwoods.Go.GoStroke.Style" />,
	/// by the <see cref="P:Northwoods.Go.GoShape.Pen" />, as well as by various link properties.
	/// Any filled arrowheads are painted by the <see cref="P:Northwoods.Go.GoShape.Brush" />.
	/// Many examples are presented in the User Guide.
	/// </para>
	/// <para>
	/// The path of a link is determined by the points in its stroke, as plotted by
	/// <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />.  For links of <see cref="P:Northwoods.Go.GoStroke.Style" />
	/// <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" />, it takes the <see cref="P:Northwoods.Go.GoStroke.Curviness" /> into
	/// account to automatically give the link a curve.
	/// </para>
	/// <para>
	/// When the link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />, it is common to set the style to
	/// <see cref="F:Northwoods.Go.GoStrokeStyle.RoundedLine" />.  Then the <see cref="P:Northwoods.Go.GoStroke.Curviness" />
	/// property controls the size of the corners in the link.
	/// </para>
	/// <para>
	/// Whenever either port is moved (normally because the port's parent node moved),
	/// <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> is called again.  By default this will calculate
	/// all of the stroke points again.  However, if you set the <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" />
	/// property, <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> will take the old path into account
	/// in calculating the new path.
	/// </para>
	/// </remarks>
	/// <example>
	/// Typical programmatic usage might be something like:
	/// <code>
	/// GoBasicNode node1 = new GoBasicNode();
	/// node1.LabelSpot = GoObject.Middle;
	/// node1.Text = "basic node 1";
	/// node1.Shape.BrushColor = Color.LightGreen;
	/// node1.Location = new PointF(75, 50);
	/// goView1.Document.Add(node1);
	///
	/// GoBasicNode node2 = new GoBasicNode();
	/// node2.LabelSpot = GoObject.Middle;
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
	public class GoLink : GoStroke, IGoLink, IGoGraphPart, IGoIdentifiablePart, IGoRoutable
	{
		/// <summary>
		/// This is a special handle ID.
		/// </summary>
		public const int RelinkableFromHandle = 1024;

		/// <summary>
		/// This is a special handle ID.
		/// </summary>
		public const int RelinkableToHandle = 1025;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedLinkUserFlags = 1300;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedLinkUserObject = 1301;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedFromPort = 1302;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToPort = 1303;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedOrthogonal = 1304;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedRelinkable = 1305;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedAbstractLink = 1306;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedAvoidsNodes = 1307;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedPartID = 1309;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedAdjustingStyle = 1310;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToolTipText = 1311;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedDraggableOrthogonalSegments = 1312;

		private const int flagNoClearPorts = 16777216;

		private const int flagLinkAvoidsNodes = 33554432;

		private const int flagLinkOrtho = 67108864;

		private const int flagRelinkable = 134217728;

		private const int flagDraggableOrthogonalSegments = 268435456;

		private const float RIGHT = 0f;

		private const float DOWN = 90f;

		private const float LEFT = 180f;

		private const float UP = 270f;

		private IGoPort myFromPort;

		private IGoPort myToPort;

		private IGoLink myAbstractLink;

		private int myUserFlags;

		private object myUserObject;

		private int myPartID = -1;

		private GoLinkAdjustingStyle myAdjustingStyle;

		private string myToolTipText;

		/// <summary>
		/// Returns itself as a <see cref="P:Northwoods.Go.GoLink.GoObject" />.
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
		/// Gets or sets an integer value associated with this link.
		/// </summary>
		/// <value>
		/// The initial value is zero.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />
		[DefaultValue(0)]
		[Description("An integer value associated with this link.")]
		public virtual int UserFlags
		{
			get
			{
				return myUserFlags;
			}
			set
			{
				int num = myUserFlags;
				if (num != value)
				{
					myUserFlags = value;
					Changed(1300, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets an object associated with this link.
		/// </summary>
		/// <value>
		/// The initial value is null.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserObject" />
		[DefaultValue(null)]
		[Description("An object associated with this link.")]
		public virtual object UserObject
		{
			get
			{
				return myUserObject;
			}
			set
			{
				object obj = myUserObject;
				if (obj != value)
				{
					myUserObject = value;
					Changed(1301, 0, obj, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
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
				return myFromPort;
			}
			set
			{
				IGoPort goPort = myFromPort;
				if (goPort != value)
				{
					IGoLink abstractLink = AbstractLink;
					if (goPort != null && abstractLink.ToPort != goPort)
					{
						goPort.RemoveLink(abstractLink);
					}
					myFromPort = value;
					value?.AddDestinationLink(abstractLink);
					Changed(1302, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						abstractLink.OnPortChanged(value, 1302, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the port that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// <seealso cref="P:Northwoods.Go.GoLink.FromPort" />
		[DefaultValue(null)]
		[Description("The port that the link is going to.")]
		public virtual IGoPort ToPort
		{
			get
			{
				return myToPort;
			}
			set
			{
				IGoPort goPort = myToPort;
				if (goPort != value)
				{
					IGoLink abstractLink = AbstractLink;
					if (goPort != null && abstractLink.FromPort != goPort)
					{
						goPort.RemoveLink(abstractLink);
					}
					myToPort = value;
					value?.AddSourceLink(abstractLink);
					Changed(1303, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						abstractLink.OnPortChanged(value, 1303, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets the node that the link is coming from.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromNode" />
		/// <seealso cref="P:Northwoods.Go.GoLink.ToNode" />
		[Description("The node that the link is coming from.")]
		public virtual IGoNode FromNode => FromPort?.Node;

		/// <summary>
		/// Gets the node that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToNode" />
		/// <seealso cref="P:Northwoods.Go.GoLink.FromNode" />
		[Description("The node that the link is going to.")]
		public virtual IGoNode ToNode => ToPort?.Node;

		/// <summary>
		/// Get the object acting as the <see cref="T:Northwoods.Go.IGoLink" />.
		/// </summary>
		/// <value>
		/// Initially this value will be this <see cref="T:Northwoods.Go.GoLink" /> itself.
		/// Any new value must not be null.
		/// </value>
		/// <remarks>
		/// This property allows a GoLink to be used in the implementation of an IGoLink
		/// without being the IGoLink.  This is important to avoid confusion in classes such as
		/// <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// When the link is copied or removed, it is actually the abstract link that is
		/// copied or removed, not just this GoLink object.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLabeledLink.RealLink" />
		[Description("The object acting as the IGoLink.")]
		public virtual IGoLink AbstractLink
		{
			get
			{
				return myAbstractLink;
			}
			set
			{
				IGoLink goLink = myAbstractLink;
				if (goLink != value && value != null)
				{
					IGoPort fromPort = FromPort;
					fromPort?.RemoveLink(goLink);
					IGoPort toPort = ToPort;
					toPort?.RemoveLink(goLink);
					myAbstractLink = value;
					fromPort?.AddDestinationLink(value);
					toPort?.AddSourceLink(value);
					Changed(1306, 0, goLink, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the segments of the link are always horizontal or vertical.
		/// </summary>
		/// <remarks>
		/// This property primarily controls the behavior of <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />,
		/// to position the points of the stroke so that straight line segments will
		/// be horizontal or vertical.
		/// You can have a link drawn with only horizontal and vertical segments if the
		/// link's stroke style is <see cref="F:Northwoods.Go.GoStrokeStyle.Line" /> or
		/// <see cref="F:Northwoods.Go.GoStrokeStyle.RoundedLine" />.
		/// If the stroke style is <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" />, some of the orthogonally
		/// positioned points act as control points to help form the curve.
		/// The orthogonal <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> algorithm adds two segments to
		/// the normal three segments so that it is possible to connect two ports using only
		/// orthogonal lines.
		/// Changing this value will call <see cref="M:Northwoods.Go.GoLink.PortsOnLinkChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> to notify
		/// both ports that this property changed.
		/// Setting this property to true will clear any existing points and then call
		/// <see cref="M:Northwoods.Go.GoLink.UpdateRoute" />, which will eventually call <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the segments of the link are always horizontal and vertical.")]
		public virtual bool Orthogonal
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x4000000) != 0;
				if (flag == value)
				{
					return;
				}
				if (value)
				{
					base.InternalFlags |= 67108864;
				}
				else
				{
					base.InternalFlags &= -67108865;
				}
				Changed(1304, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					PortsOnLinkChanged(1304, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (value)
					{
						ClearPoints();
						UpdateRoute();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user may reconnect this link to another port.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user may reconnect this link to another port.")]
		public virtual bool Relinkable
		{
			get
			{
				return (base.InternalFlags & 0x8000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x8000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 134217728;
					}
					else
					{
						base.InternalFlags &= -134217729;
					}
					Changed(1305, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user may drag a resizable segment of an Orthogonal link.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// When this is true, a selected link that is <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> will
		/// have unseen resize handles in addition to the standard resize handles at
		/// the points of this stroke.  This will allow the user to drag the segment
		/// horizontally for a vertical segment, or vertically for a horizontal segment.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the user may drag a resizable Orthogonal segment.")]
		public virtual bool DraggableOrthogonalSegments
		{
			get
			{
				return (base.InternalFlags & 0x10000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x10000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 268435456;
					}
					else
					{
						base.InternalFlags &= -268435457;
					}
					Changed(1312, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the link's path tries to avoid other nodes.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// This property is only applicable when the <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> property is true.
		/// A Bezier <see cref="P:Northwoods.Go.GoLink.Style" /> for this link is not recommended.
		/// When this property is true you will incur a noticeable overhead in each call to
		/// <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />.
		/// Changing this value will call <see cref="M:Northwoods.Go.GoLink.PortsOnLinkChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> to notify
		/// both ports that this property changed.
		/// Setting this property to true will clear any existing points and then call
		/// <see cref="M:Northwoods.Go.GoLink.UpdateRoute" />, which will eventually call <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an Orthogonal link tries to avoid crossing over any nodes.")]
		public virtual bool AvoidsNodes
		{
			get
			{
				return (base.InternalFlags & 0x2000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x2000000) != 0;
				if (flag == value)
				{
					return;
				}
				if (value)
				{
					base.InternalFlags |= 33554432;
				}
				else
				{
					base.InternalFlags &= -33554433;
				}
				Changed(1307, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					PortsOnLinkChanged(1307, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (value)
					{
						ClearPoints();
						UpdateRoute();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a string to be displayed in a tooltip.
		/// </summary>
		/// <value>
		/// The initial value is null, which means don't display a tooltip.
		/// </value>
		/// <remarks>
		/// Returning an empty string will display an empty tooltip.
		/// If you want to generate the tooltip text string dynamically,
		/// override <see cref="M:Northwoods.Go.GoLink.GetToolTip(Northwoods.Go.GoView)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />
		[Description("A string to be displayed in a tooltip.")]
		public virtual string ToolTipText
		{
			get
			{
				return myToolTipText;
			}
			set
			{
				string text = myToolTipText;
				if (text != value)
				{
					myToolTipText = value;
					Changed(1311, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Return the index of the first stroke point to get a selection handle.
		/// </summary>
		public override int FirstPickIndex
		{
			get
			{
				GoPort goPort = FromPort as GoPort;
				if (goPort == null)
				{
					return 0;
				}
				if (PointsCount <= 2)
				{
					return 0;
				}
				if (goPort.FromSpot != 0 || Orthogonal)
				{
					return 1;
				}
				return 0;
			}
		}

		/// <summary>
		/// Return the index of the last stroke point to get a selection handle.
		/// </summary>
		public override int LastPickIndex
		{
			get
			{
				int pointsCount = PointsCount;
				if (pointsCount == 0)
				{
					return 0;
				}
				GoPort goPort = ToPort as GoPort;
				checked
				{
					if (goPort == null)
					{
						return pointsCount - 1;
					}
					if (pointsCount <= 2)
					{
						return pointsCount - 1;
					}
					if (goPort.ToSpot != 0 || Orthogonal)
					{
						return pointsCount - 2;
					}
					return pointsCount - 1;
				}
			}
		}

		internal bool NoClearPorts
		{
			get
			{
				return (base.InternalFlags & 0x1000000) != 0;
			}
			set
			{
				if (value)
				{
					base.InternalFlags |= 16777216;
				}
				else
				{
					base.InternalFlags &= -16777217;
				}
			}
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.GoLink.IsSelfLoop" /> is true and <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is false,
		/// the stroke style is assumed to be <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" />.
		/// </summary>
		public override GoStrokeStyle Style
		{
			get
			{
				if (IsSelfLoop && !Orthogonal)
				{
					return GoStrokeStyle.Bezier;
				}
				return base.Style;
			}
			set
			{
				base.Style = value;
			}
		}

		/// <summary>
		/// Return true when both ports are the same.
		/// </summary>
		/// <value>
		/// By default this is true when both ports are the same and non-null.
		/// </value>
		/// <remarks>
		/// This is used by <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> and <see cref="P:Northwoods.Go.GoLink.Style" />.
		/// When this property is true, the link is drawn as a Bezier style
		/// stroke in a loop.
		/// </remarks>
		public virtual bool IsSelfLoop
		{
			get
			{
				if (FromPort == ToPort)
				{
					return FromPort != null;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets or sets how <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> behaves.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Calculate" />.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" /> and <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> are true,
		/// this property is ignored, because <see cref="M:Northwoods.Go.GoLink.CalculateStroke" />
		/// will compute the route in <see cref="M:Northwoods.Go.GoLink.AddOrthoPoints(System.Drawing.PointF,System.Single,System.Drawing.PointF,System.Single)" />
		/// without calling <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(GoLinkAdjustingStyle.Calculate)]
		[Description("How CalculateStroke behaves.")]
		public virtual GoLinkAdjustingStyle AdjustingStyle
		{
			get
			{
				return myAdjustingStyle;
			}
			set
			{
				GoLinkAdjustingStyle goLinkAdjustingStyle = myAdjustingStyle;
				if (goLinkAdjustingStyle != value)
				{
					myAdjustingStyle = value;
					Changed(1310, (int)goLinkAdjustingStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the unique ID of this part within its document.
		/// </summary>
		/// <value>
		/// The value is invalid unless this object is part of a <see cref="T:Northwoods.Go.GoDocument" />
		/// whose <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> property is true.
		/// Setting this property is normally done only by <see cref="T:Northwoods.Go.GoDocument" />
		/// when this object is added to the document, or by code deserializing
		/// a document, before this object is part of the document.
		/// The default value is -1, to indicate to the document that it needs
		/// to find and assign a unique PartID for this object when it is added
		/// to the document.
		/// </value>
		[Category("Ownership")]
		[Description("The unique ID of this part in its document.")]
		public virtual int PartID
		{
			get
			{
				return myPartID;
			}
			set
			{
				int num = myPartID;
				if (num != value)
				{
					myPartID = value;
					Changed(1309, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a link connected to no ports, and that is
		/// not movable by the user, but is relinkable.
		/// </summary>
		public GoLink()
		{
			myAbstractLink = this;
			base.InternalFlags &= -5;
			base.InternalFlags |= 134217728;
		}

		/// <summary>
		/// Copying a link does not immediately set the <see cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// and <see cref="P:Northwoods.Go.IGoLink.ToPort" /> of the copied link,
		/// but does add the link to the copy dictionary's
		/// <see cref="P:Northwoods.Go.GoCopyDictionary.Delayeds" /> collection so that later
		/// processing can set those properties.
		/// </summary>
		/// <param name="env"></param>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoLink goLink = (GoLink)base.CopyObject(env);
			if (goLink != null)
			{
				env.Delayeds.Add(this);
				goLink.myAbstractLink = (IGoLink)env.Copy(myAbstractLink.GoObject);
				goLink.myFromPort = null;
				goLink.myToPort = null;
				goLink.myPartID = -1;
			}
			return goLink;
		}

		/// <summary>
		/// Make sure both ports are newly copied ports, or else remove this link
		/// from the destination layer.
		/// </summary>
		/// <param name="env"></param>
		/// <param name="newobj"></param>
		public override void CopyObjectDelayed(GoCopyDictionary env, GoObject newobj)
		{
			base.CopyObjectDelayed(env, newobj);
			GoLink goLink = newobj as GoLink;
			if (goLink != null)
			{
				IGoPort fromPort = FromPort;
				IGoPort toPort = ToPort;
				IGoPort goPort = env[fromPort] as IGoPort;
				IGoPort goPort2 = env[toPort] as IGoPort;
				IGoLink abstractLink = goLink.AbstractLink;
				if (abstractLink.GoObject.Movable || ((fromPort == null || goPort != null) && (toPort == null || goPort2 != null)))
				{
					goLink.myFromPort = goPort;
					goLink.myToPort = goPort2;
					goPort?.AddDestinationLink(abstractLink);
					goPort2?.AddSourceLink(abstractLink);
				}
				else
				{
					abstractLink.GoObject.Remove();
				}
			}
		}

		/// <summary>
		/// Return the port at the other end of this link from the given port.
		/// </summary>
		/// <param name="p"></param>
		/// <returns>
		/// <c>GoLink.GetOtherPort(this, p)</c>, an <see cref="T:Northwoods.Go.IGoPort" />.
		/// </returns>
		public IGoPort GetOtherPort(IGoPort p)
		{
			return GetOtherPort(this, p);
		}

		/// <summary>
		/// Return the node at the other end of this link from the given node.
		/// </summary>
		/// <param name="n"></param>
		/// <returns>
		/// <c>GoLink.GetOtherNode(this, n)</c>, an <see cref="T:Northwoods.Go.IGoNode" />.
		/// </returns>
		public IGoNode GetOtherNode(IGoNode n)
		{
			return GetOtherNode(this, n);
		}

		/// <summary>
		/// Remove this link from its layer.
		/// </summary>
		/// <remarks>
		/// This method also removes this link from both ports' collections of
		/// connected links, by the implementation of <see cref="M:Northwoods.Go.GoLink.OnLayerChanged(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer,Northwoods.Go.GoObject)" />.
		/// </remarks>
		public virtual void Unlink()
		{
			AbstractLink.GoObject.Remove();
		}

		/// <summary>
		/// Let links get notifications of changes to either port or to the link itself.
		/// </summary>
		/// <param name="port">This <see cref="T:Northwoods.Go.IGoPort" /> may be null for some events on the link.</param>
		/// <param name="subhint">
		/// This integer may include <see cref="F:Northwoods.Go.GoLink.ChangedFromPort" /> and <see cref="F:Northwoods.Go.GoLink.ChangedToPort" />,
		/// stroke changes such as <see cref="F:Northwoods.Go.GoStroke.ChangedAllPoints" /> and <see cref="F:Northwoods.Go.GoStroke.ChangedStyle" />,
		/// as well as some selected <c>GoPort.Changed...</c> subhints.
		/// </param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// In general when this method is called, we call <see cref="M:Northwoods.Go.GoLink.UpdateRoute" />.
		/// Most of the parameters are the same as for <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c>.
		/// This method does nothing when <paramref name="port" /> is null.
		/// </remarks>
		public virtual void OnPortChanged(IGoPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (port == null)
			{
				return;
			}
			switch (subhint)
			{
			case 1702:
				return;
			case 1302:
			case 1303:
				if (oldVal != newVal || AdjustingStyle == GoLinkAdjustingStyle.Calculate || (AdjustingStyle == GoLinkAdjustingStyle.Scale && Orthogonal))
				{
					UpdateRoute();
				}
				PortsOnLinkChanged(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				return;
			case 2211:
				UpdateRoute();
				return;
			}
			GoPort goPort = port.GoObject as GoPort;
			if (goPort != null && subhint == 1703 && goPort.Style == GoPortStyle.Object)
			{
				return;
			}
			if (goPort != null && goPort == FromPort && PointsCount > 0)
			{
				PointF fromLinkPoint = goPort.GetFromLinkPoint(AbstractLink);
				PointF point = GetPoint(0);
				if (!IsApprox(fromLinkPoint.X, point.X) || !IsApprox(fromLinkPoint.Y, point.Y))
				{
					UpdateRoute();
				}
			}
			else if (goPort != null && goPort == ToPort && PointsCount >= 2)
			{
				PointF toLinkPoint = goPort.GetToLinkPoint(AbstractLink);
				PointF point2 = GetPoint(checked(PointsCount - 1));
				if (!IsApprox(toLinkPoint.X, point2.X) || !IsApprox(toLinkPoint.Y, point2.Y))
				{
					UpdateRoute();
				}
			}
			else
			{
				UpdateRoute();
			}
		}

		/// <summary>
		/// Notify both ports that there was a change to this link.
		/// </summary>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.IGoPort.OnLinkChanged(Northwoods.Go.IGoLink,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> on both ports.
		/// Most of the parameters are the same as for <see cref="P:Northwoods.Go.GoLink.GoObject" />.<c>Changed</c>.
		/// </remarks>
		public virtual void PortsOnLinkChanged(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (FromPort != null)
			{
				FromPort.OnLinkChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			}
			if (ToPort != null)
			{
				ToPort.OnLinkChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			}
		}

		/// <summary>
		/// If one of the stroke's points has changed, we notify this link's AbstractLink
		/// about the change, so that it has a chance to recalculate its stroke.
		/// </summary>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// This notification is useful for <see cref="T:Northwoods.Go.GoLabeledLink" /> to be notified about
		/// changes in the stroke so that its labels can be repositioned.
		/// </remarks>
		public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (!base.SuspendsUpdates)
			{
				base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				if (subhint == 1203 || subhint == 1201 || subhint == 1202 || subhint == 1204 || subhint == 1206 || subhint == 1205)
				{
					AbstractLink.OnPortChanged(null, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
					PortsOnLinkChanged(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				}
			}
		}

		/// <summary>
		/// Return a string to be displayed in a tooltip, or null for none.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// By default this returns this link's <see cref="P:Northwoods.Go.GoLink.ToolTipText" />.
		/// Override this method if you want dynamically computed tooltips.
		/// </returns>
		public override string GetToolTip(GoView view)
		{
			return ToolTipText;
		}

		/// <summary>
		/// Return the IGoPort of an IGoLink other than the given one.
		/// </summary>
		/// <param name="l">an <see cref="T:Northwoods.Go.IGoLink" /></param>
		/// <param name="p">an <see cref="T:Northwoods.Go.IGoPort" /> that the link <paramref name="l" /> is connected to</param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoPort" />, that may be null if the other end of the link is
		/// not connected, or that may return the same <paramref name="p" /> if both
		/// ends of the link are connected to the same port.
		/// </returns>
		/// <remarks>
		/// If neither this link's <see cref="P:Northwoods.Go.GoLink.FromPort" /> nor its <see cref="P:Northwoods.Go.GoLink.ToPort" />
		/// is the given port <paramref name="p" />, this will return either the
		/// <see cref="P:Northwoods.Go.GoLink.FromPort" /> or the <see cref="P:Northwoods.Go.GoLink.ToPort" />, whichever is farther away.
		/// Otherwise this will return null.
		/// </remarks>
		public static IGoPort GetOtherPort(IGoLink l, IGoPort p)
		{
			if (l.FromPort == p)
			{
				return l.ToPort;
			}
			if (l.ToPort == p)
			{
				return l.FromPort;
			}
			GoObject goObject = p as GoObject;
			if (goObject == null)
			{
				return null;
			}
			PointF center = goObject.Center;
			GoObject goObject2 = l.FromPort as GoObject;
			GoObject goObject3 = l.ToPort as GoObject;
			float num = 0f;
			float num2 = 0f;
			if (goObject2 != null)
			{
				PointF center2 = goObject2.Center;
				num = (center2.X - center.X) * (center2.X - center.X) + (center2.Y - center.Y) * (center2.Y - center.Y);
			}
			if (goObject3 != null)
			{
				PointF center3 = goObject3.Center;
				num2 = (center3.X - center.X) * (center3.X - center.X) + (center3.Y - center.Y) * (center3.Y - center.Y);
			}
			if (num2 > num)
			{
				return l.ToPort;
			}
			if (num > num2)
			{
				return l.FromPort;
			}
			return null;
		}

		/// <summary>
		/// Return the IGoNode connected to the other end of an IGoLink.
		/// </summary>
		/// <param name="l">an <see cref="T:Northwoods.Go.IGoLink" /></param>
		/// <param name="n">an <see cref="T:Northwoods.Go.IGoNode" /> that the link <paramref name="l" /> is connected to</param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoNode" />, that may be null if the other end of the
		/// link is not connected or whose port may not be part of a node,
		/// or that may return the same <paramref name="n" /> if both ends of
		/// the link are connected to the same node, even if at different ports.
		/// </returns>
		/// <remarks>
		/// If neither this link's <see cref="P:Northwoods.Go.GoLink.FromPort" /> nor its <see cref="P:Northwoods.Go.GoLink.ToPort" />
		/// is part of the given node <paramref name="n" />, this will return null.
		/// </remarks>
		public static IGoNode GetOtherNode(IGoLink l, IGoNode n)
		{
			if (l.FromPort.Node == n)
			{
				return l.ToPort.Node;
			}
			if (l.ToPort.Node == n)
			{
				return l.FromPort.Node;
			}
			return null;
		}

		internal static bool IsOrtho(IGoLink link)
		{
			if (link == null)
			{
				return false;
			}
			return (link as GoLink)?.Orthogonal ?? (link as GoLabeledLink)?.Orthogonal ?? false;
		}

		/// <summary>
		/// Handle resizing in the same manner as <see cref="T:Northwoods.Go.GoStroke" />,
		/// but when <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true, we move both points
		/// of the middle segment in order to maintain orthogonality.
		/// </summary>
		/// <param name="view">
		/// the <see cref="T:Northwoods.Go.GoView" /> whose <see cref="T:Northwoods.Go.GoToolResizing" /> is calling this method
		/// </param>
		/// <param name="origRect">
		/// the original Bounds of the object, but probably not useful for links
		/// </param>
		/// <param name="newPoint">
		/// the PointF, in document coordinates, to which the resize handle is being dragged
		/// </param>
		/// <param name="whichHandle">
		/// The <see cref="T:Northwoods.Go.IGoHandle" />.<see cref="P:Northwoods.Go.IGoHandle.HandleID" /> of the handle being dragged.
		/// Possible values include:
		/// <list type="bullet">
		/// <item><term><c>GoObject.MiddleTop</c></term><description>
		/// movement of this handle near the FromPort is constrained to be in the vertical direction
		/// because this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />
		/// </description></item>
		/// <item><term><c>GoObject.MiddleLeft</c></term><description>
		/// movement of this handle near the FromPort is constrained to be in the horizontal direction
		/// because this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />
		/// </description></item>
		/// <item><term><c>GoObject.MiddleBottom</c></term><description>
		/// movement of this handle near the ToPort is constrained to be in the vertical direction
		/// because this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />
		/// </description></item>
		/// <item><term><c>GoObject.MiddleRight</c></term><description>
		/// movement of this handle near the ToPort is constrained to be in the horizontal direction
		/// because this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />
		/// </description></item>
		/// <item><term><c>GoObject.LastHandle + 1000000</c>, or more</term><description>
		/// move both ends of the segment starting with the point whose index is the value
		/// minus <c>GoObject.LastHandle</c> minus 1000000,
		/// because <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> and <see cref="P:Northwoods.Go.GoLink.DraggableOrthogonalSegments" /> are both true
		/// </description></item>
		/// <item><term><c>GoObject.LastHandle</c> or more</term><description>
		/// set the point whose index is the value minus <c>GoObject.LastHandle</c>
		/// </description></item>
		/// <item><term><c>GoLink.RelinkableFromHandle</c></term><description>
		/// this case is normally handled by <see cref="T:Northwoods.Go.GoToolRelinking" /> rather than by this method
		/// </description></item>
		/// <item><term><c>GoLink.RelinkableToHandle</c></term><description>
		/// this case is normally handled by <see cref="T:Northwoods.Go.GoToolRelinking" /> rather than by this method
		/// </description></item>
		/// <item><term><c>GoObject.NoHandle</c></term><description>
		/// this case usually means that the <see cref="T:Northwoods.Go.GoHandle" /> should not be dragged
		/// </description></item>
		/// </list>
		/// </param>
		/// <param name="evttype">
		/// <list type="bullet">
		/// <item><term><c>GoInputState.Start</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.Start" /></description></item>
		/// <item><term><c>GoInputState.Continue</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseMove" /></description></item>
		/// <item><term><c>GoInputState.Finish</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseUp" /></description></item>
		/// <item><term><c>GoInputState.Cancel</c></term><description>when the <see cref="M:Northwoods.Go.GoToolResizing.DoCancelMouse" /></description></item>
		/// </list>
		/// </param>
		/// <param name="min">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MinimumSize" />,
		/// but probably not useful for links
		/// </param>
		/// <param name="max">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MaximumSize" />,
		/// but probably not useful for links
		/// </param>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (!ResizesRealtime && evttype != GoInputState.Finish && evttype != 0)
			{
				return;
			}
			checked
			{
				int num = FirstPickIndex + 1;
				int num2 = LastPickIndex - 1;
				switch (whichHandle)
				{
				case 32:
					SetPoint(num, new PointF(GetPoint(num - 1).X, newPoint.Y));
					SetPoint(num + 1, new PointF(GetPoint(num + 2).X, newPoint.Y));
					return;
				case 256:
					SetPoint(num, new PointF(newPoint.X, GetPoint(num - 1).Y));
					SetPoint(num + 1, new PointF(newPoint.X, GetPoint(num + 2).Y));
					return;
				case 128:
					SetPoint(num2 - 1, new PointF(GetPoint(num2 - 2).X, newPoint.Y));
					SetPoint(num2, new PointF(GetPoint(num2 + 1).X, newPoint.Y));
					return;
				case 64:
					SetPoint(num2 - 1, new PointF(newPoint.X, GetPoint(num2 - 2).Y));
					SetPoint(num2, new PointF(newPoint.X, GetPoint(num2 + 1).Y));
					return;
				}
				int pointsCount = PointsCount;
				if (pointsCount < 2 || whichHandle < 8192)
				{
					return;
				}
				int num3 = whichHandle - 8192;
				if (num3 > 1000000)
				{
					num3 -= 1000000;
					PointF point = GetPoint(num3);
					if (Orthogonal)
					{
						PointF point2 = GetPoint(num3 - 1);
						PointF point3 = GetPoint(num3 + 1);
						if (IsApprox(point2.X, point.X) && IsApprox(point.Y, point3.Y))
						{
							SetPoint(num3, new PointF(point.X, newPoint.Y));
							SetPoint(num3 + 1, new PointF(point3.X, newPoint.Y));
						}
						else if (IsApprox(point2.Y, point.Y) && IsApprox(point.X, point3.X))
						{
							SetPoint(num3, new PointF(newPoint.X, point.Y));
							SetPoint(num3 + 1, new PointF(newPoint.X, point3.Y));
						}
						else if (IsApprox(point2.X, point.X) && IsApprox(point.X, point3.X))
						{
							SetPoint(num3, new PointF(point.X, newPoint.Y));
							SetPoint(num3 + 1, new PointF(newPoint.X, point3.Y));
						}
						else if (IsApprox(point2.Y, point.Y) && IsApprox(point.Y, point3.Y))
						{
							SetPoint(num3, new PointF(newPoint.X, point.Y));
							SetPoint(num3 + 1, new PointF(point3.X, newPoint.Y));
						}
					}
				}
				else
				{
					PointF point4 = GetPoint(num3);
					if (Orthogonal)
					{
						PointF point5 = GetPoint(num3 - 1);
						PointF point6 = GetPoint(num3 + 1);
						if (IsApprox(point5.X, point4.X) && IsApprox(point4.Y, point6.Y))
						{
							SetPoint(num3 - 1, new PointF(newPoint.X, point5.Y));
							SetPoint(num3 + 1, new PointF(point6.X, newPoint.Y));
						}
						else if (IsApprox(point5.Y, point4.Y) && IsApprox(point4.X, point6.X))
						{
							SetPoint(num3 - 1, new PointF(point5.X, newPoint.Y));
							SetPoint(num3 + 1, new PointF(newPoint.X, point6.Y));
						}
						else if (IsApprox(point5.X, point4.X) && IsApprox(point4.X, point6.X))
						{
							SetPoint(num3 - 1, new PointF(newPoint.X, point5.Y));
							SetPoint(num3 + 1, new PointF(newPoint.X, point6.Y));
						}
						else if (IsApprox(point5.Y, point4.Y) && IsApprox(point4.Y, point6.Y))
						{
							SetPoint(num3 - 1, new PointF(point5.X, newPoint.Y));
							SetPoint(num3 + 1, new PointF(point6.X, newPoint.Y));
						}
					}
					SetPoint(num3, newPoint);
				}
				if (pointsCount < 3)
				{
					return;
				}
				if (num3 == 1 && FromPort != null)
				{
					GoPort goPort = FromPort.GoObject as GoPort;
					if (goPort != null)
					{
						SetPoint(0, goPort.GetFromLinkPoint(AbstractLink));
					}
				}
				if (num3 == pointsCount - 2 && ToPort != null)
				{
					GoPort goPort2 = ToPort.GoObject as GoPort;
					if (goPort2 != null)
					{
						SetPoint(pointsCount - 1, goPort2.GetToLinkPoint(AbstractLink));
					}
				}
			}
		}

		/// <summary>
		/// Unlike a normal <see cref="T:Northwoods.Go.GoStroke" /> the end handles are in the
		/// shape of diamonds if <see cref="P:Northwoods.Go.GoLink.Relinkable" /> is true.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// <para>
		/// The first and last resize handles depend on whether this link is <see cref="P:Northwoods.Go.GoLink.Relinkable" />.
		/// If it is, the handles have a <see cref="F:Northwoods.Go.GoHandleStyle.Diamond" /> style with a <see cref="P:Northwoods.Go.GoHandle.HandleID" />
		/// that is either <see cref="F:Northwoods.Go.GoLink.RelinkableFromHandle" /> or <see cref="F:Northwoods.Go.GoLink.RelinkableToHandle" />.
		/// If it is not, the handles are hollow and have a handle ID of <c>GoObject.NoHandle</c>,
		/// and the user cannot drag them. 
		/// </para>
		/// <para>
		/// The second and next-to-last resize handles,
		/// if they exist apart from first and last resize handles and if this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />,
		/// will have special <see cref="P:Northwoods.Go.GoHandle.HandleID" />s to constrain the movement of those handles in
		/// order to maintain orthogonality.  These special handles are <c>GoObject.MiddleTop</c>,
		/// <c>GoObject.MiddleRight</c>, <c>GoObject.MiddleBottom</c>, and <c>GoObject.MiddleLeft</c>.
		/// </para>
		/// <para>
		/// The <see cref="P:Northwoods.Go.GoHandle.HandleID" /> for the rest of the resize handles,
		/// if they exist in the middle parts of the link,
		/// is just the corresponding point index plus <c>GoObject.LastHandle</c>.
		/// These allow for unconstrained movement while resizing, even if the link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />.
		/// </para>
		/// <para>
		/// Furthermore, if <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> and <see cref="P:Northwoods.Go.GoLink.DraggableOrthogonalSegments" />
		/// are both true, this adds additional hidden handles (<see cref="P:Northwoods.Go.GoHandle.Style" /> is
		/// <see cref="F:Northwoods.Go.GoHandleStyle.None" />) along the middle segments to allow those segments
		/// to be dragged in a direction perpendicular to the segment.
		/// These handles have a <see cref="P:Northwoods.Go.GoHandle.HandleID" /> equal to the index of the
		/// point beginning the segment plus <c>GoObject.LastHandle</c> plus 1000000.
		/// </para>
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			if (HighlightWhenSelected || !(CanResize() && flag))
			{
				base.AddSelectionHandles(sel, selectedObj);
				return;
			}
			sel.RemoveHandles(this);
			if (PointsCount == 0)
			{
				return;
			}
			int firstPickIndex = FirstPickIndex;
			int lastPickIndex = LastPickIndex;
			bool flag3 = CanReshape() && flag2;
			bool relinkable = Relinkable;
			PointF point = GetPoint(firstPickIndex);
			int num = relinkable ? 1024 : 0;
			IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, point, num, num != 0);
			if (num == 1024)
			{
				MakeDiamondResizeHandle(handle, 0);
			}
			point = GetPoint(lastPickIndex);
			num = (relinkable ? 1025 : 0);
			handle = sel.CreateResizeHandle(this, selectedObj, point, num, num != 0);
			if (num == 1025)
			{
				MakeDiamondResizeHandle(handle, 0);
			}
			checked
			{
				for (int i = firstPickIndex + 1; i <= lastPickIndex - 1; i++)
				{
					point = GetPoint(i);
					num = 8192 + i;
					if (!flag3)
					{
						num = 0;
					}
					else if (Orthogonal)
					{
						if (PointsCount < 6)
						{
							num = 0;
						}
						else if (i == firstPickIndex + 1 && FromPort != null)
						{
							PointF point2 = GetPoint(firstPickIndex);
							if (IsApprox(point2.Y, point.Y) && !IsApprox(point2.X, point.X))
							{
								num = 256;
							}
							else if (IsApprox(point2.X, point.X) && !IsApprox(point2.Y, point.Y))
							{
								num = 32;
							}
							else if (IsApprox(point2.X, point.X) && IsApprox(point2.Y, point.Y) && firstPickIndex + 2 <= lastPickIndex)
							{
								PointF point3 = GetPoint(firstPickIndex + 2);
								if (IsApprox(point3.Y, point.Y) && !IsApprox(point3.X, point.X))
								{
									num = 32;
								}
								else if (IsApprox(point3.X, point.X) && !IsApprox(point3.Y, point.Y))
								{
									num = 256;
								}
							}
						}
						else if (i == lastPickIndex - 1 && ToPort != null)
						{
							PointF point4 = GetPoint(lastPickIndex);
							if (IsApprox(point.Y, point4.Y) && !IsApprox(point.X, point4.X))
							{
								num = 64;
							}
							else if (IsApprox(point.X, point4.X) && !IsApprox(point.Y, point4.Y))
							{
								num = 128;
							}
							else if (IsApprox(point4.X, point.X) && IsApprox(point4.Y, point.Y) && lastPickIndex - 2 >= firstPickIndex)
							{
								PointF point5 = GetPoint(lastPickIndex - 2);
								if (IsApprox(point5.Y, point.Y) && !IsApprox(point5.X, point.X))
								{
									num = 128;
								}
								else if (IsApprox(point5.X, point.X) && !IsApprox(point5.Y, point.Y))
								{
									num = 64;
								}
							}
						}
					}
					sel.CreateResizeHandle(this, selectedObj, point, num, num != 0);
					if ((num <= 8192 && (i != firstPickIndex + 1 || num == 0)) || !Orthogonal || !DraggableOrthogonalSegments)
					{
						continue;
					}
					PointF pointF = point;
					PointF point6 = GetPoint(i + 1);
					PointF pointF2 = new PointF(Math.Min(pointF.X, point6.X), Math.Min(pointF.Y, point6.Y));
					RectangleF a = new RectangleF(pointF2.X, pointF2.Y, Math.Max(pointF.X, point6.X) - pointF2.X, Math.Max(pointF.Y, point6.Y) - pointF2.Y);
					float num2 = 3f;
					float num3 = 3f;
					if (view != null)
					{
						num2 = view.ResizeHandleSize.Width / 2f / view.WorldScale.Width;
						num3 = view.ResizeHandleSize.Height / 2f / view.WorldScale.Height;
					}
					GoHandle goHandle = null;
					bool flag4 = IsApprox(a.Width, 0f);
					if (flag4)
					{
						if (Math.Abs(pointF.Y - point6.Y) > num3 * 2f)
						{
							goHandle = new GoHandle();
						}
						GoObject.InflateRect(ref a, num2, 0f - num3);
					}
					else
					{
						if (Math.Abs(pointF.X - point6.X) > num2 * 2f)
						{
							goHandle = new GoHandle();
						}
						GoObject.InflateRect(ref a, 0f - num2, num3);
					}
					if (goHandle != null)
					{
						goHandle.Style = GoHandleStyle.None;
						if (num > 8192)
						{
							goHandle.HandleID = num + 1000000;
						}
						else
						{
							goHandle.HandleID = 8192 + i + 1000000;
						}
						goHandle.SelectedObject = selectedObj;
						goHandle.Bounds = a;
						if (flag4)
						{
							goHandle.CursorName = "col-resize";
						}
						else
						{
							goHandle.CursorName = "row-resize";
						}
						sel.AddHandle(this, goHandle);
					}
				}
			}
		}

		/// <summary>
		/// When a link is removed from a layer, also remove it from the
		/// collections of connected links of both ports.
		/// </summary>
		/// <param name="oldlayer"></param>
		/// <param name="newlayer"></param>
		/// <param name="mainObj"></param>
		protected override void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
			base.OnLayerChanged(oldlayer, newlayer, mainObj);
			if (newlayer == null && !NoClearPorts && (mainObj is IGoLink || !IsChildOf(mainObj)))
			{
				IGoLink abstractLink = AbstractLink;
				FromPort?.RemoveLink(abstractLink);
				ToPort?.RemoveLink(abstractLink);
			}
			else if (newlayer != null)
			{
				IGoLink abstractLink2 = AbstractLink;
				FromPort?.AddDestinationLink(abstractLink2);
				ToPort?.AddSourceLink(abstractLink2);
			}
		}

		/// <summary>
		/// This implementation of <see cref="T:Northwoods.Go.IGoRoutable" /> just calls <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> to determine
		/// a new path (i.e. a new set of points) for this link.
		/// </summary>
		public virtual void CalculateRoute()
		{
			CalculateStroke();
		}

		/// <summary>
		/// Request the recalculation of the stroke for this link.
		/// </summary>
		/// <remarks>
		/// If this is part of a <see cref="T:Northwoods.Go.GoDocument" />,
		/// this calls <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.UpdateRoute(Northwoods.Go.IGoRoutable)" /> in order to maybe delay
		/// the call to <see cref="M:Northwoods.Go.GoLink.CalculateRoute" />.
		/// If there is no <see cref="T:Northwoods.Go.GoDocument" />, this just calls <see cref="M:Northwoods.Go.GoLink.CalculateRoute" /> immediately.
		/// </remarks>
		public virtual void UpdateRoute()
		{
			GoDocument document = base.Document;
			if (document != null)
			{
				IGoRoutable goRoutable = AbstractLink as IGoRoutable;
				if (goRoutable == null)
				{
					goRoutable = this;
				}
				document.UpdateRoute(goRoutable);
			}
			else
			{
				CalculateRoute();
			}
		}

		/// <summary>
		/// This method is responsible for determining the points in the stroke,
		/// given the positions of the ports.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The standard stroke path is determined by properties of this link
		/// and of the two ports it is connected to.
		/// If both ports are <see cref="T:Northwoods.Go.GoPort" />s, by default this method
		/// considers the <see cref="P:Northwoods.Go.GoLink.FromPort" />'s <see cref="P:Northwoods.Go.GoPort.FromSpot" />
		/// and the <see cref="P:Northwoods.Go.GoLink.ToPort" />'s <see cref="P:Northwoods.Go.GoPort.ToSpot" />.
		/// If both spots are <c>GoObject.NoSpot</c>, this link's stroke
		/// will consist of two points determined by calling
		/// <see cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" /> and <see cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />.
		/// (There are four points if the <see cref="P:Northwoods.Go.GoStroke.Style" /> is
		/// <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" />--the points are plotted according
		/// to the value of <see cref="P:Northwoods.Go.GoStroke.Curviness" />.)
		/// If both spots have "spots" (i.e. not <c>GoObject.NoSpot</c>), 
		/// there will be four points in the stroke, resulting in
		/// three straight line segments, unless <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is
		/// true, in which case there are six points (five horizontal or vertical
		/// line segments).
		/// </para>
		/// <para>
		/// However, if there are already more than the standard number of
		/// points in this link, the <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> method is called
		/// to give the link a chance to decide how it wants to modify its path.
		/// The behavior of <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> depends on the value of
		/// <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" />.
		/// If <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> returns false, this method plots the
		/// standard stroke as described above.
		/// </para>
		/// <para>
		/// If either port is not a <see cref="T:Northwoods.Go.GoPort" />, this stroke is just
		/// a single straight line segment between the centers of the objects.
		/// If either port is not a <see cref="P:Northwoods.Go.GoLink.GoObject" />, this method does nothing.
		/// </para>
		/// </remarks>
		public virtual void CalculateStroke()
		{
			IGoPort fromPort = FromPort;
			IGoPort toPort = ToPort;
			if (fromPort == null || toPort == null)
			{
				return;
			}
			GoObject goObject = fromPort.GoObject;
			GoObject goObject2 = toPort.GoObject;
			if (goObject == null || goObject2 == null)
			{
				return;
			}
			GoPort goPort = goObject as GoPort;
			GoPort goPort2 = goObject2 as GoPort;
			int pointsCount = PointsCount;
			int num = goPort?.FromSpot ?? 0;
			int num2 = goPort2?.ToSpot ?? 0;
			bool isSelfLoop = IsSelfLoop;
			bool orthogonal = Orthogonal;
			bool flag = Style == GoStrokeStyle.Bezier;
			bool flag2 = AdjustingStyle == GoLinkAdjustingStyle.Calculate;
			float curviness = Curviness;
			bool suspendsUpdates = base.SuspendsUpdates;
			bool initializing = base.Initializing;
			if (!suspendsUpdates)
			{
				Changing(1204);
			}
			base.SuspendsUpdates = true;
			base.Initializing = true;
			if (goPort == null || goPort2 == null || (!orthogonal && num == 0 && num2 == 0 && !isSelfLoop))
			{
				bool flag3 = false;
				if (!flag2 && pointsCount >= 3)
				{
					PointF result = goObject.Center;
					PointF result2 = goObject2.Center;
					if (goPort == null)
					{
						if (!goObject.GetNearestIntersectionPoint(result2, result, out result))
						{
							result = goObject.Center;
						}
					}
					else
					{
						result = goPort.GetFromLinkPoint(AbstractLink);
					}
					if (goPort2 == null)
					{
						if (!goObject2.GetNearestIntersectionPoint(result, result2, out result2))
						{
							result2 = goObject2.Center;
						}
					}
					else
					{
						result2 = goPort2.GetToLinkPoint(AbstractLink);
					}
					flag3 = AdjustPoints(0, result, checked(pointsCount - 1), result2);
				}
				if (!flag3)
				{
					if (flag)
					{
						CalculateBezierNoSpot(goObject, goPort, goObject2, goPort2);
					}
					else
					{
						CalculateLineNoSpot(goObject, goPort, goObject2, goPort2);
					}
				}
			}
			else
			{
				if (flag2 && ((orthogonal && AvoidsNodes) || isSelfLoop))
				{
					ClearPoints();
				}
				PointF pointF = goPort.GetFromLinkPoint(AbstractLink);
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				if (orthogonal || num != 0 || isSelfLoop)
				{
					float num6 = goPort.GetFromEndSegmentLength(AbstractLink);
					num5 = goPort.GetFromLinkDir(AbstractLink);
					if (isSelfLoop)
					{
						num5 -= (float)(orthogonal ? 90 : 30);
						if (curviness < 0f)
						{
							num5 -= 180f;
						}
						if (num5 < 0f)
						{
							num5 += 360f;
						}
					}
					if (flag)
					{
						num6 += 15f;
					}
					if (isSelfLoop)
					{
						num6 += Math.Abs(curviness);
					}
					if (num5 == 0f)
					{
						num3 = num6;
					}
					else if (num5 == 90f)
					{
						num4 = num6;
					}
					else if (num5 == 180f)
					{
						num3 = 0f - num6;
					}
					else if (num5 == 270f)
					{
						num4 = 0f - num6;
					}
					else
					{
						num3 = num6 * (float)Math.Cos((double)num5 * Math.PI / 180.0);
						num4 = num6 * (float)Math.Sin((double)num5 * Math.PI / 180.0);
					}
					if (num == 0 && isSelfLoop)
					{
						pointF = goPort.GetLinkPointFromPoint(new PointF(pointF.X + num3 * 1000f, pointF.Y + num4 * 1000f));
					}
				}
				PointF pointF2 = goPort2.GetToLinkPoint(AbstractLink);
				float num7 = 0f;
				float num8 = 0f;
				float num9 = 0f;
				if (orthogonal || num2 != 0 || isSelfLoop)
				{
					float num10 = goPort2.GetToEndSegmentLength(AbstractLink);
					num9 = goPort2.GetToLinkDir(AbstractLink);
					if (isSelfLoop)
					{
						num9 += (float)((!orthogonal) ? 30 : 0);
						if (curviness < 0f)
						{
							num9 += 180f;
						}
						if (num9 > 360f)
						{
							num9 -= 360f;
						}
					}
					if (flag)
					{
						num10 += 15f;
					}
					if (isSelfLoop)
					{
						num10 += Math.Abs(curviness);
					}
					if (num9 == 0f)
					{
						num7 = num10;
					}
					else if (num9 == 90f)
					{
						num8 = num10;
					}
					else if (num9 == 180f)
					{
						num7 = 0f - num10;
					}
					else if (num9 == 270f)
					{
						num8 = 0f - num10;
					}
					else
					{
						num7 = num10 * (float)Math.Cos((double)num9 * Math.PI / 180.0);
						num8 = num10 * (float)Math.Sin((double)num9 * Math.PI / 180.0);
					}
					if (num2 == 0 && isSelfLoop)
					{
						pointF2 = goPort2.GetLinkPointFromPoint(new PointF(pointF2.X + num7 * 1000f, pointF2.Y + num8 * 1000f));
					}
				}
				PointF pointF3 = pointF;
				if (orthogonal || num != 0 || isSelfLoop)
				{
					pointF3 = new PointF(pointF.X + num3, pointF.Y + num4);
				}
				PointF pointF4 = pointF2;
				if (orthogonal || num2 != 0 || isSelfLoop)
				{
					pointF4 = new PointF(pointF2.X + num7, pointF2.Y + num8);
				}
				checked
				{
					if (!flag2 && !orthogonal && num == 0 && pointsCount > 3 && AdjustPoints(0, pointF, pointsCount - 2, pointF4))
					{
						SetPoint(pointsCount - 1, pointF2);
					}
					else if (!flag2 && !orthogonal && num2 == 0 && pointsCount > 3 && AdjustPoints(1, pointF3, pointsCount - 1, pointF2))
					{
						SetPoint(0, pointF);
					}
					else if (!flag2 && !orthogonal && pointsCount > 4 && AdjustPoints(1, pointF3, pointsCount - 2, pointF4))
					{
						SetPoint(0, pointF);
						SetPoint(pointsCount - 1, pointF2);
					}
					else if (unchecked(!flag2 && orthogonal) && pointsCount >= 6 && !AvoidsNodes && AdjustPoints(1, pointF3, pointsCount - 2, pointF4))
					{
						SetPoint(0, pointF);
						SetPoint(pointsCount - 1, pointF2);
					}
					else
					{
						ClearPoints();
						AddPoint(pointF);
						unchecked
						{
							if (orthogonal || num != 0 || isSelfLoop)
							{
								AddPoint(pointF3);
							}
							if (orthogonal)
							{
								AddOrthoPoints(pointF3, num5, pointF4, num9);
							}
							if (orthogonal || num2 != 0 || isSelfLoop)
							{
								AddPoint(pointF4);
							}
							AddPoint(pointF2);
						}
					}
				}
			}
			base.InvalidBounds = true;
			base.Initializing = initializing;
			base.SuspendsUpdates = suspendsUpdates;
			if (!suspendsUpdates)
			{
				RectangleF bounds = Bounds;
				Changed(1204, 0, null, bounds, 0, null, bounds);
			}
		}

		private void CalculateLineNoSpot(GoObject fromObj, GoPort from, GoObject toObj, GoPort to)
		{
			ClearPoints();
			PointF result = fromObj.Center;
			PointF result2 = toObj.Center;
			if (from == null)
			{
				if (!fromObj.GetNearestIntersectionPoint(result2, result, out result))
				{
					result = fromObj.Center;
				}
			}
			else
			{
				result = from.GetFromLinkPoint(AbstractLink);
			}
			if (to == null)
			{
				if (!toObj.GetNearestIntersectionPoint(result, result2, out result2))
				{
					result2 = toObj.Center;
				}
			}
			else
			{
				result2 = to.GetToLinkPoint(AbstractLink);
			}
			AddPoint(result);
			AddPoint(result2);
		}

		private void CalculateBezierNoSpot(GoObject fromObj, GoPort from, GoObject toObj, GoPort to)
		{
			ClearPoints();
			PointF result = fromObj.Center;
			PointF result2 = toObj.Center;
			if (from == null)
			{
				if (!fromObj.GetNearestIntersectionPoint(result2, result, out result))
				{
					result = fromObj.Center;
				}
			}
			else
			{
				result = from.GetFromLinkPoint(AbstractLink);
			}
			if (to == null)
			{
				if (!toObj.GetNearestIntersectionPoint(result, result2, out result2))
				{
					result2 = toObj.Center;
				}
			}
			else
			{
				result2 = to.GetToLinkPoint(AbstractLink);
			}
			float num = result2.X - result.X;
			float num2 = result2.Y - result.Y;
			float curviness = Curviness;
			float num3 = Math.Abs(curviness);
			if (curviness < 0f)
			{
				num3 = 0f - num3;
			}
			float num4 = 0f;
			float num5 = 0f;
			float num6 = result.X + num / 3f;
			float num7 = result.Y + num2 / 3f;
			float num8 = num6;
			float num9 = num7;
			if (IsApprox(num2, 0f))
			{
				num9 = ((!(num > 0f)) ? (num9 + num3) : (num9 - num3));
			}
			else
			{
				num4 = (0f - num) / num2;
				num5 = (float)Math.Sqrt(num3 * num3 / (num4 * num4 + 1f));
				if (curviness < 0f)
				{
					num5 = 0f - num5;
				}
				num8 = (float)((!(num2 < 0f)) ? 1 : (-1)) * num5 + num6;
				num9 = num4 * (num8 - num6) + num7;
			}
			num6 = result.X + 2f * num / 3f;
			num7 = result.Y + 2f * num2 / 3f;
			float num10 = num6;
			float num11 = num7;
			if (IsApprox(num2, 0f))
			{
				num11 = ((!(num > 0f)) ? (num11 + num3) : (num11 - num3));
			}
			else
			{
				num10 = (float)((!(num2 < 0f)) ? 1 : (-1)) * num5 + num6;
				num11 = num4 * (num10 - num6) + num7;
			}
			AddPoint(result);
			AddPoint(num8, num9);
			AddPoint(num10, num11);
			AddPoint(result2);
			SetPoint(0, from.GetFromLinkPoint(AbstractLink));
			SetPoint(3, to.GetToLinkPoint(AbstractLink));
		}

		/// <summary>
		/// Adjust all of the existing points in this link's stroke in an inclusive range
		/// according to new first and last stroke points.
		/// </summary>
		/// <param name="startIndex">the zero-based index of the first point to be changed, to be
		/// the value of <paramref name="newFromPoint" /></param>
		/// <param name="newFromPoint"></param>
		/// <param name="endIndex">the zero-based index of the last point to be changed, to be
		/// the value of <paramref name="newToPoint" /></param>
		/// <param name="newToPoint"></param>
		/// <value>
		/// This method should return true if the stroke points were adjusted.  Return false
		/// to tell <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> to plot the standard path.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is primarily useful to help maintain intermediate inflection points
		/// in a link when one or both ports moves.
		/// By default this just calls <see cref="M:Northwoods.Go.GoLink.RescalePoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />, <see cref="M:Northwoods.Go.GoLink.StretchPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />,
		/// or <see cref="M:Northwoods.Go.GoLink.ModifyEndPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />.
		/// This method is not called when there are no existing points to be adjusted
		/// or when <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" /> is <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Calculate" />.
		/// </para>
		/// <para>
		/// When this link is <see cref="P:Northwoods.Go.GoLink.Orthogonal" />, an <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" /> of
		/// <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Scale" /> will just return false to result in
		/// the standard orthogonal path.
		/// An <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" /> of <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Stretch" />
		/// for an orthogonal link is treated as if it were <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.End" />.
		/// </para>
		/// </remarks>
		protected virtual bool AdjustPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
		{
			GoLinkAdjustingStyle goLinkAdjustingStyle = AdjustingStyle;
			if (Orthogonal)
			{
				switch (goLinkAdjustingStyle)
				{
				case GoLinkAdjustingStyle.Scale:
					return false;
				case GoLinkAdjustingStyle.Stretch:
					goLinkAdjustingStyle = GoLinkAdjustingStyle.End;
					break;
				}
			}
			switch (goLinkAdjustingStyle)
			{
			case GoLinkAdjustingStyle.Scale:
				return RescalePoints(startIndex, newFromPoint, endIndex, newToPoint);
			case GoLinkAdjustingStyle.Stretch:
				return StretchPoints(startIndex, newFromPoint, endIndex, newToPoint);
			case GoLinkAdjustingStyle.End:
				return ModifyEndPoints(startIndex, newFromPoint, endIndex, newToPoint);
			default:
				return false;
			}
		}

		/// <summary>
		/// Maintain the same shape for the stroke, but scale and rotate according to
		/// new points <paramref name="newFromPoint" /> and <paramref name="newToPoint" />.
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="newFromPoint"></param>
		/// <param name="endIndex"></param>
		/// <param name="newToPoint"></param>
		/// <value>
		/// This method should return true if the stroke points were adjusted.  Return false
		/// to tell <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> and <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> to plot
		/// the standard stroke path.
		/// </value>
		/// <remarks>
		/// The <paramref name="startIndex" /> point should be set to <paramref name="newFromPoint" />,
		/// and the <paramref name="endIndex" /> point should be set to <paramref name="newToPoint" />,
		/// and all the intermediate points should be scaled and rotated accordingly to
		/// maintain the same shape as the original set of points from <paramref name="startIndex" />
		/// to <paramref name="endIndex" />, inclusive.
		/// <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> calls this method when <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" />
		/// is <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Scale" />.
		/// This method should not be used when <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true.
		/// </remarks>
		protected virtual bool RescalePoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
		{
			PointF point = GetPoint(startIndex);
			PointF point2 = GetPoint(endIndex);
			if (point == newFromPoint && point2 == newToPoint)
			{
				return true;
			}
			double num = point.X;
			double num2 = point.Y;
			double num3 = point2.X;
			double num4 = point2.Y;
			double num5 = num3 - num;
			double num6 = num4 - num2;
			double num7 = Math.Sqrt(num5 * num5 + num6 * num6);
			if (IsApprox(num7, 0.0))
			{
				return true;
			}
			double num8;
			if (IsApprox(num5, 0.0))
			{
				num8 = ((!(num6 < 0.0)) ? (Math.PI / 2.0) : (-Math.PI / 2.0));
			}
			else
			{
				num8 = Math.Atan(num6 / Math.Abs(num5));
				if (num5 < 0.0)
				{
					num8 = Math.PI - num8;
				}
			}
			double num9 = newFromPoint.X;
			double num10 = newFromPoint.Y;
			double num11 = newToPoint.X;
			double num12 = newToPoint.Y;
			double num13 = num11 - num9;
			double num14 = num12 - num10;
			double num15 = Math.Sqrt(num13 * num13 + num14 * num14);
			double num16;
			if (IsApprox(num13, 0.0))
			{
				num16 = ((!(num14 < 0.0)) ? (Math.PI / 2.0) : (-Math.PI / 2.0));
			}
			else
			{
				num16 = Math.Atan(num14 / Math.Abs(num13));
				if (num13 < 0.0)
				{
					num16 = Math.PI - num16;
				}
			}
			double num17 = num15 / num7;
			double num18 = num16 - num8;
			SetPoint(startIndex, newFromPoint);
			checked
			{
				for (int i = startIndex + 1; i < endIndex; i++)
				{
					PointF point3 = GetPoint(i);
					num5 = (double)point3.X - num;
					num6 = (double)point3.Y - num2;
					double num19 = Math.Sqrt(num5 * num5 + num6 * num6);
					if (IsApprox(num19, 0.0))
					{
						continue;
					}
					double num20;
					if (IsApprox(num5, 0.0))
					{
						num20 = ((!(num6 < 0.0)) ? (Math.PI / 2.0) : (-Math.PI / 2.0));
					}
					else
					{
						num20 = Math.Atan(num6 / Math.Abs(num5));
						if (num5 < 0.0)
						{
							num20 = Math.PI - num20;
						}
					}
					double num21 = num20 + num18;
					double num22 = num19 * num17;
					double num23 = num9 + num22 * Math.Cos(num21);
					double num24 = num10 + num22 * Math.Sin(num21);
					SetPoint(i, new PointF((float)num23, (float)num24));
				}
				SetPoint(endIndex, newToPoint);
				return true;
			}
		}

		/// <summary>
		/// Stretch the points of this stroke by interpolating the points
		/// from <paramref name="startIndex" /> to <paramref name="endIndex" /> between the
		/// new points <paramref name="newFromPoint" /> and <paramref name="newToPoint" />.
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="newFromPoint"></param>
		/// <param name="endIndex"></param>
		/// <param name="newToPoint"></param>
		/// <value>
		/// This method should return true if the stroke points were adjusted.  Return false
		/// to tell <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> and <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> to plot
		/// the standard stroke path.
		/// </value>
		/// <remarks>
		/// The <paramref name="startIndex" /> point should be set to <paramref name="newFromPoint" />,
		/// and the <paramref name="endIndex" /> point should be set to <paramref name="newToPoint" />,
		/// and all the intermediate points should be interpolated linearly to
		/// stretch or compress the original set of points from <paramref name="startIndex" />
		/// to <paramref name="endIndex" />, inclusive, along each of the X and Y dimensions.
		/// <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> calls this method when <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" />
		/// is <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Stretch" />.
		/// This method should not be used when <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true.
		/// </remarks>
		protected virtual bool StretchPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
		{
			PointF point = GetPoint(startIndex);
			PointF point2 = GetPoint(endIndex);
			if (point == newFromPoint && point2 == newToPoint)
			{
				return true;
			}
			double num = point.X;
			double num2 = point.Y;
			double num3 = point2.X;
			double num4 = point2.Y;
			double num5 = (num3 - num) * (num3 - num) + (num4 - num2) * (num4 - num2);
			double num6 = newFromPoint.X;
			double num7 = newFromPoint.Y;
			double num8 = newToPoint.X;
			double num9 = newToPoint.Y;
			double num10 = 0.0;
			double num11 = 1.0;
			num10 = ((num8 - num6 == 0.0) ? 9000000000.0 : ((num9 - num7) / (num8 - num6)));
			if (num10 != 0.0)
			{
				num11 = Math.Sqrt(1.0 + 1.0 / (num10 * num10));
			}
			SetPoint(startIndex, newFromPoint);
			checked
			{
				for (int i = startIndex + 1; i < endIndex; i++)
				{
					PointF point3 = GetPoint(i);
					double num12 = point3.X;
					double num13 = point3.Y;
					double num14 = 0.5;
					if (num5 != 0.0)
					{
						num14 = ((num - num12) * (num - num3) + (num2 - num13) * (num2 - num4)) / num5;
					}
					double num15 = num + num14 * (num3 - num);
					double num16 = num2 + num14 * (num4 - num2);
					double num17 = Math.Sqrt((num12 - num15) * (num12 - num15) + (num13 - num16) * (num13 - num16));
					if (num13 < num10 * (num12 - num15) + num16)
					{
						num17 = 0.0 - num17;
					}
					if (num10 > 0.0)
					{
						num17 = 0.0 - num17;
					}
					double num18 = num6 + num14 * (num8 - num6);
					double num19 = num7 + num14 * (num9 - num7);
					if (num10 != 0.0)
					{
						double num20 = num18 + num17 / num11;
						double num21 = num19 - (num20 - num18) / num10;
						SetPoint(i, new PointF((float)num20, (float)num21));
					}
					else
					{
						SetPoint(i, new PointF((float)num18, (float)(num19 + num17)));
					}
				}
				SetPoint(endIndex, newToPoint);
				return true;
			}
		}

		/// <summary>
		/// Modify only the end points of this stroke to match any new
		/// <paramref name="newFromPoint" /> or <paramref name="newToPoint" /> points;
		/// intermediate points are not changed.
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="newFromPoint"></param>
		/// <param name="endIndex"></param>
		/// <param name="newToPoint"></param>
		/// <value>
		/// This method should return true if the stroke points were adjusted.  Return false
		/// to tell <see cref="M:Northwoods.Go.GoLink.AdjustPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" /> and <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> to plot
		/// the standard stroke path.
		/// </value>
		/// <remarks>
		/// The <paramref name="startIndex" /> point should be set to <paramref name="newFromPoint" />,
		/// and the <paramref name="endIndex" /> point should be set to <paramref name="newToPoint" />,
		/// and the intermediate points should be not be changed unless needed to maintain
		/// orthogonality when <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true.
		/// </remarks>
		protected virtual bool ModifyEndPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
		{
			checked
			{
				if (Orthogonal)
				{
					PointF point = GetPoint(startIndex + 1);
					PointF point2 = GetPoint(startIndex + 2);
					if (IsApprox(point.X, point2.X) && !IsApprox(point.Y, point2.Y))
					{
						SetPoint(startIndex + 1, new PointF(point.X, newFromPoint.Y));
					}
					else if (IsApprox(point.Y, point2.Y))
					{
						SetPoint(startIndex + 1, new PointF(newFromPoint.X, point.Y));
					}
					point = GetPoint(endIndex - 1);
					point2 = GetPoint(endIndex - 2);
					if (IsApprox(point.X, point2.X) && !IsApprox(point.Y, point2.Y))
					{
						SetPoint(endIndex - 1, new PointF(point.X, newToPoint.Y));
					}
					else if (IsApprox(point.Y, point2.Y))
					{
						SetPoint(endIndex - 1, new PointF(newToPoint.X, point.Y));
					}
				}
				SetPoint(startIndex, newFromPoint);
				SetPoint(endIndex, newToPoint);
				return true;
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> when <see cref="P:Northwoods.Go.GoLink.Orthogonal" />
		/// is true and both ports are instances of <see cref="T:Northwoods.Go.GoPort" /> and at least one of
		/// them have a link spot that is not <c>NoSpot</c>.
		/// </summary>
		/// <param name="startFrom">
		/// this point will already have been added to the stroke by <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> before calling this method
		/// </param>
		/// <param name="fromDir">normally 0, 90, 180, or 270 degrees</param>
		/// <param name="endTo">
		/// <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> will add this point after calling this method
		/// </param>
		/// <param name="toDir">normally 0, 90, 180, or 270 degrees</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> is responsible for adding the first two
		/// and the last two points of the stroke, including <paramref name="startFrom" /> and <paramref name="endTo" />.
		/// This method is responsible for adding any additional points in the middle of the stroke.
		/// This method calls <see cref="M:Northwoods.Go.GoLink.GetMidOrthoPosition(System.Single,System.Single,System.Boolean)" /> to determine the
		/// distance of the middle segment between the two ports.
		/// It also tries to avoid the source node and the destination node.
		/// When the <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" /> property is true, this method uses another,
		/// more computationally expensive, method for determining the proper path of the
		/// link, which may have many segments.
		/// </remarks>
		protected virtual void AddOrthoPoints(PointF startFrom, float fromDir, PointF endTo, float toDir)
		{
			fromDir = ((-45f <= fromDir && fromDir < 45f) ? 0f : ((45f <= fromDir && fromDir < 135f) ? 90f : ((!(135f <= fromDir) || !(fromDir < 225f)) ? 270f : 180f)));
			toDir = ((-45f <= toDir && toDir < 45f) ? 0f : ((45f <= toDir && toDir < 135f) ? 90f : ((!(135f <= toDir) || !(toDir < 225f)) ? 270f : 180f)));
			PointF b = startFrom;
			PointF b2 = endTo;
			float num = PenWidth + 1f;
			GoObject goObject = FromPort.GoObject;
			IGoNode node = FromPort.Node;
			RectangleF a = (node != null && node.GoObject != null) ? node.GoObject.Bounds : ((goObject.Parent == null) ? goObject.Bounds : goObject.Parent.Bounds);
			GoObject.InflateRect(ref a, num, num);
			a = GoObject.UnionRect(a, startFrom);
			GoObject goObject2 = ToPort.GoObject;
			IGoNode node2 = ToPort.Node;
			RectangleF a2 = (node2 != null && node2.GoObject != null) ? node2.GoObject.Bounds : ((goObject2.Parent == null) ? goObject2.Bounds : goObject2.Parent.Bounds);
			GoObject.InflateRect(ref a2, num, num);
			a2 = GoObject.UnionRect(a2, endTo);
			if (AvoidsNodes && base.Document != null)
			{
				GoPositionArray positions = base.Document.GetPositions();
				RectangleF a3 = GoObject.UnionRect(a, a2);
				GoObject.InflateRect(ref a3, positions.CellSize.Width * 2f, positions.CellSize.Height * 2f);
				positions.Propagate(startFrom, fromDir, endTo, toDir, a3);
				int dist = positions.GetDist(endTo.X, endTo.Y);
				if (!positions.Abort && dist == 536870911)
				{
					positions.ClearAllUnoccupied();
					float smallMargin = positions.SmallMargin;
					GoObject.InflateRect(ref a3, positions.CellSize.Width * smallMargin, positions.CellSize.Height * smallMargin);
					positions.Propagate(startFrom, fromDir, endTo, toDir, a3);
					dist = positions.GetDist(endTo.X, endTo.Y);
				}
				if (!positions.Abort && dist == 536870911)
				{
					positions.ClearAllUnoccupied();
					float largeMargin = positions.LargeMargin;
					GoObject.InflateRect(ref a3, positions.CellSize.Width * largeMargin, positions.CellSize.Height * largeMargin);
					positions.Propagate(startFrom, fromDir, endTo, toDir, a3);
					dist = positions.GetDist(endTo.X, endTo.Y);
				}
				if (!positions.Abort && dist == 536870911 && positions.WholeDocument)
				{
					positions.ClearAllUnoccupied();
					positions.Propagate(startFrom, fromDir, endTo, toDir, positions.Bounds);
					dist = positions.GetDist(endTo.X, endTo.Y);
				}
				if (!positions.Abort && dist < 536870911 && !positions.IsOccupied(endTo.X, endTo.Y))
				{
					TraversePositions(positions, endTo.X, endTo.Y, toDir, first: true);
					PointF point = GetPoint(2);
					if (PointsCount < 4)
					{
						if (fromDir == 0f || fromDir == 180f)
						{
							point.X = startFrom.X;
							point.Y = endTo.Y;
						}
						else
						{
							point.X = endTo.X;
							point.Y = startFrom.Y;
						}
						SetPoint(2, point);
						InsertPoint(3, point);
						return;
					}
					PointF point2 = GetPoint(3);
					if (fromDir == 0f || fromDir == 180f)
					{
						if (IsApprox(point.X, point2.X))
						{
							float x = (fromDir == 0f) ? Math.Max(point.X, startFrom.X) : Math.Min(point.X, startFrom.X);
							SetPoint(2, new PointF(x, startFrom.Y));
							SetPoint(3, new PointF(x, point2.Y));
						}
						else if (IsApprox(point.Y, point2.Y))
						{
							if (Math.Abs(startFrom.Y - point.Y) <= positions.CellSize.Height / 2f)
							{
								SetPoint(2, new PointF(point.X, startFrom.Y));
								SetPoint(3, new PointF(point2.X, startFrom.Y));
							}
							InsertPoint(2, new PointF(point.X, startFrom.Y));
						}
						else
						{
							SetPoint(2, new PointF(startFrom.X, point.Y));
						}
					}
					else
					{
						if (fromDir != 90f && fromDir != 270f)
						{
							return;
						}
						if (IsApprox(point.Y, point2.Y))
						{
							float y = (fromDir == 90f) ? Math.Max(point.Y, startFrom.Y) : Math.Min(point.Y, startFrom.Y);
							SetPoint(2, new PointF(startFrom.X, y));
							SetPoint(3, new PointF(point2.X, y));
						}
						else if (IsApprox(point.X, point2.X))
						{
							if (Math.Abs(startFrom.X - point.X) <= positions.CellSize.Width / 2f)
							{
								SetPoint(2, new PointF(startFrom.X, point.Y));
								SetPoint(3, new PointF(startFrom.X, point2.Y));
							}
							InsertPoint(2, new PointF(startFrom.X, point.Y));
						}
						else
						{
							SetPoint(2, new PointF(point.X, startFrom.Y));
						}
					}
					return;
				}
			}
			PointF p;
			PointF p2;
			if (fromDir == 0f)
			{
				if (b2.X > b.X || (toDir == 270f && b2.Y < b.Y && a2.Right > b.X) || (toDir == 90f && b2.Y > b.Y && a2.Right > b.X))
				{
					p = new PointF(b2.X, b.Y);
					p2 = new PointF(b2.X, (b.Y + b2.Y) / 2f);
					if (toDir == 180f)
					{
						p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
						p2.X = p.X;
						p2.Y = b2.Y;
					}
					else if ((toDir == 270f && b2.Y < b.Y) || (toDir == 90f && b2.Y > b.Y))
					{
						if (b.X < a2.Left)
						{
							p.X = GetMidOrthoPosition(b.X, a2.Left, vertical: false);
						}
						else if (b.X < a2.Right && ((toDir == 270f && b.Y < a2.Top) || (toDir == 90f && b.Y > a2.Bottom)))
						{
							p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
						}
						else
						{
							p.X = a2.Right;
						}
						p2.X = p.X;
						p2.Y = b2.Y;
					}
					else if (toDir == 0f && b.X < a2.Left && b.Y > a2.Top && b.Y < a2.Bottom)
					{
						p.X = b.X;
						if (b.Y < b2.Y)
						{
							p.Y = Math.Min(b2.Y, a2.Top);
						}
						else
						{
							p.Y = Math.Max(b2.Y, a2.Bottom);
						}
						p2.Y = p.Y;
					}
				}
				else
				{
					p = new PointF(b.X, b2.Y);
					p2 = new PointF((b.X + b2.X) / 2f, b2.Y);
					if (toDir == 180f || (toDir == 90f && b2.Y < a.Top) || (toDir == 270f && b2.Y > a.Bottom))
					{
						if (toDir == 180f && (GoObject.ContainsRect(a2, b) || GoObject.ContainsRect(a, b2)))
						{
							p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
						}
						else if (b2.Y < b.Y && (toDir == 180f || toDir == 90f))
						{
							p.Y = GetMidOrthoPosition(a.Top, Math.Max(b2.Y, a2.Bottom), vertical: true);
						}
						else if (b2.Y > b.Y && (toDir == 180f || toDir == 270f))
						{
							p.Y = GetMidOrthoPosition(a.Bottom, Math.Min(b2.Y, a2.Top), vertical: true);
						}
						p2.X = b2.X;
						p2.Y = p.Y;
					}
					if (p.Y > a.Top && p.Y < a.Bottom)
					{
						if ((b2.X >= a.Left && b2.X <= b.X) || (b.X <= a2.Right && b.X >= b2.X))
						{
							if (toDir == 90f || toDir == 270f)
							{
								p = new PointF(Math.Max((b.X + b2.X) / 2f, b.X), b.Y);
								p2 = new PointF(p.X, b2.Y);
							}
						}
						else
						{
							if (toDir == 270f || ((toDir == 0f || toDir == 180f) && b2.Y < b.Y))
							{
								p.Y = Math.Min(b2.Y, (toDir == 0f) ? a.Top : Math.Min(a.Top, a2.Top));
							}
							else
							{
								p.Y = Math.Max(b2.Y, (toDir == 0f) ? a.Bottom : Math.Max(a.Bottom, a2.Bottom));
							}
							p2.X = b2.X;
							p2.Y = p.Y;
						}
					}
				}
			}
			else if (fromDir == 180f)
			{
				if (b2.X < b.X || (toDir == 270f && b2.Y < b.Y && a2.Left < b.X) || (toDir == 90f && b2.Y > b.Y && a2.Left < b.X))
				{
					p = new PointF(b2.X, b.Y);
					p2 = new PointF(b2.X, (b.Y + b2.Y) / 2f);
					if (toDir == 0f)
					{
						p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
						p2.X = p.X;
						p2.Y = b2.Y;
					}
					else if ((toDir == 270f && b2.Y < b.Y) || (toDir == 90f && b2.Y > b.Y))
					{
						if (b.X > a2.Right)
						{
							p.X = GetMidOrthoPosition(b.X, a2.Right, vertical: false);
						}
						else if (b.X > a2.Left && ((toDir == 270f && b.Y < a2.Top) || (toDir == 90f && b.Y > a2.Bottom)))
						{
							p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
						}
						else
						{
							p.X = a2.Left;
						}
						p2.X = p.X;
						p2.Y = b2.Y;
					}
					else if (toDir == 180f && b.X > a2.Right && b.Y > a2.Top && b.Y < a2.Bottom)
					{
						p.X = b.X;
						if (b.Y < b2.Y)
						{
							p.Y = Math.Min(b2.Y, a2.Top);
						}
						else
						{
							p.Y = Math.Max(b2.Y, a2.Bottom);
						}
						p2.Y = p.Y;
					}
				}
				else
				{
					p = new PointF(b.X, b2.Y);
					p2 = new PointF((b.X + b2.X) / 2f, b2.Y);
					if (toDir == 0f || (toDir == 90f && b2.Y < a.Top) || (toDir == 270f && b2.Y > a.Bottom))
					{
						if (toDir == 0f && (GoObject.ContainsRect(a2, b) || GoObject.ContainsRect(a, b2)))
						{
							p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
						}
						else if (b2.Y < b.Y && (toDir == 0f || toDir == 90f))
						{
							p.Y = GetMidOrthoPosition(a.Top, Math.Max(b2.Y, a2.Bottom), vertical: true);
						}
						else if (b2.Y > b.Y && (toDir == 0f || toDir == 270f))
						{
							p.Y = GetMidOrthoPosition(a.Bottom, Math.Min(b2.Y, a2.Top), vertical: true);
						}
						p2.X = b2.X;
						p2.Y = p.Y;
					}
					if (p.Y > a.Top && p.Y < a.Bottom)
					{
						if ((b2.X <= a.Right && b2.X >= b.X) || (b.X >= a2.Left && b.X <= b2.X))
						{
							if (toDir == 90f || toDir == 270f)
							{
								p = new PointF(Math.Min((b.X + b2.X) / 2f, b.X), b.Y);
								p2 = new PointF(p.X, b2.Y);
							}
						}
						else
						{
							if (toDir == 270f || ((toDir == 0f || toDir == 180f) && b2.Y < b.Y))
							{
								p.Y = Math.Min(b2.Y, (toDir == 180f) ? a.Top : Math.Min(a.Top, a2.Top));
							}
							else
							{
								p.Y = Math.Max(b2.Y, (toDir == 180f) ? a.Bottom : Math.Max(a.Bottom, a2.Bottom));
							}
							p2.X = b2.X;
							p2.Y = p.Y;
						}
					}
				}
			}
			else if (fromDir == 90f)
			{
				if (b2.Y > b.Y || (toDir == 180f && b2.X < b.X && a2.Bottom > b.Y) || (toDir == 0f && b2.X > b.X && a2.Bottom > b.Y))
				{
					p = new PointF(b.X, b2.Y);
					p2 = new PointF((b.X + b2.X) / 2f, b2.Y);
					if (toDir == 270f)
					{
						p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
						p2.X = b2.X;
						p2.Y = p.Y;
					}
					else if ((toDir == 180f && b2.X < b.X) || (toDir == 0f && b2.X > b.X))
					{
						if (b.Y < a2.Top)
						{
							p.Y = GetMidOrthoPosition(b.Y, a2.Top, vertical: true);
						}
						else if (b.Y < a2.Bottom && ((toDir == 180f && b.X < a2.Left) || (toDir == 0f && b.X > a2.Right)))
						{
							p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
						}
						else
						{
							p.Y = a2.Bottom;
						}
						p2.X = b2.X;
						p2.Y = p.Y;
					}
					else if (toDir == 90f && b.Y < a2.Top && b.X > a2.Left && b.X < a2.Right)
					{
						if (b.X < b2.X)
						{
							p.X = Math.Min(b2.X, a2.Left);
						}
						else
						{
							p.X = Math.Max(b2.X, a2.Right);
						}
						p.Y = b.Y;
						p2.X = p.X;
					}
				}
				else
				{
					p = new PointF(b2.X, b.Y);
					p2 = new PointF(b2.X, (b.Y + b2.Y) / 2f);
					if (toDir == 270f || (toDir == 0f && b2.X < a.Left) || (toDir == 180f && b2.X > a.Right))
					{
						if (toDir == 270f && (GoObject.ContainsRect(a2, b) || GoObject.ContainsRect(a, b2)))
						{
							p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
						}
						else if (b2.X < b.X && (toDir == 270f || toDir == 0f))
						{
							p.X = GetMidOrthoPosition(a.Left, Math.Max(b2.X, a2.Right), vertical: false);
						}
						else if (b2.X > b.X && (toDir == 270f || toDir == 180f))
						{
							p.X = GetMidOrthoPosition(a.Right, Math.Min(b2.X, a2.Left), vertical: false);
						}
						p2.X = p.X;
						p2.Y = b2.Y;
					}
					if (p.X > a.Left && p.X < a.Right)
					{
						if ((b2.Y >= a.Top && b2.Y <= b.Y) || (b.Y <= a2.Bottom && b.Y >= b2.Y))
						{
							if (toDir == 0f || toDir == 180f)
							{
								p = new PointF(b.X, Math.Max((b.Y + b2.Y) / 2f, b.Y));
								p2 = new PointF(b2.X, p.Y);
							}
						}
						else
						{
							if (toDir == 180f || ((toDir == 90f || toDir == 270f) && b2.X < b.X))
							{
								p.X = Math.Min(b2.X, (toDir == 90f) ? a.Left : Math.Min(a.Left, a2.Left));
							}
							else
							{
								p.X = Math.Max(b2.X, (toDir == 90f) ? a.Right : Math.Max(a.Right, a2.Right));
							}
							p2.X = p.X;
							p2.Y = b2.Y;
						}
					}
				}
			}
			else if (b2.Y < b.Y || (toDir == 180f && b2.X < b.X && a2.Top < b.Y) || (toDir == 0f && b2.X > b.X && a2.Top < b.Y))
			{
				p = new PointF(b.X, b2.Y);
				p2 = new PointF((b.X + b2.X) / 2f, b2.Y);
				if (toDir == 90f)
				{
					p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
					p2.X = b2.X;
					p2.Y = p.Y;
				}
				else if ((toDir == 180f && b2.X < b.X) || (toDir == 0f && b2.X >= b.X))
				{
					if (b.Y > a2.Bottom)
					{
						p.Y = GetMidOrthoPosition(b.Y, a2.Bottom, vertical: true);
					}
					else if (b.Y > a2.Top && ((toDir == 180f && b.X < a2.Left) || (toDir == 0f && b.X > a2.Right)))
					{
						p.Y = GetMidOrthoPosition(b.Y, b2.Y, vertical: true);
					}
					else
					{
						p.Y = a2.Top;
					}
					p2.X = b2.X;
					p2.Y = p.Y;
				}
				else if (toDir == 270f && b.Y > a2.Bottom && b.X > a2.Left && b.X < a2.Right)
				{
					if (b.X < b2.X)
					{
						p.X = Math.Min(b2.X, a2.Left);
					}
					else
					{
						p.X = Math.Max(b2.X, a2.Right);
					}
					p.Y = b.Y;
					p2.X = p.X;
				}
			}
			else
			{
				p = new PointF(b2.X, b.Y);
				p2 = new PointF(b2.X, (b.Y + b2.Y) / 2f);
				if (toDir == 90f || (toDir == 0f && b2.X < a.Left) || (toDir == 180f && b2.X > a.Right))
				{
					if (toDir == 90f && (GoObject.ContainsRect(a2, b) || GoObject.ContainsRect(a, b2)))
					{
						p.X = GetMidOrthoPosition(b.X, b2.X, vertical: false);
					}
					else if (b2.X < b.X && (toDir == 90f || toDir == 0f))
					{
						p.X = GetMidOrthoPosition(a.Left, Math.Max(b2.X, a2.Right), vertical: false);
					}
					else if (b2.X > b.X && (toDir == 90f || toDir == 180f))
					{
						p.X = GetMidOrthoPosition(a.Right, Math.Min(b2.X, a2.Left), vertical: false);
					}
					p2.X = p.X;
					p2.Y = b2.Y;
				}
				if (p.X > a.Left && p.X < a.Right)
				{
					if ((b2.Y <= a.Bottom && b2.Y >= b.Y) || (b.Y >= a2.Top && b.Y <= b2.Y))
					{
						if (toDir == 0f || toDir == 180f)
						{
							p = new PointF(b.X, Math.Min((b.Y + b2.Y) / 2f, b.Y));
							p2 = new PointF(b2.X, p.Y);
						}
					}
					else
					{
						if (toDir == 180f || ((toDir == 90f || toDir == 270f) && b2.X < b.X))
						{
							p.X = Math.Min(b2.X, (toDir == 270f) ? a.Left : Math.Min(a.Left, a2.Left));
						}
						else
						{
							p.X = Math.Max(b2.X, (toDir == 270f) ? a.Right : Math.Max(a.Right, a2.Right));
						}
						p2.X = p.X;
						p2.Y = b2.Y;
					}
				}
			}
			AddPoint(p);
			AddPoint(p2);
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoLink.AddOrthoPoints(System.Drawing.PointF,System.Single,System.Drawing.PointF,System.Single)" /> to determine the distance
		/// of the middle segment between the two ports.
		/// </summary>
		/// <param name="fromPosition">The <see cref="P:Northwoods.Go.GoLink.FromPort" />'s point's X or Y coordinate, depending on the direction</param>
		/// <param name="toPosition">The <see cref="P:Northwoods.Go.GoLink.ToPort" />'s point's X or Y coordinate, depending on the direction</param>
		/// <param name="vertical">Whether the mid-position is along the vertical axis or horizontal</param>
		/// <returns></returns>
		/// <remarks>
		/// By default this returns the midpoint between the two coordinates.
		/// </remarks>
		protected virtual float GetMidOrthoPosition(float fromPosition, float toPosition, bool vertical)
		{
			return (fromPosition + toPosition) / 2f;
		}

		private void TraversePositions(GoPositionArray positions, float px, float py, float dir, bool first)
		{
			SizeF cellSize = positions.CellSize;
			int num = positions.GetDist(px, py);
			float num2 = px;
			float num3 = py;
			float num4 = num2;
			float num5 = num3;
			if (dir == 0f)
			{
				num4 += cellSize.Width;
			}
			else if (dir == 90f)
			{
				num5 += cellSize.Height;
			}
			else if (dir == 180f)
			{
				num4 -= cellSize.Width;
			}
			else
			{
				num5 -= cellSize.Height;
			}
			checked
			{
				while (num > 1 && positions.GetDist(num4, num5) == num - 1)
				{
					num2 = num4;
					num3 = num5;
					if (dir == 0f)
					{
						num4 += cellSize.Width;
					}
					else if (dir == 90f)
					{
						num5 += cellSize.Height;
					}
					else if (dir == 180f)
					{
						num4 -= cellSize.Width;
					}
					else
					{
						num5 -= cellSize.Height;
					}
					num--;
				}
				if (first)
				{
					if (num > 1)
					{
						if (dir == 180f || dir == 0f)
						{
							num2 = (float)Math.Floor(num2 / cellSize.Width) * cellSize.Width + cellSize.Width / 2f;
						}
						else if (dir == 90f || dir == 270f)
						{
							num3 = (float)Math.Floor(num3 / cellSize.Height) * cellSize.Height + cellSize.Height / 2f;
						}
					}
				}
				else
				{
					num2 = (float)Math.Floor(num2 / cellSize.Width) * cellSize.Width + cellSize.Width / 2f;
					num3 = (float)Math.Floor(num3 / cellSize.Height) * cellSize.Height + cellSize.Height / 2f;
				}
				if (num > 1)
				{
					float dir2 = dir;
					float num6 = num2;
					float num7 = num3;
					if (dir == 0f)
					{
						dir2 = 90f;
						num7 += cellSize.Height;
					}
					else if (dir == 90f)
					{
						dir2 = 180f;
						num6 -= cellSize.Width;
					}
					else if (dir == 180f)
					{
						dir2 = 270f;
						num7 -= cellSize.Height;
					}
					else if (dir == 270f)
					{
						dir2 = 0f;
						num6 += cellSize.Width;
					}
					if (positions.GetDist(num6, num7) == num - 1)
					{
						TraversePositions(positions, num6, num7, dir2, first: false);
					}
					else
					{
						float num8 = num2;
						float num9 = num3;
						if (dir == 0f)
						{
							dir2 = 270f;
							num9 -= cellSize.Height;
						}
						else if (dir == 90f)
						{
							dir2 = 0f;
							num8 += cellSize.Width;
						}
						else if (dir == 180f)
						{
							dir2 = 90f;
							num9 += cellSize.Height;
						}
						else if (dir == 270f)
						{
							dir2 = 180f;
							num8 -= cellSize.Width;
						}
						if (positions.GetDist(num8, num9) == num - 1)
						{
							TraversePositions(positions, num8, num9, dir2, first: false);
						}
					}
				}
				AddPoint(num2, num3);
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1300:
				UserFlags = e.GetInt(undo);
				break;
			case 1301:
				UserObject = e.GetValue(undo);
				break;
			case 1302:
				FromPort = (IGoPort)e.GetValue(undo);
				break;
			case 1303:
				ToPort = (IGoPort)e.GetValue(undo);
				break;
			case 1304:
				Orthogonal = (bool)e.GetValue(undo);
				break;
			case 1305:
				Relinkable = (bool)e.GetValue(undo);
				break;
			case 1306:
				AbstractLink = (IGoLink)e.GetValue(undo);
				break;
			case 1307:
				AvoidsNodes = (bool)e.GetValue(undo);
				break;
			case 1309:
				PartID = e.GetInt(undo);
				break;
			case 1310:
				AdjustingStyle = (GoLinkAdjustingStyle)e.GetInt(undo);
				break;
			case 1312:
				DraggableOrthogonalSegments = (bool)e.GetValue(undo);
				break;
			case 1311:
				ToolTipText = (string)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
