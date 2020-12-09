using System;

namespace Northwoods.Go
{
	/// <summary>
	/// These values describe the kind of action used to start or extend
	/// figures in a <see cref="T:Northwoods.Go.GoDrawing" />.
	/// </summary>
	[Flags]
	public enum GoDrawingAction : byte
	{
		/// <summary>
		/// Start a new figure at a point.
		/// </summary>
		StartAt = 0x0,
		/// <summary>
		/// Draw a straight line from the last point to this point.
		/// </summary>
		LineTo = 0x1,
		/// <summary>
		/// Draw a Bezier curve using these control points and destination point.
		/// </summary>
		CurveTo = 0x3,
		/// <summary>
		/// A line with the ClosedFlag set.
		/// </summary>
		ClosedLine = 0x81,
		/// <summary>
		/// A curve with the ClosedFlag set.
		/// </summary>
		ClosedCurve = 0x83,
		/// <summary>
		/// The first control point of a Bezier curve; equal to (<see cref="F:Northwoods.Go.GoDrawingAction.CurveTo" /> | GoDrawing.BeginCurveFlag).
		/// </summary>
		BeginCurveTo = 0x43,
		/// <summary>
		/// Mask an <see cref="M:Northwoods.Go.GoDrawing.GetAction(System.Int32)" /> value with this mask to get
		/// the <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" />, <see cref="F:Northwoods.Go.GoDrawingAction.LineTo" />, or <see cref="F:Northwoods.Go.GoDrawingAction.CurveTo" /> value.
		/// </summary>
		Mask = 0xF,
		/// <summary>
		/// An interal flag used to mark the final vertex in a figure as closed.
		/// </summary>
		ClosedFlag = 0x80
	}
}
