using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over the ports (or a subset of the ports) that are a part of this node.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the ports of a node.  In C#:
	/// <pre><code>
	/// foreach (IGoPort port in aNode.Ports) {
	///   . . .
	/// }
	/// </code></pre>
	/// In VB.NET:
	/// <pre><code>
	/// Dim port As IGoPort
	/// For Each port in aNode.Ports
	///   . . .
	/// Next
	/// </code></pre>
	/// <para>
	/// Remember that you must not modify the graph structure of ports and links and nodes connected to this node,
	/// by adding or removing any ports, links, or nodes, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoNodePortEnumerator : IEnumerator<IGoPort>, IDisposable, IEnumerator, IEnumerable<IGoPort>, IEnumerable
	{
		private GoNode myNode;

		private GoNode.Search mySearch;

		private List<IGoPort> myArray;

		private int myIndex;

		object IEnumerator.Current => Current;

		IGoPort IEnumerator<IGoPort>.Current => Current;

		/// <summary>
		///  Gets the current port in the collection of ports that are part of this node.
		/// </summary>
		public IGoPort Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoNode.GoNodePortEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Get the number of ports.
		/// </summary>
		public int Count => myArray.Count;

		/// <summary>
		/// Gets the first <see cref="T:Northwoods.Go.IGoPort" />, or null if there are no ports in this node.
		/// </summary>
		public IGoPort First
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
		/// Gets the last <see cref="T:Northwoods.Go.IGoPort" />, or null if there are no ports in this node.
		/// </summary>
		public IGoPort Last
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

		internal GoNodePortEnumerator(GoNode n, GoNode.Search s)
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

		IEnumerator<IGoPort> IEnumerable<IGoPort>.GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the IGoPorts that are part of this node.
		/// </summary>
		public GoNodePortEnumerator GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Advance the enumerator to the next IGoPort.
		/// </summary>
		/// <returns>True if there is a next IGoPort; false if it has finished iterating over the collection.</returns>
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
			myArray = myNode.findAll<IGoPort>(mySearch);
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
