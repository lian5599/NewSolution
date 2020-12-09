using System;
using System.Collections.Generic;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// The selection is a collection of <see cref="T:Northwoods.Go.GoObject" />s that the user
	/// can manipulate in a <see cref="T:Northwoods.Go.GoView" />.
	/// </summary>
	/// <remarks>
	/// You should use the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.Selection" />
	/// to display and manipulate the selection of objects that the user sees.
	/// Occasionally you may need to have a separate collection of objects that
	/// is a <see cref="T:Northwoods.Go.GoSelection" />, in which case you may want to call the
	/// constructor with a null/Nothing argument.
	/// </remarks>
	[Serializable]
	public class GoSelection : GoCollection
	{
		private static readonly IEnumerable<IGoHandle> myEmptyList = new List<IGoHandle>().AsReadOnly();

		[NonSerialized]
		private GoView myView;

		private Dictionary<GoObject, bool> myObjTable = new Dictionary<GoObject, bool>();

		private SizeF myHotSpot;

		[NonSerialized]
		private Dictionary<GoObject, object> myHandles;

		[NonSerialized]
		private Pen myBoundingHandlePen;

		[NonSerialized]
		private Pen myResizeHandlePen;

		private Color myResizeHandlePenColor = Color.Black;

		[NonSerialized]
		private SolidBrush myResizeHandleBrush;

		[NonSerialized]
		private bool myFocused = true;

		/// <summary>
		/// Gets the view that maintains this selection collection.
		/// </summary>
		public GoView View => myView;

		/// <summary>
		/// Gets the first object in this selection.
		/// </summary>
		/// <value>
		/// If this selection is empty, this value is null.
		/// </value>
		public virtual GoObject Primary => First;

		/// <summary>
		/// Gets or sets the offset for where the mouse pointer should be,
		/// relative to the <see cref="P:Northwoods.Go.GoObject.Position" /> of the <see cref="P:Northwoods.Go.GoSelection.Primary" /> selection.
		/// </summary>
		/// <remarks>
		/// This is used by <see cref="M:Northwoods.Go.GoView.GetExternalDragImage(System.Windows.Forms.DragEventArgs)" /> and <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" />
		/// to decide how to position the selection relative to the mouse.
		/// </remarks>
		public virtual SizeF HotSpot
		{
			get
			{
				return myHotSpot;
			}
			set
			{
				myHotSpot = value;
			}
		}

		/// <summary>
		/// Gets whether this selection thinks its view has gotten focus.
		/// </summary>
		public virtual bool Focused
		{
			get
			{
				return myFocused;
			}
			set
			{
				myFocused = value;
			}
		}

		/// <summary>
		/// Create an empty collection of objects representing the user's selection for a view.
		/// </summary>
		/// <param name="view">
		/// if the view is not null/Nothing, this will automatically create selection handles
		/// for document objects as they are <see cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" />'ed to this collection
		/// </param>
		/// <remarks>
		/// The only normal use of this constructor with a non-null argument should be by
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.CreateSelection" />.
		/// If you do create a <see cref="T:Northwoods.Go.GoSelection" /> with a non-null <paramref name="view" />
		/// that is not the value of that view's <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.Selection" />,
		/// you need to be responsible for maintaining the selection handles in the view.
		/// For example, a change in the bounds of an object will not automatically update the
		/// handles that are in any <see cref="T:Northwoods.Go.GoSelection" /> but the one that is the
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		public GoSelection(GoView view)
		{
			myView = view;
			base.InternalChecksForDuplicates = false;
		}

		internal void SetView(GoView v)
		{
			myView = v;
		}

		/// <summary>
		/// Add an object to this selection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// If the <paramref name="obj" /> is already part of this selection,
		/// nothing happens.
		/// Otherwise this method calls <see cref="M:Northwoods.Go.GoObject.OnGotSelection(Northwoods.Go.GoSelection)" />
		/// and raises the <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> event by
		/// calling <see cref="M:Northwoods.Go.GoView.RaiseObjectGotSelection(Northwoods.Go.GoObject)" />.
		/// No notifications occur if there is no <see cref="P:Northwoods.Go.GoSelection.View" /> for this selection.
		/// If adding an object would increase the <see cref="P:Northwoods.Go.GoCollection.Count" /> beyond
		/// the <see cref="P:Northwoods.Go.GoView.MaximumSelectionCount" />, this collection is not augmented.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.Remove(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoSelection.Contains(Northwoods.Go.GoObject)" />
		public override void Add(GoObject obj)
		{
			if (obj == null)
			{
				return;
			}
			GoView view = View;
			if ((view == null || view.Selection != this || Count < view.MaximumSelectionCount) && !Contains(obj))
			{
				if (view != null && obj.Document != view.Document && obj.View != view)
				{
					throw new ArgumentException("Selected objects must belong to the view or its document");
				}
				addToSelection(obj);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.RaiseSelectionStarting" /> and <see cref="M:Northwoods.Go.GoView.RaiseSelectionFinished" />
		/// around the addition of all of the objects in the given collection to this selection.
		/// </summary>
		/// <param name="coll"></param>
		public override void AddRange(IGoCollection coll)
		{
			if (coll != null && !coll.IsEmpty)
			{
				GoView view = View;
				view?.RaiseSelectionStarting();
				base.AddRange(coll);
				view?.RaiseSelectionFinished();
			}
		}

		/// <summary>
		/// Remove an object from this selection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// If the <paramref name="obj" /> is not part of this selection,
		/// nothing happens.
		/// Otherwise this method calls <see cref="M:Northwoods.Go.GoObject.OnLostSelection(Northwoods.Go.GoSelection)" />
		/// and raises the <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" /> event by
		/// calling <see cref="M:Northwoods.Go.GoView.RaiseObjectLostSelection(Northwoods.Go.GoObject)" />.
		/// The notification happens after the object has been removed from
		/// this selection.
		/// Removing the primary selection may cause another selected object to lose
		/// selection and then gain it back again as the primary selection.
		/// No notifications occur if there is no <see cref="P:Northwoods.Go.GoSelection.View" /> for this selection.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" />
		public override bool Remove(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (Contains(obj))
			{
				removeFromSelection(obj);
				return true;
			}
			RemoveHandles(obj);
			return false;
		}

		/// <summary>
		/// Determine if an object is part of this selection.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// The default implementation uses a hashtable to decide quickly.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" />
		public override bool Contains(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			return myObjTable.ContainsKey(obj);
		}

		/// <summary>
		/// Clearing the selection raises the <see cref="T:Northwoods.Go.GoView" />.<see cref="E:Northwoods.Go.GoView.SelectionStarting" />
		/// and <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events, if there were multiple objects selected.
		/// </summary>
		public override void Clear()
		{
			GoView view = View;
			bool num = view != null && Count > 1;
			if (num)
			{
				view.RaiseSelectionStarting();
			}
			base.Clear();
			if (num)
			{
				view.RaiseSelectionFinished();
			}
		}

		/// <summary>
		/// Make the given object the one and only selected object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns><paramref name="obj" /></returns>
		/// <remarks>
		/// This will clear any existing selection and then add the single given object.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" />
		public virtual GoObject Select(GoObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (Primary == obj && Count == 1)
			{
				return obj;
			}
			Clear();
			Add(obj);
			return obj;
		}

		/// <summary>
		/// If the given object is part of this selection, remove it; otherwise add it.
		/// </summary>
		/// <param name="obj"></param>
		/// <seealso cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoSelection.Remove(Northwoods.Go.GoObject)" />
		public virtual void Toggle(GoObject obj)
		{
			if (obj != null)
			{
				if (Contains(obj))
				{
					Remove(obj);
				}
				else
				{
					Add(obj);
				}
			}
		}

		private void addToSelection(GoObject obj)
		{
			GoObject primary = Primary;
			base.Add(obj);
			myObjTable[obj] = true;
			GoView view = View;
			if (view != null)
			{
				if (obj.IsInDocument)
				{
					obj.OnGotSelection(this);
				}
				view.RaiseObjectGotSelection(obj);
				if (primary != null && Primary != primary && primary.IsInDocument)
				{
					primary.OnLostSelection(this);
					view.RaiseObjectLostSelection(primary);
					primary.OnGotSelection(this);
					view.RaiseObjectGotSelection(primary);
				}
			}
		}

		private void removeFromSelection(GoObject obj)
		{
			GoObject primary = Primary;
			myObjTable.Remove(obj);
			base.Remove(obj);
			GoView view = View;
			if (view == null)
			{
				return;
			}
			if (obj.IsInDocument)
			{
				obj.OnLostSelection(this);
			}
			view.RaiseObjectLostSelection(obj);
			if (primary == obj && primary.IsInDocument)
			{
				primary = Primary;
				if (primary != null)
				{
					primary.OnLostSelection(this);
					view.RaiseObjectLostSelection(primary);
					primary.OnGotSelection(this);
					view.RaiseObjectGotSelection(primary);
				}
			}
		}

		/// <summary>
		/// Create and determine the appearance of a large handle around an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="selectedObj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This method uses <see cref="M:Northwoods.Go.GoObject.CreateBoundingHandle" /> to
		/// actually allocate the handle and to set the size and location of the
		/// handle, as determined by the bounding rectangle of <paramref name="obj" />.
		/// The pen of the handle is determined by
		/// <see cref="P:Northwoods.Go.GoView.PrimarySelectionColor" /> and
		/// <see cref="P:Northwoods.Go.GoView.SecondarySelectionColor" />; the brush is set to null.
		/// The new handle is associated with the <paramref name="obj" /> and its
		/// <see cref="P:Northwoods.Go.IGoHandle.SelectedObject" /> property is set to
		/// <paramref name="selectedObj" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.CreateResizeHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Drawing.PointF,System.Int32,System.Boolean)" />
		public virtual IGoHandle CreateBoundingHandle(GoObject obj, GoObject selectedObj)
		{
			IGoHandle goHandle = obj.CreateBoundingHandle();
			if (goHandle == null)
			{
				return null;
			}
			goHandle.SelectedObject = selectedObj;
			GoObject goObject = goHandle.GoObject;
			if (goObject == null)
			{
				return null;
			}
			goObject.Selectable = false;
			GoShape goShape = goObject as GoShape;
			if (goShape != null)
			{
				Color color = Color.LightGray;
				GoView view = View;
				if (view != null)
				{
					color = ((!Focused) ? view.NoFocusSelectionColor : ((Primary == null || Primary.SelectionObject != obj) ? view.SecondarySelectionColor : view.PrimarySelectionColor));
				}
				float boundingHandlePenWidth = view.BoundingHandlePenWidth;
				float num = (boundingHandlePenWidth == 0f) ? 0f : (boundingHandlePenWidth / view.WorldScale.Width);
				if (myBoundingHandlePen == null || GoShape.GetPenColor(myBoundingHandlePen, color) != color || GoShape.GetPenWidth(myBoundingHandlePen) != num)
				{
					myBoundingHandlePen = GoShape.NewPen(color, num);
				}
				goShape.Pen = myBoundingHandlePen;
				goShape.Brush = null;
			}
			AddHandle(obj, goHandle);
			return goHandle;
		}

		/// <summary>
		/// Create and determine the appearance of a small handle for an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="selectedObj"></param>
		/// <param name="loc"></param>
		/// <param name="handleid"></param>
		/// <param name="filled"></param>
		/// <returns></returns>
		/// <remarks>
		/// This method uses <see cref="M:Northwoods.Go.GoObject.CreateResizeHandle(System.Int32)" /> to
		/// actually allocate the handle.
		/// The size of the handle, if not already non-zero, is determined by
		/// <see cref="P:Northwoods.Go.GoView.ResizeHandleSize" />.
		/// The pen and brush of the handle are determined by
		/// <see cref="P:Northwoods.Go.GoView.PrimarySelectionColor" /> and <see cref="P:Northwoods.Go.GoView.SecondarySelectionColor" />.
		/// The new handle is associated with the <paramref name="obj" /> and its
		/// <see cref="P:Northwoods.Go.IGoHandle.SelectedObject" /> property is set to <paramref name="selectedObj" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.CreateBoundingHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		public virtual IGoHandle CreateResizeHandle(GoObject obj, GoObject selectedObj, PointF loc, int handleid, bool filled)
		{
			IGoHandle goHandle = obj.CreateResizeHandle(handleid);
			if (goHandle == null)
			{
				return null;
			}
			goHandle.HandleID = handleid;
			goHandle.SelectedObject = selectedObj;
			GoObject goObject = goHandle.GoObject;
			if (goObject == null)
			{
				return null;
			}
			GoView view = View;
			SizeF sizeF = goObject.Size;
			if (sizeF.Width <= 0f || sizeF.Height <= 0f)
			{
				sizeF = (view?.ResizeHandleSize ?? new SizeF(6f, 6f));
			}
			if (view != null)
			{
				sizeF.Width /= view.WorldScale.Width;
				sizeF.Height /= view.WorldScale.Height;
			}
			goObject.Bounds = new RectangleF(loc.X - sizeF.Width / 2f, loc.Y - sizeF.Height / 2f, sizeF.Width, sizeF.Height);
			if (handleid == 0)
			{
				goObject.Selectable = false;
			}
			else
			{
				goObject.Selectable = true;
			}
			GoShape goShape = goObject as GoShape;
			if (goShape != null)
			{
				Color color = Color.LightGray;
				if (view != null)
				{
					color = ((!Focused) ? view.NoFocusSelectionColor : ((Primary == null || Primary.SelectionObject != obj) ? view.SecondarySelectionColor : view.PrimarySelectionColor));
				}
				if (filled)
				{
					float resizeHandlePenWidth = view.ResizeHandlePenWidth;
					float num = (resizeHandlePenWidth == 0f) ? 0f : (resizeHandlePenWidth / view.WorldScale.Width);
					if (myResizeHandlePen == null || GoShape.GetPenColor(myResizeHandlePen, myResizeHandlePenColor) != myResizeHandlePenColor || GoShape.GetPenWidth(myResizeHandlePen) != num)
					{
						myResizeHandlePen = GoShape.NewPen(myResizeHandlePenColor, num);
					}
					goShape.Pen = myResizeHandlePen;
					if (myResizeHandleBrush == null || myResizeHandleBrush.Color != color)
					{
						myResizeHandleBrush = new SolidBrush(color);
					}
					goShape.Brush = myResizeHandleBrush;
				}
				else
				{
					float resizeHandlePenWidth2 = view.ResizeHandlePenWidth;
					float num2 = (resizeHandlePenWidth2 == 0f) ? 0f : ((resizeHandlePenWidth2 + 1f) / view.WorldScale.Width);
					if (myResizeHandlePen == null || GoShape.GetPenColor(myResizeHandlePen, color) != color || GoShape.GetPenWidth(myResizeHandlePen) != num2)
					{
						myResizeHandlePen = GoShape.NewPen(color, num2);
					}
					goShape.Pen = myResizeHandlePen;
					goShape.Brush = null;
				}
			}
			AddHandle(obj, goHandle);
			return goHandle;
		}

		/// <summary>
		/// Associate a handle with an object in this selection.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="handle"></param>
		/// <remarks>
		/// This method also adds the <paramref name="handle" />'s
		/// <see cref="P:Northwoods.Go.IGoHandle.GoObject" /> to the view's default layer.
		/// This method is called by <see cref="M:Northwoods.Go.GoSelection.CreateResizeHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Drawing.PointF,System.Int32,System.Boolean)" /> and <see cref="M:Northwoods.Go.GoSelection.CreateBoundingHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.RemoveHandles(Northwoods.Go.GoObject)" />
		public virtual void AddHandle(GoObject obj, IGoHandle handle)
		{
			if (myHandles == null)
			{
				myHandles = new Dictionary<GoObject, object>();
			}
			myHandles.TryGetValue(obj, out object value);
			if (value == null)
			{
				myHandles[obj] = handle;
			}
			else if (value is List<IGoHandle>)
			{
				((List<IGoHandle>)value).Add(handle);
			}
			else
			{
				List<IGoHandle> list = new List<IGoHandle>();
				list.Add((IGoHandle)value);
				list.Add(handle);
				myHandles[obj] = list;
			}
			if (View != null)
			{
				View.Layers.Default.Add(handle.GoObject);
			}
		}

		/// <summary>
		/// Remove all handles associated with an object in this selection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Each handle for the <paramref name="obj" /> 
		/// has its <see cref="P:Northwoods.Go.IGoHandle.SelectedObject" /> property
		/// set to null and its <see cref="P:Northwoods.Go.IGoHandle.GoObject" />
		/// removed from its view layer.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.AddHandle(Northwoods.Go.GoObject,Northwoods.Go.IGoHandle)" />
		public virtual void RemoveHandles(GoObject obj)
		{
			if (myHandles == null)
			{
				return;
			}
			myHandles.TryGetValue(obj, out object value);
			if (value == null)
			{
				return;
			}
			if (View != null)
			{
				List<IGoHandle> list = value as List<IGoHandle>;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i = checked(i + 1))
					{
						IGoHandle goHandle = list[i];
						GoObject goObject = goHandle.GoObject;
						goHandle.SelectedObject = null;
						goObject?.Layer?.Remove(goObject);
					}
				}
				else
				{
					IGoHandle obj2 = (IGoHandle)value;
					obj2.SelectedObject = null;
					GoObject goObject2 = obj2.GoObject;
					goObject2?.Layer?.Remove(goObject2);
				}
			}
			myHandles.Remove(obj);
		}

		/// <summary>
		/// Return the number of handles associated with an object in this selection.
		/// </summary>
		/// <param name="obj">the "handled" object, not the "selected" object</param>
		/// <returns></returns>
		/// <seealso cref="M:Northwoods.Go.GoSelection.GetHandleEnumerable(Northwoods.Go.GoObject)" />
		public virtual int GetHandleCount(GoObject obj)
		{
			if (myHandles == null)
			{
				return 0;
			}
			myHandles.TryGetValue(obj, out object value);
			if (value == null)
			{
				return 0;
			}
			if (value is List<IGoHandle>)
			{
				return ((List<IGoHandle>)value).Count;
			}
			return 1;
		}

		/// <summary>
		/// Return one of the handles associated with an object in this selection.
		/// </summary>
		/// <param name="obj">the "handled" object, not the "selected" object</param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoHandle" />, or null if there are none for <paramref name="obj" />.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.GoSelection.GetHandleEnumerable(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoSelection.FindHandleByID(Northwoods.Go.GoObject,System.Int32)" />
		public virtual IGoHandle GetAnExistingHandle(GoObject obj)
		{
			if (myHandles == null)
			{
				return null;
			}
			myHandles.TryGetValue(obj, out object value);
			if (value == null)
			{
				return null;
			}
			if (value is List<IGoHandle>)
			{
				List<IGoHandle> list = (List<IGoHandle>)value;
				if (list.Count > 0)
				{
					return list[0];
				}
				return null;
			}
			return (IGoHandle)value;
		}

		/// <summary>
		/// Return an enumerable collection of the selection handles for an object.
		/// </summary>
		/// <param name="obj">an object that may have selection handles; the "handled" object, not the "selected" object</param>
		/// <returns>an <c>IEnumerable</c> of <see cref="T:Northwoods.Go.IGoHandle" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoSelection.AddHandle(Northwoods.Go.GoObject,Northwoods.Go.IGoHandle)" />
		/// <seealso cref="M:Northwoods.Go.GoSelection.GetHandleCount(Northwoods.Go.GoObject)" />
		public virtual IEnumerable<IGoHandle> GetHandleEnumerable(GoObject obj)
		{
			List<IGoHandle> list;
			if (myHandles == null)
			{
				list = (List<IGoHandle>)myEmptyList;
			}
			else
			{
				myHandles.TryGetValue(obj, out object value);
				if (value == null)
				{
					list = new List<IGoHandle>();
				}
				else if (value is List<IGoHandle>)
				{
					list = (List<IGoHandle>)value;
				}
				else
				{
					list = new List<IGoHandle>();
					list.Add((IGoHandle)value);
				}
			}
			return list;
		}

		/// <summary>
		/// Find a particular selection handle for an object, given its <see cref="P:Northwoods.Go.IGoHandle.HandleID" />.
		/// </summary>
		/// <param name="obj">an object that may have selection handles; the "handled" object, not the "selected" object</param>
		/// <param name="id">the <see cref="P:Northwoods.Go.IGoHandle.HandleID" /> to look for</param>
		/// <returns>
		/// An <see cref="T:Northwoods.Go.IGoHandle" /> that has the given ID, or null if no such handle is found.
		/// Note that if there is more than one such handle with the given ID, this will just return
		/// the first one it finds.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.GoSelection.GetHandleEnumerable(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoSelection.AddHandle(Northwoods.Go.GoObject,Northwoods.Go.IGoHandle)" />
		public virtual IGoHandle FindHandleByID(GoObject obj, int id)
		{
			if (myHandles == null)
			{
				return null;
			}
			myHandles.TryGetValue(obj, out object value);
			if (value == null)
			{
				return null;
			}
			if (value is List<IGoHandle>)
			{
				foreach (IGoHandle item in (List<IGoHandle>)value)
				{
					if (item.HandleID == id)
					{
						return item;
					}
				}
				return null;
			}
			IGoHandle goHandle = (IGoHandle)value;
			if (goHandle.HandleID == id)
			{
				return goHandle;
			}
			return null;
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoObject.RemoveSelectionHandles(Northwoods.Go.GoSelection)" /> on the
		/// <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> of each selected object.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoSelection.AddAllSelectionHandles" />
		public void RemoveAllSelectionHandles()
		{
			foreach (GoObject backward in Backwards)
			{
				backward.SelectionObject?.RemoveSelectionHandles(this);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoObject.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> on the
		/// <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> of each selected object
		/// if <see cref="M:Northwoods.Go.GoObject.CanView" /> is true, or call
		/// <see cref="M:Northwoods.Go.GoObject.RemoveSelectionHandles(Northwoods.Go.GoSelection)" /> otherwise.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoSelection.RemoveAllSelectionHandles" />
		public void AddAllSelectionHandles()
		{
			using (GoCollectionEnumerator goCollectionEnumerator = GetEnumerator())
			{
				while (goCollectionEnumerator.MoveNext())
				{
					GoObject current = goCollectionEnumerator.Current;
					GoObject selectionObject = current.SelectionObject;
					if (selectionObject != null)
					{
						if (current.CanView())
						{
							selectionObject.AddSelectionHandles(this, current);
						}
						else
						{
							selectionObject.RemoveSelectionHandles(this);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoView.OnGotFocus(System.EventArgs)" /> to make
		/// sure all selection handles appear normally.
		/// </summary>
		/// <remarks>
		/// This implementation currently just calls <see cref="M:Northwoods.Go.GoSelection.AddAllSelectionHandles" />
		/// if <see cref="P:Northwoods.Go.GoView.HidesSelection" /> is true or if
		/// <see cref="P:Northwoods.Go.GoView.NoFocusSelectionColor" /> is different from
		/// <see cref="P:Northwoods.Go.GoView.PrimarySelectionColor" />.
		/// </remarks>
		public virtual void OnGotFocus()
		{
			myFocused = true;
			if (View != null)
			{
				if (View.HidesSelection)
				{
					AddAllSelectionHandles();
				}
				else if (View.NoFocusSelectionColor != View.PrimarySelectionColor)
				{
					RemoveAllSelectionHandles();
					AddAllSelectionHandles();
				}
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoView.OnLostFocus(System.EventArgs)" /> to make
		/// sure all selection handles either disappear (if
		/// <see cref="P:Northwoods.Go.GoView.HidesSelection" /> is true), or appear with the
		/// <see cref="P:Northwoods.Go.GoView.NoFocusSelectionColor" /> (if different from
		/// <see cref="P:Northwoods.Go.GoView.PrimarySelectionColor" />).
		/// </summary>
		public virtual void OnLostFocus()
		{
			myFocused = false;
			if (View != null)
			{
				if (View.HidesSelection)
				{
					RemoveAllSelectionHandles();
				}
				else if (View.NoFocusSelectionColor != View.PrimarySelectionColor)
				{
					RemoveAllSelectionHandles();
					AddAllSelectionHandles();
				}
			}
		}
	}
}
