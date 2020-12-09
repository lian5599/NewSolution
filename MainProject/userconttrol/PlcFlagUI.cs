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
using Communication.Plc;
using System.Threading;

namespace TKS
{
    public partial class PlcFlagUI : UserControl
    {
        private const int writeValueColumn = 7;
        Manager1 manager;
        BindingList<FlagModule> binding;
        public PlcFlagUI()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            manager = Manager1.GetInstance();
            binding = new BindingList<FlagModule>(manager.flags);           
        }

        private void UpdateUI()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var item in binding)
                        {
                            var result = FlagBusiness.Read(item.Id);
                            //item.CurrentValue = result.Content;
                            //PlcBase plc = manager.plcs.Find(item1 => item1.Id == item.PlcId);
                            //var result = plc?.Read(item.Address);
                            //if (result !=null && result.IsSuccess)
                            //{
                            //    item.CurrentValue = result.Content;
                            //}
                            //else
                            //{
                            //    item.CurrentValue = -1;
                            //}
                            if (result.IsSuccess)
                            {
                                dgv.Invoke((MethodInvoker)(() =>
                                {
                                    dgv.Invalidate();
                                }));
                            }
                            Thread.Sleep(5);
                        }
                        Thread.Sleep(10);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        }
        private void CHardwareUI_Load(object sender, EventArgs e)
        {
            dgv.DataSource = binding;
            UpdateUI();
        }

        private void kryptonButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                FlagModule flag = new FlagModule();
                if (manager.plcs.Count > 0)
                {
                    flag.PlcId = manager.plcs.First().Id;
                }
                binding.Add(flag);
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
                var toRemove = binding.ElementAt(currentIndex);

                if (UiHelper.AskUser("To Remove " + toRemove.Id + " ?") == false)
                    return;

                binding.RemoveAt(currentIndex);
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
                manager.flags = new List<FlagModule>(binding);
                Configuration.SaveConfig(nameof(manager.flags), manager.flags);
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private async void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 & e.ColumnIndex >= 0)
            {
                if (dgv.Columns[e.ColumnIndex].Name == "Write")
                {
                    int writeValue = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[writeValueColumn].Value);
                    FlagModule flag = binding[e.RowIndex];

                    await Task.Run(() =>
                    {
                        FlagBusiness.Write(flag.Id, writeValue);
                    });
                }
            }
        }
    }
}
