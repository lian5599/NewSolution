using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Helper;
using Communication;
using System.Reflection;
using Communication.Scanner;
using Communication.Plc;

namespace TKS
{
    public partial class CHardwareUI : UserControl
    {
        Manager1 manager;
        BindingList<CommunicationBase> bingding;
        public CHardwareUI()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            manager = Manager1.GetInstance();
            bingding = new BindingList<CommunicationBase>(manager.hardwares);
        }

        private void CHardwareUI_Load(object sender, EventArgs e)
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
                kindDataGridViewComboBoxColumn.Items.Add(item.Name.Replace("`1",""));
            }
            var mode = (types.Where(t => t.GetInterfaces().Contains(typeof(ICommunication)))).ToArray();
            foreach (var item in mode)
            {
                modeDataGridViewComboBoxColumn.Items.Add(item.Name);
            }
            
            dgv.DataSource = bingding;
        }

        private void kryptonButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bingding.Add(new CommunicationBase() { HardwareId = "Scanner",Kind = "M120",Mode = "SocketClient",
                ParamStr = "127.0.0.1",ParamInt = 8080});
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private void kryptonButtonRemove_Click(object sender, EventArgs e)
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

        private void kryptonButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (UiHelper.AskUser("To save?") == false)
                    return;
                manager.hardwares = new List<CommunicationBase>(bingding);
                Configuration.SaveConfig(nameof(manager.hardwares), manager.hardwares);
                manager.LoadConfig();
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }
    }
}
