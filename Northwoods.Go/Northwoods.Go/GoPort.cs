using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This class provides the standard implementation of <see cref="T:Northwoods.Go.IGoPort" />
	/// as a <see cref="P:Northwoods.Go.GoPort.GoObject" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A port is an object to which one or more links may be connected.
	/// Ports are normally part of a <see cref="T:Northwoods.Go.GoNode" />,
	/// although they may also be part of a <see cref="T:Northwoods.Go.GoLabeledLink" />.
	/// A node will only need more than one port when you want to make sure there is a
	/// logical distinction between the links or when you want to position the
	/// links connections at one physical location.  Both reasons apply, for example,
	/// when wanting to represent an electronic logic gate or arithmetic or database
	/// operator: you don't want to confuse where each input is getting its value from.
	/// </para>
	/// <para>
	/// Since GoPort inherits from <see cref="T:Northwoods.Go.GoShape" />, it is easy to customize
	/// the appearance of a port by setting the <see cref="P:Northwoods.Go.GoShape.BrushColor" /> and/or the
	/// <see cref="P:Northwoods.Go.GoShape.PenColor" /> properties.  The <see cref="P:Northwoods.Go.GoPort.Style" /> property
	/// controls the shape of the port within the port's bounds.
	/// The default style is <see cref="F:Northwoods.Go.GoPortStyle.Ellipse" />.  When the style is
	/// <see cref="F:Northwoods.Go.GoPortStyle.None" />, nothing is painted for the port, but it
	/// continues to behave normally.
	/// </para>
	/// <para>
	/// A less common approach to providing a port appearance is to specify a style
	/// that is <see cref="F:Northwoods.Go.GoPortStyle.Object" />.  For this case you will need to
	/// set the <see cref="P:Northwoods.Go.GoPort.PortObject" /> property to refer to a <see cref="P:Northwoods.Go.GoPort.GoObject" />
	/// that is not part of any group or layer.  This <see cref="P:Northwoods.Go.GoPort.PortObject" /> may be
	/// shared by many ports.
	/// When there is a port object, the appearance comes totally from that object --
	/// the port's brush and pen are ignored.  You'll need to make sure that that
	/// object can be resized to the same size as the port.  If you use a <see cref="T:Northwoods.Go.GoText" />
	/// as a port object, be sure to either set <see cref="P:Northwoods.Go.GoText.AutoResizes" /> true,
	/// or resize the port to be able to show all of the text.
	/// </para>
	/// <para>
	/// The links that are attached to a port are conceptually divided into two categories:
	/// ones that come into the port, and ones that go out from the port.  But often you
	/// won't care about the direction, so some of the properties and methods apply to all links.
	/// The <see cref="P:Northwoods.Go.GoPort.SourceLinks" /> property allows you to enumerate the links that are
	/// coming into a port -- e.g. the links whose <see cref="P:Northwoods.Go.IGoLink.ToPort" /> is this port.
	/// The <see cref="P:Northwoods.Go.GoPort.DestinationLinks" /> property allows you to enumerate the links that are
	/// going out of a port -- e.g. the links whose <see cref="P:Northwoods.Go.IGoLink.FromPort" /> is this port.
	/// The <see cref="P:Northwoods.Go.GoPort.Links" /> property allows you to enumerate all links connected at this port.
	/// The <see cref="P:Northwoods.Go.GoPort.SourceLinksCount" />, <see cref="P:Northwoods.Go.GoPort.DestinationLinksCount" />, and
	/// <see cref="P:Northwoods.Go.GoPort.LinksCount" /> properties give you the size of each of those three collections.
	/// The User Guide provides examples for how to traverse a graph.
	/// </para>
	/// <para>
	/// GoPort provides a fair bit of control for where and how each link will appear
	/// to connect to it.
	/// The most common case is that links connect at a particular spot on the port,
	/// usually the middle of some side.  You can get this behavior by setting the
	/// <see cref="P:Northwoods.Go.GoPort.FromSpot" /> and <see cref="P:Northwoods.Go.GoPort.ToSpot" /> properties.
	/// For example, setting <see cref="P:Northwoods.Go.GoPort.FromSpot" /> to <c>GoObject.MiddleRight</c>
	/// will cause all destination links to come out of the port at the middle of the right side.
	/// Similarly, you can set the <see cref="P:Northwoods.Go.GoPort.ToSpot" /> to be the same spot, or perhaps
	/// a different spot, say <c>GoObject.MiddleLeft</c> if you want links to go
	/// into the port from the left side.
	/// When the spot(s) are set in this manner, each link will have a short end segment
	/// at the port.
	/// </para>
	/// <para>
	/// More generally, you can override the <see cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" /> and/or
	/// <see cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" /> methods to control the exact end point for the link,
	/// for source links and for destination links, respectively.
	/// Similarly, you can override the <see cref="M:Northwoods.Go.GoPort.GetToLinkDir(Northwoods.Go.IGoLink)" /> and/or
	/// <see cref="M:Northwoods.Go.GoPort.GetFromLinkDir(Northwoods.Go.IGoLink)" /> methods to control what direction each link's end
	/// segment should travel.
	/// <see cref="M:Northwoods.Go.GoPort.GetEndSegmentLength(Northwoods.Go.IGoLink,System.Boolean)" /> controls how long each end segment should be,
	/// assuming there is one at all.  If the port spot is <c>GoObject.NoSpot</c>,
	/// there usually won't be an end segment in the link at that port.
	/// </para>
	/// <para>
	/// Many applications do not want to permit the user to draw links between any two ports.
	/// There are semantic rules that may make linking particular pairs of ports invalid.
	/// Or there may be some ports for which only one direction link may be connected.
	/// You can easily specify the most common restrictions by setting one or more of the
	/// following properties: <see cref="P:Northwoods.Go.GoPort.IsValidFrom" />, <see cref="P:Northwoods.Go.GoPort.IsValidTo" />,
	/// <see cref="P:Northwoods.Go.GoPort.IsValidSelfNode" />, <see cref="P:Northwoods.Go.GoPort.IsValidDuplicateLinks" />, and
	/// <see cref="P:Northwoods.Go.GoPort.IsValidSingleLink" />.
	/// </para>
	/// <para>
	/// For the most general or dynamic cases, you can override the <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />
	/// method, either here on <see cref="T:Northwoods.Go.GoPort" />, or on the <see cref="T:Northwoods.Go.GoToolLinkingNew" />
	/// and <see cref="T:Northwoods.Go.GoToolRelinking" /> tools associated with your <see cref="T:Northwoods.Go.GoView" />.
	/// Also consider setting the <see cref="P:Northwoods.Go.GoDocument.ValidCycle" /> property.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoPort : GoShape, IGoPort, IGoGraphPart, IGoIdentifiablePart
	{
		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedPortUserFlags = 1700;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedPortUserObject = 1701;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedStyle = 1702;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedObject = 1703;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedValidFrom = 1704;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedValidTo = 1705;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedValidSelfNode = 1706;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedFromSpot = 1707;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToSpot = 1708;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedAddedLink = 1709;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedRemovedLink = 1710;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedValidDuplicateLinks = 1711;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedEndSegmentLength = 1712;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedPartID = 1713;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedClearsLinksWhenRemoved = 1714;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedValidSingleLink = 1715;

		private const int flagValidFrom = 1048576;

		private const int flagValidTo = 2097152;

		private const int flagValidSelfNode = 4194304;

		private const int flagValidDuplicateLinks = 8388608;

		private const int flagRecursive = 16777216;

		private const int flagClearsLinksWhenRemoved = 33554432;

		private const int flagNoClearLinks = 67108864;

		private const int flagValidSingleLink = 134217728;

		private const int flagInsideCollapsedSubGraph = 268435456;

		private static bool myLinksRedirectedToSubGraphPort;

		private GoPortStyle myStyle = GoPortStyle.Ellipse;

		private GoObject myPortObject;

		private int myFromLinkSpot = 64;

		private int myToLinkSpot = 256;

		private List<IGoLink> myLinks = new List<IGoLink>();

		private float myEndSegmentLength = 10f;

		private int myUserFlags;

		private object myUserObject;

		private int myPartID = -1;

		/// <summary>
		/// Returns itself as a <see cref="P:Northwoods.Go.GoPort.GoObject" />.
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
				return myUserFlags;
			}
			set
			{
				int num = myUserFlags;
				if (num != value)
				{
					myUserFlags = value;
					Changed(1700, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
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
				return myUserObject;
			}
			set
			{
				object obj = myUserObject;
				if (obj != value)
				{
					myUserObject = value;
					Changed(1701, 0, obj, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		IEnumerable<IGoLink> IGoPort.Links => new GoPortLinkEnumerator(myLinks);

		/// <summary>
		/// Gets an enumerator over all of the links connected at this port.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoPort.Links" />
		[Description("Gets an enumerator over all of the links connected at this port.")]
		public virtual GoPortLinkEnumerator Links => new GoPortLinkEnumerator(myLinks);

		IEnumerable<IGoLink> IGoPort.SourceLinks => new GoPortFilteredLinkEnumerator(this, myLinks, dest: false);

		/// <summary>
		/// Gets an enumerator over all of the links coming into this port.
		/// </summary>
		/// <remarks>
		/// Each source link's <see cref="P:Northwoods.Go.IGoLink.ToPort" /> will be this port.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoPort.SourceLinks" />
		[Description("Gets an enumerator over the links coming into this port.")]
		public virtual GoPortFilteredLinkEnumerator SourceLinks => new GoPortFilteredLinkEnumerator(this, myLinks, dest: false);

		IEnumerable<IGoLink> IGoPort.DestinationLinks => new GoPortFilteredLinkEnumerator(this, myLinks, dest: true);

		/// <summary>
		/// Gets an enumerator over all of the links going out of this port.
		/// </summary>
		/// <remarks>
		/// Each destination link's <see cref="P:Northwoods.Go.IGoLink.FromPort" /> will be this port.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoPort.DestinationLinks" />
		[Description("Gets an enumerator over the links going out of this port.")]
		public virtual GoPortFilteredLinkEnumerator DestinationLinks => new GoPortFilteredLinkEnumerator(this, myLinks, dest: true);

		/// <summary>
		/// Gets the total number of links connected at this port.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoPort.LinksCount" />
		[Description("The total number of links connected at this port.")]
		public virtual int LinksCount => myLinks.Count;

		/// <summary>
		/// Gets the number of links coming into this port.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoPort.SourceLinksCount" />
		[Description("The number of links coming into this port.")]
		public virtual int SourceLinksCount
		{
			get
			{
				int num = 0;
				foreach (IGoLink sourceLink in SourceLinks)
				{
					if (sourceLink != null)
					{
						num = checked(num + 1);
					}
				}
				return num;
			}
		}

		/// <summary>
		/// Gets the number of links going out of this port.
		/// </summary>
		/// <remarks>
		/// This is the number of links whose <see cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// is this port.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoPort.DestinationLinksCount" />
		[Description("The number of links going out of this port.")]
		public virtual int DestinationLinksCount
		{
			get
			{
				int num = 0;
				foreach (IGoLink destinationLink in DestinationLinks)
				{
					if (destinationLink != null)
					{
						num = checked(num + 1);
					}
				}
				return num;
			}
		}

		/// <summary>
		/// Gets the node that this port is part of.
		/// </summary>
		/// <remarks>
		/// By default this uses <see cref="M:Northwoods.Go.GoPort.FindParentNode(Northwoods.Go.GoObject)" />, but you
		/// may need to override this to use <see cref="M:Northwoods.Go.GoPort.FindTopNode(Northwoods.Go.GoObject)" />
		/// instead if your node class contains other nodes as parts that
		/// you don't want to consider separate objects.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoPort.Node" />
		[Description("The node that this port is part of.")]
		public virtual IGoNode Node => FindParentNode(this);

		/// <summary>
		/// Gets or sets whether this port can be a valid value for some
		/// link's <see cref="P:Northwoods.Go.IGoLink.FromPort" />.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoPort.CanLinkFrom" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("A flag for whether a valid link can have this port as its FromPort.")]
		public virtual bool IsValidFrom
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
					Changed(1704, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this port can be a valid value for some
		/// link's <see cref="P:Northwoods.Go.IGoLink.ToPort" />.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoPort.CanLinkTo" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("A flag for whether a valid link can have this port as its ToPort.")]
		public virtual bool IsValidTo
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
					Changed(1705, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether a valid link can be made between two ports belonging to the same node.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> uses this property in the following manner:
		/// If this property is true, and the other port's <c>IsValidSelfNode</c> property is also
		/// true, then the proposed link may be valid.
		/// Otherwise, if <see cref="M:Northwoods.Go.GoPort.IsInSameNode(Northwoods.Go.IGoPort)" /> is true, it will not be a valid link.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether a valid link can be made between two ports belonging to the same node.")]
		public virtual bool IsValidSelfNode
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
					Changed(1706, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether a valid link can be made between two ports already connected
		/// by a link in the same direction.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> uses this property in the following manner:
		/// If this property is true, and the other port's <c>IsValidDuplicateLinks</c> property is also
		/// true, then the proposed link may be valid.
		/// Otherwise, if <see cref="M:Northwoods.Go.GoPort.IsLinked(Northwoods.Go.IGoPort)" /> is true, it will not be a valid link.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether a valid link can be made between two ports already connected by a link.")]
		public virtual bool IsValidDuplicateLinks
		{
			get
			{
				return (base.InternalFlags & 0x800000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x800000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 8388608;
					}
					else
					{
						base.InternalFlags &= -8388609;
					}
					Changed(1711, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoPort.CanLinkFrom" /> and <see cref="M:Northwoods.Go.GoPort.CanLinkTo" />
		/// return false when there is already a link connected at this port.
		/// </summary>
		/// <value>
		/// This defaults to false -- an unlimited number of links can be valid.
		/// </value>
		/// <remarks>
		/// If you want to provide different or more general limitations on how
		/// many links can be connected to this port, you may need to override
		/// <see cref="M:Northwoods.Go.GoPort.CanLinkFrom" /> and <see cref="M:Northwoods.Go.GoPort.CanLinkTo" />.
		/// For the most general way to control whether links might be valid,
		/// you may need to override <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />.  But first consider
		/// all of the other <see cref="T:Northwoods.Go.GoPort" /> properties governing link validity.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether this port is limited to having at most one connected link.")]
		public virtual bool IsValidSingleLink
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
					Changed(1715, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the appearance style for this port.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoPortStyle.Ellipse" />.
		/// </value>
		/// <remarks>
		/// If the value changes, the setter will also call <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// to notify all connected links about the change.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoPort.PortObject" />
		[Category("Appearance")]
		[DefaultValue(GoPortStyle.Ellipse)]
		[Description("The appearance style.")]
		public virtual GoPortStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				GoPortStyle goPortStyle = myStyle;
				if (goPortStyle != value)
				{
					myStyle = value;
					Changed(1702, (int)goPortStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(1702, (int)goPortStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoPort.GoObject" /> that may stand in for this port.
		/// </summary>
		/// <value>
		/// The initial value is null.
		/// </value>
		/// <remarks>
		/// When painting this port, if the <see cref="P:Northwoods.Go.GoPort.Style" /> is
		/// <see cref="F:Northwoods.Go.GoPortStyle.Object" />, then the painting is delegated
		/// to the value of this property, which should be an object not
		/// owned by any layer or group.
		/// <see cref="M:Northwoods.Go.GoPort.GetNearestIntersectionPoint(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF@)" /> delegates to the value of this property
		/// when the <see cref="P:Northwoods.Go.GoPort.Style" /> is not <see cref="F:Northwoods.Go.GoPortStyle.Object" />
		/// and the port object does belong to a layer.
		/// The value must not be set to another port.
		/// If the value changes, the setter will also call <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// to notify all connected links about the change.
		/// </remarks>
		[Description("The GoObject that may take this port's place and appearance or shape.")]
		public virtual GoObject PortObject
		{
			get
			{
				return myPortObject;
			}
			set
			{
				GoObject goObject = myPortObject;
				if (goObject != value)
				{
					myPortObject = value;
					Changed(1703, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(1703, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoPort.GoObject" /> spot at which to connect
		/// links coming out from this port.
		/// </summary>
		/// <value>
		/// The default value is <c>GoObject.MiddleRight</c>.
		/// </value>
		/// <remarks>
		/// This property is used for the common case where we know where and
		/// in which direction we expect to attach links to ports.
		/// The default assumes a left-to-right flow for the graph.
		/// If the value changes, the setter will also call <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// to notify all connected links about the change.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />
		[Category("Appearance")]
		[DefaultValue(64)]
		[Description("The spot for attaching links coming out from this port.")]
		public virtual int FromSpot
		{
			get
			{
				return myFromLinkSpot;
			}
			set
			{
				int num = myFromLinkSpot;
				if (num != value)
				{
					myFromLinkSpot = value;
					Changed(1707, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(1707, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoPort.GoObject" /> spot at which to connect
		/// links going into this port.
		/// </summary>
		/// <value>
		/// The default value is <c>GoObject.MiddleLeft</c>.
		/// </value>
		/// <remarks>
		/// This property is used for the common case where we know where and
		/// in which direction we expect to attach links to ports.
		/// The default assumes a left-to-right flow for the graph.
		/// If the value changes, the setter will also call <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// to notify all connected links about the change.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />
		[Category("Appearance")]
		[DefaultValue(256)]
		[Description("The spot for attaching links going into this port.")]
		public virtual int ToSpot
		{
			get
			{
				return myToLinkSpot;
			}
			set
			{
				int num = myToLinkSpot;
				if (num != value)
				{
					myToLinkSpot = value;
					Changed(1708, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(1708, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether links connected to ports that are inside collapsed <see cref="T:Northwoods.Go.GoSubGraph" />s
		/// appear to be connected to the subgraph's <see cref="P:Northwoods.Go.GoSubGraph.Port" />.
		/// </summary>
		/// <value>
		/// This value is initially false, providing compatibility with older versions of GoDiagram.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property affects the following methods:
		/// <see cref="M:Northwoods.Go.GoPort.GetFromLinkDir(Northwoods.Go.IGoLink)" />,
		/// <see cref="M:Northwoods.Go.GoPort.GetToLinkDir(Northwoods.Go.IGoLink)" />,
		/// <see cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />,
		/// <see cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />, and
		/// <see cref="M:Northwoods.Go.GoPort.GetLinkPointFromPoint(System.Drawing.PointF)" />.
		/// </para>
		/// <para>
		/// When this static/shared property is true, any link inside a collapsed <see cref="T:Northwoods.Go.GoSubGraph" />
		/// will appear to be redirected to the first visible collapsed parent subgraph,
		/// if that subgraph has a <see cref="P:Northwoods.Go.GoSubGraph.Port" />.
		/// Note that no structural changes are made to any ports or to any links -- only the
		/// point and direction at which links connect, so that they will now appear to be
		/// connected to the subgraph port.
		/// Also please note that by default <see cref="M:Northwoods.Go.GoSubGraph.CreatePort" /> returns null/nothing,
		/// so a subgraph will not have a port unless you either override that method to return
		/// a new port or you assign the <see cref="P:Northwoods.Go.GoSubGraph.Port" /> property.
		/// </para>
		/// </remarks>
		public static bool LinksRedirectedToSubGraphPort
		{
			get
			{
				return myLinksRedirectedToSubGraphPort;
			}
			set
			{
				myLinksRedirectedToSubGraphPort = value;
			}
		}

		internal bool InsideCollapsedSubGraph
		{
			get
			{
				return (base.InternalFlags & 0x10000000) != 0;
			}
			set
			{
				if ((base.InternalFlags & 0x10000000) != 0 != value)
				{
					if (value)
					{
						base.InternalFlags |= 268435456;
					}
					else
					{
						base.InternalFlags &= -268435457;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the length of the link segment closest to this port.
		/// </summary>
		/// <value>
		/// This <c>float</c> value is in document coordinates.
		/// By default this value is <c>10</c>.
		/// </value>
		/// <remarks>
		/// Basically this tells <see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> how long
		/// a line segment to draw straight out from the port before turning,
		/// assuming a well-defined (non-NoSpot) link spot for this port.
		/// If the value changes, the setter will also call <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// to notify all connected links about the change.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromEndSegmentLength(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToEndSegmentLength(Northwoods.Go.IGoLink)" />
		[Description("The length of the link segment closest to this port.")]
		public virtual float EndSegmentLength
		{
			get
			{
				return myEndSegmentLength;
			}
			set
			{
				float num = myEndSegmentLength;
				if (num != value)
				{
					myEndSegmentLength = value;
					Changed(1712, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LinksOnPortChanged(1712, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this port calls <see cref="M:Northwoods.Go.GoPort.ClearLinks" /> when it is
		/// removed from a <see cref="T:Northwoods.Go.GoLayer" /> (i.e., from a document, normally).
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoPort.OnLayerChanged(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer,Northwoods.Go.GoObject)" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether removing a port from its document causes its attached links to be removed too.")]
		public virtual bool ClearsLinksWhenRemoved
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
					Changed(1714, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		internal bool NoClearLinks
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				if (value)
				{
					base.InternalFlags |= 67108864;
				}
				else
				{
					base.InternalFlags &= -67108865;
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
					Changed(1713, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a port whose appearance is a filled black ellipse,
		/// that is both a valid source and destination for linking, that assumes that
		/// source links come in from the left and that destination links depart out
		/// to the right, and that is not selectable or resizable by the user.
		/// </summary>
		public GoPort()
		{
			base.InternalFlags &= -19;
			base.InternalFlags |= 36700160;
			Brush = GoShape.Brushes_Black;
		}

		/// <summary>
		/// Copying a port does not immediately cause its links to be copied.
		/// </summary>
		/// <param name="env"></param>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoPort.PortObject" /> is only copied if it belongs to
		/// a document; otherwise it remains shared with the original port.
		/// </remarks>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoPort goPort = (GoPort)base.CopyObject(env);
			if (goPort != null)
			{
				goPort.myLinks = new List<IGoLink>();
				goPort.myPartID = -1;
				if (myPortObject != null)
				{
					env.Delayeds.Add(this);
				}
			}
			return goPort;
		}

		/// <summary>
		/// Make sure any unshared <see cref="P:Northwoods.Go.GoPort.PortObject" /> is copied.
		/// </summary>
		/// <param name="env"></param>
		/// <param name="newobj"></param>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoPort.PortObject" /> is a shared object, both the
		/// new port and the old port refer to the same object.
		/// </remarks>
		public override void CopyObjectDelayed(GoCopyDictionary env, GoObject newobj)
		{
			base.CopyObjectDelayed(env, newobj);
			GoPort goPort = (GoPort)newobj;
			GoObject goObject = env[myPortObject] as GoObject;
			if (goObject != null)
			{
				goPort.myPortObject = goObject;
			}
		}

		/// <summary>
		/// Add a link to this port's collection of connected links,
		/// making sure the link's ToPort is this port so that the
		/// link is a connection from another source port.
		/// </summary>
		/// <param name="link"></param>
		/// <remarks>
		/// If the link is actually added to this port's collection of links,
		/// this will call <see cref="M:Northwoods.Go.GoPort.OnLinkChanged(Northwoods.Go.IGoLink,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> to allow the port to
		/// update itself.  For example, this may be useful to change the
		/// appearance of the port as the number of connected links changes.
		/// </remarks>
		public virtual void AddSourceLink(IGoLink link)
		{
			link.ToPort = this;
			if (link.ToPort == this)
			{
				InternalAddLink(link, undoing: false);
			}
		}

		/// <summary>
		/// Add a link to this port's collection of connected links,
		/// making sure the link's FromPort is this port so that the
		/// link is a connection to another destination port.
		/// </summary>
		/// <param name="link"></param>
		/// <remarks>
		/// If the link is actually added to this port's collection of links,
		/// this will call <see cref="M:Northwoods.Go.GoPort.OnLinkChanged(Northwoods.Go.IGoLink,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> to allow the port to
		/// update itself.  For example, this may be useful to change the
		/// appearance of the port as the number of connected links changes.
		/// </remarks>
		public virtual void AddDestinationLink(IGoLink link)
		{
			link.FromPort = this;
			if (link.FromPort == this)
			{
				InternalAddLink(link, undoing: false);
			}
		}

		private void InternalAddLink(IGoLink link, bool undoing)
		{
			if (!myLinks.Contains(link))
			{
				myLinks.Add(link);
				Changed(1709, 0, link, GoObject.NullRect, 0, link, GoObject.NullRect);
				OnLinkChanged(link, 1709, 0, link, GoObject.NullRect, 0, link, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Remove a link from this port's collection of connected links.
		/// </summary>
		/// <param name="link"></param>
		/// <remarks>
		/// If the link is actually removed from this port's collection of links,
		/// this will call <see cref="M:Northwoods.Go.GoPort.OnLinkChanged(Northwoods.Go.IGoLink,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> to allow the port to
		/// update itself.  For example, this may be useful to change the
		/// appearance of the port as the number of connected links changes.
		/// </remarks>
		public virtual void RemoveLink(IGoLink link)
		{
			InternalRemoveLink(link, undoing: false);
		}

		private void InternalRemoveLink(IGoLink link, bool undoing)
		{
			int num = myLinks.IndexOf(link);
			if (num >= 0)
			{
				myLinks.RemoveAt(num);
				Changed(1710, 0, link, GoObject.NullRect, 0, link, GoObject.NullRect);
				OnLinkChanged(link, 1710, 0, link, GoObject.NullRect, 0, link, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Determine if a link is connected at this port.
		/// </summary>
		/// <param name="l"></param>
		/// <returns>
		/// True if <paramref name="l" /> is in this port's collection of connected links.
		/// </returns>
		public virtual bool ContainsLink(IGoLink l)
		{
			return myLinks.Contains(l);
		}

		/// <summary>
		/// Remove all links connected to this port.
		/// </summary>
		/// <remarks>
		/// Normally this will just call <see cref="M:Northwoods.Go.IGoLink.Unlink" /> on each link.
		/// However, this method does not do so if the link is in a document
		/// but the port is in a view, or vice versa.
		/// </remarks>
		public virtual void ClearLinks()
		{
			ClearLinks(null);
		}

		private void ClearLinks(GoObject mainObj)
		{
			IGoLayerCollectionContainer goLayerCollectionContainer = (base.Layer != null) ? base.Layer.LayerCollectionContainer : null;
			int num = myLinks.Count;
			while (num > 0)
			{
				IGoLink goLink = myLinks[num = checked(num - 1)];
				GoObject goObject = goLink.GoObject;
				if (goObject == null || goObject.Layer == null || (goObject.Layer.LayerCollectionContainer == goLayerCollectionContainer && (mainObj == null || (!goObject.IsChildOf(mainObj) && !goObject.Movable))))
				{
					goLink.Unlink();
					num = Math.Min(num, myLinks.Count);
				}
			}
		}

		/// <summary>
		/// Returns a newly allocated array containing references to all of the
		/// links connected to this port.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.IGoPort.CopyLinksArray" />
		[Description("A array copy of all of the links connected at this port.")]
		public virtual IGoLink[] CopyLinksArray()
		{
			return myLinks.ToArray();
		}

		/// <summary>
		/// Return the most inclusive node containing an object.
		/// </summary>
		/// <param name="x"></param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoNode" /> that is as close to top-level in the
		/// <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Parent</c> hierarchy as possible.
		/// If <paramref name="x" /> is null, this returns null;
		/// If there is no <see cref="T:Northwoods.Go.IGoNode" /> that contains the object,
		/// this will return null.
		/// </returns>
		public static IGoNode FindTopNode(GoObject x)
		{
			if (x == null)
			{
				return null;
			}
			if (x.IsTopLevel)
			{
				return x as IGoNode;
			}
			IGoNode goNode = FindTopNode(x.Parent);
			if (goNode != null)
			{
				return goNode;
			}
			return x as IGoNode;
		}

		/// <summary>
		/// Return the node that immediately contains an object, or return the object
		/// itself if it is an <see cref="T:Northwoods.Go.IGoNode" />.
		/// </summary>
		/// <param name="x"></param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoNode" /> that either is <paramref name="x" /> or the first
		/// <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Parent</c> going up the part hierarchy.
		/// If <paramref name="x" /> is null, this returns null.
		/// </returns>
		public static IGoNode FindParentNode(GoObject x)
		{
			if (x == null)
			{
				return null;
			}
			IGoNode goNode = x as IGoNode;
			if (goNode != null)
			{
				return goNode;
			}
			return FindParentNode(x.Parent);
		}

		/// <summary>
		/// True if it is valid to create a link from this port to <paramref name="toPort" />.
		/// </summary>
		/// <param name="toPort"></param>
		/// <remarks>
		/// <para>
		/// By default this is true when <paramref name="toPort" /> is not the same as this
		/// port, <see cref="M:Northwoods.Go.GoPort.CanLinkFrom" /> is true, and <see cref="M:Northwoods.Go.GoPort.CanLinkTo" /> is true
		/// for the <paramref name="toPort" /> port.
		/// Furthermore, this considers the <see cref="P:Northwoods.Go.GoPort.IsValidSelfNode" />, the
		/// <see cref="P:Northwoods.Go.GoPort.IsValidDuplicateLinks" />, and the <see cref="P:Northwoods.Go.GoDocument.ValidCycle" />
		/// properties.
		/// </para>
		/// <para>
		/// This is called by <see cref="T:Northwoods.Go.GoToolLinkingNew" />.<see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> and
		/// <see cref="T:Northwoods.Go.GoToolRelinking" />.<see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />.
		/// </para>
		/// </remarks>
		public virtual bool IsValidLink(IGoPort toPort)
		{
			if (CanLinkFrom() && toPort != null && toPort.CanLinkTo() && ((IsValidSelfNode && toPort.GoObject is GoPort && ((GoPort)toPort.GoObject).IsValidSelfNode) || !IsInSameNode(toPort)) && ((IsValidDuplicateLinks && toPort.GoObject is GoPort && ((GoPort)toPort.GoObject).IsValidDuplicateLinks) || !IsLinked(toPort)))
			{
				return CycleOK(toPort);
			}
			return false;
		}

		private bool CycleOK(IGoPort toPort)
		{
			GoDocument document = base.Document;
			if (document == null)
			{
				return true;
			}
			switch (document.ValidCycle)
			{
			default:
				return true;
			case GoDocumentValidCycle.NotDirected:
				return !GoDocument.MakesDirectedCycle(Node, toPort.Node);
			case GoDocumentValidCycle.NotDirectedFast:
				return !GoDocument.MakesDirectedCycleFast(Node, toPort.Node);
			case GoDocumentValidCycle.NotUndirected:
				return !GoDocument.MakesUndirectedCycle(Node, toPort.Node);
			case GoDocumentValidCycle.DestinationTree:
				if (toPort.SourceLinksCount == 0)
				{
					return !GoDocument.MakesDirectedCycleFast(Node, toPort.Node);
				}
				return false;
			case GoDocumentValidCycle.SourceTree:
				if (DestinationLinksCount == 0)
				{
					return !GoDocument.MakesDirectedCycleFast(Node, toPort.Node);
				}
				return false;
			}
		}

		/// <summary>
		/// This predicate should be true if, by itself, there is no known
		/// reason why one couldn't create a valid link from this port to some port.
		/// </summary>
		/// <remarks>
		/// To be true, this port must pass <see cref="P:Northwoods.Go.GoPort.IsValidFrom" />,
		/// <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>CanView</c>, and <c>.Layer.CanLinkObjects</c> (if it
		/// belongs to a layer).
		/// Furthermore, if <see cref="P:Northwoods.Go.GoPort.IsValidSingleLink" /> is true,
		/// the value of <see cref="P:Northwoods.Go.GoPort.LinksCount" /> must be zero.
		/// To control the validity of link creation considering two ports,
		/// override <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> or set one of the properties
		/// that that method depends on.
		/// </remarks>
		public virtual bool CanLinkFrom()
		{
			if (!IsValidFrom)
			{
				return false;
			}
			if (IsValidSingleLink && LinksCount >= 1)
			{
				return false;
			}
			if (!CanView())
			{
				return false;
			}
			if (base.Layer != null && !base.Layer.CanLinkObjects())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// This predicate should be true if, by itself, there is no known
		/// reason why one couldn't create a valid link from some port to this one.
		/// </summary>
		/// <remarks>
		/// To be true, this port must pass <see cref="P:Northwoods.Go.GoPort.IsValidTo" />,
		/// <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>CanView</c>, and <c>.Layer.CanLinkObjects</c> (if it
		/// belongs to a layer).
		/// Furthermore, if <see cref="P:Northwoods.Go.GoPort.IsValidSingleLink" /> is true,
		/// the value of <see cref="P:Northwoods.Go.GoPort.LinksCount" /> must be zero.
		/// To control the validity of link creation considering two ports,
		/// override <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> or set one of the properties
		/// that that method depends on.
		/// </remarks>
		public virtual bool CanLinkTo()
		{
			if (!IsValidTo)
			{
				return false;
			}
			if (IsValidSingleLink && LinksCount >= 1)
			{
				return false;
			}
			if (!CanView())
			{
				return false;
			}
			if (base.Layer != null && !base.Layer.CanLinkObjects())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// This static method determines if both arguments belong to the same IGoNode.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>
		/// True if both ports are part of the same <see cref="T:Northwoods.Go.IGoNode" />.
		/// If the ports are not part of an <see cref="T:Northwoods.Go.IGoNode" /> (but only contained
		/// by a <see cref="T:Northwoods.Go.GoGroup" />), their <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>TopLevelObject</c>s
		/// are compared instead.
		/// </returns>
		/// <seealso cref="P:Northwoods.Go.GoPort.Node" />
		/// <seealso cref="M:Northwoods.Go.GoPort.IsInSameNode(Northwoods.Go.IGoPort)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.FindParentNode(Northwoods.Go.GoObject)" />
		public static bool IsInSameNode(IGoPort a, IGoPort b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a == b)
			{
				return true;
			}
			object obj = a.Node;
			if (obj == null && a.GoObject != null)
			{
				obj = a.GoObject.TopLevelObject;
			}
			object obj2 = b.Node;
			if (obj2 == null && b.GoObject != null)
			{
				obj2 = b.GoObject.TopLevelObject;
			}
			if (obj != null)
			{
				return obj == obj2;
			}
			return false;
		}

		/// <summary>
		/// Determine if this port is in the same group as another port.
		/// </summary>
		/// <param name="p"></param>
		/// <returns>
		/// True if this port is part of the same <see cref="T:Northwoods.Go.GoGroup" />
		/// as <paramref name="p" />.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.GoPort.IsInSameNode(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />
		public virtual bool IsInSameNode(IGoPort p)
		{
			return IsInSameNode(this, p);
		}

		/// <summary>
		/// This static method determines if there is a link going from one port to another.
		/// </summary>
		/// <param name="a">a source port</param>
		/// <param name="b">a destination port</param>
		/// <returns>
		/// True if there is an <see cref="T:Northwoods.Go.IGoLink" /> from <paramref name="a" />
		/// to <paramref name="b" />.
		/// Note that a link in the opposite direction will not satisfy this predicate.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.GoPort.IsLinked(Northwoods.Go.IGoPort)" />
		public static bool IsLinked(IGoPort a, IGoPort b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			GoPort goPort = b as GoPort;
			if (goPort != null)
			{
				foreach (IGoLink link in goPort.Links)
				{
					IGoPort fromPort = link.FromPort;
					IGoPort toPort = link.ToPort;
					if (fromPort == a && toPort == b)
					{
						return true;
					}
				}
			}
			else
			{
				foreach (IGoLink link2 in b.Links)
				{
					IGoPort fromPort2 = link2.FromPort;
					IGoPort toPort2 = link2.ToPort;
					if (fromPort2 == a && toPort2 == b)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Determine if there is a link from this port to another port.
		/// </summary>
		/// <param name="p">a destination port</param>
		/// <returns>
		/// True if there is an <see cref="T:Northwoods.Go.IGoLink" /> from this port
		/// to <paramref name="p" />.
		/// Note that a link in the opposite direction, that is one whose
		/// <see cref="P:Northwoods.Go.IGoLink.FromPort" /> is <paramref name="p" /> and
		/// whose <see cref="P:Northwoods.Go.IGoLink.ToPort" /> is this port,
		/// will not satisfy this predicate.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.GoPort.IsLinked(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />
		public virtual bool IsLinked(IGoPort p)
		{
			return IsLinked(this, p);
		}

		/// <summary>
		/// Draw a port as either a simple shape using a Pen and Brush,
		/// as an arbitrary object, or not at all.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// By default this method uses the <see cref="P:Northwoods.Go.GoPort.Style" /> property to
		/// determine the general shape of the port.
		/// When the <see cref="P:Northwoods.Go.GoPort.Style" /> is <see cref="T:Northwoods.Go.GoPortStyle" />.<see cref="F:Northwoods.Go.GoPortStyle.None" />,
		/// nothing is painted.
		/// When the <see cref="P:Northwoods.Go.GoPort.Style" /> is <see cref="T:Northwoods.Go.GoPortStyle" />.<see cref="F:Northwoods.Go.GoPortStyle.Object" />
		/// and the <see cref="P:Northwoods.Go.GoPort.PortObject" /> is an object that does not
		/// belong to any <see cref="T:Northwoods.Go.GoLayer" />,
		/// we set the <see cref="P:Northwoods.Go.GoPort.PortObject" />'s <c>Bounds</c>
		/// property to this port's bounds before painting that <see cref="P:Northwoods.Go.GoPort.PortObject" />.
		/// If the <see cref="P:Northwoods.Go.GoPort.Style" /> is not <see cref="F:Northwoods.Go.GoPortStyle.Object" />,
		/// or if the <see cref="P:Northwoods.Go.GoPort.PortObject" /> belongs to a layer
		/// (typically because it belongs to this port's node or it is the node itself),
		/// then the <see cref="P:Northwoods.Go.GoPort.PortObject" /> is ignored here -- it will be
		/// painted by the normal mechanisms and not specially for this port.
		/// This ignores the <see cref="P:Northwoods.Go.GoPort.GoObject" />.<c>Shadowed</c> property.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			if (PaintGreek(g, view))
			{
				return;
			}
			RectangleF bounds = Bounds;
			switch (Style)
			{
			case GoPortStyle.None:
				break;
			case GoPortStyle.Object:
			{
				GoObject portObject = PortObject;
				if (portObject != null && portObject.Layer == null)
				{
					portObject.Bounds = bounds;
					portObject.Paint(g, view);
				}
				break;
			}
			case GoPortStyle.Triangle:
			case GoPortStyle.TriangleTopLeft:
			case GoPortStyle.TriangleTopRight:
			case GoPortStyle.TriangleBottomRight:
			case GoPortStyle.TriangleBottomLeft:
			case GoPortStyle.TriangleMiddleTop:
			case GoPortStyle.TriangleMiddleRight:
			case GoPortStyle.TriangleMiddleBottom:
			case GoPortStyle.TriangleMiddleLeft:
			{
				PointF[] array2 = view.AllocTempPointArray(3);
				ComputeTrianglePoints(array2);
				GoShape.DrawPolygon(g, view, Pen, Brush, array2);
				view.FreeTempPointArray(array2);
				break;
			}
			case GoPortStyle.Rectangle:
				GoShape.DrawRectangle(g, view, Pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			case GoPortStyle.Diamond:
			{
				PointF[] array = view.AllocTempPointArray(4);
				array[0].X = bounds.X + bounds.Width / 2f;
				array[0].Y = bounds.Y;
				array[1].X = bounds.X + bounds.Width;
				array[1].Y = bounds.Y + bounds.Height / 2f;
				array[2].X = array[0].X;
				array[2].Y = bounds.Y + bounds.Height;
				array[3].X = bounds.X;
				array[3].Y = array[1].Y;
				GoShape.DrawPolygon(g, view, Pen, Brush, array);
				view.FreeTempPointArray(array);
				break;
			}
			case GoPortStyle.Plus:
				GoShape.DrawLine(g, view, Pen, bounds.X + bounds.Width / 2f, bounds.Y, bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height);
				GoShape.DrawLine(g, view, Pen, bounds.X, bounds.Y + bounds.Height / 2f, bounds.X + bounds.Width, bounds.Y + bounds.Height / 2f);
				break;
			case GoPortStyle.Times:
				GoShape.DrawLine(g, view, Pen, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
				GoShape.DrawLine(g, view, Pen, bounds.X + bounds.Width, bounds.Y, bounds.X, bounds.Y + bounds.Height);
				break;
			case GoPortStyle.PlusTimes:
				GoShape.DrawLine(g, view, Pen, bounds.X + bounds.Width / 2f, bounds.Y, bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height);
				GoShape.DrawLine(g, view, Pen, bounds.X, bounds.Y + bounds.Height / 2f, bounds.X + bounds.Width, bounds.Y + bounds.Height / 2f);
				GoShape.DrawLine(g, view, Pen, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
				GoShape.DrawLine(g, view, Pen, bounds.X + bounds.Width, bounds.Y, bounds.X, bounds.Y + bounds.Height);
				break;
			default:
				GoShape.DrawEllipse(g, view, Pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			}
		}

		private void ComputeTrianglePoints(PointF[] v)
		{
			RectangleF bounds = Bounds;
			int num = 1;
			switch (Style)
			{
			default:
				num = ToSpot;
				break;
			case GoPortStyle.TriangleBottomRight:
				num = 2;
				break;
			case GoPortStyle.TriangleMiddleBottom:
				num = 32;
				break;
			case GoPortStyle.TriangleBottomLeft:
				num = 4;
				break;
			case GoPortStyle.TriangleMiddleLeft:
				num = 64;
				break;
			case GoPortStyle.TriangleTopLeft:
				num = 8;
				break;
			case GoPortStyle.TriangleMiddleTop:
				num = 128;
				break;
			case GoPortStyle.TriangleTopRight:
				num = 16;
				break;
			case GoPortStyle.TriangleMiddleRight:
				num = 256;
				break;
			}
			switch (num)
			{
			case 2:
				v[0].X = bounds.X + bounds.Width / 2f;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y + bounds.Height / 2f;
				break;
			case 32:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width / 2f;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y;
				break;
			case 4:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y + bounds.Height / 2f;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X + bounds.Width / 2f;
				v[2].Y = bounds.Y;
				break;
			case 64:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y + bounds.Height / 2f;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y;
				break;
			case 8:
				v[0].X = bounds.X + bounds.Width / 2f;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y + bounds.Height / 2f;
				break;
			case 128:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X + bounds.Width / 2f;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			case 16:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y + bounds.Height / 2f;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width / 2f;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			default:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y + bounds.Height / 2f;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			}
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> representing this shape.
		/// </summary>
		/// <returns>a newly allocated <c>GraphicsPath</c></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			RectangleF bounds = Bounds;
			switch (Style)
			{
			case GoPortStyle.None:
				graphicsPath.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);
				break;
			case GoPortStyle.Object:
				graphicsPath.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);
				break;
			case GoPortStyle.Triangle:
			case GoPortStyle.TriangleTopLeft:
			case GoPortStyle.TriangleTopRight:
			case GoPortStyle.TriangleBottomRight:
			case GoPortStyle.TriangleBottomLeft:
			case GoPortStyle.TriangleMiddleTop:
			case GoPortStyle.TriangleMiddleRight:
			case GoPortStyle.TriangleMiddleBottom:
			case GoPortStyle.TriangleMiddleLeft:
			{
				PointF[] array2 = new PointF[3];
				ComputeTrianglePoints(array2);
				graphicsPath.AddPolygon(array2);
				break;
			}
			case GoPortStyle.Rectangle:
				graphicsPath.AddRectangle(bounds);
				break;
			case GoPortStyle.Diamond:
			{
				PointF[] array = new PointF[4];
				array[0].X = bounds.X + bounds.Width / 2f;
				array[0].Y = bounds.Y;
				array[1].X = bounds.X + bounds.Width;
				array[1].Y = bounds.Y + bounds.Height / 2f;
				array[2].X = array[0].X;
				array[2].Y = bounds.Y + bounds.Height;
				array[3].X = bounds.X;
				array[3].Y = array[1].Y;
				graphicsPath.AddPolygon(array);
				break;
			}
			case GoPortStyle.Plus:
				graphicsPath.AddLine(bounds.X + bounds.Width / 2f, bounds.Y, bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height);
				graphicsPath.StartFigure();
				graphicsPath.AddLine(bounds.X, bounds.Y + bounds.Height / 2f, bounds.X + bounds.Width, bounds.Y + bounds.Height / 2f);
				break;
			case GoPortStyle.Times:
				graphicsPath.AddLine(bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
				graphicsPath.StartFigure();
				graphicsPath.AddLine(bounds.X + bounds.Width, bounds.Y, bounds.X, bounds.Y + bounds.Height);
				break;
			case GoPortStyle.PlusTimes:
				graphicsPath.AddLine(bounds.X + bounds.Width / 2f, bounds.Y, bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height);
				graphicsPath.StartFigure();
				graphicsPath.AddLine(bounds.X, bounds.Y + bounds.Height / 2f, bounds.X + bounds.Width, bounds.Y + bounds.Height / 2f);
				graphicsPath.StartFigure();
				graphicsPath.AddLine(bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
				graphicsPath.StartFigure();
				graphicsPath.AddLine(bounds.X + bounds.Width, bounds.Y, bounds.X, bounds.Y + bounds.Height);
				break;
			default:
				graphicsPath.AddEllipse(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			}
			return graphicsPath;
		}

		/// <summary>
		/// Because ports are normally small objects, we heed the
		/// <see cref="P:Northwoods.Go.GoView.PaintNothingScale" /> and <see cref="P:Northwoods.Go.GoView.PaintGreekScale" />
		/// properties.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <remarks>
		/// Greeking of a port is accomplished by just drawing a rectangle with
		/// the port's bounds.
		/// If <see cref="P:Northwoods.Go.GoView.IsPrinting" /> is true, we reduce the scales at
		/// which we paint nothing or paint greek, so that the printed representation
		/// will have more detail than the on-screen one.
		/// </remarks>
		public virtual bool PaintGreek(Graphics g, GoView view)
		{
			float docScale = view.DocScale;
			float num = view.PaintNothingScale / view.WorldScale.Height;
			float num2 = view.PaintGreekScale / view.WorldScale.Height;
			if (view.IsPrinting)
			{
				num /= 4f;
				num2 /= 4f;
			}
			if (docScale <= num)
			{
				return true;
			}
			if (docScale <= num2)
			{
				if (Style != 0)
				{
					RectangleF bounds = Bounds;
					GoShape.DrawRectangle(g, view, Pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// If the <see cref="P:Northwoods.Go.GoPort.Style" /> is <see cref="F:Northwoods.Go.GoPortStyle.Object" />, we
		/// need to consider the <see cref="P:Northwoods.Go.GoPort.PortObject" />'s paint bounds.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			GoObject portObject = PortObject;
			if (portObject != null && portObject != this && Style == GoPortStyle.Object && portObject.Layer == null && (base.InternalFlags & 0x1000000) == 0)
			{
				base.InternalFlags |= 16777216;
				RectangleF result = portObject.ExpandPaintBounds(rect, view);
				base.InternalFlags &= -16777217;
				return result;
			}
			return base.ExpandPaintBounds(rect, view);
		}

		/// <summary>
		/// If there is a <see cref="P:Northwoods.Go.GoPort.PortObject" /> and the <see cref="P:Northwoods.Go.GoPort.Style" />
		/// is not <see cref="T:Northwoods.Go.GoPortStyle" />.<see cref="F:Northwoods.Go.GoPortStyle.Object" />,
		/// return the closest intersection point of the given line with that object.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		/// <remarks>
		/// This supports the case where the <see cref="P:Northwoods.Go.GoPort.PortObject" /> is either
		/// a child of this port's node, or is the whole node itself.
		/// If there is no <see cref="P:Northwoods.Go.GoPort.PortObject" /> or if the <see cref="P:Northwoods.Go.GoPort.Style" />
		/// is <see cref="F:Northwoods.Go.GoPortStyle.Object" />, we just return the default
		/// result which assumes the port is just a rectangle.
		/// </remarks>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			GoObject portObject = PortObject;
			if (portObject != null && portObject != this && Style != GoPortStyle.Object && (base.InternalFlags & 0x1000000) == 0 && (portObject.Layer != null || GoObject.FindCommonParent(this, portObject) != null))
			{
				base.InternalFlags |= 16777216;
				bool nearestIntersectionPoint = portObject.GetNearestIntersectionPoint(p1, p2, out result);
				base.InternalFlags &= -16777217;
				return nearestIntersectionPoint;
			}
			return base.GetNearestIntersectionPoint(p1, p2, out result);
		}

		/// <summary>
		/// Change the cursor to a "hand" cursor if the user can draw a new link at this port.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// "hand" if the <see cref="M:Northwoods.Go.GoView.CanLinkObjects" /> is true and if
		/// <see cref="M:Northwoods.Go.GoPort.CanLinkFrom" /> or <see cref="M:Northwoods.Go.GoPort.CanLinkTo" /> are true,
		/// null if the user cannot start drawing a new link from or to this port
		/// </returns>
		public override string GetCursorName(GoView view)
		{
			if (view != null && !view.CanLinkObjects())
			{
				return null;
			}
			if (!CanLinkFrom() && !CanLinkTo())
			{
				return null;
			}
			return "hand";
		}

		internal GoSubGraph FindVisibleCollapsedSubGraph()
		{
			if (!LinksRedirectedToSubGraphPort)
			{
				return null;
			}
			if (!InsideCollapsedSubGraph)
			{
				return null;
			}
			GoObject parent = base.Parent;
			GoSubGraph result = null;
			while (parent != null)
			{
				GoSubGraph goSubGraph = parent as GoSubGraph;
				if (goSubGraph != null && goSubGraph.CanView())
				{
					if (goSubGraph.State != GoSubGraphState.Collapsed)
					{
						return result;
					}
					result = goSubGraph;
				}
				parent = parent.Parent;
			}
			return result;
		}

		/// <summary>
		/// Determines the actual point at which
		/// a link coming out from this port should originate.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		/// <remarks>
		/// When the value of <see cref="P:Northwoods.Go.GoPort.FromSpot" /> is not <c>GoObject.NoSpot</c>,
		/// this just returns that spot's location.
		/// Otherwise this returns <see cref="M:Northwoods.Go.GoPort.GetLinkPointFromPoint(System.Drawing.PointF)" /> for
		/// a point near the end of the link.
		/// You may wish to override this method if you want to customize
		/// the link point connection dynamically.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />
		public virtual PointF GetFromLinkPoint(IGoLink link)
		{
			GoSubGraph goSubGraph = FindVisibleCollapsedSubGraph();
			if (goSubGraph != null && goSubGraph.Port != null && goSubGraph.Port != this && !link.GoObject.IsChildOf(goSubGraph))
			{
				return goSubGraph.Port.GetFromLinkPoint(link);
			}
			if (FromSpot != 0)
			{
				GoObject goObject = PortObject;
				if (goObject == null || (goObject.Layer == null && GoObject.FindCommonParent(this, goObject) == null))
				{
					goObject = this;
				}
				return goObject.GetSpotLocation(FromSpot);
			}
			if (link == null || link.ToPort == null || link.ToPort.GoObject == null)
			{
				return base.Center;
			}
			GoLink goLink = link as GoLink;
			if (goLink == null && link is GoLabeledLink)
			{
				goLink = (link as GoLabeledLink).RealLink;
			}
			PointF point;
			if (goLink != null && goLink.PointsCount > (goLink.Orthogonal ? 6 : 2))
			{
				point = goLink.GetPoint(1);
				if (goLink.Orthogonal)
				{
					point = OrthoPointToward(point);
					return GetLinkPointFromPoint(point);
				}
				PointF linkPointFromPoint = GetLinkPointFromPoint(point);
				if (linkPointFromPoint == base.Center)
				{
					linkPointFromPoint = GetLinkPointFromPoint(goLink.GetPoint(2));
					if (linkPointFromPoint == base.Center && goLink.PointsCount > 3)
					{
						linkPointFromPoint = GetLinkPointFromPoint(goLink.GetPoint(checked(goLink.PointsCount - 1)));
					}
				}
				return linkPointFromPoint;
			}
			point = link.ToPort.GoObject.Center;
			if (goLink != null && goLink.Orthogonal)
			{
				point = OrthoPointToward(point);
			}
			return GetLinkPointFromPoint(point);
		}

		private PointF OrthoPointToward(PointF p)
		{
			PointF center = base.Center;
			if (Math.Abs(p.X - center.X) > Math.Abs(p.Y - center.Y))
			{
				if (p.X >= center.X)
				{
					p.X = 9999999f;
				}
				else
				{
					p.X = -9999999f;
				}
				p.Y = center.Y;
			}
			else
			{
				if (p.Y >= center.Y)
				{
					p.Y = 9999999f;
				}
				else
				{
					p.Y = -9999999f;
				}
				p.X = center.X;
			}
			return p;
		}

		/// <summary>
		/// Determine the actual point at which
		/// a link connected to this port should terminate.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		/// <remarks>
		/// When the value of <see cref="P:Northwoods.Go.GoPort.ToSpot" /> is not <c>GoObject.NoSpot</c>,
		/// this just returns that spot's location.
		/// Otherwise this returns <see cref="M:Northwoods.Go.GoPort.GetLinkPointFromPoint(System.Drawing.PointF)" /> for
		/// a point near the end of the link.
		/// You may wish to override this method if you want to customize
		/// the link point connection dynamically.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		public virtual PointF GetToLinkPoint(IGoLink link)
		{
			GoSubGraph goSubGraph = FindVisibleCollapsedSubGraph();
			if (goSubGraph != null && goSubGraph.Port != null && goSubGraph.Port != this && !link.GoObject.IsChildOf(goSubGraph))
			{
				return goSubGraph.Port.GetToLinkPoint(link);
			}
			if (ToSpot != 0)
			{
				GoObject goObject = PortObject;
				if (goObject == null || (goObject.Layer == null && GoObject.FindCommonParent(this, goObject) == null))
				{
					goObject = this;
				}
				return goObject.GetSpotLocation(ToSpot);
			}
			if (link == null || link.FromPort == null || link.FromPort.GoObject == null)
			{
				return base.Center;
			}
			GoLink goLink = link as GoLink;
			if (goLink == null && link is GoLabeledLink)
			{
				goLink = (link as GoLabeledLink).RealLink;
			}
			checked
			{
				PointF point;
				if (goLink != null && goLink.PointsCount > (goLink.Orthogonal ? 6 : 2))
				{
					point = goLink.GetPoint(goLink.PointsCount - 2);
					if (goLink.Orthogonal)
					{
						point = OrthoPointToward(point);
						return GetLinkPointFromPoint(point);
					}
					PointF linkPointFromPoint = GetLinkPointFromPoint(point);
					if (linkPointFromPoint == base.Center)
					{
						linkPointFromPoint = GetLinkPointFromPoint(goLink.GetPoint(goLink.PointsCount - 3));
						if (linkPointFromPoint == base.Center && goLink.PointsCount > 3)
						{
							linkPointFromPoint = GetLinkPointFromPoint(goLink.GetPoint(0));
						}
					}
					return linkPointFromPoint;
				}
				point = link.FromPort.GoObject.Center;
				if (goLink != null && goLink.Orthogonal)
				{
					point = OrthoPointToward(point);
				}
				return GetLinkPointFromPoint(point);
			}
		}

		/// <summary>
		/// Determine the actual point at which
		/// a link connected to this port should terminate when the spot is
		/// <c>GoObject.NoSpot</c>.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// By default this is the same as <see cref="M:Northwoods.Go.GoPort.GetNearestIntersectionPoint(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF@)" />
		/// for the line from <paramref name="p" /> to the center of this port.
		/// But this will just return the <c>GoObject.Center</c> if the point is
		/// contained in this port (or <see cref="P:Northwoods.Go.GoPort.PortObject" />, if any).
		/// You may wish to override this method if you want to customize
		/// the link point connection dynamically.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />
		public virtual PointF GetLinkPointFromPoint(PointF p)
		{
			GoObject goObject = PortObject;
			GoSubGraph goSubGraph = FindVisibleCollapsedSubGraph();
			if (goSubGraph != null)
			{
				if (goSubGraph.CollapsedObject != null)
				{
					goObject = goSubGraph.CollapsedObject;
				}
				else if (goSubGraph != null && goSubGraph.Port != null && goSubGraph.Port != this)
				{
					return goSubGraph.Port.GetLinkPointFromPoint(p);
				}
			}
			if (goObject == null || (goObject.Layer == null && GoObject.FindCommonParent(this, goObject) == null))
			{
				goObject = this;
			}
			if (goObject.ContainsPoint(p))
			{
				return goObject.Center;
			}
			if (GetNearestIntersectionPoint(p, base.Center, out PointF result))
			{
				return result;
			}
			return goObject.Center;
		}

		/// <summary>
		/// Determine the direction that a link will go when coming out of this port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>
		/// A direction in degrees, where the positive X axis is zero, and where
		/// <c>90</c> is downward along the positive Y axis.
		/// </returns>
		/// <remarks>
		/// When the value of <see cref="P:Northwoods.Go.GoPort.FromSpot" /> is not
		/// <c>GoObject.NoSpot</c> or <c>GoObject.Middle</c>,
		/// this just returns the result of calling <see cref="M:Northwoods.Go.GoPort.GetLinkDir(System.Int32)" />.
		/// Otherwise this returns one of the horizontal or vertical directions,
		/// based on the relative positions of the <paramref name="link" />'s other port.
		/// You may wish to override this method if you want to customize dynamically
		/// the direction of the link at the link point.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkDir(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetLinkDir(System.Int32)" />
		public virtual float GetFromLinkDir(IGoLink link)
		{
			GoSubGraph goSubGraph = FindVisibleCollapsedSubGraph();
			if (goSubGraph != null && goSubGraph.Port != null && goSubGraph.Port != this && !link.GoObject.IsChildOf(goSubGraph))
			{
				return goSubGraph.Port.GetFromLinkDir(link);
			}
			int fromSpot = FromSpot;
			if (fromSpot != 0 && fromSpot != 1)
			{
				return GetLinkDir(fromSpot);
			}
			if (link == null || link.ToPort == null || link.ToPort.GoObject == null)
			{
				return 0f;
			}
			GoLink goLink = link as GoLink;
			if (goLink == null && link is GoLabeledLink)
			{
				goLink = (link as GoLabeledLink).RealLink;
			}
			PointF point;
			if (goLink != null && goLink.PointsCount > (goLink.Orthogonal ? 6 : 2))
			{
				point = goLink.GetPoint(1);
				if (goLink.Orthogonal)
				{
					point = OrthoPointToward(point);
				}
				else
				{
					point = GetLinkPointFromPoint(point);
					if (point == base.Center)
					{
						point = GetLinkPointFromPoint(goLink.GetPoint(2));
						if (point == base.Center && goLink.PointsCount > 3)
						{
							point = GetLinkPointFromPoint(goLink.GetPoint(checked(goLink.PointsCount - 1)));
						}
					}
				}
			}
			else
			{
				point = link.ToPort.GoObject.Center;
			}
			PointF center = base.Center;
			if (Math.Abs(point.X - center.X) > Math.Abs(point.Y - center.Y))
			{
				if (point.X >= center.X)
				{
					return 0f;
				}
				return 180f;
			}
			if (point.Y >= center.Y)
			{
				return 90f;
			}
			return 270f;
		}

		/// <summary>
		/// Determine the direction that a link will go when going into this port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>
		/// A direction in degrees, where the positive X axis is zero, and where
		/// <c>90</c> is downward along the positive Y axis.
		/// </returns>
		/// <remarks>
		/// When the value of <see cref="P:Northwoods.Go.GoPort.ToSpot" /> is not
		/// <c>GoObject.NoSpot</c> or <c>GoObject.Middle</c>,
		/// this just returns the result of calling <see cref="M:Northwoods.Go.GoPort.GetLinkDir(System.Int32)" />.
		/// Otherwise this returns one of the horizontal or vertical directions,
		/// based on the relative positions of the <paramref name="link" />'s other port.
		/// You may wish to override this method if you want to customize dynamically
		/// the direction of the link at the link point.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkDir(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetLinkDir(System.Int32)" />
		public virtual float GetToLinkDir(IGoLink link)
		{
			GoSubGraph goSubGraph = FindVisibleCollapsedSubGraph();
			if (goSubGraph != null && goSubGraph.Port != null && goSubGraph.Port != this && !link.GoObject.IsChildOf(goSubGraph))
			{
				return goSubGraph.Port.GetToLinkDir(link);
			}
			int toSpot = ToSpot;
			if (toSpot != 0 && toSpot != 1)
			{
				return GetLinkDir(toSpot);
			}
			if (link == null || link.FromPort == null || link.FromPort.GoObject == null)
			{
				return 0f;
			}
			GoLink goLink = link as GoLink;
			if (goLink == null && link is GoLabeledLink)
			{
				goLink = (link as GoLabeledLink).RealLink;
			}
			checked
			{
				PointF point;
				if (goLink != null && goLink.PointsCount > (goLink.Orthogonal ? 6 : 2))
				{
					point = goLink.GetPoint(goLink.PointsCount - 2);
					if (goLink.Orthogonal)
					{
						point = OrthoPointToward(point);
					}
					else
					{
						point = GetLinkPointFromPoint(point);
						if (point == base.Center)
						{
							point = GetLinkPointFromPoint(goLink.GetPoint(goLink.PointsCount - 3));
							if (point == base.Center && goLink.PointsCount > 3)
							{
								point = GetLinkPointFromPoint(goLink.GetPoint(0));
							}
						}
					}
				}
				else
				{
					point = link.FromPort.GoObject.Center;
				}
				PointF center = base.Center;
				if (Math.Abs(point.X - center.X) > Math.Abs(point.Y - center.Y))
				{
					if (point.X >= center.X)
					{
						return 0f;
					}
					return 180f;
				}
				if (point.Y >= center.Y)
				{
					return 90f;
				}
				return 270f;
			}
		}

		/// <summary>
		/// Determine the direction in which the link should go from the link point.
		/// </summary>
		/// <param name="spot"></param>
		/// <returns>
		/// A direction in degrees, where the positive X axis is zero, and where
		/// <c>90</c> is downward along the positive Y axis.
		/// </returns>
		/// <remarks>
		/// By default this handles the eight standard object spots around the edges.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoPort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.GetToLinkPoint(Northwoods.Go.IGoLink)" />
		public virtual float GetLinkDir(int spot)
		{
			switch (spot)
			{
			default:
				return 0f;
			case 2:
				return 225f;
			case 32:
				return 270f;
			case 4:
				return 315f;
			case 64:
				return 0f;
			case 8:
				return 45f;
			case 128:
				return 90f;
			case 16:
				return 135f;
			case 256:
				return 180f;
			}
		}

		private float GetEndSegmentLength(IGoLink link, bool atEnd)
		{
			float endSegmentLength = EndSegmentLength;
			if (link == null)
			{
				return endSegmentLength;
			}
			if (!Visible)
			{
				return endSegmentLength;
			}
			if (!GoLink.IsOrtho(link))
			{
				return endSegmentLength;
			}
			IGoPort otherPort = link.GetOtherPort(this);
			if (otherPort == null)
			{
				return endSegmentLength;
			}
			GoObject goObject = otherPort.GoObject;
			if (goObject == null)
			{
				return endSegmentLength;
			}
			PointF center = goObject.Center;
			GoObject parentNode = base.ParentNode;
			GoGeneralNode goGeneralNode = parentNode as GoGeneralNode;
			GoMultiTextNode goMultiTextNode = parentNode as GoMultiTextNode;
			if (goGeneralNode == null && goMultiTextNode == null)
			{
				return endSegmentLength;
			}
			float num = atEnd ? 4 : 8;
			if (goGeneralNode != null)
			{
				num = (atEnd ? goGeneralNode.ToEndSegmentLengthStep : goGeneralNode.FromEndSegmentLengthStep);
			}
			else if (goMultiTextNode != null)
			{
				num = (atEnd ? goMultiTextNode.ToEndSegmentLengthStep : goMultiTextNode.FromEndSegmentLengthStep);
			}
			if (num == 0f)
			{
				return endSegmentLength;
			}
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			checked
			{
				if (goGeneralNode != null)
				{
					flag = ((goGeneralNode.Orientation == Orientation.Horizontal && base.Center.X < goGeneralNode.Center.X) || (goGeneralNode.Orientation == Orientation.Vertical && base.Center.Y < goGeneralNode.Center.Y));
					if (!flag)
					{
						GoGeneralNodePort goGeneralNodePort = this as GoGeneralNodePort;
						if (goGeneralNodePort != null)
						{
							flag = goGeneralNodePort.LeftSide;
						}
					}
					if (flag)
					{
						int leftPortsCount = goGeneralNode.LeftPortsCount;
						for (int i = 0; i < leftPortsCount; i++)
						{
							GoGeneralNodePort leftPort = goGeneralNode.GetLeftPort(i);
							if (leftPort != null)
							{
								if (leftPort == this)
								{
									num2 = num3;
								}
								if (leftPort.LinksCount > 0)
								{
									num3++;
								}
							}
						}
					}
					else
					{
						int rightPortsCount = goGeneralNode.RightPortsCount;
						for (int j = 0; j < rightPortsCount; j++)
						{
							GoGeneralNodePort rightPort = goGeneralNode.GetRightPort(j);
							if (rightPort != null)
							{
								if (rightPort == this)
								{
									num2 = num3;
								}
								if (rightPort.LinksCount > 0)
								{
									num3++;
								}
							}
						}
					}
				}
				else if (goMultiTextNode != null)
				{
					flag = (base.Center.X < goMultiTextNode.Center.X);
					int num4 = goMultiTextNode.ListGroup.TopIndex;
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (flag)
					{
						int itemCount = goMultiTextNode.ItemCount;
						for (int k = num4; k < itemCount; k++)
						{
							GoObject leftPort2 = goMultiTextNode.GetLeftPort(k);
							if (leftPort2 == null)
							{
								continue;
							}
							if (leftPort2 == this)
							{
								num2 = num3;
							}
							if (leftPort2.Visible)
							{
								GoPort goPort = leftPort2 as GoPort;
								if (goPort != null && goPort.LinksCount > 0)
								{
									num3++;
								}
							}
						}
					}
					else
					{
						int itemCount2 = goMultiTextNode.ItemCount;
						for (int l = num4; l < itemCount2; l++)
						{
							GoObject rightPort2 = goMultiTextNode.GetRightPort(l);
							if (rightPort2 == null)
							{
								continue;
							}
							if (rightPort2 == this)
							{
								num2 = num3;
							}
							if (rightPort2.Visible)
							{
								GoPort goPort2 = rightPort2 as GoPort;
								if (goPort2 != null && goPort2.LinksCount > 0)
								{
									num3++;
								}
							}
						}
					}
				}
				if (num3 <= 1)
				{
					return endSegmentLength;
				}
				RectangleF a = goObject.ParentNode.Bounds;
				GoObject.InflateRect(ref a, endSegmentLength, endSegmentLength);
				PointF center2 = base.Center;
				bool flag2 = true;
				if (goGeneralNode != null)
				{
					flag2 = (goGeneralNode.Orientation == Orientation.Horizontal);
				}
				float num5 = endSegmentLength;
				float num6 = flag2 ? base.Height : base.Width;
				if (flag2 ? (center.Y < center2.Y - num6) : (center.X < center2.X - num6))
				{
					num5 = endSegmentLength + (float)num2 * num + num / 2f;
					if (flag2 && GoObject.IntersectsLineSegment(a, center2, new PointF(center2.X + (flag ? (0f - num5) : num5), center2.Y)))
					{
						return endSegmentLength;
					}
					if (!flag2 && GoObject.IntersectsLineSegment(a, center2, new PointF(center2.X, center2.Y + (flag ? (0f - num5) : num5))))
					{
						return endSegmentLength;
					}
				}
				else if (flag2 ? (center.Y > center2.Y + num6) : (center.X > center2.X + num6))
				{
					num5 = endSegmentLength + (float)(num3 - 1 - num2) * num;
					if (flag2 && GoObject.IntersectsLineSegment(a, center2, new PointF(center2.X + (flag ? (0f - num5) : num5), center2.Y)))
					{
						return endSegmentLength;
					}
					if (!flag2 && GoObject.IntersectsLineSegment(a, center2, new PointF(center2.X, center2.Y + (flag ? (0f - num5) : num5))))
					{
						return endSegmentLength;
					}
				}
				return num5;
			}
		}

		/// <summary>
		/// Return the length of the link's first segment, at its <see cref="P:Northwoods.Go.IGoLink.FromPort" />.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>By default this just returns the value of <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />.</returns>
		public virtual float GetFromEndSegmentLength(IGoLink link)
		{
			return GetEndSegmentLength(link, atEnd: false);
		}

		/// <summary>
		/// Return the length of the link's last segment, at its <see cref="P:Northwoods.Go.IGoLink.ToPort" />.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>By default this just returns the value of <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />.</returns>
		public virtual float GetToEndSegmentLength(IGoLink link)
		{
			return GetEndSegmentLength(link, atEnd: true);
		}

		/// <summary>
		/// Tell any connected links about any change to this port
		/// </summary>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// All the parameters just get passed on through calls to <see cref="M:Northwoods.Go.IGoLink.OnPortChanged(Northwoods.Go.IGoPort,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </remarks>
		public virtual void LinksOnPortChanged(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			foreach (IGoLink link in Links)
			{
				link?.OnPortChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				if (subhint == 1001)
				{
					IGoPort otherPort = link.GetOtherPort(this);
					if (otherPort != null)
					{
						(otherPort as GoBoxPort)?.UpdateAllLinkPoints();
					}
				}
			}
		}

		/// <summary>
		/// When a link connected to this port is changed, this method is called.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// This lets port subclasses handle changes to any links on this port.
		/// <paramref name="subhint" /> may include <see cref="F:Northwoods.Go.GoPort.ChangedAddedLink" />
		/// and <see cref="F:Northwoods.Go.GoPort.ChangedRemovedLink" />,
		/// as well as possibly some subhints such as <see cref="F:Northwoods.Go.GoLink.ChangedFromPort" />
		/// and <see cref="F:Northwoods.Go.GoLink.ChangedToPort" />.
		/// The default implementation of this method does nothing.
		/// If you override this, you will probably want to do nothing if the link parameter,
		/// <paramref name="l" />, is null.
		/// </remarks>
		public virtual void OnLinkChanged(IGoLink l, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
		}

		/// <summary>
		/// When a port is moved, we update any links connected to this port
		/// by calling <see cref="M:Northwoods.Go.GoPort.LinksOnPortChanged(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		/// <param name="old"></param>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			LinksOnPortChanged(1001, 0, null, old, 0, null, Bounds);
		}

		/// <summary>
		/// When a port is removed from a layer, we remove all of the links connected
		/// to the port.
		/// </summary>
		/// <param name="oldlayer"></param>
		/// <param name="newlayer"></param>
		/// <param name="mainObj"></param>
		/// <remarks>
		/// This does not remove any links that are part of the <paramref name="mainObj" />.
		/// Nor does it remove any links that belong to a view, if this port is part of a document.
		/// And conversely, it does not remove any links that belong to a document if this port
		/// is part of a view.
		/// Finally, this calls <see cref="M:Northwoods.Go.GoPort.ClearLinks" /> only when <see cref="P:Northwoods.Go.GoPort.ClearsLinksWhenRemoved" />
		/// is true.
		/// </remarks>
		protected override void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
			base.OnLayerChanged(oldlayer, newlayer, mainObj);
			if (newlayer == null && ClearsLinksWhenRemoved && !NoClearLinks)
			{
				ClearLinks(mainObj);
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1700:
				UserFlags = e.GetInt(undo);
				break;
			case 1701:
				UserObject = e.GetValue(undo);
				break;
			case 1702:
				Style = (GoPortStyle)e.GetInt(undo);
				break;
			case 1703:
				PortObject = (GoObject)e.GetValue(undo);
				break;
			case 1704:
				IsValidFrom = (bool)e.GetValue(undo);
				break;
			case 1705:
				IsValidTo = (bool)e.GetValue(undo);
				break;
			case 1706:
				IsValidSelfNode = (bool)e.GetValue(undo);
				break;
			case 1711:
				IsValidDuplicateLinks = (bool)e.GetValue(undo);
				break;
			case 1707:
				FromSpot = e.GetInt(undo);
				break;
			case 1708:
				ToSpot = e.GetInt(undo);
				break;
			case 1709:
			{
				IGoLink link2 = (IGoLink)e.OldValue;
				if (undo)
				{
					InternalRemoveLink(link2, undoing: true);
				}
				else
				{
					InternalAddLink(link2, undoing: true);
				}
				break;
			}
			case 1710:
			{
				IGoLink link = (IGoLink)e.OldValue;
				if (undo)
				{
					InternalAddLink(link, undoing: true);
				}
				else
				{
					InternalRemoveLink(link, undoing: true);
				}
				break;
			}
			case 1712:
				EndSegmentLength = e.GetFloat(undo);
				break;
			case 1714:
				ClearsLinksWhenRemoved = (bool)e.GetValue(undo);
				break;
			case 1715:
				IsValidSingleLink = (bool)e.GetValue(undo);
				break;
			case 1713:
				PartID = e.GetInt(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
