using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This class provides the standard implementation of <see cref="T:Northwoods.Go.IGoNode" />
	/// as a <see cref="P:Northwoods.Go.GoNode.GoObject" />.
	/// </summary>
	[Serializable]
	public class GoNode : GoGroup, IGoNode, IGoGraphPart, IGoLabeledNode, IGoLabeledPart, IGoIdentifiablePart
	{
		[Flags]
		internal enum Search
		{
			Ports = 0x1,
			LinksIn = 0x2,
			LinksOut = 0x4,
			NodesIn = 0x8,
			NodesOut = 0x10,
			NotSelf = 0x20
		}

		private const int flagPropsOnSelObj = 4194304;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedNodeUserFlags = 2000;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedNodeUserObject = 2001;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedToolTipText = 2002;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		internal const int ChangedPropertiesDelegatedToSelectionObject = 2003;

		/// <summary>
		/// This is a <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedPartID = 2004;

		private string myToolTipText;

		private int myUserFlags;

		private object myUserObject;

		private int myPartID = -1;

		/// <summary>
		/// Returns itself as a <see cref="P:Northwoods.Go.GoNode.GoObject" />.
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
		/// Gets or sets an integer value associated with this node.
		/// </summary>
		/// <value>
		/// The initial value is zero.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />
		[DefaultValue(0)]
		[Description("An integer value associated with this node.")]
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
					Changed(2000, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets an object associated with this node.
		/// </summary>
		/// <value>
		/// The initial value is null.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserObject" />
		[DefaultValue(null)]
		[Description("An object associated with this node.")]
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
					Changed(2001, 0, obj, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		IEnumerable<IGoNode> IGoNode.Sources => GetNodeEnumerator(Search.NodesIn);

		/// <summary>
		/// Gets an enumerator over all of the nodes that have links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// destination links coming into any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Sources" />
		[Description("Gets an enumerator over all of the nodes that have links coming into this node.")]
		public virtual GoNodeNodeEnumerator Sources => GetNodeEnumerator(Search.NodesIn);

		IEnumerable<IGoNode> IGoNode.Destinations => GetNodeEnumerator(Search.NodesOut);

		/// <summary>
		/// Gets an enumerator over all of the nodes that have links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// source links going out of any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Destinations" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.Nodes" />
		[Description("Gets an enumerator over all of the nodes that have links going out of this node.")]
		public virtual GoNodeNodeEnumerator Destinations => GetNodeEnumerator(Search.NodesOut);

		IEnumerable<IGoNode> IGoNode.Nodes => GetNodeEnumerator(Search.NodesIn | Search.NodesOut);

		/// <summary>
		/// Gets an enumerator over all of the nodes that are connected to this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that are
		/// connected in either direction to any port of this node.
		/// Note that this does NOT enumerate over all of the child objects that are
		/// <see cref="T:Northwoods.Go.IGoNode" />s.  It enumerates all connected <see cref="T:Northwoods.Go.IGoNode" />s,
		/// which will include any child nodes that are connected to any child ports.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Sources" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.Destinations" />
		[Description("Gets an enumerator over all of the nodes that are connected to this node.")]
		public virtual GoNodeNodeEnumerator Nodes => GetNodeEnumerator(Search.NodesIn | Search.NodesOut);

		IEnumerable<IGoLink> IGoNode.SourceLinks => GetLinkEnumerator(Search.LinksIn);

		/// <summary>
		/// Gets an enumerator over all of the links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that come into
		/// any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.SourceLinks" />
		[Description("Gets an enumerator over all of the links coming into this node.")]
		public virtual GoNodeLinkEnumerator SourceLinks => GetLinkEnumerator(Search.LinksIn);

		IEnumerable<IGoLink> IGoNode.DestinationLinks => GetLinkEnumerator(Search.LinksOut);

		/// <summary>
		/// Gets an enumerator over all of the links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that go out of
		/// any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.DestinationLinks" />
		[Description("Gets an enumerator over all of the links going out of this node.")]
		public virtual GoNodeLinkEnumerator DestinationLinks => GetLinkEnumerator(Search.LinksOut);

		IEnumerable<IGoLink> IGoNode.Links => GetLinkEnumerator(Search.LinksIn | Search.LinksOut);

		/// <summary>
		/// Gets an enumerator over all of the links connected to this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that come into
		/// or go out of any port of this node.
		/// Note that this does NOT enumerate over all of the child objects that are
		/// <see cref="T:Northwoods.Go.IGoLink" />s.  It enumerates all connected <see cref="T:Northwoods.Go.IGoLink" />s,
		/// which will include any child links that are connected to any child ports.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.SourceLinks" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.DestinationLinks" />
		[Description("Gets an enumerator over all of the links connected to this node.")]
		public virtual GoNodeLinkEnumerator Links => GetLinkEnumerator(Search.LinksIn | Search.LinksOut);

		IEnumerable<IGoPort> IGoNode.Ports => GetPortEnumerator();

		/// <summary>
		/// Gets an enumerator over all of the ports that are part of this node.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Ports" />
		[Description("Gets an enumerator over all of the ports that are part of this node.")]
		public virtual GoNodePortEnumerator Ports => GetPortEnumerator();

		/// <summary>
		/// Get the principal text string associated with a node.
		/// </summary>
		/// <value>
		/// By default this will just get and set the <see cref="P:Northwoods.Go.GoNode.Label" />'s
		/// <see cref="P:Northwoods.Go.GoText.Text" /> property.
		/// If no such label exists, get returns an empty string and set
		/// does nothing.
		/// </value>
		/// <remarks>
		/// This property can be used by code that needs to get some string
		/// that may identify this node, albeit not necessarily uniquely and
		/// not necessarily visibly.
		/// </remarks>
		public virtual string Text
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
				GoText label = Label;
				if (label != null)
				{
					label.Text = value;
				}
			}
		}

		/// <summary>
		/// Get the main GoText object associated with a node.
		/// </summary>
		/// <value>
		/// By default this searches for and returns the first child that is a <see cref="T:Northwoods.Go.GoText" />.
		/// </value>
		/// <remarks>
		/// By default setting this property does nothing.
		/// However, some derived classes may implement setting this property.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoText Label
		{
			get
			{
				return FindLabel(this);
			}
			set
			{
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
		/// override <see cref="M:Northwoods.Go.GoNode.GetToolTip(Northwoods.Go.GoView)" />.
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
					Changed(2002, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Assume the natural location of a node is the center of the <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>SelectionObject</c>,
		/// if it is different from the whole group itself.
		/// </summary>
		/// <remarks>
		/// If there is no separate <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>SelectionObject</c>, then
		/// the <c>Location</c> is the same as the <c>Position</c>.
		/// Setting the location would naturally be implemented as
		/// <code>this.SelectionObject.Center = value</code>
		/// but for efficiency, it instead is implemented by setting this node's
		/// <c>Position</c> so that the <see cref="P:Northwoods.Go.GoNode.GoObject" />.<c>SelectionObject</c>'s
		/// <c>Center</c> is at the new location.
		/// </remarks>
		public override PointF Location
		{
			get
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					return SelectionObject.Center;
				}
				return base.Position;
			}
			set
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					SizeF sizeF = GoTool.SubtractPoints(SelectionObject.Center, base.Position);
					base.Position = new PointF(value.X - sizeF.Width, value.Y - sizeF.Height);
				}
				else
				{
					base.Position = value;
				}
			}
		}

		/// <summary>
		/// Giving this node a shadow really means giving the <c>SelectionObject</c> a shadow.
		/// </summary>
		public override bool Shadowed
		{
			get
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					return SelectionObject.Shadowed;
				}
				return base.Shadowed;
			}
			set
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					SelectionObject.Shadowed = value;
				}
				else
				{
					base.Shadowed = value;
				}
			}
		}

		/// <summary>
		/// Whether this node is resizable is really determined by whether the <c>SelectionObject</c> is resizable.
		/// </summary>
		public override bool Resizable
		{
			get
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					return SelectionObject.Resizable;
				}
				return base.Resizable;
			}
			set
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					SelectionObject.Resizable = value;
				}
				else
				{
					base.Resizable = value;
				}
			}
		}

		/// <summary>
		/// Whether this node is reshapable is really determined by whether the <c>SelectionObject</c> is reshapable.
		/// </summary>
		public override bool Reshapable
		{
			get
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					return SelectionObject.Reshapable;
				}
				return base.Reshapable;
			}
			set
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					SelectionObject.Reshapable = value;
				}
				else
				{
					base.Reshapable = value;
				}
			}
		}

		/// <summary>
		/// Whether the user can resize this node continuously is really determined by the <c>SelectionObject</c>.
		/// </summary>
		public override bool ResizesRealtime
		{
			get
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					return SelectionObject.ResizesRealtime;
				}
				return base.ResizesRealtime;
			}
			set
			{
				if (PropertiesDelegatedToSelectionObject && SelectionObject != this)
				{
					SelectionObject.ResizesRealtime = value;
				}
				else
				{
					base.ResizesRealtime = value;
				}
			}
		}

		internal bool PropertiesDelegatedToSelectionObject
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
					Changed(2003, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
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
					Changed(2004, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Make sure cached information is not copied.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoNode goNode = (GoNode)base.CopyObject(env);
			if (goNode != null)
			{
				goNode.myPartID = -1;
			}
			return goNode;
		}

		private GoNodeNodeEnumerator GetNodeEnumerator(Search s)
		{
			return new GoNodeNodeEnumerator(this, s);
		}

		private GoNodeLinkEnumerator GetLinkEnumerator(Search s)
		{
			return new GoNodeLinkEnumerator(this, s);
		}

		private GoNodePortEnumerator GetPortEnumerator()
		{
			return new GoNodePortEnumerator(this, Search.Ports);
		}

		internal List<T> findAll<T>(Search s)
		{
			List<T> list = new List<T>();
			findAllAux(this, s, list);
			return list;
		}

		private void findAllAux<T>(GoObject obj, Search s, List<T> items)
		{
			IGoPort goPort = obj as IGoPort;
			if (goPort != null)
			{
				if ((s & Search.Ports) != 0)
				{
					addItem(items, (T)goPort);
				}
				GoPort goPort2 = goPort as GoPort;
				if (goPort2 != null)
				{
					foreach (IGoLink link in goPort2.Links)
					{
						considerLink(link, goPort, s, items);
					}
				}
				else
				{
					foreach (IGoLink link2 in goPort.Links)
					{
						considerLink(link2, goPort, s, items);
					}
				}
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					findAllAux(item, s, items);
				}
			}
		}

		private void considerLink<T>(IGoLink l, IGoPort p, Search s, List<T> items)
		{
			bool flag = (s & Search.NotSelf) == 0;
			if (l.FromPort == p && (flag || l.ToPort.GoObject == null || !l.ToPort.GoObject.IsChildOf(this)))
			{
				if ((s & Search.LinksOut) != 0)
				{
					addItem(items, (T)l);
				}
				if ((s & Search.NodesOut) != 0)
				{
					addItem(items, (T)l.ToNode);
				}
			}
			if (l.ToPort == p && (flag || l.FromPort.GoObject == null || !l.FromPort.GoObject.IsChildOf(this)))
			{
				if ((s & Search.LinksIn) != 0)
				{
					addItem(items, (T)l);
				}
				if ((s & Search.NodesIn) != 0)
				{
					addItem(items, (T)l.FromNode);
				}
			}
		}

		private void addItem<T>(List<T> items, T obj)
		{
			if (obj != null && !items.Contains(obj))
			{
				items.Add(obj);
			}
		}

		/// <summary>
		/// This just moves all the children from the group's former location.
		/// </summary>
		/// <param name="prevRect">
		/// The original bounds, in document coordinates.
		/// </param>
		/// <remarks>
		/// This first moves all <see cref="T:Northwoods.Go.IGoLink" />s, and then all the other child objects.
		/// </remarks>
		protected override void MoveChildren(RectangleF prevRect)
		{
			float num = base.Left - prevRect.X;
			float num2 = base.Top - prevRect.Y;
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current = goGroupEnumerator.Current;
					if (current is IGoLink)
					{
						RectangleF bounds = current.Bounds;
						current.Bounds = new RectangleF(bounds.X + num, bounds.Y + num2, bounds.Width, bounds.Height);
					}
				}
			}
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current2 = goGroupEnumerator.Current;
					if (!(current2 is IGoLink))
					{
						RectangleF bounds2 = current2.Bounds;
						current2.Bounds = new RectangleF(bounds2.X + num, bounds2.Y + num2, bounds2.Width, bounds2.Height);
					}
				}
			}
		}

		/// <summary>
		/// This handles the general case of a resize by scaling and repositioning all the children.
		/// </summary>
		/// <param name="prevRect">
		/// The original bounds, in document coordinates.
		/// </param>
		/// <remarks>
		/// Any children whose <c>AutoRescales</c> property is false is not
		/// resized and repositioned.
		/// This first rescales all <see cref="T:Northwoods.Go.IGoLink" />s, and then all the other child objects.
		/// </remarks>
		protected override void RescaleChildren(RectangleF prevRect)
		{
			if (!(prevRect.Width <= 0f) && !(prevRect.Height <= 0f))
			{
				RectangleF bounds = Bounds;
				float num = bounds.Width / prevRect.Width;
				float num2 = bounds.Height / prevRect.Height;
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current = goGroupEnumerator.Current;
						if (current is IGoLink && current.AutoRescales)
						{
							RectangleF bounds2 = current.Bounds;
							float x = bounds.X + (bounds2.X - prevRect.X) * num;
							float y = bounds.Y + (bounds2.Y - prevRect.Y) * num2;
							float width = bounds2.Width * num;
							float height = bounds2.Height * num2;
							current.Bounds = new RectangleF(x, y, width, height);
						}
					}
				}
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current2 = goGroupEnumerator.Current;
						if (!(current2 is IGoLink) && current2.AutoRescales)
						{
							RectangleF bounds3 = current2.Bounds;
							float x2 = bounds.X + (bounds3.X - prevRect.X) * num;
							float y2 = bounds.Y + (bounds3.Y - prevRect.Y) * num2;
							float width2 = bounds3.Width * num;
							float height2 = bounds3.Height * num2;
							current2.Bounds = new RectangleF(x2, y2, width2, height2);
						}
					}
				}
			}
		}

		/// <summary>
		/// Start editing the label.
		/// </summary>
		/// <param name="view"></param>
		public override void DoBeginEdit(GoView view)
		{
			if (Label != null)
			{
				Label.DoBeginEdit(view);
			}
		}

		internal static GoText FindLabel(GoObject obj)
		{
			GoText goText = obj as GoText;
			if (goText != null)
			{
				return goText;
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					GoText goText2 = FindLabel(item);
					if (goText2 != null)
					{
						return goText2;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Return a string to be displayed in a tooltip, or null for none.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// By default this returns this node's <see cref="P:Northwoods.Go.GoNode.ToolTipText" />.
		/// Override this method if you want dynamically computed tooltips.
		/// </returns>
		public override string GetToolTip(GoView view)
		{
			return ToolTipText;
		}

		internal void CopyPropertiesFromSelectionObject(GoObject oldobj, GoObject newobj)
		{
			if (oldobj != null && newobj != null && oldobj == SelectionObject)
			{
				newobj.Center = oldobj.Center;
				newobj.Selectable = oldobj.Selectable;
				newobj.Resizable = oldobj.Resizable;
				newobj.Reshapable = oldobj.Reshapable;
				newobj.ResizesRealtime = oldobj.ResizesRealtime;
				newobj.Shadowed = oldobj.Shadowed;
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2000:
				UserFlags = e.GetInt(undo);
				break;
			case 2001:
				UserObject = e.GetValue(undo);
				break;
			case 2002:
				ToolTipText = (string)e.GetValue(undo);
				break;
			case 2003:
				PropertiesDelegatedToSelectionObject = (bool)e.GetValue(undo);
				break;
			case 2004:
				PartID = e.GetInt(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
