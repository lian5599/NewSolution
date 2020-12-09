using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over all of the GoObjects in all of the layers, either forwards or backwards.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the objects that are part of a document.  In C#:
	/// <code>
	/// foreach (GoObject obj in aDocument) {
	///   . . .
	/// }
	/// </code>
	/// In VB.NET:
	/// <code>
	/// Dim obj As GoObject
	/// For Each obj in aDocument
	///   . . .
	/// Next
	/// </code>
	/// <para>
	/// Remember that you must not modify any layer collection, by adding,
	/// removing, or re-ordering any objects, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoLayerCollectionObjectEnumerator : IEnumerator<GoObject>, IDisposable, IEnumerator, IEnumerable<GoObject>, IEnumerable
	{
		private List<GoLayer> myArray;

		private bool myForward;

		private int myIndex;

		private bool myEnumeratorValid;

		private GoLayerEnumerator myEnumerator;

		object IEnumerator.Current => Current;

		GoObject IEnumerator<GoObject>.Current => Current;

		/// <summary>
		///  Gets the current object in the collection of layers.
		/// </summary>
		public GoObject Current
		{
			get
			{
				if (myEnumeratorValid)
				{
					return myEnumerator.Current;
				}
				throw new InvalidOperationException("GoLayerCollectionObjectEnumerator is not at a valid position for the List of Layers");
			}
		}

		internal GoLayerCollectionObjectEnumerator(List<GoLayer> a, bool forward)
		{
			myArray = a;
			myForward = forward;
			myIndex = -1;
			myEnumerator = GoLayerEnumerator.Empty;
			myEnumeratorValid = false;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = this;
			goLayerCollectionObjectEnumerator.Reset();
			return goLayerCollectionObjectEnumerator;
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = this;
			goLayerCollectionObjectEnumerator.Reset();
			return goLayerCollectionObjectEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the all of the GoObjects in all of the layers.
		/// </summary>
		public GoLayerCollectionObjectEnumerator GetEnumerator()
		{
			GoLayerCollectionObjectEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next GoObject in the whole collection of layers of GoObjects.
		/// </summary>
		/// <returns>True if there is a next GoObject; false if it has finished iterating over the collection.</returns>
		public bool MoveNext()
		{
			if (myEnumeratorValid)
			{
				if (myEnumerator.MoveNext())
				{
					return true;
				}
				myEnumeratorValid = false;
			}
			checked
			{
				if (myForward)
				{
					while (myIndex + 1 < myArray.Count)
					{
						myIndex++;
						GoLayer goLayer = myArray[myIndex];
						myEnumerator = goLayer.GetEnumerator();
						myEnumeratorValid = true;
						if (myEnumerator.MoveNext())
						{
							return true;
						}
					}
					return false;
				}
				while (myIndex - 1 >= 0)
				{
					myIndex--;
					GoLayer goLayer2 = myArray[myIndex];
					myEnumerator = goLayer2.Backwards;
					myEnumeratorValid = true;
					if (myEnumerator.MoveNext())
					{
						return true;
					}
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
			myEnumeratorValid = false;
		}

		/// <summary>
		/// There are no resources to clean up.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
