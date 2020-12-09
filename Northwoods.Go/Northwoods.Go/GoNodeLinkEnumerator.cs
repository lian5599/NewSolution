using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the links (or a subset of the links) connected to this node.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the links connected to a port of a node.  In C#:
	/// <pre><code>
	/// foreach (IGoLink link in aNode.DestinationLinks) {
	///   . . .
	/// }
	/// </code></pre>
	/// <para>
	/// Remember that you must not modify the graph structure of ports and links and nodes connected to this node,
	/// by adding or removing any ports, links, or nodes, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoNodeLinkEnumerator : IEnumerator<IGoLink>, IDisposable, IEnumerator, IEnumerable<IGoLink>, IEnumerable
	{
		private GoNode myNode;

		private GoNode.Search mySearch;

		private List<IGoLink> myArray;

		private int myIndex;

		object IEnumerator.Current => Current;

		IGoLink IEnumerator<IGoLink>.Current => Current;

		/// <summary>
		///  Gets the current link in the collection of links connected to this node.
		/// </summary>
		public IGoLink Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoNode.GoNodeLinkEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Get the number of links.
		/// </summary>
		public int Count => myArray.Count;

		/// <summary>
		/// Gets the first <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to ports in this node.
		/// </summary>
		public IGoLink First
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
		/// Gets the last <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to ports in this node.
		/// </summary>
		public IGoLink Last
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

		internal GoNodeLinkEnumerator(GoNode n, GoNode.Search s)
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

		IEnumerator<IGoLink> IEnumerable<IGoLink>.GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the IGoLinks connected to this node.
		/// </summary>
		public GoNodeLinkEnumerator GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Advance the enumerator to the next IGoLink.
		/// </summary>
		/// <returns>True if there is a next IGoLink; false if it has finished iterating over the collection.</returns>
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
			myArray = myNode.findAll<IGoLink>(mySearch);
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
