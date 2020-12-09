using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Northwoods.Go
{
	/// <summary>
	/// The GoMultiTextNode class displays a number of text objects vertically
	/// using a <see cref="T:Northwoods.Go.GoListGroup" />, and associates ports on each side of each text object,
	/// plus a port at the top and one at the bottom of the node.
	/// </summary>
	/// <remarks>
	/// Although this group typically displays strings by holding <see cref="T:Northwoods.Go.GoText" />
	/// objects, you can actually insert any <see cref="T:Northwoods.Go.GoObject" />.
	/// This object is not meant to be resizable, so <see cref="P:Northwoods.Go.GoObject.Resizable" />
	/// is false.
	/// If you want to change the width of all of the items in the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />,
	/// you can set the <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> property.
	/// The initial value of <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" />, -1, does not cause any of the items'
	/// <see cref="P:Northwoods.Go.GoObject.Width" /> to be set -- they will keep their natural width.
	/// If you really want the user to be able to change the width of this node
	/// interactively, one way to do so is to override <see cref="M:Northwoods.Go.GoObject.ComputeResize(System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,System.Drawing.SizeF,System.Drawing.SizeF,System.Boolean)" />
	/// to reset the <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> to an appropriate value, accounting for
	/// any <see cref="P:Northwoods.Go.GoMultiTextNode.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoMultiTextNode.BottomRightMargin" /> and any
	/// ports.
	/// </remarks>
	[Serializable]
	public class GoMultiTextNode : GoNode
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int InsertedLeftPort = 3001;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int InsertedRightPort = 3002;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedLeftPort = 3003;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedRightPort = 3004;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ReplacedPort = 3005;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoMultiTextNode.TopPort" /> property.
		/// </summary>
		public const int ChangedTopPort = 3006;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoMultiTextNode.BottomPort" /> property.
		/// </summary>
		public const int ChangedBottomPort = 3007;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> property.
		/// </summary>
		public const int ChangedItemWidth = 3008;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoMultiTextNode.FromEndSegmentLengthStep" /> property.
		/// </summary>
		public const int ChangedFromEndSegmentLengthStep = 3009;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoMultiTextNode.ToEndSegmentLengthStep" /> property.
		/// </summary>
		public const int ChangedToEndSegmentLengthStep = 3010;

		private GoListGroup myListGroup;

		private GoObject myTopPort;

		private GoObject myBottomPort;

		private List<GoObject> myLeftPorts = new List<GoObject>();

		private List<GoObject> myRightPorts = new List<GoObject>();

		private float myItemWidth = -1f;

		private float myFromEndSegmentLengthStep = 8f;

		private float myToEndSegmentLengthStep = 4f;

		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
		/// <value>
		/// This is the same as <c>ListGroup.Count</c>.
		/// </value>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoGroup.Count" /> property refers to the number of immediate
		/// child objects in this node, which includes the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />
		/// and some ports, not the items in the <see cref="T:Northwoods.Go.GoListGroup" />.
		/// </remarks>
		public int ItemCount => myListGroup.Count;

		/// <summary>
		/// Gets or sets the object, normally a port, that is located at the middle
		/// top spot of the node.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject TopPort
		{
			get
			{
				return myTopPort;
			}
			set
			{
				GoObject goObject = myTopPort;
				if (goObject != value)
				{
					if (goObject != null)
					{
						base.Remove(goObject);
					}
					myTopPort = value;
					if (value != null)
					{
						base.Add(value);
					}
					Changed(3006, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the object, normally a port, that is located at the middle
		/// bottom spot of the node.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject BottomPort
		{
			get
			{
				return myBottomPort;
			}
			set
			{
				GoObject goObject = myBottomPort;
				if (goObject != value)
				{
					if (goObject != null)
					{
						base.Remove(goObject);
					}
					myBottomPort = value;
					if (value != null)
					{
						base.Add(value);
					}
					Changed(3007, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the standard width for all items, and the wrapping width for all <see cref="T:Northwoods.Go.GoText" /> items.
		/// </summary>
		/// <value>
		/// After changing the value of this property, the setter calls <see cref="M:Northwoods.Go.GoMultiTextNode.OnItemWidthChanged(System.Single)" />
		/// to modify the widths (and possibly other properties) of the child item objects.
		/// The default value is -1, which means that the <see cref="P:Northwoods.Go.GoObject.Width" /> property is
		/// not set on any child object.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(-1)]
		[Description("The width for all items, and the wrapping width for all text items")]
		public virtual float ItemWidth
		{
			get
			{
				return myItemWidth;
			}
			set
			{
				float num = myItemWidth;
				if (num != value)
				{
					myItemWidth = value;
					Changed(3008, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						OnItemWidthChanged(num);
					}
				}
			}
		}

		/// <summary>
		/// Get the <see cref="T:Northwoods.Go.GoListGroup" /> actually holding all of the items and painting
		/// its separator lines, borders, and background brush.
		/// </summary>
		/// <remarks>
		/// This <see cref="T:Northwoods.Go.GoListGroup" /> must have an <see cref="P:Northwoods.Go.GoListGroup.Orientation" />
		/// that is <c>Vertical</c>.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public GoListGroup ListGroup => myListGroup;

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoObject.Shadowed" /> property.
		/// </summary>
		public override bool Shadowed
		{
			get
			{
				return ListGroup.Shadowed;
			}
			set
			{
				ListGroup.Shadowed = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.Spacing" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The additional vertical distance between items.")]
		public float Spacing
		{
			get
			{
				return ListGroup.Spacing;
			}
			set
			{
				ListGroup.Spacing = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.Alignment" /> property.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(256)]
		[Description("How each item is positioned along the X axis.")]
		public int Alignment
		{
			get
			{
				return ListGroup.Alignment;
			}
			set
			{
				ListGroup.Alignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.LinePen" /> property.
		/// </summary>
		[Browsable(false)]
		public Pen LinePen
		{
			get
			{
				return ListGroup.LinePen;
			}
			set
			{
				ListGroup.LinePen = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.BorderPen" /> property.
		/// </summary>
		[Browsable(false)]
		public Pen BorderPen
		{
			get
			{
				return ListGroup.BorderPen;
			}
			set
			{
				ListGroup.BorderPen = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.Brush" /> property.
		/// </summary>
		[Browsable(false)]
		public Brush Brush
		{
			get
			{
				return ListGroup.Brush;
			}
			set
			{
				ListGroup.Brush = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.Corner" /> property.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum radial width and height of each corner")]
		public SizeF Corner
		{
			get
			{
				return ListGroup.Corner;
			}
			set
			{
				ListGroup.Corner = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.TopLeftMargin" /> property.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the left side and the top")]
		public SizeF TopLeftMargin
		{
			get
			{
				return ListGroup.TopLeftMargin;
			}
			set
			{
				ListGroup.TopLeftMargin = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />'s <see cref="P:Northwoods.Go.GoListGroup.BottomRightMargin" /> property.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the right side and the bottom")]
		public SizeF BottomRightMargin
		{
			get
			{
				return ListGroup.BottomRightMargin;
			}
			set
			{
				ListGroup.BottomRightMargin = value;
			}
		}

		/// <summary>
		/// Gets or sets the incremental distance at when the ports'
		/// <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" /> is extended
		/// to help reduce the amount of overlapping orthogonal links coming out of this node.
		/// </summary>
		/// <value>
		/// This defaults to 8.  Set this to zero to have
		/// <see cref="T:Northwoods.Go.GoPort" />.<see cref="M:Northwoods.Go.GoPort.GetFromEndSegmentLength(Northwoods.Go.IGoLink)" /> just
		/// return the value of <see cref="T:Northwoods.Go.GoPort" />.<see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />
		/// for the "from" end of links.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(8f)]
		public virtual float FromEndSegmentLengthStep
		{
			get
			{
				return myFromEndSegmentLengthStep;
			}
			set
			{
				float num = myFromEndSegmentLengthStep;
				if (num != value)
				{
					myFromEndSegmentLengthStep = value;
					Changed(3009, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the incremental distance at when the ports'
		/// <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" /> is extended
		/// to help reduce the amount of overlapping orthogonal links going into this node.
		/// </summary>
		/// <value>
		/// This defaults to 4.  Set this to zero to have
		/// <see cref="T:Northwoods.Go.GoPort" />.<see cref="M:Northwoods.Go.GoPort.GetToEndSegmentLength(Northwoods.Go.IGoLink)" /> just
		/// return the value of <see cref="T:Northwoods.Go.GoPort" />.<see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />
		/// for the "to" end of links.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(4f)]
		public virtual float ToEndSegmentLengthStep
		{
			get
			{
				return myToEndSegmentLengthStep;
			}
			set
			{
				float num = myToEndSegmentLengthStep;
				if (num != value)
				{
					myToEndSegmentLengthStep = value;
					Changed(3010, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Create an empty-lookng node, containing no items and only the top and bottom ports.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoMultiTextNode.LinePen" /> and <see cref="P:Northwoods.Go.GoMultiTextNode.BorderPen" /> properties default to black pens.
		/// This constructs a <see cref="T:Northwoods.Go.GoListGroup" /> as the initial value for <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />.
		/// This calls <see cref="M:Northwoods.Go.GoMultiTextNode.CreateEndPort(System.Boolean)" /> to get initial values for both
		/// <see cref="P:Northwoods.Go.GoMultiTextNode.TopPort" /> and <see cref="P:Northwoods.Go.GoMultiTextNode.BottomPort" />.
		/// </remarks>
		public GoMultiTextNode()
		{
			base.Initializing = true;
			myListGroup = new GoMultiTextNodeListGroup();
			myListGroup.Selectable = false;
			myListGroup.LinePen = GoShape.Pens_Black;
			myListGroup.BorderPen = GoShape.Pens_Black;
			myListGroup.Alignment = 1;
			Add(myListGroup);
			myTopPort = CreateEndPort(top: true);
			Add(myTopPort);
			myBottomPort = CreateEndPort(top: false);
			Add(myBottomPort);
			base.InternalFlags &= -17;
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Copy the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" /> and all of the ports.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			GoMultiTextNode goMultiTextNode = (GoMultiTextNode)newgroup;
			base.CopyChildren(newgroup, env);
			goMultiTextNode.myLeftPorts = new List<GoObject>();
			goMultiTextNode.myRightPorts = new List<GoObject>();
			goMultiTextNode.myListGroup = (GoListGroup)env[myListGroup];
			goMultiTextNode.myTopPort = (GoObject)env[myTopPort];
			goMultiTextNode.myBottomPort = (GoObject)env[myBottomPort];
			checked
			{
				for (int i = 0; i < myLeftPorts.Count; i++)
				{
					GoObject key = myLeftPorts[i];
					GoObject item = (GoObject)env[key];
					goMultiTextNode.myLeftPorts.Add(item);
				}
				for (int j = 0; j < myRightPorts.Count; j++)
				{
					GoObject key2 = myRightPorts[j];
					GoObject item2 = (GoObject)env[key2];
					goMultiTextNode.myRightPorts.Add(item2);
				}
			}
		}

		/// <summary>
		/// Create a top or bottom port for this node.
		/// </summary>
		/// <param name="top"></param>
		/// <returns></returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  public virtual GoObject CreateEndPort(bool top) {
		///    GoPort p = new GoPort();
		///    p.Size = new SizeF(5, 3);
		///    p.Style = GoPortStyle.None;
		///    if (top) {
		///      p.FromSpot = MiddleTop;
		///      p.ToSpot = MiddleTop;
		///    } else {
		///      p.FromSpot = MiddleBottom;
		///      p.ToSpot = MiddleBottom;
		///    }
		///    return p;
		///  }
		/// </code>
		/// </example>
		public virtual GoObject CreateEndPort(bool top)
		{
			GoPort goPort = new GoPort();
			goPort.Size = new SizeF(5f, 3f);
			goPort.Style = GoPortStyle.None;
			if (top)
			{
				goPort.FromSpot = 32;
				goPort.ToSpot = 32;
			}
			else
			{
				goPort.FromSpot = 128;
				goPort.ToSpot = 128;
			}
			return goPort;
		}

		/// <summary>
		/// Create and initialize a port that may go on either the left side or the right side.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="idx"></param>
		/// <returns>by default, a <see cref="T:Northwoods.Go.GoPort" /></returns>
		/// <remarks>
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  public virtual GoObject CreatePort(bool left, int idx) {
		///    GoPort p = new GoPort();
		///    p.Size = new SizeF(3, 5);
		///    p.Style = GoPortStyle.None;
		///    if (left) {
		///      p.FromSpot = MiddleLeft;
		///      p.ToSpot = MiddleLeft;
		///    } else {
		///      p.FromSpot = MiddleRight;
		///      p.ToSpot = MiddleRight;
		///    }
		///    return p;
		///  }
		/// </code>
		/// </example>
		public virtual GoObject CreatePort(bool left, int idx)
		{
			GoPort goPort = new GoPort();
			goPort.Size = new SizeF(3f, 5f);
			goPort.Style = GoPortStyle.None;
			if (left)
			{
				goPort.FromSpot = 256;
				goPort.ToSpot = 256;
			}
			else
			{
				goPort.FromSpot = 64;
				goPort.ToSpot = 64;
			}
			return goPort;
		}

		/// <summary>
		/// Create and initialize a text item given a string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="idx"></param>
		/// <returns>a <see cref="T:Northwoods.Go.GoText" /> object that is the new item</returns>
		/// <remarks>
		/// Note that the width of the text object is not set unless
		/// <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> is a positive value.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  public virtual GoText CreateText(String s, int idx) {
		///    GoText t = new GoText();
		///    t.Selectable = false;
		///    t.Alignment = GoObject.Middle;
		///    t.Multiline = true;
		///    t.BackgroundOpaqueWhenSelected = true;
		///    t.BackgroundColor = Color.LightBlue;
		///    t.DragsNode = true;
		///    t.Text = s;
		///    t.Wrapping = true;
		///    if (this.ItemWidth &gt; 0) {
		///      t.WrappingWidth = this.ItemWidth;
		///      t.Width = this.ItemWidth;
		///    }
		///    return t;
		///  }
		/// </code>
		/// </example>
		public virtual GoText CreateText(string s, int idx)
		{
			GoText goText = new GoText();
			goText.Selectable = false;
			goText.Alignment = 1;
			goText.Multiline = true;
			goText.BackgroundOpaqueWhenSelected = true;
			goText.BackgroundColor = Color.LightBlue;
			goText.DragsNode = true;
			goText.Text = s;
			goText.Wrapping = true;
			if (ItemWidth > 0f)
			{
				goText.WrappingWidth = ItemWidth;
				goText.Width = ItemWidth;
			}
			return goText;
		}

		/// <summary>
		/// Position all of the left and right port objects at the sides of each of the items.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true, this method does nothing.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (!base.Initializing && myListGroup != null)
			{
				base.Initializing = true;
				if (TopPort != null)
				{
					TopPort.SetSpotLocation(128, myListGroup, 32);
				}
				if (BottomPort != null)
				{
					BottomPort.SetSpotLocation(32, myListGroup, 128);
				}
				int num = 0;
				foreach (GoObject item in myListGroup)
				{
					if (item != null && num < myLeftPorts.Count)
					{
						GoObject goObject = myLeftPorts[num];
						if (goObject != null)
						{
							if (item.Visible)
							{
								PointF spotLocation = item.GetSpotLocation(256);
								spotLocation.X = myListGroup.Left;
								goObject.SetSpotLocation(64, spotLocation);
							}
							else
							{
								goObject.SetSpotLocation(64, new PointF(myListGroup.Left, item.Top));
							}
						}
					}
					if (item != null && num < myRightPorts.Count)
					{
						GoObject goObject2 = myRightPorts[num];
						if (goObject2 != null)
						{
							if (item.Visible)
							{
								PointF spotLocation2 = item.GetSpotLocation(64);
								spotLocation2.X = myListGroup.Right;
								goObject2.SetSpotLocation(256, spotLocation2);
							}
							else
							{
								goObject2.SetSpotLocation(256, new PointF(myListGroup.Right, item.Top));
							}
						}
					}
					num = checked(num + 1);
				}
				base.Initializing = false;
			}
		}

		/// <summary>
		/// This convenience method gets the string associated with a given item in the list.
		/// </summary>
		/// <param name="i"></param>
		/// <returns>
		/// if the <paramref name="i" />'th item is a <see cref="T:Northwoods.Go.GoText" />,
		/// this returns its <see cref="P:Northwoods.Go.GoText.Text" /> property,
		/// otherwise it returns an empty string
		/// </returns>
		public virtual string GetString(int i)
		{
			GoText goText = myListGroup[i] as GoText;
			if (goText != null)
			{
				return goText.Text;
			}
			return "";
		}

		/// <summary>
		/// This convenience method sets the string associated with a given item in the list.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="s">a new <see cref="P:Northwoods.Go.GoText.Text" /> value</param>
		/// <remarks>
		/// If the <paramref name="i" />'th item is not a <see cref="T:Northwoods.Go.GoText" />, this method does nothing.
		/// </remarks>
		public virtual void SetString(int i, string s)
		{
			GoText goText = myListGroup[i] as GoText;
			if (goText != null)
			{
				goText.Text = s;
			}
		}

		/// <summary>
		/// This convenience method adds an item containing a string to the end of the list.
		/// </summary>
		/// <param name="s">the initial <see cref="P:Northwoods.Go.GoText.Text" /> value for the new <see cref="T:Northwoods.Go.GoText" /> item</param>
		/// <returns>the new <see cref="T:Northwoods.Go.GoText" /> item</returns>
		/// <remarks>
		/// This is defined as:
		/// <pre><code>
		///   int num = this.ItemCount;
		///   GoText t = CreateText(s, num);
		///   AddItem(t, CreatePort(true, num), CreatePort(false, num));
		///   return t;
		/// </code></pre>
		/// </remarks>
		public virtual GoText AddString(string s)
		{
			int itemCount = ItemCount;
			GoText goText = CreateText(s, itemCount);
			AddItem(goText, CreatePort(left: true, itemCount), CreatePort(left: false, itemCount));
			return goText;
		}

		/// <summary>
		/// This convenience method inserts an item containing a string into the list.
		/// </summary>
		/// <param name="i">the intended index for the item and its accompanying objects</param>
		/// <param name="s">the initial <see cref="P:Northwoods.Go.GoText.Text" /> value for the new <see cref="T:Northwoods.Go.GoText" /> item</param>
		/// <returns>the new <see cref="T:Northwoods.Go.GoText" /> item</returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoMultiTextNode.CreateText(System.String,System.Int32)" /> and <see cref="M:Northwoods.Go.GoMultiTextNode.InsertItem(System.Int32,Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoObject)" /> with
		/// the resulting <see cref="T:Northwoods.Go.GoText" /> object and the results of two calls to
		/// <see cref="M:Northwoods.Go.GoMultiTextNode.CreatePort(System.Boolean,System.Int32)" />.
		/// </remarks>
		public virtual GoText InsertString(int i, string s)
		{
			if (i < 0)
			{
				i = 0;
			}
			else if (i > ItemCount)
			{
				i = ItemCount;
			}
			GoText goText = CreateText(s, i);
			InsertItem(i, goText, CreatePort(left: true, i), CreatePort(left: false, i));
			return goText;
		}

		/// <summary>
		/// Gets the item object at a particular index.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public GoObject GetItem(int i)
		{
			return myListGroup[i];
		}

		/// <summary>
		/// Replaces an item object at a particular index.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="obj"></param>
		/// <remarks>
		/// This does not change either associated "port" object.
		/// </remarks>
		public void SetItem(int i, GoObject obj)
		{
			myListGroup[i] = obj;
		}

		/// <summary>
		/// Add an item and associated objects at a particular index.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="leftport"></param>
		/// <param name="rightport"></param>
		public void AddItem(GoObject item, GoObject leftport, GoObject rightport)
		{
			InsertItem(ItemCount, item, leftport, rightport);
		}

		/// <summary>
		/// Insert an item and its ports to be at a particular position in the list.
		/// </summary>
		/// <param name="i">the intended index for the item and its accompanying objects</param>
		/// <param name="item">any <see cref="T:Northwoods.Go.GoObject" /></param>
		/// <param name="leftport">any object, but typically a <see cref="T:Northwoods.Go.GoPort" /> or null</param>
		/// <param name="rightport">any object, but typically a <see cref="T:Northwoods.Go.GoPort" /> or null</param>
		public virtual void InsertItem(int i, GoObject item, GoObject leftport, GoObject rightport)
		{
			if (i < 0)
			{
				i = 0;
			}
			else if (i > ItemCount)
			{
				i = ItemCount;
			}
			myListGroup.Insert(i, item);
			if (i >= 0 && i <= myLeftPorts.Count)
			{
				myLeftPorts.Insert(i, leftport);
				Add(leftport);
				Changed(3001, i, null, GoObject.NullRect, i, leftport, GoObject.NullRect);
			}
			if (i >= 0 && i <= myRightPorts.Count)
			{
				myRightPorts.Insert(i, rightport);
				Add(rightport);
				Changed(3002, i, null, GoObject.NullRect, i, rightport, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Remove the item and its ports that are at a given index.
		/// </summary>
		/// <param name="i"></param>
		public virtual void RemoveItem(int i)
		{
			myListGroup.RemoveAt(i);
		}

		/// <summary>
		/// Determine how to change the width of all of items in the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />
		/// when the <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> changes.
		/// </summary>
		/// <param name="old">the former <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> value</param>
		/// <remarks>
		/// This changes the <see cref="P:Northwoods.Go.GoObject.Width" /> of each item to the new
		/// <see cref="P:Northwoods.Go.GoMultiTextNode.ItemWidth" /> value, if the new width is positive.
		/// Furthermore, if the item is an instance of <see cref="T:Northwoods.Go.GoText" />, the
		/// <see cref="P:Northwoods.Go.GoText.WrappingWidth" /> is set to the same new item width.
		/// A non-positive value does not change either <see cref="P:Northwoods.Go.GoObject.Width" /> or
		/// <see cref="P:Northwoods.Go.GoText.WrappingWidth" />.
		/// You may wish to override this method in order to customize how the
		/// properties of the items are set when the item width is changed.
		/// </remarks>
		public virtual void OnItemWidthChanged(float old)
		{
			float itemWidth = ItemWidth;
			if (!(itemWidth <= 0f))
			{
				foreach (GoObject item in ListGroup)
				{
					GoText goText = item as GoText;
					if (goText != null)
					{
						goText.WrappingWidth = itemWidth;
					}
					item.Width = itemWidth;
				}
			}
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		/// <remarks>
		/// LeftPorts and RightPorts have generated names that are of the form:
		/// "Lnnn" or "Rnnn", where "nnn" is the index of that port.
		/// </remarks>
		public override string FindName(GoObject child)
		{
			if (child == ListGroup)
			{
				return "ListGroup";
			}
			if (child == TopPort)
			{
				return "TopPort";
			}
			if (child == BottomPort)
			{
				return "BottomPort";
			}
			int num = myLeftPorts.IndexOf(child);
			if (num >= 0)
			{
				return "L" + num.ToString(NumberFormatInfo.InvariantInfo);
			}
			num = myRightPorts.IndexOf(child);
			if (num >= 0)
			{
				return "R" + num.ToString(NumberFormatInfo.InvariantInfo);
			}
			return base.FindName(child);
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// LeftPorts and RightPorts have generated names that are of the form:
		/// "Lnnn" or "Rnnn", where "nnn" is the index of that port.
		/// </remarks>
		public override GoObject FindChild(string name)
		{
			if (name == "ListGroup")
			{
				return ListGroup;
			}
			if (name == "TopPort")
			{
				return TopPort;
			}
			if (name == "BottomPort")
			{
				return BottomPort;
			}
			if (name.Length >= 2)
			{
				if (name[0] == 'L')
				{
					int num = -1;
					try
					{
						num = int.Parse(name.Substring(1), NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
					if (num >= 0 && num < myLeftPorts.Count)
					{
						return myLeftPorts[num];
					}
				}
				else if (name[0] == 'R')
				{
					int num2 = -1;
					try
					{
						num2 = int.Parse(name.Substring(1), NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
					if (num2 >= 0 && num2 < myRightPorts.Count)
					{
						return myRightPorts[num2];
					}
				}
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// Return a port by its index position on the left side of this node.
		/// </summary>
		/// <param name="i">a zero-based, non-negative index</param>
		/// <returns>null if <paramref name="i" /> is greater than or equal to <see cref="P:Northwoods.Go.GoMultiTextNode.ItemCount" /></returns>
		public virtual GoObject GetLeftPort(int i)
		{
			if (i < 0 || i >= myLeftPorts.Count)
			{
				return null;
			}
			return myLeftPorts[i];
		}

		/// <summary>
		/// Return a port by its index position on the right side of this node.
		/// </summary>
		/// <param name="i">a zero-based, non-negative index</param>
		/// <returns>null if <paramref name="i" /> is greater than or equal to <see cref="P:Northwoods.Go.GoMultiTextNode.ItemCount" /></returns>
		public virtual GoObject GetRightPort(int i)
		{
			if (i < 0 || i >= myRightPorts.Count)
			{
				return null;
			}
			return myRightPorts[i];
		}

		/// <summary>
		/// Replace the port at a particular index position on the
		/// left side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		/// <param name="p"></param>
		public virtual void SetLeftPort(int i, GoObject p)
		{
			GoObject leftPort = GetLeftPort(i);
			if (leftPort == p)
			{
				return;
			}
			if (leftPort != null)
			{
				if (p != null)
				{
					p.Bounds = leftPort.Bounds;
				}
				base.Remove(leftPort);
			}
			myLeftPorts[i] = p;
			Add(p);
			checked
			{
				Changed(3005, -(i + 1), leftPort, GoObject.NullRect, -(i + 1), p, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Replace the port at a particular index position on the
		/// right side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		/// <param name="p"></param>
		public virtual void SetRightPort(int i, GoObject p)
		{
			GoObject rightPort = GetRightPort(i);
			if (rightPort == p)
			{
				return;
			}
			if (rightPort != null)
			{
				if (p != null)
				{
					p.Bounds = rightPort.Bounds;
				}
				base.Remove(rightPort);
			}
			myRightPorts[i] = p;
			Add(p);
			Changed(3005, i, rightPort, GoObject.NullRect, i, p, GoObject.NullRect);
		}

		/// <summary>
		/// When a port is removed, make sure we remove any reference to it.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Removing any child object, whether it be an "item" object or a "port" object,
		/// does not remove the whole item from the list.
		/// </remarks>
		public override bool Remove(GoObject obj)
		{
			int num = myLeftPorts.IndexOf(obj);
			if (num >= 0)
			{
				myLeftPorts[num] = null;
			}
			else
			{
				int num2 = myRightPorts.IndexOf(obj);
				if (num2 >= 0)
				{
					myRightPorts[num2] = null;
				}
			}
			return base.Remove(obj);
		}

		internal void RemoveOnlyPorts(int i)
		{
			if (i >= 0 && i < myLeftPorts.Count)
			{
				GoObject goObject = myLeftPorts[i];
				myLeftPorts.RemoveAt(i);
				if (goObject != null)
				{
					base.Remove(goObject);
				}
				Changed(3003, i, goObject, GoObject.NullRect, i, null, GoObject.NullRect);
			}
			if (i >= 0 && i < myRightPorts.Count)
			{
				GoObject goObject2 = myRightPorts[i];
				myRightPorts.RemoveAt(i);
				if (goObject2 != null)
				{
					base.Remove(goObject2);
				}
				Changed(3004, i, goObject2, GoObject.NullRect, i, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Given a "port" object, find the item index whose <see cref="M:Northwoods.Go.GoMultiTextNode.GetLeftPort(System.Int32)" />
		/// or <see cref="M:Northwoods.Go.GoMultiTextNode.GetRightPort(System.Int32)" /> returns that object.
		/// </summary>
		/// <param name="p"></param>
		/// <returns>
		/// -1 if <paramref name="p" /> is null or is not found amongst the LeftPorts or the RightPorts.
		/// </returns>
		/// <remarks>
		/// This does not search the <see cref="P:Northwoods.Go.GoMultiTextNode.ListGroup" />.
		/// </remarks>
		public virtual int FindPortIndex(GoObject p)
		{
			if (p == null)
			{
				return -1;
			}
			int num = myLeftPorts.IndexOf(p);
			if (num >= 0)
			{
				return num;
			}
			int num2 = myRightPorts.IndexOf(p);
			if (num2 >= 0)
			{
				return num2;
			}
			return -1;
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
			case 3001:
			{
				int newInt = e.NewInt;
				GoObject goObject2 = (GoObject)e.GetValue(undo);
				if (!undo)
				{
					myLeftPorts.Insert(newInt, goObject2);
					Add(goObject2);
				}
				break;
			}
			case 3002:
			{
				int newInt2 = e.NewInt;
				GoObject goObject4 = (GoObject)e.GetValue(undo);
				if (!undo)
				{
					myRightPorts.Insert(newInt2, goObject4);
					Add(goObject4);
				}
				break;
			}
			case 3003:
			{
				int oldInt2 = e.OldInt;
				GoObject goObject = (GoObject)e.GetValue(undo);
				if (undo)
				{
					myLeftPorts.Insert(oldInt2, goObject);
					Add(goObject);
				}
				break;
			}
			case 3004:
			{
				int oldInt3 = e.OldInt;
				GoObject goObject3 = (GoObject)e.GetValue(undo);
				if (undo)
				{
					myRightPorts.Insert(oldInt3, goObject3);
					Add(goObject3);
				}
				break;
			}
			case 3005:
			{
				int oldInt = e.OldInt;
				if (oldInt < 0)
				{
					oldInt = checked(-oldInt - 1);
					SetLeftPort(oldInt, (GoObject)e.GetValue(undo));
				}
				else
				{
					SetRightPort(oldInt, (GoObject)e.GetValue(undo));
				}
				break;
			}
			case 3006:
				TopPort = (GoObject)e.GetValue(undo);
				break;
			case 3007:
				BottomPort = (GoObject)e.GetValue(undo);
				break;
			case 3008:
				ItemWidth = e.GetFloat(undo);
				break;
			case 3009:
				FromEndSegmentLengthStep = e.GetFloat(undo);
				break;
			case 3010:
				ToEndSegmentLengthStep = e.GetFloat(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
