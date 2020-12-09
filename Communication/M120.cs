using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication;
using Communication.Helper;

namespace Scanner
{
    public class M120<T> : IScanner where T : ICommunication,new()
    {
        public string Id { get ; set ; }
        public string Sn { get; set; }
        public int responseTimeout { get; set; }

        private T _m120 { get; set; }

        private bool _responsed { get; set; } = false;

        private string _responsedMessage { get; set; }
        
        private static readonly object PortWriteLocker = new object();

        public M120(string id,int timeout = 2000)
        {
            Id = id;
            responseTimeout = timeout;
        }
        public void Connect()
        {
            try
            {
                _m120?.Reset();
                _m120 = new T();
                _m120.Initialize(Id);
                _m120.Open();
                _m120.MessageRecieved += _m120Serial_MessageRecieved;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public Result<string> Trigger()
        {
            try
            {
                SendCommand("LOFF\r\n", false);
                SendCommand("LON\r\n");
                SendCommand("LOFF\r\n", false);
                return new Result<string>() { Content = Sn};
            }
            catch (Exception e)
            {
                return new Result<string>(120, e.Message);
            }
        }

        public Task<Result<string>> TriggerAsync()
        {
            return Task.Run<Result<string>>(() =>
            {
                return Trigger();
            });
        }


        private void _m120Serial_MessageRecieved(object sender, string recievedMessage)
        {
            _responsedMessage = _responsedMessage + recievedMessage;

            if (_responsedMessage.Contains("Z97A"))
            {
                _responsed = true;
                _responsedMessage = _responsedMessage.Replace("Z97A", "");
                Sn = _responsedMessage.Replace("A79Z", "");
            }
        }

        private void SendCommand(string command, bool isWait = true)
        {
            lock (PortWriteLocker)
            {
                try
                {
                    _responsed = false;
                    _responsedMessage = string.Empty;
                    _m120.Send(command);
                    if (isWait)
                        WaitReceive(responseTimeout);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void WaitReceive(int milliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_responsed == false)
            {
                if (stopwatch.ElapsedMilliseconds > milliseconds)
                {
                    SendCommand("LOFF\r\n", false);
                    throw new Exception("Response timeout:"+ milliseconds.ToString());
                }
                Thread.Sleep(50);
            }
        }
    }
}
