using Communication;
using Communication.Scanner;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class ScanTool : ToolBase
    {
        public ScanTool() : base()
        {
            Hardware_ChangedEvent();
            Hardware.ScannerChangedEvent -= Hardware_ChangedEvent;
            Hardware.ScannerChangedEvent += Hardware_ChangedEvent;
        }

        private void Hardware_ChangedEvent()
        {
            string plcDefineStr = "";
            foreach (var item in Hardware.Instance.scanners)
            {
                plcDefineStr += item.Id + ",";
            }
            if (plcDefineStr.Length > 0) plcDefineStr = plcDefineStr.Remove(plcDefineStr.Length - 1, 1);
            Settings.Find(item => item.Name == "扫码枪Id").Converter = new MyComboItemConvert(plcDefineStr);
        }
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "扫码枪Id";
            xprop.Value = "0";
            xprop.ProType = typeof(MyComboItemConvert);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "输出";
            xprop.Value = "条码";
            xprop.ProType = typeof(string);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输入";
            xprop.Name = "输入";
            xprop.Value = new XList<string>();
            xprop.ProType = typeof(XList<string>);
            xprop.Converter = new XListConverter<string>();
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            ScanFinish = false;
            Result re;
            try
            {
                string scannerId = GetPropValue("扫码枪Id").ToString();
                object[] inputs = GetInputs();
                string msg = "";
                
                IScanner scanner = Hardware.Instance.scanner(scannerId);
                bool manual = scanner.GetType().Name.ToLower().Contains("manual");
                if (!manual) re = scanner.Trigger();
                else
                {
                    scanner.SnRecieved -= Scanner_SnRecieved;
                    scanner.SnRecieved += Scanner_SnRecieved;
                    UiHelper.ShowInfo("等待手动扫码", graphNode.Text);
                    msg = scannerId + "扫码失败";
                    if (ScanFinish)
                    {
                        msg = scannerId + "扫码成功";
                    }
                    re = new Result<string>(sn, ScanFinish, msg);
                }
                if (re.IsSuccess)
                {
                    string key = inputs == null ? string.Format(GetPropValue("输出").ToString()) : string.Format(GetPropValue("输出").ToString(), inputs);
                    Flow.Instance.AddOutput(key, re.GetContent());
                    msg = re.Message + "=>"+ key + ":" + re.GetContent();
                    Log.Run(LogLevel.Info, msg);
                    return re;
                }
                else
                {
                    //re.ErrorCode = (int)ErrorCode.fatalError;
                    Log.Run(LogLevel.Error, re.Message);
                }
                return re;
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message, e);
                return new Result(e.Message, (int)ErrorCode.fatalError);
            }
        }

        bool ScanFinish = false;
        string sn = "";
        private void Scanner_SnRecieved(object sender, string sn)
        {
            ScanFinish = true;
            this.sn = sn;
            IntPtr dlg = FindWindow(null, graphNode.Text);
            if (dlg != IntPtr.Zero)
            {
                PostMessage(dlg, WM_CLOSE, IntPtr.Zero, IntPtr.Zero); ;
            }
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
