using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over all the links connected to this port.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the links of a port.  In C#:
	/// <pre><code>
	/// foreach (IGoLink link in aPort.Links) {
	///   . . .
	/// }
	/// </code></pre>
	/// In VB.NET:
	/// <pre><code>
	/// Dim link As IGoLink
	/// For Each link in aPort.Links
	///   . . .
	/// Next
	/// </code></pre>
	/// <para>
	/// Remember that you must not modify the graph structure of links and nodes connected to this port,
	/// by adding or removing any links or nodes, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoPortLinkEnumerator : IEnumerator<IGoLink>, IDisposable, IEnumerator, IEnumerable<IGoLink>, IEnumerable
	{
		private List<IGoLink> myArray;

		private int myIndex;

		object IEnumerator.Current => Current;

		IGoLink IEnumerator<IGoLink>.Current => Current;

		/// <summary>
		///  Gets the current link in the collection of link connected at this port.
		/// </summary>
		public IGoLink Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoPort.GoPortLinkEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Gets the first <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to this port.
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
		/// Gets the last <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to this port.
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

		internal GoPortLinkEnumerator(List<IGoLink> a)
		{
			myArray = a;
			myIndex = -1;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoPortLinkEnumerator goPortLinkEnumerator = this;
			goPortLinkEnumerator.Reset();
			return goPortLinkEnumerator;
		}

		IEnumerator<IGoLink> IEnumerable<IGoLink>.GetEnumerator()
		{
			GoPortLinkEnumerator goPortLinkEnumerator = this;
			goPortLinkEnumerator.Reset();
			return goPortLinkEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the IGoLinks connected to this port.
		/// </summary>
		public GoPortLinkEnumerator GetEnumerator()
		{
			GoPortLinkEnumerator result = this;
			result.Reset();
			return result;
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
