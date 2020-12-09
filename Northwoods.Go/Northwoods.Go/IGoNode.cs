using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface represents an abstract object that is connected to other
	/// nodes using links.
	/// </summary>
	/// <remarks>
	/// Every <c>IGoNode</c> also implements <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" />,
	/// <see cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />, and <see cref="P:Northwoods.Go.IGoGraphPart.UserObject" />.
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoNode" />
	/// <seealso cref="T:Northwoods.Go.IGoPort" />
	/// <seealso cref="T:Northwoods.Go.IGoLink" />
	/// <seealso cref="T:Northwoods.Go.IGoLabeledPart" />
	public interface IGoNode : IGoGraphPart
	{
		/// <summary>
		/// Gets an enumerator over all of the nodes that have links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// destination links coming into any port of this node.
		/// </remarks>
		IEnumerable<IGoNode> Sources
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the nodes that have links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// source links going out of any port of this node.
		/// </remarks>
		IEnumerable<IGoNode> Destinations
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the nodes that have links going out of or coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the all nodes that have ports that have any
		/// links connected to any port of this node.
		/// </remarks>
		IEnumerable<IGoNode> Nodes
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that come into
		/// any port of this node.
		/// </remarks>
		IEnumerable<IGoLink> SourceLinks
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that go out of
		/// any port of this node.
		/// </remarks>
		IEnumerable<IGoLink> DestinationLinks
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the links connected at this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that are connected
		/// at any port of this node.
		/// </remarks>
		IEnumerable<IGoLink> Links
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the ports that are part of this node.
		/// </summary>
		IEnumerable<IGoPort> Ports
		{
			get;
		}
	}
}
