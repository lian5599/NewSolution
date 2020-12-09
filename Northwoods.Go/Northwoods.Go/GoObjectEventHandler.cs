using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Represents methods that handle <see cref="T:Northwoods.Go.GoObjectEventArgs" />.
	/// </summary>
	[Serializable]
	public delegate void GoObjectEventHandler(object sender, GoObjectEventArgs e);
}
