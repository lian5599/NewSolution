using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public static class UiHelper
    {
        private static readonly string Split = "  :  ";
        public static void ShowInfo(string info,string caption = "Info")
        {
            KryptonMessageBox.Show(info, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string error)
        {
            KryptonMessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowException(Exception ex)
        {
            KryptonMessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool AskUser(string toComfirm)
        {
            var result = KryptonMessageBox.Show(toComfirm, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private static ListViewItem createMessageListViewItem(string value, string type)
        {
            return new ListViewItem(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + Split + type + Split + value);
        }
    }

}
