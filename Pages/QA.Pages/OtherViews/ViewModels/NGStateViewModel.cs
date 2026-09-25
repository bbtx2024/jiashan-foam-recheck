using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Station;
using QA.Business.Steps;
using QA.Pages.Models;

namespace QA.Pages.OtherViews.ViewModels
{
    [Export("NGStateViewModel")]
    public class NGStateViewModel : Screen, IHandle<NGStateMessage>, IHandle<List<IComponent>>
    {
        #region Field

        private IWindowManager _windowManager;
        private IEventAggregator _eventAggregator;
        private TaskManager _taskManager = null;
        private IBaseBiz _baseBiz;
        private StepStatus _stepStatus;
        public const int layerCount = 7;//NG仓层数加1

        #endregion

        #region Property

        private string _carrierSn;
        public string CarrierSn
        {
            get { return _carrierSn; }
            set
            {
                _carrierSn = value;
                NotifyOfPropertyChange(() => CarrierSn);
            }
        }

        private int _carrCols = 3;      //载具列数
        public int CarrCols
        {
            get { return _carrCols; }
            set { _carrCols = value; NotifyOfPropertyChange(() => CarrCols); }
        }

        private int _carrRows = 4;      //载具行数
        public int CarrRows
        {
            get { return _carrRows; }
            set { _carrRows = value; NotifyOfPropertyChange(() => CarrRows); }
        }

        public ObservableCollection<TrayInfoModel>[] TrayInfos { get; set; } = new ObservableCollection<TrayInfoModel>[layerCount]; //穴位信息集合
        public string[] CarrierSns { get; set; } = new string[layerCount]; //载具SN集合

        private ObservableCollection<TrayInfoModel> _selectedTrayInfos = new ObservableCollection<TrayInfoModel>();
        public ObservableCollection<TrayInfoModel> SelectedTrayInfos
        {
            get { return _selectedTrayInfos; }
            set { _selectedTrayInfos = value; NotifyOfPropertyChange(() => SelectedTrayInfos); }
        }

        private int _selectedLater = layerCount - 1;      //当前选择层
        public int SelectedLater
        {
            get { return _selectedLater; }
            set
            {
                _selectedLater = value;
                NotifyOfPropertyChange(() => SelectedLater);
                NotifyOfPropertyChange(() => IsLayerSelected);
                SelectedTrayInfos = TrayInfos[SelectedLater];
            }
        }

        public bool[] IsLayerSelected
        {
            get
            {
                bool[] b = new bool[layerCount];
                if (SelectedLater >= 1 && SelectedLater < b.Length)
                {
                    b[SelectedLater] = true;
                }
                else
                {
                    b[1] = true;
                }
                return b;
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

        #endregion

        #region Constructor

        public NGStateViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _taskManager = IoC.Get<TaskManager>();
            _baseBiz = IoC.Get<IBaseBiz>();
            _stepStatus = IoC.Get<StepStatus>();
            int[] cavityOrder = new[] { 10, 11, 12, 7, 8, 9, 4, 5, 6, 1, 2, 3 };
            for (int layer = 0; layer < TrayInfos.Length; layer++)
            {
                TrayInfos[layer] = new ObservableCollection<TrayInfoModel>();
                for (int i = 0; i < cavityOrder.Length; i++)
                {
                    TrayInfos[layer].Add(new TrayInfoModel()
                    {
                        CavityNum = cavityOrder[i],
                    });
                }
            }
            SelectedTrayInfos = TrayInfos[SelectedLater];
        }

        #endregion

        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
        }

        public void Handle(NGStateMessage message)
        {
            SetState(message.Status, message.CarrierSns);
        }

        /// <summary>
        /// 修改主界面穴位显示状态
        /// </summary>
        /// <param name="cavity"></param>
        /// <param name="status"></param>
        public void SetState(EN_TrayStatus[,] status, string[] carrierSns)
        {
            for (int layer = 0; layer < TrayInfos.Length; layer++)
            {
                foreach (TrayInfoModel model in TrayInfos[layer])
                {
                    model.Status = status[layer, model.CavityNum - 1];
                }
            }
            SelectedTrayInfos = TrayInfos[SelectedLater];
            CarrierSns = carrierSns;
            CarrierSn = carrierSns[SelectedLater];
        }

        public void SelectLayer(string layer)
        {
            SelectedLater = int.Parse(layer);
            CarrierSn = CarrierSns[SelectedLater];
        }
    }
}
