using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!Security.IsRegisterd(out TimeSpan t))
            {
                FormRegister register = new FormRegister();
                if (register.ShowDialog() != DialogResult.OK) return;
            }
            Application.Run(new MainForm());
        }
    }
}
