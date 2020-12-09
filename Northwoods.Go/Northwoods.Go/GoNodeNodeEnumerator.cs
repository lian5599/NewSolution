using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the nodes (or a subset of the nodes) connected to this node.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the nodes connected to a node.  In C#:
	/// <pre><code>
	/// foreach (IGoNode node in aNode.Destinations) {
	///   . . .
	/// }
	/// </code></pre>
	/// In VB.NET:
	/// <pre><code>
	/// Dim node As IGoNode
	/// For Each node in aNode.Destinations
	///   . . .
	/// Next
	/// </code></pre>
	/// <para>
	/// Remember that you must not modify the graph structure of ports and links and nodes connected to this node,
	/// by adding or removing any ports, links, or nodes, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoNodeNodeEnumerator : IEnumerator<IGoNode>, IDisposable, IEnumerator, IEnumerable<IGoNode>, IEnumerable
	{
		private GoNode myNode;

		private GoNode.Search mySearch;

		private List<IGoNode> myArray;

		private int myIndex;

		object IEnumerator.Current => Current;

		IGoNode IEnumerator<IGoNode>.Current => Current;

		/// <summary>
		///  Gets the current node in the collection of connected nodes.
		/// </summary>
		public IGoNode Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoNode.GoNodeNodeEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Get the number of nodes.
		/// </summary>
		public int Count => myArray.Count;

		/// <summary>
		/// Gets the first <see cref="T:Northwoods.Go.IGoNode" />, or null if there are no nodes connected to this node.
		/// </summary>
		public IGoNode First
		{
			get
			{
				if (myArray.Count > 0)
				{
					return myArray[0];
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the last <see cref="T:Northwoods.Go.IGoNode" />, or null if there are no nodes connected to this node.
		/// </summary>
		public IGoNode Last
		{
			get
			{
				if (myArray.Count > 0)
				{
					return myArray[checked(myArray.Count - 1)];
				}
				return null;
			}
		}

		internal GoNodeNodeEnumerator(GoNode n, GoNode.Search s)
		{
			myNode = n;
			mySearch = s;
			myArray = null;
			myIndex = -1;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		IEnumerator<IGoNode> IEnumerable<IGoNode>.GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the IGoNodes connected to this node.
		/// </summary>
		public GoNodeNodeEnumerator GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Advance the enumerator to the next IGoNode.
		/// </summary>
		/// <returns>True if there is a next IGoNode; false if it has finished iterating over the collection.</returns>
		public bool MoveNext()
		{
			checked
			{
				if (myIndex + 1 < myArray.Count)
				{
					myIndex++;
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
			myArray = myNode.findAll<IGoNode>(mySearch);
			myIndex = -1;
		}

		/// <summary>
		/// There are no resources to clean up.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
