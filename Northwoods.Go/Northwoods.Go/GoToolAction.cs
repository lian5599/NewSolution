using System;

namespace Northwoods.Go
{
	/// <summary>
	/// This modeless tool is used to handle objects like buttons or knobs that
	/// implement the <see cref="T:Northwoods.Go.IGoActionObject" /> interface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An instance of this tool is the first tool in the default
	/// <see cref="T:Northwoods.Go.GoView" />'s <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
	/// </para>
	/// <para>
	/// A mouse down activates the <see cref="T:Northwoods.Go.IGoActionObject" />
	/// under that mouse point.  The object can use the setting of the
	/// <see cref="P:Northwoods.Go.IGoActionObject.ActionActivated" /> property to true to render differently.
	/// For example, a <see cref="T:Northwoods.Go.GoButton" /> will appear "pressed".
	/// </para>
	/// <para>
	/// A mouse move will invoke <see cref="M:Northwoods.Go.IGoActionObject.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" />.
	/// The object can use this notificaton to track the user's mouse
	/// in adjusting the state of the object.  For example, a knob's
	/// direction and value can follow the mouse over a range of values.
	/// </para>
	/// <para>
	/// A mouse up will invoke <see cref="M:Northwoods.Go.IGoActionObject.OnAction(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" />.
	/// The object can use this notification to perform some action.
	/// A button can actually do some work; a knob could actually set the
	/// new value in the document.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoToolAction : GoTool
	{
		[NonSerialized]
		private IGoActionObject myActionObject;

		[NonSerialized]
		private bool myAdjustedAutomatically;

		/// <summary>
		/// True when <see cref="T:Northwoods.Go.IGoActionObject" />.<see cref="M:Northwoods.Go.IGoActionObject.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" />.
		/// is called automatically, due to a Windows Forms Timer.
		/// </summary>
		public bool AdjustedAutomatically
		{
			get
			{
				return myAdjustedAutomatically;
			}
			set
			{
				myAdjustedAutomatically = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.IGoActionObject" /> being manipulated.
		/// </summary>
		/// <value>
		/// This is normally set to the value returned by <see cref="M:Northwoods.Go.GoToolAction.PickActionObject" />.
		/// Typically this value will refer to the same object as <see cref="P:Northwoods.Go.GoTool.CurrentObject" />.
		/// </value>
		public IGoActionObject ActionObject
		{
			get
			{
				return myActionObject;
			}
			set
			{
				myActionObject = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolAction(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// This tool is appropriate when the last input event is over an object
		/// that implements <see cref="T:Northwoods.Go.IGoActionObject" />, or is over a part of
		/// an <see cref="T:Northwoods.Go.IGoActionObject" />.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This tool is not runnable when the user uses the context button.
		/// </remarks>
		public override bool CanStart()
		{
			if (base.FirstInput.IsContextButton)
			{
				return false;
			}
			return PickActionObject() != null;
		}

		/// <summary>
		/// Get the <see cref="T:Northwoods.Go.IGoActionObject" /> that the mouse is over, and activate it
		/// by setting its <see cref="P:Northwoods.Go.IGoActionObject.ActionActivated" /> property to true
		/// and calling its <see cref="M:Northwoods.Go.IGoActionObject.OnActionActivated(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" /> method.
		/// </summary>
		public override void Start()
		{
			ActionObject = PickActionObject();
			if (ActionObject == null)
			{
				StopTool();
				return;
			}
			ActionObject.ActionActivated = true;
			ActionObject.OnActionActivated(base.View, base.LastInput);
		}

		/// <summary>
		/// Find an <see cref="T:Northwoods.Go.IGoActionObject" /> that the last input event is over.
		/// </summary>
		/// <returns>an <see cref="T:Northwoods.Go.IGoActionObject" />, or null if none</returns>
		/// <remarks>
		/// This proceeds up the chain of <see cref="P:Northwoods.Go.GoObject.Parent" />s to find an
		/// object that implements <see cref="T:Northwoods.Go.IGoActionObject" /> and whose
		/// <see cref="P:Northwoods.Go.IGoActionObject.ActionEnabled" /> property is true.
		/// The result is remembered as the value of the <see cref="P:Northwoods.Go.GoToolAction.ActionObject" /> property.
		/// </remarks>
		public virtual IGoActionObject PickActionObject()
		{
			for (GoObject goObject = base.View.PickObject(doc: true, view: false, base.LastInput.DocPoint, selectableOnly: false); goObject != null; goObject = goObject.Parent)
			{
				IGoActionObject goActionObject = goObject as IGoActionObject;
				if (goActionObject != null && goActionObject.ActionEnabled)
				{
					base.CurrentObject = goObject;
					return goActionObject;
				}
			}
			return null;
		}

		/// <summary>
		/// Make sure any <see cref="T:Northwoods.Go.IGoActionObject" /> is deactivated
		/// and that any automatic adjustments calls are stopped.
		/// </summary>
		public override void Stop()
		{
			StopAutoAdjusting();
			if (ActionObject != null)
			{
				ActionObject.ActionActivated = false;
			}
			ActionObject = null;
			base.CurrentObject = null;
		}

		/// <summary>
		/// Invoke the <see cref="M:Northwoods.Go.IGoActionObject.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" /> method.
		/// </summary>
		public override void DoMouseMove()
		{
			if (ActionObject != null)
			{
				ActionObject.OnActionAdjusted(base.View, base.LastInput);
			}
		}

		/// <summary>
		/// Invoke the <see cref="M:Northwoods.Go.IGoActionObject.OnAction(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" /> method.
		/// </summary>
		/// <remarks>
		/// This also stops the tool.
		/// </remarks>
		public override void DoMouseUp()
		{
			if (ActionObject != null)
			{
				ActionObject.OnAction(base.View, base.LastInput);
			}
			StopTool();
		}

		/// <summary>
		/// When this tool is cancelled, call the <see cref="M:Northwoods.Go.IGoActionObject.OnActionCancelled(Northwoods.Go.GoView)" /> method.
		/// </summary>
		public override void DoCancelMouse()
		{
			if (ActionObject != null)
			{
				ActionObject.OnActionCancelled(base.View);
			}
			base.DoCancelMouse();
		}

		/// <summary>
		/// On Windows Forms, start a Timer to repeatedly call
		/// <see cref="T:Northwoods.Go.IGoActionObject" />.<see cref="M:Northwoods.Go.IGoActionObject.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		public virtual void StartAutoAdjusting()
		{
			base.View.DoAutoAction();
		}

		/// <summary>
		/// On Windows Forms, stop any Timer that is automatically calling
		/// <see cref="T:Northwoods.Go.IGoActionObject" />.<see cref="M:Northwoods.Go.IGoActionObject.OnActionAdjusted(Northwoods.Go.GoView,Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		public virtual void StopAutoAdjusting()
		{
			base.View.StopAutoScroll();
			AdjustedAutomatically = false;
		}

		internal void AutoAdjust()
		{
			if (ActionObject != null)
			{
				AdjustedAutomatically = true;
				ActionObject.OnActionAdjusted(base.View, base.LastInput);
				AdjustedAutomatically = false;
			}
		}
	}
}
