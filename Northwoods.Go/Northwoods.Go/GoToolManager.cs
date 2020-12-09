using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool, normally the default tool for a view, used to handle input and
	/// decide if any other tools would be appropriate as the view's current tool.
	/// </summary>
	[Serializable]
	public class GoToolManager : GoTool
	{
		[NonSerialized]
		private bool myStarted;

		/// <summary>
		/// Gets or sets whether we have performed a mouse down as part of a mouse down-move-up gesture.
		/// </summary>
		/// <remarks>
		/// This property is initially false.
		/// It is set to true after all of the <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> have had 
		/// <see cref="M:Northwoods.Go.IGoTool.CanStart" /> return false.
		/// This property helps avoid any tool behavior where the mouse down actually occurred
		/// in a different window.
		/// </remarks>
		public bool Started
		{
			get
			{
				return myStarted;
			}
			set
			{
				myStarted = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolManager(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoToolManager.Started" /> property to false.
		/// </summary>
		public override void Stop()
		{
			Started = false;
		}

		/// <summary>
		/// Search <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> for the first tool that we can start;
		/// if we find one, we start it by making it the view's current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </summary>
		/// <remarks>
		/// This sets the <see cref="P:Northwoods.Go.GoToolManager.Started" /> property to true if we did not find a startable
		/// tool, so that later searches for tools in the <see cref="M:Northwoods.Go.GoToolManager.DoMouseMove" /> and
		/// <see cref="M:Northwoods.Go.GoToolManager.DoMouseUp" /> methods can proceed.
		/// </remarks>
		public override void DoMouseDown()
		{
			foreach (IGoTool mouseDownTool in base.View.MouseDownTools)
			{
				IGoTool goTool = mouseDownTool as IGoTool;
				if (goTool != null && goTool.CanStart())
				{
					base.View.Tool = goTool;
					return;
				}
			}
			Started = true;
		}

		/// <summary>
		/// Search <see cref="P:Northwoods.Go.GoView.MouseMoveTools" /> for the first tool that we can start;
		/// if we find one, we start it by making it the view's current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </summary>
		/// <remarks>
		/// This implementation does not do the search when <see cref="P:Northwoods.Go.GoToolManager.Started" /> is false,
		/// presumably because of a mouse motion without a mouse down in this view.
		/// This method calls <see cref="M:Northwoods.Go.GoToolManager.DoMouseOver(Northwoods.Go.GoInputEventArgs)" /> if no startable tool is
		/// found and started.
		/// </remarks>
		public override void DoMouseMove()
		{
			if (Started)
			{
				foreach (IGoTool mouseMoveTool in base.View.MouseMoveTools)
				{
					IGoTool goTool = mouseMoveTool as IGoTool;
					if (goTool != null && goTool.CanStart())
					{
						base.View.Tool = goTool;
						return;
					}
				}
			}
			DoMouseOver(base.LastInput);
		}

		/// <summary>
		/// Call <see cref="T:Northwoods.Go.GoView" /> methods <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" /> and <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method keeps track of the current document object that the mouse is over.
		/// When the current document object changes, this calls <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" />.
		/// This also calls <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />.
		/// </remarks>
		public virtual void DoMouseOver(GoInputEventArgs evt)
		{
			GoObject currentObject = base.CurrentObject;
			GoObject goObject = base.View.PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			if (currentObject != goObject)
			{
				base.CurrentObject = goObject;
				base.View.DoObjectEnterLeave(currentObject, goObject, evt);
			}
			base.View.DoMouseOver(evt);
		}

		/// <summary>
		/// Search <see cref="P:Northwoods.Go.GoView.MouseUpTools" /> for the first tool that we can start;
		/// if we find one, we start it by making it the view's current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </summary>
		public override void DoMouseUp()
		{
			if (Started)
			{
				foreach (IGoTool mouseUpTool in base.View.MouseUpTools)
				{
					IGoTool goTool = mouseUpTool as IGoTool;
					if (goTool != null && goTool.CanStart())
					{
						base.View.Tool = goTool;
						break;
					}
				}
			}
		}

		/// <summary>
		/// When there are no other tools running, a mouse wheel event scrolls or zooms
		/// the view by calling <see cref="M:Northwoods.Go.GoView.DoWheel(Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		public override void DoMouseWheel()
		{
			base.View.DoWheel(base.LastInput);
		}

		/// <summary>
		/// When there are no other tools running, a mouse hover just invokes
		/// <see cref="M:Northwoods.Go.GoView.DoHover(Northwoods.Go.GoInputEventArgs)" />, which in turn raises <see cref="E:Northwoods.Go.GoView.ObjectHover" />
		/// and <see cref="E:Northwoods.Go.GoView.BackgroundHover" /> events.
		/// </summary>
		public override void DoMouseHover()
		{
			base.View.DoHover(base.LastInput);
		}

		/// <summary>
		/// Provide default behavior, when not running some other tool.
		/// </summary>
		/// <remarks>
		/// By default this handles:
		/// <list type="bullet">
		/// <item>Delete: the Delete key deletes the current selection</item>
		/// <item>Select All: Ctrl-A selects all selectable document objects</item>
		/// <item>Copy, Cut, Paste: The Ctrl-C, Ctrl-Insert, Ctrl-X, Shift-Delete, Ctrl-V, and Shift-Insert keys do the standard clipboard operations</item>
		/// <item>Edit: the F2 key starts in-place editing of the current node's text label</item>
		/// <item>PageDown, PageUp: The PageDown and PageUp keys scroll vertically; Shift-PageDown and Shift-PageUp
		/// scroll horizontally</item>
		/// <item>Home, End: the Home and End keys scroll to the left side and right sides of the document;
		/// Ctrl-Home and Ctrl-End scroll to the top-left and bottom-right corners of the document, respectively</item>
		/// <item>Undo, Redo: Ctrl-Z and Ctrl-Y (or Ctrl-Shift-Z) perform undo and redo</item>
		/// <item>Arrow keys: moves the selection in the given direction, or else scrolls the view that way;
		/// the Ctrl modifier causes the movement or scrolling to just be one pixel at a time.
		/// This uses the <see cref="P:Northwoods.Go.GoView.ArrowMoveLarge" /> and <see cref="P:Northwoods.Go.GoView.ArrowMoveSmall" /> properties
		/// to control how far the selection is moved.</item>
		/// <item>Escape: the Escape key may clear the selection and then cancels the current input operation</item>
		/// <item>letters and digits: selects the next node whose text starts with that character</item>
		/// </list>
		/// However, the value of <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DisableKeys" /> can disable
		/// each of these behaviors.
		/// </remarks>
		public override void DoKeyDown()
		{
			GoInputEventArgs lastInput = base.LastInput;
			bool control = lastInput.Control;
			bool shift = lastInput.Shift;
			GoViewDisableKeys disableKeys = base.View.DisableKeys;
			Keys key = lastInput.Key;
			if (key == Keys.Delete && (disableKeys & GoViewDisableKeys.Delete) == 0)
			{
				base.View.EditDelete();
				return;
			}
			if (control && key == Keys.A && (disableKeys & GoViewDisableKeys.SelectAll) == 0)
			{
				base.View.SelectAll();
				return;
			}
			if (((control && key == Keys.C) /*|| (control && key == Keys.Insert) bylzy*/) && (disableKeys & GoViewDisableKeys.Clipboard) == 0)
			{
				base.View.EditCopy();
				return;
			}
			if (((control && key == Keys.X) || (shift && key == Keys.Delete)) && (disableKeys & GoViewDisableKeys.Clipboard) == 0)
			{
				base.View.EditCut();
				return;
			}
			if (((control && key == Keys.V) || (shift && key == Keys.Insert)) && (disableKeys & GoViewDisableKeys.Clipboard) == 0)
			{
				base.View.EditPaste();
				return;
			}
			if (key == Keys.F2 && (disableKeys & GoViewDisableKeys.Edit) == 0)
			{
				base.View.EditEdit();
				return;
			}
			if (key == Keys.Next && (disableKeys & GoViewDisableKeys.Page) == 0)
			{
				if (shift)
				{
					base.View.ScrollPage(1f, 0f);
				}
				else
				{
					base.View.ScrollPage(0f, 1f);
				}
				return;
			}
			if (key == Keys.Prior && (disableKeys & GoViewDisableKeys.Page) == 0)
			{
				if (shift)
				{
					base.View.ScrollPage(-1f, 0f);
				}
				else
				{
					base.View.ScrollPage(0f, -1f);
				}
				return;
			}
			if (key == Keys.Home && (disableKeys & GoViewDisableKeys.Home) == 0)
			{
				RectangleF rectangleF = base.View.ComputeDocumentBounds();
				if (control)
				{
					base.View.DocPosition = new PointF(rectangleF.X, rectangleF.Y);
				}
				else
				{
					base.View.DocPosition = new PointF(rectangleF.X, base.View.DocPosition.Y);
				}
				return;
			}
			if (key == Keys.End && (disableKeys & GoViewDisableKeys.End) == 0)
			{
				RectangleF rectangleF2 = base.View.ComputeDocumentBounds();
				SizeF docExtentSize = base.View.DocExtentSize;
				PointF pointF = control ? new PointF(rectangleF2.X + rectangleF2.Width - docExtentSize.Width, rectangleF2.Y + rectangleF2.Height - docExtentSize.Height) : new PointF(rectangleF2.X + rectangleF2.Width - docExtentSize.Width, base.View.DocPosition.Y);
				base.View.DocPosition = new PointF(Math.Max(0f, pointF.X), Math.Max(0f, pointF.Y));
				return;
			}
			if (control && !shift && key == Keys.Z && (disableKeys & GoViewDisableKeys.Undo) == 0)
			{
				base.View.Undo();
				return;
			}
			if (((control && key == Keys.Y) || (control && shift && key == Keys.Z)) && (disableKeys & GoViewDisableKeys.Undo) == 0)
			{
				base.View.Redo();
				return;
			}
			if (key == Keys.Escape && (disableKeys & GoViewDisableKeys.CancelDeselects) == 0)
			{
				if (base.View.CanSelectObjects())
				{
					base.Selection.Clear();
				}
				base.DoKeyDown();
				return;
			}
			if (key == Keys.Up || key == Keys.Down || key == Keys.Left || key == Keys.Right)
			{
				float num = 0f;
				float num2 = 0f;
				switch (key)
				{
				case Keys.Left:
					num = 0f - base.View.WorldScale.Width;
					break;
				case Keys.Right:
					num = base.View.WorldScale.Width;
					break;
				case Keys.Up:
					num2 = 0f - base.View.WorldScale.Height;
					break;
				case Keys.Down:
					num2 = base.View.WorldScale.Height;
					break;
				}
				if (!base.View.Selection.IsEmpty && base.View.CanMoveObjects() && (disableKeys & GoViewDisableKeys.ArrowMove) == 0)
				{
					float num3 = 1f;
					num3 = ((!control) ? base.View.ArrowMoveLarge : base.View.ArrowMoveSmall);
					GoToolDragging goToolDragging = base.View.FindMouseTool(typeof(GoToolDragging), subclass: true) as GoToolDragging;
					if (goToolDragging != null)
					{
						GoSelection sel = goToolDragging.ComputeEffectiveSelection(base.View.Selection, move: true);
						base.View.MoveSelection(sel, new SizeF(num * num3, num2 * num3), grid: true);
						base.View.RaiseSelectionMoved();
						base.View.ScrollRectangleToVisible(base.View.Selection.Primary.Bounds);
					}
				}
				else if ((disableKeys & GoViewDisableKeys.ArrowScroll) == 0)
				{
					if (control)
					{
						base.View.DocPosition = new PointF(base.View.DocPosition.X + num, base.View.DocPosition.Y + num2);
					}
					else
					{
						base.View.ScrollLine(num, num2);
					}
				}
				else
				{
					base.DoKeyDown();
				}
				return;
			}
			bool flag = false;
			if (!control && !lastInput.Alt && (disableKeys & GoViewDisableKeys.SelectsByFirstChar) == 0)
			{
				string text = TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(null, CultureInfo.CurrentCulture, lastInput.Key);
				char c = '\0';
				if (text.Length == 1)
				{
					c = text[0];
				}
				else if (text.Length == 2 && text[0] == 'D')
				{
					c = text[1];
				}
				if (char.IsLetterOrDigit(c))
				{
					flag = base.View.SelectNextNode(c);
				}
			}
			if (!flag)
			{
				base.DoKeyDown();
			}
		}
	}
}
