using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This graphical object provides a way to get the appearance and behavior
	/// from an existing Windows Forms <c>Control</c>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <c>GoControl</c> is a <see cref="T:Northwoods.Go.GoObject" /> that manages a
	/// Windows Forms <c>Control</c> in a <see cref="T:Northwoods.Go.GoView" />.
	/// The <c>Control</c> is automatically created when the <see cref="T:Northwoods.Go.GoObject" />
	/// becomes visible in the view, and is added to the view's <c>Controls</c>
	/// collection.  By default the <c>Control</c> has the same size
	/// as the <c>GoControl</c>, and it moves around or changes size
	/// on the screen as the <c>GoControl</c> is scrolled around or resized.
	/// </para>
	/// <para>
	/// <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" /> is responsible for creating an instance
	/// of a <c>Control</c>.  By default it just creates an instance of the
	/// <see cref="P:Northwoods.Go.GoControl.ControlType" /> <c>Type</c>, and gives it the <see cref="T:Northwoods.Go.GoControl" />'s
	/// <see cref="P:Northwoods.Go.GoObject.Bounds" /> in view coordinates.
	/// </para>
	/// <para>
	/// This object keeps a hashtable mapping <see cref="T:Northwoods.Go.GoView" />'s to <c>Control</c>s.
	/// <see cref="M:Northwoods.Go.GoControl.FindControl(Northwoods.Go.GoView)" /> finds any <c>Control</c> for this <c>GoControl</c>
	/// in a given <see cref="T:Northwoods.Go.GoView" />.
	/// <see cref="M:Northwoods.Go.GoControl.GetControl(Northwoods.Go.GoView)" /> is responsible for making sure a <c>Control</c> exists;
	/// if <see cref="M:Northwoods.Go.GoControl.FindControl(Northwoods.Go.GoView)" /> returns null, it calls <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" />.
	/// </para>
	/// <para>
	/// The most common use of <c>GoControl</c>s is as in-place editors for a
	/// <see cref="T:Northwoods.Go.GoObject" />.  <see cref="T:Northwoods.Go.GoText" /> for example, uses customized
	/// <c>TextBox</c>, <c>ComboBox</c>, and <c>NumericUpDown</c> <c>Control</c>s
	/// to allow the user to edit text.  When used as an editor, a <c>GoControl</c>
	/// is added to a <see cref="T:Northwoods.Go.GoView" /> layer, not to a document.  The use of
	/// the <c>Control</c> managed by the <c>GoControl</c> is effectively modal.
	/// The <see cref="P:Northwoods.Go.GoControl.EditedObject" /> property is used when this <c>GoControl</c>
	/// is used as a <see cref="T:Northwoods.Go.GoObject" /> editor.  This <c>GoControl</c>
	/// becomes the value of <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" />.
	/// </para>
	/// <para>
	/// However, you can also use <c>GoControl</c>s in <see cref="T:Northwoods.Go.GoDocument" />s.
	/// When they are used as regular document objects, usually as part of a
	/// <see cref="T:Northwoods.Go.GoGroup" />, a <c>Control</c> is created for each <see cref="T:Northwoods.Go.GoView" />
	/// in which the <c>GoControl</c> can be seen.
	/// Please be aware of the restrictions that are imposed by Windows Controls.
	/// Because each <c>Control</c>, including <see cref="T:Northwoods.Go.GoView" />, is responsible
	/// for completely painting itself and handling all events that it receives,
	/// it is not possible to interleave the painting done by <see cref="T:Northwoods.Go.GoView" />
	/// and the painting done by <c>Control</c>s.
	/// All <c>Controls</c> that are parented by a <see cref="T:Northwoods.Go.GoView" /> must appear
	/// in front of the <see cref="T:Northwoods.Go.GoView" /> itself.
	/// Hence two <see cref="T:Northwoods.Go.GoGroup" />s that each contain a <see cref="T:Northwoods.Go.GoControl" />
	/// and that overlap each other will produce the odd effect of not having one
	/// group properly appear in front of the other one, because the <c>Control</c>s
	/// managed by the nested <see cref="T:Northwoods.Go.GoControl" />s are in front of everything
	/// that the <see cref="T:Northwoods.Go.GoView" /> paints.
	/// </para>
	/// <para>
	/// Besides having all <c>Control</c>s appear in front of all <see cref="T:Northwoods.Go.GoObject" />s,
	/// the use of <see cref="T:Northwoods.Go.GoControl" /> has additional limitations and difficulties of use.
	/// You will need to handle the initialization of the <c>Control</c>, given information
	/// that is present on or accessible from the <see cref="T:Northwoods.Go.GoControl" /> or its
	/// <see cref="P:Northwoods.Go.GoObject.Parent" /> or <see cref="P:Northwoods.Go.GoObject.ParentNode" />.
	/// This is often done in the <see cref="T:Northwoods.Go.IGoControlObject" />.<see cref="P:Northwoods.Go.IGoControlObject.GoControl" />
	/// setter, if you subclass the <c>Control</c> and implement the <see cref="T:Northwoods.Go.IGoControlObject" />
	/// interface.  Or you can do the initialization in a call from an override
	/// of <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" />.
	/// </para>
	/// <para>
	/// Another limitation is that your application will need to deal with focus
	/// issues, scaling, and printing, and synchronization.
	/// Printing, in particular, is not naturally supported by Windows Forms controls.
	/// And at the current time, scaling <c>Control</c>s is not performed in a good manner.
	/// And if you have a <c>GoControl</c> visible in two <see cref="T:Northwoods.Go.GoView" />s
	/// at the same time, you will need to deal with synchronization issues between
	/// the two <c>Control</c>s that are created for the single <c>GoControl</c> object.
	/// </para>
	/// <para>
	/// For simplicity, <see cref="M:Northwoods.Go.GoControl.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> does not call <see cref="M:Northwoods.Go.GoControl.GetControl(Northwoods.Go.GoView)" />
	/// to make sure a <c>Control</c> exists in a <see cref="T:Northwoods.Go.GoOverview" />.
	/// Instead it just paints a gray rectangle.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoControl : GoObject
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <c>ControlType</c> property.
		/// </summary>
		public const int ChangedControlType = 1901;

		private Type myControlType;

		private GoObject myEditedObject;

		[NonSerialized]
		private Dictionary<GoView, Control> myMap;

		/// <summary>
		/// Gets or sets the <c>Type</c> used to specify which <c>Control</c> to create.
		/// </summary>
		/// <value>
		/// The value must be a <c>Type</c> that is a subclass of <c>Control</c>.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" /> uses this value to create a <c>Control</c> for each
		/// <see cref="T:Northwoods.Go.GoView" /> that this GoControl is being displayed in.
		/// </remarks>
		[Description("The Type used to specify which Control to create when first displayed in a GoView.")]
		public virtual Type ControlType
		{
			get
			{
				return myControlType;
			}
			set
			{
				Type type = myControlType;
				if (type != value)
				{
					myControlType = value;
					Changed(1901, 0, type, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoObject" /> for which this control is acting as an editor.
		/// </summary>
		/// <remarks>
		/// This does not raise a Changed notification, since it should only be used for view objects.
		/// </remarks>
		[Description("The GoObject for which this control is acting as an editor.")]
		public virtual GoObject EditedObject
		{
			get
			{
				return myEditedObject;
			}
			set
			{
				myEditedObject = value;
			}
		}

		/// <summary>
		/// Gets the <c>Dictionary</c> that maps <see cref="T:Northwoods.Go.GoView" />s to <c>Control</c>s.
		/// </summary>
		/// <remarks>
		/// There may be zero, one, or many views that are displaying this GoControl.
		/// Each one needs to have its own instantiation of a <c>Control</c>.
		/// </remarks>
		[Description("The dictionary that maps GoViews to Controls for this GoControl.")]
		public Dictionary<GoView, Control> Map
		{
			get
			{
				if (myMap == null)
				{
					myMap = new Dictionary<GoView, Control>();
				}
				return myMap;
			}
		}

		/// <summary>
		/// Copying a GoControl does not copy any view to Control mappings.
		/// </summary>
		/// <param name="env"></param>
		/// <returns>The copied GoControl.</returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoControl obj = (GoControl)base.CopyObject(env);
			obj.myEditedObject = (GoObject)env[myEditedObject];
			obj.myMap = null;
			return obj;
		}

		/// <summary>
		/// This method just calls <see cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" /> on the
		/// <see cref="P:Northwoods.Go.GoControl.EditedObject" />.
		/// </summary>
		/// <param name="view"></param>
		public override void DoEndEdit(GoView view)
		{
			EditedObject?.DoEndEdit(view);
		}

		/// <summary>
		/// When a GoControl is added to a document, we need to add corresponding
		/// Controls to all of its views; when it is removed from a document, we
		/// need to remove all of its Controls in all of its views.
		/// </summary>
		/// <param name="oldlayer"></param>
		/// <param name="newlayer"></param>
		/// <param name="mainObj"></param>
		protected override void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
			base.OnLayerChanged(oldlayer, newlayer, mainObj);
			if (oldlayer != null && newlayer == null && oldlayer.IsInDocument)
			{
				foreach (KeyValuePair<GoView, Control> item in Map)
				{
					GoView key = item.Key;
					Control value = item.Value;
					if (key != null && value != null)
					{
						DisposeControl(value, key);
					}
				}
				Map.Clear();
			}
			else
			{
				if (oldlayer == null || newlayer != null || !oldlayer.IsInView)
				{
					return;
				}
				GoView view = oldlayer.View;
				Control control = FindControl(view);
				if (control != null)
				{
					Map.Remove(view);
					if (control != null)
					{
						DisposeControl(control, view);
					}
				}
			}
		}

		/// <summary>
		/// This returns the <c>Control</c> that exists for this object
		/// in the given <paramref name="view" />.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>a <c>Control</c>, or null if <see cref="M:Northwoods.Go.GoControl.GetControl(Northwoods.Go.GoView)" />
		/// hasn't been called or if <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" /> returned null</returns>
		public virtual Control FindControl(GoView view)
		{
			if (Map.TryGetValue(view, out Control value))
			{
				return value;
			}
			return null;
		}

		/// <summary>
		/// This is responsible for creating and initializing a <c>Control</c>
		/// and adding it to a view, if does not already exist.
		/// </summary>
		/// <param name="view">must not be null,
		/// and either <c>this.View == view</c> or <c>this.Document == view.Document</c>
		/// </param>
		/// <returns>A <c>Control</c>, added to the <pararef name="view" />.</returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoControl.FindControl(Northwoods.Go.GoView)" />.
		/// If the result is null, this calls <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" /> and
		/// remembers the resulting <c>Control</c> in the <see cref="P:Northwoods.Go.GoControl.Map" />,
		/// for <see cref="M:Northwoods.Go.GoControl.FindControl(Northwoods.Go.GoView)" /> to return.
		/// </remarks>
		public virtual Control GetControl(GoView view)
		{
			if (view == null)
			{
				return null;
			}
			if (base.IsInView && base.View != view)
			{
				return null;
			}
			if (base.IsInDocument && base.Document != view.Document)
			{
				return null;
			}
			Control control = FindControl(view);
			if (control == null)
			{
				control = CreateControl(view);
				if (control != null)
				{
					Map[view] = control;
					view.AddGoControl(this, control);
				}
			}
			return control;
		}

		/// <summary>
		/// Create a Control by calling the zero argument constructor
		/// of the <see cref="P:Northwoods.Go.GoControl.ControlType" />.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>A <c>Control</c>.</returns>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoControl.GetControl(Northwoods.Go.GoView)" /> is reponsible for adding the <c>Control</c>
		/// to the view's collection of controls.
		/// </remarks>
		[UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows)]
		public virtual Control CreateControl(GoView view)
		{
			Type controlType = ControlType;
			if (controlType != null)
			{
				Control obj = (Control)Activator.CreateInstance(controlType);
				RectangleF bounds = Bounds;
				Rectangle rectangle2 = obj.Bounds = view.ConvertDocToView(bounds);
				IGoControlObject goControlObject = obj as IGoControlObject;
				if (goControlObject != null)
				{
					goControlObject.GoView = view;
					goControlObject.GoControl = this;
				}
				return obj;
			}
			return null;
		}

		/// <summary>
		/// This method is called when a <c>Control</c> created by
		/// <see cref="M:Northwoods.Go.GoControl.CreateControl(Northwoods.Go.GoView)" /> needs to be removed from a view.
		/// </summary>
		/// <param name="comp"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// By default this also calls <c>Control.Dispose()</c>.
		/// If the view is in "edit" mode using this GoControl,
		/// the <c>Control</c> is removed and disposed by the view,
		/// after <see cref="P:Northwoods.Go.GoView.EditControl" /> is set to null.
		/// </remarks>
		public virtual void DisposeControl(Control comp, GoView view)
		{
			if (comp != null && view != null)
			{
				if (view.EditControl != this)
				{
					view.RemoveGoControl(this, comp);
					comp.Dispose();
				}
				else
				{
					comp.Visible = false;
				}
			}
		}

		/// <summary>
		/// If the GoControl's visibility changes, we need to change the
		/// visibility of all of its corresponding Controls.
		/// </summary>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (!base.SuspendsUpdates)
			{
				base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				if (subhint == 1003)
				{
					foreach (KeyValuePair<GoView, Control> item in Map)
					{
						GoView key = item.Key;
						Control value = item.Value;
						if (key != null && !CanView() && value != null)
						{
							value.Visible = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Make sure the Control in the view exists and is positioned and sized correctly.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// This ignores the <see cref="P:Northwoods.Go.GoObject.Shadowed" /> property.
		/// Controls are not created for <see cref="T:Northwoods.Go.GoOverview" /> views--instead
		/// they are drawn as light gray rectangles.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			if (view is GoOverview)
			{
				RectangleF bounds = Bounds;
				GoShape.DrawRectangle(g, view, GoShape.Pens_Black, GoShape.Brushes_LightGray, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				return;
			}
			Control control = GetControl(view);
			if (control != null)
			{
				RectangleF bounds2 = Bounds;
				Rectangle bounds3 = view.ConvertDocToView(bounds2);
				if (!(control is GoText.ComboBoxControl))
				{
					control.Bounds = bounds3;
				}
				if (!control.Visible)
				{
					control.Visible = true;
				}
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			int subHint = e.SubHint;
			if (subHint == 1901)
			{
				ControlType = (Type)e.GetValue(undo);
			}
			else
			{
				base.ChangeValue(e, undo);
			}
		}
	}
}
