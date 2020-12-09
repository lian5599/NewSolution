using Communication;
using Communication.Scanner;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using FlowCharter;
using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKS
{
    public partial class MainForm : KryptonForm
    {
        #region 默认参数
        //默认配置文件路径
        private const string defaultCfgFilePath = @"Config\";
        //板路径文件名
        private const string cfgFileName = "BoardPathsFile";
        //页面配置文件名
        private const string UiCfgFileName = @"UiCfg.cfg";
        //标题字体
        private Font TitleFont = new Font("宋体", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
        #endregion

        Manager1 manager;

        public MainForm()
        {
            InitializeComponent();
            manager = Manager1.GetInstance();
            ExDelegate.HardWareConnectionChanged = UpdateHardwareState;
            ExEvent.LoginChangeEvent += ExEvent_LoginChangeEvent;
            //manager.CurrentUser = UserType.Operator;
            //manager.LoadConfig();
        }
        private void ExEvent_LoginChangeEvent(UserType type)
        {
            switch (type)
            {
                case UserType.Administrator:
                    RibbonTab_Setting.Visible = true;
                    break;
                case UserType.Technician:
                    RibbonTab_Setting.Visible = true;
                    break;
                case UserType.Operator:
                    RibbonTab_Setting.Visible = false;
                    break;
                default: break;
            }
        }
        private void UpdateHardwareState(object sender, bool isconnected)
        {
            try
            {
                toolStrip1.Invoke((MethodInvoker)(() =>
                    {
                        CommunicationBase communication = (CommunicationBase)sender;
                        int index = toolStrip1.Items.IndexOfKey(communication.Id);
                        if (index == -1) index = toolStrip1.Items.Count;
                        else toolStrip1.Items.RemoveAt(index);
                        if (isconnected)
                        {
                            ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
                            toolStripStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                            toolStripStatusLabel.Name = communication.Id;
                            toolStripStatusLabel.Size = new System.Drawing.Size(73, 28);
                            toolStripStatusLabel.Text = communication.Id;
                            toolStripStatusLabel.Image = isconnected ?
                                Properties.Resources.成功 : Properties.Resources.失败;
                            toolStrip1.Items.Insert(index, toolStripStatusLabel);
                        }
                    }));
            }
            catch (Exception)
            {
            }
        }

        #region dockable
        private void kryptonDockableWorkspace_WorkspaceCellAdding(object sender, ComponentFactory.Krypton.Workspace.WorkspaceCellEventArgs e)
        {
            //e.Cell.Bar.ItemAlignment = RelativePositionAlign.Center;
            //e.Cell.Bar.TabBorderStyle = TabBorderStyle.SlantEqualFar;
            //e.Cell.StateCommon.Tab.Content.ShortText.Font = TitleFont;
            //new Font("宋体", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 134);
        }

        private void NewBoard(string Id, UserControl userControl)
        {
            foreach (var item in kryptonDockingManager.Pages)
            {
                if (item.Text == Id)
                {
                    return;
                }
            }
            KryptonPage[] kryptonPages = new KryptonPage[] { NewPage(Id, userControl) };

            try
            {
                kryptonDockingManager.AddToWorkspace("Workspace", kryptonPages);
            }
            catch (Exception e)
            {
                MessageBox.Show(Id + "已存在:" + e.Message);
            }
        }

        private KryptonPage NewPage(string name, Control content)
        {
            // Create new page with title and image
            KryptonPage p = new KryptonPage();
            p.Text = name;
            p.TextTitle = name;
            p.TextDescription = name;
            p.UniqueName = name;
            p.StateCommon.Tab.Content.ShortText.Font = TitleFont;
            // Add the control for display inside the page
            content.Dock = DockStyle.Fill;
            p.Controls.Add(content);
            return p;
        }
        #endregion
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Setup docking functionality
            KryptonDockingWorkspace w = kryptonDockingManager.ManageWorkspace(kryptonDockableWorkspace);
            kryptonDockingManager.ManageControl(kryptonPanel, w);
            kryptonDockingManager.ManageFloating(this);

            Btn_Node_Click(null, null);
            Btn_Param_Click(null, null);
            Btn_Process_Click(null, null);
            //NewBoard("日志",new LogUI());
            //NewBoard("Plc信号",new PlcFlagUI());
            try
            {
                kryptonDockingManager.LoadConfigFromFile(defaultCfgFilePath + UiCfgFileName);//加载保存的UI显示配置
            }
            catch (Exception ex)
            {
                MessageBox.Show(UiCfgFileName + "配置文件有误");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            kryptonDockingManager.SaveConfigToFile(defaultCfgFilePath + UiCfgFileName);
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            Manager1.GetInstance().Start();
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Manager1.GetInstance().Stop();
        }

        private void Btn_Hardware_Click(object sender, EventArgs e)
        {
            NewBoard("硬件", new CHardwareUI());
        }

        private void Btn_Flag_Click(object sender, EventArgs e)
        {
            NewBoard("Plc信号", new PlcFlagUI());
        }

        private void Btn_Log_Click(object sender, EventArgs e)
        {
            NewBoard("日志", new LogUI());
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                if (Btn_Login.TextLine1 != "注销")
                {
                    string user = TextBox_User.Text;
                    string password = TextBox_Password.Text;
                    var userByte = Tool.Encrypt(user);
                    var passwordByte = Tool.Encrypt(password);
                    bool match = false;
                    foreach (var item in manager.UserConfigs)
                    {
                        if (item.Id == userByte && item.Password == passwordByte)
                        {
                            var ok = Enum.TryParse(Tool.Decrypt(item.Type), out UserType outType);
                            if (ok == false)
                                throw new Exception("Unknown user type: " + item.Type);

                            manager.CurrentUser = outType;
                            match = true;
                            break;
                        }
                    }
                    if (match == false)
                    {
                        throw new Exception("User or password error!");
                    }
                    this.Text = user;
                    Btn_Login.TextLine1 = "注销";
                }
                else
                {
                    TextBox_User.Text = "User";
                    TextBox_Password.Text = "123456";
                    manager.CurrentUser = UserType.Operator;
                    this.Text = "Visitor";
                    Btn_Login.TextLine1 = "登录";
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(ex.Message);
                //Log.Error(ex.Message, ex);
            }
        }

        private void Btn_Users_Click(object sender, EventArgs e)
        {
            NewBoard("账号设置", new UserManagerUI());
        }

        private void Btn_Node_Click(object sender, EventArgs e)
        {
            //NewBoard("工具", new PaletteUI());
        }

        private void Btn_Param_Click(object sender, EventArgs e)
        {
            //NewBoard("参数", new GraphNodeUI());
        }

        private void Btn_Process_Click(object sender, EventArgs e)
        {
            //NewBoard("流程", new GraphViewUI());
        }

        private void Btn_OpenProcess_Click(object sender, EventArgs e)
        {
            
        }

        private void Btn_NewProcess_Click(object sender, EventArgs e)
        {

        }

        private void Btn_SaveProcess_Click(object sender, EventArgs e)
        {
        }
    }
}
