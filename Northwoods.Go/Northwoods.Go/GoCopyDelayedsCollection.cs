using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// This class is used as part of a <see cref="T:Northwoods.Go.GoCopyDictionary" /> to
	/// remember objects that were not completely copied during the first
	/// pass of the copying process in <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
	/// </summary>
	[Serializable]
	public class GoCopyDelayedsCollection : ICollection<object>, IEnumerable<object>, IEnumerable
	{
		private Dictionary<object, bool> myObjects = new Dictionary<object, bool>();

		/// <summary>
		/// Returns true if the <see cref="P:Northwoods.Go.GoCopyDelayedsCollection.Count" /> is zero.
		/// </summary>
		public virtual bool IsEmpty => Count == 0;

		/// <summary>
		/// The number of objects whose copying needs additional work.
		/// </summary>
		public virtual int Count => myObjects.Count;

		/// <summary>
		/// True when this collection may not be modified -- but this is normally false.
		/// </summary>
		public virtual bool IsReadOnly => false;

		/// <summary>
		/// Make sure a delayed object is present in this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Null is ignored.
		/// </remarks>
		public virtual void Add(object obj)
		{
			if (obj != null)
			{
				myObjects[obj] = true;
			}
		}

		/// <summary>
		/// Remove an object from this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Null is ignored.
		/// </remarks>
		public virtual bool Remove(object obj)
		{
			if (obj != null)
			{
				return myObjects.Remove(obj);
			}
			return false;
		}

		/// <summary>
		/// Return true if a particular object is in this collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual bool Contains(object obj)
		{
			if (obj != null)
			{
				return myObjects.ContainsKey(obj);
			}
			return false;
		}

		/// <summary>
		/// Remove all objects from this collection.
		/// </summary>
		public virtual void Clear()
		{
			myObjects.Clear();
		}

		/// <summary>
		/// Fill a newly allocated array with references to the source objects that were delayed.
		/// </summary>
		public virtual object[] CopyArray()
		{
			object[] array = new object[Count];
			CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Fill an array with references to the source objects that were copied but
		/// need additional work.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public virtual void CopyTo(object[] array, int index)
		{
			int num = index;
			foreach (KeyValuePair<object, bool> myObject in myObjects)
			{
				if (num >= array.Length)
				{
					break;
				}
				array.SetValue(myObject.Key, checked(num++));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return myObjects.Keys.GetEnumerator();
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return myObjects.Keys.GetEnumerator();
		}

		/// <summary>
		/// Return an iterator over all of the delayed objects.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator<object> GetEnumerator()
		{
			return myObjects.Keys.GetEnumerator();
		}
	}
}
