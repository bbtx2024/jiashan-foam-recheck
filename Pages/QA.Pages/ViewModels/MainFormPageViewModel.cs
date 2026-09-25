using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Interfaces;
using QA.Business.Model.RunTimeInfo;
using QA.Business.Station;
using QA.Pages.Interfaces;
using QA.Pages.OtherViews.ViewModels;

namespace QA.Pages.ViewModels
{
    [Export(typeof(IPageViewModel))]
    public class MainFormPageViewModel : Screen, IPageViewModel, IHandle<List<IComponent>>
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private IBaseBiz _baseBiz;
        #endregion

        #region Property
        public ushort OrderID { get; set; } = 0;

        public RuntimeLogs RunTimeLogs { get; } = IoC.Get<RuntimeLogs>();

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

        #region UI
        [Import("CarrierInfoPanelViewModel")]
        public CarrierInfoPanelViewModel CarrierInfoPanelViewModel { get; set; }

        [Import("NGStateViewModel")]
        public NGStateViewModel NGStateViewModel { get; set; }

        //[Import("VisionViewModel")]
        //public VisionViewModel VisionViewModel { get; set; }

        [Import("HiveChartViewModel")]
        public HiveChartViewModel HiveChartViewModel { get; set; }

        [Import("RunStep1ViewModel")]
        public RunStep1ViewModel ShowRunStep1ViewModel { get; set; }

        [Import("RunStep2ViewModel")]
        public RunStep2ViewModel ShowRunStep2ViewModel { get; set; }

        [Import("StatisticViewModel")]
        public StatisticViewModel StatisticViewModel { get; set; }
        #endregion

        #region Constructor
        public MainFormPageViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _baseBiz = IoC.Get<IBaseBiz>();
            _baseBiz.Initial();
            _baseBiz.Start();
        }
        #endregion

        #region Handle
        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
        }
        #endregion

        #region Method
        public void ClearLogs()
        {
            if (MessageBox.Show("确定清空运行日志吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                RunTimeLogs.ClearLogs();
            }
        }

        public void ClearAlarmLogs()
        {
            if (MessageBox.Show("确定清空报警信息吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                RunTimeLogs.ClearAlarmLogs();
            }
        }
        #endregion
    }
}
