using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// Groups are a way of treating a collection of <see cref="T:Northwoods.Go.GoObject" /> instances
	/// as a single object.
	/// </summary>
	[Serializable]
	public class GoGroup : GoObject, IGoCollection, ICollection<GoObject>, IEnumerable<GoObject>, IEnumerable, IList<GoObject>
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int InsertedObject = 1051;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedObject = 1052;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedZOrder = 1053;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ReplacedObject = 1054;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGroup.PickableBackground" /> property.
		/// </summary>
		public const int ChangedPickableBackground = 1055;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int AddedChildName = 1056;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedChildName = 1057;

		private const int flagInvalidPaintBounds = 1048576;

		private const int flagPickableBackground = 2097152;

		private List<GoObject> myObjects = new List<GoObject>();

		private Dictionary<object, object> myNames;

		[NonSerialized]
		private SizeF myPaintBoundsShadowOffset;

		[NonSerialized]
		private float myLeft;

		[NonSerialized]
		private float myTop;

		[NonSerialized]
		private float myRight;

		[NonSerialized]
		private float myBottom;

		/// <summary>
		/// Gets the first child object of this group.
		/// </summary>
		/// <value>
		/// The value may be null if this group is empty.
		/// </value>
		/// <remarks>
		/// The first object will appear behind all other objects in this group.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoGroup.Last" />.
		[Description("The first child object of this group.")]
		public GoObject First
		{
			get
			{
				if (myObjects.Count == 0)
				{
					return null;
				}
				return myObjects[0];
			}
		}

		/// <summary>
		/// Gets the last child object of this group.
		/// </summary>
		/// <value>
		/// The value may be null if this group is empty.
		/// </value>
		/// <remarks>
		/// The last object will appear in front of all other objects in this group.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoGroup.First" />.
		[Description("The last child object of this group.")]
		public GoObject Last
		{
			get
			{
				int count = myObjects.Count;
				if (count == 0)
				{
					return null;
				}
				return myObjects[checked(count - 1)];
			}
		}

		/// <summary>
		/// This predicate is true when there are no objects in this group.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.IsEmpty" />
		[Browsable(false)]
		public virtual bool IsEmpty => Count == 0;

		/// <summary>
		/// Implement the <c>ICollection</c> of <c>GoObject</c> property.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		[Browsable(false)]
		public virtual bool IsReadOnly => false;

		/// <summary>
		/// This <c>IList</c> indexed property gets or sets the object at
		/// a position in the group's array of children.
		/// </summary>
		/// <remarks>
		/// You can also refer to objects by name (a string) if they have
		/// been named, either with a call to <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />
		/// or with an override of <see cref="M:Northwoods.Go.GoGroup.FindChild(System.String)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertBefore(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertAfter(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.RemoveAt(System.Int32)" />
		public virtual GoObject this[int index]
		{
			get
			{
				return myObjects[index];
			}
			set
			{
				if (myObjects[index] != value && value != null)
				{
					if (value.Parent != this)
					{
						replaceAt(index, value, undoing: false);
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of child objects in this group.
		/// </summary>
		[Description("The number of child objects in this group.")]
		public virtual int Count => myObjects.Count;

		/// <summary>
		/// Gets a Dictionary of name/child mappings in both directions.
		/// </summary>
		/// <value>
		/// A <c>Dictionary</c>, but will be null/nothing until you call <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// The dictionary should contain mappings in both directions--
		/// from String name to child GoObject, and the reverse, from object to name.
		/// In order to save space, overrides of <see cref="M:Northwoods.Go.GoGroup.FindName(Northwoods.Go.GoObject)" /> and <see cref="M:Northwoods.Go.GoGroup.FindChild(System.String)" />
		/// will handle common child names rather than using a hash table data structure.
		/// For example, <see cref="T:Northwoods.Go.GoBasicNode" /> provides the name "Label" for its
		/// <see cref="T:Northwoods.Go.GoText" /> part that is the value of its <see cref="P:Northwoods.Go.GoBasicNode.Label" /> property.
		/// </para>
		/// <para>
		/// To modify this hash table, call <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />,
		/// <see cref="M:Northwoods.Go.GoGroup.RemoveChildName(System.String)" />, or <see cref="M:Northwoods.Go.GoGroup.RemoveChildName(Northwoods.Go.GoObject)" />.
		/// If you modify this dictionary directly, such changes are not recorded
		/// by the undo manager.
		/// </para>
		/// </remarks>
		public Dictionary<object, object> ChildNames => myNames;

		/// <summary>
		/// For convenience, you can access named child objects using an indexer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The getter just calls <see cref="M:Northwoods.Go.GoGroup.FindChild(System.String)" />.
		/// The setter just calls <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />.
		/// </para>
		/// <para>
		/// For example:
		/// <pre><code>
		/// GoBasicNode n = ...;
		/// System.Diagnostics.Debug.Assert(n["Label"] == n.Label);
		/// </code></pre>
		/// Remember that you can also refer to a particular child object
		/// by its position (integer index) in this group, since <see cref="T:Northwoods.Go.GoGroup" />
		/// implements <c>IList</c>.
		/// </para>
		/// </remarks>
		public GoObject this[string name]
		{
			get
			{
				return FindChild(name);
			}
			set
			{
				AddChildName(name, value);
			}
		}

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the group's child GoObjects in reverse order,
		/// starting with the last child, which is painted in front of all other children in the group.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.Backwards" />
		[Browsable(false)]
		public GoGroupEnumerator Backwards => new GoGroupEnumerator(myObjects, forward: false);

		IEnumerable<GoObject> IGoCollection.Backwards => new GoGroupEnumerator(myObjects, forward: false);

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoGroup.Pick(System.Drawing.PointF,System.Boolean)" /> returns this node when the tested point
		/// is in the background of this group, not on any of its child objects.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether picking in the background of this node selects the node.")]
		public virtual bool PickableBackground
		{
			get
			{
				return (base.InternalFlags & 0x200000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x200000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 2097152;
					}
					else
					{
						base.InternalFlags &= -2097153;
					}
					Changed(1055, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Produces a copy of this object within the given copy dictionary.
		/// </summary>
		/// <param name="env"></param>
		/// <returns>
		/// The copied group with copies of all of its children.
		/// </returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoGroup.CopyChildren(Northwoods.Go.GoGroup,Northwoods.Go.GoCopyDictionary)" /> to perform the copying.
		/// This temporarily sets <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Initializing" /> to true
		/// during the call to <see cref="M:Northwoods.Go.GoGroup.CopyChildren(Northwoods.Go.GoGroup,Northwoods.Go.GoCopyDictionary)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" />
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoGroup goGroup = (GoGroup)base.CopyObject(env);
			if (goGroup != null)
			{
				goGroup.myObjects = new List<GoObject>();
				goGroup.myNames = null;
				bool initializing = goGroup.Initializing;
				goGroup.Initializing = true;
				CopyChildren(goGroup, env);
				goGroup.Initializing = initializing;
				goGroup.InvalidBounds = true;
				if (myNames != null)
				{
					goGroup.myNames = new Dictionary<object, object>();
					{
						foreach (KeyValuePair<object, object> myName in myNames)
						{
							object key = myName.Key;
							object value = myName.Value;
							if (key is GoObject)
							{
								GoObject key2 = (GoObject)key;
								GoObject key3 = (GoObject)env[key2];
								goGroup.myNames[key3] = value;
							}
							else if (value is GoObject)
							{
								GoObject key4 = (GoObject)value;
								GoObject value2 = (GoObject)env[key4];
								goGroup.myNames[key] = value2;
							}
						}
						return goGroup;
					}
				}
			}
			return goGroup;
		}

		/// <summary>
		/// Copy this group's children.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		/// <remarks>
		/// This method is responsible for copying and adding this group's
		/// child objects to the <paramref name="newgroup" />.
		/// By default, this simply calls
		/// <c>newgroup.Add(env.Copy(obj))</c> for each child <c>obj</c>.
		/// However, your group subclass may want to keep track of some or all
		/// of the children for its own purposes.  To that end you can override
		/// this method to do the copying manually, thereby correctly maintaining
		/// your subclass's internal pointers to children.
		/// You probably should not be calling <see cref="T:Northwoods.Go.GoObject" />'s <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" />,
		/// but <see cref="T:Northwoods.Go.GoCopyDictionary" />'s <see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" />
		/// or indexed lookup instead.
		/// </remarks>
		/// <example>
		/// For example, a group holding an Icon, a Label, an InPort, and an OutPort
		/// might implement this method as follows:
		/// <pre><code>
		/// base.CopyChildren(newgroup, env);
		/// MyNode newnode = (MyNode)newgroup;
		/// newnode.myIcon = (GoObject)env[myIcon];
		/// newnode.myLabel = (GoText)env[myLabel];
		/// newnode.myInPort = (GoPort)env[myInPort];
		/// newnode.myOutPort = (GoPort)env[myOutPort];
		/// </code></pre>
		/// Note that this indexing use of <see cref="T:Northwoods.Go.GoCopyDictionary" /> can handle null
		/// references--it returns null.
		/// </example>
		protected virtual void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			foreach (GoObject item in GetEnumerator())
			{
				GoObject obj = env.Copy(item);
				newgroup.Add(obj);
			}
		}

		/// <summary>
		/// Add an object to this group.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// The <paramref name="obj" /> must not already belong to another group or to a layer.
		/// If the object already belongs to this group, nothing happens.
		/// Otherwise, afterwards, the <paramref name="obj" />'s <see cref="P:Northwoods.Go.GoObject.Parent" />
		/// will be this group and its <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Layer" />
		/// will be the same as this group's layer.
		/// When you add an object to a group, you will normally make that child object
		/// not <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Selectable" />.
		/// If the child is supposed to be selectable, you need to consider whether users
		/// can move or copy the child on its own, or whether this parent group should be
		/// moved or copied instead.  For the latter case you should set the
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.DragsNode" /> property to true for each such child, so
		/// that <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.DraggingObject" /> will return the child's parent node.
		/// If instead you want to allow selected children to be able to move on their own,
		/// you should make sure <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" /> does not control the positioning
		/// of these child objects.
		/// </remarks>
		public virtual void Add(GoObject obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj == this)
			{
				throw new ArgumentException("Cannot add a group to itself");
			}
			GoGroup parent = obj.Parent;
			if (parent == null)
			{
				if (obj.Layer != null)
				{
					throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
				}
				insertAt(myObjects.Count, obj, undoing: false);
			}
			else if (parent != this)
			{
				throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
			}
		}

		private void insertAt(int idx, GoObject obj, bool undoing)
		{
			RectangleF bounds = obj.Bounds;
			if (!undoing || myObjects.IndexOf(obj) < 0)
			{
				if (idx < 0 || idx > myObjects.Count)
				{
					idx = myObjects.Count;
				}
				myObjects.Insert(idx, obj);
			}
			obj.SetParent(this, undoing);
			Changed(1051, 0, null, GoObject.NullRect, idx, obj, bounds);
			if (!undoing)
			{
				LayoutChildren(obj);
				base.InvalidBounds = true;
				_ = Bounds;
			}
		}

		private void moveTo(int newidx, GoObject obj, int oldidx)
		{
			RectangleF bounds = obj.Bounds;
			myObjects.Insert(newidx, obj);
			Changed(1053, oldidx, obj, bounds, newidx, obj, bounds);
		}

		private void replaceAt(int index, GoObject newobj, bool undoing)
		{
			GoObject goObject = myObjects[index];
			goObject.SetBeingRemoved(value: true);
			goObject.SetParent(null, undoing);
			goObject.SetBeingRemoved(value: false);
			myObjects[index] = newobj;
			_ = newobj.Bounds;
			newobj.SetParent(this, undoing);
			Changed(1054, index, goObject, GoObject.NullRect, index, newobj, GoObject.NullRect);
			if (!undoing)
			{
				string text = FindName(goObject);
				if (text != null && text.Length > 0)
				{
					RemoveChildName(goObject);
					AddChildName(text, newobj);
				}
				LayoutChildren(newobj);
				base.InvalidBounds = true;
				_ = Bounds;
			}
		}

		private void removeAt(int index, GoObject obj, bool undoing)
		{
			try
			{
				obj.SetBeingRemoved(value: true);
				if (undoing)
				{
					int num = myObjects.IndexOf(obj);
					if (num >= 0)
					{
						if (index < 0 || index >= myObjects.Count)
						{
							index = num;
						}
						myObjects.RemoveAt(index);
					}
				}
				else
				{
					myObjects.RemoveAt(index);
				}
				RectangleF bounds = obj.Bounds;
				Changed(1052, index, obj, bounds, 0, null, GoObject.NullRect);
				if (!undoing)
				{
					RemoveChildName(obj);
					LayoutChildren(obj);
					base.InvalidBounds = true;
					_ = Bounds;
				}
			}
			catch (Exception ex)
			{
				GoObject.Trace("GoGroup.Remove: " + ex.ToString());
				throw;
			}
			finally
			{
				obj.SetParent(null, undoing);
				obj.SetBeingRemoved(value: false);
			}
		}

		/// <summary>
		/// Add or move an object to be before an existing child.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="newobj"></param>
		/// <remarks>
		/// The children of a group are ordered with respect to each other.
		/// This method makes the <paramref name="newobj" /> be in the group at the position
		/// before the <paramref name="child" />, so that the <paramref name="newobj" />
		/// will be painted immediately before the <paramref name="child" />.
		/// If the <paramref name="newobj" /> already belonged to this group, only its
		/// position in the list is changed.
		/// The <paramref name="newobj" /> must not already be part of a different group or be
		/// a top-level object in some layer.
		/// The existing <paramref name="child" /> must be either null or a member of this
		/// group (but not be the same as <paramref name="newobj" />).
		/// If the <paramref name="child" /> is null, the <paramref name="newobj" />
		/// is added at the beginning of the list, behind all other objects in this group.
		/// </remarks>
		public virtual void InsertBefore(GoObject child, GoObject newobj)
		{
			if (newobj == null)
			{
				return;
			}
			if (child != null && child.Parent != this)
			{
				throw new ArgumentException("Cannot insert an object into a group before (behind) a child that is not a member of the group.");
			}
			GoGroup parent = newobj.Parent;
			if (parent == null)
			{
				if (newobj.Layer != null)
				{
					throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
				}
				int idx = (child != null) ? myObjects.IndexOf(child) : 0;
				insertAt(idx, newobj, undoing: false);
				return;
			}
			if (parent != this)
			{
				throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
			}
			int num = (child != null) ? myObjects.IndexOf(child) : 0;
			int num2 = myObjects.IndexOf(newobj);
			checked
			{
				if (num - 1 != num2 && num != num2)
				{
					myObjects.RemoveAt(num2);
					if (num > num2)
					{
						num--;
					}
					moveTo(num, newobj, num2);
				}
			}
		}

		/// <summary>
		/// Add or move an object to be after an existing child.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="newobj"></param>
		/// <remarks>
		/// The children of a group are ordered with respect to each other.
		/// This method makes the <paramref name="newobj" /> be in the group at the position
		/// after the <paramref name="child" />, so that the <paramref name="newobj" />
		/// will be painted immediately after the <paramref name="child" />.
		/// If the <paramref name="newobj" /> already belonged to this group, only its
		/// position in the list is changed.
		/// The <paramref name="newobj" /> must not already be part of a different group or be
		/// a top-level object in some layer.
		/// The existing <paramref name="child" /> must be either null or a member of this
		/// group (but not be the same as <paramref name="newobj" />).
		/// If the <paramref name="child" /> is null, the <paramref name="newobj" />
		/// is added at the end of the list, in front of all other objects in this group.
		/// </remarks>
		public virtual void InsertAfter(GoObject child, GoObject newobj)
		{
			if (newobj == null)
			{
				return;
			}
			if (child != null && child.Parent != this)
			{
				throw new ArgumentException("Cannot insert an object into a group after a child that is not a member of the group.");
			}
			GoGroup parent = newobj.Parent;
			checked
			{
				if (parent == null)
				{
					if (newobj.Layer != null)
					{
						throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
					}
					int num = (child == null) ? (myObjects.Count - 1) : myObjects.IndexOf(child);
					insertAt(num + 1, newobj, undoing: false);
					return;
				}
				if (parent != this)
				{
					throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
				}
				int num2 = (child == null) ? (myObjects.Count - 1) : myObjects.IndexOf(child);
				int num3 = myObjects.IndexOf(newobj);
				if (num2 + 1 != num3 && num2 != num3)
				{
					myObjects.RemoveAt(num3);
					if (num2 > num3)
					{
						num2--;
					}
					moveTo(num2 + 1, newobj, num3);
				}
			}
		}

		/// <summary>
		/// Remove an object from this group.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// The <paramref name="obj" /> must not belong to a different group.
		/// This method does nothing if the <paramref name="obj" /> has already been
		/// removed from this group.
		/// Afterwards, the <paramref name="obj" />'s <see cref="P:Northwoods.Go.GoObject.Parent" /> will
		/// be null and its <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Layer" /> will also be null, because
		/// it will be removed from the layer if the group and it belonged to a layer.
		/// </remarks>
		public virtual bool Remove(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			GoGroup parent = obj.Parent;
			if (parent == null)
			{
				return false;
			}
			if (parent == this)
			{
				int num = myObjects.IndexOf(obj);
				if (num >= 0)
				{
					removeAt(num, obj, undoing: false);
					return true;
				}
				return false;
			}
			throw new ArgumentException("Cannot remove an object from a group if it doesn't belong to that group.");
		}

		/// <summary>
		/// Move some objects into this group, as immediate children of this group, even if they
		/// are part of other groups in this same layer.
		/// </summary>
		/// <param name="coll">a collection of <see cref="T:Northwoods.Go.GoObject" />s</param>
		/// <param name="reparentLinks">
		/// whether to make sure all connected links belong to the proper subgraph or layer;
		/// if true, this will call <see cref="T:Northwoods.Go.GoSubGraphBase" />.<see cref="M:Northwoods.Go.GoSubGraphBase.ReparentAllLinksToSubGraphs(Northwoods.Go.IGoCollection,System.Boolean,Northwoods.Go.GoLayer)" />
		/// </param>
		/// <remarks>
		/// <para>
		/// This method tries to preserve the links connecting any ports of the objects
		/// being moved into this group.
		/// Of course, none of the objects to be added to this group may be the group itself
		/// nor a parent of this group.
		/// </para>
		/// <para>
		/// Just as with the <see cref="M:Northwoods.Go.GoGroup.Add(Northwoods.Go.GoObject)" /> method, you may find that you will want to 
		/// set the <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Selectable" /> property to false for each of the objects
		/// being added to this group.  The need for this depends on the behavior you want
		/// in your application.
		/// </para>
		/// <para>
		/// This method is normally called on instances of <see cref="T:Northwoods.Go.GoSubGraphBase" />.
		/// If calling this method on a group that is not a subgraph, you will probably want
		/// to specify the value of <paramref name="reparentLinks" /> to be false, since otherwise
		/// any links that you think you are adding to the group will be taken right out again
		/// to be added to the enclosing subgraph or to be a top-level object.
		/// </para>
		/// </remarks>
		public virtual IGoCollection AddCollection(IGoCollection coll, bool reparentLinks)
		{
			foreach (GoObject item in coll)
			{
				if (IsChildOf(item) || this == item)
				{
					throw new ArgumentException("Cannot add a group to itself or to one of its own children.");
				}
			}
			GoCollection goCollection = new GoCollection();
			goCollection.AddRange(coll);
			foreach (GoObject item2 in goCollection)
			{
				if (item2.Parent != this && item2.Layer != null)
				{
					setAllNoClear(item2, b: true);
				}
			}
			foreach (GoObject item3 in goCollection)
			{
				if (item3.Parent != this)
				{
					item3.Remove();
					Add(item3);
				}
			}
			foreach (GoObject item4 in goCollection)
			{
				setAllNoClear(item4, b: false);
			}
			if (reparentLinks && base.IsInDocument)
			{
				GoSubGraphBase.ReparentAllLinksToSubGraphs(goCollection, behind: true, base.Document.LinksLayer);
			}
			return goCollection;
		}

		internal static void setAllNoClear(GoObject obj, bool b)
		{
			GoPort goPort = obj as GoPort;
			if (goPort != null)
			{
				goPort.NoClearLinks = b;
				return;
			}
			GoLink goLink = obj as GoLink;
			if (goLink != null)
			{
				goLink.NoClearPorts = b;
				return;
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				GoBalloon goBalloon = obj as GoBalloon;
				if (goBalloon != null)
				{
					goBalloon.NoClearAnchors = b;
				}
				foreach (GoObject item in goGroup)
				{
					setAllNoClear(item, b);
				}
			}
		}

		/// <summary>
		/// This predicate is true when an object belongs to this group.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This predicate is false if the <paramref name="obj" /> is null, or
		/// if its <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Parent" /> is not this group.
		/// </remarks>
		public virtual bool Contains(GoObject obj)
		{
			if (obj != null)
			{
				return obj.Parent == this;
			}
			return false;
		}

		/// <summary>
		/// Remove all children from this group.
		/// </summary>
		/// <remarks>
		/// This repeatedly calls <see cref="M:Northwoods.Go.GoGroup.Remove(Northwoods.Go.GoObject)" />.
		/// The default implementation tries to avoid duplicate removals,
		/// in case removing one object automatically removes another one.
		/// </remarks>
		public virtual void Clear()
		{
			int num;
			for (num = myObjects.Count; num > 0; num = Math.Min(num, myObjects.Count))
			{
				GoObject obj = myObjects[num = checked(num - 1)];
				Remove(obj);
			}
		}

		/// <summary>
		/// Returns a newly allocated array of all of the immediate child objects in this group.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.IGoCollection.CopyArray" />
		public virtual GoObject[] CopyArray()
		{
			return myObjects.ToArray();
		}

		/// <summary>
		/// Provide a <see cref="T:Northwoods.Go.GoObject" />-specific implementation of IndexOf.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>the non-negative position in the array, or -1 if not present</returns>
		public virtual int IndexOf(GoObject obj)
		{
			return myObjects.IndexOf(obj);
		}

		/// <summary>
		/// Provide a <see cref="T:Northwoods.Go.GoObject" />-specific implementation of Insert,
		/// which just calls <see cref="M:Northwoods.Go.GoGroup.Add(Northwoods.Go.GoObject)" /> or <see cref="M:Northwoods.Go.GoGroup.InsertBefore(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="obj"></param>
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertBefore(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertAfter(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.Remove(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.RemoveAt(System.Int32)" />
		public virtual void Insert(int index, GoObject obj)
		{
			if (index == Count)
			{
				InsertAfter(null, obj);
				return;
			}
			GoObject child = myObjects[index];
			InsertBefore(child, obj);
		}

		/// <summary>
		/// Implement the <c>IList</c> method, which just calls <see cref="M:Northwoods.Go.GoGroup.Remove(Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <param name="index"></param>
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertBefore(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertAfter(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.Add(Northwoods.Go.GoObject)" />
		public void RemoveAt(int index)
		{
			Remove(myObjects[index]);
		}

		/// <summary>
		/// Copy references to this group's immediate children into the given array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(GoObject[] array, int index)
		{
			myObjects.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new GoGroupEnumerator(myObjects, forward: true);
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			return new GoGroupEnumerator(myObjects, forward: true);
		}

		/// <summary>
		/// Return a name for a child object of this group.
		/// </summary>
		/// <param name="child">a <see cref="T:Northwoods.Go.GoObject" /> that is an immediate child of this group</param>
		/// <returns>a String, perhaps an empty one if no name is known</returns>
		/// <remarks>
		/// A child object will not have a name unless you call <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />
		/// first, or unless this method is overridden to provide a name.
		/// This is frequently overridden when defining a group or node that
		/// has well-known parts.  Often such parts are also accessible as a property
		/// on the class, and the name of the part will be the same as the name of
		/// the property.
		/// If you override this, you will probably also need to override <see cref="M:Northwoods.Go.GoGroup.FindChild(System.String)" />.
		/// </remarks>
		public virtual string FindName(GoObject child)
		{
			if (child == null || myNames == null)
			{
				return "";
			}
			if (myNames.TryGetValue(child, out object value))
			{
				return value as string;
			}
			return "";
		}

		/// <summary>
		/// Return a child object that is known by the given name.
		/// </summary>
		/// <param name="name">a String, which may be the same as the property name
		/// for those child objects that are also accessible via a property</param>
		/// <returns>a child of this group, or null if no such named child object is known</returns>
		/// <remarks>
		/// A child object will not have a name unless you call <see cref="M:Northwoods.Go.GoGroup.AddChildName(System.String,Northwoods.Go.GoObject)" />
		/// first, or unless this method is overridden to provide a name.
		/// This is frequently overridden when defining a group or node that
		/// has well-known parts.  Often such parts are also accessible as a property
		/// on the class, and the name of the part will be the same as the name of
		/// the property.
		/// If you override this, you will probably also need to override <see cref="M:Northwoods.Go.GoGroup.FindName(Northwoods.Go.GoObject)" />.
		/// </remarks>
		public virtual GoObject FindChild(string name)
		{
			if (name == null || myNames == null)
			{
				return null;
			}
			if (myNames.TryGetValue(name, out object value))
			{
				return value as GoObject;
			}
			return null;
		}

		/// <summary>
		/// Add an association of a string name with a child object.
		/// </summary>
		/// <param name="name">must not be an empty string or the name of an existing child</param>
		/// <param name="child">must be an immediate child of this group and must not already be known by a name</param>
		/// <remarks>
		/// If successful, this will create the <see cref="P:Northwoods.Go.GoGroup.ChildNames" /> hash table if needed,
		/// and add two entries: one to map the name to the object and another to map the object to the name.
		/// </remarks>
		public virtual void AddChildName(string name, GoObject child)
		{
			if (name != null && child != null)
			{
				if (name.Length == 0)
				{
					throw new ArgumentException("Name must not be an empty string", "name");
				}
				if (child.Parent != this)
				{
					throw new ArgumentException("To be named, the object must be a child of this GoGroup", "child");
				}
				GoObject goObject = FindChild(name);
				if (goObject != null && goObject != child)
				{
					throw new InvalidOperationException("Child name already in use: " + name);
				}
				string text = FindName(child);
				if (text != null && text.Length > 0 && text != name)
				{
					throw new InvalidOperationException("Child object cannot be named: '" + name + "' because it already has a different name: '" + text + "'");
				}
				AddChildNameInternal(name, child, text, goObject);
			}
		}

		private void AddChildNameInternal(string name, GoObject child, string oldname, GoObject oldchild)
		{
			if (myNames == null)
			{
				myNames = new Dictionary<object, object>();
			}
			if (oldchild == null)
			{
				myNames[name] = child;
			}
			if (oldname == null || oldname.Length == 0)
			{
				myNames[child] = name;
			}
			Changed(1056, 0, name, GoObject.NullRect, 0, child, GoObject.NullRect);
		}

		/// <summary>
		/// Remove any association of a string name with a child object.
		/// </summary>
		/// <param name="name"></param>
		public virtual void RemoveChildName(string name)
		{
			if (name != null && myNames != null)
			{
				RemoveChildNameInternal(name);
			}
		}

		private void RemoveChildNameInternal(string name)
		{
			GoObject goObject = FindChild(name);
			if (goObject != null)
			{
				myNames.Remove(goObject);
			}
			bool flag = myNames.ContainsKey(name);
			if (flag)
			{
				myNames.Remove(name);
			}
			if (goObject != null || flag)
			{
				Changed(1057, 0, name, GoObject.NullRect, 0, goObject, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Remove any association of a string name with a child object.
		/// </summary>
		/// <param name="child"></param>
		public virtual void RemoveChildName(GoObject child)
		{
			if (child != null && myNames != null)
			{
				string text = FindName(child);
				bool num = text != null && text.Length > 0;
				if (num)
				{
					myNames.Remove(text);
				}
				bool flag = myNames.ContainsKey(child);
				if (flag)
				{
					myNames.Remove(child);
				}
				if (num || flag)
				{
					Changed(1057, 0, text, GoObject.NullRect, 0, child, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Get an enumerator for iterating over the group's children, starting with
		/// the first child, which is painted behind all other children in the group.
		/// </summary>
		/// <returns></returns>
		public GoGroupEnumerator GetEnumerator()
		{
			return new GoGroupEnumerator(myObjects, forward: true);
		}

		/// <summary>
		/// Painting a group just paints all of its children.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// This method paints first the first child in the group's ordered list.
		/// This calls <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CanView" /> to decide if the child
		/// should be painted.
		/// When <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.IsPrinting" /> is true, it calls
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CanPrint" /> instead of <see cref="M:Northwoods.Go.GoObject.CanView" />
		/// to decide about calling the GoObject's <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> method.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />
		public override void Paint(Graphics g, GoView view)
		{
			bool isPrinting = view.IsPrinting;
			RectangleF clipBounds = g.ClipBounds;
			bool flag = GoObject.ContainsRect(clipBounds, Bounds);
			foreach (GoObject item in GetEnumerator())
			{
				if (isPrinting ? item.CanPrint() : item.CanView())
				{
					bool flag2 = flag;
					if (!flag2)
					{
						RectangleF bounds = item.Bounds;
						bounds = item.ExpandPaintBounds(bounds, view);
						flag2 = GoObject.IntersectsRect(bounds, clipBounds);
					}
					if (flag2)
					{
						item.Paint(g, view);
					}
				}
			}
		}

		/// <summary>
		/// The expanded paint bounds for a group is just the union of expanded
		/// paint bounds for all of its children.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <seealso cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			if ((base.InternalFlags & 0x100000) != 0 || view == null || myPaintBoundsShadowOffset != GetShadowOffset(view))
			{
				ComputePaintBounds(view);
			}
			return new RectangleF(rect.X - myLeft, rect.Y - myTop, rect.Width + myLeft + myRight, rect.Height + myTop + myBottom);
		}

		internal void InvalidatePaintBounds()
		{
			if ((base.InternalFlags & 0x100000) == 0)
			{
				base.InternalFlags |= 1048576;
				base.Parent?.InvalidatePaintBounds();
			}
		}

		private void ComputePaintBounds(GoView view)
		{
			base.InternalFlags &= -1048577;
			RectangleF bounds = Bounds;
			float num = bounds.X;
			float num2 = bounds.Y;
			float num3 = num + bounds.Width;
			float num4 = num2 + bounds.Height;
			bool flag = view?.IsPrinting ?? false;
			foreach (GoObject item in GetEnumerator())
			{
				if (!(flag ? (!item.Printable) : (!item.Visible)))
				{
					RectangleF bounds2 = item.Bounds;
					bounds2 = item.ExpandPaintBounds(bounds2, view);
					num = Math.Min(num, bounds2.X);
					num2 = Math.Min(num2, bounds2.Y);
					num3 = Math.Max(num3, bounds2.X + bounds2.Width);
					num4 = Math.Max(num4, bounds2.Y + bounds2.Height);
				}
			}
			if (view != null)
			{
				myPaintBoundsShadowOffset = GetShadowOffset(view);
			}
			myLeft = bounds.X - num;
			myTop = bounds.Y - num2;
			myRight = num3 - (bounds.X + bounds.Width);
			myBottom = num4 - (bounds.Y + bounds.Height);
		}

		/// <summary>
		/// A group contains a point if any of its visible children contain that point.
		/// </summary>
		/// <param name="p">
		/// A <c>PointF</c> in document coordinates.
		/// </param>
		/// <returns></returns>
		/// <remarks>
		/// This ignores child objects that are not <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Visible" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.ContainsPoint(System.Drawing.PointF)" />
		public override bool ContainsPoint(PointF p)
		{
			if (!GoObject.ContainsRect(Bounds, p))
			{
				return false;
			}
			foreach (GoObject item in GetEnumerator())
			{
				if (item.Visible && item.ContainsPoint(p))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// A group's closest intersection point with a line is the closest such
		/// point among all of the visible children.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		/// <remarks>
		/// Currently, this means that a point on a child object that does not
		/// intersect the line from <paramref name="p1" /> to <paramref name="p2" />
		/// may be closer than a point on some other child that actually does
		/// intersect the line.
		/// This ignores child objects that are not <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Visible" />.
		/// </remarks>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			float num = 1E+21f;
			PointF pointF = default(PointF);
			foreach (GoObject item in GetEnumerator())
			{
				if (item.Visible && item.GetNearestIntersectionPoint(p1, p2, out PointF result2))
				{
					float num2 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
					if (num2 < num)
					{
						num = num2;
						pointF = result2;
					}
				}
			}
			result = pointF;
			return num < 1E+21f;
		}

		/// <summary>
		/// Picking a group first tries to pick a child, starting with
		/// the last one which is most in front.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="selectableOnly"></param>
		/// <returns></returns>
		/// <remarks>
		/// If <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CanView" /> is false for this group, this method returns null.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" />
		public override GoObject Pick(PointF p, bool selectableOnly)
		{
			if (!GoObject.ContainsRect(Bounds, p))
			{
				return null;
			}
			if (!CanView())
			{
				return null;
			}
			foreach (GoObject backward in Backwards)
			{
				GoObject goObject = backward.Pick(p, selectableOnly);
				if (goObject != null)
				{
					return goObject;
				}
			}
			if (PickableBackground)
			{
				if (!selectableOnly)
				{
					return this;
				}
				if (CanSelect())
				{
					return this;
				}
				for (GoObject parent = base.Parent; parent != null; parent = parent.Parent)
				{
					if (parent.CanSelect())
					{
						return parent;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Return a collection of objects that can be picked at a particular point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> location in document coordinates.</param>
		/// <param name="selectableOnly">If true, only consider objects for which <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.</param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// For a group, this is implemented to add the object found by a call to <see cref="M:Northwoods.Go.GoGroup.Pick(System.Drawing.PointF,System.Boolean)" />,
		/// if any object is found.
		/// Thus for a typical node that happens to have several objects underneath each other, only
		/// the top-most (front-most) object is added.
		/// If <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CanView" /> is false for this group, no children are added to the collection.
		/// </remarks>
		public virtual IGoCollection PickObjects(PointF p, bool selectableOnly, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection();
			}
			if (coll.Count >= max)
			{
				return coll;
			}
			if (!CanView())
			{
				return coll;
			}
			GoObject goObject = Pick(p, selectableOnly);
			if (goObject != null)
			{
				coll.Add(goObject);
			}
			return coll;
		}

		/// <summary>
		/// The bounding rectangle of a group is just the union of the bounds of its children.
		/// </summary>
		/// <returns></returns>
		/// <seealso cref="M:Northwoods.Go.GoObject.ComputeBounds" />
		protected override RectangleF ComputeBounds()
		{
			RectangleF rectangleF = Bounds;
			bool flag = false;
			foreach (GoObject item in GetEnumerator())
			{
				if (!flag)
				{
					rectangleF = item.Bounds;
					flag = true;
				}
				else
				{
					rectangleF = GoObject.UnionRect(rectangleF, item.Bounds);
				}
			}
			return rectangleF;
		}

		/// <summary>
		/// Provides default behavior, assuming all children are already correctly
		/// placed and sized, and only need to be moved and/or scaled to fit the
		/// new bounds of this GoGroup.
		/// </summary>
		/// <param name="old"></param>
		/// <remarks>
		/// The default behavior, if the size did not change, is just to call
		/// <see cref="M:Northwoods.Go.GoGroup.MoveChildren(System.Drawing.RectangleF)" />.
		/// If the size did change, we call <see cref="M:Northwoods.Go.GoGroup.RescaleChildren(System.Drawing.RectangleF)" />
		/// to rescale and proportionally reposition all of the children, and
		/// then we call <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" /> to give the node a chance
		/// to do special positioning of any of the children.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnBoundsChanged(System.Drawing.RectangleF)" />
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			SizeF size = base.Size;
			if (old.Width == size.Width && old.Height == size.Height)
			{
				MoveChildren(old);
				return;
			}
			RescaleChildren(old);
			LayoutChildren(null);
			base.InvalidBounds = true;
		}

		/// <summary>
		/// Called after a child of this group has had its bounds changed.
		/// </summary>
		/// <param name="child">
		/// The child object whose <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Bounds" /> were changed.
		/// </param>
		/// <param name="old">
		/// A <c>RectangleF</c> in document coordinates holding the child's previous bounds.
		/// </param>
		/// <remarks>
		/// By default this method just calls <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />,
		/// thus giving the group a chance to adjust to the changed bounds of
		/// one of its children by moving and/or resizing the other children.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.OnChildBoundsChanged(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />
		protected internal virtual void OnChildBoundsChanged(GoObject child, RectangleF old)
		{
			LayoutChildren(child);
			base.InvalidBounds = true;
		}

		/// <summary>
		/// This just moves all the children from the group's former location.
		/// </summary>
		/// <param name="old">
		/// The original bounds, in document coordinates.
		/// </param>
		protected virtual void MoveChildren(RectangleF old)
		{
			float num = base.Left - old.X;
			float num2 = base.Top - old.Y;
			foreach (GoObject item in GetEnumerator())
			{
				RectangleF bounds = item.Bounds;
				item.Bounds = new RectangleF(bounds.X + num, bounds.Y + num2, bounds.Width, bounds.Height);
			}
		}

		/// <summary>
		/// This handles the general case of a resize by scaling and repositioning all the children.
		/// </summary>
		/// <param name="old">
		/// The original bounds, in document coordinates.
		/// </param>
		/// <remarks>
		/// Any children whose <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.AutoRescales" /> property is false is not
		/// resized and repositioned.
		/// </remarks>
		protected virtual void RescaleChildren(RectangleF old)
		{
			if (!(old.Width <= 0f) && !(old.Height <= 0f))
			{
				RectangleF bounds = Bounds;
				float num = bounds.Width / old.Width;
				float num2 = bounds.Height / old.Height;
				foreach (GoObject item in GetEnumerator())
				{
					if (item.AutoRescales)
					{
						RectangleF bounds2 = item.Bounds;
						float x = bounds.X + (bounds2.X - old.X) * num;
						float y = bounds.Y + (bounds2.Y - old.Y) * num2;
						float width = bounds2.Width * num;
						float height = bounds2.Height * num2;
						item.Bounds = new RectangleF(x, y, width, height);
					}
				}
			}
		}

		/// <summary>
		/// Reposition this group's children to achieve a particular appearance.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// <para>
		/// By default this method does nothing.
		/// However, this method is frequently overridden.
		/// </para>
		/// <para>
		/// Implementations of this method probably should not refer, directly or
		/// indirectly, to this group's <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Bounds" /> property.
		/// Instead, you should just position and size the children based on the
		/// bounds of the children (not this group's bounds), and let this
		/// group's bounds be determined by the union of the bounds of the children.
		/// </para>
		/// <para>
		/// For groups that may have many children, overrides will often
		/// check the <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Initializing" /> flag.  If true, this method
		/// usually does nothing; later when all the changes have been performed is
		/// that flag set to false and this method is called explicitly with a null
		/// argument.
		/// </para>
		/// </remarks>
		public virtual void LayoutChildren(GoObject childchanged)
		{
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1051:
			{
				int num2 = e.NewInt;
				GoObject goObject2 = e.NewValue as GoObject;
				if (undo)
				{
					if (num2 < 0)
					{
						num2 = myObjects.IndexOf(goObject2);
					}
					if (num2 >= 0)
					{
						removeAt(num2, goObject2, undoing: true);
					}
				}
				else
				{
					if (num2 < 0)
					{
						num2 = myObjects.Count;
					}
					if (myObjects.IndexOf(goObject2) < 0)
					{
						insertAt(num2, goObject2, undoing: true);
					}
				}
				break;
			}
			case 1052:
			{
				int num = e.OldInt;
				GoObject goObject = e.OldValue as GoObject;
				if (undo)
				{
					if (num < 0)
					{
						num = myObjects.Count;
					}
					if (myObjects.IndexOf(goObject) < 0)
					{
						insertAt(num, goObject, undoing: true);
					}
				}
				else
				{
					if (num < 0)
					{
						num = myObjects.IndexOf(goObject);
					}
					if (num >= 0)
					{
						removeAt(num, goObject, undoing: true);
					}
				}
				break;
			}
			case 1053:
			{
				GoObject goObject3 = (GoObject)e.OldValue;
				int oldInt2 = e.OldInt;
				int newInt = e.NewInt;
				myObjects.Remove(goObject3);
				if (undo)
				{
					moveTo(oldInt2, goObject3, newInt);
				}
				else
				{
					moveTo(newInt, goObject3, oldInt2);
				}
				break;
			}
			case 1054:
			{
				GoObject newobj = (GoObject)e.OldValue;
				GoObject newobj2 = (GoObject)e.NewValue;
				int oldInt = e.OldInt;
				if (undo)
				{
					replaceAt(oldInt, newobj, undoing: true);
				}
				else
				{
					replaceAt(oldInt, newobj2, undoing: true);
				}
				break;
			}
			case 1055:
				PickableBackground = (bool)e.GetValue(undo);
				break;
			case 1056:
			{
				string name2 = (string)e.OldValue;
				GoObject child2 = (GoObject)e.NewValue;
				if (undo)
				{
					RemoveChildNameInternal(name2);
				}
				else
				{
					AddChildNameInternal(name2, child2, FindName(child2), FindChild(name2));
				}
				break;
			}
			case 1057:
			{
				string name = (string)e.OldValue;
				GoObject child = (GoObject)e.NewValue;
				if (undo)
				{
					AddChildNameInternal(name, child, FindName(child), FindChild(name));
				}
				else
				{
					RemoveChildNameInternal(name);
				}
				break;
			}
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
