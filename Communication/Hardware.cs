using Communication;
using Communication.Plc;
using Communication.Scanner;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    public class Hardware
    {
        private static volatile Hardware _instance = null;
        private static readonly object _locker = new object();
        public static Hardware Instance 
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new Hardware();
                        }
                    }
                }
                return _instance;
            }
        }

        public static ConnectionChangedHandler HardWareConnectionChanged;

        public static event Action PlcChangedEvent;
        public static event Action ScannerChangedEvent;
        public void OnPlcChanged()
        {
            PlcChangedEvent?.Invoke();
        }
        public void OnScannerChanged()
        {
            ScannerChangedEvent?.Invoke();
        }

        //public event 
        public List<CommunicationBase> hardwares { get; set; }
        public List<PlcBase> plcs { get; set; }
        public List<ScannerBase> scanners { get; set; }

        private Hardware()
        {
            hardwares = new List<CommunicationBase>();
            plcs = new List<PlcBase>();
            scanners = new List<ScannerBase>();
            LoadConfig();
        }

        public IPlc plc(string Id)
        {
            PlcBase plc = plcs.Find(item => item.Id == Id);
            if (plc != null)
            {
                return plc;
            }
            throw new Exception("PLC\"" + Id + "\"不存在");
        }

        public IScanner scanner(string Id)
        {
            ScannerBase scanner = scanners.Find(item => item.Id == Id);
            if (scanner != null)
            {
                return scanner;
            }
            throw new Exception("扫码枪\"" + Id + "\"不存在");
        }

        public void LoadConfig()
        {
            hardwares = Configuration.GetConfig<List<CommunicationBase>>(nameof(hardwares));
            #region RESET
            foreach (var item in plcs)
            {
                item.ConnectionChangeEvent -= HardWareConnectionChanged;
                HardWareConnectionChanged(item, false);
                item.Close();
            }
            plcs.Clear();
            foreach (var item in scanners)
            {
                item.ConnectionChangeEvent -= HardWareConnectionChanged;
                HardWareConnectionChanged(item, false);
                item.Close();
            }
            scanners.Clear();

            #endregion
            foreach (var item in hardwares)
            {
                var assembly = Assembly.GetAssembly(typeof(CommunicationBase));
                var type = assembly.GetType("Communication." + item.HardwareId + "." + item.Kind + "`1");
                var instanceType = type.MakeGenericType(assembly.GetType("Communication." + item.Mode));
                object[] parameters = new object[4];
                parameters[0] = item.Id;
                parameters[1] = item.ParamStr;
                parameters[2] = item.ParamInt;
                parameters[3] = item.Timeout;
                switch (item.HardwareId)
                {
                    case "Plc":
                        PlcBase plc = (PlcBase)Activator.CreateInstance(instanceType, parameters);
                        plc.ConnectionChangeEvent += HardWareConnectionChanged;
                        Task.Run(() => { plc.Connect(); });
                        plcs.Add(plc);
                        break;
                    case "Scanner":
                        ScannerBase scanner = (ScannerBase)Activator.CreateInstance(instanceType, parameters);
                        scanner.ConnectionChangeEvent += HardWareConnectionChanged;
                        Task.Run(() => { scanner.Connect(); });
                        //scanner.Connect();
                        scanners.Add(scanner);
                        break;
                    default:
                        break;
                }
            }
            OnPlcChanged();
            OnScannerChanged();
        }

        public void SaveConfig()
        {
            Configuration.SaveConfig(nameof(hardwares), hardwares);
            LoadConfig();
        }
    }
}
