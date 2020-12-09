using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the layers in this collection, either forwards or backwards.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the layers that are part of a layer collection.  In C#:
	/// <code>
	/// foreach (GoLayer layer in aDocument.Layers) {
	///   . . .
	/// }
	/// </code>
	/// In VB.NET:
	/// <code>
	/// Dim layer As GoLayer
	/// For Each layer in aDocument.Layers
	///   . . .
	/// Next
	/// </code>
	/// This structure type does not support comparison for equality.
	/// </remarks>
	public struct GoLayerCollectionEnumerator : IEnumerator<GoLayer>, IDisposable, IEnumerator, IEnumerable<GoLayer>, IEnumerable
	{
		private List<GoLayer> myArray;

		private bool myForward;

		private int myIndex;

		object IEnumerator.Current => Current;

		GoLayer IEnumerator<GoLayer>.Current => Current;

		/// <summary>
		///  Gets the current layer in the collection.
		/// </summary>
		public GoLayer Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoLayerCollection.GoLayerCollectionEnumerator is not at a valid position for the List");
			}
		}

		internal GoLayerCollectionEnumerator(List<GoLayer> a, bool forward)
		{
			myArray = a;
			myForward = forward;
			myIndex = -1;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoLayerCollectionEnumerator goLayerCollectionEnumerator = this;
			goLayerCollectionEnumerator.Reset();
			return goLayerCollectionEnumerator;
		}

		IEnumerator<GoLayer> IEnumerable<GoLayer>.GetEnumerator()
		{
			GoLayerCollectionEnumerator goLayerCollectionEnumerator = this;
			goLayerCollectionEnumerator.Reset();
			return goLayerCollectionEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the GoLayers in this collection.
		/// </summary>
		public GoLayerCollectionEnumerator GetEnumerator()
		{
			GoLayerCollectionEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next GoLayer.
		/// </summary>
		/// <returns>True if there is a next GoLayer; false if it has finished iterating over the collection.</returns>
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
