using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A node with a resizable icon, a label, and a port on each side.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Call the <c>Initialize</c> method to construct the standard parts of this kind of node.
	/// The ports are normally on the left side and on the right side of the icon,
	/// and the <see cref="P:Northwoods.Go.GoSimpleNode.Label" /> is on the bottom.
	/// If you set <see cref="P:Northwoods.Go.GoSimpleNode.Orientation" /> to <c>Orientation.Vertical</c>, the
	/// <see cref="P:Northwoods.Go.GoSimpleNode.InPort" /> will be on top and the <see cref="P:Northwoods.Go.GoSimpleNode.OutPort" /> will be on the bottom,
	/// and the <see cref="P:Northwoods.Go.GoSimpleNode.Label" /> will be on the right side.
	/// In order to remove either port you can set either <see cref="P:Northwoods.Go.GoSimpleNode.InPort" /> or
	/// <see cref="P:Northwoods.Go.GoSimpleNode.OutPort" /> to null, or you can call <see cref="M:Northwoods.Go.GoObject.Remove" /> on the port.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoSimpleNode.Icon" /> is normally a <see cref="T:Northwoods.Go.GoNodeIcon" />,
	/// but could be any kind of <see cref="T:Northwoods.Go.GoObject" />.
	/// When it is a <see cref="T:Northwoods.Go.GoImage" />, the <see cref="P:Northwoods.Go.GoSimpleNode.Image" /> property gives you
	/// convenient access to the icon's image properties.
	/// The image will normally be sized at <see cref="P:Northwoods.Go.GoSimpleNode.MinimumIconSize" />.
	/// </para>
	/// <para>
	/// When then first two arguments to <see cref="M:Northwoods.Go.GoSimpleNode.Initialize(System.Resources.ResourceManager,System.String,System.String)" />
	/// are null, the node is initialized to use a <see cref="T:Northwoods.Go.GoDrawing" /> instead of a
	/// <see cref="T:Northwoods.Go.GoImage" />.  After initialization you can set the <see cref="P:Northwoods.Go.GoSimpleNode.Figure" />
	/// property if you want to display one of the standard <see cref="T:Northwoods.Go.GoFigure" />s.
	/// </para>
	/// <para>
	/// You can also set the <see cref="P:Northwoods.Go.GoSimpleNode.Icon" /> to be any kind of <see cref="T:Northwoods.Go.GoObject" />,
	/// including other kinds of <see cref="T:Northwoods.Go.GoShape" />s.  The <see cref="P:Northwoods.Go.GoSimpleNode.Shape" /> property
	/// provides convenient access to the icon when it is a <see cref="T:Northwoods.Go.GoShape" />, to allow
	/// you to initialize its brush and/or pen.  If your icon is a <see cref="T:Northwoods.Go.GoGroup" />,
	/// it might be convenient to override <see cref="P:Northwoods.Go.GoSimpleNode.Shape" /> and perhaps <see cref="P:Northwoods.Go.GoSimpleNode.Figure" />
	/// to refer to the appropriate shape or drawing in the group.
	/// </para>
	/// <para>
	/// Remember that <see cref="T:Northwoods.Go.GoPort" /> also inherits from <see cref="T:Northwoods.Go.GoShape" />,
	/// so you can easily set the brush and/or pen, as well as setting the port's
	/// <see cref="P:Northwoods.Go.GoPort.Style" />.
	/// </para>
	/// <para>
	/// Setting the <see cref="P:Northwoods.Go.GoNode.Location" />, <see cref="P:Northwoods.Go.GoNode.Resizable" />,
	/// <see cref="P:Northwoods.Go.GoNode.Reshapable" /> and <see cref="P:Northwoods.Go.GoNode.Shadowed" />
	/// properties actually set the same properties on the
	/// <see cref="P:Northwoods.Go.GoSimpleNode.SelectionObject" />, which is the <see cref="P:Northwoods.Go.GoSimpleNode.Icon" />.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoSimpleNode : GoNode, IGoNodeIconConstraint
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.Text" /> property.
		/// </summary>
		public const int ChangedText = 2601;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.Icon" /> property.
		/// </summary>
		public const int ChangedIcon = 2602;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2603;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.InPort" /> property.
		/// </summary>
		public const int ChangedInPort = 2604;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.OutPort" /> property.
		/// </summary>
		public const int ChangedOutPort = 2605;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSimpleNode.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 2606;

		private string myText = "";

		private GoObject myIcon;

		private GoText myLabel;

		private GoPort myInPort;

		private GoPort myOutPort;

		private Orientation myOrientation;

		/// <summary>
		/// Assume the minimum node size is 20x20.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The minimum size for the icon")]
		public virtual SizeF MinimumIconSize
		{
			get
			{
				if (Orientation == Orientation.Horizontal)
				{
					float width = 20f;
					float num = 20f;
					GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
					if (goNodeIcon != null)
					{
						width = goNodeIcon.MinimumIconSize.Width;
						num = goNodeIcon.MinimumIconSize.Height;
					}
					if (InPort != null)
					{
						num = Math.Max(num, InPort.Height);
					}
					if (OutPort != null)
					{
						num = Math.Max(num, OutPort.Height);
					}
					return new SizeF(width, num);
				}
				float num2 = 20f;
				float height = 20f;
				GoNodeIcon goNodeIcon2 = Icon as GoNodeIcon;
				if (goNodeIcon2 != null)
				{
					num2 = goNodeIcon2.MinimumIconSize.Width;
					height = goNodeIcon2.MinimumIconSize.Height;
				}
				if (InPort != null)
				{
					num2 = Math.Max(num2, InPort.Width);
				}
				if (OutPort != null)
				{
					num2 = Math.Max(num2, OutPort.Width);
				}
				return new SizeF(num2, height);
			}
			set
			{
				GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
				if (goNodeIcon != null)
				{
					goNodeIcon.MinimumIconSize = value;
				}
			}
		}

		/// <summary>
		/// Assume the maximum node size is 100x200.
		/// </summary>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum size for the icon")]
		public virtual SizeF MaximumIconSize
		{
			get
			{
				return (Icon as GoNodeIcon)?.MaximumIconSize ?? new SizeF(100f, 200f);
			}
			set
			{
				GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
				if (goNodeIcon != null)
				{
					goNodeIcon.MaximumIconSize = value;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoObject" /> acting as the icon for this node.
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoSimpleNode.CreateIcon(System.Resources.ResourceManager,System.String)" /> method(s)
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
					Changed(2602, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
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
		/// as it would be if you call <see cref="M:Northwoods.Go.GoSimpleNode.Initialize(System.Resources.ResourceManager,System.String,System.String)" />
		/// with a null String icon name.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape => Icon as GoShape;

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDrawing" />.<see cref="P:Northwoods.Go.GoDrawing.Figure" />
		/// of the <see cref="P:Northwoods.Go.GoSimpleNode.Shape" /> if the icon is a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		/// <value>
		/// This returns <see cref="T:Northwoods.Go.GoFigure" />.<see cref="F:Northwoods.Go.GoFigure.None" />
		/// in the typical case where the <see cref="P:Northwoods.Go.GoSimpleNode.Icon" /> is a <see cref="T:Northwoods.Go.GoImage" />.
		/// Setting this property has no effect if the <see cref="P:Northwoods.Go.GoSimpleNode.Shape" /> is not a <see cref="T:Northwoods.Go.GoDrawing" />.
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
		/// When this node's <see cref="P:Northwoods.Go.GoSimpleNode.Text" /> string changes,
		/// be sure to update the label's text.
		/// </summary>
		/// <remarks>
		/// This has a Text string that is normally the same as the Label's text string,
		/// but the Label might not exist or might show a different string.
		/// </remarks>
		public override string Text
		{
			get
			{
				return myText;
			}
			set
			{
				string text = myText;
				if (text != value)
				{
					myText = value;
					Changed(2601, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && Label != null)
					{
						Label.Text = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoText" /> label.
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoSimpleNode.CreateLabel(System.String)" /> method
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
						goText.RemoveObserver(this);
					}
					myLabel = value;
					if (value != null)
					{
						Add(value);
						value.AddObserver(this);
					}
					Changed(2603, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the port on the left side of this node, normally acting as the "input".
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoSimpleNode.CreatePort(System.Boolean)" /> method
		/// if you want to create a different kind of <see cref="T:Northwoods.Go.GoPort" /> when
		/// constructing this kind of node.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort InPort
		{
			get
			{
				return myInPort;
			}
			set
			{
				GoPort goPort = myInPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myInPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2604, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the port on the right side of this node, normally acting as the "output".
		/// </summary>
		/// <remarks>
		/// You might want to override the <see cref="M:Northwoods.Go.GoSimpleNode.CreatePort(System.Boolean)" /> method
		/// if you want to create a different kind of <see cref="T:Northwoods.Go.GoPort" /> when
		/// constructing this kind of node.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort OutPort
		{
			get
			{
				return myOutPort;
			}
			set
			{
				GoPort goPort = myOutPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myOutPort = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2605, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the general orientation of the node
		/// </summary>
		/// <value>
		/// This defaults to <c>Orientation.Horizontal</c>
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Horizontal)]
		[Description("The general orientation of the node and how links connect to it")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				Orientation orientation = myOrientation;
				if (orientation != value)
				{
					myOrientation = value;
					Changed(2606, (int)orientation, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						OnOrientationChanged(orientation);
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
		/// Initialize an empty GoSimpleNode to have an icon, a label, and two ports.
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
		/// you can set <see cref="P:Northwoods.Go.GoSimpleNode.Image" />.<see cref="P:Northwoods.Go.GoImage.Name" /> to show or change
		/// the image displayed by the <see cref="T:Northwoods.Go.GoImage" /> that is the <see cref="P:Northwoods.Go.GoSimpleNode.Image" />.
		/// </para>
		/// <para>
		/// If the value is null, the <see cref="P:Northwoods.Go.GoSimpleNode.Icon" /> is not a <see cref="T:Northwoods.Go.GoImage" />
		/// but a <see cref="T:Northwoods.Go.GoDrawing" />; you can then set the <see cref="P:Northwoods.Go.GoSimpleNode.Figure" />
		/// to change the shape shown as the icon.
		/// </para>
		/// </param>
		/// <param name="name">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoSimpleNode.Label" />.
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
		/// Initialize an empty GoSimpleNode to have an icon, a label, and two ports.
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
		/// The initial string value for the <see cref="P:Northwoods.Go.GoSimpleNode.Label" />.
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
			myText = name;
			myLabel = CreateLabel(name);
			Add(myLabel);
			if (myLabel != null)
			{
				myLabel.AddObserver(this);
			}
			myInPort = CreatePort(input: true);
			Add(myInPort);
			myOutPort = CreatePort(input: false);
			Add(myOutPort);
			base.PropertiesDelegatedToSelectionObject = true;
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoImage" /> or a <see cref="T:Northwoods.Go.GoDrawing" /> to act as the node's icon.
		/// </summary>
		/// <param name="res"></param>
		/// <param name="iconname">
		/// a null value causes no <see cref="T:Northwoods.Go.GoNodeIcon" /> to be allocated,
		/// but instead a <see cref="T:Northwoods.Go.GoDrawing" /> initialized to look like a rectangle.
		/// </param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoNodeIcon" /> that obeys this node's <see cref="P:Northwoods.Go.GoSimpleNode.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoSimpleNode.MaximumIconSize" /> properties
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(ResourceManager res, String iconname) {
		///    if (iconname != null) {
		///      GoNodeIcon ni = new GoNodeIcon();
		///      if (res != null)
		///        ni.ResourceManager = res;
		///      ni.Name = iconname;
		///      ni.MinimumIconSize = new SizeF(20, 20);
		///      ni.MaximumIconSize = new SizeF(100, 200);
		///      ni.Size = ni.MinimumIconSize;
		///      return ni;
		///    } else {
		///      GoDrawing rect = new GoDrawing(GoFigure.Rectangle);
		///      rect.Selectable = false;
		///      rect.Resizable = false;
		///      rect.Size = new SizeF(20, 20);
		///      return rect;
		///    }
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ResourceManager res, string iconname)
		{
			if (iconname != null)
			{
				GoNodeIcon goNodeIcon = new GoNodeIcon();
				if (res != null)
				{
					goNodeIcon.ResourceManager = res;
				}
				goNodeIcon.Name = iconname;
				goNodeIcon.MinimumIconSize = new SizeF(20f, 20f);
				goNodeIcon.MaximumIconSize = new SizeF(100f, 200f);
				goNodeIcon.Size = goNodeIcon.MinimumIconSize;
				return goNodeIcon;
			}
			return new GoDrawing(GoFigure.Rectangle)
			{
				Selectable = false,
				Resizable = false,
				Size = new SizeF(20f, 20f)
			};
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoImage" /> to act as the node's icon.
		/// </summary>
		/// <param name="imglist"></param>
		/// <param name="imgindex"></param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoNodeIcon" /> that obeys this node's <see cref="P:Northwoods.Go.GoSimpleNode.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoSimpleNode.MaximumIconSize" /> properties
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(System.Windows.Forms.ImageList imglist, int imgindex) {
		///    GoNodeIcon ni = new GoNodeIcon();
		///    ni.ImageList = imglist;
		///    ni.Index = imgindex;
		///    ni.MinimumIconSize = new SizeF(20, 20);
		///    ni.MaximumIconSize = new SizeF(100, 200);
		///    ni.Size = ni.MinimumIconSize;
		///    return ni;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ImageList imglist, int imgindex)
		{
			GoNodeIcon obj = new GoNodeIcon
			{
				ImageList = imglist,
				Index = imgindex,
				MinimumIconSize = new SizeF(20f, 20f),
				MaximumIconSize = new SizeF(100f, 200f)
			};
			obj.Size = obj.MinimumIconSize;
			return obj;
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
		///      l.Selectable = false;
		///      l.Alignment = Middle;
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
				goText.Selectable = false;
				goText.Alignment = 1;
			}
			return goText;
		}

		/// <summary>
		/// Create and initialize a port to be either on the left or on the right of this node.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>a <see cref="T:Northwoods.Go.GoPort" /></returns>
		/// <remarks>
		/// You may want to override this to return null when you don't want a port
		/// on a particular side.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoPort CreatePort(bool input) {
		///    GoPort p = new GoPort();
		///    p.Size = new SizeF(6, 6);
		///    p.IsValidFrom = !input;
		///    p.IsValidTo = input;
		///    return p;
		///  }
		/// </code>
		/// </example>
		protected virtual GoPort CreatePort(bool input)
		{
			return new GoPort
			{
				Size = new SizeF(6f, 6f),
				IsValidFrom = !input,
				IsValidTo = input
			};
		}

		/// <summary>
		/// Make copies of the icon, label and two ports.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		/// <remarks>
		/// Remember to override this to copy any objects you add to this class.
		/// </remarks>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoSimpleNode obj = (GoSimpleNode)newgroup;
			obj.myIcon = (GoObject)env[myIcon];
			obj.myLabel = (GoText)env[myLabel];
			obj.myInPort = (GoPort)env[myInPort];
			obj.myOutPort = (GoPort)env[myOutPort];
		}

		/// <summary>
		/// Position the label and two ports relative to the icon.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true, this method does nothing.
		/// This method also does nothing if there is no <see cref="P:Northwoods.Go.GoSimpleNode.Icon" />.
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
			if (Orientation == Orientation.Horizontal)
			{
				if (Label != null)
				{
					Label.SetSpotLocation(32, icon, 128);
				}
				if (InPort != null)
				{
					InPort.SetSpotLocation(64, icon, 256);
				}
				if (OutPort != null)
				{
					OutPort.SetSpotLocation(256, icon, 64);
				}
			}
			else
			{
				if (Label != null)
				{
					Label.SetSpotLocation(256, icon, 64);
				}
				if (InPort != null)
				{
					InPort.SetSpotLocation(128, icon, 32);
				}
				if (OutPort != null)
				{
					OutPort.SetSpotLocation(32, icon, 128);
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
			if (obj == myIcon)
			{
				myIcon = null;
				return result;
			}
			if (obj == myLabel)
			{
				myLabel.RemoveObserver(this);
				myLabel = null;
				return result;
			}
			if (obj == myInPort)
			{
				myInPort = null;
				return result;
			}
			if (obj == myOutPort)
			{
				myOutPort = null;
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
			if (child == Icon)
			{
				return "Icon";
			}
			if (child == Label)
			{
				return "Label";
			}
			if (child == InPort)
			{
				return "InPort";
			}
			if (child == OutPort)
			{
				return "OutPort";
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
			if (name == "InPort")
			{
				return InPort;
			}
			if (name == "OutPort")
			{
				return OutPort;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// Determine how to change the whole node when the <see cref="P:Northwoods.Go.GoSimpleNode.Orientation" /> changes.
		/// </summary>
		/// <param name="old">the former <see cref="P:Northwoods.Go.GoSimpleNode.Orientation" /> value</param>
		/// <remarks>
		/// A horizontal orientation has the ports positioned on the left and right of the
		/// node, with the source links coming into the <see cref="P:Northwoods.Go.GoSimpleNode.InPort" /> on the left and
		/// the destination links going out from the <see cref="P:Northwoods.Go.GoSimpleNode.OutPort" /> on the right.
		/// A vertical orientation has the <see cref="P:Northwoods.Go.GoSimpleNode.InPort" /> on top, so links come in
		/// at the top, and the <see cref="P:Northwoods.Go.GoSimpleNode.OutPort" /> is on the bottom of the node, so links
		/// go out from the bottom.
		/// </remarks>
		public virtual void OnOrientationChanged(Orientation old)
		{
			if (Orientation == Orientation.Vertical)
			{
				if (InPort != null)
				{
					InPort.ToSpot = 32;
					InPort.FromSpot = 128;
				}
				if (OutPort != null)
				{
					OutPort.ToSpot = 32;
					OutPort.FromSpot = 128;
				}
			}
			else
			{
				if (InPort != null)
				{
					InPort.ToSpot = 256;
					InPort.FromSpot = 64;
				}
				if (OutPort != null)
				{
					OutPort.ToSpot = 256;
					OutPort.FromSpot = 64;
				}
			}
			LayoutChildren(null);
		}

		/// <summary>
		/// When the label changes its text string, change this node's <see cref="P:Northwoods.Go.GoSimpleNode.Text" /> property too.
		/// </summary>
		/// <param name="observed"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		protected override void OnObservedChanged(GoObject observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			base.OnObservedChanged(observed, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			if (subhint == 1501 && observed == Label)
			{
				Text = (string)newVal;
			}
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
			case 2601:
				Text = (string)e.GetValue(undo);
				break;
			case 2602:
				Icon = (GoObject)e.GetValue(undo);
				break;
			case 2603:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2604:
				InPort = (GoPort)e.GetValue(undo);
				break;
			case 2605:
				OutPort = (GoPort)e.GetValue(undo);
				break;
			case 2606:
				Orientation = (Orientation)e.GetInt(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
