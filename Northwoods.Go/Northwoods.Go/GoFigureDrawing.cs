using System;
using System.Drawing;

namespace Northwoods.Go
{
	internal class GoFigureDrawing
	{
		private static byte[][] myActions;

		private static PointF[][] myPoints;

		public static void Init(GoDrawing s, GoFigure f)
		{
			if (f != 0)
			{
				InitData();
				s.SetData(myActions[(int)f], myPoints[(int)f]);
				InitShape(s, f);
			}
		}

		private static void InitShape(GoDrawing s, GoFigure f)
		{
			bool initializing = s.Initializing;
			s.Initializing = true;
			s.Angle = 0f;
			switch (f)
			{
			case GoFigure.Line1:
			case GoFigure.Line2:
			case GoFigure.Curve1:
			case GoFigure.Curve2:
			case GoFigure.Curve3:
			case GoFigure.Curve4:
				s.SameEndPoints = false;
				s.ReshapablePoints = true;
				break;
			case GoFigure.YinYang:
			case GoFigure.Peace:
			case GoFigure.CircleLine:
			case GoFigure.LogicImplies:
			case GoFigure.LogicIff:
			case GoFigure.LogicNot:
			case GoFigure.LogicAnd:
			case GoFigure.LogicOr:
			case GoFigure.LogicXor:
			case GoFigure.LogicTruth:
			case GoFigure.LogicFalsity:
			case GoFigure.LogicThereExists:
			case GoFigure.LogicForAll:
			case GoFigure.LogicIsDefinedAs:
			case GoFigure.LogicIntersect:
			case GoFigure.LogicUnion:
			case GoFigure.Cone2:
			case GoFigure.Cylinder1:
			case GoFigure.Cylinder2:
			case GoFigure.Cylinder3:
			case GoFigure.Cylinder4:
			case GoFigure.Database:
			case GoFigure.DirectData:
			case GoFigure.DiskStorage:
			case GoFigure.File:
			case GoFigure.MagneticData:
			case GoFigure.MultiDocument:
			case GoFigure.MultiProcess:
			case GoFigure.Clock:
			case GoFigure.XnorGate:
			case GoFigure.XorGate:
			case GoFigure.Capacitor:
			case GoFigure.Resistor:
			case GoFigure.Inductor:
			case GoFigure.ACvoltageSource:
			case GoFigure.DCvoltageSource:
			case GoFigure.Diode:
			case GoFigure.Email:
			case GoFigure.Ethernet:
			case GoFigure.Power:
			case GoFigure.BpmnActivityLoop:
			case GoFigure.BpmnActivityParallel:
			case GoFigure.BpmnActivitySequential:
			case GoFigure.BpmnActivityAdHoc:
				s.SameEndPoints = false;
				s.ReshapablePoints = false;
				break;
			default:
				s.SameEndPoints = true;
				s.ReshapablePoints = false;
				s.CloseAllFigures();
				break;
			}
			s.Initializing = initializing;
		}

