using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKS.Manager
{
    public static class GlobalVariable
    {
		private static List<GlobalVariableCfg> GlobalVariables = new List<GlobalVariableCfg>();
		public static void Set(string key, object value, string description = null)
		{
			var find = GlobalVariables.Find(item => item.Id.ToLower() == key.ToLower());
			if (find == null)
			{
				GlobalVariables.Add(new GlobalVariableCfg(key, value, description));
			}
			else
            {
				find.Value = value;
				find.Description = description;
            }
		}

		public static void Delete(string key)
		{
			GlobalVariables.RemoveAll(item => item.Id.ToLower() == key.ToLower());
		}

		public static void Motify(string key, string value, string description = null)
		{
			var selector = GlobalVariables.Find(item => item.Id.ToLower() == key.ToLower());
			selector.Value = value;
			selector.Description = description;
		}


		public static object Get(string key)
		{
			var selector = GlobalVariables.Find(item => item.Id.ToLower() == key.ToLower());
			if (selector != null)
			{
				return selector.Value;
			}
			return null;
		}

		public static void Save()
        {
			Configuration.SaveConfig(nameof(GlobalVariableCfg), GlobalVariables);
        }
	}

	public class GlobalVariableCfg
    {
        public string Id { get; set; }
		public string Description { get; set; }
		public object Value { get; set; }
		public GlobalVariableCfg(string id,object value, string description = null)
        {
			Id = id;
			Description = description;
			Value = value;
        }
	}
}
