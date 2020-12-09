using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace Northwoods.Go
{
	internal sealed class GoSizeFConverter : TypeConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new SizeF((float)propertyValues["Width"], (float)propertyValues["Height"]);
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
				string[] array = ((string)value).Split('x');
				return new SizeF(float.Parse(array[0], NumberFormatInfo.InvariantInfo), float.Parse(array[1], NumberFormatInfo.InvariantInfo));
			}
			if (value is InstanceDescriptor)
			{
				InstanceDescriptor instanceDescriptor = (InstanceDescriptor)value;
				if (instanceDescriptor.Arguments.Count == 2)
				{
					object[] array2 = new object[2];
					instanceDescriptor.Arguments.CopyTo(array2, 0);
					return new SizeF((float)array2[0], (float)array2[1]);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is SizeF)
			{
				SizeF sizeF = (SizeF)value;
				if (destinationType == typeof(string))
				{
					return sizeF.Width.ToString(NumberFormatInfo.InvariantInfo) + "x" + sizeF.Height.ToString(NumberFormatInfo.InvariantInfo);
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(SizeF).GetConstructor(new Type[2]
					{
						typeof(float),
						typeof(float)
					});
					if (constructor != null)
					{
						return new InstanceDescriptor(constructor, new object[2]
						{
							sizeF.Width,
							sizeF.Height
						}, isComplete: true);
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
