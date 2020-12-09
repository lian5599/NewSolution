using Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MesAdapter
{
    public static class HuaBei
    {
        #region Original
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MESBackFunc(StringBuilder data);
        /// <summary>
        /// mes函数执行成功
        /// </summary>
        public const int MesBackOK = 0;
        /// <summary>
        /// MES的句柄
        /// </summary>
        public static int hMes = 0;
        private const string DLLPATH = "HQMES.dll";
        [DllImport(DLLPATH)]
        public static extern int MesInit(MESBackFunc func, ref int hMes, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        public static extern int MesStart(int hMes, string SN, string ActionName, string Tools, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        public static extern int MesStart2(int hMes, string SN, string SNType, string ActionName, string Tools, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        public static extern int MesEnd(int hMes, string SN, string ActionName, string Tools, string ErrorCode, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        public static extern int MesEnd2(int hMes, string SN, string SNType, string ActionName, string Tools, string ErrorCode, string AllData, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesSaveAndGetExtraInfo(int hMes, string G_TYPE, string G_POSITION, string G_KEY, string G_VALUE, string G_EXTINFO, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        public static extern int MesUnInit(int hMes);
        [DllImport(DLLPATH)]
        public static extern int MesStart3(int hMes, string SN, string SNType, string ActionName, string Tools, string Extinfo, StringBuilder sInfo, ref int InfoLen);
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int WriteLogFile(StringBuilder data)
        {
            //MessageBox.Show(data.ToString());
            return 0;
        }
        #endregion


        //初始化，软件启动时调用
        public static void Init() 
        {
            try
            {
                Settings.AddSetting("ActionName");
                Settings.AddSetting("Tools");
                Settings.AddSetting("SNType");
                Settings.AddSetting("allDataFormat");

                int len = 102400;
                StringBuilder strdata = new StringBuilder(len);
                int response = MesInit(null, ref hMes, strdata, ref len);
                if (response == MesBackOK)
                {
                    JObject jt = (JObject)JsonConvert.DeserializeObject(strdata.ToString());
                    string version = "";
                    version = jt.GetValue("H_MSG") == null ? "" : jt.GetValue("H_MSG").ToString();
                    Log.Run(LogLevel.Info, "初始化链接MES 成功 ！MEShelper版本：" + version);
                }
                else Log.Run(LogLevel.Error, "初始化链接MES 失败 ！返回值：" + response);
            }
            catch (Exception ex)
            {
                Log.Run(LogLevel.Error,ex.Message, ex);
            }
        }
        public static void Close()
        {
            try
            {
                int response = MesUnInit(hMes);
            }
            catch (Exception ex)
            {
                Log.Run(LogLevel.Error, ex.Message, ex);
            }
        }
        public static Result MesStart(string sn)
        {
            Result result = new Result();
            string ActionName = Settings.GetSetting("ActionName");
            string Tools = Settings.GetSetting("Tools");
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            int response = MesStart(hMes, sn, ActionName, Tools, strdata, ref len);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strdata.ToString());
            if (response == MesBackOK)
            {
                result.IsSuccess = true;
                result.Message = "";//jo.GetValue("G_NEXTWS") == null ? "" : jo.GetValue("G_NEXTWS").ToString();
            }
            else
            {
                Log.Debug("Mes服务器回复=>错误码：" + response + "；错误信息：" + strdata);
                result.IsSuccess = false;
                //(jo.GetValue("HEAD")as JObject).GetValue("H_MSG").ToString()
                result.Message = jo == null || jo.GetValue("H_MSG") == null ? "错误详情请咨询Mes服务器人员"
                    : jo.GetValue("H_MSG").ToString();
                result.Message = "MesStart 调用错误=>返回值:" + response + ";" + result.Message;
            }
            return result;
        }
        public static Result MesEnd(string sn) 
        {
            Result result = new Result();
            string ActionName = Settings.GetSetting("ActionName");
            string Tools = Settings.GetSetting("Tools");
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            int response = MesEnd(hMes, sn, ActionName, Tools, "0", strdata, ref len);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strdata.ToString());
            if (response == MesBackOK)
            {
                result.IsSuccess = true;
                result.Message = jo?.GetValue("G_NEXTWS") == null ? "" : jo.GetValue("G_NEXTWS").ToString();
            }
            else
            {
                Log.Debug("Mes服务器回复=>错误码：" + response + "；错误信息：" + strdata);
                result.IsSuccess = false;
                result.Message = jo == null || jo.GetValue("H_MSG") == null ? "MesEnd 调用错误=>返回值:" + response +
                    "\r\n错误详情请咨询Mes服务器人员"
                    : jo.GetValue("H_MSG").ToString();
            }
            return result;
        }
        public static Result MesEnd2(string sn,string data1)
        {
            Result result = new Result();
            string ActionName = Settings.GetSetting("ActionName");
            string Tools = Settings.GetSetting("Tools");
            string SNType = Settings.GetSetting("SNType");
            string allDataFormat = Settings.GetSetting("allDataFormat");
            string allData = string.Format(allDataFormat, data1);
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            int response = MesEnd2(hMes, sn, SNType, ActionName, Tools, "0", allData,strdata, ref len);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strdata.ToString());
            if (response == MesBackOK)
            {
                result.IsSuccess = true;
                result.Message = jo?.GetValue("G_NEXTWS") == null ? "" : jo.GetValue("G_NEXTWS").ToString();
            }
            else
            {
                Log.Debug("Mes服务器回复=>错误码：" + response + "；错误信息：" + strdata);
                result.IsSuccess = false;
                result.Message = jo == null || jo.GetValue("H_MSG") == null ? "MesEnd 调用错误=>返回值:" + response +
                    "\r\n错误详情请咨询Mes服务器人员"
                    : jo.GetValue("H_MSG").ToString();
            }
            return result;
        }
    }
}
