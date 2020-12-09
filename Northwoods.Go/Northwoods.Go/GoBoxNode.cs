using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A GoBoxNode has a single rectangular port that surrounds another object, named the <see cref="P:Northwoods.Go.GoBoxNode.Body" />.
	/// This is most useful when that body object is basically rectangular, and when links
	/// can connect at any side, determined dynamically.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class uses a <see cref="T:Northwoods.Go.GoBoxPort" /> to allow links to come in orthogonally
	/// and (optionally) be positioned intelligently apart from each other.
	/// The <see cref="P:Northwoods.Go.GoBoxNode.Port" /> is slightly larger than the <see cref="P:Northwoods.Go.GoBoxNode.Body" />, so that it appears as if
	/// it were a border for the body object.  Thus users can start drawing links
	/// by mouse-down and dragging along the edge of the GoBoxNode.
	/// You can customize the appearance of that "border" by setting the <see cref="P:Northwoods.Go.GoBoxNode.Port" />'s
	/// properties.  For example:
	/// <pre><code>
	/// GoBoxNode node1 = new GoBoxNode();
	/// node1.Text = "box node\n1";
	/// node1.Label.Alignment = GoObject.Middle;
	/// node1.Port.PenColor = Color.Black;
	/// node1.Port.BrushColor = Color.Pink;
	/// node1.PortBorderMargin = new SizeF(6, 6);
	/// node1.LinksPointsSpread = true;
	/// </code></pre>
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoBoxNode.Body" /> is normally a <see cref="T:Northwoods.Go.GoText" />, but could
	/// be any kind of <see cref="T:Northwoods.Go.GoObject" />, including complex <see cref="T:Northwoods.Go.GoGroup" />s.
	/// In this case you may find it advantageous to override the <see cref="P:Northwoods.Go.GoNode.Label" />
	/// property to return (and possibly to set) the appropriate <see cref="T:Northwoods.Go.GoText" />
	/// object inside your group.
	/// </para>
	/// <para>
	/// As you connect links to this node's <see cref="P:Northwoods.Go.GoBoxNode.Port" />,
	/// when <see cref="P:Northwoods.Go.GoBoxNode.LinkPointsSpread" /> is true, other links on the same side
	/// are not actually spread out until you either move the node or call
	/// <see cref="M:Northwoods.Go.GoBoxNode.UpdateAllLinkPoints" />.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoBoxNode : GoNode
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxNode.Body" /> property.
		/// </summary>
		public const int ChangedBody = 2201;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxNode.PortBorderMargin" /> property.
		/// </summary>
		public const int ChangedPortBorderMargin = 2202;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxNode.Port" /> property.
		/// </summary>
		public const int ChangedPort = 2203;

		private GoObject myBody;

		private SizeF myPortBorderMargin = new SizeF(4f, 4f);

		private GoPort myPort;

		/// <summary>
		/// Gets or sets the main object that this node is displaying.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject Body
		{
			get
			{
				return myBody;
			}
			set
			{
				GoObject goObject = myBody;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					Remove(goObject);
				}
				myBody = value;
				if (value != null)
				{
					if (goObject != null)
					{
						value.Center = goObject.Center;
					}
					Add(value);
				}
				Changed(2201, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets or sets the margin that is always visible for the port on each side of the body.
		/// </summary>
		/// <value>
		/// This specifies the width of each side margin and the height of top margin and the
		/// height of the bottom margin, in document coordinates.
		/// By default the width and height are each <c>4</c>, so that the port is <c>8</c>
		/// units wider and taller than the <see cref="P:Northwoods.Go.GoBoxNode.Body" />.
		/// The width and height must be non-negative.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin that is always visible for the port on each side of the body")]
		public virtual SizeF PortBorderMargin
		{
			get
			{
				return myPortBorderMargin;
			}
			set
			{
				SizeF sizeF = myPortBorderMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myPortBorderMargin = value;
					Changed(2202, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the port for this node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the port.
		/// If non-null, the port should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the port after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoBoxNode.CreatePort" /> method.
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
						InsertBefore(null, value);
					}
					Changed(2203, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the link points of the links connected to this node's <see cref="T:Northwoods.Go.GoBoxPort" />
		/// are spread evenly along the side of the port, or if they are all at the
		/// mid-point of the side.
		/// </summary>
		/// <value>
		/// The value actually comes from the <see cref="P:Northwoods.Go.GoBoxNode.Port" />, which is normally a <see cref="T:Northwoods.Go.GoBoxPort" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the link points are distributed evenly along each side")]
		public virtual bool LinkPointsSpread
		{
			get
			{
				return (Port as GoBoxPort)?.LinkPointsSpread ?? false;
			}
			set
			{
				GoBoxPort goBoxPort = Port as GoBoxPort;
				if (goBoxPort != null)
				{
					goBoxPort.LinkPointsSpread = value;
				}
			}
		}

		/// <summary>
		/// Construct a GoBoxNode that by default has a body that is a multiline
		/// <see cref="T:Northwoods.Go.GoText" /> object with a white background.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoBoxNode.CreatePort" /> and <see cref="M:Northwoods.Go.GoBoxNode.CreateBody" />
		/// to get initial values for <see cref="P:Northwoods.Go.GoBoxNode.Port" /> and <see cref="P:Northwoods.Go.GoBoxNode.Body" />.
		/// </remarks>
		public GoBoxNode()
		{
			base.InternalFlags |= 131072;
			base.InternalFlags &= -17;
			myPort = CreatePort();
			Add(myPort);
			myBody = CreateBody();
			Add(myBody);
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// This method is called by the <see cref="T:Northwoods.Go.GoBoxNode" /> constructor to
		/// provide the object in the middle of the rectangular port.
		/// </summary>
		/// <returns>
		/// By default this returns a multiline <see cref="T:Northwoods.Go.GoText" /> with a white background.
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateBody() {
		///    GoText t = new GoText();
		///    t.TransparentBackground = false;
		///    t.BackgroundColor = Color.White;
		///    t.Multiline = true;
		///    t.Selectable = false;
		///    return t;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateBody()
		{
			return new GoText
			{
				TransparentBackground = false,
				BackgroundColor = Color.White,
				Multiline = true,
				Selectable = false
			};
		}

		/// <summary>
		/// This method is called by the <see cref="T:Northwoods.Go.GoBoxNode" /> constructor to
		/// provide the node's single port.
		/// </summary>
		/// <returns>
		/// By default this just returns a <see cref="T:Northwoods.Go.GoBoxPort" />.
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoPort CreatePort() {
		///    GoBoxPort p = new GoBoxPort();
		///    return p;
		///  }
		/// </code>
		/// </example>
		protected virtual GoPort CreatePort()
		{
			return new GoBoxPort();
		}

		/// <summary>
		/// Copy the <see cref="P:Northwoods.Go.GoBoxNode.Port" /> and <see cref="P:Northwoods.Go.GoBoxNode.Body" />.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoBoxNode obj = (GoBoxNode)newgroup;
			obj.myPort = (GoPort)env[myPort];
			obj.myBody = (GoObject)env[myBody];
		}

		/// <summary>
		/// Position the <see cref="P:Northwoods.Go.GoBoxNode.Port" /> to be centered on the <see cref="P:Northwoods.Go.GoBoxNode.Body" />,
		/// but sized larger by <see cref="P:Northwoods.Go.GoBoxNode.PortBorderMargin" /> width and height on
		/// each side.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// This method does nothing if there is no <see cref="P:Northwoods.Go.GoBoxNode.Body" />.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoObject body = Body;
			if (body != null)
			{
				GoObject port = Port;
				if (port != null)
				{
					RectangleF a = body.Bounds;
					SizeF portBorderMargin = PortBorderMargin;
					GoObject.InflateRect(ref a, portBorderMargin.Width, portBorderMargin.Height);
					port.Bounds = a;
				}
			}
		}

		/// <summary>
		/// If any part is removed from this group,
		/// be sure to remove any references in local fields.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myBody)
			{
				myBody = null;
				return result;
			}
			if (obj == myPort)
			{
				myPort = null;
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
			if (child == Body)
			{
				return "Body";
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
			if (name == "Body")
			{
				return Body;
			}
			if (name == "Port")
			{
				return Port;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// Reroute all of the links connected at this port,
		/// if <see cref="P:Northwoods.Go.GoBoxNode.LinkPointsSpread" /> is true, so that
		/// all of the link points are up-to-date.
		/// </summary>
		public virtual void UpdateAllLinkPoints()
		{
			(Port as GoBoxPort)?.UpdateAllLinkPoints();
		}

		/// <summary>
		/// Perform changes to the body or port margin for undo.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2201:
				Body = (GoObject)e.GetValue(undo);
				break;
			case 2202:
				PortBorderMargin = e.GetSize(undo);
				break;
			case 2203:
				Port = (GoPort)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
