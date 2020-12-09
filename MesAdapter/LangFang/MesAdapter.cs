using Communication;
using Helper;
using RestSharp;
using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace MesAdapter
{
    #region Virtual serial
    public class MesConnector<T> : CommunicationBase where T : ICommunication, new()
    {
        bool firstTime = true;
        bool mesResult = false;
        public MesConnector(string paramStr, int paramInt, int timeout = 2000)
        {
            Id = "Mes";
            ParamStr = paramStr;
            ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
            Connect();
        }
        public bool PassStation(string sn, int result, string UploadFormat)
        {
            mesResult = false;
            SendCommand(string.Format(UploadFormat, sn, result)/*, !firstTime*/);//"SN={0}^RESULT={1}"
            firstTime = false;
            return mesResult;
        }
        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            string responseStr = Encoding.ASCII.GetString(recievedMessage);
            responseStr = responseStr.Replace(" ", "");
            responseStr = responseStr.Replace("\r", "");
            responseStr = responseStr.Replace("\n", "");
            Log.Debug("Mes 回复：" + responseStr);
            if (responseStr == "0")
            {
                mesResult = true;
                ResponseEvent.Set();
            }
            else if (responseStr == "1")
            {
                mesResult = false;
                ResponseEvent.Set();
            }
        }
    }
    public static class MesAdapterBySerial
    {
        static bool[] LastErrorArray = new bool[96];
        static string paramStr;
        static int paramInt;
        static int timeout;
        static MesConnector<Serial> Mes;
        public static Result Init()
        {
            ErrorCodeCollection.LoadCfg();

            string paramStrNew = Settings.GetSetting("mes端口");
            int paramIntNew = Convert.ToInt32(Settings.GetSetting("mes波特率"));
            int timeoutNew = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            if (Mes != null)
            {
                bool isChanged = paramStr != paramStrNew
                    || paramInt != paramIntNew
                    || timeout != timeoutNew;
                if (!isChanged)
                {
                    string resultMsg = "";

                    if (!Mes.IsConnected())
                    {
                        resultMsg = "Mes连接失败,请检查串口线是否接好和参数是否设置正确";
                    }
                    return new Result(Mes.IsConnected(), resultMsg);
                }
                Mes.Close();
            }
            paramStr = paramStrNew;
            paramInt = paramIntNew;
            timeout = timeoutNew;
            Mes = new MesConnector<Serial>(paramStr, paramInt, timeout);
            Thread.Sleep(20);
            if (Mes.IsConnected())
            {
                //Mes.PassStation("1", 1, Settings.GetSetting("上传格式"));
                //Thread.Sleep(500);
                return new Result(true, "Mes连接成功");
            }
            return new Result(false, "Mes连接失败,请检查参数是否设置正确");
        }
        public static Result 过站(string sn, int result)
        {
            try
            {
                result--;
                bool IsSuccess = Mes.PassStation(sn, result, Settings.GetSetting("上传格式"));
                if (IsSuccess) return new Result(true, "过站成功");
                else return new Result(false, "过站失败");

            }
            catch (Exception e)
            {
                return new Result(e.Message);
            }
        }
        public static Result 机故上报(int e1, int e2, int e3)
        {
            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool[] ErrorArray = new bool[96];
            for (int i = 0; i < 32; i++)
            {
                ErrorArray[i] = BitHandle.GetBit(e1, i);
                ErrorArray[i + 32] = BitHandle.GetBit(e2, i);
                ErrorArray[i + 64] = BitHandle.GetBit(e3, i);
            }
            for (int i = 0; i < 96; i++)
            {
                if (ErrorArray[i] && !LastErrorArray[i])
                {
                    int code = i + 1;
                    string errorStr = ErrorCodeCollection.GetError(code);
                    bool IsSuccess = Post(errorStr, datetime, out string responseContent);
                    LastErrorArray[i] = ErrorArray[i];
                    if (IsSuccess)
                    {
                        Log.Run(LogLevel.Info, "机故上报=>编码:" + code.ToString() + "；描述:" + errorStr);
                        Log.Debug("机故上报成功，IOT回复：" + responseContent);
                    }
                    else
                    {
                        return new Result(false, "机故上报出错：" + responseContent/*, (int)ErrorCode.fatalError*/);
                    }
                }
            }
            LastErrorArray = ErrorArray;
            Thread.Sleep(50);
            return new Result(true);
        }

        private static bool Post(string errorMsg, string dateTime, out string responseContent)
        {
            string uri = Settings.GetSetting("IOT_URI");
            var client = new RestClient(uri);
            //client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("", DataFormat.Json).AddJsonBody(/*new Info() { ID = 1, Name = "张三" }*/new PostCfg(errorMsg, dateTime));
            client.Timeout = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            // execute the request
            IRestResponse response = client.Post(request);
            responseContent = response.Content;
            if (response.ErrorException != null)
            {
                Log.Error(response.ErrorMessage, response.ErrorException);
                responseContent = response.ErrorMessage;
            }
            return response.IsSuccessful;
        }
    }
    #endregion

    #region Socket Client
    public class MesConnectorBySocketClient<T> : CommunicationBase where T : ICommunication, new()
    {
        bool mesResult = false;
        public MesConnectorBySocketClient(string paramStr, int paramInt, int timeout = 2000)
        {
            Id = "Mes";
            ParamStr = paramStr;
            ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
            ConnectionChangeEvent -= Hardware.HardWareConnectionChanged;
            ConnectionChangeEvent += Hardware.HardWareConnectionChanged;
            Connect();
        }
        public bool CheckRoute(string sn)
        {
            mesResult = false;
            string UploadFormat = Settings.GetSetting("路由上传格式");
            SendCommand(string.Format(UploadFormat == "" ? "{0}" : UploadFormat, sn));
            return mesResult;
        }
        public bool PassStation(string sn)
        {
            mesResult = false;
            string UploadFormat = Settings.GetSetting("过站上传格式");
            SendCommand(string.Format(UploadFormat == "" ? "{0}" : UploadFormat, sn));
            return mesResult;
        }
        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            string responseStr = Encoding.ASCII.GetString(recievedMessage);
            Log.Debug("Mes 回复：" + responseStr);
            responseStr = responseStr.Replace(" ", "");
            responseStr = responseStr.Replace("\r", "");
            responseStr = responseStr.Replace("\n", "");
            int endIndex = responseStr.IndexOf("^");
            endIndex = endIndex == -1 ? responseStr.Length : endIndex;
            responseStr = responseStr.Substring(0, endIndex);
            if (responseStr == "OK")
            {
                mesResult = true;
                ResponseEvent.Set();
            }
            else if (responseStr == "NG")
            {
                mesResult = false;
                ResponseEvent.Set();
            }
        }
    }

    public static class MesAdapterBySocketClient
    {
        static bool[] LastErrorArray = new bool[96];
        static string paramStr;
        static int paramInt;
        static int timeout;
        static MesConnectorBySocketClient<SocketClient> Mes;
        public static Result Init()
        {
            ErrorCodeCollection.LoadCfg();
            //MyPing();
            return new Result();
           
            string paramStrNew = Settings.GetSetting("mes IP");
            int paramIntNew = Convert.ToInt32(Settings.GetSetting("mes 端口"));
            int timeoutNew = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            if (Mes != null)
            {
                bool isChanged = paramStr != paramStrNew
                    || paramInt != paramIntNew
                    || timeout != timeoutNew;
                if (!isChanged)
                {
                    string resultMsg = "";

                    if (!Mes.IsConnected())
                    {
                        resultMsg = "Mes连接失败,请检查参数是否设置正确";
                    }
                    return new Result(Mes.IsConnected(), resultMsg);
                }
                Mes.Close();
            }
            paramStr = paramStrNew;
            paramInt = paramIntNew;
            timeout = timeoutNew;
            Mes = new MesConnectorBySocketClient<SocketClient>(paramStr, paramInt, timeout);
            Thread.Sleep(20);
            if (Mes.IsConnected())
            {
                //Mes.PassStation("1", 1, Settings.GetSetting("上传格式"));
                //Thread.Sleep(500);
                return new Result(true, "Mes连接成功");
            }
            return new Result(false, "Mes连接失败,请检查参数是否设置正确");
        }
        public static Result 检查路由(string sn)
        {
            try
            {
                bool IsSuccess = Mes.CheckRoute(sn);
                if (IsSuccess) return new Result(true, "检查路由OK");
                else return new Result(false, "检查路由NG");

            }
            catch (Exception e)
            {
                return new Result(e.Message);
            }
        }
        public static Result 过站(string sn)
        {
            try
            {
                bool IsSuccess = Mes.PassStation(sn);
                if (IsSuccess) return new Result(true, "过站OK");
                else return new Result(false, "过站NG");

            }
            catch (Exception e)
            {
                return new Result(e.Message);
            }
        }
        public static Result 机故上报(int e1, int e2, int e3)
        {
            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool[] ErrorArray = new bool[96];
            for (int i = 0; i < 32; i++)
            {
                ErrorArray[i] = BitHandle.GetBit(e1, i);
                ErrorArray[i + 32] = BitHandle.GetBit(e2, i);
                ErrorArray[i + 64] = BitHandle.GetBit(e3, i);
            }
            for (int i = 0; i < 96; i++)
            {
                if (ErrorArray[i] && !LastErrorArray[i])
                {
                    int code = i + 1;
                    string errorStr = ErrorCodeCollection.GetError(code);
                    bool IsSuccess = Post(errorStr, datetime, out string responseContent);
                    LastErrorArray[i] = ErrorArray[i];
                    if (IsSuccess)
                    {
                        Log.Run(LogLevel.Info, "机故上报=>编码:" + code.ToString() + "；描述:" + errorStr);
                        Log.Debug("机故上报成功，IOT回复：" + responseContent);
                    }
                    else
                    {
                        return new Result(false, "机故上报出错：" + responseContent/*, (int)ErrorCode.fatalError*/);
                    }
                }
            }
            LastErrorArray = ErrorArray;
            Thread.Sleep(50);
            return new Result(true);
        }
        public static Result 机故上报2(int e1, int e2, int e3)
        {
            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool[] ErrorArray = new bool[96];
            for (int i = 0; i < 32; i++)
            {
                ErrorArray[i] = BitHandle.GetBit(e1, i);
                ErrorArray[i + 32] = BitHandle.GetBit(e2, i);
                ErrorArray[i + 64] = BitHandle.GetBit(e3, i);
            }
            for (int i = 0; i < 96; i++)
            {
                if (ErrorArray[i] && !LastErrorArray[i])
                {
                    int code = i + 1;
                    string errorStr = ErrorCodeCollection.GetError(code);
                    bool IsSuccess = Post2(errorStr, datetime, out string responseContent);
                    LastErrorArray[i] = ErrorArray[i];
                    if (IsSuccess)
                    {
                        Log.Run(LogLevel.Info, "机故上报=>编码:" + code.ToString() + "；描述:" + errorStr);
                        Log.Debug("机故上报成功，IOT回复：" + responseContent);
                    }
                    else
                    {
                        return new Result(false, "机故上报出错：" + responseContent/*, (int)ErrorCode.fatalError*/);
                    }
                }
            }
            LastErrorArray = ErrorArray;
            Thread.Sleep(50);
            return new Result(true);
        }
        public static Result 机故上报3()
        {
            string uri = Settings.GetSetting("IOT_URI1");
            var client = new RestClient(uri);
            var request = new RestRequest("", DataFormat.Json);
            string UploadFormat = Settings.GetSetting("格式1");
            request.AddParameter("application/json", UploadFormat, ParameterType.RequestBody);
            client.Timeout = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            // execute the request
            IRestResponse response = client.Post(request);
            Log.Debug(JsonConvertX.Serialize(response));
            if (response.ErrorException != null)
            {
                Log.Error(response.ErrorMessage, response.ErrorException);
            }
            return new Result(true);
        }

        private static bool Post(string errorMsg, string dateTime, out string responseContent)
        {
            string uri = Settings.GetSetting("IOT_URI");
            var client = new RestClient(uri);
            //client.Authenticator = new HttpBasicAuthenticator(username, password);
            var postObj = new PostCfg(errorMsg, dateTime);
            Log.Debug("上传数据：" + JsonConvertX.Serialize(postObj));
            var request = new RestRequest("", DataFormat.Json).AddJsonBody(/*new Info() { ID = 1, Name = "张三" }*/postObj);
            client.Timeout = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            // execute the request
            IRestResponse response = client.Post(request);
            responseContent = response.Content;
            if (response.ErrorException != null)
            {
                Log.Error(response.ErrorMessage, response.ErrorException);
                responseContent = response.ErrorMessage;
            }
            //response.ResponseStatus
            return response.IsSuccessful;
        }

        private static bool Post2(string errorMsg, string dateTime, out string responseContent)
        {
            string uri = Settings.GetSetting("IOT_URI");
            var client = new RestClient(uri);
            var request = new RestRequest("", DataFormat.Json);
            string UploadFormat = Settings.GetSetting("格式");
            string uploadStr = string.Format(UploadFormat, Settings.GetSetting("DeviceNo"), errorMsg, dateTime);
            Log.Debug("上传数据：" + uploadStr);
            request.AddParameter("application/json", uploadStr, ParameterType.RequestBody);
            client.Timeout = Convert.ToInt32(Settings.GetSetting("mes超时时间"));
            // execute the request
            IRestResponse response = client.Post(request);
            responseContent = response.Content;
            if (response.ErrorException != null)
            {
                Log.Error(response.ErrorMessage, response.ErrorException);
                responseContent = response.ErrorMessage;
            }
            //response.ResponseStatus
            return response.IsSuccessful;
        }
        static void MyPing()
        {
            Ping pingSender = new Ping();
            string URI = Settings.GetSetting("IOT_URI");
            int indexStart = URI.IndexOf("//") + 2;
            URI = URI.Substring(indexStart);
            int indexEnd = URI.IndexOf(":");
            URI = URI.Substring(0, indexEnd);
            PingReply reply = pingSender.Send(URI);
            if (reply.Status != IPStatus.Success)
            {
                Log.Run(LogLevel.Error,"Ping "+ URI+" 错误：" + reply.Status.ToString());
            }
        }
    } 
    #endregion

    public struct PostCfg
    {
        public string DeviceNo { get; set; }
        public string AlertDesc { get; set; }
        public string Datetime { get; set; }

        public PostCfg(string alert, string datetime)
        {
            DeviceNo = Settings.GetSetting("DeviceNo");
            AlertDesc = alert;
            Datetime = datetime;
        }
    }

    [Serializable]
    public class Info
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
