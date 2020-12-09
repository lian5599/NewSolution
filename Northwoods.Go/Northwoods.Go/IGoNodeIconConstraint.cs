using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface is used by <see cref="T:Northwoods.Go.GoNodeIcon" /> to constrain
	/// the resizing of the image.
	/// </summary>
	/// <remarks>
	/// <see cref="T:Northwoods.Go.GoSimpleNode" /> and <see cref="T:Northwoods.Go.GoGeneralNode" /> implement
	/// this interface.
	/// </remarks>
	public interface IGoNodeIconConstraint
	{
		/// <summary>
		/// Gets the minimum non-negative size, in document coordinates, for the icon.
		/// </summary>
		SizeF MinimumIconSize
		{
			get;
		}

		/// <summary>
		/// Gets the maximum size not less than the <see cref="P:Northwoods.Go.IGoNodeIconConstraint.MinimumIconSize" />,
		/// in document coordinates, for the icon.
		/// </summary>
		SizeF MaximumIconSize
		{
			get;
		}
	}
}
