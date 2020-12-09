using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKS
{
    public partial class LogUI : UserControl
    {
		private readonly int MAX_LOG_COUNT = 1000;
		private readonly string Split = "  :  ";
		public LogUI()
        {
            InitializeComponent();
			ExEvent.LogEvent -= Log;
			ExEvent.LogEvent += Log;
		}
		private void Log(LogLevel level,string msg)
        {
			ListViewItem listViewItem;
			listViewItem = createMessageListViewItem(msg);
			switch (level)
            {
				case LogLevel.Info:
					break;
				case LogLevel.Warming:
					listViewItem.ForeColor = Color.Orange;
					break;
				case LogLevel.Error:
					listViewItem.ForeColor = Color.Red;
					break;
				default:return;
			}
			LogItemEx(listViewItem);
		}
		private void LogItemEx(ListViewItem item)
		{
			try
			{
				Invoke((Action)delegate
					{
						listView_Log.BeginUpdate();
						if (listView_Log.Items.Count > MAX_LOG_COUNT)
						{
							listView_Log.Items.RemoveAt(MAX_LOG_COUNT);
						}
						listView_Log.Items.Insert(0, item);
						listView_Log.EndUpdate();
					});
			}
			catch (Exception)
			{
			}
		}
		private ListViewItem createMessageListViewItem(string value)
		{
			return new ListViewItem(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + Split + value);
		}

        private void LogUI_Load(object sender, EventArgs e)
        {
			listView_Log.Columns.Add("消息", listView_Log.Size.Width - 5, HorizontalAlignment.Left);
			#region // 设置行高 20
			ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 20); //分别是宽和高
            listView_Log.SmallImageList = imgList; 
            #endregion
        }

        private void listView_Log_SizeChanged(object sender, EventArgs e)
        {
			if (listView_Log.Columns.Count > 0)
			{
				listView_Log.Columns[0].Width = listView_Log.Size.Width - 5;
			}
		}

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
			listView_Log.Items.Clear();
		}
    }
}
