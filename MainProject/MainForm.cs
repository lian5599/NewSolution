using Communication;
using Communication.Scanner;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using FlowCharter;
using Helper;
using TKS.SubForm;
using Newtonsoft.Json.Linq;
using Northwoods.Go;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using TKS.Manager;
using TKS.FlowChart.Tool;
using Newtonsoft.Json;
using System.Configuration;
using System.Reflection;

namespace TKS
{
    public partial class MainForm : KryptonForm
    {
        #region Event
        public static Action UpdatePropertyGridEvent;

        #endregion
        UserType User;
        Assembly MesAssembly;
        DockPanel mdockPanel;
        bool _running = false;
        bool manualExit = false;

        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                if (value != _running)
                {
                    _running = value;
                    if (_running)
                    {
                        Btn_Start.Enabled = false;
                        Btn_Stop.Enabled = true;
                        Log.Run(LogLevel.Warming, "流程开始");
                    }
                    else
                    {
                        Btn_Start.Enabled = true;
                        Btn_Stop.Enabled = false;
                        Log.Run(LogLevel.Warming, "流程停止");
                    }
                }
            }
        }
        public MainForm()
        {
            ThreadPool.SetMinThreads(50, 50);//防止线程池阻塞，提高效率
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            LoadImage();
            this.IsMdiContainer = true;
            mdockPanel = new DockPanel();
            mdockPanel.Dock = DockStyle.Fill;
            mdockPanel.ActiveContentChanged += DockPanel1_ActiveContentChanged;
            this.Controls.Add(mdockPanel);
            toolStripLabel_Version.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void LoadImage()
        {
            #region image
            IconImage.loadIcon(@".\icon");
            //Btn_Start.ImageLarge = IconImage.Get("开始");
            //Btn_Stop.ImageLarge = IconImage.Get("停止");
            Btn_Start.Image = IconImage.Get("Play");
            Btn_Stop.Image = IconImage.Get("Pause");
            Btn_User.ImageLarge = IconImage.Get("用户");
            Btn_Hardware.ImageLarge = IconImage.Get("硬件调试");
            Btn_Log.ImageLarge = IconImage.Get("Log");
            Btn_Login.ImageSmall = IconImage.Get("登录");
            Btn_Param.ImageLarge = IconImage.Get("参数");
            Btn_Node.ImageLarge = IconImage.Get("工具");
            Btn_OpenProcess.ImageLarge = IconImage.Get("打开");
            Btn_NewProcess.ImageLarge = IconImage.Get("新建");
            Btn_SaveProcess.ImageLarge = IconImage.Get("保存");
            Btn_Flag.ImageLarge = IconImage.Get("信号");
            Btn_Setting.ImageLarge = IconImage.Get("参数");
            Btn_ShowAll.ImageLarge = IconImage.Get("显示");
            #endregion
        }
        private void LoginChange(UserType type)
        {
            switch (type)
            {
                case UserType.Technician:
                    Btn_User.Enabled = false;
                    SetModifiable(true);
                    break;
                case UserType.Administrator:
                    Btn_User.Enabled = true;
                    SetModifiable(true);
                    break;
                case UserType.Operator:
                    Btn_User.Enabled = false;
                    SetModifiable(false);
                    break;
                default: break;
            }
            UpdatePropertyGrid();
        }
        private void SetModifiable(bool b)
        {
            var subForms = mdockPanel.Contents.Where(item => item.GetType() == typeof(GraphViewWindow)).Select(item => item as DockContent);
            foreach (var item in subForms)
            {
                if (!item.IsHidden)
                {
                    item.CloseButton = b;
                }
            }
            RibbonTab_Setting.Visible = b;
            RibbonTab_Process.Visible = b;
            Flow.Instance.SetModifiable(b);
        }
        private void UpdateHardwareState(object sender, bool isconnected)
        {
            try
            {
                toolStrip1.Invoke((MethodInvoker)(() =>
                {
                    CommunicationBase communication = (CommunicationBase)sender;
                    foreach (var item in toolStrip1.Items)
                    {
                        ToolStripStatusLabel label = item as ToolStripStatusLabel;
                        if (label != null && label.Alignment == ToolStripItemAlignment.Left
                        && Hardware.Instance.hardwares.Find(hardware => hardware.Id == label.Name) == null)
                        {
                            toolStrip1.Items.Remove(label);
                        }
                    }
                    int index = toolStrip1.Items.IndexOfKey(communication.Id);
                    if (index == -1)
                    {
                        index = toolStrip1.Items.Count;
                        ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
                        toolStripStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
                        toolStripStatusLabel.Name = communication.Id;
                        //toolStripStatusLabel.Size = new System.Drawing.Size(73, 28);
                        toolStripStatusLabel.Text = communication.Id;
                        toolStripStatusLabel.BackColor = isconnected ? Color.LimeGreen : Color.Red;
                        //toolStripStatusLabel.Image = IconImage.Get("成功");
                        toolStrip1.Items.Insert(index, toolStripStatusLabel);
                    }
                    else
                    {
                        ToolStripStatusLabel toolStripStatusLabel = toolStrip1.Items[index] as ToolStripStatusLabel;
                        toolStripStatusLabel.BackColor = isconnected ? Color.LimeGreen : Color.Red;
                    }
                }));
            }
            catch (Exception)
            {
            }
        }
        private void periodOfValidityChanged(TimeSpan t)
        {
            try
            {
                toolStrip1.Invoke((MethodInvoker)(() =>
                {
                    if (t.Days < 30)
                    {
                        toolStripLabel_Tip.ForeColor = Color.Red;
                        toolStripLabel_Tip.Text = "有效期剩余：" + t.Days.ToString() + "天";
                    }
                    else
                    {
                        toolStripLabel_Tip.ForeColor = Color.Black;
                        toolStripLabel_Tip.Text = "TECHSON——Automation Changes The World";
                    }
                }));
            }
            catch (Exception E)
            {

            }
        }

        #region 弃用
        //标题字体
        private Font TitleFont = new Font("宋体", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
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
            User = (UserType)Convert.ToInt32(ConfigurationManager.AppSettings["AccountLevel"]);
            Security.periodOfValidityChanged = periodOfValidityChanged;
            Hardware.HardWareConnectionChanged = UpdateHardwareState;
            Account.LoginChangeEvent += LoginChange;
            UpdatePropertyGridEvent = UpdatePropertyGrid;

            HardWareForm.Instance.Create();
            SettingFrom.Instance.Create();
            LoadSubForm();
            timer_DeleteLog_Tick(null, null);
            Task.Run(() =>
            {
                MesAssembly = Assembly.LoadFrom("MesAdapter.dll");
                MesAssembly.GetType(ConfigurationManager.AppSettings["MesAdapter"])?.GetMethod("Init")?.Invoke(null, null);
            });
            Account.Instance.CurrentUser = User;//UserType.Operator;            
            //Btn_Start_Click(null, null);
        }
        private void UpdatePropertyGrid()
        {
            GraphNodeForm.GetInstance().RefreshUI();
            //GraphViewWindow w = this.ActiveMdiChild as GraphViewWindow;
            //if (w != null)
            //{
            //    GraphNodeForm.GetInstance().View = w.ActiveControl as GoView;
            //}
        }
        private void LoadSubForm()
        {
            #region GraphViewWindow init
            var views = GraphViewWindow.OpenAll();
            foreach (var item in views)
            {
                LoadUserObject(item);
            }
            #endregion

            int index = -1;
            try
            {
                mdockPanel.LoadFromXml(@".\Config\DockPanel.config", delegate (string persistString)
                    {
                        if (persistString == typeof(LogForm).ToString())
                        {
                            return LogForm.Instance;
                        }
                        if (persistString == typeof(GraphViewWindow).ToString())
                        {
                            index++;
                            if (views.Count > index)
                            {
                                return views[index];
                            }
                            return null;
                        }
                        if (User == UserType.Administrator)
                        {
                            if (persistString == typeof(FlowCharter.PaletteForm).ToString())
                            {
                                return FlowCharter.PaletteForm.GetInstance();
                            }
                            if (persistString == typeof(GraphNodeForm).ToString())
                            {
                                return GraphNodeForm.GetInstance();
                            } 
                        }
                        return null;
                    });

                for (index++; index < views.Count; index++)
                {
                    views[index].Show(mdockPanel, DockState.Float);
                }
            }
            catch (Exception)
            {

            }
            UpdateFlowInfo();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                manualExit = true;
                mdockPanel.SaveAsXml(@".\Config\DockPanel.config");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            foreach (var item in this.MdiChildren)
            {
                item.Dispose();
            }
            MesAssembly.GetType(ConfigurationManager.AppSettings["MesAdapter"])?.GetMethod("Close")?.Invoke(null, null);
        }

        private bool UpdateFlowInfo()
        {
            try
            {
                var subViews = mdockPanel.Contents.Where(item => item.GetType() == typeof(GraphViewWindow)).Select(item => item as GraphViewWindow);
                Flow.Instance.SubFlows.Clear();
                Flow.Instance.MainFlows.Clear();
                foreach (var canvas in subViews)
                {
                    GoDocument doc = canvas.Doc;
                    foreach (var item in canvas.Doc)
                    {
                        if (item is GraphNode)
                        {
                            GraphNode nodeItem = (GraphNode)item;
                            if (nodeItem.Kind == "SubStart")
                            {
                                Flow.Instance.SubFlows.Add(nodeItem.Text, nodeItem);
                            }
                            if (nodeItem.Kind == "Start")
                            {
                                Flow.Instance.MainFlows.Add(nodeItem);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                UiHelper.ShowError(e.Message);
                return false;
            }
        }

        private async void Btn_Start_Click(object sender, EventArgs e)
        {
            if (Running || !UpdateFlowInfo()) return;
            Running = true;
            manualExit = false;

            List<Task> tasks = new List<Task>();
            foreach (var item in Flow.Instance.MainFlows)
            {
                tasks.Add(Task.Run(() =>
                {
                    Result re;
                    ToolBase tool = item.UserObject as ToolBase;
                    do
                    {
                        re = tool.RunAll();
                        Thread.Sleep(10);
                    } while (re.ErrorCode != (int)ErrorCode.fatalError && !manualExit);

                    manualExit = true;
                }));
            }
            foreach (var item in tasks)
            {
                await item;
            }
            Running = false;
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            manualExit = true;
        }

        private void Btn_Hardware_Click(object sender, EventArgs e)
        {
            HardWareForm.Instance.Show(mdockPanel, DockState.DockLeft);
            //NewBoard("硬件", new CHardwareUI());
        }

        private void Btn_Flag_Click(object sender, EventArgs e)
        {
            //NewBoard("Plc信号", new PlcFlagUI());
        }

        private void Btn_Log_Click(object sender, EventArgs e)
        {
            LogForm.Instance.Show(mdockPanel, DockState.DockRight);
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

                    var userMatchId = Account.Instance.UserConfigs.Where(item => item.Id == userByte);
                    if (userMatchId.Count() == 0)
                    {
                        throw new Exception("用户名不存在!");
                    }
                    foreach (var item in userMatchId)
                    {
                        if (item.Password == passwordByte)
                        {
                            var ok = Enum.TryParse(Tool.Decrypt(item.Type), out UserType outType);
                            if (ok == false)
                                throw new Exception("不能识别的用户类型: " + item.Type);

                            Account.Instance.CurrentUser = outType;
                            this.Text = user;
                            Btn_Login.TextLine1 = "注销";
                            return;
                        }
                    }
                    throw new Exception("密码错误!");
                }
                else
                {
                    TextBox_User.Text = "admin";
                    TextBox_Password.Text = "";
                    Account.Instance.CurrentUser = UserType.Operator;
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
        private void Btn_Setting_Click(object sender, EventArgs e)
        {
            SettingFrom.Instance.Show(mdockPanel, DockState.DockRight);
        }

        private void Btn_Users_Click(object sender, EventArgs e)
        {
            AccountForm.GetInstance().Show(mdockPanel, DockState.Document);
        }

        private void Btn_Node_Click(object sender, EventArgs e)
        {
            FlowCharter.PaletteForm.GetInstance().Show(mdockPanel, DockState.DockRight);
        }

        private void Btn_Param_Click(object sender, EventArgs e)
        {
            GraphNodeForm.GetInstance().Show(mdockPanel, DockState.DockRight);
        }

        private void Btn_OpenProcess_Click(object sender, EventArgs e)
        {
            var canvas = GraphViewWindow.Open(mdockPanel);
            if (canvas == null) return;

            LoadUserObject(canvas);
            UpdateFlowInfo();
        }

        private void LoadUserObject(GraphViewWindow canvas)
        {
            var objs = Flow.Instance.FlowUserObj(canvas.Doc.Name);
            int index = 0;

            foreach (var item in canvas.Doc)
            {
                if (item is GraphNode)
                {
                    GraphNode nodeItem = (GraphNode)item;
                    try
                    {
                        for (int i = 0; i < objs[index].Count(); i++)
                        {
                            Type type = (nodeItem.UserObject as ToolBase).Settings[i].ProType;
                            object value = null;
                            if (!type.IsValueType && type != typeof(string) && type != typeof(MyComboItemConvert))
                            {
                                var method = typeof(JToken).GetMethod("ToObject", new Type[0]);
                                JToken token = objs[index][i].Value as JToken;
                                if (token != null)
                                {//object类型 反序列化后 需类型转换
                                    value = method.MakeGenericMethod(type).Invoke(token, null);
                                }
                            }
                            else value = objs[index][i].Value;
                            (nodeItem.UserObject as ToolBase).Settings[i].Value = value;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        index++;
                    }
                }
            }
        }

        private void Btn_NewProcess_Click(object sender, EventArgs e)
        {
            GraphViewWindow canvas = new GraphViewWindow();
            canvas.Show(mdockPanel, DockState.Document);
            canvas.View.UpdateFormInfo();
            GraphDoc doc = canvas.View.Doc;
            //doc.AddTitleAndAnnotation();
            doc.UndoManager.Clear();
            doc.IsModified = false;
        }

        private void Btn_SaveProcess_Click(object sender, EventArgs e)
        {
            GraphViewWindow canvas = mdockPanel.ActiveContent as GraphViewWindow;
            if (canvas == null) return;
            bool bSave = canvas.Save();
            if (bSave)
            {
                var objs = Flow.Instance.FlowUserObj(canvas.Doc.Name);
                objs.Clear();
                foreach (var item in canvas.Doc)
                {
                    if (item is GraphNode)
                    {
                        GraphNode nodeItem = (GraphNode)item;
                        (nodeItem.UserObject as ToolBase).SetPropValue("ID", objs.Count);
                        objs.Add((nodeItem.UserObject as ToolBase).Settings);
                    }
                }
                Flow.Instance.Save(canvas.Doc.Name);
                UpdateFlowInfo();
            }          
        }

        //protected override void OnMdiChildActivate(EventArgs evt)
        //{
        //    base.OnMdiChildActivate(evt);
        //    GraphViewWindow w = dockPanel1.ActiveContent as GraphViewWindow;//this.ActiveMdiChild as GraphViewWindow;
        //    if (w != null)
        //    {
        //        GraphNodeForm.GetInstance().View = w.ActiveControl as GoView;
        //        w.View.UpdateFormInfo();
        //    }
        //}

        private void DockPanel1_ActiveContentChanged(object sender, EventArgs e)
        {
            GraphViewWindow w = mdockPanel.ActiveContent as GraphViewWindow;
            if (w != null/*&& w.ActiveControl as GoView !=null*/)
            {
                GoView goView = w.ActiveControl as GoView;
                if (goView == null) return;
                GraphNodeForm.GetInstance().View = goView;
                w.View.UpdateFormInfo();
            }
        }

        private void Btn_ShowAll_Click(object sender, EventArgs e)
        {
            var subViews = mdockPanel.Contents.Where(item => item.GetType() == typeof(GraphViewWindow)).Select(item => item as GraphViewWindow);
            foreach (var item in subViews)
            {
                item.Show(mdockPanel, DockState.Document);
            }
        }

        private async void timer_DeleteLog_Tick(object sender, EventArgs e)
        {
            timer_DeleteLog.Enabled = false;
            await Task.Run(() =>
            {
                if (!Security.IsRegisterd(out TimeSpan t))
                {
                    Btn_Stop_Click(null, null);
                    FormRegister register = new FormRegister();
                    if (register.ShowDialog() != DialogResult.OK) Close();
                }

                Log.DeleteFile(30);
            });
            timer_DeleteLog.Enabled = true;
        }
    }
}
