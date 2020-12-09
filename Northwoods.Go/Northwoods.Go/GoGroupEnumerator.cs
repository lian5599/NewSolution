using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the immediate child objects of this group, either forwards or backwards.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the children of a group.  In C#:
	/// <pre><code>
	/// foreach (GoObject obj in aGroup) {
	///   . . .
	/// }
	/// </code></pre>
	/// In VB.NET:
	/// <pre><code>
	/// Dim obj As GoObject
	/// For Each obj in aGroup
	///   . . .
	/// Next
	/// </code></pre>
	/// Also, since <see cref="T:Northwoods.Go.GoGroup" /> implements the <c>IList</c> interface,
	/// you can refer to the items of the group by a zero-based index.
	/// <para>
	/// Remember that you must not modify the group collection, by adding,
	/// removing, or re-ordering the child objects, while you are
	/// iterating over the children.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoGroupEnumerator : IEnumerator<GoObject>, IDisposable, IEnumerator, IEnumerable<GoObject>, IEnumerable
	{
		private List<GoObject> myArray;

		private bool myForward;

		private int myIndex;

		object IEnumerator.Current => Current;

		GoObject IEnumerator<GoObject>.Current => Current;

		/// <summary>
		///  Gets the current object in the group.
		/// </summary>
		public GoObject Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoGroup.GoGroupEnumerator is not at a valid position for the List");
			}
		}

		internal GoGroupEnumerator(List<GoObject> a, bool forward)
		{
			myArray = a;
			myForward = forward;
			myIndex = -1;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoGroupEnumerator goGroupEnumerator = this;
			goGroupEnumerator.Reset();
			return goGroupEnumerator;
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			GoGroupEnumerator goGroupEnumerator = this;
			goGroupEnumerator.Reset();
			return goGroupEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the child GoObjects of this group.
		/// </summary>
		public GoGroupEnumerator GetEnumerator()
		{
			GoGroupEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next child GoObject.
		/// </summary>
		/// <returns>True if there is a next GoObject; false if it has finished iterating over the collection.</returns>
		public bool MoveNext()
		{
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
