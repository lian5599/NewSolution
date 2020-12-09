using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface represents an abstract connection between nodes, using ports
	/// to specify more specific connection points on the nodes.
	/// </summary>
	/// <remarks>
	/// Every <c>IGoLink</c> also implements <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" />,
	/// <see cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />, and <see cref="P:Northwoods.Go.IGoGraphPart.UserObject" />.
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoLink" />
	/// <seealso cref="T:Northwoods.Go.GoLabeledLink" />
	/// <seealso cref="T:Northwoods.Go.IGoPort" />
	/// <seealso cref="T:Northwoods.Go.IGoNode" />
	public interface IGoLink : IGoGraphPart
	{
		/// <summary>
		/// Gets or sets the port that the link is coming from.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromNode" />
		IGoPort FromPort
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the port that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToNode" />
		IGoPort ToPort
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the node that the link is coming from.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToNode" />
		IGoNode FromNode
		{
			get;
		}

		/// <summary>
		/// Gets the node that the link is going to.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// <seealso cref="P:Northwoods.Go.IGoLink.FromNode" />
		IGoNode ToNode
		{
			get;
		}

		/// <summary>
		/// Given one of the ports connected to this link, return this link's other port.
		/// </summary>
		IGoPort GetOtherPort(IGoPort p);

		/// <summary>
		/// Given one of the nodes connected to this link, return this link's other node.
		/// </summary>
		IGoNode GetOtherNode(IGoNode n);

		/// <summary>
		/// Remove this link by disconnecting it from both of its ports.
		/// </summary>
		void Unlink();

		/// <summary>
		/// This method is called when one of this link's two ports is changed.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		void OnPortChanged(IGoPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);
	}
}
