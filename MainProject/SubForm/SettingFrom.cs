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
    public partial class SettingFrom : DockContent
    {
        #region singleton
        private static volatile SettingFrom _instance = null;
        private static readonly object _locker = new object();
        public static SettingFrom Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new SettingFrom();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        //private List<SettingCfg> settings = new List<SettingCfg>();
        public void Create() { }

        BindingList<SettingCfg> bingding;
        private SettingFrom()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            Settings.Config = Configuration.GetConfig<List<SettingCfg>>("Setting");
            bingding = new BindingList<SettingCfg>(Settings.Config);
            this.HideOnClose = true;//only hide instead close
        }

        private void HardWareFrom_Load(object sender, EventArgs e)
        {
            dgv.DataSource = bingding;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bingding.Add(new SettingCfg("",""));
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

                if (UiHelper.AskUser("To Remove " + toRemove.Key + " ?") == false)
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
                Configuration.SaveConfig("Setting", Settings.Config);
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

        //public string GetSetting(string key)
        //{
        //    var setting = Settings.Config.Find(item => item.Key == key);
        //    return setting == null ? "" : setting.Value.ToString();
        //}
        //public void SetSetting(string key,string value)
        //{
        //    var setting = Settings.Config.Find(item => item.Key == key);
        //    if (setting == null) bingding.Add(new SettingCfg(key, value));
        //    else
        //    {
        //        setting.Value = value;
        //        //bingding = new BindingList<SettingCfg>(settings);
        //    }
        //}

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
    //public class SettingCfg
    //{
    //    public string Key { get; set; }
    //    public string Value { get; set; }
    //    public SettingCfg(string key, string value)
    //    {
    //        Key = key;
    //        Value = value;
    //    }
    //}

}
