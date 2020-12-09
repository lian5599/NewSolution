using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to implement dragging behavior, for moving and copying objects.
	/// </summary>
	/// <remarks>
	/// This tool is expected to be invoked upon a mouse move.
	/// </remarks>
	[Serializable]
	public class GoToolDragging : GoTool
	{
		private bool myCopiesEffectiveSelection;

		private bool myEffectiveSelectionIncludesLinks = true;

		private bool myHidesSelectionHandles = true;

		[NonSerialized]
		private GoSelection myEffectiveSelection;

		[NonSerialized]
		private GoSelection myDragSelection;

		[NonSerialized]
		private GoObject myDragSelectionOrigObj;

		[NonSerialized]
		private SizeF myMoveOffset;

		[NonSerialized]
		private bool mySelectionHidden;

		private bool mySelectsWhenStarts = true;

		[NonSerialized]
		private GoObject myLastObject;

		[NonSerialized]
		private bool myModalDropped;

		/// <summary>
		/// Gets or sets the alternative selection collection, holding an image
		/// of the view's selection.
		/// </summary>
		/// <remarks>
		/// This is created by <see cref="M:Northwoods.Go.GoToolDragging.CreateDragSelection" />, and is reset by
		/// <see cref="M:Northwoods.Go.GoToolDragging.ClearDragSelection" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoToolDragging.MakeDragSelection" />
		public GoSelection DragSelection
		{
			get
			{
				return myDragSelection;
			}
			set
			{
				myDragSelection = value;
			}
		}

		/// <summary>
		/// Gets or sets what had been the <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> before the drag
		/// selection substituted its own.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoToolDragging.DragSelection" />
		public GoObject DragSelectionOriginalObject
		{
			get
			{
				return myDragSelectionOrigObj;
			}
			set
			{
				myDragSelectionOrigObj = value;
			}
		}

		/// <summary>
		/// Gets the cached result of a call to <see cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" />.
		/// </summary>
		public GoSelection EffectiveSelection => myEffectiveSelection;

		/// <summary>
		/// Control whether <see cref="M:Northwoods.Go.GoToolDragging.Start" /> may modify the view's selection.
		/// </summary>
		/// <value>
		/// This defaults to true--<see cref="M:Northwoods.Go.GoToolDragging.Start" /> changes the selection
		/// based on where the mouse is and what modifier keys are in effect.
		/// </value>
		/// <remarks>
		/// You would set this to false when using this tool in a modal fashion,
		/// where your code has already specified the objects to be dragged in the view's
		/// <see cref="T:Northwoods.Go.GoSelection" />.  If you set this property to true,
		/// you should also set <see cref="P:Northwoods.Go.GoToolDragging.MoveOffset" /> and <see cref="P:Northwoods.Go.GoTool.CurrentObject" />.
		/// </remarks>
		public bool SelectsWhenStarts
		{
			get
			{
				return mySelectsWhenStarts;
			}
			set
			{
				mySelectsWhenStarts = value;
			}
		}

		/// <summary>
		/// Gets or sets the offset of the mouse point within the current object.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// The mouse is normally inside the current object, which is just one
		/// of the selected objects being dragged.
		/// </remarks>
		public SizeF MoveOffset
		{
			get
			{
				return myMoveOffset;
			}
			set
			{
				myMoveOffset = value;
			}
		}

		/// <summary>
		/// Gets or sets whether for a copying operation the <see cref="P:Northwoods.Go.GoView.Selection" />
		/// <see cref="P:Northwoods.Go.GoToolDragging.EffectiveSelection" /> is copied.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// Basically this controls whether <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> is
		/// called with the view's <see cref="P:Northwoods.Go.GoView.Selection" /> or with the possibly
		/// augmented <see cref="P:Northwoods.Go.GoToolDragging.EffectiveSelection" />.  The latter collection typically
		/// will hold all of the links that connect selected nodes.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" />
		public virtual bool CopiesEffectiveSelection
		{
			get
			{
				return myCopiesEffectiveSelection;
			}
			set
			{
				myCopiesEffectiveSelection = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" /> should include
		/// links that have both ports in (or part of) the selection being dragged.
		/// </summary>
		/// <value>The default value is true.</value>
		/// <remarks>
		/// When this is true, links that connect dragged nodes will be moved along
		/// with the nodes.  In such cases links will normally not be rerouted and thus
		/// will maintain the shape that they had originally connecting the nodes.
		/// When this property is false, <see cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" />
		/// will not include such links in the effective selection -- after the nodes
		/// are dragged, the links will need to recalculate their strokes, thereby
		/// potentially losing any special path that they may have had between the nodes.
		/// </remarks>
		public virtual bool EffectiveSelectionIncludesLinks
		{
			get
			{
				return myEffectiveSelectionIncludesLinks;
			}
			set
			{
				myEffectiveSelectionIncludesLinks = value;
			}
		}

		/// <summary>
		/// Gets or sets whether selection handles should be removed as the drag starts.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		public virtual bool HidesSelectionHandles
		{
			get
			{
				return myHidesSelectionHandles;
			}
			set
			{
				myHidesSelectionHandles = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolDragging(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// The dragging tool is applicable when the user can move or copy one or more objects.
		/// </summary>
		/// <returns>
		/// This predicate returns true when:
		/// <list type="bullet">
		/// <item>the user has started moving the mouse with a mouse button down</item>
		/// <item>the view allows objects to be moved or copied or dragged out of the window</item>
		/// <item>the mouse button is not the context menu button</item>
		/// <item>there is a selectable object under the mouse</item>
		/// <item>and that object can be moved or copied</item>
		/// </list>
		/// </returns>
		public override bool CanStart()
		{
			if (!base.View.CanMoveObjects() && !base.View.CanCopyObjects() && !base.View.AllowDragOut)
			{
				return false;
			}
			if (base.LastInput.IsContextButton)
			{
				return false;
			}
			if (!IsBeyondDragSize())
			{
				return false;
			}
			GoObject goObject = base.View.PickObject(doc: true, view: false, base.FirstInput.DocPoint, selectableOnly: true);
			if (goObject == null)
			{
				return false;
			}
			if (!goObject.CanMove() && !goObject.CanCopy())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Start a drag-and-drop operation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This first remembers the <see cref="P:Northwoods.Go.GoToolDragging.MoveOffset" /> between the <see cref="P:Northwoods.Go.GoTool.CurrentObject" />'s
		/// position and the mouse point (the first input event point).
		/// It removes any selection handles, so those do not need to be dragged along.
		/// It also starts a transaction.
		/// If the view's <see cref="P:Northwoods.Go.GoView.AllowDragOut" /> property is true, we call
		/// <c>Control.DoDragDrop</c> to start the standard modal drag-and-drop process.
		/// </para>
		/// <para>
		/// This depends on the cooperation of <see cref="M:Northwoods.Go.GoView.OnDragOver(System.Windows.Forms.DragEventArgs)" />, <see cref="M:Northwoods.Go.GoView.OnDragDrop(System.Windows.Forms.DragEventArgs)" />,
		/// and <see cref="M:Northwoods.Go.GoView.OnQueryContinueDrag(System.Windows.Forms.QueryContinueDragEventArgs)" /> to call <see cref="M:Northwoods.Go.GoToolDragging.DoMouseMove" />,
		/// <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" />, and <see cref="M:Northwoods.Go.GoToolDragging.DoCancelMouse" /> appropriately when the
		/// drop target is the same view as the drag source.
		/// If the view's <see cref="P:Northwoods.Go.GoView.AllowDragOut" /> property is false, the
		/// normal calls to <see cref="M:Northwoods.Go.GoToolDragging.DoMouseMove" />, <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" />, and
		/// <see cref="M:Northwoods.Go.GoToolDragging.DoCancelMouse" /> occur.
		/// </para>
		/// <para>
		/// If <see cref="P:Northwoods.Go.GoToolDragging.SelectsWhenStarts" /> is false, this method does not
		/// modify the current selection and does not set
		/// <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> or <see cref="P:Northwoods.Go.GoToolDragging.MoveOffset" />.
		/// </para>
		/// </remarks>
		public override void Start()
		{
			if (SelectsWhenStarts)
			{
				base.CurrentObject = base.View.PickObject(doc: true, view: false, base.FirstInput.DocPoint, selectableOnly: true);
				if (base.CurrentObject == null)
				{
					return;
				}
				MoveOffset = GoTool.SubtractPoints(base.FirstInput.DocPoint, base.CurrentObject.Position);
			}
			StartTransaction();
			if (SelectsWhenStarts && !base.Selection.Contains(base.CurrentObject))
			{
				if (base.FirstInput.Shift || base.FirstInput.Control)
				{
					base.Selection.Add(base.CurrentObject);
				}
				else
				{
					base.Selection.Select(base.CurrentObject);
				}
			}
			if (!base.View.DragRoutesRealtime && base.View.DragsRealtime)
			{
				base.View.Document.SuspendsRouting = true;
			}
			if (HidesSelectionHandles)
			{
				mySelectionHidden = true;
				base.Selection.RemoveAllSelectionHandles();
			}
			if (base.View.AllowDragOut)
			{
				myModalDropped = false;
				try
				{
					if (base.Selection.Primary != null)
					{
						SizeF hotSpot = GoTool.SubtractPoints(base.LastInput.DocPoint, base.Selection.Primary.Position);
						base.Selection.HotSpot = hotSpot;
					}
					DoDragDrop(base.Selection, DragDropEffects.All);
				}
				catch (VerificationException ex)
				{
					GoObject.Trace("GoToolDragging Start: " + ex.ToString());
				}
				catch (SecurityException ex2)
				{
					GoObject.Trace("GoToolDragging Start: " + ex2.ToString());
				}
				finally
				{
					if (!myModalDropped)
					{
						DoCancelMouse();
					}
					else
					{
						StopTool();
					}
					base.Selection.HotSpot = default(SizeF);
				}
			}
		}

		/// <summary>
		/// Call <c>Control.DoDragDrop</c>
		/// </summary>
		/// <param name="coll">a collection of objects being dragged,
		/// normally the view's <see cref="P:Northwoods.Go.GoView.Selection" /></param>
		/// <param name="allow">this is passed as the second argument to <c>Control.DoDragDrop</c></param>
		/// <remarks>
		/// This is in a separate method for easy overriding, so that you can
		/// substitute other serializable objects or <c>IDataObject</c>s to be
		/// dragged out to other windows.
		/// </remarks>
		public virtual void DoDragDrop(IGoCollection coll, DragDropEffects allow)
		{
			base.View.DoDragDrop(coll, allow);
		}

		/// <summary>
		/// Clean up after any drag.
		/// </summary>
		/// <remarks>
		/// This restores any hidden selection handles, removes any
		/// drag selection objects, and stops the current transaction.
		/// </remarks>
		public override void Stop()
		{
			base.View.Document.SuspendsRouting = false;
			if (!base.View.DragRoutesRealtime && base.View.DragsRealtime && base.TransactionResult != null)
			{
				base.View.Document.DoDelayedRouting(EffectiveSelection);
			}
			base.View.StopAutoScroll();
			if (mySelectionHidden)
			{
				mySelectionHidden = false;
				base.Selection.AddAllSelectionHandles();
			}
			ClearDragSelection();
			myEffectiveSelection = null;
			MoveOffset = default(SizeF);
			base.CurrentObject = null;
			SelectsWhenStarts = true;
			StopTransaction();
		}

		/// <summary>
		/// Mouse drags just call <see cref="M:Northwoods.Go.GoToolDragging.DoDragging(Northwoods.Go.GoInputState)" /> and <see cref="M:Northwoods.Go.GoToolDragging.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		/// <remarks>
		/// By default this sets the <c>Effect</c> according to the
		/// values of <see cref="M:Northwoods.Go.GoToolDragging.MayBeCopying" />, <see cref="M:Northwoods.Go.GoToolDragging.MayBeMoving" />,
		/// <see cref="M:Northwoods.Go.GoToolDragging.MustBeCopying" />, <see cref="M:Northwoods.Go.GoToolDragging.MustBeMoving" />
		/// and whether all of the objects in the selection can be copied or moved.
		/// Finally it calls <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoAutoScroll(System.Drawing.Point)" />.
		/// </remarks>
		public override void DoMouseMove()
		{
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null)
			{
				if (MustBeCopying() && MayBeCopying())
				{
					dragEventArgs.Effect = DragDropEffects.Copy;
				}
				else if (MayBeMoving())
				{
					if (!base.View.PretendsInternalDrag)
					{
						dragEventArgs.Effect = DragDropEffects.Move;
					}
				}
				else
				{
					dragEventArgs.Effect = DragDropEffects.None;
				}
			}
			base.LastInput.InputState = GoInputState.Continue;
			if (dragEventArgs != null && base.View.DoSelectionDropReject(base.LastInput))
			{
				dragEventArgs.Effect = DragDropEffects.None;
			}
			DoDragging(GoInputState.Continue);
			DoMouseOver(base.LastInput);
			base.View.DoAutoScroll(base.LastInput.ViewPoint);
		}

		/// <summary>
		/// Call <see cref="T:Northwoods.Go.GoView" /> method <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method keeps track of the current document object that the mouse is over.
		/// It ignores any objects that are part of the selection.
		/// When the current document object changes, this calls <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" />.
		/// </remarks>
		public virtual void DoMouseOver(GoInputEventArgs evt)
		{
			GoObject goObject = myLastObject;
			GoObject goObject2 = base.View.PickObjectExcluding(doc: true, view: false, evt.DocPoint, selectableOnly: false, base.Selection);
			if (goObject != goObject2)
			{
				myLastObject = goObject2;
				base.View.DoObjectEnterLeave(goObject, goObject2, evt);
			}
		}

		/// <summary>
		/// The release of the mouse makes a final call to <see cref="M:Northwoods.Go.GoToolDragging.DoDragging(Northwoods.Go.GoInputState)" /> before
		/// finishing the transaction.
		/// </summary>
		public override void DoMouseUp()
		{
			myModalDropped = true;
			base.LastInput.InputState = GoInputState.Finish;
			if (base.View.DoSelectionDropReject(base.LastInput))
			{
				DoCancelMouse();
				DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
				if (dragEventArgs != null && base.View.ExternalDragDropsOnEnter && !base.View.IsInternalDragDrop(dragEventArgs))
				{
					base.View.DeleteSelection(base.View.Selection);
				}
				return;
			}
			bool num = MustBeCopying() && MayBeCopying();
			bool flag = MayBeMoving();
			DoDragging(GoInputState.Finish);
			base.View.DoSelectionDropped(base.LastInput);
			if (num)
			{
				base.TransactionResult = "Copy Selection";
				base.View.RaiseSelectionCopied();
				StopTool();
			}
			else if (flag)
			{
				base.TransactionResult = "Move Selection";
				base.View.RaiseSelectionMoved();
				StopTool();
			}
			else
			{
				DoCancelMouse();
			}
			DoMouseOver(base.LastInput);
		}

		/// <summary>
		/// Cancelling a drag involves moving the selection back to the original position
		/// before aborting the transaction.
		/// </summary>
		public override void DoCancelMouse()
		{
			if (base.CurrentObject != null && DragSelection == null)
			{
				SizeF offset = GoTool.SubtractPoints(GoTool.SubtractPoints(base.FirstInput.DocPoint, MoveOffset), base.CurrentObject.Position);
				if (offset.Width != 0f || offset.Height != 0f)
				{
					base.View.MoveSelection((EffectiveSelection != null) ? EffectiveSelection : base.Selection, offset, grid: false);
				}
			}
			base.TransactionResult = null;
			StopTool();
		}

		/// <summary>
		/// This predicate is true when the view allows objects to be copied and inserted,
		/// and some object in the Selection is copyable.
		/// </summary>
		/// <returns></returns>
		public virtual bool MayBeCopying()
		{
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null && (dragEventArgs.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy)
			{
				return false;
			}
			if (!base.View.CanInsertObjects())
			{
				return false;
			}
			if (!base.View.CanCopyObjects())
			{
				return false;
			}
			foreach (GoObject item in base.Selection)
			{
				if (item.CanCopy())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This predicate is true when the user is trying to force a copy by holding
		/// down the Control modifier.
		/// </summary>
		/// <returns></returns>
		public virtual bool MustBeCopying()
		{
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null && (dragEventArgs.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy)
			{
				return false;
			}
			if (base.LastInput.Control)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// This predicate is true when the view allows objects to be moved,
		/// and some object in the Selection is movable.
		/// </summary>
		/// <returns></returns>
		public virtual bool MayBeMoving()
		{
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null && (dragEventArgs.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move && !base.View.PretendsInternalDrag)
			{
				return false;
			}
			if (!base.View.CanMoveObjects())
			{
				return false;
			}
			foreach (GoObject item in base.Selection)
			{
				if (item.CanMove())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This predicate is true when the user is trying to force a move by holding
		/// down the Shift modifier.
		/// </summary>
		/// <returns></returns>
		public virtual bool MustBeMoving()
		{
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null && (dragEventArgs.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move && !base.View.PretendsInternalDrag)
			{
				return false;
			}
			if (base.LastInput.Shift)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Perform the drag, for both moving and copying, including the final move or copy
		/// on a mouse up event.
		/// </summary>
		/// <param name="evttype"></param>
		/// <remarks>
		/// Whether the drag is performing a move or a copy is determined by the
		/// value of the <see cref="M:Northwoods.Go.GoToolDragging.MayBeCopying" /> and <see cref="M:Northwoods.Go.GoToolDragging.MayBeMoving" /> predicates.
		/// This method is sensitive to the <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> property.
		/// When this property is false, dragging uses the <see cref="P:Northwoods.Go.GoToolDragging.DragSelection" />
		/// selection instead of the normal <see cref="P:Northwoods.Go.GoView.Selection" /> collection.
		/// It calls <see cref="M:Northwoods.Go.GoToolDragging.MakeDragSelection" /> to create the drag selection if needed
		/// and then moves the drag selection.
		/// When not copying and when <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> is true, it calls
		/// <see cref="M:Northwoods.Go.GoToolDragging.ClearDragSelection" /> to stop using any drag selection and then it
		/// moves the regular <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		public virtual void DoDragging(GoInputState evttype)
		{
			if (base.CurrentObject == null)
			{
				return;
			}
			SizeF sizeF = GoTool.SubtractPoints(base.LastInput.DocPoint, base.CurrentObject.Position);
			SizeF sizeF2 = new SizeF(sizeF.Width - MoveOffset.Width, sizeF.Height - MoveOffset.Height);
			bool flag = MayBeCopying() && MustBeCopying();
			bool flag2 = MayBeMoving();
			DragEventArgs dragEventArgs = base.LastInput.DragEventArgs;
			if (dragEventArgs != null && !base.View.IsInternalDragDrop(dragEventArgs))
			{
				flag = false;
			}
			if (EffectiveSelection == null)
			{
				myEffectiveSelection = ComputeEffectiveSelection(base.Selection, !flag);
			}
			if (evttype != GoInputState.Finish)
			{
				GoSelection goSelection = null;
				if (flag || !base.View.DragsRealtime)
				{
					MakeDragSelection();
					goSelection = DragSelection;
				}
				else if (flag2)
				{
					ClearDragSelection();
					goSelection = EffectiveSelection;
				}
				if (goSelection != null)
				{
					GoObject goObject = null;
					foreach (GoObject item in goSelection)
					{
						if (!(item is IGoLink) && item.CanMove())
						{
							goObject = item;
							break;
						}
					}
					SizeF offset = sizeF2;
					if (goObject != null)
					{
						PointF location = goObject.Location;
						PointF p = new PointF(location.X + sizeF2.Width, location.Y + sizeF2.Height);
						p = base.View.SnapPoint(p, goObject);
						offset.Width = p.X - location.X;
						offset.Height = p.Y - location.Y;
					}
					base.View.MoveSelection(goSelection, offset, grid: false);
				}
				return;
			}
			SizeF offset2 = sizeF2;
			if (DragSelection != null)
			{
				offset2 = GoTool.SubtractPoints(base.CurrentObject.Position, DragSelectionOriginalObject.Position);
				ClearDragSelection();
			}
			if (flag)
			{
				if (CopiesEffectiveSelection)
				{
					myEffectiveSelection = ComputeEffectiveSelection(base.Selection, move: false);
					base.View.CopySelection(EffectiveSelection, offset2, grid: true);
				}
				else
				{
					base.View.CopySelection(base.Selection, offset2, grid: true);
				}
			}
			else if (flag2)
			{
				if (EffectiveSelection == null)
				{
					myEffectiveSelection = ComputeEffectiveSelection(base.Selection, move: true);
				}
				base.View.MoveSelection(EffectiveSelection, offset2, grid: true);
			}
		}

		/// <summary>
		/// Produce a new <see cref="T:Northwoods.Go.GoSelection" /> that is the real set of objects
		/// to be moved by <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> or copied by
		/// <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="move">true for moving, false for copying</param>
		/// <returns>a <see cref="T:Northwoods.Go.GoSelection" /> that is cached as <see cref="P:Northwoods.Go.GoToolDragging.EffectiveSelection" /></returns>
		/// <remarks>
		/// This method is used to try to avoid problems with double-moving
		/// due to duplicate entries or both a parent and its child being in
		/// the argument collection.
		/// This also removes objects whose <see cref="P:Northwoods.Go.GoObject.DraggingObject" />
		/// is null or has a false value for <see cref="M:Northwoods.Go.GoObject.CanMove" /> (if
		/// <paramref name="move" /> is true) or a false value for <see cref="M:Northwoods.Go.GoObject.CanCopy" />
		/// (if <paramref name="move" /> is false).
		/// Furthermore this adds to the collection all links that have both
		/// ports in the selection.
		/// </remarks>
		public virtual GoSelection ComputeEffectiveSelection(IGoCollection coll, bool move)
		{
			Dictionary<GoObject, bool> dictionary = new Dictionary<GoObject, bool>();
			GoCollection goCollection = null;
			GoSelection goSelection = new GoSelection(null);
			foreach (GoObject item in coll)
			{
				GoObject draggingObject = item.DraggingObject;
				if (draggingObject != null && !(move ? (!draggingObject.CanMove()) : (!draggingObject.CanCopy())) && !alreadyDragged(dictionary, draggingObject))
				{
					dictionary[draggingObject] = true;
					if (!draggingObject.IsTopLevel)
					{
						if (goCollection == null)
						{
							goCollection = new GoCollection();
						}
						goCollection.Add(draggingObject);
					}
					goSelection.Add(draggingObject);
				}
			}
			if (EffectiveSelectionIncludesLinks)
			{
				GoObject[] array = goSelection.CopyArray();
				for (int i = 0; i < array.Length; i++)
				{
					IGoNode goNode = array[i] as IGoNode;
					if (goNode != null)
					{
						foreach (IGoLink destinationLink in goNode.DestinationLinks)
						{
							if (!alreadyDragged(dictionary, destinationLink.GoObject) && (destinationLink.ToPort == null || alreadyDragged(dictionary, destinationLink.ToPort.GoObject)))
							{
								dictionary[destinationLink.GoObject] = true;
								goSelection.Add(destinationLink.GoObject);
							}
						}
						foreach (IGoLink sourceLink in goNode.SourceLinks)
						{
							if (!alreadyDragged(dictionary, sourceLink.GoObject) && (sourceLink.FromPort == null || alreadyDragged(dictionary, sourceLink.FromPort.GoObject)))
							{
								dictionary[sourceLink.GoObject] = true;
								goSelection.Add(sourceLink.GoObject);
							}
						}
					}
				}
			}
			if (goCollection != null)
			{
				GoCollection goCollection2 = null;
				foreach (GoObject item2 in goSelection)
				{
					GoObject draggingObject2 = item2.DraggingObject;
					if (draggingObject2 is GoGroup)
					{
						foreach (GoObject item3 in goCollection)
						{
							if (item3.IsChildOf(draggingObject2))
							{
								if (goCollection2 == null)
								{
									goCollection2 = new GoCollection();
								}
								goCollection2.Add(item3);
							}
						}
					}
				}
				if (goCollection2 != null)
				{
					foreach (GoObject item4 in goCollection2)
					{
						goSelection.Remove(item4);
					}
					return goSelection;
				}
			}
			return goSelection;
		}

		private bool alreadyDragged(Dictionary<GoObject, bool> draggeds, GoObject o)
		{
			for (GoObject goObject = o; goObject != null; goObject = goObject.Parent)
			{
				if (draggeds.ContainsKey(goObject))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Create a new selection object containing an image of all of the real selected objects.
		/// </summary>
		/// <returns>a new <see cref="T:Northwoods.Go.GoSelection" /> holding view objects that represent the
		/// objects in the <see cref="P:Northwoods.Go.GoView.Selection" /></returns>
		/// <remarks>
		/// This creates a new <see cref="T:Northwoods.Go.GoSelection" /> for this view.
		/// The objects that are in this selection have been added to the default
		/// layer of the view.
		/// </remarks>
		public virtual GoSelection CreateDragSelection()
		{
			GoSelection goSelection = new GoSelection(null);
			PointF position = base.CurrentObject.Position;
			SizeF offset = GoTool.SubtractPoints(base.CurrentObject.Location, position);
			GoDragRectangle goDragRectangle = new GoDragRectangle();
			goDragRectangle.Bounds = base.CurrentObject.Bounds;
			goDragRectangle.Offset = offset;
			goDragRectangle.Visible = false;
			base.View.Layers.Default.Add(goDragRectangle);
			goSelection.Add(goDragRectangle);
			GoCollection goCollection = new GoCollection();
			goCollection.InternalChecksForDuplicates = false;
			foreach (GoObject item in (EffectiveSelection != null) ? EffectiveSelection : base.Selection)
			{
				goCollection.Add(item.DraggingObject);
			}
			base.View.Document.Layers.SortByZOrder(goCollection);
			RectangleF bounds = GoDocument.ComputeBounds(goCollection, base.View);
			float num = 1E+21f;
			float num2 = 1E+21f;
			foreach (GoObject item2 in goCollection)
			{
				if (item2.Top < num)
				{
					num = item2.Top;
				}
				if (item2.Left < num2)
				{
					num2 = item2.Left;
				}
			}
			float num3 = base.View.WorldScale.Width;
			if (bounds.Width * num3 > 2000f || bounds.Height * num3 > 2000f)
			{
				num3 *= Math.Min(2000f / (bounds.Width * num3), 2000f / (bounds.Height * num3));
			}
			Bitmap bitmapFromCollection = base.View.GetBitmapFromCollection(goCollection, bounds, num3, paper: false);
			GoDragImage goDragImage = new GoDragImage();
			goDragImage.Image = bitmapFromCollection;
			goDragImage.Bounds = new RectangleF(bounds.X, bounds.Y, (float)bitmapFromCollection.Width / num3, (float)bitmapFromCollection.Height / num3);
			if (num < 1E+21f && num2 < 1E+21f)
			{
				goDragImage.Offset = new SizeF(num2 - bounds.X + offset.Width, num - bounds.Y + offset.Height);
			}
			else
			{
				goDragImage.Offset = offset;
			}
			base.View.Layers.Default.Add(goDragImage);
			goSelection.Add(goDragImage);
			return goSelection;
		}

		/// <summary>
		/// Start using a <see cref="P:Northwoods.Go.GoToolDragging.DragSelection" />, creating it if needed, and moving the
		/// originally selected objects back to their original position.
		/// </summary>
		/// <remarks>
		/// This also sets <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> to a corresponding
		/// outline object in the drag selection, and remembers the original current
		/// object in the <see cref="P:Northwoods.Go.GoToolDragging.DragSelectionOriginalObject" /> property.  This
		/// allows <see cref="M:Northwoods.Go.GoToolDragging.DoDragging(Northwoods.Go.GoInputState)" />to continue calculating move offsets based
		/// on the position of the <see cref="P:Northwoods.Go.GoTool.CurrentObject" />, even though 
		/// the objects are part of the drag selection instead of the
		/// original selected objects.
		/// </remarks>
		public virtual void MakeDragSelection()
		{
			if (DragSelection != null)
			{
				return;
			}
			DragSelectionOriginalObject = base.CurrentObject;
			DragSelection = CreateDragSelection();
			if (DragSelection == null || DragSelection.IsEmpty)
			{
				DragSelectionOriginalObject = null;
				DragSelection = null;
				return;
			}
			SizeF offset = GoTool.SubtractPoints(GoTool.SubtractPoints(base.FirstInput.DocPoint, MoveOffset), DragSelectionOriginalObject.Position);
			if (offset.Width != 0f || offset.Height != 0f)
			{
				base.View.MoveSelection((EffectiveSelection != null) ? EffectiveSelection : base.Selection, offset, grid: false);
			}
			if (base.CurrentObject.View != base.View)
			{
				base.CurrentObject = DragSelection.Primary;
			}
		}

		/// <summary>
		/// Stop using the <see cref="P:Northwoods.Go.GoToolDragging.DragSelection" />.
		/// </summary>
		/// <remarks>
		/// Remove all of the objects from the <see cref="P:Northwoods.Go.GoToolDragging.DragSelection" /> from the view,
		/// clear the <see cref="P:Northwoods.Go.GoToolDragging.DragSelection" /> property, and
		/// set the <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> back to the original current object.
		/// </remarks>
		public virtual void ClearDragSelection()
		{
			if (DragSelection != null)
			{
				foreach (GoObject item in DragSelection)
				{
					item.Remove();
				}
				DragSelection = null;
				base.CurrentObject = DragSelectionOriginalObject;
				DragSelectionOriginalObject = null;
			}
		}
	}
}
