using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace Northwoods.Go
{
	internal sealed class GoPointFConverter : TypeConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new PointF((float)propertyValues["X"], (float)propertyValues["Y"]);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || sourceType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] array = ((string)value).Split(',');
				return new PointF(float.Parse(array[0], NumberFormatInfo.InvariantInfo), float.Parse(array[1], NumberFormatInfo.InvariantInfo));
			}
			if (value is InstanceDescriptor)
			{
				InstanceDescriptor instanceDescriptor = (InstanceDescriptor)value;
				if (instanceDescriptor.Arguments.Count == 2)
				{
					object[] array2 = new object[2];
					instanceDescriptor.Arguments.CopyTo(array2, 0);
					return new PointF((float)array2[0], (float)array2[1]);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is PointF)
			{
				PointF pointF = (PointF)value;
				if (destinationType == typeof(string))
				{
					return pointF.X.ToString(NumberFormatInfo.InvariantInfo) + ", " + pointF.Y.ToString(NumberFormatInfo.InvariantInfo);
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(PointF).GetConstructor(new Type[2]
					{
						typeof(float),
						typeof(float)
					});
					if (constructor != null)
					{
						return new InstanceDescriptor(constructor, new object[2]
						{
							pointF.X,
							pointF.Y
						}, isComplete: true);
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
