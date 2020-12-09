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

namespace TKS
{
    public partial class UserManagerUI : UserControl
    {
        Manager1 manager;
        BindingList<UserConfig> bingding;
        public UserManagerUI()
        {
            InitializeComponent();
            dgv.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            manager = Manager1.GetInstance();
            bingding = new BindingList<UserConfig>();
        }

        private void CHardwareUI_Load(object sender, EventArgs e)
        {
            foreach (var item in Enum.GetValues(typeof(UserType)))
            {
                typeDataGridViewTextBoxColumn.Items.Add(item.ToString());
            }
            bingding.Clear();
            foreach (var item in manager.UserConfigs)
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

        private void kryptonButtonAdd_Click(object sender, EventArgs e)
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
                manager.UserConfigs.Clear();
                foreach (var item in bingding)
                {
                    manager.UserConfigs.Add(new UserConfig()
                    {
                        Id = Tool.Encrypt(item.Id),
                        Password = Tool.Encrypt(item.Password),
                        Type = Tool.Encrypt(item.Type.ToString()),
                    });
                }
                Configuration.SaveConfig(nameof(UserConfig), manager.UserConfigs);
                manager.LoadConfig();
            }
            catch (Exception ex)
            {
                UiHelper.ShowException(ex);
            }
        }
    }
}
