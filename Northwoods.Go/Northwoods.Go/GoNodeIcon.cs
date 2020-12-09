using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A <see cref="T:Northwoods.Go.GoImage" /> whose <see cref="P:Northwoods.Go.GoObject.Bounds" /> and resizing
	/// are constrained by its parent's implementation of <see cref="T:Northwoods.Go.IGoNodeIconConstraint" />.
	/// </summary>
	/// <remarks>
	/// This class also implements <see cref="T:Northwoods.Go.IGoNodeIconConstraint" /> by providing
	/// settable properties <see cref="P:Northwoods.Go.GoNodeIcon.MinimumIconSize" /> and <see cref="P:Northwoods.Go.GoNodeIcon.MaximumIconSize" />.
	/// </remarks>
	[Serializable]
	public class GoNodeIcon : GoImage, IGoNodeIconConstraint
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoNodeIcon.MinimumIconSize" /> property.
		/// </summary>
		public const int ChangedMinimumIconSize = 2050;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoNodeIcon.MaximumIconSize" /> property.
		/// </summary>
		public const int ChangedMaximumIconSize = 2051;

		private SizeF myMinimumIconSize = new SizeF(1f, 1f);

		private SizeF myMaximumIconSize = new SizeF(9999f, 9999f);

		/// <summary>
		/// Gets the object that implements the
		/// <see cref="P:Northwoods.Go.IGoNodeIconConstraint.MinimumIconSize" /> and
		/// <see cref="P:Northwoods.Go.IGoNodeIconConstraint.MaximumIconSize" /> methods.
		/// </summary>
		/// <remarks>
		/// This returns the <see cref="P:Northwoods.Go.GoObject.Parent" /> if it implements
		/// <see cref="T:Northwoods.Go.IGoNodeIconConstraint" />; otherwise this returns itself.
		/// </remarks>
		public virtual IGoNodeIconConstraint Constraint
		{
			get
			{
				IGoNodeIconConstraint goNodeIconConstraint = base.Parent as IGoNodeIconConstraint;
				if (goNodeIconConstraint != null)
				{
					return goNodeIconConstraint;
				}
				return this;
			}
		}

		/// <summary>
		/// Gets or sets the minimum size for this image.
		/// </summary>
		/// <remarks>
		/// By default this imposes a minimum of 1x1 on the size.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The minimum size for the icon")]
		public virtual SizeF MinimumIconSize
		{
			get
			{
				return myMinimumIconSize;
			}
			set
			{
				SizeF sizeF = myMinimumIconSize;
				if (sizeF != value)
				{
					myMinimumIconSize = value;
					Changed(2050, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum size for this image.
		/// </summary>
		/// <remarks>
		/// By default this imposes a maximum of 9999x9999 on the size.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum size for the icon")]
		public virtual SizeF MaximumIconSize
		{
			get
			{
				return myMaximumIconSize;
			}
			set
			{
				SizeF sizeF = myMaximumIconSize;
				if (sizeF != value)
				{
					myMaximumIconSize = value;
					Changed(2051, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Force the size of this image to always be within the <see cref="P:Northwoods.Go.GoNodeIcon.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoNodeIcon.MaximumIconSize" />.
		/// </summary>
		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				IGoNodeIconConstraint constraint = Constraint;
				SizeF minimumIconSize = constraint.MinimumIconSize;
				SizeF maximumIconSize = constraint.MaximumIconSize;
				float width = value.Width;
				if (width < minimumIconSize.Width)
				{
					width = minimumIconSize.Width;
				}
				else if (width > maximumIconSize.Width)
				{
					width = maximumIconSize.Width;
				}
				float height = value.Height;
				if (height < minimumIconSize.Height)
				{
					height = minimumIconSize.Height;
				}
				else if (height > maximumIconSize.Height)
				{
					height = maximumIconSize.Height;
				}
				base.Bounds = new RectangleF(value.X, value.Y, width, height);
			}
		}

		/// <summary>
		/// Create an unselectable <see cref="T:Northwoods.Go.GoImage" /> whose size is
		/// constrained when you set the <see cref="P:Northwoods.Go.GoNodeIcon.Constraint" /> property.
		/// </summary>
		public GoNodeIcon()
		{
			base.InternalFlags &= -3;
		}

		/// <summary>
		/// Prevent user resizing of this image beyond the <see cref="P:Northwoods.Go.GoNodeIcon.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoNodeIcon.MaximumIconSize" />.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			IGoNodeIconConstraint constraint = Constraint;
			SizeF minimumIconSize = constraint.MinimumIconSize;
			SizeF maximumIconSize = constraint.MaximumIconSize;
			base.DoResize(view, origRect, newPoint, whichHandle, evttype, minimumIconSize, maximumIconSize);
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
			case 2050:
				MinimumIconSize = e.GetSize(undo);
				break;
			case 2051:
				MaximumIconSize = e.GetSize(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