		private static void InitData()
		{
			if (myActions == null)
			{
				GoDrawing goDrawing = new GoDrawing();
				Array values = Enum.GetValues(typeof(GoFigure));
				int num = 0;
				foreach (GoFigure item in values)
				{
					num = Math.Max(num, (int)item);
				}
				num = checked(num + 1);
				myActions = new byte[num][];
				myPoints = new PointF[num][];
				foreach (GoFigure item2 in values)
				{
					goDrawing.ClearPoints();
					InitShape(goDrawing, item2);
					switch (item2)
					{
					case GoFigure.Line1:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.Line2:
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.Curve1:
					{
						double num169 = Math.PI / 4.0;
						float num170 = (float)(4.0 * (1.0 - Math.Cos(num169)) / (3.0 * Math.Sin(num169)));
						goDrawing.StartAt(0f, 0f);
						goDrawing.CurveTo(num170, 0f, 1f, 1f - num170, 1f, 1f);
						break;
					}
					case GoFigure.Curve2:
					{
						double num167 = Math.PI / 4.0;
						float num168 = (float)(4.0 * (1.0 - Math.Cos(num167)) / (3.0 * Math.Sin(num167)));
						goDrawing.StartAt(0f, 0f);
						goDrawing.CurveTo(0f, num168, 1f - num168, 1f, 1f, 1f);
						break;
					}
					case GoFigure.Curve3:
					{
						double num165 = Math.PI / 4.0;
						float num166 = (float)(4.0 * (1.0 - Math.Cos(num165)) / (3.0 * Math.Sin(num165)));
						goDrawing.StartAt(1f, 0f);
						goDrawing.CurveTo(1f, num166, num166, 1f, 0f, 1f);
						break;
					}
					case GoFigure.Curve4:
					{
						double num163 = Math.PI / 4.0;
						float num164 = (float)(4.0 * (1.0 - Math.Cos(num163)) / (3.0 * Math.Sin(num163)));
						goDrawing.StartAt(1f, 0f);
						goDrawing.CurveTo(1f - num164, 0f, 0f, 1f - num164, 0f, 1f);
						break;
					}
					case GoFigure.Triangle:
					case GoFigure.Alternative:
					case GoFigure.Merge:
						SetLines(goDrawing, CreatePolygon(3));
						break;
					case GoFigure.Diamond:
					case GoFigure.Decision:
						SetLines(goDrawing, CreatePolygon(4));
						break;
					case GoFigure.Pentagon:
						SetLines(goDrawing, CreatePolygon(5));
						break;
					case GoFigure.Hexagon:
					case GoFigure.DataTransmission:
						SetLines(goDrawing, CreatePolygon(6));
						break;
					case GoFigure.Heptagon:
						SetLines(goDrawing, CreatePolygon(7));
						break;
					case GoFigure.Octagon:
						SetLines(goDrawing, CreatePolygon(8));
						break;
					case GoFigure.Nonagon:
						SetLines(goDrawing, CreatePolygon(9));
						break;
					case GoFigure.Decagon:
						SetLines(goDrawing, CreatePolygon(10));
						break;
					case GoFigure.Dodecagon:
						SetLines(goDrawing, CreatePolygon(12));
						break;
					case GoFigure.FivePointedStar:
						SetLines(goDrawing, CreateStar(5));
						break;
					case GoFigure.SixPointedStar:
						SetLines(goDrawing, CreateStar(6));
						break;
					case GoFigure.SevenPointedStar:
						SetLines(goDrawing, CreateStar(7));
						break;
					case GoFigure.EightPointedStar:
						SetLines(goDrawing, CreateStar(8));
						break;
					case GoFigure.NinePointedStar:
						SetLines(goDrawing, CreateStar(9));
						break;
					case GoFigure.TenPointedStar:
						SetLines(goDrawing, CreateStar(10));
						break;
					case GoFigure.FivePointedBurst:
						SetCurves(goDrawing, CreateBurst(5));
						break;
					case GoFigure.SixPointedBurst:
						SetCurves(goDrawing, CreateBurst(6));
						break;
					case GoFigure.SevenPointedBurst:
						SetCurves(goDrawing, CreateBurst(7));
						break;
					case GoFigure.EightPointedBurst:
						SetCurves(goDrawing, CreateBurst(8));
						break;
					case GoFigure.NinePointedBurst:
						SetCurves(goDrawing, CreateBurst(9));
						break;
					case GoFigure.TenPointedBurst:
						SetCurves(goDrawing, CreateBurst(10));
						break;
					case GoFigure.Cloud:
						goDrawing.StartAt(0.08034461f, 0.1944299f);
						goDrawing.CurveTo(-0.09239631f, 0.07836421f, 0.1406031f, -0.0542823f, 0.2008615f, 0.05349299f);
						goDrawing.CurveTo(0.2450511f, -0.01697547f, 0.3776197f, -0.02112067f, 0.4338609f, 0.074219f);
						goDrawing.CurveTo(0.4539471f, 0f, 0.6066018f, -0.02526587f, 0.6558228f, 0.07004196f);
						goDrawing.CurveTo(0.6914277f, -0.02904177f, 554f / 621f, -0.02220843f, 554f / 621f, 0.08370865f);
						goDrawing.CurveTo(1.036446f, 0.04105738f, 1.020377f, 0.3022052f, 0.9147671f, 0.3194596f);
						goDrawing.CurveTo(1.04448f, 0.360238f, 0.992256f, 0.5219009f, 0.9082935f, 0.562044f);
						goDrawing.CurveTo(1.042337f, 0.5771781f, 1.028411f, 0.8120651f, 0.9212406f, 0.8217117f);
						goDrawing.CurveTo(1.028411f, 0.9571472f, 0.8556702f, 1.052487f, (float)Math.E * 212f / 759f, 0.9156953f);
						goDrawing.CurveTo(0.7431877f, 1.019325f, 0.5624123f, 1.031761f, 0.5101666f, 0.9310455f);
						goDrawing.CurveTo(0.4820677f, 1.031761f, 0.3030112f, 1.002796f, 0.2609328f, 0.9344623f);
						goDrawing.CurveTo(0.2329994f, 1.01518f, 0.03213784f, 1.01518f, 0.08034461f, 0.870098f);
						goDrawing.CurveTo(-0.02812061f, 0.9032597f, -0.01205169f, 0.6835638f, 0.06829292f, 0.6545475f);
						goDrawing.CurveTo(-0.02812061f, 0.6089503f, -0.01606892f, 0.4555777f, 0.06427569f, 0.4265613f);
						goDrawing.CurveTo(-0.01606892f, 0.3892545f, -0.01205169f, 0.1944299f, 0.08034461f, 0.1944299f);
						break;
					case GoFigure.Crescent:
					case GoFigure.Gate:
						goDrawing.StartAt(0f, 0f);
						goDrawing.CurveTo(1f, 0f, 1f, 1f, 0f, 1f);
						goDrawing.CurveTo(0.5f, 0.75f, 0.5f, 0.25f, 0f, 0f);
						break;
					case GoFigure.Circle:
					case GoFigure.Ellipse:
					case GoFigure.Connector:
					{
						double num160 = Math.PI / 4.0;
						float num161 = (float)(4.0 * (1.0 - Math.Cos(num160)) / (3.0 * Math.Sin(num160))) * 0.5f;
						float num162 = 0.5f;
						goDrawing.StartAt(1f, num162);
						goDrawing.CurveTo(1f, num162 + num161, num162 + num161, 1f, num162, 1f);
						goDrawing.CurveTo(num162 - num161, 1f, 0f, num162 + num161, 0f, num162);
						goDrawing.CurveTo(0f, num162 - num161, num162 - num161, 0f, num162, 0f);
						goDrawing.CurveTo(num162 + num161, 0f, 1f, num162 - num161, 1f, num162);
						break;
					}
					case GoFigure.FramedRectangle:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.StartAt(0.1f, 0.1f);
						goDrawing.LineTo(0.9f, 0.1f);
						goDrawing.LineTo(0.9f, 0.9f);
						goDrawing.LineTo(0.1f, 0.9f);
						goDrawing.LineTo(0.1f, 0.1f);
						break;
					case GoFigure.HalfEllipse:
					case GoFigure.Delay:
					{
						double num158 = Math.PI / 4.0;
						float num159 = (float)(4.0 * (1.0 - Math.Cos(num158)) / (3.0 * Math.Sin(num158)));
						goDrawing.StartAt(0f, 0f);
						goDrawing.CurveTo(num159, 0f, 1f, 0.5f - num159 / 2f, 1f, 0.5f);
						goDrawing.CurveTo(1f, 0.5f + num159 / 2f, num159, 1f, 0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					}
					case GoFigure.Heart:
						goDrawing.StartAt(0.5f, 0.25f);
						goDrawing.CurveTo(0.5f, 0f, 1f, 0f, 1f, 0.3f);
						goDrawing.CurveTo(1f, 0.5f, 0.5f, 0.9f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f, 0.9f, 0f, 0.5f, 0f, 0.3f);
						goDrawing.CurveTo(0f, 0f, 0.5f, 0f, 0.5f, 0.25f);
						break;
					case GoFigure.Spade:
						goDrawing.StartAt(0.5f, 0.5625f);
						goDrawing.CurveTo(0.5f, 0.75f, 1f, 0.75f, 1f, 0.525f);
						goDrawing.CurveTo(1f, 0.275f, 0.5f, 0.075f, 0.5f, 0f);
						goDrawing.CurveTo(0.5f, 0.075f, 0f, 0.275f, 0f, 0.525f);
						goDrawing.CurveTo(0f, 0.75f, 0.5f, 0.75f, 0.5f, 0.5625f);
						goDrawing.StartAt(0.5f, 0.5625f);
						goDrawing.CurveTo(0.5f, 0.75f, 0.6f, 1f, 0.75f, 1f);
						goDrawing.LineTo(0.25f, 1f);
						goDrawing.CurveTo(0.4f, 1f, 0.5f, 0.75f, 0.5f, 0.5625f);
						break;
					case GoFigure.Club:
					{
						double num155 = Math.PI / 4.0;
						float num156 = (float)(4.0 * (1.0 - Math.Cos(num155)) / (3.0 * Math.Sin(num155))) * 0.15f;
						float num157 = 0.15f;
						goDrawing.StartAt(0.5f, 0.7f);
						goDrawing.CurveTo(0.5f, 0.85f, 0.6f, 1f, 0.65f, 1f);
						goDrawing.LineTo(0.35f, 1f);
						goDrawing.CurveTo(0.4f, 1f, 0.5f, 0.85f, 0.5f, 0.7f);
						PointF pointF25 = new PointF(0.65f, 0.7f);
						goDrawing.BreakUpBezier(new PointF(pointF25.X - num157, pointF25.Y), new PointF(pointF25.X - num157, pointF25.Y - num156), new PointF(pointF25.X - num156, pointF25.Y - num157), new PointF(pointF25.X, pointF25.Y - num157), 2f / 3f, out PointF curve1cp14, out PointF curve1cp15, out PointF midpoint11, out PointF curve2cp15, out PointF curve2cp16);
						goDrawing.StartAt(midpoint11);
						goDrawing.CurveTo(curve2cp15, curve2cp16, new PointF(pointF25.X, pointF25.Y - num157));
						goDrawing.CurveTo(pointF25.X + num156, pointF25.Y - num157, pointF25.X + num157, pointF25.Y - num156, pointF25.X + num157, pointF25.Y);
						goDrawing.CurveTo(pointF25.X + num157, pointF25.Y + num156, pointF25.X + num156, pointF25.Y + num157, pointF25.X, pointF25.Y + num157);
						goDrawing.CurveTo(pointF25.X - num156, pointF25.Y + num157, pointF25.X - num157, pointF25.Y + num156, pointF25.X - num157, pointF25.Y);
						pointF25 = new PointF(0.35f, 0.7f);
						goDrawing.CurveTo(pointF25.X + num157, pointF25.Y + num156, pointF25.X + num156, pointF25.Y + num157, pointF25.X, pointF25.Y + num157);
						goDrawing.CurveTo(pointF25.X - num156, pointF25.Y + num157, pointF25.X - num157, pointF25.Y + num156, pointF25.X - num157, pointF25.Y);
						goDrawing.CurveTo(pointF25.X - num157, pointF25.Y - num156, pointF25.X - num156, pointF25.Y - num157, pointF25.X, pointF25.Y - num157);
						goDrawing.BreakUpBezier(new PointF(pointF25.X, pointF25.Y - num157), new PointF(pointF25.X + num156, pointF25.Y - num157), new PointF(pointF25.X + num157, pointF25.Y - num156), new PointF(pointF25.X + num157, pointF25.Y), 0.333333343f, out curve1cp14, out curve1cp15, out midpoint11, out curve2cp15, out curve2cp16);
						goDrawing.CurveTo(curve1cp14, curve1cp15, midpoint11);
						pointF25 = new PointF(0.5f, 0.7f - 0.15f * (float)Math.Sqrt(3.0));
						goDrawing.BreakUpBezier(new PointF(pointF25.X, pointF25.Y + num157), new PointF(pointF25.X - num156, pointF25.Y + num157), new PointF(pointF25.X - num157, pointF25.Y + num156), new PointF(pointF25.X - num157, pointF25.Y), 0.333333343f, out curve1cp14, out curve1cp15, out midpoint11, out curve2cp15, out curve2cp16);
						goDrawing.CurveTo(curve2cp15, curve2cp16, new PointF(pointF25.X - num157, pointF25.Y));
						goDrawing.CurveTo(pointF25.X - num157, pointF25.Y - num156, pointF25.X - num156, pointF25.Y - num157, pointF25.X, pointF25.Y - num157);
						goDrawing.CurveTo(pointF25.X + num156, pointF25.Y - num157, pointF25.X + num157, pointF25.Y - num156, pointF25.X + num157, pointF25.Y);
						goDrawing.BreakUpBezier(new PointF(pointF25.X + num157, pointF25.Y), new PointF(pointF25.X + num157, pointF25.Y + num156), new PointF(pointF25.X + num156, pointF25.Y + num157), new PointF(pointF25.X, pointF25.Y + num157), 2f / 3f, out curve1cp14, out curve1cp15, out midpoint11, out curve2cp15, out curve2cp16);
						goDrawing.CurveTo(curve1cp14, curve1cp15, midpoint11);
						break;
					}
					case GoFigure.YinYang:
					{
						double num151 = Math.PI / 4.0;
						float num152 = (float)(4.0 * (1.0 - Math.Cos(num151)) / (3.0 * Math.Sin(num151)));
						float num153 = num152 * 0.5f;
						float num154 = 0.5f;
						goDrawing.StartAt(num154, 0f);
						goDrawing.CurveTo(num154 + num153, 0f, 1f, num154 - num153, 1f, num154);
						goDrawing.CurveTo(1f, num154 + num153, num154 + num153, 1f, num154, 1f);
						goDrawing.CurveTo(1f, num154, 0f, num154, num154, 0f);
						goDrawing.CloseFigure(0);
						goDrawing.StartAt(num154, 1f);
						goDrawing.CurveTo(num154 - num153, 1f, 0f, num154 + num153, 0f, num154);
						goDrawing.CurveTo(0f, num154 - num153, num154 - num153, 0f, num154, 0f);
						PointF pointF24 = new PointF(0.5f, 0.75f);
						num154 = 0.1f;
						num153 = num152 * 0.1f;
						goDrawing.StartAt(pointF24.X - num154, pointF24.Y);
						goDrawing.CurveTo(pointF24.X - num154, pointF24.Y - num153, pointF24.X - num153, pointF24.Y - num154, pointF24.X, pointF24.Y - num154);
						goDrawing.CurveTo(pointF24.X + num153, pointF24.Y - num154, pointF24.X + num154, pointF24.Y - num153, pointF24.X + num154, pointF24.Y);
						goDrawing.CurveTo(pointF24.X + num154, pointF24.Y + num153, pointF24.X + num153, pointF24.Y + num154, pointF24.X, pointF24.Y + num154);
						goDrawing.CurveTo(pointF24.X - num153, pointF24.Y + num154, pointF24.X - num154, pointF24.Y + num153, pointF24.X - num154, pointF24.Y);
						goDrawing.CloseFigure(29);
						pointF24 = new PointF(0.5f, 0.25f);
						goDrawing.StartAt(pointF24.X, pointF24.Y - num154);
						goDrawing.CurveTo(pointF24.X - num153, pointF24.Y - num154, pointF24.X - num154, pointF24.Y - num153, pointF24.X - num154, pointF24.Y);
						goDrawing.CurveTo(pointF24.X - num154, pointF24.Y + num153, pointF24.X - num153, pointF24.Y + num154, pointF24.X, pointF24.Y + num154);
						goDrawing.CurveTo(pointF24.X + num153, pointF24.Y + num154, pointF24.X + num154, pointF24.Y + num153, pointF24.X + num154, pointF24.Y);
						goDrawing.CurveTo(pointF24.X + num154, pointF24.Y - num153, pointF24.X + num153, pointF24.Y - num154, pointF24.X, pointF24.Y - num154);
						goDrawing.CloseFigure(42);
						goDrawing.SameEndPoints = true;
						break;
					}
					case GoFigure.Peace:
					{
						double num147 = Math.PI / 4.0;
						float num148 = (float)(4.0 * (1.0 - Math.Cos(num147)) / (3.0 * Math.Sin(num147)));
						float num149 = num148 * 0.5f;
						float num150 = 0.4f;
						num150 = 0.1f;
						num149 = num148 * 0.1f;
						PointF pointF23 = new PointF(0.62f, 0.6f);
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.LineTo(pointF23.X - num150, pointF23.Y);
						num149 = num148 * 0.5f;
						num150 = 0.4f;
						pointF23 = new PointF(0.5f, 0.5f);
						num149 = num148 * 0.4f;
						goDrawing.StartAt(pointF23.X, pointF23.Y - num150);
						goDrawing.CurveTo(pointF23.X - num149, pointF23.Y - num150, pointF23.X - num150, pointF23.Y - num149, pointF23.X - num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X - num150, pointF23.Y + num149, pointF23.X - num149, pointF23.Y + num150, pointF23.X, pointF23.Y + num150);
						goDrawing.CurveTo(pointF23.X + num149, pointF23.Y + num150, pointF23.X + num150, pointF23.Y + num149, pointF23.X + num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X + num150, pointF23.Y - num149, pointF23.X + num149, pointF23.Y - num150, pointF23.X, pointF23.Y - num150);
						num150 = 0.5f;
						num149 = num148 * 0.5f;
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X - num150, pointF23.Y - num149, pointF23.X - num149, pointF23.Y - num150, pointF23.X, pointF23.Y - num150);
						goDrawing.CurveTo(pointF23.X + num149, pointF23.Y - num150, pointF23.X + num150, pointF23.Y - num149, pointF23.X + num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X + num150, pointF23.Y + num149, pointF23.X + num149, pointF23.Y + num150, pointF23.X, pointF23.Y + num150);
						goDrawing.CurveTo(pointF23.X - num149, pointF23.Y + num150, pointF23.X - num150, pointF23.Y + num149, pointF23.X - num150, pointF23.Y);
						goDrawing.CloseFigure(25);
						goDrawing.CloseFigure(12);
						num150 = 0.1f;
						num149 = num148 * 0.1f;
						pointF23 = new PointF(0.62f, 0.6f);
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.LineTo(pointF23.X - num150, pointF23.Y);
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X - num150, pointF23.Y - num149, pointF23.X - num149, pointF23.Y - num150, pointF23.X, pointF23.Y - num150);
						goDrawing.CurveTo(pointF23.X + num149, pointF23.Y - num150, pointF23.X + num150, pointF23.Y - num149, pointF23.X + num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X + num150, pointF23.Y + num149, pointF23.X + num149, pointF23.Y + num150, pointF23.X, pointF23.Y + num150);
						goDrawing.CurveTo(pointF23.X - num149, pointF23.Y + num150, pointF23.X - num150, pointF23.Y + num149, pointF23.X - num150, pointF23.Y);
						pointF23 = new PointF(0.5f, 0.4f);
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X - num150, pointF23.Y - num149, pointF23.X - num149, pointF23.Y - num150, pointF23.X, pointF23.Y - num150);
						goDrawing.CurveTo(pointF23.X + num149, pointF23.Y - num150, pointF23.X + num150, pointF23.Y - num149, pointF23.X + num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X + num150, pointF23.Y + num149, pointF23.X + num149, pointF23.Y + num150, pointF23.X, pointF23.Y + num150);
						goDrawing.CurveTo(pointF23.X - num149, pointF23.Y + num150, pointF23.X - num150, pointF23.Y + num149, pointF23.X - num150, pointF23.Y);
						pointF23 = new PointF(0.38f, 0.6f);
						goDrawing.StartAt(pointF23.X - num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X - num150, pointF23.Y - num149, pointF23.X - num149, pointF23.Y - num150, pointF23.X, pointF23.Y - num150);
						goDrawing.CurveTo(pointF23.X + num149, pointF23.Y - num150, pointF23.X + num150, pointF23.Y - num149, pointF23.X + num150, pointF23.Y);
						goDrawing.CurveTo(pointF23.X + num150, pointF23.Y + num149, pointF23.X + num149, pointF23.Y + num150, pointF23.X, pointF23.Y + num150);
						goDrawing.CurveTo(pointF23.X - num149, pointF23.Y + num150, pointF23.X - num150, pointF23.Y + num149, pointF23.X - num150, pointF23.Y);
						goDrawing.CloseFigure(38);
						goDrawing.CloseFigure(51);
						goDrawing.CloseFigure(64);
						goDrawing.SameEndPoints = true;
						break;
					}
					case GoFigure.NotAllowed:
					{
						double num143 = Math.PI / 4.0;
						float num144 = (float)(4.0 * (1.0 - Math.Cos(num143)) / (3.0 * Math.Sin(num143)));
						float num145 = num144 * 0.5f;
						float num146 = 0.5f;
						PointF pointF22 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF22.X, pointF22.Y - num146);
						goDrawing.CurveTo(pointF22.X - num145, pointF22.Y - num146, pointF22.X - num146, pointF22.Y - num145, pointF22.X - num146, pointF22.Y);
						goDrawing.CurveTo(pointF22.X - num146, pointF22.Y + num145, pointF22.X - num145, pointF22.Y + num146, pointF22.X, pointF22.Y + num146);
						goDrawing.CurveTo(pointF22.X + num145, pointF22.Y + num146, pointF22.X + num146, pointF22.Y + num145, pointF22.X + num146, pointF22.Y);
						goDrawing.CurveTo(pointF22.X + num146, pointF22.Y - num145, pointF22.X + num145, pointF22.Y - num146, pointF22.X, pointF22.Y - num146);
						num146 = 0.4f;
						num145 = num144 * 0.4f;
						goDrawing.BreakUpBezier(new PointF(pointF22.X, pointF22.Y - num146), new PointF(pointF22.X + num145, pointF22.Y - num146), new PointF(pointF22.X + num146, pointF22.Y - num145), new PointF(pointF22.X + num146, pointF22.Y), 0.42f, out PointF curve1cp10, out PointF curve1cp11, out PointF midpoint7, out PointF curve2cp10, out curve2cp10);
						goDrawing.BreakUpBezier(new PointF(pointF22.X, pointF22.Y - num146), new PointF(pointF22.X + num145, pointF22.Y - num146), new PointF(pointF22.X + num146, pointF22.Y - num145), new PointF(pointF22.X + num146, pointF22.Y), 0.58f, out curve2cp10, out curve2cp10, out PointF midpoint8, out PointF curve2cp11, out PointF curve2cp12);
						goDrawing.BreakUpBezier(new PointF(pointF22.X, pointF22.Y + num146), new PointF(pointF22.X - num145, pointF22.Y + num146), new PointF(pointF22.X - num146, pointF22.Y + num145), new PointF(pointF22.X - num146, pointF22.Y), 0.42f, out PointF curve1cp12, out PointF curve1cp13, out PointF midpoint9, out curve2cp10, out curve2cp10);
						goDrawing.BreakUpBezier(new PointF(pointF22.X, pointF22.Y + num146), new PointF(pointF22.X - num145, pointF22.Y + num146), new PointF(pointF22.X - num146, pointF22.Y + num145), new PointF(pointF22.X - num146, pointF22.Y), 0.58f, out curve2cp10, out curve2cp10, out PointF midpoint10, out PointF curve2cp13, out PointF curve2cp14);
						goDrawing.StartAt(midpoint10);
						goDrawing.CurveTo(curve2cp13, curve2cp14, new PointF(pointF22.X - num146, pointF22.Y));
						goDrawing.CurveTo(pointF22.X - num146, pointF22.Y - num145, pointF22.X - num145, pointF22.Y - num146, pointF22.X, pointF22.Y - num146);
						goDrawing.CurveTo(curve1cp10, curve1cp11, midpoint7);
						goDrawing.LineTo(midpoint10);
						goDrawing.StartAt(midpoint9);
						goDrawing.LineTo(midpoint8);
						goDrawing.CurveTo(curve2cp11, curve2cp12, new PointF(pointF22.X + num146, pointF22.Y));
						goDrawing.CurveTo(pointF22.X + num146, pointF22.Y + num145, pointF22.X + num145, pointF22.Y + num146, pointF22.X, pointF22.Y + num146);
						goDrawing.CurveTo(curve1cp12, curve1cp13, midpoint9);
						break;
					}
					case GoFigure.Fragile:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.25f, 0f);
						goDrawing.LineTo(0.2f, 0.15f);
						goDrawing.LineTo(0.3f, 0.25f);
						goDrawing.LineTo(0.25f, 0.4f);
						goDrawing.LineTo(0.35f, 0.25f);
						goDrawing.LineTo(0.3f, 0.15f);
						goDrawing.LineTo(0.4f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.CurveTo(1f, 0.25f, 0.75f, 0.5f, 0.55f, 0.5f);
						goDrawing.LineTo(0.55f, 0.9f);
						goDrawing.LineTo(0.7f, 0.9f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.LineTo(0.3f, 0.9f);
						goDrawing.LineTo(0.45f, 0.9f);
						goDrawing.LineTo(0.45f, 0.5f);
						goDrawing.CurveTo(0.25f, 0.5f, 0f, 0.25f, 0f, 0f);
						break;
					case GoFigure.HourGlass:
						goDrawing.StartAt(0.65f, 0.5f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0.35f, 0.5f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.65f, 0.5f);
						break;
					case GoFigure.Lightning:
						goDrawing.StartAt(0f, 0.55f);
						goDrawing.LineTo(0.38f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(0.25f, 0.45f);
						goDrawing.LineTo(0.9f, 0.48f);
						goDrawing.LineTo(0.4f, 1f);
						goDrawing.LineTo(0.65f, 0.55f);
						goDrawing.LineTo(0f, 0.55f);
						break;
					case GoFigure.Parallelogram1:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.25f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.75f, 1f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.Input:
					case GoFigure.Output:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.1f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.Parallelogram2:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.ThickCross:
						goDrawing.StartAt(0.3f, 0f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.LineTo(1f, 0.3f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.LineTo(0.7f, 0.7f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.LineTo(0.3f, 0.7f);
						goDrawing.LineTo(0f, 0.7f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.LineTo(0.3f, 0.3f);
						goDrawing.LineTo(0.3f, 0f);
						break;
					case GoFigure.ThickX:
						goDrawing.StartAt(0.3f, 0f);
						goDrawing.LineTo(0.5f, 0.2f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.3f);
						goDrawing.LineTo(0.8f, 0.5f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.5f, 0.8f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.LineTo(0f, 0.7f);
						goDrawing.LineTo(0.2f, 0.5f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.LineTo(0.3f, 0f);
						break;
					case GoFigure.ThinCross:
						goDrawing.StartAt(0.45f, 0f);
						goDrawing.LineTo(0.55f, 0f);
						goDrawing.LineTo(0.55f, 0.45f);
						goDrawing.LineTo(1f, 0.45f);
						goDrawing.LineTo(1f, 0.55f);
						goDrawing.LineTo(0.55f, 0.55f);
						goDrawing.LineTo(0.55f, 1f);
						goDrawing.LineTo(0.45f, 1f);
						goDrawing.LineTo(0.45f, 0.55f);
						goDrawing.LineTo(0f, 0.55f);
						goDrawing.LineTo(0f, 0.45f);
						goDrawing.LineTo(0.45f, 0.45f);
						goDrawing.LineTo(0.45f, 0f);
						break;
					case GoFigure.ThinX:
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.5f, 0.4f);
						goDrawing.LineTo(0.9f, 0f);
						goDrawing.LineTo(1f, 0.1f);
						goDrawing.LineTo(0.6f, 0.5f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0.5f, 0.6f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.LineTo(0f, 0.9f);
						goDrawing.LineTo(0.4f, 0.5f);
						goDrawing.LineTo(0f, 0.1f);
						goDrawing.LineTo(0.1f, 0f);
						break;
					case GoFigure.None:
					case GoFigure.Rectangle:
					case GoFigure.Square:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.RightTriangle:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.RoundedIBeam:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.CurveTo(0.5f, 0.25f, 0.5f, 0.75f, 1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.CurveTo(0.5f, 0.75f, 0.5f, 0.25f, 0f, 0f);
						break;
					case GoFigure.RoundedRectangle:
					{
						double num141 = Math.PI / 4.0;
						float num142 = (float)(4.0 * (1.0 - Math.Cos(num141)) / (3.0 * Math.Sin(num141))) * 0.3f;
						goDrawing.StartAt(0.3f, 0f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.CurveTo(0.7f + num142, 0f, 1f, 0.3f - num142, 1f, 0.3f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.CurveTo(1f, 0.7f + num142, 0.7f + num142, 1f, 0.7f, 1f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.CurveTo(0.3f - num142, 1f, 0f, 0.7f + num142, 0f, 0.7f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.CurveTo(0f, 0.3f - num142, 0.3f - num142, 0f, 0.3f, 0f);
						break;
					}
					case GoFigure.SquareIBeam:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.2f);
						goDrawing.LineTo(0.6f, 0.2f);
						goDrawing.LineTo(0.6f, 0.8f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.8f);
						goDrawing.LineTo(0.4f, 0.8f);
						goDrawing.LineTo(0.4f, 0.2f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Trapezoid:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.25f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.ManualLoop:
					case GoFigure.ManualOperation:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.GenderMale:
					{
						double num137 = Math.PI / 4.0;
						float num138 = (float)(4.0 * (1.0 - Math.Cos(num137)) / (3.0 * Math.Sin(num137)));
						float num139 = num138 * 0.4f;
						float num140 = 0.4f;
						PointF pointF20 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF20.X - num140, pointF20.Y);
						goDrawing.CurveTo(pointF20.X - num140, pointF20.Y - num139, pointF20.X - num139, pointF20.Y - num140, pointF20.X, pointF20.Y - num140);
						goDrawing.BreakUpBezier(new PointF(pointF20.X, pointF20.Y - num140), new PointF(pointF20.X + num139, pointF20.Y - num140), new PointF(pointF20.X + num140, pointF20.Y - num139), new PointF(pointF20.X + num140, pointF20.Y), 0.44f, out PointF curve1cp8, out PointF curve1cp9, out PointF midpoint6, out PointF curve2cp9, out curve2cp9);
						goDrawing.CurveTo(curve1cp8, curve1cp9, midpoint6);
						PointF pointF21 = new PointF(midpoint6.X, midpoint6.Y);
						goDrawing.BreakUpBezier(new PointF(pointF20.X, pointF20.Y - num140), new PointF(pointF20.X + num139, pointF20.Y - num140), new PointF(pointF20.X + num140, pointF20.Y - num139), new PointF(pointF20.X + num140, pointF20.Y), 0.56f, out curve2cp9, out curve2cp9, out midpoint6, out curve1cp8, out curve1cp9);
						PointF p4 = new PointF(midpoint6.X, midpoint6.Y);
						goDrawing.LineTo(pointF21.X * 0.1f + 0.854999959f, pointF21.Y * 0.1f);
						goDrawing.LineTo(0.85f, pointF21.Y * 0.1f);
						goDrawing.LineTo(0.85f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.15f);
						goDrawing.LineTo(p4.X * 0.1f + 0.9f, 0.15f);
						goDrawing.LineTo(p4.X * 0.1f + 0.9f, p4.Y * 0.1f + 0.0449999981f);
						goDrawing.LineTo(p4);
						goDrawing.CurveTo(curve1cp8, curve1cp9, new PointF(pointF20.X + num140, pointF20.Y));
						goDrawing.CurveTo(pointF20.X + num140, pointF20.Y + num139, pointF20.X + num139, pointF20.Y + num140, pointF20.X, pointF20.Y + num140);
						goDrawing.CurveTo(pointF20.X - num139, pointF20.Y + num140, pointF20.X - num140, pointF20.Y + num139, pointF20.X - num140, pointF20.Y);
						num140 = 0.35f;
						num139 = num138 * 0.35f;
						goDrawing.StartAt(pointF20.X, pointF20.Y - num140);
						goDrawing.CurveTo(pointF20.X - num139, pointF20.Y - num140, pointF20.X - num140, pointF20.Y - num139, pointF20.X - num140, pointF20.Y);
						goDrawing.CurveTo(pointF20.X - num140, pointF20.Y + num139, pointF20.X - num139, pointF20.Y + num140, pointF20.X, pointF20.Y + num140);
						goDrawing.CurveTo(pointF20.X + num139, pointF20.Y + num140, pointF20.X + num140, pointF20.Y + num139, pointF20.X + num140, pointF20.Y);
						goDrawing.CurveTo(pointF20.X + num140, pointF20.Y - num139, pointF20.X + num139, pointF20.Y - num140, pointF20.X, pointF20.Y - num140);
						goDrawing.CloseFigure(25);
						goDrawing.CloseFigure(12);
						break;
					}
					case GoFigure.GenderFemale:
					{
						double num133 = Math.PI / 4.0;
						float num134 = (float)(4.0 * (1.0 - Math.Cos(num133)) / (3.0 * Math.Sin(num133)));
						float num135 = num134 * 0.3f;
						float num136 = 0.3f;
						PointF pointF18 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF18.X - num136, pointF18.Y);
						goDrawing.CurveTo(pointF18.X - num136, pointF18.Y - num135, pointF18.X - num135, pointF18.Y - num136, pointF18.X, pointF18.Y - num136);
						goDrawing.CurveTo(pointF18.X + num135, pointF18.Y - num136, pointF18.X + num136, pointF18.Y - num135, pointF18.X + num136, pointF18.Y);
						goDrawing.BreakUpBezier(new PointF(pointF18.X + num136, pointF18.Y), new PointF(pointF18.X + num136, pointF18.Y + num135), new PointF(pointF18.X + num135, pointF18.Y + num136), new PointF(pointF18.X, pointF18.Y + num136), 0.95f, out PointF curve1cp6, out PointF curve1cp7, out PointF midpoint5, out PointF curve2cp8, out curve2cp8);
						goDrawing.CurveTo(curve1cp6, curve1cp7, midpoint5);
						PointF pointF19 = new PointF(midpoint5.X, midpoint5.Y);
						goDrawing.BreakUpBezier(new PointF(pointF18.X, pointF18.Y + num136), new PointF(pointF18.X - num135, pointF18.Y + num136), new PointF(pointF18.X - num136, pointF18.Y + num135), new PointF(pointF18.X - num136, pointF18.Y), 0.05f, out curve2cp8, out curve2cp8, out midpoint5, out curve1cp6, out curve1cp7);
						PointF p3 = new PointF(midpoint5.X, midpoint5.Y);
						goDrawing.LineTo(pointF19.X, 0.9f);
						goDrawing.LineTo(pointF19.X + 0.05f, 0.9f);
						goDrawing.LineTo(pointF19.X + 0.05f, 0.95f);
						goDrawing.LineTo(pointF19.X, 0.95f);
						goDrawing.LineTo(pointF19.X, 1f);
						goDrawing.LineTo(p3.X, 1f);
						goDrawing.LineTo(p3.X, 0.95f);
						goDrawing.LineTo(p3.X - 0.05f, 0.95f);
						goDrawing.LineTo(p3.X - 0.05f, 0.9f);
						goDrawing.LineTo(p3.X, 0.9f);
						goDrawing.LineTo(p3);
						goDrawing.CurveTo(curve1cp6, curve1cp7, new PointF(pointF18.X - num136, pointF18.Y));
						num136 = 0.25f;
						num135 = num134 * 0.25f;
						goDrawing.StartAt(pointF18.X, pointF18.Y - num136);
						goDrawing.CurveTo(pointF18.X - num135, pointF18.Y - num136, pointF18.X - num136, pointF18.Y - num135, pointF18.X - num136, pointF18.Y);
						goDrawing.CurveTo(pointF18.X - num136, pointF18.Y + num135, pointF18.X - num135, pointF18.Y + num136, pointF18.X, pointF18.Y + num136);
						goDrawing.CurveTo(pointF18.X + num135, pointF18.Y + num136, pointF18.X + num136, pointF18.Y + num135, pointF18.X + num136, pointF18.Y);
						goDrawing.CurveTo(pointF18.X + num136, pointF18.Y - num135, pointF18.X + num135, pointF18.Y - num136, pointF18.X, pointF18.Y - num136);
						goDrawing.CloseFigure(25);
						goDrawing.CloseFigure(12);
						break;
					}
					case GoFigure.PlusLine:
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.XLine:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(1f, 0f);
						break;
					case GoFigure.AsteriskLine:
					{
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						float num132 = (float)(0.20000000298023224 / Math.Sqrt(2.0));
						goDrawing.StartAt(num132, num132);
						goDrawing.LineTo(1f - num132, 1f - num132);
						goDrawing.StartAt(num132, 1f - num132);
						goDrawing.LineTo(1f - num132, num132);
						break;
					}
					case GoFigure.CircleLine:
					{
						double num129 = Math.PI / 4.0;
						float num130 = (float)(4.0 * (1.0 - Math.Cos(num129)) / (3.0 * Math.Sin(num129))) * 0.5f;
						float num131 = 0.5f;
						goDrawing.StartAt(1f, num131);
						goDrawing.CurveTo(1f, num131 + num130, num131 + num130, 1f, num131, 1f);
						goDrawing.CurveTo(num131 - num130, 1f, 0f, num131 + num130, 0f, num131);
						goDrawing.CurveTo(0f, num131 - num130, num131 - num130, 0f, num131, 0f);
						goDrawing.CurveTo(num131 + num130, 0f, 1f, num131 - num130, 1f, num131);
						break;
					}
					case GoFigure.Pie:
					{
						double num126 = Math.PI / 4.0;
						float num127 = (float)(4.0 * (1.0 - Math.Cos(num126)) / (3.0 * Math.Sin(num126))) * 0.5f;
						float num128 = 0.5f;
						PointF pointF17 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF17.X - num128, pointF17.Y);
						goDrawing.CurveTo(pointF17.X - num128, pointF17.Y - num127, pointF17.X - num127, pointF17.Y - num128, pointF17.X, pointF17.Y - num128);
						goDrawing.BreakUpBezier(new PointF(pointF17.X, pointF17.Y - num128), new PointF(pointF17.X + num127, pointF17.Y - num128), new PointF(pointF17.X + num128, pointF17.Y - num127), new PointF(pointF17.X + num128, pointF17.Y), 0.42f, out PointF curve1cp4, out PointF curve1cp5, out PointF midpoint4, out PointF curve2cp7, out curve2cp7);
						goDrawing.CurveTo(curve1cp4, curve1cp5, midpoint4);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.LineTo(pointF17.X + num128, pointF17.Y);
						goDrawing.CurveTo(pointF17.X + num128, pointF17.Y + num127, pointF17.X + num127, pointF17.Y + num128, pointF17.X, pointF17.Y + num128);
						goDrawing.CurveTo(pointF17.X - num127, pointF17.Y + num128, pointF17.X - num128, pointF17.Y + num127, pointF17.X - num128, pointF17.Y);
						break;
					}
					case GoFigure.PiePiece:
					{
						double num123 = Math.PI / 4.0;
						float num124 = (float)(4.0 * (1.0 - Math.Cos(num123)) / (3.0 * Math.Sin(num123))) * 0.5f;
						float num125 = 0.5f;
						PointF pointF16 = new PointF(0.5f, 0.5f);
						goDrawing.BreakUpBezier(new PointF(pointF16.X, pointF16.Y - num125), new PointF(pointF16.X + num124, pointF16.Y - num125), new PointF(pointF16.X + num125, pointF16.Y - num124), new PointF(pointF16.X + num125, pointF16.Y), 0.42f, out PointF curve1cp3, out curve1cp3, out PointF midpoint3, out PointF curve2cp5, out PointF curve2cp6);
						goDrawing.StartAt(midpoint3);
						goDrawing.CurveTo(curve2cp5, curve2cp6, new PointF(pointF16.X + num125, pointF16.Y));
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.LineTo(midpoint3);
						break;
					}
					case GoFigure.StopSign:
					{
						float num122 = (float)(1.0 / (Math.Sqrt(2.0) + 2.0));
						goDrawing.StartAt(num122, 0f);
						goDrawing.LineTo(1f - num122, 0f);
						goDrawing.LineTo(1f, num122);
						goDrawing.LineTo(1f, 1f - num122);
						goDrawing.LineTo(1f - num122, 1f);
						goDrawing.LineTo(num122, 1f);
						goDrawing.LineTo(0f, 1f - num122);
						goDrawing.LineTo(0f, num122);
						goDrawing.LineTo(num122, 0f);
						break;
					}
					case GoFigure.LogicImplies:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0.85f, 0.35f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.85f, 0.65f);
						break;
					case GoFigure.LogicIff:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0.85f, 0.35f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.85f, 0.65f);
						goDrawing.StartAt(0.15f, 0.35f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.LineTo(0.15f, 0.65f);
						break;
					case GoFigure.LogicNot:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.3f);
						break;
					case GoFigure.LogicAnd:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.LogicOr:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(1f, 0f);
						break;
					case GoFigure.LogicXor:
					{
						double num119 = Math.PI / 4.0;
						float num120 = (float)(4.0 * (1.0 - Math.Cos(num119)) / (3.0 * Math.Sin(num119))) * 0.5f;
						float num121 = 0.5f;
						goDrawing.StartAt(1f, num121);
						goDrawing.CurveTo(1f, num121 + num120, num121 + num120, 1f, num121, 1f);
						goDrawing.CurveTo(num121 - num120, 1f, 0f, num121 + num120, 0f, num121);
						goDrawing.CurveTo(0f, num121 - num120, num121 - num120, 0f, num121, 0f);
						goDrawing.CurveTo(num121 + num120, 0f, 1f, num121 - num120, 1f, num121);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					}
					case GoFigure.LogicTruth:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						break;
					case GoFigure.LogicFalsity:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						break;
					case GoFigure.LogicThereExists:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.LogicForAll:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.StartAt(0.25f, 0.5f);
						goDrawing.LineTo(0.75f, 0.5f);
						break;
					case GoFigure.LogicIsDefinedAs:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.LogicIntersect:
					{
						double num116 = Math.PI / 4.0;
						float num117 = (float)(4.0 * (1.0 - Math.Cos(num116)) / (3.0 * Math.Sin(num116))) * 0.5f;
						float num118 = 0.5f;
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0f, num118);
						goDrawing.CurveTo(0f, num118 - num117, num118 - num117, 0f, num118, 0f);
						goDrawing.CurveTo(num118 + num117, 0f, 1f, num118 - num117, 1f, num118);
						goDrawing.LineTo(1f, 1f);
						break;
					}
					case GoFigure.LogicUnion:
					{
						double num113 = Math.PI / 4.0;
						float num114 = (float)(4.0 * (1.0 - Math.Cos(num113)) / (3.0 * Math.Sin(num113))) * 0.5f;
						float num115 = 0.5f;
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, num115);
						goDrawing.CurveTo(1f, num115 + num114, num115 + num114, 1f, num115, 1f);
						goDrawing.CurveTo(num115 - num114, 1f, 0f, num115 + num114, 0f, num115);
						goDrawing.LineTo(0f, 0f);
						break;
					}
					case GoFigure.Arrow:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.7f, 0.7f);
						goDrawing.LineTo(0f, 0.7f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.Chevron:
					case GoFigure.ISOProcess:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.DoubleArrow:
						goDrawing.StartAt(0.4f, 0.333333343f);
						goDrawing.LineTo(0.4f, 0.6666666f);
						goDrawing.StartAt(0.4f, 0.333333343f);
						goDrawing.LineTo(0.4f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.4f, 1f);
						goDrawing.LineTo(0.4f, 0.6666666f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(0.4f, 0.333333343f);
						break;
					case GoFigure.DoubleEndArrow:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.7f, 0.7f);
						goDrawing.LineTo(0.3f, 0.7f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.LineTo(0.3f, 0f);
						goDrawing.LineTo(0.3f, 0.3f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.IBeamArrow:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.7f, 0.7f);
						goDrawing.LineTo(0.2f, 0.7f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(0.2f, 0f);
						goDrawing.LineTo(0.2f, 0.3f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.Pointer:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0.2f, 0.5f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.RoundedPointer:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.CurveTo(0.5f, 0.75f, 0.5f, 0.25f, 0f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.SplitEndArrow:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0.7f, 0.7f);
						goDrawing.LineTo(0f, 0.7f);
						goDrawing.LineTo(0.2f, 0.5f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.SquareArrow:
					case GoFigure.MessageToUser:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.Cone1:
					{
						double num109 = Math.PI / 4.0;
						float num110 = (float)(4.0 * (1.0 - Math.Cos(num109)) / (3.0 * Math.Sin(num109)));
						float num111 = num110 * 0.5f;
						float num112 = num110 * 0.1f;
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 0.9f + num112, 0.5f + num111, 1f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f - num111, 1f, 0f, 0.9f + num112, 0f, 0.9f);
						break;
					}
					case GoFigure.Cone2:
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.CurveTo(0.055555556f, 0.8f, 17f / 18f, 0.8f, 1f, 0.9f);
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 1f, 0f, 1f, 0f, 0.9f);
						goDrawing.CloseFigure(9);
						break;
					case GoFigure.Cube1:
						goDrawing.StartAt(0f, 0.15f);
						goDrawing.LineTo(0.5f, 0.3f);
						goDrawing.StartAt(0.5f, 1f);
						goDrawing.LineTo(0.5f, 0.3f);
						goDrawing.StartAt(1f, 0.15f);
						goDrawing.LineTo(0.5f, 0.3f);
						goDrawing.StartAt(0f, 0.15f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(1f, 0.15f);
						goDrawing.LineTo(1f, 0.85f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.85f);
						goDrawing.LineTo(0f, 0.15f);
						break;
					case GoFigure.Cube2:
						goDrawing.StartAt(0f, 0.3f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.StartAt(0.7f, 1f);
						goDrawing.LineTo(0.7f, 0.3f);
						goDrawing.StartAt(0f, 0.3f);
						goDrawing.LineTo(0.3f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.3f);
						break;
					case GoFigure.Cylinder1:
					case GoFigure.MagneticData:
					{
						double num105 = Math.PI / 4.0;
						float num106 = (float)(4.0 * (1.0 - Math.Cos(num105)) / (3.0 * Math.Sin(num105)));
						float num107 = num106 * 0.5f;
						float num108 = num106 * 0.1f;
						goDrawing.StartAt(0f, 0.1f);
						goDrawing.CurveTo(0f, 0.1f + num108, 0.5f - num107, 0.2f, 0.5f, 0.2f);
						goDrawing.CurveTo(0.5f + num107, 0.2f, 1f, 0.1f + num108, 1f, 0.1f);
						goDrawing.StartAt(0f, 0.1f);
						goDrawing.CurveTo(0f, 0.1f - num108, 0.5f - num107, 0f, 0.5f, 0f);
						goDrawing.CurveTo(0.5f + num107, 0f, 1f, 0.1f - num108, 1f, 0.1f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 0.9f + num108, 0.5f + num107, 1f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f - num107, 1f, 0f, 0.9f + num108, 0f, 0.9f);
						goDrawing.LineTo(0f, 0.1f);
						goDrawing.CloseFigure(21);
						break;
					}
					case GoFigure.Cylinder2:
					{
						double num101 = Math.PI / 4.0;
						float num102 = (float)(4.0 * (1.0 - Math.Cos(num101)) / (3.0 * Math.Sin(num101)));
						float num103 = num102 * 0.5f;
						float num104 = num102 * 0.1f;
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.CurveTo(0f, 0.9f - num104, 0.5f - num103, 0.8f, 0.5f, 0.8f);
						goDrawing.CurveTo(0.5f + num103, 0.8f, 1f, 0.9f - num104, 1f, 0.9f);
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.LineTo(0f, 0.1f);
						goDrawing.CurveTo(0f, 0.1f - num104, 0.5f - num103, 0f, 0.5f, 0f);
						goDrawing.CurveTo(0.5f + num103, 0f, 1f, 0.1f - num104, 1f, 0.1f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 0.9f + num104, 0.5f + num103, 1f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f - num103, 1f, 0f, 0.9f + num104, 0f, 0.9f);
						goDrawing.CloseFigure(21);
						break;
					}
					case GoFigure.Cylinder3:
					{
						double num97 = Math.PI / 4.0;
						float num98 = (float)(4.0 * (1.0 - Math.Cos(num97)) / (3.0 * Math.Sin(num97)));
						float num99 = num98 * 0.1f;
						float num100 = num98 * 0.5f;
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.CurveTo(0.1f + num99, 0f, 0.2f, 0.5f - num100, 0.2f, 0.5f);
						goDrawing.CurveTo(0.2f, 0.5f + num100, 0.1f + num99, 1f, 0.1f, 1f);
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.9f, 0f);
						goDrawing.CurveTo(0.9f + num99, 0f, 1f, 0.5f - num100, 1f, 0.5f);
						goDrawing.CurveTo(1f, 0.5f + num100, 0.9f + num99, 1f, 0.9f, 1f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.CurveTo(0.1f - num99, 1f, 0f, 0.5f + num100, 0f, 0.5f);
						goDrawing.CurveTo(0f, 0.5f - num100, 0.1f - num99, 0f, 0.1f, 0f);
						goDrawing.CloseFigure(21);
						break;
					}
					case GoFigure.Cylinder4:
					case GoFigure.DirectData:
					{
						double num93 = Math.PI / 4.0;
						float num94 = (float)(4.0 * (1.0 - Math.Cos(num93)) / (3.0 * Math.Sin(num93)));
						float num95 = num94 * 0.1f;
						float num96 = num94 * 0.5f;
						goDrawing.StartAt(0.9f, 0f);
						goDrawing.CurveTo(0.9f - num95, 0f, 0.8f, 0.5f - num96, 0.8f, 0.5f);
						goDrawing.CurveTo(0.8f, 0.5f + num96, 0.9f - num95, 1f, 0.9f, 1f);
						goDrawing.StartAt(0.9f, 0f);
						goDrawing.CurveTo(0.9f + num95, 0f, 1f, 0.5f - num96, 1f, 0.5f);
						goDrawing.CurveTo(1f, 0.5f + num96, 0.9f + num95, 1f, 0.9f, 1f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.CurveTo(0.1f - num95, 1f, 0f, 0.5f + num96, 0f, 0.5f);
						goDrawing.CurveTo(0f, 0.5f - num96, 0.1f - num95, 0f, 0.1f, 0f);
						goDrawing.LineTo(0.9f, 0f);
						goDrawing.CloseFigure(21);
						break;
					}
					case GoFigure.Prism1:
						goDrawing.StartAt(0.25f, 0.25f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(0.25f, 0.25f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 0.75f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0.25f, 0.25f);
						break;
					case GoFigure.Prism2:
						goDrawing.StartAt(0f, 0.25f);
						goDrawing.LineTo(0.25f, 0.5f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.25f, 0.5f);
						goDrawing.StartAt(1f, 0.25f);
						goDrawing.LineTo(0.25f, 0.5f);
						goDrawing.StartAt(0f, 0.25f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 0.25f);
						goDrawing.LineTo(0.75f, 0.75f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.25f);
						break;
					case GoFigure.Pyramid1:
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(1f, 0.75f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.75f);
						goDrawing.LineTo(0.5f, 0f);
						break;
					case GoFigure.Pyramid2:
						goDrawing.StartAt(0f, 0.85f);
						goDrawing.LineTo(0.5f, 0.7f);
						goDrawing.StartAt(0.5f, 0.7f);
						goDrawing.LineTo(1f, 0.85f);
						goDrawing.StartAt(0.5f, 0.7f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(1f, 0.85f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.85f);
						goDrawing.LineTo(0.5f, 0f);
						break;
					case GoFigure.Actor:
					{
						double num89 = Math.PI / 4.0;
						float num90 = (float)(4.0 * (1.0 - Math.Cos(num89)) / (3.0 * Math.Sin(num89)));
						SizeF sizeF5 = new SizeF(0.2f, 0.1f);
						SizeF sizeF6 = new SizeF(num90 * sizeF5.Width, num90 * sizeF5.Height);
						PointF pointF15 = new PointF(0.5f, 0.1f);
						goDrawing.StartAt(pointF15.X, pointF15.Y + sizeF5.Height);
						goDrawing.CurveTo(pointF15.X - sizeF6.Width, pointF15.Y + sizeF5.Height, pointF15.X - sizeF5.Width, pointF15.Y + sizeF6.Height, pointF15.X - sizeF5.Width, pointF15.Y);
						goDrawing.CurveTo(pointF15.X - sizeF5.Width, pointF15.Y - sizeF6.Height, pointF15.X - sizeF6.Width, pointF15.Y - sizeF5.Height, pointF15.X, pointF15.Y - sizeF5.Height);
						goDrawing.CurveTo(pointF15.X + sizeF6.Width, pointF15.Y - sizeF5.Height, pointF15.X + sizeF5.Width, pointF15.Y - sizeF6.Height, pointF15.X + sizeF5.Width, pointF15.Y);
						goDrawing.CurveTo(pointF15.X + sizeF5.Width, pointF15.Y + sizeF6.Height, pointF15.X + sizeF6.Width, pointF15.Y + sizeF5.Height, pointF15.X, pointF15.Y + sizeF5.Height);
						float num91 = 0.05f;
						float num92 = num90 * num91;
						pointF15 = new PointF(0.05f, 0.25f);
						goDrawing.StartAt(0.5f, 0.2f);
						goDrawing.LineTo(0.95f, 0.2f);
						pointF15 = new PointF(0.95f, 0.25f);
						goDrawing.CurveTo(pointF15.X + num92, pointF15.Y - num91, pointF15.X + num91, pointF15.Y - num92, pointF15.X + num91, pointF15.Y);
						goDrawing.LineTo(1f, 0.6f);
						goDrawing.LineTo(0.85f, 0.6f);
						goDrawing.LineTo(0.85f, 0.35f);
						num91 = 0.025f;
						num92 = num90 * num91;
						pointF15 = new PointF(0.825f, 0.35f);
						goDrawing.CurveTo(pointF15.X + num91, pointF15.Y - num92, pointF15.X + num92, pointF15.Y - num91, pointF15.X, pointF15.Y - num91);
						goDrawing.CurveTo(pointF15.X - num92, pointF15.Y - num91, pointF15.X - num91, pointF15.Y - num92, pointF15.X - num91, pointF15.Y);
						goDrawing.LineTo(0.8f, 1f);
						goDrawing.LineTo(0.55f, 1f);
						goDrawing.LineTo(0.55f, 0.7f);
						num91 = 0.05f;
						num92 = num90 * num91;
						pointF15 = new PointF(0.5f, 0.7f);
						goDrawing.CurveTo(pointF15.X + num91, pointF15.Y - num92, pointF15.X + num92, pointF15.Y - num91, pointF15.X, pointF15.Y - num91);
						goDrawing.CurveTo(pointF15.X - num92, pointF15.Y - num91, pointF15.X - num91, pointF15.Y - num92, pointF15.X - num91, pointF15.Y);
						goDrawing.LineTo(0.45f, 1f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.LineTo(0.2f, 0.35f);
						num91 = 0.025f;
						num92 = num90 * num91;
						pointF15 = new PointF(0.175f, 0.35f);
						goDrawing.CurveTo(pointF15.X + num91, pointF15.Y - num92, pointF15.X + num92, pointF15.Y - num91, pointF15.X, pointF15.Y - num91);
						goDrawing.CurveTo(pointF15.X - num92, pointF15.Y - num91, pointF15.X - num91, pointF15.Y - num92, pointF15.X - num91, pointF15.Y);
						goDrawing.LineTo(0.15f, 0.6f);
						goDrawing.LineTo(0f, 0.6f);
						goDrawing.LineTo(0f, 0.25f);
						num91 = 0.05f;
						num92 = num90 * num91;
						pointF15 = new PointF(0.05f, 0.25f);
						goDrawing.CurveTo(pointF15.X - num91, pointF15.Y - num92, pointF15.X - num92, pointF15.Y - num91, pointF15.X, pointF15.Y - num91);
						goDrawing.LineTo(0.5f, 0.2f);
						break;
					}
					case GoFigure.Card:
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.LineTo(0.2f, 0f);
						goDrawing.LineTo(1f, 0f);
						break;
					case GoFigure.Collate:
						goDrawing.StartAt(0.5f, 0.5f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.StartAt(0.5f, 0.5f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0.5f, 0.5f);
						break;
					case GoFigure.CreateRequest:
						goDrawing.StartAt(0f, 0.1f);
						goDrawing.LineTo(1f, 0.1f);
						goDrawing.StartAt(0f, 0.9f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Database:
					{
						double num85 = Math.PI / 4.0;
						float num86 = (float)(4.0 * (1.0 - Math.Cos(num85)) / (3.0 * Math.Sin(num85)));
						float num87 = num86 * 0.5f;
						float num88 = num86 * 0.1f;
						goDrawing.StartAt(1f, 0.1f);
						goDrawing.CurveTo(1f, 0.1f + num88, 0.5f + num87, 0.2f, 0.5f, 0.2f);
						goDrawing.CurveTo(0.5f - num87, 0.2f, 0f, 0.1f + num88, 0f, 0.1f);
						goDrawing.StartAt(1f, 0.2f);
						goDrawing.CurveTo(1f, 0.2f + num88, 0.5f + num87, 0.3f, 0.5f, 0.3f);
						goDrawing.CurveTo(0.5f - num87, 0.3f, 0f, 0.2f + num88, 0f, 0.2f);
						goDrawing.StartAt(1f, 0.3f);
						goDrawing.CurveTo(1f, 0.3f + num88, 0.5f + num87, 0.4f, 0.5f, 0.4f);
						goDrawing.CurveTo(0.5f - num87, 0.4f, 0f, 0.3f + num88, 0f, 0.3f);
						goDrawing.StartAt(1f, 0.1f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 0.9f + num88, 0.5f + num87, 1f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f - num87, 1f, 0f, 0.9f + num88, 0f, 0.9f);
						goDrawing.LineTo(0f, 0.1f);
						goDrawing.CurveTo(0f, 0.1f - num88, 0.5f - num87, 0f, 0.5f, 0f);
						goDrawing.CurveTo(0.5f + num87, 0f, 1f, 0.1f - num88, 1f, 0.1f);
						goDrawing.CloseFigure(35);
						break;
					}
					case GoFigure.DataStorage:
					{
						double num81 = Math.PI / 4.0;
						float num82 = (float)(4.0 * (1.0 - Math.Cos(num81)) / (3.0 * Math.Sin(num81)));
						float num83 = 0.25f;
						float num84 = num82 * num83;
						goDrawing.StartAt(0f, 0.25f);
						goDrawing.LineTo(0.75f, 0.25f);
						goDrawing.CurveTo(0.75f + num84, 0.25f, 1f, 0.5f - num84, 1f, 0.5f);
						goDrawing.CurveTo(1f, 0.5f + num84, 0.75f + num84, 0.75f, 0.75f, 0.75f);
						goDrawing.LineTo(0f, 0.75f);
						goDrawing.CurveTo(num84, 0.75f, 0.25f, 0.5f + num84, 0.25f, 0.5f);
						goDrawing.CurveTo(0.25f, 0.5f - num84, num84, 0.25f, 0f, 0.25f);
						break;
					}
					case GoFigure.DiskStorage:
					{
						double num77 = Math.PI / 4.0;
						float num78 = (float)(4.0 * (1.0 - Math.Cos(num77)) / (3.0 * Math.Sin(num77)));
						float num79 = num78 * 0.5f;
						float num80 = num78 * 0.1f;
						goDrawing.StartAt(1f, 0.1f);
						goDrawing.CurveTo(1f, 0.1f + num80, 0.5f + num79, 0.2f, 0.5f, 0.2f);
						goDrawing.CurveTo(0.5f - num79, 0.2f, 0f, 0.1f + num80, 0f, 0.1f);
						goDrawing.StartAt(1f, 0.2f);
						goDrawing.CurveTo(1f, 0.2f + num80, 0.5f + num79, 0.3f, 0.5f, 0.3f);
						goDrawing.CurveTo(0.5f - num79, 0.3f, 0f, 0.2f + num80, 0f, 0.2f);
						goDrawing.StartAt(1f, 0.1f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.CurveTo(1f, 0.9f + num80, 0.5f + num79, 1f, 0.5f, 1f);
						goDrawing.CurveTo(0.5f - num79, 1f, 0f, 0.9f + num80, 0f, 0.9f);
						goDrawing.LineTo(0f, 0.1f);
						goDrawing.CurveTo(0f, 0.1f - num80, 0.5f - num79, 0f, 0.5f, 0f);
						goDrawing.CurveTo(0.5f + num79, 0f, 1f, 0.1f - num80, 1f, 0.1f);
						goDrawing.CloseFigure(28);
						break;
					}
					case GoFigure.Display:
						goDrawing.StartAt(0.25f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.CurveTo(1f, 0f, 1f, 1f, 0.75f, 1f);
						goDrawing.LineTo(0.25f, 1f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.LineTo(0.25f, 0f);
						break;
					case GoFigure.DividedEvent:
					{
						double num75 = Math.PI / 4.0;
						float num76 = (float)(4.0 * (1.0 - Math.Cos(num75)) / (3.0 * Math.Sin(num75))) * 0.2f;
						goDrawing.StartAt(0f, 0.2f);
						goDrawing.LineTo(1f, 0.2f);
						goDrawing.StartAt(0f, 0.2f);
						goDrawing.CurveTo(0f, 0.2f - num76, 0.2f - num76, 0f, 0.2f, 0f);
						goDrawing.LineTo(0.8f, 0f);
						goDrawing.CurveTo(0.8f + num76, 0f, 1f, 0.2f - num76, 1f, 0.2f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.CurveTo(1f, 0.8f + num76, 0.8f + num76, 1f, 0.8f, 1f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.CurveTo(0.2f - num76, 1f, 0f, 0.8f + num76, 0f, 0.8f);
						goDrawing.LineTo(0f, 0.2f);
						break;
					}
					case GoFigure.DividedProcess:
						goDrawing.StartAt(0f, 0.1f);
						goDrawing.LineTo(1f, 0.1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Document:
						goDrawing.StartAt(0f, 0.7f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.CurveTo(0.5f, 0.4f, 0.5f, 1f, 0f, 0.7f);
						break;
					case GoFigure.ExternalOrganization:
						goDrawing.StartAt(0.2f, 0f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.StartAt(1f, 0.2f);
						goDrawing.LineTo(0.8f, 0f);
						goDrawing.StartAt(0f, 0.8f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.StartAt(0.8f, 1f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.ExternalProcess:
						goDrawing.StartAt(0.1f, 0.4f);
						goDrawing.LineTo(0.1f, 0.6f);
						goDrawing.StartAt(0.9f, 0.6f);
						goDrawing.LineTo(0.9f, 0.4f);
						goDrawing.StartAt(0.6f, 0.1f);
						goDrawing.LineTo(0.4f, 0.1f);
						goDrawing.StartAt(0.4f, 0.9f);
						goDrawing.LineTo(0.6f, 0.9f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.LineTo(0.5f, 0f);
						break;
					case GoFigure.File:
						goDrawing.StartAt(0.75f, 0f);
						goDrawing.LineTo(0.75f, 0.25f);
						goDrawing.LineTo(1f, 0.25f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 0.25f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.CloseFigure(8);
						break;
					case GoFigure.Interupt:
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						break;
					case GoFigure.InternalStorage:
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.StartAt(0f, 0.1f);
						goDrawing.LineTo(1f, 0.1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Junction:
					{
						double num69 = Math.PI / 4.0;
						float num70 = (float)(4.0 * (1.0 - Math.Cos(num69)) / (3.0 * Math.Sin(num69)));
						float num71 = (float)(1.0 / Math.Sqrt(2.0));
						float num72 = (float)((1.0 - 1.0 / Math.Sqrt(2.0)) / 2.0);
						float num73 = num70 * 0.5f;
						float num74 = 0.5f;
						goDrawing.StartAt(num72 + num71, num72 + num71);
						goDrawing.LineTo(num72, num72);
						goDrawing.StartAt(num72, num72 + num71);
						goDrawing.LineTo(num72 + num71, num72);
						goDrawing.StartAt(1f, num74);
						goDrawing.CurveTo(1f, num74 + num73, num74 + num73, 1f, num74, 1f);
						goDrawing.CurveTo(num74 - num73, 1f, 0f, num74 + num73, 0f, num74);
						goDrawing.CurveTo(0f, num74 - num73, num74 - num73, 0f, num74, 0f);
						goDrawing.CurveTo(num74 + num73, 0f, 1f, num74 - num73, 1f, num74);
						break;
					}
					case GoFigure.LinedDocument:
						goDrawing.StartAt(0.117751122f, 0f);
						goDrawing.LineTo(0.117751122f, 0.967692852f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.8789773f);
						goDrawing.CurveTo(0.5576732f, 0.5457239f, 0.506654f, 1.09692192f, 0.117751122f, 0.967692852f);
						goDrawing.CurveTo(0.09145379f, 0.9564233f, 0.0421640873f, 0.9392872f, 0f, 0.8789773f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.LoopLimit:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0f, 0.25f);
						goDrawing.LineTo(0.25f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 0.25f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						break;
					case GoFigure.MagneticTape:
					case GoFigure.SequentialData:
					{
						double num66 = Math.PI / 4.0;
						float num67 = (float)(4.0 * (1.0 - Math.Cos(num66)) / (3.0 * Math.Sin(num66))) * 0.5f;
						float num68 = 0.5f;
						goDrawing.StartAt(0.5f, 1f);
						goDrawing.CurveTo(num68 - num67, 1f, 0f, num68 + num67, 0f, num68);
						goDrawing.CurveTo(0f, num68 - num67, num68 - num67, 0f, num68, 0f);
						goDrawing.CurveTo(num68 + num67, 0f, 1f, num68 - num67, 1f, num68);
						goDrawing.CurveTo(1f, num68 + num67, num68 + num67, 0.9f, num68 + 0.1f, 0.9f);
						goDrawing.LineTo(1f, 0.9f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0.5f, 1f);
						break;
					}
					case GoFigure.ManualInput:
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.25f);
						goDrawing.LineTo(1f, 0f);
						break;
					case GoFigure.MessageFromUser:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.7f, 0.5f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.MicroformProcessing:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 0.25f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0.5f, 0.75f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.MicroformRecording:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.75f, 0.25f);
						goDrawing.LineTo(1f, 0.15f);
						goDrawing.LineTo(1f, 0.85f);
						goDrawing.LineTo(0.75f, 0.75f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.MultiDocument:
						goDrawing.StartAt(0.1f, 0.2f);
						goDrawing.LineTo(0.8f, 0.2f);
						goDrawing.LineTo(0.8f, 0.54f);
						goDrawing.StartAt(0.2f, 0.1f);
						goDrawing.LineTo(0.9f, 0.1f);
						goDrawing.LineTo(0.9f, 0.44f);
						goDrawing.StartAt(0.1f, 0.1f);
						goDrawing.LineTo(0.2f, 0.1f);
						goDrawing.LineTo(0.2f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.CurveTo(0.96f, 0.47f, 0.93f, 0.45f, 0.9f, 0.44f);
						goDrawing.LineTo(0.9f, 0.6f);
						goDrawing.CurveTo(0.86f, 0.57f, 0.83f, 0.55f, 0.8f, 0.54f);
						goDrawing.LineTo(0.8f, 0.7f);
						goDrawing.CurveTo(0.4f, 0.4f, 0.4f, 1f, 0f, 0.7f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.LineTo(0.1f, 0.2f);
						goDrawing.LineTo(0.1f, 0.1f);
						goDrawing.CloseFigure(15);
						break;
					case GoFigure.MultiProcess:
						goDrawing.StartAt(0.2f, 0.1f);
						goDrawing.LineTo(0.9f, 0.1f);
						goDrawing.LineTo(0.9f, 0.8f);
						goDrawing.StartAt(0.1f, 0.2f);
						goDrawing.LineTo(0.8f, 0.2f);
						goDrawing.LineTo(0.8f, 0.9f);
						goDrawing.StartAt(0.1f, 0.1f);
						goDrawing.LineTo(0.2f, 0.1f);
						goDrawing.LineTo(0.2f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.LineTo(0.9f, 0.8f);
						goDrawing.LineTo(0.9f, 0.9f);
						goDrawing.LineTo(0.8f, 0.9f);
						goDrawing.LineTo(0.8f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.LineTo(0.1f, 0.2f);
						goDrawing.LineTo(0.1f, 0.1f);
						goDrawing.CloseFigure(18);
						break;
					case GoFigure.OfflineStorage:
						goDrawing.StartAt(0.1f, 0.2f);
						goDrawing.LineTo(0.9f, 0.2f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.OffPageConnector:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.75f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Or:
					{
						double num63 = Math.PI / 4.0;
						float num64 = (float)(4.0 * (1.0 - Math.Cos(num63)) / (3.0 * Math.Sin(num63))) * 0.5f;
						float num65 = 0.5f;
						goDrawing.StartAt(1f, 0.5f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.StartAt(0.5f, 1f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.StartAt(1f, num65);
						goDrawing.CurveTo(1f, num65 + num64, num65 + num64, 1f, num65, 1f);
						goDrawing.CurveTo(num65 - num64, 1f, 0f, num65 + num64, 0f, num65);
						goDrawing.CurveTo(0f, num65 - num64, num65 - num64, 0f, num65, 0f);
						goDrawing.CurveTo(num65 + num64, 0f, 1f, num65 - num64, 1f, num65);
						break;
					}
					case GoFigure.PaperTape:
						goDrawing.StartAt(0f, 0.7f);
						goDrawing.LineTo(0f, 0.3f);
						goDrawing.CurveTo(0.5f, 0.6f, 0.5f, 0f, 1f, 0.3f);
						goDrawing.LineTo(1f, 0.7f);
						goDrawing.CurveTo(0.5f, 0.4f, 0.5f, 1f, 0f, 0.7f);
						break;
					case GoFigure.PrimitiveFromCall:
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.7f, 0.5f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.PrimitiveToCall:
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.7f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Procedure:
					case GoFigure.Subroutine:
						goDrawing.StartAt(0.9f, 0f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Process:
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Sort:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.LineTo(0.5f, 0f);
						break;
					case GoFigure.Start:
						goDrawing.StartAt(0.25f, 0f);
						goDrawing.LineTo(0.25f, 1f);
						goDrawing.StartAt(0.75f, 0f);
						goDrawing.LineTo(0.75f, 1f);
						goDrawing.StartAt(0.25f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.CurveTo(1f, 0f, 1f, 1f, 0.75f, 1f);
						goDrawing.LineTo(0.25f, 1f);
						goDrawing.CurveTo(0f, 1f, 0f, 0f, 0.25f, 0f);
						break;
					case GoFigure.StoredData:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.CurveTo(1f, 0f, 1f, 1f, 0.75f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.CurveTo(0.25f, 1f, 0.25f, 0f, 0f, 0f);
						break;
					case GoFigure.Terminator:
						goDrawing.StartAt(0.25f, 0f);
						goDrawing.LineTo(0.75f, 0f);
						goDrawing.CurveTo(1f, 0f, 1f, 1f, 0.75f, 1f);
						goDrawing.LineTo(0.25f, 1f);
						goDrawing.CurveTo(0f, 1f, 0f, 0f, 0.25f, 0f);
						break;
					case GoFigure.TransmittalTape:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0.75f, 0.9f);
						goDrawing.LineTo(0f, 0.9f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.AndGate:
					{
						double num61 = Math.PI / 4.0;
						float num62 = (float)(4.0 * (1.0 - Math.Cos(num61)) / (3.0 * Math.Sin(num61))) * 0.5f;
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.CurveTo(0.5f + num62, 0f, 1f, 0.5f - num62, 1f, 0.5f);
						goDrawing.CurveTo(1f, 0.5f + num62, 0.5f + num62, 1f, 0.5f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					}
					case GoFigure.Buffer:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						break;
					case GoFigure.Clock:
					{
						double num58 = Math.PI / 4.0;
						float num59 = (float)(4.0 * (1.0 - Math.Cos(num58)) / (3.0 * Math.Sin(num58))) * 0.5f;
						float num60 = 0.5f;
						goDrawing.StartAt(1f, num60);
						goDrawing.LineTo(1f, num60);
						goDrawing.StartAt(0.8f, 0.75f);
						goDrawing.LineTo(0.8f, 0.25f);
						goDrawing.LineTo(0.6f, 0.25f);
						goDrawing.LineTo(0.6f, 0.75f);
						goDrawing.LineTo(0.4f, 0.75f);
						goDrawing.LineTo(0.4f, 0.25f);
						goDrawing.LineTo(0.2f, 0.25f);
						goDrawing.LineTo(0.2f, 0.75f);
						goDrawing.StartAt(1f, num60);
						goDrawing.CurveTo(1f, num60 + num59, num60 + num59, 1f, num60, 1f);
						goDrawing.CurveTo(num60 - num59, 1f, 0f, num60 + num59, 0f, num60);
						goDrawing.CurveTo(0f, num60 - num59, num60 - num59, 0f, num60, 0f);
						goDrawing.CurveTo(num60 + num59, 0f, 1f, num60 - num59, 1f, num60);
						goDrawing.CloseFigure(20);
						break;
					}
					case GoFigure.Ground:
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 0.4f);
						goDrawing.StartAt(0.2f, 0.6f);
						goDrawing.LineTo(0.8f, 0.6f);
						goDrawing.StartAt(0.3f, 0.8f);
						goDrawing.LineTo(0.7f, 0.8f);
						goDrawing.StartAt(0.4f, 1f);
						goDrawing.LineTo(0.6f, 1f);
						break;
					case GoFigure.Inverter:
					{
						double num55 = Math.PI / 4.0;
						float num56 = (float)(4.0 * (1.0 - Math.Cos(num55)) / (3.0 * Math.Sin(num55))) * 0.1f;
						float num57 = 0.1f;
						PointF pointF14 = new PointF(0.9f, 0.5f);
						goDrawing.StartAt(pointF14.X + num57, pointF14.Y);
						goDrawing.CurveTo(pointF14.X + num57, pointF14.Y + num56, pointF14.X + num56, pointF14.Y + num57, pointF14.X, pointF14.Y + num57);
						goDrawing.CurveTo(pointF14.X - num56, pointF14.Y + num57, pointF14.X - num57, pointF14.Y + num56, pointF14.X - num57, pointF14.Y);
						goDrawing.CurveTo(pointF14.X - num57, pointF14.Y - num56, pointF14.X - num56, pointF14.Y - num57, pointF14.X, pointF14.Y - num57);
						goDrawing.CurveTo(pointF14.X + num56, pointF14.Y - num57, pointF14.X + num57, pointF14.Y - num56, pointF14.X + num57, pointF14.Y);
						goDrawing.StartAt(0.8f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(0.8f, 0.5f);
						break;
					}
					case GoFigure.NandGate:
					{
						double num49 = Math.PI / 4.0;
						float num50 = (float)(4.0 * (1.0 - Math.Cos(num49)) / (3.0 * Math.Sin(num49)));
						float num51 = num50 * 0.5f;
						float num52 = num50 * 0.4f;
						float num53 = num50 * 0.1f;
						float num54 = 0.1f;
						PointF pointF13 = new PointF(0.9f, 0.5f);
						goDrawing.StartAt(pointF13.X + num54, pointF13.Y);
						goDrawing.CurveTo(pointF13.X + num54, pointF13.Y + num53, pointF13.X + num53, pointF13.Y + num54, pointF13.X, pointF13.Y + num54);
						goDrawing.CurveTo(pointF13.X - num53, pointF13.Y + num54, pointF13.X - num54, pointF13.Y + num53, pointF13.X - num54, pointF13.Y);
						goDrawing.CurveTo(pointF13.X - num54, pointF13.Y - num53, pointF13.X - num53, pointF13.Y - num54, pointF13.X, pointF13.Y - num54);
						goDrawing.CurveTo(pointF13.X + num53, pointF13.Y - num54, pointF13.X + num54, pointF13.Y - num53, pointF13.X + num54, pointF13.Y);
						goDrawing.StartAt(0.8f, 0.5f);
						goDrawing.CurveTo(0.8f, 0.5f + num52, 0.4f + num51, 1f, 0.4f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.LineTo(0.4f, 0f);
						goDrawing.CurveTo(0.4f + num51, 0f, 0.8f, 0.5f - num52, 0.8f, 0.5f);
						break;
					}
					case GoFigure.NorGate:
					{
						double num45 = Math.PI / 4.0;
						float num46 = (float)(4.0 * (1.0 - Math.Cos(num45)) / (3.0 * Math.Sin(num45)));
						float num47 = num46 * 0.1f;
						float num48 = 0.1f;
						PointF pointF12 = new PointF(0.9f, 0.5f);
						goDrawing.StartAt(pointF12.X - num48, pointF12.Y);
						goDrawing.CurveTo(pointF12.X - num48, pointF12.Y - num47, pointF12.X - num47, pointF12.Y - num48, pointF12.X, pointF12.Y - num48);
						goDrawing.CurveTo(pointF12.X + num47, pointF12.Y - num48, pointF12.X + num48, pointF12.Y - num47, pointF12.X + num48, pointF12.Y);
						goDrawing.CurveTo(pointF12.X + num48, pointF12.Y + num47, pointF12.X + num47, pointF12.Y + num48, pointF12.X, pointF12.Y + num48);
						goDrawing.CurveTo(pointF12.X - num47, pointF12.Y + num48, pointF12.X - num48, pointF12.Y + num47, pointF12.X - num48, pointF12.Y);
						num48 = 0.5f;
						num47 = num46 * num48;
						pointF12 = new PointF(0f, 0.5f);
						goDrawing.StartAt(0.8f, 0.5f);
						goDrawing.CurveTo(0.7f, pointF12.Y + num47, pointF12.X + num47, pointF12.Y + num48, 0f, 1f);
						goDrawing.CurveTo(0.25f, 0.75f, 0.25f, 0.25f, 0f, 0f);
						goDrawing.CurveTo(pointF12.X + num47, pointF12.Y - num48, 0.7f, pointF12.Y - num47, 0.8f, 0.5f);
						break;
					}
					case GoFigure.OrGate:
					{
						double num41 = Math.PI / 4.0;
						float num42 = (float)(4.0 * (1.0 - Math.Cos(num41)) / (3.0 * Math.Sin(num41)));
						float num43 = 0.5f;
						float num44 = num42 * num43;
						PointF pointF11 = new PointF(0f, 0.5f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.CurveTo(pointF11.X + num44 + num44, pointF11.Y - num43, 0.8f, pointF11.Y - num44, 1f, 0.5f);
						goDrawing.CurveTo(0.8f, pointF11.Y + num44, pointF11.X + num44 + num44, pointF11.Y + num43, 0f, 1f);
						goDrawing.CurveTo(0.25f, 0.75f, 0.25f, 0.25f, 0f, 0f);
						break;
					}
					case GoFigure.XnorGate:
					{
						double num37 = Math.PI / 4.0;
						float num38 = (float)(4.0 * (1.0 - Math.Cos(num37)) / (3.0 * Math.Sin(num37)));
						float num39 = num38 * 0.1f;
						float num40 = 0.1f;
						PointF pointF10 = new PointF(0.9f, 0.5f);
						goDrawing.StartAt(pointF10.X - num40, pointF10.Y);
						goDrawing.CurveTo(pointF10.X - num40, pointF10.Y - num39, pointF10.X - num39, pointF10.Y - num40, pointF10.X, pointF10.Y - num40);
						goDrawing.CurveTo(pointF10.X + num39, pointF10.Y - num40, pointF10.X + num40, pointF10.Y - num39, pointF10.X + num40, pointF10.Y);
						goDrawing.CurveTo(pointF10.X + num40, pointF10.Y + num39, pointF10.X + num39, pointF10.Y + num40, pointF10.X, pointF10.Y + num40);
						goDrawing.CurveTo(pointF10.X - num39, pointF10.Y + num40, pointF10.X - num40, pointF10.Y + num39, pointF10.X - num40, pointF10.Y);
						goDrawing.CloseFigure(0);
						num40 = 0.5f;
						num39 = num38 * num40;
						pointF10 = new PointF(0.2f, 0.5f);
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.CurveTo(0.35f, 0.25f, 0.35f, 0.75f, 0.1f, 1f);
						goDrawing.StartAt(0.2f, 0f);
						goDrawing.StartAt(0.8f, 0.5f);
						goDrawing.CurveTo(0.7f, pointF10.Y + num39, pointF10.X + num39, pointF10.Y + num40, 0.2f, 1f);
						goDrawing.CurveTo(0.45f, 0.75f, 0.45f, 0.25f, 0.2f, 0f);
						goDrawing.CurveTo(pointF10.X + num39, pointF10.Y - num40, 0.7f, pointF10.Y - num39, 0.8f, 0.5f);
						goDrawing.CloseFigure(20);
						break;
					}
					case GoFigure.XorGate:
					{
						double num33 = Math.PI / 4.0;
						float num34 = (float)(4.0 * (1.0 - Math.Cos(num33)) / (3.0 * Math.Sin(num33)));
						float num35 = 0.5f;
						float num36 = num34 * num35;
						PointF pointF9 = new PointF(0.2f, 0.5f);
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.CurveTo(0.35f, 0.25f, 0.35f, 0.75f, 0.1f, 1f);
						goDrawing.StartAt(0.2f, 0f);
						goDrawing.CurveTo(pointF9.X + num36, pointF9.Y - num35, 0.9f, pointF9.Y - num36, 1f, 0.5f);
						goDrawing.CurveTo(0.9f, pointF9.Y + num36, pointF9.X + num36, pointF9.Y + num35, 0.2f, 1f);
						goDrawing.CurveTo(0.45f, 0.75f, 0.45f, 0.25f, 0.2f, 0f);
						goDrawing.CloseFigure(13);
						break;
					}
					case GoFigure.Capacitor:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.Resistor:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(0.1f, 0f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.LineTo(0.3f, 0f);
						goDrawing.LineTo(0.4f, 1f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(0.6f, 1f);
						goDrawing.LineTo(0.7f, 0.5f);
						break;
					case GoFigure.Inductor:
					{
						double num30 = Math.PI / 4.0;
						float num31 = (float)(4.0 * (1.0 - Math.Cos(num30)) / (3.0 * Math.Sin(num30))) * 0.1f;
						float num32 = 0.1f;
						PointF pointF8 = new PointF(0.1f, 0.5f);
						goDrawing.StartAt(pointF8.X - num31 * 0.5f, pointF8.Y + num32);
						goDrawing.CurveTo(pointF8.X - num31, pointF8.Y + num32, pointF8.X - num32, pointF8.Y + num31, pointF8.X + num32, pointF8.Y + num31);
						pointF8 = new PointF(0.3f, 0.5f);
						goDrawing.CurveTo(pointF8.X + num32, pointF8.Y + num31, pointF8.X + num31, pointF8.Y + num32, pointF8.X, pointF8.Y + num32);
						goDrawing.CurveTo(pointF8.X - num31, pointF8.Y + num32, pointF8.X - num32, pointF8.Y + num31, pointF8.X + num32, pointF8.Y + num31);
						pointF8 = new PointF(0.5f, 0.5f);
						goDrawing.CurveTo(pointF8.X + num32, pointF8.Y + num31, pointF8.X + num31, pointF8.Y + num32, pointF8.X, pointF8.Y + num32);
						goDrawing.CurveTo(pointF8.X - num31, pointF8.Y + num32, pointF8.X - num32, pointF8.Y + num31, pointF8.X + num32, pointF8.Y + num31);
						pointF8 = new PointF(0.7f, 0.5f);
						goDrawing.CurveTo(pointF8.X + num32, pointF8.Y + num31, pointF8.X + num31, pointF8.Y + num32, pointF8.X, pointF8.Y + num32);
						goDrawing.CurveTo(pointF8.X - num31, pointF8.Y + num32, pointF8.X - num32, pointF8.Y + num31, pointF8.X + num32, pointF8.Y + num31);
						pointF8 = new PointF(0.9f, 0.5f);
						goDrawing.CurveTo(pointF8.X + num32, pointF8.Y + num31, pointF8.X + num31, pointF8.Y + num32, pointF8.X + num31 * 0.5f, pointF8.Y + num32);
						break;
					}
					case GoFigure.ACvoltageSource:
					{
						double num27 = Math.PI / 4.0;
						float num28 = (float)(4.0 * (1.0 - Math.Cos(num27)) / (3.0 * Math.Sin(num27))) * 0.5f;
						float num29 = 0.5f;
						PointF pointF7 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF7.X - num29, pointF7.Y);
						goDrawing.CurveTo(pointF7.X - num29, pointF7.Y - num28, pointF7.X - num28, pointF7.Y - num29, pointF7.X, pointF7.Y - num29);
						goDrawing.CurveTo(pointF7.X + num28, pointF7.Y - num29, pointF7.X + num29, pointF7.Y - num28, pointF7.X + num29, pointF7.Y);
						goDrawing.CurveTo(pointF7.X + num29, pointF7.Y + num28, pointF7.X + num28, pointF7.Y + num29, pointF7.X, pointF7.Y + num29);
						goDrawing.CurveTo(pointF7.X - num28, pointF7.Y + num29, pointF7.X - num29, pointF7.Y + num28, pointF7.X - num29, pointF7.Y);
						goDrawing.StartAt(pointF7.X - num29 + 0.1f, pointF7.Y);
						goDrawing.CurveTo(pointF7.X, pointF7.Y - num29, pointF7.X, pointF7.Y + num29, pointF7.X + num29 - 0.1f, pointF7.Y);
						break;
					}
					case GoFigure.DCvoltageSource:
						goDrawing.StartAt(0f, 0.75f);
						goDrawing.LineTo(0f, 0.25f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.Diode:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.Wifi:
					{
						double num23 = Math.PI / 4.0;
						float num24 = (float)(4.0 * (1.0 - Math.Cos(num23)) / (3.0 * Math.Sin(num23)));
						float num25 = num24 * 0.2f;
						float num26 = 0.2f;
						PointF pointF6 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF6.X - num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X - num26, pointF6.Y - num25, pointF6.X - num25, pointF6.Y - num26, pointF6.X, pointF6.Y - num26);
						goDrawing.CurveTo(pointF6.X + num25, pointF6.Y - num26, pointF6.X + num26, pointF6.Y - num25, pointF6.X + num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X + num26, pointF6.Y + num25, pointF6.X + num25, pointF6.Y + num26, pointF6.X, pointF6.Y + num26);
						goDrawing.CurveTo(pointF6.X - num25, pointF6.Y + num26, pointF6.X - num26, pointF6.Y + num25, pointF6.X - num26, pointF6.Y);
						num25 = num24 * 0.4f;
						num26 = 0.4f;
						pointF6 = new PointF(0.8f, 0.5f);
						goDrawing.StartAt(pointF6.X, pointF6.Y - num26);
						goDrawing.CurveTo(pointF6.X + num25, pointF6.Y - num26, pointF6.X + num26, pointF6.Y - num25, pointF6.X + num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X + num26, pointF6.Y + num25, pointF6.X + num25, pointF6.Y + num26, pointF6.X, pointF6.Y + num26);
						goDrawing.CurveTo(pointF6.X, pointF6.Y + num26, pointF6.X + num26 - num25 * 0.5f, pointF6.Y + num25, pointF6.X + num26 - num25 * 0.5f, pointF6.Y);
						goDrawing.CurveTo(pointF6.X + num26 - num25 * 0.5f, pointF6.Y - num25, pointF6.X, pointF6.Y - num26, pointF6.X, pointF6.Y - num26);
						num25 = num24 * 0.8f;
						num26 = 0.8f;
						pointF6 = new PointF(1f, 0.5f);
						goDrawing.StartAt(pointF6.X, pointF6.Y - num26);
						goDrawing.CurveTo(pointF6.X + num25, pointF6.Y - num26, pointF6.X + num26, pointF6.Y - num25, pointF6.X + num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X + num26, pointF6.Y + num25, pointF6.X + num25, pointF6.Y + num26, pointF6.X, pointF6.Y + num26);
						goDrawing.CurveTo(pointF6.X, pointF6.Y + num26, pointF6.X + num26 - num25 * 0.5f, pointF6.Y + num25, pointF6.X + num26 - num25 * 0.5f, pointF6.Y);
						goDrawing.CurveTo(pointF6.X + num26 - num25 * 0.5f, pointF6.Y - num25, pointF6.X, pointF6.Y - num26, pointF6.X, pointF6.Y - num26);
						num25 = num24 * 0.4f;
						num26 = 0.4f;
						pointF6 = new PointF(0.2f, 0.5f);
						goDrawing.StartAt(pointF6.X, pointF6.Y + num26);
						goDrawing.CurveTo(pointF6.X - num25, pointF6.Y + num26, pointF6.X - num26, pointF6.Y + num25, pointF6.X - num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X - num26, pointF6.Y - num25, pointF6.X - num25, pointF6.Y - num26, pointF6.X, pointF6.Y - num26);
						goDrawing.CurveTo(pointF6.X, pointF6.Y - num26, pointF6.X - num26 + num25 * 0.5f, pointF6.Y - num25, pointF6.X - num26 + num25 * 0.5f, pointF6.Y);
						goDrawing.CurveTo(pointF6.X - num26 + num25 * 0.5f, pointF6.Y + num25, pointF6.X, pointF6.Y + num26, pointF6.X, pointF6.Y + num26);
						num25 = num24 * 0.8f;
						num26 = 0.8f;
						pointF6 = new PointF(0f, 0.5f);
						goDrawing.StartAt(pointF6.X, pointF6.Y + num26);
						goDrawing.CurveTo(pointF6.X - num25, pointF6.Y + num26, pointF6.X - num26, pointF6.Y + num25, pointF6.X - num26, pointF6.Y);
						goDrawing.CurveTo(pointF6.X - num26, pointF6.Y - num25, pointF6.X - num25, pointF6.Y - num26, pointF6.X, pointF6.Y - num26);
						goDrawing.CurveTo(pointF6.X, pointF6.Y - num26, pointF6.X - num26 + num25 * 0.5f, pointF6.Y - num25, pointF6.X - num26 + num25 * 0.5f, pointF6.Y);
						goDrawing.CurveTo(pointF6.X - num26 + num25 * 0.5f, pointF6.Y + num25, pointF6.X, pointF6.Y + num26, pointF6.X, pointF6.Y + num26);
						break;
					}
					case GoFigure.Email:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0.5f, 0.6f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(0.5f, 0.6f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.45f, 0.54f);
						goDrawing.StartAt(1f, 1f);
						goDrawing.LineTo(0.55f, 0.54f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0f);
						goDrawing.CloseAllFigures();
						break;
					case GoFigure.Ethernet:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0.5f, 0.5f);
						goDrawing.LineTo(0.5f, 0.4f);
						goDrawing.StartAt(0.75f, 0.5f);
						goDrawing.LineTo(0.75f, 0.6f);
						goDrawing.StartAt(0.25f, 0.5f);
						goDrawing.LineTo(0.25f, 0.6f);
						goDrawing.StartAt(0.35f, 0f);
						goDrawing.LineTo(0.65f, 0f);
						goDrawing.LineTo(0.65f, 0.4f);
						goDrawing.LineTo(0.35f, 0.4f);
						goDrawing.LineTo(0.35f, 0f);
						goDrawing.CloseFigure(12);
						goDrawing.StartAt(0.1f, 1f);
						goDrawing.LineTo(0.4f, 1f);
						goDrawing.LineTo(0.4f, 0.6f);
						goDrawing.LineTo(0.1f, 0.6f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.CloseFigure(17);
						goDrawing.StartAt(0.6f, 1f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0.9f, 0.6f);
						goDrawing.LineTo(0.6f, 0.6f);
						goDrawing.LineTo(0.6f, 1f);
						goDrawing.CloseFigure(22);
						break;
					case GoFigure.Power:
					{
						double num19 = Math.PI / 4.0;
						float num20 = (float)(4.0 * (1.0 - Math.Cos(num19)) / (3.0 * Math.Sin(num19)));
						float num21 = num20 * 0.4f;
						float num22 = 0.4f;
						PointF pointF5 = new PointF(0.5f, 0.5f);
						goDrawing.BreakUpBezier(new PointF(pointF5.X, pointF5.Y - num22), new PointF(pointF5.X + num21, pointF5.Y - num22), new PointF(pointF5.X + num22, pointF5.Y - num21), new PointF(pointF5.X + num22, pointF5.Y), 0.5f, out PointF curve1cp2, out curve1cp2, out PointF midpoint2, out PointF curve2cp3, out PointF curve2cp4);
						PointF p2 = new PointF(midpoint2.X, midpoint2.Y);
						goDrawing.StartAt(midpoint2);
						goDrawing.CurveTo(curve2cp3, curve2cp4, new PointF(pointF5.X + num22, pointF5.Y));
						goDrawing.CurveTo(pointF5.X + num22, pointF5.Y + num21, pointF5.X + num21, pointF5.Y + num22, pointF5.X, pointF5.Y + num22);
						goDrawing.CurveTo(pointF5.X - num21, pointF5.Y + num22, pointF5.X - num22, pointF5.Y + num21, pointF5.X - num22, pointF5.Y);
						goDrawing.BreakUpBezier(new PointF(pointF5.X - num22, pointF5.Y), new PointF(pointF5.X - num22, pointF5.Y - num21), new PointF(pointF5.X - num21, pointF5.Y - num22), new PointF(pointF5.X, pointF5.Y - num22), 0.5f, out curve2cp3, out curve2cp4, out midpoint2, out curve1cp2, out curve1cp2);
						goDrawing.CurveTo(curve2cp3, curve2cp4, midpoint2);
						num21 = num20 * 0.3f;
						num22 = 0.3f;
						goDrawing.BreakUpBezier(new PointF(pointF5.X - num22, pointF5.Y), new PointF(pointF5.X - num22, pointF5.Y - num21), new PointF(pointF5.X - num21, pointF5.Y - num22), new PointF(pointF5.X, pointF5.Y - num22), 0.5f, out curve2cp3, out curve2cp4, out midpoint2, out curve1cp2, out curve1cp2);
						goDrawing.LineTo(midpoint2);
						goDrawing.CurveTo(curve2cp4, curve2cp3, new PointF(pointF5.X - num22, pointF5.Y));
						goDrawing.CurveTo(pointF5.X - num22, pointF5.Y + num21, pointF5.X - num21, pointF5.Y + num22, pointF5.X, pointF5.Y + num22);
						goDrawing.CurveTo(pointF5.X + num21, pointF5.Y + num22, pointF5.X + num22, pointF5.Y + num21, pointF5.X + num22, pointF5.Y);
						goDrawing.BreakUpBezier(new PointF(pointF5.X, pointF5.Y - num22), new PointF(pointF5.X + num21, pointF5.Y - num22), new PointF(pointF5.X + num22, pointF5.Y - num21), new PointF(pointF5.X + num22, pointF5.Y), 0.5f, out curve1cp2, out curve1cp2, out midpoint2, out curve2cp3, out curve2cp4);
						goDrawing.CurveTo(curve2cp4, curve2cp3, midpoint2);
						goDrawing.LineTo(p2);
						goDrawing.CloseFigure(26);
						goDrawing.StartAt(0.45f, 0f);
						goDrawing.LineTo(0.45f, 0.5f);
						goDrawing.LineTo(0.55f, 0.5f);
						goDrawing.LineTo(0.55f, 0f);
						goDrawing.LineTo(0.45f, 0f);
						goDrawing.CloseFigure(31);
						break;
					}
					case GoFigure.Fallout:
					{
						double num14 = Math.PI / 4.0;
						float num15 = (float)(4.0 * (1.0 - Math.Cos(num14)) / (3.0 * Math.Sin(num14))) * 0.5f;
						float num16 = 0.5f;
						PointF pointF4 = new PointF(0.5f, 0.5f);
						goDrawing.StartAt(pointF4.X - num16, pointF4.Y);
						goDrawing.CurveTo(pointF4.X - num16, pointF4.Y - num15, pointF4.X - num15, pointF4.Y - num16, pointF4.X, pointF4.Y - num16);
						goDrawing.CurveTo(pointF4.X + num15, pointF4.Y - num16, pointF4.X + num16, pointF4.Y - num15, pointF4.X + num16, pointF4.Y);
						goDrawing.CurveTo(pointF4.X + num16, pointF4.Y + num15, pointF4.X + num15, pointF4.Y + num16, pointF4.X, pointF4.Y + num16);
						goDrawing.CurveTo(pointF4.X - num15, pointF4.Y + num16, pointF4.X - num16, pointF4.Y + num15, pointF4.X - num16, pointF4.Y);
						float num17 = 0f;
						float num18 = 0f;
						goDrawing.StartAt(0.3f + num17, 0.8f + num18);
						goDrawing.LineTo(0.5f + num17, 0.5f + num18);
						goDrawing.LineTo(0.1f + num17, 0.5f + num18);
						goDrawing.LineTo(0.3f + num17, 0.8f + num18);
						num17 = 0.4f;
						num18 = 0f;
						goDrawing.StartAt(0.3f + num17, 0.8f + num18);
						goDrawing.LineTo(0.5f + num17, 0.5f + num18);
						goDrawing.LineTo(0.1f + num17, 0.5f + num18);
						goDrawing.LineTo(0.3f + num17, 0.8f + num18);
						num17 = 0.2f;
						num18 = -0.3f;
						goDrawing.StartAt(0.3f + num17, 0.8f + num18);
						goDrawing.LineTo(0.5f + num17, 0.5f + num18);
						goDrawing.LineTo(0.1f + num17, 0.5f + num18);
						goDrawing.LineTo(0.3f + num17, 0.8f + num18);
						break;
					}
					case GoFigure.IrritationHazard:
						goDrawing.StartAt(0.2f, 0f);
						goDrawing.LineTo(0.5f, 0.3f);
						goDrawing.LineTo(0.8f, 0f);
						goDrawing.LineTo(1f, 0.2f);
						goDrawing.LineTo(0.7f, 0.5f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.LineTo(0.8f, 1f);
						goDrawing.LineTo(0.5f, 0.7f);
						goDrawing.LineTo(0.2f, 1f);
						goDrawing.LineTo(0f, 0.8f);
						goDrawing.LineTo(0.3f, 0.5f);
						goDrawing.LineTo(0f, 0.2f);
						goDrawing.LineTo(0.2f, 0f);
						break;
					case GoFigure.ElectricalHazard:
						goDrawing.StartAt(0.37f, 0f);
						goDrawing.LineTo(0.5f, 0.11f);
						goDrawing.LineTo(0.77f, 0.04f);
						goDrawing.LineTo(0.33f, 0.49f);
						goDrawing.LineTo(1f, 0.37f);
						goDrawing.LineTo(0.63f, 0.86f);
						goDrawing.LineTo(0.77f, 0.91f);
						goDrawing.LineTo(0.34f, 1f);
						goDrawing.LineTo(0.34f, 0.78f);
						goDrawing.LineTo(0.44f, 0.8f);
						goDrawing.LineTo(0.65f, 0.56f);
						goDrawing.LineTo(0f, 0.68f);
						break;
					case GoFigure.FireHazard:
						goDrawing.StartAt(0.1f, 1f);
						goDrawing.CurveTo(-0.26f, 0.63f, 0.45f, 0.44f, 0.29f, 0f);
						goDrawing.CurveTo(0.48f, 0.17f, 0.54f, 0.35f, 0.51f, 0.42f);
						goDrawing.CurveTo(0.59f, 0.29f, 0.58f, 0.28f, 0.59f, 0.18f);
						goDrawing.CurveTo(0.8f, 0.34f, 0.88f, 0.43f, 0.75f, 0.6f);
						goDrawing.CurveTo(0.87f, 0.48f, 0.88f, 0.43f, 0.88f, 0.31f);
						goDrawing.CurveTo(1.18f, 0.76f, 0.82f, 0.8f, 0.9f, 1f);
						goDrawing.LineTo(0.1f, 1f);
						break;
					case GoFigure.BpmnActivityLoop:
					{
						double num11 = Math.PI / 4.0;
						float num12 = (float)(4.0 * (1.0 - Math.Cos(num11)) / (3.0 * Math.Sin(num11))) * 0.4f;
						float num13 = 0.4f;
						PointF pointF3 = new PointF(0.5f, 0.5f);
						goDrawing.BreakUpBezier(new PointF(pointF3.X, pointF3.Y - num13), new PointF(pointF3.X + num12, pointF3.Y - num13), new PointF(pointF3.X + num13, pointF3.Y - num12), new PointF(pointF3.X + num13, pointF3.Y), 0.2f, out PointF curve1cp, out curve1cp, out PointF midpoint, out PointF curve2cp, out PointF curve2cp2);
						new PointF(midpoint.X, midpoint.Y);
						PointF p = new PointF(midpoint.X + 0.25f, midpoint.Y - 0f);
						goDrawing.StartAt(p);
						goDrawing.LineTo(midpoint);
						p = new PointF(midpoint.X + 0f, midpoint.Y + 0.25f);
						goDrawing.LineTo(p);
						goDrawing.StartAt(midpoint);
						goDrawing.CurveTo(curve2cp, curve2cp2, new PointF(pointF3.X + num13, pointF3.Y));
						goDrawing.CurveTo(pointF3.X + num13, pointF3.Y + num12, pointF3.X + num12, pointF3.Y + num13, pointF3.X, pointF3.Y + num13);
						goDrawing.CurveTo(pointF3.X - num12, pointF3.Y + num13, pointF3.X - num13, pointF3.Y + num12, pointF3.X - num13, pointF3.Y);
						goDrawing.BreakUpBezier(new PointF(pointF3.X - num13, pointF3.Y), new PointF(pointF3.X - num13, pointF3.Y - num12), new PointF(pointF3.X - num12, pointF3.Y - num13), new PointF(pointF3.X, pointF3.Y - num13), 0.5f, out curve2cp, out curve2cp2, out midpoint, out curve1cp, out curve1cp);
						goDrawing.CurveTo(curve2cp, curve2cp2, midpoint);
						goDrawing.FlipVertical();
						goDrawing.FlipHorizontal();
						break;
					}
					case GoFigure.BpmnActivityParallel:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.StartAt(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.StartAt(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.BpmnActivitySequential:
						goDrawing.StartAt(0f, 0f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(1f, 0.5f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(1f, 1f);
						break;
					case GoFigure.BpmnActivityAdHoc:
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.CurveTo(new PointF(0.2f, 0.35f), new PointF(0.3f, 0.35f), new PointF(0.5f, 0.5f));
						goDrawing.CurveTo(new PointF(0.7f, 0.65f), new PointF(0.8f, 0.65f), new PointF(1f, 0.5f));
						goDrawing.StartAt(1f, 1f);
						break;
					case GoFigure.BpmnActivityCompensation:
						goDrawing.StartAt(0f, 0.5f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(0.5f, 1f);
						goDrawing.LineTo(0f, 0.5f);
						goDrawing.StartAt(0.5f, 0.5f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0.5f, 0.5f);
						break;
					case GoFigure.BpmnTaskMessage:
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(0f, 0.2f);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.StartAt(1f, 0.2f);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.StartAt(0f, 0.2f);
						goDrawing.LineTo(1f, 0.2f);
						goDrawing.LineTo(1f, 0.8f);
						goDrawing.LineTo(0f, 0.8f);
						goDrawing.LineTo(0f, 0.8f);
						goDrawing.StartAt(1f, 1f);
						goDrawing.CloseAllFigures();
						break;
					case GoFigure.BpmnTaskScript:
						goDrawing.StartAt(0.7f, 1f);
						goDrawing.LineTo(0.3f, 1f);
						goDrawing.CurveTo(0.6f, 0.5f, 0f, 0.5f, 0.3f, 0f);
						goDrawing.LineTo(0.7f, 0f);
						goDrawing.CurveTo(0.4f, 0.5f, 1f, 0.5f, 0.7f, 1f);
						goDrawing.StartAt(0.45f, 0.73f);
						goDrawing.LineTo(0.7f, 0.73f);
						goDrawing.StartAt(0.38f, 0.5f);
						goDrawing.LineTo(0.63f, 0.5f);
						goDrawing.StartAt(0.31f, 0.27f);
						goDrawing.LineTo(0.56f, 0.27f);
						break;
					case GoFigure.BpmnEventConditional:
						goDrawing.StartAt(0.2f, 0.2f);
						goDrawing.LineTo(0.8f, 0.2f);
						goDrawing.StartAt(0.2f, 0.4f);
						goDrawing.LineTo(0.8f, 0.4f);
						goDrawing.StartAt(0.2f, 0.6f);
						goDrawing.LineTo(0.8f, 0.6f);
						goDrawing.StartAt(0.2f, 0.8f);
						goDrawing.LineTo(0.8f, 0.8f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(0.1f, 0f);
						goDrawing.LineTo(0.9f, 0f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0.1f, 1f);
						goDrawing.LineTo(0.1f, 0f);
						goDrawing.StartAt(1f, 1f);
						break;
					case GoFigure.BpmnEventError:
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0.33f, 0f);
						goDrawing.LineTo(0.66f, 0.5f);
						goDrawing.LineTo(1f, 0f);
						goDrawing.LineTo(0.66f, 1f);
						goDrawing.LineTo(0.33f, 0.5f);
						goDrawing.CloseFigure();
						break;
					case GoFigure.BpmnEventEscalation:
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(1f, 1f);
						goDrawing.StartAt(0.1f, 1f);
						goDrawing.LineTo(0.5f, 0f);
						goDrawing.LineTo(0.9f, 1f);
						goDrawing.LineTo(0.5f, 0.5f);
						goDrawing.LineTo(0.1f, 1f);
						break;
					case GoFigure.BpmnEventTimer:
					{
						double num8 = Math.PI / 4.0;
						float num9 = (float)(4.0 * (1.0 - Math.Cos(num8)) / (3.0 * Math.Sin(num8))) * 0.5f;
						float num10 = 0.5f;
						goDrawing.StartAt(1f, num10);
						goDrawing.CurveTo(1f, num10 + num9, num10 + num9, 1f, num10, 1f);
						goDrawing.CurveTo(num10 - num9, 1f, 0f, num10 + num9, 0f, num10);
						goDrawing.CurveTo(0f, num10 - num9, num10 - num9, 0f, num10, 0f);
						goDrawing.CurveTo(num10 + num9, 0f, 1f, num10 - num9, 1f, num10);
						goDrawing.CloseFigure();
						goDrawing.StartAt(num10, 0f);
						goDrawing.LineTo(num10, 0.15f);
						goDrawing.StartAt(num10, 1f);
						goDrawing.LineTo(num10, 0.85f);
						goDrawing.StartAt(0f, num10);
						goDrawing.LineTo(0.15f, num10);
						goDrawing.StartAt(1f, num10);
						goDrawing.LineTo(0.85f, num10);
						goDrawing.StartAt(num10, num10);
						goDrawing.LineTo(0.58f, 0.1f);
						goDrawing.StartAt(num10, num10);
						goDrawing.LineTo(0.78f, 0.54f);
						break;
					}
					case GoFigure.BpmnTaskPersonShirt:
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(0f, 1f);
						goDrawing.LineTo(0f, 0.68f);
						goDrawing.CurveTo(new PointF(0.02f, 0.54f), new PointF(0.12f, 0.46f), new PointF(0.335f, 0.445f));
						goDrawing.LineTo(0.335f, 0.595f);
						goDrawing.LineTo(0.664999962f, 0.595f);
						goDrawing.LineTo(0.664999962f, 0.445f);
						goDrawing.CurveTo(new PointF(0.88f, 0.46f), new PointF(0.98f, 0.54f), new PointF(1f, 0.68f));
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.StartAt(0.2f, 1f);
						goDrawing.LineTo(0.2f, 0.8f);
						goDrawing.StartAt(0.8f, 1f);
						goDrawing.LineTo(0.8f, 0.8f);
						break;
					case GoFigure.BpmnTaskPersonHead:
					{
						goDrawing.StartAt(1f, 1f);
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(0.635f, 0.404999971f);
						goDrawing.LineTo(0.664999962f, 0.445f);
						goDrawing.LineTo(0.664999962f, 0.595f);
						goDrawing.LineTo(0.335f, 0.595f);
						goDrawing.LineTo(0.335f, 0.445f);
						goDrawing.LineTo(0.365f, 0.404999971f);
						float num5 = 0.215f;
						PointF pointF2 = new PointF(0.5f, num5);
						double num6 = Math.PI / 4.0;
						float num7 = (float)(4.0 * (1.0 - Math.Cos(num6)) / (3.0 * Math.Sin(num6)));
						SizeF sizeF3 = new SizeF(num5, num5);
						SizeF sizeF4 = new SizeF(num7 * sizeF3.Width, num7 * sizeF3.Height);
						goDrawing.CurveTo(pointF2.X - (sizeF4.Width + sizeF3.Width) / 2f, pointF2.Y + (sizeF3.Height + sizeF4.Height) / 2f, pointF2.X - sizeF3.Width, pointF2.Y + sizeF4.Height, pointF2.X - sizeF3.Width, pointF2.Y);
						goDrawing.CurveTo(pointF2.X - sizeF3.Width, pointF2.Y - sizeF4.Height, pointF2.X - sizeF4.Width, pointF2.Y - sizeF3.Height, pointF2.X, pointF2.Y - sizeF3.Height);
						goDrawing.CurveTo(pointF2.X + sizeF4.Width, pointF2.Y - sizeF3.Height, pointF2.X + sizeF3.Width, pointF2.Y - sizeF4.Height, pointF2.X + sizeF3.Width, pointF2.Y);
						goDrawing.CurveTo(pointF2.X + sizeF3.Width, pointF2.Y + sizeF4.Height, pointF2.X + (sizeF4.Width + sizeF3.Width) / 2f, pointF2.Y + (sizeF3.Height + sizeF4.Height) / 2f, 0.635f, 0.404999971f);
						break;
					}
					case GoFigure.BpmnTaskUser:
					{
						goDrawing.StartAt(0f, 0f);
						goDrawing.StartAt(new PointF(0.335f, 0.445f));
						goDrawing.LineTo(0.335f, 0.595f);
						goDrawing.LineTo(0.664999962f, 0.595f);
						goDrawing.LineTo(0.664999962f, 0.445f);
						goDrawing.CurveTo(new PointF(0.88f, 0.46f), new PointF(0.98f, 0.54f), new PointF(1f, 0.68f));
						goDrawing.LineTo(1f, 1f);
						goDrawing.LineTo(0f, 1f);
						goDrawing.LineTo(0f, 0.68f);
						goDrawing.CurveTo(new PointF(0.02f, 0.54f), new PointF(0.12f, 0.46f), new PointF(0.335f, 0.445f));
						goDrawing.LineTo(0.365f, 0.404999971f);
						float num2 = 0.215f;
						PointF pointF = new PointF(0.5f, num2);
						double num3 = Math.PI / 4.0;
						float num4 = (float)(4.0 * (1.0 - Math.Cos(num3)) / (3.0 * Math.Sin(num3)));
						SizeF sizeF = new SizeF(num2, num2);
						SizeF sizeF2 = new SizeF(num4 * sizeF.Width, num4 * sizeF.Height);
						goDrawing.CurveTo(pointF.X - (sizeF2.Width + sizeF.Width) / 2f, pointF.Y + (sizeF.Height + sizeF2.Height) / 2f, pointF.X - sizeF.Width, pointF.Y + sizeF2.Height, pointF.X - sizeF.Width, pointF.Y);
						goDrawing.CurveTo(pointF.X - sizeF.Width, pointF.Y - sizeF2.Height, pointF.X - sizeF2.Width, pointF.Y - sizeF.Height, pointF.X, pointF.Y - sizeF.Height);
						goDrawing.CurveTo(pointF.X + sizeF2.Width, pointF.Y - sizeF.Height, pointF.X + sizeF.Width, pointF.Y - sizeF2.Height, pointF.X + sizeF.Width, pointF.Y);
						goDrawing.CurveTo(pointF.X + sizeF.Width, pointF.Y + sizeF2.Height, pointF.X + (sizeF2.Width + sizeF.Width) / 2f, pointF.Y + (sizeF.Height + sizeF2.Height) / 2f, 0.635f, 0.404999971f);
						goDrawing.LineTo(0.635f, 0.404999971f);
						goDrawing.LineTo(0.664999962f, 0.445f);
						goDrawing.LineTo(0.664999962f, 0.595f);
						goDrawing.LineTo(0.335f, 0.595f);
						goDrawing.StartAt(0.2f, 1f);
						goDrawing.LineTo(0.2f, 0.8f);
						goDrawing.StartAt(0.8f, 1f);
						goDrawing.LineTo(0.8f, 0.8f);
						break;
					}
					}
					myActions[(int)item2] = goDrawing.CopyActionsArray();
					PointF[] array = goDrawing.CopyPointsArray();
					for (int i = 0; i < array.Length; i = checked(i + 1))
					{
						array[i] = new PointF(array[i].X * 10f, array[i].Y * 10f);
					}
					myPoints[(int)item2] = array;
				}
			}
		}

		private static void SetLines(GoDrawing s, PointF[] points)
		{
			s.StartAt(points[0]);
			for (int i = 1; i < points.Length; i = checked(i + 1))
			{
				s.LineTo(points[i]);
			}
		}

		private static void SetCurves(GoDrawing s, PointF[] points)
		{
			s.StartAt(points[0]);
			checked
			{
				for (int i = 1; i < points.Length; i += 3)
				{
					s.CurveTo(points[i], points[i + 1], points[i + 2]);
				}
			}
		}

		private static PointF[] CreatePolygon(int sides)
		{
			checked
			{
				PointF[] array = new PointF[sides + 1];
				float num = 0.5f;
				float num2 = 0.5f;
				double num3 = 4.71238898038469;
				double num4 = 0.0;
				for (int i = 0; i < sides; i++)
				{
					num4 = Math.PI * 2.0 / (double)sides * (double)i + num3;
					array[i] = new PointF((float)((double)num2 + (double)num * Math.Cos(num4)), (float)((double)num2 + (double)num * Math.Sin(num4)));
				}
				array[array.Length - 1] = array[0];
				return array;
			}
		}

		private static PointF[] CreateStar(int points)
		{
			PointF[] array = CreatePolygon(points);
			PointF[] array2 = new PointF[checked(points * 2 + 1)];
			int num = array.Length / 2;
			int num2 = checked(array.Length - 1);
			int num3 = (points % 2 != 0) ? 1 : 2;
			checked
			{
				for (int i = 0; i < num2; i++)
				{
					array2[i * 2] = array[i];
					array2[i * 2 + 1] = unchecked(GetIntersection(array[i], array[checked(num + i - 1) % num2], array[checked(i + 1)], array[checked(num + i + num3) % num2]));
				}
				array2[array2.Length - 1] = array2[0];
				return array2;
			}
		}

		private static PointF[] CreateBurst(int points)
		{
			PointF[] array = CreateStar(points);
			checked
			{
				PointF[] array2 = new PointF[points * 3 + 1];
				array2[0] = array[0];
				int num = 1;
				int num2 = 1;
				while (num < array.Length)
				{
					array2[num2] = array[num];
					array2[num2 + 1] = array[num];
					array2[num2 + 2] = array[num + 1];
					num += 2;
					num2 += 3;
				}
				return array2;
			}
		}

		private static PointF GetIntersection(PointF l1p1, PointF l1p2, PointF l2p1, PointF l2p2)
		{
			float num = l1p1.X - l1p2.X;
			float num2 = l2p1.X - l2p2.X;
			float num5;
			float y;
			if (num == 0f || num2 == 0f)
			{
				if (num == 0f)
				{
					float num3 = (l2p1.Y - l2p2.Y) / num2;
					float num4 = l2p1.Y - num3 * l2p1.X;
					num5 = l1p1.X;
					y = num3 * num5 + num4;
				}
				else
				{
					float num6 = (l1p1.Y - l1p2.Y) / num;
					float num7 = l1p1.Y - num6 * l1p1.X;
					num5 = l2p1.X;
					y = num6 * num5 + num7;
				}
			}
			else
			{
				float num8 = (l1p1.Y - l1p2.Y) / num;
				float num9 = (l2p1.Y - l2p2.Y) / num2;
				float num10 = l1p1.Y - num8 * l1p1.X;
				num5 = (l2p1.Y - num9 * l2p1.X - num10) / (num8 - num9);
				y = num8 * num5 + num10;
			}
			return new PointF(num5, y);
		}
	}
}
