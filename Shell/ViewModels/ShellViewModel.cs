using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using FontAwesome5;
using HandyControl.Data;
using QA.Business;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Component.MES;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Procedure;
using QA.Business.Station;
using QA.IntelligentEquipment.Models;
using QA.Pages.Interfaces;
using QA.Pages.Models;
using QA.Pages.OtherViews.ViewModels;
using QA.Pages.ViewModels;
using QA.UserControls.ViewModels;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;
using StepStatus = QA.Business.Steps.StepStatus;
using UserManager = QA.Business.Manager.UserManager;//20250716 刷卡
using EN_UserType = QA.Business.Manager.EN_UserType;//20250716 刷卡

namespace QA.IntelligentEquipment.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IPageViewModel>.Collection.OneActive, IHandle<LoginSuccessMessage>, IHandle<List<IComponent>>/*, IHandle<TimeMessage>*///20250716 刷卡
    {
        #region Field
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private ParamManager _paramManager;
        private CacheParamManager _cacheParamManager;
        private TaskManager _taskManager;
        private UserManager _userManager;//20250716 刷卡
        private StepStatus _stepStatus;
        private GlobalVariable _globalVariable;
        private IBaseBiz _baseBiz;
        private Camera_Component _camera_Component;
        private MotionGoogol_Component _mGoogol_Component;
        private PLC_Component _plc_Component;
        private MES_Component _mes_Component;
        private Hive_Component _hive_Component;
        private static readonly SolidColorBrush QuickBackGround = new SolidColorBrush(Color.FromRgb(0x00, 0x8d, 0x86));//快克绿色
        private List<IComponent> _component;
        private List<IPageViewModel> viewModels;
        private string targetVMName = "";
        private DateTime lastActivityTime;
        #endregion

        #region Property
        public IPageViewModel _rbItemMainForm;
        public IPageViewModel rbItemMainForm
        {
            get => _rbItemMainForm;
            set
            {
                _rbItemMainForm = value;
                NotifyOfPropertyChange(() => rbItemMainForm);
            }
        }

        public IPageViewModel _rbItemSetup;
        public IPageViewModel rbItemSetup
        {
            get => _rbItemSetup;
            set
            {
                _rbItemSetup = value;
                NotifyOfPropertyChange(() => rbItemSetup);
            }
        }

        public IPageViewModel _rbItemCCD;
        public IPageViewModel rbItemCCD
        {
            get => _rbItemCCD;
            set
            {
                _rbItemCCD = value;
                NotifyOfPropertyChange(() => rbItemCCD);
            }
        }

        public IPageViewModel _rbItemAlarm;
        public IPageViewModel rbItemAlarm
        {
            get => _rbItemAlarm;
            set
            {
                _rbItemAlarm = value;
                NotifyOfPropertyChange(() => rbItemAlarm);
            }
        }

        public IPageViewModel _rbItemProductivity;
        public IPageViewModel rbItemProductivity
        {
            get => _rbItemProductivity;
            set
            {
                _rbItemProductivity = value;
                NotifyOfPropertyChange(() => rbItemProductivity);
            }
        }

        public IPageViewModel _rbItemLogin;
        public IPageViewModel rbItemLogin
        {
            get => _rbItemLogin;
            set
            {
                _rbItemLogin = value;
                NotifyOfPropertyChange(() => rbItemLogin);
            }
        }

        public GlobalVariable GlobalVariableProperty { get => _globalVariable; }

        public int _selecetedIndex;
        public int SelecetedIndex
        {
            get => _selecetedIndex;
            set
            {
                _selecetedIndex = value;
                NotifyOfPropertyChange("SelecetedIndex");
            }
        }

        private string _machineName = "LA400";
        public string MachineName
        {
            get => _machineName;
            set
            {
                _machineName = value;
                NotifyOfPropertyChange(() => MachineName);
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        private WindowState _windowState = WindowState.Maximized;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }

        private string _userStatus = "未登录";
        public string UserStatus
        {
            get => _userStatus;
            set
            {
                _userStatus = value;
                NotifyOfPropertyChange(() => UserStatus);
            }
        }

        private string _machineStatus;
        public string MachineStatus
        {
            get => _machineStatus;
            set
            {
                _machineStatus = value;
                NotifyOfPropertyChange(() => MachineStatus);
            }
        }

        private string _localStatusInfo;
        public string LocalStatusInfo
        {
            get => _localStatusInfo;
            set
            {
                _localStatusInfo = value;
                NotifyOfPropertyChange(() => LocalStatusInfo);
            }
        }

        private SolidColorBrush _statusForeColor;
        public SolidColorBrush StatusForeColor
        {
            get => _statusForeColor;
            set
            {
                _statusForeColor = value;
                NotifyOfPropertyChange(() => StatusForeColor);
            }
        }

        private LocalStatusModel _localStatus = new LocalStatusModel();
        public LocalStatusModel LocalStatus
        {
            get => _localStatus;
            set
            {
                _localStatus = value;
                NotifyOfPropertyChange(() => LocalStatus);
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        private bool _isHaveAlarm;
        public bool IsHaveAlarm
        {
            get => _isHaveAlarm;
            set
            {
                _isHaveAlarm = value;
                NotifyOfPropertyChange(() => IsHaveAlarm);
            }
        }

        private bool _isAutoRun = false;
        public bool IsAutoRun
        {
            get => _isAutoRun;
            set
            {
                _isAutoRun = value;
                NotifyOfPropertyChange(() => IsAutoRun);
            }
        }

        private string _softRunDateTime = string.Empty;
        public string SoftRunDateTime
        {
            get => _softRunDateTime;
            set
            {
                _softRunDateTime = value;
                NotifyOfPropertyChange(() => SoftRunDateTime);
            }
        }

        private string _taskName = string.Empty;
        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                NotifyOfPropertyChange(() => TaskName);
            }
        }

        private ObservableCollection<MenuItemModel> _menuItemModels = new ObservableCollection<MenuItemModel>();
        public ObservableCollection<MenuItemModel> MenuItemModels
        {
            get => _menuItemModels;
            set
            {
                _menuItemModels = value;
                NotifyOfPropertyChange(() => MenuItemModels);
            }
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        private SolidColorBrush _runModeBrush;
        public SolidColorBrush RunModeBrush
        {
            get => _runModeBrush;
            set
            {
                _runModeBrush = value;
                NotifyOfPropertyChange(() => RunModeBrush);
            }
        }

        private RobotRealStateInfo _robotRealStateInfo = new RobotRealStateInfo();
        public RobotRealStateInfo RobotRealStateInfos
        {
            get => _robotRealStateInfo;
            set
            {
                _robotRealStateInfo = value;
                NotifyOfPropertyChange(() => RobotRealStateInfos);
            }
        }

        private string _robotRealInfo;
        public string RobotRealInfo
        {
            get => _robotRealInfo;
            set
            {
                _robotRealInfo = value;
                NotifyOfPropertyChange(() => RobotRealInfo);
            }
        }

        private bool _enableButtons = false;
        public bool EnableButtons
        {
            get => _enableButtons;
            set
            {
                _enableButtons = value;
                NotifyOfPropertyChange(() => EnableButtons);
            }
        }

        public EN_HiveStatus HiveStatus => _hive_Component.HiveStatus;
        #endregion

        #region UI
        [Import("CarrierInfoPanelViewModel")]
        public CarrierInfoPanelViewModel CarrierInfoPanelViewModel { get; set; }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _paramManager = IoC.Get<ParamManager>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _taskManager = IoC.Get<TaskManager>();
            _userManager = IoC.Get<UserManager>();//20250716 刷卡
            _stepStatus = IoC.Get<StepStatus>();
            _globalVariable = IoC.Get<GlobalVariable>();
            _baseBiz = IoC.Get<IBaseBiz>();
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _mes_Component = (MES_Component)IoC.Get<IMES>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();

            MachineStatus = "  FA06-005  ";
            RunModeBrush = QuickBackGround;

            _paramManager.LoadParams();
            _cacheParamManager.LoadParams();

            BC750.Start();

            Init();
        }
        #endregion

        private object loginMsgLockobj = new object();

        /// <summary>
        /// 切换导航页面
        /// </summary>
        /// <param name="tag"></param>
        public void NavigatePage(string vmName)
        {
            lock (loginMsgLockobj)
            {
                if (string.IsNullOrEmpty(vmName))
                {
                    return;
                }
                var targetIndex = viewModels.ToList().FindIndex(t => t.GetType().Name.Equals(vmName));
                var loginIndex = viewModels.ToList().FindIndex(t => t.GetType().Equals(typeof(LoginPageViewModel)));
                if (targetIndex == -1 || loginIndex == -1)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"找不到页面{vmName}，需要修改软件", En_Logout_Type.Exception, true);
                    return;
                }
                if (vmName == typeof(MainFormPageViewModel).Name)
                {
                    //20250716 刷卡 点击主页面时自动注销
                    _userManager.UnLogin();
                    _hive_Component.InSpotCheckPage = false;
                }
                else if (vmName == typeof(SetupPageViewModel).Name || vmName == typeof(CCDPageViewModel).Name)
                {
                    //点击某些页面时需要Engineer权限，修改登录页面的用户选择
                    if (_userManager.CurrUserType < EN_UserType.Engineer)//20250716 刷卡
                    {
                        PublishNeedLoginMessage(EN_UserType.Engineer);//20250716 刷卡
                        SelecetedIndex = loginIndex;
                        targetVMName = vmName;
                        return;
                    }
                    _hive_Component.InSpotCheckPage = true;
                }
                else if (targetIndex == loginIndex)
                {
                    //点击登录页面时，修改登录页面的用户选择
                    PublishNeedLoginMessage(EN_UserType.Operator);//20250716 刷卡
                }
                try
                {
                    targetVMName = "";
                    SelecetedIndex = targetIndex;
                }
                catch (Exception ex)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Exception, true);
                }
            }
        }

        public void PublishNeedLoginMessage(EN_UserType targetUserType)
        {
            _eventAggregator.Publish(new NeedLoginMessage() { TargetUserType = targetUserType }, action => { Task.Run(action); });
        }

        public void OpenImgDir()
        {
            try
            {
                string dir = ((CameraParam)_camera_Component.Param).ImgPath;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                System.Diagnostics.Process.Start(dir);
            }
            catch (Exception e)
            {
                //ignored
            }
        }

        public void OpenCsvDir()
        {
            try
            {
                //string dir = @"D:\CsvData";
                string dir = @"D:\QKProject\Data";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                System.Diagnostics.Process.Start(dir);
            }
            catch (Exception e)
            {
                //ignored
            }
        }

        public void OpenLogDir()
        {
            //string dir = AppDomain.CurrentDomain.BaseDirectory + "Logs";
            string dir = @"D:\QKProject\Logs";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            System.Diagnostics.Process.Start(dir);
        }

        protected override void OnInitialize()
        {
            viewModels = IoC.GetAll<IPageViewModel>().OrderBy(item => item.OrderID).ToList();
            rbItemMainForm = viewModels[0];
            rbItemSetup = viewModels[1];
            rbItemCCD = viewModels[2];
            rbItemAlarm = viewModels[3];
            rbItemProductivity = viewModels[4];
            rbItemLogin = viewModels[5];
            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            InitCarrierPanel(_stepStatus.CurrentProcedure);
            base.OnActivate();
        }

        private void Init()
        {
            MachineName = System.Configuration.ConfigurationManager.AppSettings["MachineName"];
            StatusForeColor = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            LocalStatus.Version = $"Ver {Application.ResourceAssembly.GetName().Version}";
            LocalStatus.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            LocalStatusInfo = LocalStatus.ToString();
            RefreshLocalStatus();

            _stepStatus.CurrentProcedure = _taskManager.GetTaskByName(_cacheParamManager.RunInfoParam.CurTaskName);
            if (_stepStatus.CurrentProcedure == null)
            {
                if (_taskManager.Tasks.Count > 0)
                {
                    //选择已有的第一个制程
                    _stepStatus.CurrentProcedure = _taskManager.Tasks[0];
                    MessageBox.Warning("制程为空！\n自动选择制程 " + _stepStatus.CurrentProcedure.ProcedureName + "\n请重启软件！", "操作提示");
                }
                else
                {
                    //新生成一个制程
                    LaserSprayProcedure task = new LaserSprayProcedure();
                    task.ProcedureName = "默认制程";
                    _taskManager.SaveTask(null, task, out string info);
                    _stepStatus.CurrentProcedure = task;
                }
                _cacheParamManager.RunInfoParam.CurTaskName = _stepStatus.CurrentProcedure.ProcedureName;
                _cacheParamManager.SaveRunInfoParam();
                BC750.Stop();
                _mGoogol_Component.SetServeOnOff(false);//关闭所有伺服
                _cacheParamManager?.SaveRunInfoParam();
                TryClose();
                Environment.Exit(0);
            }
            if (!_stepStatus.CurrentProcedure.IsCavityReasonable())
            {
                MessageBoxResult vr = MessageBox.Show("该制程穴位号有冲突，修改后才能选择该制程！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Question);
                _stepStatus.CurrentProcedure = null;
                TaskName = "未选择制程";
                return;
            }
            TaskName = _cacheParamManager.RunInfoParam.CurTaskName;
            //这里修改显示没用，界面还没加载出来，要放到OnActivate里面
            //InitCarrierPanel(_stepStatus.CurProcedure);
        }

        /// <summary>
        /// 打开系统设置窗口
        /// </summary>
        public void SystemConfigWindow()
        {
            var loginSettings = new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } };

            //20250716 刷卡
            var previousType = _userManager.CurrUserType;
            _userManager.IsLoginDialogShowing = true;
            bool? result = _windowManager.ShowDialog(new LoginWindowViewModel(EN_UserType.Engineer, false), null, loginSettings);//20250716
            _userManager.IsLoginDialogShowing = false;

            if (result != true)
            {
                return;
            }
            dynamic settings = new ExpandoObject();
            settings.Height = 800;
            settings.Width = 1200;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.NoResize;
            _hive_Component.InSettingPage = true;
            _windowManager.ShowDialog(new SystemConfigPageViewModel(_windowManager, _eventAggregator), null, settings);
            //20250716 刷卡
            //_myUserManager.Login(previousType);
            _hive_Component.InSettingPage = false;
        }

        public void BtnSelectTask()
        {
            SelectTaskViewModel selectTaskViewModel = new SelectTaskViewModel();
            bool? result = _windowManager.ShowDialog(selectTaskViewModel);
            if (result == true && !string.IsNullOrEmpty(selectTaskViewModel.SelectedTaskName))
            {
                LaserSprayProcedure task = _taskManager.GetTaskByName(selectTaskViewModel.SelectedTaskName);
                if (task == null)
                {
                    MessageBox.Show("制程为空！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Question);
                    return;
                }
                if (!task.IsCavityReasonable())
                {
                    MessageBox.Show("该制程穴位号有冲突，修改后才能选择该制程！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Question);
                    return;
                }
                _stepStatus.CurrentProcedure = task;
                TaskName = selectTaskViewModel.SelectedTaskName;
                _cacheParamManager.RunInfoParam.CurTaskName = TaskName;
                _cacheParamManager.SaveRunInfoParam();
                InitCarrierPanel(task);
            }
        }

        public void InitCarrierPanel(LaserSprayProcedure procedure)
        {
            if (procedure == null)
            {
                return;
            }
            int rows = procedure.GeneralParam.CarrierRows;
            int cols = procedure.GeneralParam.CarrierColumns;
            CarrierInfoPanelViewModel.SetCarrierArray(rows, cols);
            CarrierInfoPanelViewModel.SetProcedureName(TaskName);

            List<TrayInfoModel> trayInfos = new List<TrayInfoModel>();
            if (procedure.VisionPoints.Count == 12)
            {
                //针对12个穴位的制程，专门进行匹配
                int[] cavityOrder = new[] { 10, 11, 12, 7, 8, 9, 4, 5, 6, 1, 2, 3, };
                for (int idx = 0; idx < 12; idx++)
                {
                    int cavity = cavityOrder[idx];
                    foreach (var item in procedure.VisionPoints)
                    {
                        if (item.CavityNum == cavity)
                        {
                            trayInfos.Add(new TrayInfoModel()
                            {
                                CavityNum = item.CavityNum,
                                IsUsed = item.IsUsed,
                                Status = item.IsUsed ? EN_TrayStatus.待料 : EN_TrayStatus.禁用,
                            });
                        }
                    }
                }
            }
            else
            {
                foreach (var item in procedure.VisionPoints)
                {
                    trayInfos.Add(new TrayInfoModel()
                    {
                        CavityNum = item.CavityNum,
                        IsUsed = item.IsUsed,
                        Status = item.IsUsed ? EN_TrayStatus.待料 : EN_TrayStatus.禁用,
                    });
                }
            }
            CarrierInfoPanelViewModel.SetTrayInfos(trayInfos);
        }

        public void MinimizeWindow()
        {
            WindowState = WindowState.Minimized;
        }

        public void MaximizeWindow()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public void CloseWindow()
        {
            if (MessageBox.Show(new MessageBoxInfo
            {
                Message = "退出软件?",
                Caption = "提示",
                Button = MessageBoxButton.YesNo,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.AskGeometry,
            }) != MessageBoxResult.Yes)
            {
                return;
            }
            BC750.Stop();
            _mGoogol_Component.SetServeOnOff(false);//关闭所有伺服
            _cacheParamManager?.SaveRunInfoParam();
            TryClose();
            Environment.Exit(0);
        }

        //public void ChangeWorkMode()
        //{
        //    if (!EnableButtons)
        //    {
        //        return;
        //    }
        //    //需要权限才能修改模式
        //    var settings = new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } };
        //    _myUserManager.IsLoginDialogShowing = true;
        //    bool? result = _windowManager.ShowDialog(new LoginWindowViewModel(MyUserManager.userTypeEngineer, true), null, settings);
        //    _myUserManager.IsLoginDialogShowing = false;
        //    if (result != true)
        //    {
        //        return;
        //    }
        //    if (_globalVariable.WorkMode == WorkMode.Product)
        //    {
        //        var dialogResult = MessageBox.Show("要切换至哪种模式？\n是表示切换至调试模式\n否表示切换至计划停机模式", "操作提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        //        if (dialogResult == MessageBoxResult.Yes)
        //        {
        //            _globalVariable.WorkMode = WorkMode.Debug;
        //        }
        //        else if (dialogResult == MessageBoxResult.No)
        //        {
        //            _globalVariable.WorkMode = WorkMode.PlannedDT;
        //        }
        //    }
        //    else
        //    {
        //        _globalVariable.WorkMode = WorkMode.Product;
        //    }
        //}

        public void ChangeHiveState()
        {
            if (!EnableButtons)
            {
                return;
            }
            lock (loginMsgLockobj)
            {
                var loginIndex = viewModels.ToList().FindIndex(t => t.GetType().Equals(typeof(LoginPageViewModel)));
                if (loginIndex == -1)
                {
                    return;
                }
                string targetVMName1 = viewModels[SelecetedIndex].GetType().Name;
                if (_userManager.CurrUserType < EN_UserType.Engineer)//20250716 刷卡
                {
                    PublishNeedLoginMessage(EN_UserType.Engineer);//20250716 刷卡
                    SelecetedIndex = loginIndex;
                    if (targetVMName1 != typeof(MainFormPageViewModel).Name)
                    {
                        targetVMName = targetVMName1;
                    }
                    return;
                }
            }
            var settings = new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } };
            bool? result = _windowManager.ShowDialog(new ChangeHiveState1ViewModel(), null, settings);
            if (result != true)
            {
                return;
            }
        }

        public void ChangeStartMode(object _icon)
        {
            //string ttt = (_icon as TextBlock).Tag.ToString();           
            if (null != _icon)
            {
                EFontAwesomeIcon icon = (EFontAwesomeIcon)_icon;

                switch (icon)
                {
                    case EFontAwesomeIcon.Solid_Play:
                        {
                            //_globalVariable.isStartMode = true;
                            break;
                        }
                    case EFontAwesomeIcon.Solid_Pause:
                        {
                            break;
                        }
                    case EFontAwesomeIcon.Solid_UndoAlt:
                        {
                            //_globalVariable.isStartMode = false;
                            break;
                        }
                }
            }
            else
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ChangeStartMode Method Exception{_icon.ToString()}");
            }
        }

        /// <summary>
        /// 实时刷新当前时间
        /// </summary>
        private async void RefreshLocalStatus()
        {
            await Task.Factory.StartNew(() =>
            {
                //bool NeedUpdateLastTime = false;//20250717 一楼版本 是否需要更新软件更新时间
                //_cacheParamManager.HomeUiParam.Statistic.AddSoftwareVersionIfNeed(_hive_Component.MainSoftwareSHA1, out NeedUpdateLastTime);
                //if (NeedUpdateLastTime)
                //{
                //    _paramManager.HiveParam.LastUpdateTime = DateTime.Now;//20250717 一楼版本 版本更新时更新软件更新时间
                //}

                int count = 0;
                string[] aliveName = new string[] { "1号撕膜吸嘴", "2号撕膜吸嘴", "3号撕膜吸嘴", "1号保压头", "2号保压头", "3号保压头", "4号保压头" };
                int[] alive = new int[aliveName.Length];
                var _baseBiz = (BaseBiz)IoC.Get<IBaseBiz>();
                while (true)
                {
                    Thread.Sleep(50);
                    if (_component == null)
                    {
                        continue;
                    }

                    count++;
                    //修改主界面顶端报警指示灯的颜色
                    if (count % 5 == 0)
                    {
                        if (_baseBiz.GetAlarmInfo().Count > 0)
                        {
                            IsHaveAlarm = !IsHaveAlarm;
                        }
                        else
                        {
                            IsHaveAlarm = true;
                        }
                    }
                    //大约1s
                    if (count % 20 == 0)
                    {
                        //定时刷新并保存uph数据
                        _cacheParamManager.HomeUiParam.Statistic.ResetUphAndSaveDataIfNeed();
                        //定时写心跳
                        _plc_Component.SetHeartBeat();
                        //当前数目，给plc写
                        _plc_Component.SetAddrValue(0, 5333, (ushort)_cacheParamManager.HomeUiParam.Statistic.CurrentNum);
                        //目标数目，从plc读
                        ushort value = 0;
                        _plc_Component.GetAddrValue(0, 5334, ref value);
                        _cacheParamManager.HomeUiParam.Statistic.TargetNum = value;
                        //ok ng，给plc写
                        _plc_Component.SetAddrValue(0, 5335, (ushort)_cacheParamManager.HomeUiParam.Statistic.OkCount);
                        _plc_Component.SetAddrValue(0, 5336, (ushort)_cacheParamManager.HomeUiParam.Statistic.NgCount);
                        //ct，给plc写
                        _plc_Component.SetAddrValue(0, 5340, (ushort)(_globalVariable.CT.CycleCT * 100));
                        //ok率
                        _plc_Component.SetAddrValue(0, 5516, (ushort)(_cacheParamManager.HomeUiParam.Statistic.OkRate * 1000));
                        //uph
                        ushort[] values5550 = new ushort[24];
                        for (int i = 0; i < 24; i++)
                        {
                            values5550[i] = (ushort)_cacheParamManager.HomeUiParam.Statistic._uph[i];
                        }
                        _plc_Component.SetAddrMultiValue(0, 5550, values5550);
                        //判断寿命是否清零
                        for (int index = 0; index < alive.Length; index++)
                        {
                            int aliveCount = _plc_Component.GetAlive(index);
                            if (aliveCount != -1)
                            {
                                if (aliveCount < alive[index])
                                {
                                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, aliveName[index] + "寿命变化：" + alive[index] + "->" + aliveCount, En_Logout_Type.Default, true);
                                }
                                alive[index] = aliveCount;
                            }
                        }
                        //全清
                        _plc_Component.GetAddrValue(0, 1805, ref value);
                        if (value == 1)
                        {
                            _cacheParamManager.HomeUiParam.Statistic.ResetUph();
                            _cacheParamManager.HomeUiParam.Statistic.CurrentNum = 0;
                            _cacheParamManager.HomeUiParam.Statistic.ResetCount();
                            _cacheParamManager.SaveHomeUiParam();
                            _plc_Component.SetAddrValue(0, 1805, 0);
                        }
                        //报警次数，给plc写
                        _plc_Component.SetAddrValue(0, 1590, (ushort)_cacheParamManager.HomeUiParam.Statistic.AlarmCount);
                        //三个时间，从plc读
                        _plc_Component.GetAddrValue(0, 1592, ref value);
                        _cacheParamManager.HomeUiParam.Statistic.RunTimeMinute = value;
                        _plc_Component.GetAddrValue(0, 1594, ref value);
                        _cacheParamManager.HomeUiParam.Statistic.IdleTimeMinute = value;
                        _plc_Component.GetAddrValue(0, 1596, ref value);
                        _cacheParamManager.HomeUiParam.Statistic.ErrTimeMinute = value;
                    }
                    //定时保存生产数据，大约30s
                    if (count % 600 == 0)
                    {
                        count = 0;
                        _cacheParamManager.SaveHomeUiParam();
                    }
                    //清零次数，600是上面所有次数的最小公倍数
                    if (count >= 600)
                    {
                        count -= 600;
                    }
                    //鼠标无操作并且不在系统参数设置界面
                    //if ((SelecetedIndex != 0) && (lastActivityTime.Minute > 0) && (!_hive_Component.InSettingPage))
                    //{
                    //    if (count % 6 == 0)
                    //    {
                    //        Logout();
                    //        lastActivityTime = DateTime.MinValue;
                    //        SelecetedIndex = 0;
                    //    }
                    //}
                    LocalStatus.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //LocalStatusInfo = LocalStatus.ToString();
                    LocalStatusInfo = $"{_paramManager.OtherSettingParam.SoftwareVersion}   {LocalStatus.CurrentTime}";
                    IsAutoRun = _baseBiz.AutoRun;
                    SoftRunDateTime = _cacheParamManager.GetAllTotalRunTime();
                    RobotRealInfo = "";
                }
            });
        }

        /// <summary>
        /// 更新当前登录用户及登录后跳转页面
        /// </summary>
        /// <param name="message"></param>
        public void Handle(LoginSuccessMessage message)
        {
            UserStatus = $"当前用户:{_userManager.CurrUserName}";//20250716 刷卡
            NavigatePage(targetVMName);
        }

        public void Start()
        {
            if (MessageBox.Ask("确定启动吗？", "提示") == MessageBoxResult.OK)
            {
                _plc_Component.TrigPlcButton(EN_Plc_TrigButton.Start);
            }
        }

        public void Pause()
        {
            if (MessageBox.Ask("确定暂停吗？", "提示") == MessageBoxResult.OK)
            {
                _plc_Component.TrigPlcButton(EN_Plc_TrigButton.Pause);
            }
        }

        public void Stop()
        {
            if (MessageBox.Ask("确定强制复位吗？\n即使机台正在运行，也会强制复位！", "提示") == MessageBoxResult.OK)
            {
                _plc_Component.TrigPlcButton(EN_Plc_TrigButton.Reset);
            }
        }

        public void Handle(List<IComponent> message)
        {
            _component = message;
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
            NotifyOfPropertyChange(() => HiveStatus);
        }

        //20250716 刷卡
        //public void Handle(TimeMessage message)
        //{
        //    // 更新最后活动时间  
        //    lastActivityTime = DateTime.Now;
        //}

        ///// <summary>
        ///// 切换界面和退出当前用户登录
        ///// </summary>
        //private void Logout()
        //{
        //    _myUserManager.Logout();
        //    NavigatePage("MainFormPageViewModel");
        //}

    }
}
