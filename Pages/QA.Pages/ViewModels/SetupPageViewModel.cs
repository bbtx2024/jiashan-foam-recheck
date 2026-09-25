/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-23
 * 说明：（点检功能逻辑）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using QA.Business;
using QA.Business.Component.Camera;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.PLC;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Model.RunTimeInfo;
using QA.Business.Procedure;
using QA.Business.Recipe;
using QA.Business.Steps;
using QA.Pages.Interfaces;
using QA.SpotCheckPages;
using QA.UserControls.Interfaces;
using QA.UserControls.ViewModels;
using QA_Infrastructure;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.Pages.ViewModels
{
    [Export(typeof(IPageViewModel))]
    public class SetupPageViewModel : Screen, INotifyPropertyChanged, IPageViewModel
    {
        #region Field
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private ParamManager _paramManager;
        private CacheParamManager _cacheParamManager;
        private RecipeManager _recipeManager;
        private TaskManager _taskManager;
        private GlobalVariable _globalVariable;
        private MotionGoogol_Component _mGoogol_Component;
        private Camera_Component _camera_Component;
        private PLC_Component _plc_Component;
        private StepStatus _stepStatus;
        #endregion

        #region UI
        [Import("SpotCheckMesPageViewModel", typeof(ISpotPageViewModel))]
        public ISpotPageViewModel SpotCheckMesPageViewModel { get; set; }

        [Import("SpotCheckScannerPageViewModel", typeof(ISpotPageViewModel))]
        public ISpotPageViewModel SpotCheckScannerPageViewModel { get; set; }

        [Import("SpotCheckPlcPageViewModel", typeof(ISpotPageViewModel))]
        public ISpotPageViewModel SpotCheckPlcPageViewModel { get; set; }

        [Import("SpotCheckCameraPageViewModel", typeof(ISpotPageViewModel))]
        public ISpotPageViewModel SpotCheckCameraPageViewModel { get; set; }

        [Import("SpotCheckFTPUploadImgViewModel", typeof(ISpotPageViewModel))]
        public ISpotPageViewModel SpotCheckFTPUploadImgViewModel { get; set; }

        [Import("RobotTeachNewViewModel", typeof(IUserControl))]
        public RobotTeachNewViewModel RobotTeachNewViewModel;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "Two";

        public ushort OrderID { get; set; } = 1;

        public RuntimeLogs RunTimeLogs { get; } = IoC.Get<RuntimeLogs>();

        public byte _selectQuickPage { get; set; } = 0;
        public byte SelectQuickPage
        {
            get { return _selectQuickPage; }
            set
            {
                _selectQuickPage = value;
                NotifyOfPropertyChange(() => SelectQuickPage);
            }
        }

        /// <summary>
        /// 选择相机类型List
        /// </summary>
        private List<string> _selectCameraType = new List<string>();
        public List<string> SelectCameraType
        {
            get
            {
                _selectCameraType.Clear();
                _selectCameraType.Add("上相机");
                _selectCameraType.Add("下相机");
                return _selectCameraType;
            }
            set { _selectCameraType = value; NotifyOfPropertyChange(() => SelectCameraType); }
        }

        /// <summary>
        /// 相机名称
        /// </summary>
        private string _cameraName = "上相机";
        public string CameraName
        {
            get { return _cameraName; }
            set { _cameraName = value; NotifyOfPropertyChange(() => CameraName); }
        }

        //Hidden,Collapsed,Visible
        private string _visualStep1 = "Visible";
        public string VisualStep1
        {
            get => _visualStep1;
            set
            {
                _visualStep1 = value;
                NotifyOfPropertyChange(() => VisualStep1);
            }
        }

        private string _visualStep2 = "Hidden";
        public string VisualStep2
        {
            get => _visualStep2;
            set
            {
                _visualStep2 = value;
                NotifyOfPropertyChange(() => VisualStep2);
            }
        }

        private string _visualStep3 = "Hidden";
        public string VisualStep3
        {
            get => _visualStep3;
            set
            {
                _visualStep3 = value;
                NotifyOfPropertyChange(() => VisualStep3);
            }
        }

        private string _visualStep4 = "Hidden";
        public string VisualStep4
        {
            get => _visualStep4;
            set
            {
                _visualStep4 = value;
                NotifyOfPropertyChange(() => VisualStep4);
            }
        }

        private int _selNozzleNo = 1;
        public int SelNozzleNo
        {
            get => _selNozzleNo;
            set
            {
                _selNozzleNo = value;
                NotifyOfPropertyChange(() => SelNozzleNo);
            }
        }

        private List<int> _nozzleNoLst = new List<int>();
        public List<int> NozzleNoLst
        {
            get => _nozzleNoLst;
            set
            {
                _nozzleNoLst = value;
                NotifyOfPropertyChange(() => NozzleNoLst);
            }
        }

        private int _selCavityNo = 1;
        public int SelCavityNo
        {
            get => _selCavityNo;
            set
            {
                _selCavityNo = value;
                NotifyOfPropertyChange(() => SelCavityNo);
            }
        }

        private List<int> _cavityNoLst = new List<int>();
        public List<int> CavityNoLst
        {
            get => _cavityNoLst;
            set
            {
                _cavityNoLst = value;
                NotifyOfPropertyChange(() => CavityNoLst);
            }
        }

        /// <summary>
        /// 制程页面显示的制程
        /// </summary>
        private LaserSprayProcedure _currProcedure = null;
        public LaserSprayProcedure CurrProcedure
        {
            get { return _currProcedure; }
            set
            {
                _currProcedure = value;
                NotifyOfPropertyChange(() => CurrProcedure);
            }
        }

        /// <summary>
        /// 制程页面显示的制程对应的索引。如果该制程并非加载的制程，则索引为-1
        /// </summary>
        private int IdxOfOriProcedure = -1;

        /// <summary>
        /// 如果为加载制程，该值表示被加载的制程副本
        /// </summary>
        private LaserSprayProcedure OriProcedure = null;

        /// <summary>
        /// 当前选择的视觉点
        /// </summary>
        public LaserSprayVisionPoint _selectedVisionPoint = null;
        public LaserSprayVisionPoint SelectedVisionPoint
        {
            get => _selectedVisionPoint;
            set
            {
                _selectedVisionPoint = value;
                NotifyOfPropertyChange(() => SelectedVisionPoint);
            }
        }

        /// <summary>
        /// 当前选择的贴合点
        /// </summary>
        public LaserSpraySolderPoint _selectedSolderPoint = null;
        public LaserSpraySolderPoint SelectedSolderPoint
        {
            get => _selectedSolderPoint;
            set
            {
                _selectedSolderPoint = value;
                NotifyOfPropertyChange(() => SelectedSolderPoint);
            }
        }

        /// <summary>
        /// 当前焊接配方
        /// </summary>
        private LaserSprayRecipe _selectedRecipe = null;
        public LaserSprayRecipe SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;
                NotifyOfPropertyChange(() => SelectedRecipe);
            }
        }

        /// <summary>
        /// 配方列表
        /// </summary>
        public ObservableCollection<LaserSprayRecipe> _recipes = new ObservableCollection<LaserSprayRecipe>();
        public ObservableCollection<LaserSprayRecipe> Recipes
        {
            get => _recipes;
            set
            {
                _recipes = value;
                NotifyOfPropertyChange(() => Recipes);
            }
        }

        #endregion

        public SetupPageViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _paramManager = IoC.Get<ParamManager>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _recipeManager = IoC.Get<RecipeManager>();
            _taskManager = IoC.Get<TaskManager>();
            _globalVariable = IoC.Get<GlobalVariable>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _stepStatus = IoC.Get<StepStatus>();

            Recipes.Clear();
            if (_recipeManager.Recipes != null)
            {
                foreach (LaserSprayRecipe recipe in _recipeManager.Recipes)
                {
                    Recipes.Add(recipe);
                }
            }
            NozzleNoLst.Add(1);
            NozzleNoLst.Add(2);
            NozzleNoLst.Add(3);
            NozzleNoLst.Add(4);
            CavityNoLst.Add(1);
            CavityNoLst.Add(2);
            CavityNoLst.Add(3);
            CavityNoLst.Add(4);
            CavityNoLst.Add(5);
            CavityNoLst.Add(6);
            CavityNoLst.Add(7);
            CavityNoLst.Add(8);
            CavityNoLst.Add(9);
            CavityNoLst.Add(10);
            CavityNoLst.Add(11);
            CavityNoLst.Add(12);
            VisualStep1 = "Visible";
            VisualStep2 = "Hidden";
            VisualStep3 = "Hidden";
            VisualStep4 = "Hidden";
        }

        #region Caliburn.Micro
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (RobotTeachNewViewModel != null && RobotTeachNewViewModel.IsActive)
            {
                RobotTeachNewViewModel.TryClose();
            }
            base.OnDeactivate(close);
        }
        #endregion

        #region 制程操作

        /// <summary>
        /// 返回当前显示在制程页面的制程是否属于制程列表，且该制程有变动
        /// </summary>
        /// <returns>如果有变动，返回true；否则返回false</returns>
        private bool IsProcedureModifyAndNotSave()
        {
            if (CurrProcedure == null)
            {
                // 当前为null说明制程页面没有制程
                return false;
            }
            if (IdxOfOriProcedure == -1 || OriProcedure == null)
            {
                // 制程页面有制程，且该制程非读取得到，此时该制程仅在制程页面，应该要存储
                return true;
            }
            // 现在二者都不为空，必定为载入的制程，比较是否有变化
            return !OriProcedure.Equals(CurrProcedure);
        }

        /// <summary>
        /// 加载制程。选择一个制程，让其副本显示在制程页面。保存时会移除原有制程。
        /// </summary>
        public void LoadProcedure()
        {
            if (IsProcedureModifyAndNotSave())
            {
                if (MessageBox.Show("当前制程未保存！\n如果继续，未保存的改动将会丢失！\n确认继续吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    return;
                }
            }
            SelectTaskViewModel selectTaskViewModel = new SelectTaskViewModel();
            selectTaskViewModel.DisplayName = "请选择要加载的制程";
            bool? result = _windowManager.ShowDialog(selectTaskViewModel);
            if (result == true)
            {
                OriProcedure = _taskManager.GetTaskByName(selectTaskViewModel.SelectedTaskName);
                IdxOfOriProcedure = _taskManager.Tasks.IndexOf(OriProcedure);
                CurrProcedure = OriProcedure.DeepCopy();
                SelectedVisionPoint = null;
                SelectedSolderPoint = null;
                if (!CurrProcedure.IsCavityReasonable())
                {
                    MessageBox.Show("当前制程穴位号有冲突，修改后才能保存！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Question);
                }
            }
        }

        /// <summary>
        /// 新建制程。让一个新制程显示在编辑页面。保存时需检查是否存在同名制程。
        /// </summary>
        public void NewProcedure()
        {
            if (IsProcedureModifyAndNotSave())
            {
                if (MessageBox.Show("当前制程未保存！\n如果继续，未保存的改动将会丢失！\n确认继续吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    return;
                }
            }
            OriProcedure = null;
            IdxOfOriProcedure = -1;
            CurrProcedure = new LaserSprayProcedure()
            {
                ProcedureName = $"{DateTime.Now.ToString("MMdd_HHmmss")}"
            };
            SelectedVisionPoint = null;
            SelectedSolderPoint = null;
            MessageBox.Show("已新建并加载制程 " + CurrProcedure.ProcedureName + "！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 复制制程。选择一个制程，让其副本改名后显示在制程页面。保存时需检查是否存在同名制程。
        /// </summary>
        public void CopyProcedure()
        {
            SelectTaskViewModel selectTaskViewModel = new SelectTaskViewModel();
            selectTaskViewModel.DisplayName = "请选择要复制的制程";
            bool? result = _windowManager.ShowDialog(selectTaskViewModel);
            if (result == true)
            {
                OriProcedure = null;
                IdxOfOriProcedure = -1;
                CurrProcedure = _taskManager.GetTaskByName(selectTaskViewModel.SelectedTaskName).DeepCopy();
                CurrProcedure.ProcedureName += " - 副本";
                //判断是否已有同名，有则递增序号
                if (_taskManager.GetTaskByName(CurrProcedure.ProcedureName) != null)
                {
                    int i = 2;
                    while (_taskManager.GetTaskByName(CurrProcedure.ProcedureName + " (" + i + ")") != null)
                    {
                        i++;
                    }
                    CurrProcedure.ProcedureName += " (" + i + ")";
                }
                SelectedVisionPoint = null;
                SelectedSolderPoint = null;
                if (!CurrProcedure.IsCavityReasonable())
                {
                    MessageBox.Show("当前制程穴位号有冲突，修改后才能保存！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                MessageBox.Show("已生成制程 " + CurrProcedure.ProcedureName + "，可载入该制程后进行编辑！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 删除制程。选择一个制程，将该制程移除列表，并删除对应制程文件。
        /// </summary>
        public void DeleteProcedure()
        {
            SelectTaskViewModel selectTaskViewModel = new SelectTaskViewModel();
            selectTaskViewModel.DisplayName = "请选择要删除的制程";
            bool? result = _windowManager.ShowDialog(selectTaskViewModel);
            if (result == true)
            {
                LaserSprayProcedure task = _taskManager.GetTaskByName(selectTaskViewModel.SelectedTaskName).DeepCopy();
                if (MessageBox.Show("确定删除制程 " + selectTaskViewModel.SelectedTaskName + " 吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    if (OriProcedure == task)
                    {
                        // 说明当前制程是要删除的制程
                        OriProcedure = null;
                        IdxOfOriProcedure = -1;
                    }
                    _taskManager.DeleteTask(task);
                }
                MessageBox.Show("已删除制程 " + task.ProcedureName + "！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 保存制程。保存显示在制程页面的制程。如果该制程源于载入，则删除原制程后修改列表中对应制程；否则作为新制程添加到列表并保存文件。
        /// </summary>
        public void SaveProcedure()
        {
            if (CurrProcedure == null)
            {
                MessageBox.Warning("请先新建或加载制程！", "操作提示");
                return;
            }
            if (!CurrProcedure.IsCavityReasonable())
            {
                MessageBox.Warning("当前制程穴位号有冲突，无法保存！", "操作提示");
                return;
            }
            if (MessageBox.Show("确定保存当前制程吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (!_taskManager.SaveTask(OriProcedure, CurrProcedure, out string info))
                {
                    MessageBox.Error($"制程保存失败！\n{info}", "操作提示");
                }
                else
                {
                    OriProcedure = _taskManager.GetTaskByName(CurrProcedure.ProcedureName);
                    IdxOfOriProcedure = _taskManager.Tasks.IndexOf(OriProcedure);
                    CurrProcedure = OriProcedure.DeepCopy();
                    SelectedVisionPoint = null;
                    SelectedSolderPoint = null;
                    MessageBox.Info(info, "制程变动项");
                    MessageBox.Success($"制程保存成功！\n主界面【重新选择】该制程后，改动才会生效！", "操作提示");
                }
            }
        }

        #endregion

        #region 打开辅助窗口
        /// <summary>
        /// 打开/关闭示教小窗口
        /// </summary>
        public void OpenTeachPanel()
        {
            if (RobotTeachNewViewModel == null)
                return;
            if (!RobotTeachNewViewModel.IsActive)
            {
                var settings = new Dictionary<string, object>
                {
                    { "ResizeMode", ResizeMode.NoResize },
                    { "WindowStartupLocation", WindowStartupLocation.Manual },
                    { "Left", 100 },
                    { "Top", 300 },
                };
                _windowManager.ShowWindow(RobotTeachNewViewModel, null, settings);
            }
            else
            {
                RobotTeachNewViewModel.TryClose();
            }
        }
        #endregion

        public void ClearLogs()
        {
            if (MessageBox.Show("确定清空运行日志吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                RunTimeLogs.ClearLogs();
            }
        }
    }
}
