using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public static class IconImage
    {
		public static ImageList imageList = new ImageList();
		public static void loadIcon(string filePath)
		{
			//string filename = "icons\\" + name + ".png";
			var iconFileNames = Directory.GetFiles(filePath, "*.png");
			try
			{
                foreach (var item in iconFileNames)
                {
					Image image = Image.FromFile(item);
					int index = item.LastIndexOf("\\") + 1;
					string name = item.Substring(index).Replace(".png", "");
					imageList.Images.Add(name, image);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		public static Image Get(string name)
		{
			return imageList.Images[name];
		}
	}
}
