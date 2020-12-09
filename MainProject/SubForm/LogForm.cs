using Helper;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
namespace TKS.SubForm
{
    public partial class LogForm : DockContent
    {

        #region singleton
        private static volatile LogForm _instance = null;
        private static readonly object _locker = new object();
        public static LogForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogForm();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion
        private LogForm()
        {
            InitializeComponent();
            Helper.Log.LogEvent += Log;
            listView_Log.Sorting = SortOrder.None;
            this.HideOnClose = true;//only hide instead close
        }

        private readonly int MAX_LOG_COUNT = 1000;
        private readonly string Split = " : ";
        private void Log(LogLevel level, string msg)
        {
            ListViewItem listViewItem;
            listViewItem = createMessageListViewItem(msg);
            switch (level)
            {
                case LogLevel.Info:
                    listViewItem.ForeColor = Color.Black;
                    break;
                case LogLevel.Warming:
                    listViewItem.ForeColor = Color.Orange;
                    break;
                case LogLevel.Error:
                    listViewItem.ForeColor = Color.Red;
                    break;
                default: return;
            }
            LogItemEx(listViewItem);
        }
        private void LogItemEx(ListViewItem item)
        {
            try
            {
                Invoke((Action)delegate
                {
                    //listView_Log.BeginUpdate();
                    lock (this)
                    {
                        if (listView_Log.Items.Count > MAX_LOG_COUNT)
                        {
                            listView_Log.Items.RemoveAt(MAX_LOG_COUNT);
                        }
                        listView_Log.Items.Insert(0, item);

                    }
                    //listView_Log.EndUpdate();
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

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _instance = null;

        }
    }
}
