using Communication;
using Communication.Plc;
using Communication.Scanner;
using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKS;
using WeifenLuo.WinFormsUI.Docking;

namespace TKS.SubForm
{
    public partial class HardWareForm : DockContent
    {
        #region singleton
        private static volatile HardWareForm _instance = null;
        private static readonly object _locker = new object();
        public static HardWareForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new HardWareForm();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion
        public void Create() { }

        Hardware hardware;
        BindingList<CommunicationBase> bingding;
        private HardWareForm()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            hardware = Hardware.Instance;
            bingding = new BindingList<CommunicationBase>(hardware.hardwares);
        }

        private void HardWareFrom_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetAssembly(typeof(CommunicationBase));
            Type[] types = assembly.GetTypes();
            var hardware = types.Where(t => t.BaseType == typeof(CommunicationBase));//(t => t.IsSubclassOf(typeof(CommunicationBase)));
            foreach (var item in hardware)
            {
                hardwareIdDataGridViewComboBoxColumn.Items.Add(item.Name.Replace("Base", ""));
            }
            var ScannerKind = types.Where(t => t.BaseType == typeof(ScannerBase));
            var PlcKind = types.Where(t => t.BaseType == typeof(PlcBase));
            foreach (var item in ScannerKind)
            {
                kindDataGridViewComboBoxColumn.Items.Add(item.Name.Replace("`1", ""));
            }
            foreach (var item in PlcKind)
            {
                kindDataGridViewComboBoxColumn.Items.Add(item.Name.Replace("`1", ""));
            }
            var mode = (types.Where(t => t.GetInterfaces().Contains(typeof(ICommunication)))).ToArray();
            foreach (var item in mode)
            {
                modeDataGridViewComboBoxColumn.Items.Add(item.Name);
            }

            dgv.DataSource = bingding;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bingding.Add(new CommunicationBase()
                {
                    HardwareId = "Scanner",
                    Kind = "M120",
                    Mode = "SocketClient",
                    ParamStr = "127.0.0.1",
                    ParamInt = 8080
                });
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            try
            {
                int currentIndex = dgv.CurrentRow.Index;
                var toRemove = bingding.ElementAt(currentIndex);

                if (UiHelper.AskUser("To Remove " + toRemove.Id + " ?") == false)
                    return;

                bingding.RemoveAt(currentIndex);
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (UiHelper.AskUser("To save?") == false)
                    return;
                hardware.hardwares = new List<CommunicationBase>(bingding);
                //Configuration.SaveConfig(nameof(hardware.hardwares), hardware.hardwares);
                hardware.SaveConfig();
                //hardware.LoadConfig();
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            _instance = null;

        }

        private void contextMenuStripOutput_Opening(object sender, CancelEventArgs e)
        {
            if (dgv.CurrentCell != null)
            {
                try
                {
                    dgv.CurrentCell.ReadOnly = true;
                    dgv.CurrentCell.ReadOnly = false;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
