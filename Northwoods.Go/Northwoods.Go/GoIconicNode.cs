using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A node with an image for an icon, a label, and a single port centered on the image.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Call the <c>Initialize</c> method to construct the standard parts of this kind of node.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> is normally a <see cref="T:Northwoods.Go.GoImage" />,
	/// but could be any kind of <see cref="T:Northwoods.Go.GoObject" />.
	/// When it is a <see cref="T:Northwoods.Go.GoImage" />, the <see cref="P:Northwoods.Go.GoIconicNode.Image" /> property gives you
	/// convenient access to the icon's image properties.
	/// The image will normally be sized to match the natural size of the <c>Image</c>
	/// being drawn.  You may want to set the <c>Icon.Size</c> property to make sure the
	/// icon isn't too big or too small.
	/// </para>
	/// <para>
	/// When then first two arguments to <see cref="M:Northwoods.Go.GoIconicNode.Initialize(System.Resources.ResourceManager,System.String,System.String)" />
	/// are null, the node is initialized to use a <see cref="T:Northwoods.Go.GoDrawing" /> instead of a
	/// <see cref="T:Northwoods.Go.GoImage" />.  After initialization you can set the <see cref="P:Northwoods.Go.GoIconicNode.Figure" />
	/// property if you want to display one of the standard <see cref="T:Northwoods.Go.GoFigure" />s.
	/// </para>
	/// <para>
	/// You can also set the <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> to be any kind of <see cref="T:Northwoods.Go.GoObject" />,
	/// including other kinds of <see cref="T:Northwoods.Go.GoShape" />s.  The <see cref="P:Northwoods.Go.GoIconicNode.Shape" /> property
	/// provides convenient access to the icon when it is a <see cref="T:Northwoods.Go.GoShape" />, to allow
	/// you to initialize its brush and/or pen.  If your icon is a <see cref="T:Northwoods.Go.GoGroup" />,
	/// it might be convenient to override <see cref="P:Northwoods.Go.GoIconicNode.Shape" /> and perhaps <see cref="P:Northwoods.Go.GoIconicNode.Figure" />
	/// to refer to the appropriate shape or drawing in the group.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoIconicNode.Label" /> is normally centered below the <see cref="P:Northwoods.Go.GoIconicNode.Icon" />.
	/// You can set the <see cref="P:Northwoods.Go.GoIconicNode.LabelOffset" /> property to position it elsewhere
	/// relative to the <see cref="P:Northwoods.Go.GoIconicNode.Icon" />.
	/// Furthermore, you can set the <see cref="P:Northwoods.Go.GoIconicNode.DraggableLabel" /> property to allow the
	/// user to move it around.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoIconicNode : GoNode
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoIconicNode.DraggableLabel" /> property.
		/// </summary>
		public const int ChangedDraggableLabel = 2651;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> property.
		/// </summary>
		public const int ChangedIcon = 2652;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoIconicNode.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2653;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoIconicNode.Port" /> property.
		/// </summary>
		public const int ChangedPort = 2654;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoIconicNode.LabelOffset" /> property.
		/// </summary>
		public const int ChangedLabelOffset = 2655;

		private const int flagDraggableLabel = 16777216;

		private GoObject myIcon;

		private GoText myLabel;

		private GoPort myPort;

		private SizeF myLabelOffset = new SizeF(-99999f, -99999f);

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoObject" /> acting as the icon for this node.
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoIconicNode.CreateIcon(System.Resources.ResourceManager,System.String)" /> method(s)
		/// if you want to create a different kind of <see cref="T:Northwoods.Go.GoImage" /> when
		/// constructing this kind of node.
		/// The new shape will have its Center location,
		/// Selectable, Resizable, Reshapable, ResizesRealtime, and Shadowed
		/// properties copied from the old shape.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject Icon
		{
			get
			{
				return myIcon;
			}
			set
			{
				GoObject goObject = myIcon;
				if (goObject != value)
				{
					CopyPropertiesFromSelectionObject(goObject, value);
					if (goObject != null)
					{
						Remove(goObject);
					}
					myIcon = value;
					if (value != null)
					{
						InsertBefore(null, value);
					}
					Changed(2652, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && Port != null && Port.PortObject == goObject)
					{
						Port.PortObject = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets this node's icon, assuming it is a <see cref="T:Northwoods.Go.GoImage" />, as it usually is.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoImage Image => Icon as GoImage;

		/// <summary>
		/// Gets this node's icon, assuming it is a <see cref="T:Northwoods.Go.GoShape" />,
		/// as it would be if you call <see cref="M:Northwoods.Go.GoIconicNode.Initialize(System.Resources.ResourceManager,System.String,System.String)" />
		/// with a null String icon name.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape => Icon as GoShape;

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDrawing" />.<see cref="P:Northwoods.Go.GoDrawing.Figure" />
		/// of the <see cref="P:Northwoods.Go.GoIconicNode.Shape" /> if the icon is a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		/// <value>
		/// This returns <see cref="T:Northwoods.Go.GoFigure" />.<see cref="F:Northwoods.Go.GoFigure.None" />
		/// in the typical case where the <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> is a <see cref="T:Northwoods.Go.GoImage" />.
		/// Setting this property has no effect if the <see cref="P:Northwoods.Go.GoIconicNode.Shape" /> is not a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoFigure.None)]
		[Description("The GoFigure defining the shape of the Icon, if it is a GoDrawing")]
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
		/// Gets or sets the <see cref="T:Northwoods.Go.GoText" /> label.
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoIconicNode.CreateLabel(System.String)" /> method
		/// if you want to create a different kind of <see cref="T:Northwoods.Go.GoText" /> when
		/// constructing this kind of node.
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
					Changed(2653, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the only port of this node, a <see cref="T:Northwoods.Go.GoPort" />.
		/// </summary>
		/// <remarks>
		/// Setting this property to a new port will also set that port's
		/// <see cref="P:Northwoods.Go.GoPort.PortObject" /> to be this node,
		/// if it didn't already have a <see cref="P:Northwoods.Go.GoPort.PortObject" />.
		/// You might want to override the <see cref="M:Northwoods.Go.GoIconicNode.CreatePort" /> method
		/// if you want to create a different kind of <see cref="T:Northwoods.Go.GoPort" /> when
		/// constructing this kind of node.
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
					Changed(2654, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && value != null && value.PortObject == null)
					{
						value.PortObject = Icon;
					}
				}
			}
		}

		/// <summary>
		/// The selection object, what the user sees as being selected and what the user
		/// actually resizes, is the icon.
		/// </summary>
		public override GoObject SelectionObject
		{
			get
			{
				GoObject icon = Icon;
				if (icon != null)
				{
					return icon;
				}
				return this;
			}
		}

		/// <summary>
		/// Gets or sets whether the user can drag the label around independently of the node.
		/// </summary>
		/// <value>
		/// Initially this value is false--the label is always positioned by
		/// <see cref="M:Northwoods.Go.GoIconicNode.LayoutChildren(Northwoods.Go.GoObject)" /> and users cannot move the
		/// label without moving the whole node.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether users can drag the label independently of the node")]
		public virtual bool DraggableLabel
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
					Changed(2651, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && Label != null)
					{
						Label.Selectable = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the relative position of the <see cref="P:Northwoods.Go.GoIconicNode.Label" />'s position
		/// compared to the <see cref="P:Northwoods.Go.GoIconicNode.Icon" />'s position.
		/// </summary>
		/// <value>
		/// A very large negative offset causes <see cref="M:Northwoods.Go.GoIconicNode.LayoutChildren(Northwoods.Go.GoObject)" /> to
		/// ignore this value, resulting in the <see cref="P:Northwoods.Go.GoIconicNode.Label" /> being
		/// placed underneath the <see cref="P:Northwoods.Go.GoIconicNode.Icon" />.
		/// </value>
		/// <remarks>
		/// This is automatically updated if the <see cref="P:Northwoods.Go.GoIconicNode.DraggableLabel" /> is 
		/// true and the user drags the label around.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The offset of the Label relative to the Icon")]
		public virtual SizeF LabelOffset
		{
			get
			{
				return myLabelOffset;
			}
			set
			{
				SizeF sizeF = myLabelOffset;
				if (sizeF != value)
				{
					myLabelOffset = value;
					Changed(2655, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Initialize an empty GoIconicNode to have an icon, a label, and one port.
		/// </summary>
		/// <param name="res">
		/// Provides the <c>ResourceManager</c> holding an <c>Image</c> resource named by
		/// <paramref name="iconname" />.  If this parameter is null,
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is used instead.
		/// </param>
		/// <param name="iconname">
		/// <para>
		/// The name of the <c>Image</c> resource in the <c>ResourceManager</c>
		/// given by <paramref name="res" />, or else a file name if no resource manager
		/// can be used (i.e., when both <paramref name="res" /> is null and
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is null).
		/// </para>
		/// <para>
		/// If the value is an empty string, the <c>Image</c> will be blank;
		/// you can set <see cref="P:Northwoods.Go.GoIconicNode.Image" />.<see cref="P:Northwoods.Go.GoImage.Name" /> to show or change
		/// the image displayed by the <see cref="T:Northwoods.Go.GoImage" /> that is the <see cref="P:Northwoods.Go.GoIconicNode.Image" />.
		/// </para>
		/// <para>
		/// If the value is null, the <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> is not a <see cref="T:Northwoods.Go.GoImage" />
		/// but a <see cref="T:Northwoods.Go.GoDrawing" />; you can then set the <see cref="P:Northwoods.Go.GoIconicNode.Figure" />
		/// to change the shape shown as the icon.
		/// </para>
		/// </param>
		/// <param name="name">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoIconicNode.Label" />.
		/// If this value is null, no label is created for this node.
		/// </param>
		public virtual void Initialize(ResourceManager res, string iconname, string name)
		{
			base.Initializing = true;
			myIcon = CreateIcon(res, iconname);
			Add(myIcon);
			initializeCommon(name);
		}

		/// <summary>
		/// Initialize an empty GoIconicNode to have an icon, a label, and one port.
		/// </summary>
		/// <param name="imglist">
		/// Provide the <c>ImageList</c> whose <paramref name="imgindex" /> specifies
		/// the actual image to use for the icon.  If this is null, the
		/// <see cref="P:Northwoods.Go.GoView.ImageList" /> property is used by <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// </param>
		/// <param name="imgindex">
		/// The zero-based index of the <c>Image</c> contained in an <c>ImageList</c>,
		/// given either by <paramref name="imglist" /> or by <see cref="P:Northwoods.Go.GoView.ImageList" />.
		/// </param>
		/// <param name="name">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoIconicNode.Label" />.
		/// If this value is null, no label is created for this node.
		/// </param>
		public virtual void Initialize(ImageList imglist, int imgindex, string name)
		{
			base.Initializing = true;
			myIcon = CreateIcon(imglist, imgindex);
			Add(myIcon);
			initializeCommon(name);
		}

		private void initializeCommon(string name)
		{
			myLabel = CreateLabel(name);
			Add(myLabel);
			myPort = CreatePort();
			Add(myPort);
			base.PropertiesDelegatedToSelectionObject = true;
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoImage" /> or a <see cref="T:Northwoods.Go.GoDrawing" /> to act as the node's icon.
		/// </summary>
		/// <param name="res">
		/// Provides the <c>ResourceManager</c> holding an <c>Image</c> resource named by
		/// <paramref name="iconname" />.  If this parameter is null,
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is used instead.
		/// </param>
		/// <param name="iconname">
		/// <para>
		/// The name of the <c>Image</c> resource in the <c>ResourceManager</c>
		/// given by <paramref name="res" />, or else a file name if no resource manager
		/// can be used (i.e., when both <paramref name="res" /> is null and
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is null).
		/// </para>
		/// <para>
		/// If the value is an empty string, the <c>Image</c> will be blank;
		/// you can set <see cref="P:Northwoods.Go.GoIconicNode.Image" />.<see cref="P:Northwoods.Go.GoImage.Name" /> to show or change
		/// the image displayed by the <see cref="T:Northwoods.Go.GoImage" /> that is the <see cref="P:Northwoods.Go.GoIconicNode.Image" />.
		/// </para>
		/// <para>
		/// If the value is null, the <see cref="P:Northwoods.Go.GoIconicNode.Icon" /> is not a <see cref="T:Northwoods.Go.GoImage" />
		/// but a <see cref="T:Northwoods.Go.GoDrawing" />; you can then set the <see cref="P:Northwoods.Go.GoIconicNode.Figure" />
		/// to change the shape shown as the icon.
		/// </para>
		/// </param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoImage" />
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(ResourceManager res, String iconname) {
		///    if (iconname != null) {
		///      GoImage img = new GoImage();
		///      if (res != null)
		///        img.ResourceManager = res;
		///      img.Name = iconname;
		///      img.Selectable = false;
		///      img.Resizable = false;
		///      return img;
		///    } else {
		///      GoDrawing rect = new GoDrawing(GoFigure.Rectangle);
		///      rect.Selectable = false;
		///      rect.Resizable = false;
		///      rect.Size = new SizeF(40, 40);
		///      return rect;
		///    }
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ResourceManager res, string iconname)
		{
			if (iconname != null)
			{
				GoImage goImage = new GoImage();
				if (res != null)
				{
					goImage.ResourceManager = res;
				}
				goImage.Name = iconname;
				goImage.Selectable = false;
				goImage.Resizable = false;
				return goImage;
			}
			return new GoDrawing(GoFigure.Rectangle)
			{
				Selectable = false,
				Resizable = false,
				Size = new SizeF(40f, 40f)
			};
		}

		/// <summary>
		/// Create and initialize an image to act as the node's icon.
		/// </summary>
		/// <param name="imglist"></param>
		/// <param name="imgindex"></param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoImage" />
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(System.Windows.Forms.ImageList imglist, int imgindex) {
		///    GoImage img = new GoImage();
		///    img.ImageList = imglist;
		///    img.Index = imgindex;
		///    img.Selectable = false;
		///    img.Resizable = false;
		///    return img;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ImageList imglist, int imgindex)
		{
			return new GoImage
			{
				ImageList = imglist,
				Index = imgindex,
				Selectable = false,
				Resizable = false
			};
		}

		/// <summary>
		/// Create and initialize a text label.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>a non-editable, non-selectable, middle-aligned, non-rescaling <see cref="T:Northwoods.Go.GoText" /> object</returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel(String name) {
		///    GoText l = null;
		///    if (name != null) {
		///      l = new GoText();
		///      l.Text = name;
		///      l.Selectable = this.DraggableLabel;
		///      l.Alignment = MiddleTop;
		///    }
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel(string name)
		{
			GoText goText = null;
			if (name != null)
			{
				goText = new GoText();
				goText.Text = name;
				goText.Selectable = DraggableLabel;
				goText.Alignment = 32;
			}
			return goText;
		}

		/// <summary>
		/// Create and initialize the node's port, which is normally not visible and at
		/// the middle of the icon.
		/// </summary>
		/// <returns>a <see cref="T:Northwoods.Go.GoPort" /></returns>
		/// <remarks>
		/// <see cref="P:Northwoods.Go.GoPort.IsValidFrom" /> and <see cref="P:Northwoods.Go.GoPort.IsValidTo" />
		/// are true, by default, thus allowing users to draw links from or
		/// to these nodes.  You may want to set those properties to false
		/// if you want prevent users from drawing links with a mouse-down
		/// and drag from the port, or to create a port with a different size
		/// or appearance.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoPort CreatePort() {
		///    GoPort p = new GoPort();
		///    p.Style = GoPortStyle.None;
		///    p.Size = new SizeF(6, 6);
		///    p.FromSpot = NoSpot;
		///    p.ToSpot = NoSpot;
		///    p.PortObject = this;
		///    return p;
		///  }
		/// </code>
		/// </example>
		protected virtual GoPort CreatePort()
		{
			return new GoPort
			{
				Style = GoPortStyle.None,
				Size = new SizeF(6f, 6f),
				FromSpot = 0,
				ToSpot = 0,
				PortObject = this
			};
		}

		/// <summary>
		/// Make copies of the icon, label and port.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		/// <remarks>
		/// Remember to override this to copy any objects you add to this class.
		/// </remarks>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoIconicNode obj = (GoIconicNode)newgroup;
			obj.myIcon = (GoObject)env[myIcon];
			obj.myLabel = (GoText)env[myLabel];
			obj.myPort = (GoPort)env[myPort];
		}

		/// <summary>
		/// Because the label may be selectable and deleted by the user,
		/// be sure to remove any reference to the label.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Remember to override this to clear any child object references you add to this class.
		/// </remarks>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myIcon)
			{
				myIcon = null;
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
		/// Position the label and port relative to the icon.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// Initially the label is positioned below the icon.
		/// When <see cref="P:Northwoods.Go.GoIconicNode.DraggableLabel" /> is true, the user may select
		/// and drag the label around independently of the node.
		/// This class maintains the last known offset of the label with
		/// respect to the icon, so that this method can place the label
		/// correctly when <see cref="P:Northwoods.Go.GoIconicNode.DraggableLabel" /> is false or the
		/// <paramref name="childchanged" /> is not the label.
		/// When <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true, this method does nothing.
		/// This method also does nothing if there is no <see cref="P:Northwoods.Go.GoIconicNode.Icon" />.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoObject icon = Icon;
			if (icon == null)
			{
				return;
			}
			GoText label = Label;
			if (label != null)
			{
				if (DraggableLabel && childchanged == label)
				{
					myLabelOffset = new SizeF(label.Left - icon.Left, label.Top - icon.Top);
					return;
				}
				if (myLabelOffset.Width > -99999f)
				{
					label.Position = new PointF(icon.Left + myLabelOffset.Width, icon.Top + myLabelOffset.Height);
				}
				else
				{
					label.SetSpotLocation(32, icon, 128);
				}
			}
			if (Port != null)
			{
				Port.SetSpotLocation(1, icon, 1);
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
			if (child == Icon)
			{
				return "Icon";
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
			if (name == "Icon")
			{
				return Icon;
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
			case 2651:
				DraggableLabel = (bool)e.GetValue(undo);
				break;
			case 2652:
				Icon = (GoObject)e.GetValue(undo);
				break;
			case 2653:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2654:
				Port = (GoPort)e.GetValue(undo);
				break;
			case 2655:
				LabelOffset = e.GetSize(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
