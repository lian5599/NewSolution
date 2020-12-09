using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the GoObjects that are in this layer.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the objects that are part of a layer.  In C#:
	/// <code>
	/// foreach (GoObject obj in aLayer) {
	///   . . .
	/// }
	/// </code>
	/// In VB.NET:
	/// <code>
	/// Dim obj As GoObject
	/// For Each obj in aLayer
	///   . . .
	/// Next
	/// </code>
	/// <para>
	/// Remember that you must not modify the layer collection, by adding,
	/// removing, or re-ordering its objects, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoLayerEnumerator : IEnumerator<GoObject>, IDisposable, IEnumerator, IEnumerable<GoObject>, IEnumerable
	{
		internal static GoLayerEnumerator Empty;

		private List<GoObject> myArray;

		private bool myForward;

		private int myIndex;

		object IEnumerator.Current => Current;

		GoObject IEnumerator<GoObject>.Current => Current;

		/// <summary>
		///  Gets the current object in the layer.
		/// </summary>
		public GoObject Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoLayer.GoLayerEnumerator is not at a valid position for the List");
			}
		}

		internal GoLayerEnumerator(List<GoObject> a, bool forward)
		{
			myArray = a;
			myForward = forward;
			myIndex = -1;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoLayerEnumerator goLayerEnumerator = this;
			goLayerEnumerator.Reset();
			return goLayerEnumerator;
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			GoLayerEnumerator goLayerEnumerator = this;
			goLayerEnumerator.Reset();
			return goLayerEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the GoObjects in this layer.
		/// </summary>
		public GoLayerEnumerator GetEnumerator()
		{
			GoLayerEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next GoObject.
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
