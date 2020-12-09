using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A simple class that holds both the array of bytes of <see cref="T:Northwoods.Go.GoDrawingAction" />s
	/// and the array of <c>PointF</c>s describing the figures of a <see cref="T:Northwoods.Go.GoDrawing" />.
	/// </summary>
	[Serializable]
	public class GoDrawingData
	{
		/// <summary>
		/// Gets or sets the array of <c>byte</c>s representing the actions.
		/// </summary>
		public byte[] Actions;

		/// <summary>
		/// Gets or sets the array of <c>PointF</c>s.
		/// </summary>
		public PointF[] Points;

		/// <summary>
		/// Constructs a <c>GoDrawingData</c> containing
		/// an array of <c>byte</c> <see cref="T:Northwoods.Go.GoDrawingAction" />s and
		/// a corresponding array of <c>PointF</c>s.
		/// </summary>
		/// <param name="acts"></param>
		/// <param name="pts"></param>
		public GoDrawingData(byte[] acts, PointF[] pts)
		{
			Actions = acts;
			Points = pts;
		}
	}
}
