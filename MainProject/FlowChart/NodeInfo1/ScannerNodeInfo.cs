using Helper;
using TKS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;
using Communication.Scanner;
using System.Runtime.InteropServices;

namespace FlowCharter
{
    [Serializable]
    public class ScannerNodeInfo : GraphNodeInfoBase
    {
        public ScannerNodeInfo(GraphNode n) : base(n) { }

        [Category("Param")]
        [Description("ID")]
        public string ScannerId 
        { 
            get; 
            set; 
        } = "";

        protected override Result Action()
        {
            sn = "";
            ScanFinish = false;
            Result<string> re;
            try
            {               
                IScanner scanner = Hardware.Instance.scanner(ScannerId);
                bool manual = scanner.GetType().Name.ToLower().Contains("manual");
                if (!manual) re = scanner.Trigger();
                else
                {
                    scanner.SnRecieved -= Scanner_SnRecieved;
                    scanner.SnRecieved += Scanner_SnRecieved;
                    UiHelper.ShowInfo("等待手动扫码");
                    string msg = ScannerId + "扫码失败";
                    if (ScanFinish)
                    {
                        msg = ScannerId + "扫码成功:" + sn;
                    }
                    re = new Result<string>(sn,ScanFinish,msg);
                }
                if (re.IsSuccess)
                {                    
                    Output.OutputData(myNode.Document.Name, ScannerId, re.Content);
                    Log.Run(LogLevel.Info, re.Message);
                    return new Result<string>(sn, ScanFinish, ScannerId + "扫码成功:" + sn);
                }
                else Log.Run(LogLevel.Error, re.Message);
                return re;
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message,e);
                //UiHelper.ShowError(e.Message);
                return new Result(e.Message);
            }
        }
        bool ScanFinish = false;
        string sn = "";
        private void Scanner_SnRecieved(object sender, string sn)
        {
            ScanFinish = true;
            this.sn = sn;
            IntPtr dlg = FindWindow(null, "Info");
            if (dlg != IntPtr.Zero) 
            {
                PostMessage(dlg, WM_CLOSE, IntPtr.Zero, IntPtr.Zero); ;
            }
        }   

        public override Result RunAll()
        {
            return base.RunAll();
        }

        public const int WM_CLOSE = 0x10;
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool EndDialog(IntPtr hDlg, out IntPtr nResult);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }

}
