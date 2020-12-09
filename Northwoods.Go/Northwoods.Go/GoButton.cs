using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A GoButton looks like a regular button, with a text label and/or an icon,
	/// but is composed of GoObjects so that it is very light weight and flexible.
	/// </summary>
	/// <remarks>
	/// The <see cref="P:Northwoods.Go.GoButton.Background" /> is normally a <see cref="T:Northwoods.Go.GoRectangle" />,
	/// and the <see cref="P:Northwoods.Go.GoButton.Icon" /> is normally null/nothing.  However both properties
	/// can be any kind of <see cref="T:Northwoods.Go.GoObject" />.
	/// </remarks>
	[Serializable]
	public class GoButton : GoGroup, IGoLabeledNode, IGoLabeledPart, IGoActionObject
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.Background" /> property.
		/// </summary>
		public const int ChangedBackground = 2901;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.Icon" /> property.
		/// </summary>
		public const int ChangedIcon = 2902;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2903;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 2904;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 2905;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.ActionEnabled" /> property.
		/// </summary>
		public const int ChangedActionEnabled = 2906;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoButton.AutoRepeating" /> property.
		/// </summary>
		public const int ChangedAutoRepeating = 2907;

		private const int flagActionEnabled = 16777216;

		private const int flagActionActivated = 33554432;

		private const int flagAutoRepeating = 67108864;

		private GoObject myBack;

		private GoObject myIcon;

		private GoText myLabel;

		private SizeF myTopLeftMargin = new SizeF(3f, 2f);

		private SizeF myBottomRightMargin = new SizeF(2f, 3f);

		[NonSerialized]
		private GoInputEventHandler myActionEvent;

		/// <summary>
		/// Gets or sets the background object for this button.
		/// </summary>
		/// <remarks>
		/// Instead of setting the background shape after creating a <c>GoButton</c>, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoButton.CreateBackground" /> method.
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
				if (goObject != value)
				{
					if (goObject != null)
					{
						Remove(goObject);
					}
					myBack = value;
					if (value != null)
					{
						value.Selectable = false;
						InsertBefore(null, value);
					}
					Changed(2901, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the icon object for this button.
		/// </summary>
		/// <remarks>
		/// Instead of setting the icon after allocation, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoButton.CreateIcon" /> method.
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
					if (goObject != null)
					{
						Remove(goObject);
					}
					myIcon = value;
					if (value != null)
					{
						value.Selectable = false;
						Add(value);
					}
					Changed(2902, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text string for this button.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The text string for the button")]
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
		/// Gets or sets the text label for this button.
		/// </summary>
		/// <remarks>
		/// Instead of setting the label after allocation, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoButton.CreateLabel" /> method.
		/// </remarks>
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
						value.Selectable = false;
						Add(value);
					}
					Changed(2903, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the amount of space to leave at the top and left sides between
		/// the icon and label and the edge of the background.
		/// </summary>
		/// <value>
		/// This defaults to 3x2, in document coordinates.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the icon and label inside the background at the left side and the top")]
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
					Changed(2904, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the amount of space to leave at the bottom and right sides between
		/// the icon and label and the edge of the background.
		/// </summary>
		/// <value>
		/// This defaults to 2x3, in document coordinates.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the icon and label inside the background at the right side and the bottom")]
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
					Changed(2905, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the button calls <see cref="M:Northwoods.Go.GoButton.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" /> repeatedly
		/// when the mouse remains down, after a delay.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the button repeatedly adjusts itself even though the mouse doesn't move.")]
		public virtual bool AutoRepeating
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				if ((base.InternalFlags & 0x4000000) != 0 != value)
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
		}

		/// <summary>
		/// Gets or sets whether the user can click on this button to get it to perform an action.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can click on this button to perform an action")]
		public virtual bool ActionEnabled
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
					Changed(2906, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the button appears "depressed".
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// A change to this state is considered transient, and does not
		/// invoke <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the button appears depressed")]
		public virtual bool ActionActivated
		{
			get
			{
				return (base.InternalFlags & 0x2000000) != 0;
			}
			set
			{
				if ((base.InternalFlags & 0x2000000) != 0 != value)
				{
					if (value)
					{
						base.InternalFlags |= 33554432;
					}
					else
					{
						base.InternalFlags &= -33554433;
					}
					InvalidateViews();
				}
			}
		}

		/// <summary>
		/// An Action event happens whenever the user clicks on this button.
		/// </summary>
		public event GoInputEventHandler Action
		{
			add
			{
				myActionEvent = (GoInputEventHandler)Delegate.Combine(myActionEvent, value);
			}
			remove
			{
				myActionEvent = (GoInputEventHandler)Delegate.Remove(myActionEvent, value);
			}
		}

		/// <summary>
		/// Construct a <see cref="T:Northwoods.Go.GoButton" /> with an empty text label and no icon.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoButton.CreateBackground" />, <see cref="M:Northwoods.Go.GoButton.CreateIcon" />, and
		/// <see cref="M:Northwoods.Go.GoButton.CreateLabel" /> to provide initial values for this group's
		/// <see cref="P:Northwoods.Go.GoButton.Background" />, <see cref="P:Northwoods.Go.GoButton.Icon" />, and <see cref="P:Northwoods.Go.GoButton.Label" />
		/// properties.
		/// </remarks>
		public GoButton()
		{
			base.InternalFlags &= -17;
			base.InternalFlags |= 16777216;
			myBack = CreateBackground();
			Add(myBack);
			myIcon = CreateIcon();
			Add(myIcon);
			myLabel = CreateLabel();
			Add(myLabel);
		}

		/// <summary>
		/// Make the background rectangle for the button.
		/// </summary>
		/// <returns>By default this returns a <see cref="T:Northwoods.Go.GoRectangle" /></returns>
		/// <remarks>
		/// This is called by the constructor.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateBackground() {
		///    GoRectangle r = new GoRectangle();
		///    r.Selectable = false;
		///    r.Pen = null;
		///    r.Brush = GoShape.SystemBrushes_Control;
		///    return r;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateBackground()
		{
			return new GoRectangle
			{
				Selectable = false,
				Pen = null,
				Brush = GoShape.SystemBrushes_Control
			};
		}

		/// <summary>
		/// Make the icon for the button.
		/// </summary>
		/// <returns>By default this returns null</returns>
		/// <remarks>
		/// This is called by the constructor.
		/// You can override this to return a newly allocated <see cref="T:Northwoods.Go.GoImage" />,
		/// or some other object to serve as the decoration for the button.
		/// You can also set the <see cref="P:Northwoods.Go.GoButton.Icon" /> property at any time.
		/// </remarks>
		protected virtual GoObject CreateIcon()
		{
			return null;
		}

		/// <summary>
		/// Make the text label for the button.
		/// </summary>
		/// <returns>By default this returns an empty <see cref="T:Northwoods.Go.GoText" /> object</returns>
		/// <remarks>
		/// This is called by the constructor.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel() {
		///    GoText l = new GoText();
		///    l.Selectable = false;
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel()
		{
			return new GoText
			{
				Selectable = false
			};
		}

		/// <summary>
		/// Copy the <see cref="P:Northwoods.Go.GoButton.Background" />, <see cref="P:Northwoods.Go.GoButton.Label" />, and <see cref="P:Northwoods.Go.GoButton.Icon" />.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			base.CopyChildren(newgroup, env);
			GoButton obj = (GoButton)newgroup;
			obj.InternalFlags = (base.InternalFlags & -33554433);
			obj.myBack = (GoObject)env[myBack];
			obj.myIcon = (GoObject)env[myIcon];
			obj.myLabel = (GoText)env[myLabel];
			obj.myActionEvent = null;
		}

		/// <summary>
		/// Position the <see cref="P:Northwoods.Go.GoButton.Icon" /> to the left of the <see cref="P:Northwoods.Go.GoButton.Label" />,
		/// and surround both with the <see cref="P:Northwoods.Go.GoButton.Background" />, leaving
		/// <see cref="P:Northwoods.Go.GoButton.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoButton.BottomRightMargin" /> space
		/// along the sides.
		/// </summary>
		/// <param name="childchanged"></param>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoObject background = Background;
			GoText label = Label;
			GoObject icon = Icon;
			if (icon != null && label != null)
			{
				icon.SetSpotLocation(64, label, 256);
			}
			if (background == null)
			{
				return;
			}
			RectangleF bounds = Bounds;
			if (label != null)
			{
				bounds = label.Bounds;
			}
			else
			{
				if (icon == null)
				{
					return;
				}
				bounds = icon.Bounds;
			}
			if (icon != null && label != null)
			{
				bounds.X -= icon.Width;
				bounds.Width += icon.Width;
				if (icon.Height > label.Height)
				{
					bounds.Y -= (icon.Height - label.Height) / 2f;
					bounds.Height = icon.Height;
				}
			}
			SizeF topLeftMargin = TopLeftMargin;
			SizeF bottomRightMargin = BottomRightMargin;
			bounds.X -= topLeftMargin.Width;
			bounds.Width += topLeftMargin.Width + bottomRightMargin.Width;
			bounds.Y -= topLeftMargin.Height;
			bounds.Height += topLeftMargin.Height + bottomRightMargin.Height;
			background.Bounds = bounds;
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
				return result;
			}
			if (obj == myIcon)
			{
				myIcon = null;
			}
			return result;
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoButton.PaintButton(System.Drawing.Graphics,Northwoods.Go.GoView)" /> in addition to painting the
		/// <see cref="P:Northwoods.Go.GoButton.Background" />, <see cref="P:Northwoods.Go.GoButton.Label" />, and <see cref="P:Northwoods.Go.GoButton.Icon" />.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			base.Paint(g, view);
			PaintButton(g, view);
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoButton.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> to display any button state,
		/// such as whether it is "pressed".
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		protected virtual void PaintButton(Graphics g, GoView view)
		{
			RectangleF bounds = Bounds;
			Pen pen;
			Pen pen2;
			Pen pen3;
			Pen pen4;
			if (ActionActivated)
			{
				pen = GoShape.SystemPens_ControlDarkDark;
				pen2 = GoShape.SystemPens_ControlLightLight;
				pen3 = GoShape.SystemPens_ControlDark;
				pen4 = GoShape.SystemPens_Control;
			}
			else
			{
				pen = GoShape.SystemPens_ControlLightLight;
				pen2 = GoShape.SystemPens_ControlDarkDark;
				pen3 = GoShape.SystemPens_Control;
				pen4 = GoShape.SystemPens_ControlDark;
			}
			PointF[] array = view.AllocTempPointArray(3);
			float num = 0.5f;
			float num2 = 0.5f;
			if (base.Document != null)
			{
				num /= base.Document.WorldScale.Width;
				num2 /= base.Document.WorldScale.Height;
			}
			PointF pointF = new PointF(bounds.X + num, bounds.Y + num2);
			PointF pointF2 = new PointF(bounds.X + bounds.Width - num, bounds.Y + num2);
			PointF pointF3 = new PointF(bounds.X + num, bounds.Y + bounds.Height - num2);
			PointF pointF4 = new PointF(bounds.X + bounds.Width - num, bounds.Y + bounds.Height - num2);
			array[0] = pointF2;
			array[1] = pointF;
			array[2] = pointF3;
			GoShape.DrawLines(g, view, pen3, array);
			array[0].Y -= num2;
			array[1] = pointF4;
			array[2].X -= num;
			GoShape.DrawLines(g, view, pen4, array);
			pointF.X -= num * 2f;
			pointF.Y -= num2 * 2f;
			pointF2.X += num * 2f;
			pointF2.Y -= num2 * 2f;
			pointF4.X += num * 2f;
			pointF4.Y += num2 * 2f;
			pointF3.X -= num * 2f;
			pointF3.Y += num2 * 2f;
			array[0] = pointF2;
			array[1] = pointF;
			array[2] = pointF3;
			GoShape.DrawLines(g, view, pen, array);
			array[0].Y -= num2;
			array[1] = pointF4;
			array[2].X -= num;
			GoShape.DrawLines(g, view, pen2, array);
			view.FreeTempPointArray(array);
		}

		/// <summary>
		/// Additional painted area includes part of the border giving 3D control appearance.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			GoObject.InflateRect(ref rect, 2f, 2f);
			return rect;
		}

		/// <summary>
		/// The properties referring to parts of this node
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
			if (child == Icon)
			{
				return "Icon";
			}
			if (child == Label)
			{
				return "Label";
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
			if (name == "Background")
			{
				return Background;
			}
			if (name == "Icon")
			{
				return Icon;
			}
			if (name == "Label")
			{
				return Label;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// If <see cref="P:Northwoods.Go.GoButton.AutoRepeating" /> is true, have the <see cref="T:Northwoods.Go.GoToolAction" />
		/// tool start calling <see cref="M:Northwoods.Go.GoButton.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" /> repeatedly while
		/// the mouse is down.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="e"></param>
		public virtual void OnActionActivated(GoView view, GoInputEventArgs e)
		{
			if (AutoRepeating && view != null)
			{
				(view.Tool as GoToolAction)?.StartAutoAdjusting();
			}
		}

		/// <summary>
		/// We don't care about any mouse movement over a button.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="e"></param>
		public virtual void OnActionAdjusted(GoView view, GoInputEventArgs e)
		{
		}

		/// <summary>
		/// Call all of the Action event handlers.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that is handling input events for this <see cref="T:Northwoods.Go.IGoActionObject" /></param>
		/// <param name="e">assumed to be the same as the <paramref name="view" />'s <see cref="P:Northwoods.Go.GoView.LastInput" /></param>
		/// <remarks>
		/// This method is called when the user does a mouse press and release
		/// on this button.
		/// If the mouse point is no longer over this object, no Action
		/// event handlers are called, and <see cref="M:Northwoods.Go.GoButton.OnActionCancelled(Northwoods.Go.GoView)" />
		/// is called instead.
		/// </remarks>
		public virtual void OnAction(GoView view, GoInputEventArgs e)
		{
			if (view == null)
			{
				return;
			}
			GoToolAction goToolAction = view.Tool as GoToolAction;
			IGoActionObject goActionObject = this;
			if (goToolAction != null)
			{
				goActionObject = goToolAction.PickActionObject();
			}
			if (goActionObject == this)
			{
				if (myActionEvent != null)
				{
					myActionEvent(this, e);
				}
			}
			else
			{
				OnActionCancelled(view);
			}
		}

		/// <summary>
		/// We don't care if pressing the button was cancelled.
		/// </summary>
		/// <param name="view"></param>
		public virtual void OnActionCancelled(GoView view)
		{
		}

		/// <summary>
		/// Handle undo and redo changes.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2901:
				Background = (GoObject)e.GetValue(undo);
				break;
			case 2902:
				Icon = (GoObject)e.GetValue(undo);
				break;
			case 2903:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2904:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 2905:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 2906:
				ActionEnabled = (bool)e.GetValue(undo);
				break;
			case 2907:
				AutoRepeating = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
