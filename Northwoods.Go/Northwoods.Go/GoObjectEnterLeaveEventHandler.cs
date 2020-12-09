using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Represents methods that handle <see cref="T:Northwoods.Go.GoObjectEnterLeaveEventArgs" />.
	/// </summary>
	[Serializable]
	public delegate void GoObjectEnterLeaveEventHandler(object sender, GoObjectEnterLeaveEventArgs e);
}
