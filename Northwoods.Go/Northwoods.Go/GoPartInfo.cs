using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Northwoods.Go
{
	/// <summary>
	/// This class holds information to be transmitted to the client as a JavaScript
	/// object with property values, describing some <see cref="T:Northwoods.Go.GoObject" />.
	/// </summary>
	/// <remarks>
	/// The standard set of properties to be generated include <see cref="P:Northwoods.Go.GoPartInfo.ID" />,
	/// <see cref="P:Northwoods.Go.GoPartInfo.Text" />, <see cref="P:Northwoods.Go.GoPartInfo.SingleClick" />, <see cref="P:Northwoods.Go.GoPartInfo.DoubleClick" />, 
	/// and <see cref="P:Northwoods.Go.GoPartInfo.ContextClick" />.
	/// You can add whatever properties your web application needs to have on
	/// the client by calling the <see cref="M:Northwoods.Go.GoPartInfo.SetProperty(System.String,System.Object)" /> method.
	/// </remarks>
	[Serializable]
	public class GoPartInfo
	{
		private static readonly char[] myQuotables = new char[4]
		{
			'\'',
			'"',
			'\n',
			'\r'
		};

		private List<KeyValuePair<string, object>> myProperties = new List<KeyValuePair<string, object>>();

		/// <summary>
		/// Gets or sets an identifier describing an object.
		/// </summary>
		/// <value>
		/// <para>
		/// By default this gets the <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />,
		/// if the object is an <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> and the <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />
		/// is not -1.
		/// Otherwise this value is null, and you will probably want to set this to whatever
		/// value you are using in your application.
		/// </para>
		/// <para>
		/// This is not the same as the "id" property for a DHTML element.
		/// </para>
		/// </value>
		/// <remarks>
		/// This just gets or sets the "ID" property.
		/// </remarks>
		public string ID
		{
			get
			{
				return GetProperty("ID") as string;
			}
			set
			{
				SetProperty("ID", value);
			}
		}

		/// <summary>
		/// Gets or sets a text string describing an object.
		/// </summary>
		/// <value>
		/// By default this gets the <see cref="T:Northwoods.Go.IGoLabeledNode" />.<see cref="P:Northwoods.Go.IGoLabeledPart.Text" />,
		/// if the object is an <see cref="T:Northwoods.Go.IGoLabeledNode" />.
		/// Otherwise this value is null, and you may find it convenient to set this to whatever
		/// descriptive text that would be useful to your application.
		/// This just gets or sets the "Text" property.
		/// </value>
		public string Text
		{
			get
			{
				return GetProperty("Text") as string;
			}
			set
			{
				SetProperty("Text", value);
			}
		}

		/// <summary>
		/// Gets or sets the JavaScript to be executed when the user clicks an object.
		/// </summary>
		/// <value>This defaults to null</value>
		/// <remarks>
		/// This just gets or sets the "SingleClick" property.
		/// </remarks>
		public string SingleClick
		{
			get
			{
				return GetProperty("SingleClick") as string;
			}
			set
			{
				SetProperty("SingleClick", value);
			}
		}

		/// <summary>
		/// Gets or sets the JavaScript to be executed when the user double-clicks an object.
		/// </summary>
		/// <value>This defaults to null</value>
		/// <remarks>
		/// This just gets or sets the "DoubleClick" property.
		/// </remarks>
		public string DoubleClick
		{
			get
			{
				return GetProperty("DoubleClick") as string;
			}
			set
			{
				SetProperty("DoubleClick", value);
			}
		}

		/// <summary>
		/// Gets or sets the JavaScript to be executed when the user context-clicks an object.
		/// </summary>
		/// <value>This defaults to null</value>
		/// <remarks>
		/// This code (if present) takes precedence over any context menus that are associated
		/// with this object, so this property is not frequently used.
		/// This just gets or sets the "ContextClick" property.
		/// </remarks>
		public string ContextClick
		{
			get
			{
				return GetProperty("ContextClick") as string;
			}
			set
			{
				SetProperty("ContextClick", value);
			}
		}

		/// <summary>
		/// This indexed property makes it syntactically easier to get and
		/// set properties.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoPartInfo.GetProperty(System.String)" />
		/// <seealso cref="M:Northwoods.Go.GoPartInfo.SetProperty(System.String,System.Object)" />
		public object this[string p]
		{
			get
			{
				return GetProperty(p);
			}
			set
			{
				SetProperty(p, value);
			}
		}

		/// <summary>
		/// Construct an empty GoPartInfo.
		/// </summary>
		public GoPartInfo(IGoPartInfoRenderer renderer)
		{
		}

		/// <summary>
		/// Compare all of the properties.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			GoPartInfo goPartInfo = obj as GoPartInfo;
			if (goPartInfo == null)
			{
				return false;
			}
			if (myProperties.Count != goPartInfo.myProperties.Count)
			{
				return false;
			}
			for (int i = 0; i < myProperties.Count; i = checked(i + 1))
			{
				if (myProperties[i].Key != goPartInfo.myProperties[i].Key)
				{
					return false;
				}
				if (myProperties[i].Value != goPartInfo.myProperties[i].Value)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Consider all of the properties.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < myProperties.Count; i = checked(i + 1))
			{
				if (myProperties[i].Key != null)
				{
					num ^= myProperties[i].Key.GetHashCode();
				}
				if (myProperties[i].Value != null)
				{
					num ^= myProperties[i].Value.GetHashCode();
				}
			}
			return num;
		}

		/// <summary>
		/// Produce a JavaScript expression that constructs a JavaScript object
		/// holding all of the property/value pairs described by this GoPartInfo.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// For property values that are strings, they are just transmitted as
		/// string literals.
		/// For property values that are not strings, this calls <c>Object.ToString()</c>
		/// to produce an expression that JavaScript can evaluate to produce a
		/// number, string, array or object.
		/// </remarks>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < myProperties.Count; i = checked(i + 1))
			{
				AddProp(stringBuilder, myProperties[i].Key, myProperties[i].Value);
			}
			return "{" + stringBuilder.ToString() + "}";
		}

		private void AddProp(StringBuilder s, string p, object v)
		{
			if (p != null && p.Length != 0 && v != null)
			{
				if (s.Length > 0)
				{
					s.Append(",");
				}
				s.Append(DQuote(p) + ":");
				AddPropVal(s, v);
			}
		}

		private void AddPropVal(StringBuilder s, object v)
		{
			if (v is string)
			{
				s.Append(DQuote((string)v));
			}
			else if (v is bool)
			{
				bool flag = (bool)v;
				s.Append(flag ? "true" : "false");
			}
			else if (v is int)
			{
				s.Append(((int)v).ToString(CultureInfo.InvariantCulture));
			}
			else if (v is float)
			{
				s.Append(((float)v).ToString(CultureInfo.InvariantCulture));
			}
			else if (v is double)
			{
				s.Append(((double)v).ToString(CultureInfo.InvariantCulture));
			}
			else if (v is Array)
			{
				Array obj = (Array)v;
				s.Append("[");
				foreach (object item in obj)
				{
					AddPropVal(s, item);
				}
				s.Append("]");
			}
			else
			{
				s.Append(v.ToString());
			}
		}

		/// <summary>
		/// Given a string, produce the contents of a JavaScript string literal
		/// with single-quote and double-quote characters back-slashified,
		/// surrounded by double quote marks.
		/// </summary>
		/// <param name="s"></param>
		/// <returns>a string containing a JavaScript string literal surrounded by double quote marks</returns>
		public static string DQuote(string s)
		{
			return Quote(s, '"');
		}

		/// <summary>
		/// Given a string, produce the contents of a JavaScript string literal
		/// with single-quote and double-quote characters back-slashified,
		/// surrounded by single quote marks.
		/// </summary>
		/// <param name="s"></param>
		/// <returns>a string containing a JavaScript string literal surrounded by single quote marks</returns>
		public static string SQuote(string s)
		{
			return Quote(s, '\'');
		}

		/// <summary>
		/// Given a string, produce the contents of a JavaScript string literal
		/// with single-quote and double-quote characters back-slashified.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="quote"></param>
		/// <returns>a string literal surrounded by <paramref name="quote" /> characters</returns>
		public static string Quote(string s, char quote)
		{
			string text = s;
			if (text.IndexOfAny(myQuotables) >= 0)
			{
				text = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\n")
					.Replace("'", "\\'")
					.Replace("\"", "\\\"");
			}
			if (quote != 0)
			{
				return quote + text + quote;
			}
			return text;
		}

		/// <summary>
		/// This method provides a general way to get the value for any property.
		/// </summary>
		/// <param name="p">a String</param>
		/// <returns>null if the property name is not present</returns>
		public object GetProperty(string p)
		{
			for (int i = 0; i < myProperties.Count; i = checked(i + 1))
			{
				if (myProperties[i].Key == p)
				{
					return myProperties[i].Value;
				}
			}
			return null;
		}

		/// <summary>
		/// This method provides a general way to set the value for any property.
		/// </summary>
		/// <param name="p">a String</param>
		/// <param name="v">a value of null makes this property disappear from the generated JavaScript</param>
		/// <remarks>
		/// If the property name is not already present, it is added.
		/// </remarks>
		public void SetProperty(string p, object v)
		{
			for (int i = 0; i < myProperties.Count; i = checked(i + 1))
			{
				if (myProperties[i].Key == p)
				{
					myProperties[i] = new KeyValuePair<string, object>(p, v);
					return;
				}
			}
			KeyValuePair<string, object> item = new KeyValuePair<string, object>(p, v);
			myProperties.Add(item);
		}
	}
}
