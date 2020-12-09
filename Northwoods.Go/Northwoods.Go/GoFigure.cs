namespace Northwoods.Go
{
	/// <summary>
	/// Predefined <see cref="T:Northwoods.Go.GoDrawing" /> shapes.
	/// </summary>
	/// <remarks>
	/// Use as the argument to the <see cref="M:Northwoods.Go.GoDrawing.#ctor(Northwoods.Go.GoFigure)" /> constructor:
	/// <c>new GoDrawing(GoFigure.Rectangle)</c>.
	/// </remarks>
	public enum GoFigure
	{
		/// <summary>
		/// Used when there is no particular figure for a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		None,
		/// <summary>
		/// Represents a line.
		/// </summary>
		Line1,
		/// <summary>
		/// Represents a line.
		/// </summary>
		Line2,
		/// <summary>
		/// Represents a curve.
		/// </summary>
		Curve1,
		/// <summary>
		/// Represents a curve.
		/// </summary>
		Curve2,
		/// <summary>
		/// Represents a curve.
		/// </summary>
		Curve3,
		/// <summary>
		/// Represents a curve.
		/// </summary>
		Curve4,
		/// <summary>
		/// Represents a three-sided figure (a triangle). This is the same shape
		/// represented by GoFigure.Alternative and GoFigure.Merge.
		/// </summary>
		Triangle,
		/// <summary>
		/// Represents a four-sided figure (a diamond). This is the same shape
		/// represented by GoFigure.Decision.
		/// </summary>
		Diamond,
		/// <summary>
		/// Represents a five-sided figure (a pentagon).
		/// </summary>
		Pentagon,
		/// <summary>
		/// Represents a six-sided figure (a hexagon). This is the same shape
		/// represented by GoFigure.DataTransmission.
		/// </summary>
		Hexagon,
		/// <summary>
		/// Represents a seven-sided figure (a heptagon).
		/// </summary>
		Heptagon,
		/// <summary>
		/// Represents an eight-sided figure (an octagon).
		/// </summary>
		Octagon,
		/// <summary>
		/// Represents a nine-sided figure (a nonagon).
		/// </summary>
		Nonagon,
		/// <summary>
		/// Represents a ten-sided figure (a decagon).
		/// </summary>
		Decagon,
		/// <summary>
		/// Represents a twelve-sided figure (a dodecagon).
		/// </summary>
		Dodecagon,
		/// <summary>
		/// Represents a five-pointed star.
		/// </summary>
		FivePointedStar,
		/// <summary>
		/// Represents a six-pointed star.
		/// </summary>
		SixPointedStar,
		/// <summary>
		/// Represents a seven-pointed star.
		/// </summary>
		SevenPointedStar,
		/// <summary>
		/// Represents an eight-pointed star.
		/// </summary>
		EightPointedStar,
		/// <summary>
		/// Represents a nine-pointed star.
		/// </summary>
		NinePointedStar,
		/// <summary>
		/// Represents a ten-pointed star.
		/// </summary>
		TenPointedStar,
		/// <summary>
		/// Represents a five-pointed burst.
		/// </summary>
		FivePointedBurst,
		/// <summary>
		/// Represents a six-pointed burst.
		/// </summary>
		SixPointedBurst,
		/// <summary>
		/// Represents a seven-pointed burst.
		/// </summary>
		SevenPointedBurst,
		/// <summary>
		/// Represents an eight-pointed burst.
		/// </summary>
		EightPointedBurst,
		/// <summary>
		/// Represents a nine-pointed burst.
		/// </summary>
		NinePointedBurst,
		/// <summary>
		/// Represents a ten-pointed burst.
		/// </summary>
		TenPointedBurst,
		/// <summary>
		///
		/// </summary>
		Circle,
		/// <summary>
		///
		/// </summary>
		Cloud,
		/// <summary>
		///
		/// </summary>
		Crescent,
		/// <summary>
		///
		/// </summary>
		Ellipse,
		/// <summary>
		/// Represents a shape containing a rectangle within another regtangle.
		/// </summary>
		FramedRectangle,
		/// <summary>
		///
		/// </summary>
		HalfEllipse,
		/// <summary>
		/// Represents a shape resembling a heart.
		/// </summary>
		Heart,
		/// <summary>
		/// Represents a shape resembling a spade.
		/// </summary>
		Spade,
		/// <summary>
		/// Represents a shape resembling a club.
		/// </summary>
		Club,
		/// <summary>
		/// Represents an hour glass shape.
		/// </summary>
		HourGlass,
		/// <summary>
		/// Represents a shape resembling a bolt of lightning.
		/// </summary>
		Lightning,
		/// <summary>
		/// Represents a four-sided figure containing two acute opposite angles,
		/// and two obtuse opposite angles.
		/// </summary>
		Parallelogram1,
		/// <summary>
		/// Represents a four-sided figure containing two acute opposite angles,
		/// and two obtuse opposite angles.
		/// </summary>
		Parallelogram2,
		/// <summary>
		/// Represents a four-sided figure containing four ninety degree angles.
		/// </summary>
		Rectangle,
		/// <summary>
		/// Represents a three-sided figure containing one ninety degree angle.
		/// </summary>
		RightTriangle,
		/// <summary>
		///
		/// </summary>
		RoundedIBeam,
		/// <summary>
		///
		/// </summary>
		RoundedRectangle,
		/// <summary>
		///
		/// </summary>
		Square,
		/// <summary>
		/// Represents a figure in the shape of an 'I'.
		/// </summary>
		SquareIBeam,
		/// <summary>
		/// Represents a figure in the shape of a '+'.
		/// </summary>
		ThickCross,
		/// <summary>
		/// Represents a figure in the shape of a 'X'.
		/// </summary>
		ThickX,
		/// <summary>
		/// Represents a figure in the shape of a '+'.
		/// </summary>
		ThinCross,
		/// <summary>
		/// Represents a figure in the shape of a 'X'.
		/// </summary>
		ThinX,
		/// <summary>
		/// Represents a four-sided figure containing two acute adjacent angles,
		/// and two obtuse adjacent angles. This is the same shape represented by
		/// GoFigure.ManualLoop and GoFigure.ManualOperation
		/// </summary>
		Trapezoid,
		/// <summary>
		/// Represents the Yin-Yang symbol.
		/// </summary>
		YinYang,
		/// <summary>
		/// Represents the universal peace symbol.
		/// </summary>
		Peace,
		/// <summary>
		/// Represents a figure used to mean "Not Allowed." In the shape of a circle with a line through.
		/// </summary>
		NotAllowed,
		/// <summary>
		/// Represents a figure used to mean "Fragile." In the shape of a broken glass.
		/// </summary>
		Fragile,
		/// <summary>
		/// Represents the male gender in biology. In the shape of the astronomical Mars symbol.
		/// </summary>
		GenderMale,
		/// <summary>
		/// Represents the female gender in biology. In the shape of the astronomical Venus symbol.
		/// </summary>
		GenderFemale,
		/// <summary>
		/// Represents the "+" symbol using unfilled lines.
		/// </summary>
		PlusLine,
		/// <summary>
		/// Represents an "X" symbol using unfilled lines.
		/// </summary>
		XLine,
		/// <summary>
		/// Represents the "*" symbol using unfilled lines.
		/// </summary>
		AsteriskLine,
		/// <summary>
		/// Represents an unfilled circle.
		/// </summary>
		CircleLine,
		/// <summary>
		/// Represents a Pie with a piece taken out.
		/// </summary>
		Pie,
		/// <summary>
		/// Represents a piece of a pie.
		/// </summary>
		PiePiece,
		/// <summary>
		/// Represents an octagonal Stop Sign.
		/// </summary>
		StopSign,
		/// <summary>
		/// Logical symbol, represented by an arrow.
		/// </summary>
		LogicImplies,
		/// <summary>
		/// Logical symbol for If and Only If, represented by a double-headed arrow.
		/// </summary>
		LogicIff,
		/// <summary>
		/// Logical symbol for Not or Negation, represented by a horizontal line with a small vertical bar at the end.
		/// </summary>
		LogicNot,
		/// <summary>
		/// Logical symbol for And, represented by a vertically flipped "V".
		/// </summary>
		LogicAnd,
		/// <summary>
		/// Logical symbol for Or, represented by a "V".
		/// </summary>
		LogicOr,
		/// <summary>
		/// Logical symbol for Exclusive Or, represented by a circle with a "+" inscribed inside.
		/// </summary>
		LogicXor,
		/// <summary>
		/// Logical symbol for an unconditional Truth, represented by a "T".
		/// </summary>
		LogicTruth,
		/// <summary>
		/// Logical symbol for an unconditional Falsity, represented by a vertically flipped "T".
		/// </summary>
		LogicFalsity,
		/// <summary>
		/// Logical symbol for existential quantification, represented by a horizontally flipped "E".
		/// </summary>
		LogicThereExists,
		/// <summary>
		/// Logical symbol for universal quantification, represented by a vertically flipped "A".
		/// </summary>
		LogicForAll,
		/// <summary>
		/// Logical symbol for Definition, represented by three horizontal bars.
		/// </summary>
		LogicIsDefinedAs,
		/// <summary>
		/// Logical symbol for Intersection, represented by a vertically flipped "U".
		/// </summary>
		LogicIntersect,
		/// <summary>
		/// Logical symbol for Union, represented by a "U".
		/// </summary>
		LogicUnion,
		/// <summary>
		/// Represents a basic arrow shape with a square end.
		/// </summary>
		Arrow,
		/// <summary>
		/// A chevron type arrow. This is the same shape represented by
		/// GoFigure.ISOProcess.
		/// </summary>
		Chevron,
		/// <summary>
		/// Represents a shape consisting of two arrows.
		/// </summary>
		DoubleArrow,
		/// <summary>
		/// Represents an arrow with directional points on each end.
		/// </summary>
		DoubleEndArrow,
		/// <summary>
		/// Represents an arrow with an I-Beam end.
		/// </summary>
		IBeamArrow,
		/// <summary>
		///
		/// </summary>
		Pointer,
		/// <summary>
		///
		/// </summary>
		RoundedPointer,
		/// <summary>
		/// Represents an arrow with a triangle shaped split at the end.
		/// </summary>
		SplitEndArrow,
		/// <summary>
		///
		/// </summary>
		SquareArrow,
		/// <summary>
		///
		/// </summary>
		Cone1,
		/// <summary>
		///
		/// </summary>
		Cone2,
		/// <summary>
		/// A two dimensional representation of a cube.
		/// </summary>
		Cube1,
		/// <summary>
		/// A two dimensional representation of a cube.
		/// </summary>
		Cube2,
		/// <summary>
		///
		/// </summary>
		Cylinder1,
		/// <summary>
		///
		/// </summary>
		Cylinder2,
		/// <summary>
		///
		/// </summary>
		Cylinder3,
		/// <summary>
		///
		/// </summary>
		Cylinder4,
		/// <summary>
		///
		/// </summary>
		Prism1,
		/// <summary>
		///
		/// </summary>
		Prism2,
		/// <summary>
		///
		/// </summary>
		Pyramid1,
		/// <summary>
		///
		/// </summary>
		Pyramid2,
		/// <summary>
		///
		/// </summary>
		Actor,
		/// <summary>
		/// Flowchart 'alternative' symbol. This is the same shape represented by
		/// GoFigure.Triangle.
		/// </summary>
		Alternative,
		/// <summary>
		/// Flowchart 'card' symbol.
		/// </summary>
		Card,
		/// <summary>
		/// Flowchart 'collate' symbol.
		/// </summary>
		Collate,
		/// <summary>
		/// Flowchart 'connector' symbol. This is the same shape represented by
		/// GoFigure.Ellipse.
		/// </summary>
		Connector,
		/// <summary>
		/// Flowchart 'create request' symbol.
		/// </summary>
		CreateRequest,
		/// <summary>
		/// Flowchart 'database' symbol.
		/// </summary>
		Database,
		/// <summary>
		/// Flowchart 'data storage' symbol.
		/// </summary>
		DataStorage,
		/// <summary>
		/// Flowchart 'data transmission' symbol. This is the same shape
		/// represented by GoFigure.Hexagon.
		/// </summary>
		DataTransmission,
		/// <summary>
		/// Flowchart 'decision' symbol. This is the same shape represented by
		/// GoFigure.Diamond.
		/// </summary>
		Decision,
		/// <summary>
		/// Flowchart 'delay' symbol. This is the same shape represented by
		/// GoFigure.HalfEllipse.
		/// </summary>
		Delay,
		/// <summary>
		/// Flowchart 'direct data' symbol. This is the same shape represented by
		/// GoFigure.Cylinder4.
		/// </summary>
		DirectData,
		/// <summary>
		/// Flowchart 'disk storage' symbol.
		/// </summary>
		DiskStorage,
		/// <summary>
		/// Flowchart 'display' symbol.
		/// </summary>
		Display,
		/// <summary>
		/// Flowchart 'divided event' symbol.
		/// </summary>
		DividedEvent,
		/// <summary>
		/// Flowchart 'divided process' symbol.
		/// </summary>
		DividedProcess,
		/// <summary>
		/// Flowchart 'document' symbol.
		/// </summary>
		Document,
		/// <summary>
		/// Flowchart 'external organization' symbol.
		/// </summary>
		ExternalOrganization,
		/// <summary>
		/// Flowchart 'external process' symbol.
		/// </summary>
		ExternalProcess,
		/// <summary>
		/// Flowchart 'file' symbol.
		/// </summary>
		File,
		/// <summary>
		/// Flowchart 'gate' symbol. This is the same shape represented by
		/// GoFigure.Crescent.
		/// </summary>
		Gate,
		/// <summary>
		/// Flowchart 'input' symbol. This is the same shape represented by
		/// GoFigure.Parallelogram1.
		/// </summary>
		Input,
		/// <summary>
		/// Flowchart 'interupt' symbol.
		/// </summary>
		Interupt,
		/// <summary>
		/// Flowchart 'internal storage' symbol.
		/// </summary>
		InternalStorage,
		/// <summary>
		/// Flowchart 'ISO Process' symbol. This is the same shape represented by
		/// GoFigure.Chevron
		/// </summary>
		ISOProcess,
		/// <summary>
		/// Flowchart 'junction' symbol.
		/// </summary>
		Junction,
		/// <summary>
		///
		/// </summary>
		LinedDocument,
		/// <summary>
		/// Flowchart 'loop limit' symbol.
		/// </summary>
		LoopLimit,
		/// <summary>
		/// Flowchart 'magnetic data' symbol. This is the same shape represented by
		/// GoFigure.Cylinder1.
		/// </summary>
		MagneticData,
		/// <summary>
		/// Flowchart 'magetic tape' symbol.
		/// </summary>
		MagneticTape,
		/// <summary>
		/// Flowchart 'manual input' symbol.
		/// </summary>
		ManualInput,
		/// <summary>
		/// Flowchart 'manual loop' symbol. This is the same shape represented by
		/// GoFigure.Trapezoid.
		/// </summary>
		ManualLoop,
		/// <summary>
		/// Flowchart 'manual operation' symbol. This is the same shape represented
		/// by GoFigure.Trapezoid and GoFigure.ManualInput.
		/// </summary>
		ManualOperation,
		/// <summary>
		/// Flowchart 'merge' symbol. This is the same shape represented by
		/// GoFigure.Triangle.
		/// </summary>
		Merge,
		/// <summary>
		/// Flowchart 'message from user' symbol.
		/// </summary>
		MessageFromUser,
		/// <summary>
		/// Flowchart 'message to user' symbol.
		/// </summary>
		MessageToUser,
		/// <summary>
		///
		/// </summary>
		MicroformProcessing,
		/// <summary>
		///
		/// </summary>
		MicroformRecording,
		/// <summary>
		/// Flowchart 'multiple document' symbol.
		/// </summary>
		MultiDocument,
		/// <summary>
		/// Flowchart 'multiple process' symbol.
		/// </summary>
		MultiProcess,
		/// <summary>
		///
		/// </summary>
		OfflineStorage,
		/// <summary>
		/// Flowchart 'off page connector' symbol.
		/// </summary>
		OffPageConnector,
		/// <summary>
		/// Flowchart 'or' symbol
		/// </summary>
		Or,
		/// <summary>
		/// Flowchart 'output' symbol. This is the same shape represented by
		/// GoFigure.Parallelogram1.
		/// </summary>
		Output,
		/// <summary>
		///
		/// </summary>
		PaperTape,
		/// <summary>
		///
		/// </summary>
		PrimitiveFromCall,
		/// <summary>
		///
		/// </summary>
		PrimitiveToCall,
		/// <summary>
		/// Flowchart 'procedure' symbol.
		/// </summary>
		Procedure,
		/// <summary>
		/// Flowchart 'process' symbol.
		/// </summary>
		Process,
		/// <summary>
		/// Flowchart 'sequential data' symbol.
		/// </summary>
		SequentialData,
		/// <summary>
		/// Flowchart 'sort' symbol.
		/// </summary>
		Sort,
		/// <summary>
		/// Flowchart 'start' symbol.
		/// </summary>
		Start,
		/// <summary>
		/// Flowchart 'stored data' symbol.
		/// </summary>
		StoredData,
		/// <summary>
		/// Flowchart 'subroutine' symbol.
		/// </summary>
		Subroutine,
		/// <summary>
		/// Flowchart 'terminator' symbol.
		/// </summary>
		Terminator,
		/// <summary>
		/// Flowchart 'transmittal tape' symbol.
		/// </summary>
		TransmittalTape,
		/// <summary>
		/// Represents an and gate logic circuit.
		/// </summary>
		AndGate,
		/// <summary>
		/// Represents a buffer logic circuit.
		/// </summary>
		Buffer,
		/// <summary>
		/// Represents a system clock.
		/// </summary>
		Clock,
		/// <summary>
		/// Represents ground.
		/// </summary>
		Ground,
		/// <summary>
		/// Represents an inverter logic circuit.
		/// </summary>
		Inverter,
		/// <summary>
		/// Represents a nand gate logic circuit.
		/// </summary>
		NandGate,
		/// <summary>
		/// Represents a nor gate logic circuit.
		/// </summary>
		NorGate,
		/// <summary>
		/// Represents an or gate logic circuit.
		/// </summary>
		OrGate,
		/// <summary>
		/// Represents an xnor gate logic circuit.
		/// </summary>
		XnorGate,
		/// <summary>
		/// Represents an xor gate logic circuit.
		/// </summary>
		XorGate,
		/// <summary>
		/// Represents a capacitor.
		/// </summary>
		Capacitor,
		/// <summary>
		/// Represents a resistor.
		/// </summary>
		Resistor,
		/// <summary>
		/// Represents an inductor.
		/// </summary>
		Inductor,
		/// <summary>
		/// Represents an AC voltage source.
		/// </summary>
		ACvoltageSource,
		/// <summary>
		/// Represents a DC voltage source.
		/// </summary>
		DCvoltageSource,
		/// <summary>
		/// Represents a diode.
		/// </summary>
		Diode,
		/// <summary>
		/// Represents a wifi symbol.
		/// </summary>
		Wifi,
		/// <summary>
		/// Represents an email symbol.
		/// </summary>
		Email,
		/// <summary>
		/// Represents an ethernet jack symbol.
		/// </summary>
		Ethernet,
		/// <summary>
		/// Represents the power symbol.
		/// </summary>
		Power,
		/// <summary>
		/// Represents the Fallout Shelter symbol.
		/// </summary>
		Fallout,
		/// <summary>
		/// Represents the Irritation Hazard symbol, in the shape of an 'X'.
		/// </summary>
		IrritationHazard,
		/// <summary>
		/// Represents an Electrical Hazard symbol, in the shape of a lightning bolt.
		/// </summary>
		ElectricalHazard,
		/// <summary>
		/// Represents a Fire Hazard symbol, in the shape of a fire.
		/// </summary>
		FireHazard,
		/// <summary>
		/// BPMN Symbol for Activity loop marker
		/// </summary>
		BpmnActivityLoop,
		/// <summary>
		/// BPMN Symbol for Activity Parallel Multi-Instance marker
		/// </summary>
		BpmnActivityParallel,
		/// <summary>
		/// BPMN Symbol for Activity Sequential Multi-Instance marker
		/// </summary>
		BpmnActivitySequential,
		/// <summary>
		/// BPMN Symbol for Activity Ad Hoc marker
		/// </summary>
		BpmnActivityAdHoc,
		/// <summary>
		/// BPMN Symbol for Activity Compensation marker (also use for Compensation Event)
		/// </summary>
		BpmnActivityCompensation,
		/// <summary>
		/// BPMN Symbol for Task Type Send/Receive
		/// </summary>
		BpmnTaskMessage,
		/// <summary>
		/// BPMN Symbol for Task Type Script
		/// </summary>
		BpmnTaskScript,
		/// <summary>
		/// BPMN Symbol for Task Type User
		/// </summary>
		BpmnTaskUser,
		/// <summary>
		/// BPMN Symbol for Task Type User, to be used in a GoGroup with PersonHead Figure.
		/// </summary>
		BpmnTaskPersonShirt,
		/// <summary>
		/// BPMN Symbol for Task Type User, to be used in a GoGroup with PersonShirt Figure.
		/// </summary>
		BpmnTaskPersonHead,
		/// <summary>
		/// BPMN Symbol for Event Type Condition
		/// </summary>
		BpmnEventConditional,
		/// <summary>
		/// BPMN Symbol for Event Type Error
		/// </summary>
		BpmnEventError,
		/// <summary>
		/// BPMN Symbol for Event Type Escalation
		/// </summary>
		BpmnEventEscalation,
		/// <summary>
		/// BPMN Symbol for Event Type Timer
		/// </summary>
		BpmnEventTimer
	}
}
