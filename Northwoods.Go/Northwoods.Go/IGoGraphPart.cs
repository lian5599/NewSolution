namespace Northwoods.Go
{
	/// <summary>
	/// This interface represents an abstract part of a graph, and is the supertype for
	/// <see cref="T:Northwoods.Go.IGoNode" />, <see cref="T:Northwoods.Go.IGoPort" />, and <see cref="T:Northwoods.Go.IGoLink" />.
	/// </summary>
	public interface IGoGraphPart
	{
		/// <summary>
		/// Gets a <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" /> associated with this node, port, or link.
		/// </summary>
		/// <remarks>
		/// This property is how an abstract network created with <see cref="T:Northwoods.Go.IGoNode" />,
		/// <see cref="T:Northwoods.Go.IGoPort" />, and <see cref="T:Northwoods.Go.IGoLink" /> can be tied to concrete
		/// subclasses of <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" /> that can be added to a
		/// <see cref="T:Northwoods.Go.GoDocument" /> and displayed in a <see cref="T:Northwoods.Go.GoView" />.
		/// <see cref="T:Northwoods.Go.GoNode" />, <see cref="T:Northwoods.Go.GoPort" />, and <see cref="T:Northwoods.Go.GoLink" /> are all
		/// subclasses of <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" />, so those implementations of this
		/// property just return themselves and don't allow the property to be set.
		/// </remarks>
		GoObject GoObject
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an integer value associated with this port.
		/// </summary>
		/// <remarks>
		/// You can use this property for many different purposes, such as associating
		/// some application specific data with the node, port, or link.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserObject" />
		int UserFlags
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an object associated with this port.
		/// </summary>
		/// <remarks>
		/// You can use this property for many different purposes, such as associating
		/// some application specific data with the node, port, or link.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />
		object UserObject
		{
			get;
			set;
		}
	}
}
