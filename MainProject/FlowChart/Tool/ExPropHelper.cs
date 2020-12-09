using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace TKS.FlowChart.Tool
{
    public class XProp
    {
        private object mvalue = null;
        //public string Id { get; set; } = "";
        [JsonIgnore]
        public string Category { get; set; } = "";
        [JsonIgnore]
        public bool ReadOnly { get; set; } = false;
        public string Name { get; set; } = "";
        public object Value 
        {
            get
            {
                return mvalue;
            }
            set
            {
                if (mvalue != value)
                {
                    mvalue = value;
                    ValueChanged?.Invoke(value);
                }
            }
            }
        [JsonIgnore]
        public string Description { get; set; } = "";
        [JsonIgnore]
        public System.Type ProType { get; set; } = null;
        [JsonIgnore]
        public bool Browsable { get; set; } = true;
        [JsonIgnore]
        public virtual TypeConverter Converter { get; set; } = null;
        public object DeepCopyValue()
        {
            var Settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };//多态序列化
            string str = JsonConvert.SerializeObject(this, Formatting.Indented, Settings);
            var obj2 = JsonConvert.DeserializeObject<XProp>(str, Settings);
            return obj2.Value;
        }
        [JsonIgnore]
        public Action<object> ValueChanged { get; set; } = null;
        [JsonIgnore]
        public Type EditorType { get; set; } = null;
    }

    public class XProps : List<XProp>, ICustomTypeDescriptor
    {
        #region ICustomTypeDescriptor 成员
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }
        public object GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            ArrayList props = new ArrayList();
            for (int i = 0; i < this.Count; i++)
            {  //判断属性是否显示
                if (this[i].Browsable == true)
                {
                    XPropDescriptor psd = new XPropDescriptor(this[i], attributes);
                    props.Add(psd);
                }
            }
            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor));
            return new PropertyDescriptorCollection(propArray);
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append("[" + i + "] " + this[i].ToString() + System.Environment.NewLine);
            }
            return sb.ToString();
        }
    }

    public class XPropDescriptor : PropertyDescriptor
    {
        XProp theProp;
        //添加后面...编辑按钮
        public override object GetEditor(Type editorBaseType)
        {
            if (theProp.EditorType!=null)
            {
                return Activator.CreateInstance(theProp.EditorType);
            }
            return base.GetEditor(editorBaseType);
        }

        public XPropDescriptor(XProp prop, Attribute[] attrs) : base(prop.Name, attrs)
        {
            theProp = prop;
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override string Category
        {
            get { return theProp.Category; }
        }
        public override string Description
        {
            get { return theProp.Description; }
        }
        public override TypeConverter Converter
        {
            get { return theProp.Converter; }
        }
        public override System.Type ComponentType
        {
            get { return this.GetType(); }
        }
        public override object GetValue(object component)
        {
            return theProp.Value;
        }
        public override bool IsReadOnly
        {
            get { return theProp.ReadOnly; }
        }
        public override System.Type PropertyType
        {
            get { return theProp.ProType; }
        }
        public override void ResetValue(object component)
        {
        }
        public override void SetValue(object component, object value)
        {
            theProp.Value = value;
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

    //重写下拉菜单中的项，使之与属性页的项关联
    public abstract class ComboBoxItemTypeConvert : TypeConverter
    {
        public Hashtable myhash = null;
        public ComboBoxItemTypeConvert()
        {
            myhash = new Hashtable();
            GetConvertHash();
        }
        public abstract void GetConvertHash();

        //是否支持选择列表的编辑
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        //重写combobox的选择列表
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] ids = new string[myhash.Values.Count];          
            for (int i = 0; i < myhash.Values.Count; i++)
            {
                ids[i] = i.ToString();
            }
            //int i = 0;
            //foreach (DictionaryEntry myDE in myhash)
            //{
            //    ids[i++] = (myDE.Key).ToString();//Convert.ToInt32((myDE.Key));
            //}
            return new StandardValuesCollection(ids);
        }
        //判断转换器是否可以工作
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }
        //重写转换器，将选项列表（即下拉菜单）中的值转换到该类型的值
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object obj)
        {
            if (obj is string)
            {
                foreach (DictionaryEntry myDE in myhash)
                {
                    if (myDE.Value.Equals((obj.ToString())))
                        return myDE.Key;
                }
            }
            return base.ConvertFrom(context, culture, obj);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);

        }

        //重写转换器将该类型的值转换到选择列表中
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object obj, Type destinationType)
        {

            if (destinationType == typeof(string))
            {
                foreach (DictionaryEntry myDE in myhash)
                {
                    if (myDE.Key.Equals(obj))
                        return myDE.Value.ToString();
                }
                return "";
            }
            return base.ConvertTo(context, culture, obj, destinationType);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }


    //重写下拉菜单，在这里实现定义下拉菜单内的项
    [Serializable]
    public class MyComboItemConvert : ComboBoxItemTypeConvert
    {
        private Hashtable hash;
        public override void GetConvertHash()
        {
            try
            {
                myhash = hash;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
        public MyComboItemConvert() { }
        public MyComboItemConvert(string str)
        {
            hash = new Hashtable();
            string[] stest = str.Split(',');
            for (int i = 0; i < stest.Length; i++)
            {
                hash.Add(i.ToString(), stest[i]);
            }
            GetConvertHash();

            value = "0";
        }

        public string value { get; set; }

        public MyComboItemConvert(string str, string s)
        {
            hash = new Hashtable();
            string[] stest = str.Split(',');
            for (int i = 0; i < stest.Length; i++)
            {
                hash.Add(i.ToString(), stest[i]);
            }
            GetConvertHash();
            value = s;
        }
    }


    #region 重写list
    public class XList<T> : List<T>
    {
        [BrowsableAttribute(false)]
        public new int Count { get; }
        [BrowsableAttribute(false)]
        public new int Capacity { get; set; }
        public override string ToString()
        {
            string str = "";
            foreach (var item in this)
            {
                str += item.ToString() + ",";
            }
            if (str.Length > 0)
            {
                str = str.Remove(str.Length - 1, 1);
            }
            return str;
        }
    }
    public class XListConverter<T> : ExpandableObjectConverter
    {
        /// <summary>
        /// 类型转换器
        /// </summary>

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string) == sourceType)
            {
                return true;
            }
            return false;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return new XList<T>();
                }
                else
                {
                    XList<T> list = new XList<T>();

                    string[] split = value.ToString().Split(',');
                    for (int i = 0; i < split.Length; i++)
                    {
                        list.Add((T)typeof(Convert).GetMethod("To" + typeof(T).Name, new Type[] { typeof(object) }).Invoke(null, new object[] { split[i] }));
                        //list[i] = (T)typeof(Convert).GetMethod("To" + typeof(T).Name, new Type[] { typeof(object) }).Invoke(null, new object[] { split[i] });
                    }
                    return list;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    if (destinationType == typeof(string))
        //    {
        //        return true;
        //    }
        //    return base.CanConvertTo(context, destinationType);
        //}

        //public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        //{
        //    if (destinationType == typeof(string))
        //    {
        //        var list = value as Array;               
        //        return string.Join(",", list);
        //    }
        //    return base.ConvertTo(context, culture, value, destinationType);
        //}
    }
    #endregion


    public class Editor1 : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            // 编辑属性值时，在右侧显示...更多按钮
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            var edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (edSvc != null)
            {
                value = new ModelRoi(2, "abc");
            }
            return base.EditValue(context, provider, value);
        }
    }

}
