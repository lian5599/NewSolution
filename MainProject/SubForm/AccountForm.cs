using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKS.Manager;
using WeifenLuo.WinFormsUI.Docking;

namespace TKS.SubForm
{
    public partial class AccountForm : DockContent
    {
        #region singleton
        private static volatile AccountForm _instance = null;
        private static readonly object _locker = new object();
        public static AccountForm GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new AccountForm();
                    }
                }
            }
            return _instance;
        }
        #endregion

        BindingList<UserConfig> bingding;
        private AccountForm()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            bingding = new BindingList<UserConfig>();
            this.HideOnClose = true;//only hide instead close
        }

        private void _Load(object sender, EventArgs e)
        {
            foreach (var item in Enum.GetValues(typeof(UserType)))
            {
                typeDataGridViewComboBoxColumn.Items.Add(item.ToString());
            }
            bingding.Clear();
            foreach (var item in Account.Instance.UserConfigs)
            {
                bingding.Add(new UserConfig()
                {
                    Id = Tool.Decrypt(item.Id),
                    Password = Tool.Decrypt(item.Password),
                    Type = Tool.Decrypt(item.Type.ToString()),
                });
            }

            dgv.DataSource = bingding;
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bingding.Add(new UserConfig());
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (UiHelper.AskUser("To save?") == false)
                    return;
                Account.Instance.UserConfigs.Clear();
                foreach (var item in bingding)
                {
                    Account.Instance.UserConfigs.Add(new UserConfig()
                    {
                        Id = Tool.Encrypt(item.Id),
                        Password = Tool.Encrypt(item.Password),
                        Type = Tool.Encrypt(item.Type.ToString()),
                    });
                }
                Configuration.SaveConfig(nameof(UserConfig), Account.Instance.UserConfigs);
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
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
