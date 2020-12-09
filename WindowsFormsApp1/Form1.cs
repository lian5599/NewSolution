using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //var client = new RestClient(@"http://192.168.30.100:11234/PersonInfoQuery/Info");
            #region 1
            //client.Authenticator = new HttpBasicAuthenticator(username, password);
            //Info info = new Info() { ID = 1, Name = "张三" };
            //var request = new RestRequest("").AddParameter("byProvinceName", "广东", ParameterType.GetOrPost);//.AddXmlBody(new A() { byProvinceName = "广东" });
            ////request.AddHeader("content-type", "application/x-www-form-urlencoded");

            //client.Timeout = 1000;
            //IRestResponse response = client.Post(request);
            //var content = response.Content; // raw content as string 
            #endregion
            #region 2          
            //Info info = new Info() { ID = 1, Name = "张三" };
            //var request = new RestRequest("", DataFormat.Json).AddJsonBody(info);
            //client.Timeout = 1000;
            //IRestResponse response = client.Post(request);
            //var content = response.Content; // raw content as string 
            #endregion

            //var request = new RestRequest("http://192.168.30.100:11234/GetScore2?name=%E5%BC%A0%E4%B8%89", DataFormat.Json);

            //Info info = new Info() { ID = 1, Name = "张三" };

            //// execute the request
            //IRestResponse response = client.Get(request);
            //var content = response.Content; // raw content as string


            string data = "{\"ID\":\"1\",\"Name\":\"张三\"}";
            string str = HttpPost(@"http://127.0.0.1:7788/PersonInfoQuery/Info", data);
            //string str = HttpGet(@"http://192.168.30.100:11234/GetScore2?name=%E5%BC%A0%E4%B8%89");
            //User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(str);
            
        }


        public static string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                retString = string.Format("请求数据失败. 状态码：{0} ；消息：{1}", response.StatusCode, retString);
            }
            return retString;
        }

        public static string HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";//charset=utf-8//"application/x-www-form-urlencoded";//
                byte[] postDatabyte = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = postDatabyte.Length;
                Stream requesrStream = request.GetRequestStream();
                requesrStream.Write(postDatabyte, 0, postDatabyte.Length);
                requesrStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                reader.Close();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    retString = string.Format("请求数据失败. 状态码：{0} ；消息：{1}", response.StatusCode, retString);
                }
                return retString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }

    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Score { get; set; }
    }
    public class Info
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
    public struct A
    {
        public string byProvinceName { get; set; }
    }
}
