using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This tool supports both automatic and manual panning in a view.
	/// </summary>
	/// <remarks>
	/// When autopanning, this remembers an initial panning point and
	/// then autoscrolls the view in the direction of the
	/// current mouse point relative to the original panning point.
	/// This tool can be used in either a modal or a mode-less manner.
	/// To use modally, where the first mouse click will establish the
	/// panning origin, mouse moves determine autopanning direction
	/// and speed, and the second mouse up will stop the tool:
	/// <pre><code>
	///   aView.Tool = new GoToolPanning(aView);
	/// </code></pre>
	/// If you set the <see cref="P:Northwoods.Go.GoToolPanning.Origin" /> before the tool starts,
	/// the first mouse click is not needed.
	/// <pre><code>
	///   GoToolPanning tool = new GoToolPanning(aView);
	///   tool.Origin = aView.LastInput.ViewPoint;  // or another point in the view
	///   aView.Tool = tool;
	/// </code></pre>
	/// <para>
	/// It is started mode-lessly when the user presses the middle
	/// mouse button, which is normally the mouse wheel.  An instance
	/// of this tool is in the <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
	/// </para>
	/// <para>
	/// For manual panning, you will need to create a separate instance
	/// of this class and set <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> to false.
	/// Then the user's left-mouse down and drag and up will pan the view.
	/// When you set the <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> property, this tool will
	/// remain in this mode even after the user does a mouse up.
	/// They can cancel this mode by pressing the Cancel key.
	/// </para>
	/// <para>
	/// By default no manual-panning tool is installed in a <see cref="T:Northwoods.Go.GoView" />.
	/// To implement a "Manual Pan" command:
	/// <pre><code>
	/// GoToolPanning panningtool = new GoToolPanning(myView);
	/// panningtool.AutoPan = false;
	/// panningtool.Modal = true;
	/// myView.Tool = panningtool;
	/// </code></pre>
	/// However, if you do not need the user to do multiple selections by
	/// using the <see cref="T:Northwoods.Go.GoToolRubberBanding" /> tool,
	/// you may find it nicer to use the manual panning tool
	/// in a mode-less manner, so that the user can use all the other standard tools
	/// in a natural fashion.
	/// <pre><code>
	/// GoToolPanning panningtool = new GoToolPanning(myView);
	/// panningtool.AutoPan = false;
	/// myView.MouseDownTools.Add(panningtool);
	/// </code></pre>
	/// Both <see cref="T:Northwoods.Go.GoToolRubberBanding" /> and this <see cref="T:Northwoods.Go.GoToolPanning" />
	/// (when <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false) are started when the user does a
	/// mouse-down and drag in the background, so those two tools would conflict.
	/// But the user can still do multiple selections by using Shift- or Control-click.
	/// </para>
	/// <para>
	/// However, in ASP.NET WebForms, the panning gesture consists of
	/// only a single mouse-down, drag, mouse-up.  Since mouse moves
	/// are only simulated on WebForms, and auto-panning is not
	/// possible with no mouse time information and no immediate feedback,
	/// a simpler gesture is easier to use.  This results in just a
	/// single scroll, according to the distance and direction
	/// between the <c>FirstInput.ViewPoint</c> and <c>LastInput.ViewPoint</c>.
	/// </para>
	/// <para>
	/// So for WebForms, the default (modeless) panning tool that is installed
	/// as a <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> is not an auto-panning tool
	/// but a manual one: <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false.  It uses the middle
	/// mouse button instead of the left mouse button.  Only when <see cref="P:Northwoods.Go.GoToolPanning.Modal" />
	/// is true, so that the user is in a special "panning mode", does the left mouse
	/// button start and perform the pan.  If you want to let the user pan
	/// the GoDiagram Web view with the left mouse button in a modeless fashion,
	/// rather than with the middle mouse button, remove the rubberband selection tool:
	/// <c>goView1.ReplaceMouseTool(typeof(GoToolRubberBanding), null);</c>
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoToolPanning : GoTool
	{
		private bool myAutoPan = true;

		private bool myModal;

		[NonSerialized]
		private Point myLastViewPoint;

		[NonSerialized]
		private bool myActive;

		[NonSerialized]
		private bool myOriginSet;

		[NonSerialized]
		private Point myOrigin;

		[NonSerialized]
		private PaintEventHandler myPaintHandler;

		private bool Active
		{
			get
			{
				return myActive;
			}
			set
			{
				if (myActive != value)
				{
					myActive = value;
					if (value)
					{
						myLastViewPoint = base.LastInput.ViewPoint;
					}
				}
			}
		}

		private Rectangle OriginRect
		{
			get
			{
				Cursor noMove2D = Cursors.NoMove2D;
				int width = noMove2D.Size.Width;
				int height = noMove2D.Size.Height;
				checked
				{
					return new Rectangle(Origin.X - unchecked(width / 2), Origin.Y - unchecked(height / 2), width, height);
				}
			}
		}

		/// <summary>
		/// Gets or sets the original panning point.
		/// </summary>
		/// <value>
		/// This is a point in view coordinates.
		/// It is set on the first mouse up.
		/// Once this value is set, mouse moves cause
		/// </value>
		/// <remarks>
		/// This is only relevant when <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is true.
		/// </remarks>
		public Point Origin
		{
			get
			{
				return myOrigin;
			}
			set
			{
				if (myOrigin != value)
				{
					myOrigin = value;
					myOriginSet = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this tool is used to implement autopanning or manual panning.
		/// </summary>
		/// <value>the initial value is true for WinForms, false for WebForms</value>
		/// <remarks>
		/// When this value is true, this tool implements the standard
		/// auto-scrolling panning, initiated by a middle-mouse button click and
		/// terminated by a second click or a key press.
		/// When this value is false, this tool implements the standard
		/// manual-scrolling panning, initiated by a left-mouse drag in the background
		/// and terminated by a mouse up (when <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> is false).
		/// </remarks>
		public virtual bool AutoPan
		{
			get
			{
				return myAutoPan;
			}
			set
			{
				myAutoPan = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this tool is used in a modal fashion when <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false.
		/// </summary>
		/// <value>the initial value is false</value>
		/// <remarks>
		/// This property is ignored when <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is true.
		/// <para>
		/// When you add an instance of this non-autopanning tool to the <see cref="P:Northwoods.Go.GoView.MouseDownTools" />,
		/// you are using it in a non-Modal fashion.
		/// <pre><code>
		/// GoToolPanning panningtool = new GoToolPanning(myView);
		/// panningtool.AutoPan = false;
		/// myView.MouseDownTools.Add(panningtool);
		/// </code></pre>
		/// This allows the user to use the other mode-less mouse tools,
		/// such as selecting, dragging, resizing, and linking as well as this manual panning tool
		/// when the user does a mouse down in the background.
		/// </para>
		/// <para>
		/// If you use this manual panning tool in a modal fashion, the user
		/// will remain in this tool, able to pan the view whenever the user drags
		/// the mouse anywhere in the view.
		/// <pre><code>
		/// GoToolPanning panningtool = new GoToolPanning(myView);
		/// panningtool.AutoPan = false;
		/// panningtool.Modal = true;
		/// myView.Tool = panningtool;
		/// </code></pre>
		/// The user can leave this mode by pressing the Cancel key or any other key.
		/// </para>
		/// </remarks>
		public virtual bool Modal
		{
			get
			{
				return myModal;
			}
			set
			{
				myModal = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolPanning(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// This tool can start when the middle mouse button is pressed,
		/// unless <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false (for manual panning),
		/// in which case the left mouse button is used.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// However, for manual panning on WebForms, the left mouse button
		/// is expected only when <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> is true.
		/// When used modelessly, the middle button must be used, since the
		/// whole operation will consist of a background-mouse-down and a mouse-up,
		/// and that gesture will not be confused with a background rubber-band
		/// selection.
		/// </remarks>
		public override bool CanStart()
		{
			GoInputEventArgs lastInput = base.LastInput;
			if (lastInput.Alt || lastInput.Control || lastInput.Shift)
			{
				return false;
			}
			if (AutoPan)
			{
				return lastInput.Buttons == MouseButtons.Middle;
			}
			if (lastInput.Buttons != MouseButtons.Left)
			{
				return false;
			}
			return base.View.PickObject(doc: true, view: true, lastInput.DocPoint, selectableOnly: true) == null;
		}

		/// <summary>
		/// Initialize this tool.
		/// </summary>
		public override void Start()
		{
			if (AutoPan)
			{
				base.View.CursorName = "nomove2d";
				if (myOriginSet)
				{
					SetPaintingOriginMarker(b: true);
				}
			}
			else
			{
				base.View.CursorName = "move";
			}
		}

		/// <summary>
		/// Stop any auto-panning in the view and remove the original panning point marker.
		/// </summary>
		public override void Stop()
		{
			if (AutoPan)
			{
				myOriginSet = false;
				base.View.StopAutoScroll();
				SetPaintingOriginMarker(b: false);
			}
			else
			{
				Active = false;
			}
			base.View.CursorName = "default";
		}

		/// <summary>
		/// When manually panning (i.e. <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false),
		/// a mouse down causes future mouse moves to change the view's
		/// <see cref="P:Northwoods.Go.GoView.DocPosition" /> to move along with the mouse.
		/// </summary>
		public override void DoMouseDown()
		{
			if (AutoPan)
			{
				base.DoMouseDown();
			}
			else
			{
				Active = true;
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.DoAutoPan(System.Drawing.Point,System.Drawing.Point)" /> to pan the view according to
		/// the current mouse point relative to the <see cref="P:Northwoods.Go.GoToolPanning.Origin" />.
		/// </summary>
		/// <remarks>
		/// When autopanning, until the <see cref="P:Northwoods.Go.GoToolPanning.Origin" /> panning point is set, this method
		/// does nothing.
		/// When manually panning, this changes the view's <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// along with changes of the mouse position in the view, thereby panning the view.
		/// However, when <see cref="P:Northwoods.Go.GoToolPanning.AutoPan" /> is false and <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> is true,
		/// mouse moves have no effect unless a drag is in progress--i.e. after a mouse down.
		/// </remarks>
		public override void DoMouseMove()
		{
			checked
			{
				if (AutoPan)
				{
					if (!myOriginSet)
					{
						return;
					}
					Size size = new Size(16, 16);
					int width = size.Width;
					int height = size.Height;
					int num = base.LastInput.ViewPoint.X - Origin.X;
					int num2 = base.LastInput.ViewPoint.Y - Origin.Y;
					if (num < -width)
					{
						if (num2 < -height)
						{
							base.View.CursorName = "pannw";
						}
						else if (num2 > height)
						{
							base.View.CursorName = "pansw";
						}
						else
						{
							base.View.CursorName = "panwest";
						}
					}
					else if (num > width)
					{
						if (num2 < -height)
						{
							base.View.CursorName = "panne";
						}
						else if (num2 > height)
						{
							base.View.CursorName = "panse";
						}
						else
						{
							base.View.CursorName = "paneast";
						}
					}
					else if (num2 < -height)
					{
						base.View.CursorName = "pannorth";
					}
					else if (num2 > height)
					{
						base.View.CursorName = "pansouth";
					}
					else
					{
						base.View.CursorName = "nomove2d";
					}
					base.View.DoAutoPan(Origin, base.LastInput.ViewPoint);
				}
				else if (!Active)
				{
					if (!Modal)
					{
						Active = true;
					}
				}
				else
				{
					DoManualPan();
				}
			}
		}

		private void DoManualPan()
		{
			PointF docPosition = base.View.DocPosition;
			Size s = checked(new Size(myLastViewPoint.X - base.LastInput.ViewPoint.X, myLastViewPoint.Y - base.LastInput.ViewPoint.Y));
			SizeF sizeF = base.View.ConvertViewToDoc(s);
			myLastViewPoint = base.LastInput.ViewPoint;
			base.View.DocPosition = new PointF(docPosition.X + sizeF.Width, docPosition.Y + sizeF.Height);
		}

		/// <summary>
		/// On the first mouse up, set the <see cref="P:Northwoods.Go.GoToolPanning.Origin" /> point and display
		/// the panning origin marker; on the second second mouse up, stop this tool.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For WinForms, when autopanning, if the <see cref="P:Northwoods.Go.GoToolPanning.Origin" /> has already been set,
		/// a mouse up just stops this tool.
		/// When manually panning, a mouse up just stops this tool, unless
		/// <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> is true, in which case this tool waits for a mouse
		/// down again to start panning during mouse moves.
		/// </para>
		/// <para>
		/// For WebForms, the behavior is different.
		/// When autopanning, this method always sets
		/// the <see cref="P:Northwoods.Go.GoToolPanning.Origin" /> to the <c>FirstInput.ViewPoint</c>,
		/// scrolls the view according to <see cref="M:Northwoods.Go.GoView.ComputeAutoPanDocPosition(System.Drawing.Point,System.Drawing.Point)" />,
		/// and then stops this tool.
		/// When manually panning, this changes the <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// according to the difference between the mouse down and the mouse up points.
		/// Again, the value of <see cref="P:Northwoods.Go.GoToolPanning.Modal" /> determines whether this tool then stops,
		/// or if it waits for another mouse down/up.
		/// </para>
		/// </remarks>
		public override void DoMouseUp()
		{
			if (AutoPan)
			{
				if (!myOriginSet)
				{
					Origin = base.LastInput.ViewPoint;
					SetPaintingOriginMarker(b: true);
				}
				else
				{
					StopTool();
				}
				return;
			}
			if (!IsBeyondDragSize())
			{
				DoSelect(base.LastInput);
				DoClick(base.LastInput);
			}
			if (Modal)
			{
				Active = false;
			}
			else
			{
				StopTool();
			}
		}

		/// <summary>
		/// Stop panning whenever the mouse wheel turns.
		/// </summary>
		public override void DoMouseWheel()
		{
			StopTool();
		}

		/// <summary>
		/// Stop panning whenever any key is pressed.
		/// </summary>
		public override void DoKeyDown()
		{
			StopTool();
		}

		private void SetPaintingOriginMarker(bool b)
		{
			if (b)
			{
				myPaintHandler = HandlePaint;
				base.View.Paint += myPaintHandler;
			}
			else if (myPaintHandler != null)
			{
				base.View.Paint -= myPaintHandler;
				myPaintHandler = null;
			}
			base.View.Invalidate(OriginRect);
		}

		private void HandlePaint(object sender, PaintEventArgs evt)
		{
			if (myOriginSet)
			{
				Cursors.NoMove2D.Draw(evt.Graphics, OriginRect);
			}
		}
	}
}
