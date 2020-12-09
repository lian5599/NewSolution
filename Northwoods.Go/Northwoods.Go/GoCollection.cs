using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// This default implementation of <see cref="T:Northwoods.Go.IGoCollection" /> just
	/// uses a <c>List{GoObject}</c>.
	/// </summary>
	/// <remarks>
	/// This is a generic implementation of a collection of <see cref="T:Northwoods.Go.GoObject" />s.
	/// It does not raise any <see cref="T:Northwoods.Go.GoDocument" /> <c>Changed</c> events as
	/// objects are added or removed from this collection.
	/// </remarks>
	[Serializable]
	public class GoCollection : IGoCollection, ICollection<GoObject>, IEnumerable<GoObject>, IEnumerable
	{
		private List<GoObject> myObjects = new List<GoObject>();

		private bool myInternalChecksForDuplicates = true;

		internal int myChanges;

		/// <summary>
		/// This predicate is true when there are no objects in this collection.
		/// </summary>
		public virtual bool IsEmpty => myObjects.Count == 0;

		/// <summary>
		/// Gets the number of objects in this collection.
		/// </summary>
		public virtual int Count => myObjects.Count;

		/// <summary>
		/// True when this collection may not be modified.
		/// </summary>
		public virtual bool IsReadOnly => false;

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the GoObjects in reverse order.
		/// </summary>
		public virtual GoCollectionEnumerator Backwards
		{
			get
			{
				GoCollectionEnumerator result = new GoCollectionEnumerator(myObjects, forward: false);
				result.myOriginalCollection = this;
				result.myOriginalChanges = myChanges;
				return result;
			}
		}

		IEnumerable<GoObject> IGoCollection.Backwards => Backwards;

		/// <summary>
		/// Gets the first object in this collection.
		/// </summary>
		/// <value>
		/// If this collection is empty, this value is null.
		/// </value>
		public virtual GoObject First
		{
			get
			{
				if (IsEmpty)
				{
					return null;
				}
				return myObjects[0];
			}
		}

		/// <summary>
		/// Gets the last object in this collection.
		/// </summary>
		/// <value>
		/// If this collection is empty, this value is null.
		/// </value>
		public virtual GoObject Last
		{
			get
			{
				if (IsEmpty)
				{
					return null;
				}
				return myObjects[checked(Count - 1)];
			}
		}

		internal bool InternalChecksForDuplicates
		{
			get
			{
				return myInternalChecksForDuplicates;
			}
			set
			{
				myInternalChecksForDuplicates = value;
			}
		}

		/// <summary>
		/// Add an object to this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Duplicates are not allowed.
		/// </remarks>
		public virtual void Add(GoObject obj)
		{
			checked
			{
				if (obj != null && (!InternalChecksForDuplicates || !Contains(obj)))
				{
					myObjects.Add(obj);
					myChanges++;
				}
			}
		}

		/// <summary>
		/// Iterate over the <see cref="T:Northwoods.Go.GoObject" />s in the given collection <paramref name="coll" />
		/// and <see cref="M:Northwoods.Go.GoCollection.Add(Northwoods.Go.GoObject)" /> each one to this collection.
		/// </summary>
		/// <param name="coll"></param>
		public virtual void AddRange(IGoCollection coll)
		{
			if (coll != null)
			{
				foreach (GoObject item in coll)
				{
					Add(item);
				}
			}
		}

		/// <summary>
		/// Remove any occurrence of an object from this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// If this collection does not contain the object, nothing happens.
		/// </remarks>
		public virtual bool Remove(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			checked
			{
				if (fastRemove(myObjects, obj) >= 0)
				{
					myChanges++;
					return true;
				}
				return false;
			}
		}

		internal static int fastRemove<T>(List<T> a, T o)
		{
			int num = -1;
			int count = a.Count;
			if (count > 1000)
			{
				num = a.IndexOf(o, checked(count - 50), 50);
			}
			if (num < 0)
			{
				num = a.IndexOf(o);
			}
			if (num >= 0)
			{
				a.RemoveAt(num);
			}
			return num;
		}

		/// <summary>
		/// Determine if the given object is present in this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual bool Contains(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			return myObjects.Contains(obj);
		}

		/// <summary>
		/// Remove all of the objects in this collection.
		/// </summary>
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
		/// Returns a newly allocated array of all of the GoObjects in the collection.
		/// </summary>
		public virtual GoObject[] CopyArray()
		{
			return myObjects.ToArray();
		}

		internal void Sort(IComparer<GoObject> comp)
		{
			checked
			{
				if (myObjects.Count > 1)
				{
					myObjects.Sort(comp);
					myChanges++;
				}
			}
		}

		/// <summary>
		/// Copy references to all of the objects in this collection into an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public virtual void CopyTo(GoObject[] array, int index)
		{
			myObjects.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Gets an enumerator for iterating over the GoObjects in this collection.
		/// </summary>
		public virtual GoCollectionEnumerator GetEnumerator()
		{
			GoCollectionEnumerator result = new GoCollectionEnumerator(myObjects, forward: true);
			result.myOriginalCollection = this;
			result.myOriginalChanges = myChanges;
			return result;
		}
	}
}
