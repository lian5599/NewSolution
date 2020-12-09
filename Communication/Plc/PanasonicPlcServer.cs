using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communication.Plc
{
    public class PanasonicPlcServer<T> where T : ICommunication, new()
    {
        public byte[] Temp {get;set;}
        protected string receiveMsg { get; set; }

        ICommunication communication;
        public PanasonicPlcServer(string paramStr, int paramInt)
        {
            Temp = new byte[65536];
            communication = new T();
            communication.Initialize(paramStr, paramInt);
            communication.Open();
            communication.MessageRecieved += MessageRecieved;
        }


        public bool IsConnected()
        {
            if (communication == null)
            {
                return false;
            }
            return communication.IsConnected;
        }
        protected  void MessageRecieved(object sender, byte[] recievedMessage)
        {
            //<01#RDD1234512345**\r
            //<01$RD1234**\r
            //<01#WDD12345123450001**\r
            //<01$WD**\r
            receiveMsg = receiveMsg + Encoding.ASCII.GetString(recievedMessage);
            string responseStr = "";
            int ResponseEnd = receiveMsg.IndexOf("\r");
            bool read = receiveMsg.Contains("#RDD");
            bool write = receiveMsg.Contains("#WDD");
            string begin = receiveMsg.Substring(0, 3);
            string end = "**\r";
            if (ResponseEnd > 0)
            {
                int startAdd = Convert.ToInt32(receiveMsg.Substring(7, 5));
                int endAdd = Convert.ToInt32(receiveMsg.Substring(12, 5)) + 1;
                int length = endAdd - startAdd;
                if (read)
                {
                    byte[] responseByte = new byte[length * 2];
                    Array.Copy(Temp, startAdd, responseByte, 0, responseByte.Length);
                    string readValue = BitConverter.ToString(responseByte).Replace("-", "");
                    
                    responseStr = begin + "$RD" + readValue + end;
                    communication.Send(responseStr);
                }
                else if (write)
                {                   
                    string value = receiveMsg.Substring(17, 4 * length);
                    byte[] by = new byte[length * 2];
                    for (int i = 0; i < length * 2; i++)
                    {
                        by[i] = Convert.ToByte(value.Substring(0 + 2 * i, 2),16);
                    }
                    Array.Copy(by, 0, Temp, startAdd, by.Length);
                    responseStr = begin + "$WD" + end;
                    communication.Send(responseStr);
                }
                receiveMsg = receiveMsg.Substring(ResponseEnd + 1);
            //receiveMsg = string.Empty;
            }
        }
    }
}
