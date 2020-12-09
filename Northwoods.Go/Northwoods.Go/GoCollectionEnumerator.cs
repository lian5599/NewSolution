using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over all of the GoObjects in this collection, either forwards or backwards.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the objects that are part of a collection.
	/// <para>
	/// Remember that you must not modify the collection, by adding or
	/// removing any child objects, while you are
	/// iterating over the children.
	/// If you do, you may get an InvalidOperationException.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	/// <example>
	/// /// In C#:
	/// <code>
	/// foreach (GoObject obj in aGoCollection) {
	///   . . .
	/// }
	/// </code>
	/// In VB.NET:
	/// <code>
	/// Dim obj As GoObject
	/// For Each obj in aGoCollection
	///   . . .
	/// Next
	/// </code>
	/// </example>
	public struct GoCollectionEnumerator : IEnumerator<GoObject>, IDisposable, IEnumerator, IEnumerable<GoObject>, IEnumerable
	{
		/// <summary>
		/// Return an enumerator that doesn't iterate at all.
		/// </summary>
		public static readonly GoCollectionEnumerator Empty = new GoCollectionEnumerator(new List<GoObject>(), forward: true);

		private List<GoObject> myArray;

		private bool myForward;

		private int myIndex;

		internal int myOriginalChanges;

		internal GoCollection myOriginalCollection;

		object IEnumerator.Current => Current;

		/// <summary>
		///  Gets the current object in the collection for this enumerator.
		/// </summary>
		public GoObject Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoCollectionEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Construct an IEnumerator for iterating either forwards or backwards over a 
		/// <see cref="T:System.Collections.Generic.List`1" />s.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="forward"></param>
		public GoCollectionEnumerator(List<GoObject> a, bool forward)
		{
			myArray = a;
			myForward = forward;
			myIndex = -1;
			myOriginalChanges = 0;
			myOriginalCollection = null;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoCollectionEnumerator goCollectionEnumerator = this;
			goCollectionEnumerator.Reset();
			return goCollectionEnumerator;
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			GoCollectionEnumerator goCollectionEnumerator = this;
			goCollectionEnumerator.Reset();
			return goCollectionEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the GoObjects.
		/// </summary>
		public GoCollectionEnumerator GetEnumerator()
		{
			GoCollectionEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next GoObject.
		/// </summary>
		/// <returns>True if there is a next GoObject; false if it has finished iterating over the collection.</returns>
		public bool MoveNext()
		{
			if (myOriginalCollection != null && myOriginalCollection.myChanges != myOriginalChanges)
			{
				throw new InvalidOperationException(myOriginalCollection.GetType().FullName + " was modified during enumeration.");
			}
			checked
			{
				if (myForward)
				{
					if (myIndex + 1 < myArray.Count)
					{
						myIndex++;
						return true;
					}
					return false;
				}
				if (myIndex - 1 >= 0)
				{
					myIndex--;
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Reset the enumerator to its original position.
		/// </summary>
		public void Reset()
		{
			if (myOriginalCollection != null && myOriginalCollection.myChanges != myOriginalChanges)
			{
				throw new InvalidOperationException(myOriginalCollection.GetType().FullName + " was modified during enumeration.");
			}
			if (myForward)
			{
				myIndex = -1;
			}
			else
			{
				myIndex = myArray.Count;
			}
		}

		/// <summary>
		/// There are no resources to clean up.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
