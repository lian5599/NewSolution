using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A simple group displaying text with a shadowed background.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoComment.Background" /> is normally a <see cref="T:Northwoods.Go.GoRectangle" />,
	/// but could be any kind of <see cref="T:Northwoods.Go.GoObject" />.  Hence you can easily
	/// customize that rectangle's appearance by setting properties on the
	/// <see cref="P:Northwoods.Go.GoComment.Shape" />, such as:
	/// <pre><code>
	///   comment.Shape.BrushColor = Color.LightSkyBlue
	///   comment.Shape.PenColor = Color.DeepSkyBlue
	/// </code></pre>
	/// Or you could replace the <see cref="P:Northwoods.Go.GoComment.Background" /> with a different <see cref="T:Northwoods.Go.GoShape" />.
	/// </para>
	/// <para>
	/// <see cref="M:Northwoods.Go.GoComment.LayoutChildren(Northwoods.Go.GoObject)" /> sizes the <see cref="P:Northwoods.Go.GoComment.Background" /> to fit around
	/// the <see cref="P:Northwoods.Go.GoComment.Label" />, using the <see cref="P:Northwoods.Go.GoComment.TopLeftMargin" /> and the
	/// <see cref="P:Northwoods.Go.GoComment.BottomRightMargin" /> to leave space between the <see cref="P:Northwoods.Go.GoComment.Label" />
	/// and the bounds of the <see cref="P:Northwoods.Go.GoComment.Background" /> shape.  If you use other shapes,
	/// you may need to increase these margins.
	/// </para>
	/// <para>
	/// Because this class has a <see cref="P:Northwoods.Go.GoComment.Label" />, it implements the
	/// <see cref="T:Northwoods.Go.IGoLabeledNode" /> interface, even though this class is not a "node".
	/// It also implements <see cref="T:Northwoods.Go.IGoIdentifiablePart" />, to make it easier to keep
	/// track of the different comments that might be in your diagram, if you have set
	/// <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> to true in your <see cref="T:Northwoods.Go.GoDocument" />.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoComment : GoGroup, IGoLabeledNode, IGoLabeledPart, IGoIdentifiablePart
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoComment.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 2301;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoComment.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 2302;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoComment.PartID" /> property.
		/// </summary>
		public const int ChangedPartID = 2303;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoComment.Background" /> property.
		/// </summary>
		public const int ChangedBackground = 2304;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoComment.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2305;

		private GoText myLabel;

		private GoObject myBack;

		private SizeF myTopLeftMargin = new SizeF(4f, 2f);

		private SizeF myBottomRightMargin = new SizeF(4f, 2f);

		private int myPartID = -1;

		/// <summary>
		/// Giving this comment a shadow really means giving the background a shadow.
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
		/// <see cref="M:Northwoods.Go.GoComment.CreateBackground" />, you will probably want to set this margin
		/// and <see cref="P:Northwoods.Go.GoComment.BottomRightMargin" /> appropriately so that the text fits.
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
					Changed(2301, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
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
		/// <see cref="M:Northwoods.Go.GoComment.CreateBackground" />, you will probably want to set this margin
		/// and <see cref="P:Northwoods.Go.GoComment.TopLeftMargin" /> appropriately so that the text fits.
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
					Changed(2302, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the label for this group, implementing <see cref="T:Northwoods.Go.IGoLabeledNode" />.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoText Label
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
					Changed(2305, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text string for this group, implementing <see cref="T:Northwoods.Go.IGoLabeledNode" />.
		/// </summary>
		public virtual string Text
		{
			get
			{
				return Label.Text;
			}
			set
			{
				Label.Text = value;
			}
		}

		/// <summary>
		/// Gets the background object for this comment.
		/// </summary>
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
				Changed(2304, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Gets this comment's background shape, assuming it is a <see cref="T:Northwoods.Go.GoShape" />, as it usually is.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape => Background as GoShape;

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
					Changed(2303, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Create a GoComment displaying an empty string.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoComment.CreateBackground" /> and <see cref="M:Northwoods.Go.GoComment.CreateLabel" />
		/// to provide initial values for <see cref="P:Northwoods.Go.GoComment.Background" /> and <see cref="P:Northwoods.Go.GoComment.Label" />.
		/// </remarks>
		public GoComment()
		{
			base.InternalFlags &= -17;
			myBack = CreateBackground();
			Add(myBack);
			myLabel = CreateLabel();
			Add(myLabel);
		}

		/// <summary>
		/// Create and initialize the background for the comment, which is normally a
		/// <see cref="T:Northwoods.Go.GoRectangle" /> with a light yellow <see cref="P:Northwoods.Go.GoShape.Brush" />
		/// and a drop-shadow effect.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateBackground() {
		///    GoRectangle r = new GoRectangle();
		///    r.Shadowed = true;
		///    r.Selectable = false;
		///    r.Pen = GoShape.Pens_LightGray;
		///    r.Brush = GoShape.Brushes_LemonChiffon;
		///    return r;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateBackground()
		{
			return new GoRectangle
			{
				Shadowed = true,
				Selectable = false,
				Pen = GoShape.Pens_LightGray,
				Brush = GoShape.Brushes_LemonChiffon
			};
		}

		/// <summary>
		/// Create and initialize the <see cref="T:Northwoods.Go.GoText" /> label for displaying the
		/// comment, which normally supports multiple lines and is editable.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel() {
		///    GoText l = new GoText();
		///    l.Selectable = false;
		///    l.Multiline = true;
		///    l.Editable = true;
		///    this.Editable = true;
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel()
		{
			GoText result = new GoText
			{
				Selectable = false,
				Multiline = true,
				Editable = true
			};
			Editable = true;
			return result;
		}

		/// <summary>
		/// Make copies of the background and the text object.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoComment obj = (GoComment)newgroup;
			obj.myPartID = -1;
			obj.myBack = (GoObject)env[myBack];
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
			if (obj == myLabel)
			{
				myLabel = null;
				return result;
			}
			if (obj == myBack)
			{
				myBack = null;
			}
			return result;
		}

		/// <summary>
		/// This method uses the <see cref="P:Northwoods.Go.GoComment.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoComment.BottomRightMargin" />
		/// properties to decide how much bigger the background should be than the text label.
		/// </summary>
		/// <param name="childchanged"></param>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoText label = Label;
			if (label != null)
			{
				GoObject background = Background;
				if (background != null)
				{
					SizeF topLeftMargin = TopLeftMargin;
					SizeF bottomRightMargin = BottomRightMargin;
					background.Bounds = new RectangleF(label.Left - topLeftMargin.Width, label.Top - topLeftMargin.Height, label.Width + topLeftMargin.Width + bottomRightMargin.Width, label.Height + topLeftMargin.Height + bottomRightMargin.Height);
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

		/// <summary>
		/// The properties referring to parts of this group
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override string FindName(GoObject child)
		{
			if (child == Background)
			{
				return "Background";
			}
			if (child == Label)
			{
				return "Label";
			}
			return base.FindName(child);
		}

		/// <summary>
		/// The properties referring to parts of this group
		/// are also the names of those parts.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override GoObject FindChild(string name)
		{
			if (name == "Background")
			{
				return Background;
			}
			if (name == "Label")
			{
				return Label;
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
			case 2301:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 2302:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 2303:
				PartID = e.GetInt(undo);
				break;
			case 2304:
				Background = (GoObject)e.GetValue(undo);
				break;
			case 2305:
				Label = (GoText)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
