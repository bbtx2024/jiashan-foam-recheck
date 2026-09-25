using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Wpf;
using QA.Business;
using QA.Business.CacheParam;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Model;
using QA.Business.Station;
using QA.Business.Steps;
using QA.Pages.Interfaces;
using QA.Pages.Models;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.Pages.ViewModels
{
    [Export(typeof(IPageViewModel))]
    public class ProductivityPageViewModel : Screen, IPageViewModel, IHandle<List<IComponent>>
    {
        #region Field
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly GlobalVariable _globalVariable;
        private readonly ParamManager _paramManager;
        private readonly CacheParamManager _cacheParamManager;
        private readonly StepStatus _stepStatus;
        private IBaseBiz _baseBiz;
        #endregion

        #region Property

        public ushort OrderID { get; set; } = 4;

        private ObservableCollection<ComponentShowModel> _componentObservableCollection = new ObservableCollection<ComponentShowModel>();
        public ObservableCollection<ComponentShowModel> ComponentObservableCollection
        {
            get => _componentObservableCollection;
            set
            {
                _componentObservableCollection = value;
                NotifyOfPropertyChange(() => ComponentObservableCollection);
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

        public HomeUiParam_Statistic Statistic { get; private set; }  //长期统计信息，比如ok数目等等
        public HomeUiStatus_CT CT { get; private set; }  //临时统计信息，如CT等临时变量
        public ConnectState ConnectState { get; set; }  //连接状态
        #endregion

        #region Property2
        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection ToDayHistogramSeriesCollection { get; set; }
        public SeriesCollection ToDayPieSeriesCollection { get; set; }
        public SeriesCollection ChoiceTimeSlotHistogramSeriesCollection { get; set; }
        public SeriesCollection ChoiceTimeSlotPieSeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<int, string> Formatter { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public OutputPerHour outputPerHour { get; set; }
        public TossingPerHour bumperTossingPerHour { get; set; }
        public TossingPerHour GNDTossingPerHour { get; set; }
        public RecheckNGPerHour recheckNGPerHour { get; set; }

        private DateTime _choicetData = DateTime.Now.Date;
        public DateTime ChoicetData
        {
            get
            {
                return _choicetData;
            }
            set
            {
                _choicetData = value;
                NotifyOfPropertyChange(() => ChoicetData);
                QueryHistoryOneDay();
            }
        }

        private DateTime _startData = DateTime.Now.Date.AddDays(-6);
        public DateTime StartData
        {
            get
            {
                return _startData;
            }
            set
            {
                _startData = value;
                NotifyOfPropertyChange(() => StartData);
                QueryHistoryDays();
            }
        }

        private DateTime _endData = DateTime.Now.Date;
        public DateTime EndData
        {
            get
            {
                return _endData;
            }
            set
            {
                _endData = value;
                NotifyOfPropertyChange(() => EndData);
                QueryHistoryDays();
            }
        }

        private string[] _choiceTimeSlotLabels;
        public string[] ChoiceTimeSlotLabels
        {
            get
            {
                return _choiceTimeSlotLabels;
            }
            set
            {
                _choiceTimeSlotLabels = value;
                NotifyOfPropertyChange(() => ChoiceTimeSlotLabels);
            }
        }


        //bumper Tossing抛料界面数据集合，Label集合
        public SeriesCollection DayPieTossingSeriesCollection { get; set; }
        private string _dayPieTossingTB;
        public string DayPieTossingTB
        {
            get { return _dayPieTossingTB; }
            set
            {
                _dayPieTossingTB = value;
                NotifyOfPropertyChange(() => DayPieTossingTB);
            }
        }
        public SeriesCollection DayHistogramUPHSeriesCollection { get; set; }
        public SeriesCollection DayHistogramTossingSeriesCollection { get; set; }
        public SeriesCollection WeekHistogramTossingSeriesCollection { get; set; }
        public SeriesCollection NozzleHistogramTossingSeriesCollection { get; set; }
        public SeriesCollection TossingReasonHistogramSeriesCollection { get; set; }

        public string[] LabelsNozzleBumper { get; set; }
        public string[] LabelsTossReasonBumper { get; set; }

        private string[] _labelsWeekBumper;
        public string[] LabelsWeekBumper
        {
            get { return _labelsWeekBumper; }
            set
            {
                _labelsWeekBumper = value;
                NotifyOfPropertyChange(() => LabelsWeekBumper);
            }
        }

        private DateTime _tossingStartDataBumper = DateTime.Now.Date.AddDays(-6);
        public DateTime TossingStartDataBumper
        {
            get { return _tossingStartDataBumper; }
            set
            {
                _tossingStartDataBumper = value;
                NotifyOfPropertyChange(() => TossingStartDataBumper);
            }
        }

        private DateTime _tossingEndDataBumper = DateTime.Now.Date;
        public DateTime TossingEndDataBumper
        {
            get { return _tossingEndDataBumper; }
            set
            {
                _tossingEndDataBumper = value;
                NotifyOfPropertyChange(() => TossingEndDataBumper);
            }
        }



        //GND Tossing抛料界面数据集合，Label集合
        public SeriesCollection DayPieTossingSeriesCollection1 { get; set; }
        private string _dayPieTossingTB1;
        public string DayPieTossingTB1
        {
            get { return _dayPieTossingTB1; }
            set
            {
                _dayPieTossingTB1 = value;
                NotifyOfPropertyChange(() => DayPieTossingTB1);
            }
        }
        public SeriesCollection DayHistogramUPHSeriesCollection1 { get; set; }
        public SeriesCollection DayHistogramTossingSeriesCollection1 { get; set; }
        public SeriesCollection WeekHistogramTossingSeriesCollection1 { get; set; }
        public SeriesCollection NozzleHistogramTossingSeriesCollection1 { get; set; }
        public SeriesCollection TossingReasonHistogramSeriesCollection1 { get; set; }
        public string[] LabelsNozzleGND { get; set; }
        public string[] LabelsTossReasonGND { get; set; }

        private string[] _labelsWeekGND;
        public string[] LabelsWeekGND
        {
            get { return _labelsWeekGND; }
            set
            {
                _labelsWeekGND = value;
                NotifyOfPropertyChange(() => LabelsWeekGND);
            }
        }

        private DateTime _tossingStartDataGND = DateTime.Now.Date.AddDays(-6);
        public DateTime TossingStartDataGND
        {
            get { return _tossingStartDataGND; }
            set
            {
                _tossingStartDataGND = value;
                NotifyOfPropertyChange(() => TossingStartDataGND);
            }
        }

        private DateTime _tossingEndDataGND = DateTime.Now.Date;
        public DateTime TossingEndDataGND
        {
            get { return _tossingEndDataGND; }
            set
            {
                _tossingEndDataGND = value;
                NotifyOfPropertyChange(() => TossingEndDataGND);
            }
        }



        //复检界面数据集合
        public SeriesCollection DayPieReckeckSeriesCollection { get; set; }
        private string _dayPieReckeckTB;
        public string DayPieReckeckTB
        {
            get { return _dayPieReckeckTB; }
            set
            {
                _dayPieReckeckTB = value;
                NotifyOfPropertyChange(() => DayPieReckeckTB);
            }
        }
        public SeriesCollection DayHistogramUPHSeriesCollection2 { get; set; }
        public SeriesCollection DayHistogramTossingSeriesCollection2 { get; set; }
        public SeriesCollection WeekReckeckNGHistogramSeriesCollection { get; set; }
        public SeriesCollection ReckeckNGReasonHistogramSeriesCollection { get; set; }
        public SeriesCollection NozzleHistogramReckeckSeriesCollection { get; set; }
        public SeriesCollection CavityHistogramReckeckSeriesCollection { get; set; }
        public string[] LabelsCavity { get; set; }
        public string[] LabelsReacheckReason { get; set; } = new[] {
            "MesNG", "Alert缺失", "GND缺失", "GNDLXZ缺失", "Alert未撕膜", "GND未撕膜", "Alert偏移", "GND偏移", "TP缺失"
        };
        private string[] _labelsWeekRecheck;
        public string[] LabelsWeekRecheck
        {
            get { return _labelsWeekRecheck; }
            set
            {
                _labelsWeekRecheck = value;
                NotifyOfPropertyChange(() => LabelsWeekRecheck);
            }
        }

        private string[] _labelsNozzle;
        public string[] LabelsNozzle
        {
            get { return _labelsNozzle; }
            set
            {
                _labelsNozzle = value;
                NotifyOfPropertyChange(() => LabelsNozzle);
            }
        }

        private DateTime _recheckNGStartData = DateTime.Now.Date.AddDays(-6);
        public DateTime RecheckNGStartData
        {
            get { return _recheckNGStartData; }
            set
            {
                _recheckNGStartData = value;
                NotifyOfPropertyChange(() => RecheckNGStartData);
            }
        }

        private DateTime _recheckNGEndData = DateTime.Now.Date;
        public DateTime RecheckNGEndData
        {
            get { return _recheckNGEndData; }
            set
            {
                _recheckNGEndData = value;
                NotifyOfPropertyChange(() => RecheckNGEndData);
            }
        }

        public bool _currentShowUPH = true;
        public bool CurrentShowUPH
        {
            get => _currentShowUPH;
            set
            {
                _currentShowUPH = value;
                NotifyOfPropertyChange(() => CurrentShowUPH);
                NotifyOfPropertyChange(() => UPHTossingButtonStr);
                NotifyOfPropertyChange(() => ShowUPH);
                NotifyOfPropertyChange(() => ShowTossing);
            }
        }
        public string UPHTossingButtonStr => _currentShowUPH ? "切换至Tossing" : "切换至UPH";
        public bool ShowUPH => _currentShowUPH;
        public bool ShowTossing => !_currentShowUPH;

        public bool _currentShowMes = true;
        public bool CurrentShowMes
        {
            get => _currentShowMes;
            set
            {
                _currentShowMes = value;
                NotifyOfPropertyChange(() => CurrentShowMes);
                NotifyOfPropertyChange(() => UseMesButtonStr);
            }
        }
        public string UseMesButtonStr => _currentShowMes ? "切换至调试" : "切换至生产";
        #endregion

        public ProductivityPageViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);

            _globalVariable = IoC.Get<GlobalVariable>();
            _paramManager = IoC.Get<ParamManager>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _baseBiz = IoC.Get<IBaseBiz>();
            _stepStatus = IoC.Get<StepStatus>();

            Statistic = _cacheParamManager.HomeUiParam.Statistic;
            ConnectState = _globalVariable.ConnectState;
            CT = _globalVariable.CT;

            outputPerHour = _stepStatus.OutputPerHour;
            bumperTossingPerHour = _stepStatus.BumperTossingPerHour;
            GNDTossingPerHour = _stepStatus.GNDTossingPerHour;
            recheckNGPerHour = _stepStatus.ReacheckNGPerHour;

            Initialization();
            QueryHistoryOneDay();
            QueryHistoryDays();
            InitialBumperTossingChart();
            InitialGNDTossingChart();
            InitialRecheckNGChart();
            QueryBumperTossingCount();
            QueryGNDTossingCount();
            QueryRecheckNGCount();
        }

        #region Caliburn.Micro
        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
            foreach (var item in message)
            {
                if (item is IPLC)
                {
                    if (_paramManager.PLCParam.BUse)
                    {
                        _globalVariable.ConnectState.IsPlcConnected = (item as IPLC).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                    }
                    else
                    {
                        _globalVariable.ConnectState.IsPlcConnected = En_DeviceStatus.Disabled;
                    }
                }
                //else if (item is IMES)
                //{
                //    if (_paramManager.MESParam.BUse)
                //    {
                //        _globalVariable.ConnectState.IsMesConnected = (item as IMES).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                //    }
                //    else
                //    {
                //        _globalVariable.ConnectState.IsMesConnected = En_DeviceStatus.Disabled;
                //    }
                //}
                else if (item is IScanner)
                {
                    if (_paramManager.ScannerParam.BUse)
                    {
                        _globalVariable.ConnectState.IsScannerConnected = (item as IScanner).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                    }
                    else
                    {
                        _globalVariable.ConnectState.IsScannerConnected = En_DeviceStatus.Disabled;
                    }
                }
                else if (item is IScannerRechck)
                {
                    if (_paramManager.ScannerParamRecheck.BUse)
                    {
                        _globalVariable.ConnectState.IsScannerConnectedRecheck = (item as IScannerRechck).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                    }
                    else
                    {
                        _globalVariable.ConnectState.IsScannerConnectedRecheck = En_DeviceStatus.Disabled;
                    }
                }
                //else if (item is ILaserHeightSensor)
                //{
                //    if (_paramManager.LaserHeightSensorParam.BUse)
                //    {
                //        _globalVariable.ConnectState.IsLaserHeightSensorConnected = (item as ILaserHeightSensor).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                //    }
                //    else
                //    {
                //        _globalVariable.ConnectState.IsLaserHeightSensorConnected = En_DeviceStatus.Disabled;
                //    }
                //}
                else if (item is ICamera)
                {
                    if (_paramManager.CameraParam.BUse)
                    {
                        _globalVariable.ConnectState.IsVisionConnected = (item as ICamera).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                    }
                    else
                    {
                        _globalVariable.ConnectState.IsVisionConnected = En_DeviceStatus.Disabled;
                    }
                }
                //else if (item is IMGoogol)
                //{
                //    if (_paramManager.MotionGoogolParam.BUse)
                //    {
                //        _globalVariable.ConnectState.IsMotionConnected = (item as IMGoogol).IsConnected ? En_DeviceStatus.Connected : En_DeviceStatus.DisConnected;
                //    }
                //    else
                //    {
                //        _globalVariable.ConnectState.IsMotionConnected = En_DeviceStatus.Disabled;
                //    }
                //}
            }
        }
        #endregion

        #region Override
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
        #endregion

        #region Method

        public void ResetCurrentNum()
        {
            if (MessageBox.Show("确定重置当前产量吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Statistic.CurrentNum = 0;
                _cacheParamManager.SaveHomeUiParam();
            }
        }

        public void ResetCount()
        {
            if (MessageBox.Show("确定清零数据吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Statistic.ResetCount();
                _cacheParamManager.SaveHomeUiParam();
            }
        }

        public void Initialization()
        {
            BrushConverter brushConverter = new BrushConverter();
            ToDayHistogramSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    // Fill = System.Windows.Media.Brushes.Green,Fill="#008D86"
                    Fill = (Brush)brushConverter.ConvertFromString("#008D86"),
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = System.Windows.Media.Brushes.Red,Fill="#FF6A6A"
                    // Fill = (Brush)brushConverter.ConvertFromString("#FF6A6A"),
                    Fill = Brushes.Red,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };

            ToDayPieSeriesCollection = new SeriesCollection
            {
                new PieSeries()
                {
                    Title = "OK产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    Fill = (Brush)brushConverter.ConvertFromString("#008D86"),
                    LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    {
                        return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    })
                },
                new PieSeries()
                {
                    Title = "NG产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    Fill = (Brush)brushConverter.ConvertFromString("#FF6A6A"),
                    LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    {
                        return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    })
                }
            };

            ChoiceTimeSlotHistogramSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int>(),
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                   // Fill=System.Windows.Media.Brushes.Green,Fill="#008D86"
                    Fill =(Brush)brushConverter.ConvertFromString("#008D86"),
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int>(),
                    StackMode = StackMode.Values,
                   // Fill=System.Windows.Media.Brushes.Red,Fill="#FF6A6A"
                    // Fill = (Brush)brushConverter.ConvertFromString("#FF6A6A"),
                    Fill = Brushes.Red,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };

            ChoiceTimeSlotPieSeriesCollection = new SeriesCollection
            {
                new PieSeries()
                {
                    Title = "OK产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    Fill = (Brush)brushConverter.ConvertFromString("#008D86"),
                    LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    {
                        return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    })
                },
                new PieSeries()
                {
                    Title = "NG产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    // Fill = (Brush)brushConverter.ConvertFromString("#FF6A6A"),
                    Fill = Brushes.Red,
                    LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    {
                        return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    })
                }
            };
            Labels = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
            Formatter = value => value.ToString("N");
            PointLabel = chartPoint =>
            string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
        }

        public void QueryHistoryOneDay()
        {
            if (ChoicetData.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                for (int i = 0; i < 24; i++)
                {
                    ToDayHistogramSeriesCollection[0].Values[i] = _stepStatus.OutputPerHour.ProductionQuantitys[i].OkCount;
                    ToDayHistogramSeriesCollection[1].Values[i] = _stepStatus.OutputPerHour.ProductionQuantitys[i].NgCount;
                }
                ToDayPieSeriesCollection[0].Values[0] = _stepStatus.OutputPerHour.OkCount;
                ToDayPieSeriesCollection[1].Values[0] = _stepStatus.OutputPerHour.NgCount;
            }
            else
            {
                if (outputPerHour.ReadOutputPerHour(_choicetData))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        ToDayHistogramSeriesCollection[0].Values[i] = outputPerHour.ProductionQuantitys[i].OkCount;
                        ToDayHistogramSeriesCollection[1].Values[i] = outputPerHour.ProductionQuantitys[i].NgCount;
                    }
                    ToDayPieSeriesCollection[0].Values[0] = outputPerHour.OkCount;
                    ToDayPieSeriesCollection[1].Values[0] = outputPerHour.NgCount;
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        ToDayHistogramSeriesCollection[0].Values[i] = 0;
                        ToDayHistogramSeriesCollection[1].Values[i] = 0;
                    }
                    ToDayPieSeriesCollection[0].Values[0] = 0;
                    ToDayPieSeriesCollection[1].Values[0] = 0;
                }

            }
        }

        public void QueryHistoryDays()
        {
            TimeSpan timeSpan = EndData - StartData;
            if (timeSpan.TotalMilliseconds < 0)
            {
                return;
            }
            ChoiceTimeSlotLabels = new string[timeSpan.Days + 1];
            if (timeSpan.Days + 1 > 90)
            {
                return;
            }
            ChoiceTimeSlotPieSeriesCollection[0].Values[0] = 0;
            ChoiceTimeSlotPieSeriesCollection[1].Values[0] = 0;
            ChoiceTimeSlotHistogramSeriesCollection[0].Values.Clear();
            ChoiceTimeSlotHistogramSeriesCollection[1].Values.Clear();
            int allOKCount = 0;
            int allNGCount = 0;
            for (int i = 0; i < timeSpan.Days + 1; i++)
            {
                ChoiceTimeSlotLabels[i] = StartData.AddDays(i).ToString("MM-dd");
                ChoiceTimeSlotHistogramSeriesCollection[0].Values.Add(0);
                ChoiceTimeSlotHistogramSeriesCollection[1].Values.Add(0);
                if (outputPerHour.ReadOutputPerHour(StartData.AddDays(i)))
                {
                    ChoiceTimeSlotHistogramSeriesCollection[0].Values[i] = outputPerHour.OkCount;
                    ChoiceTimeSlotHistogramSeriesCollection[1].Values[i] = outputPerHour.NgCount;
                    allOKCount += outputPerHour.OkCount;
                    allNGCount += outputPerHour.NgCount;
                }
                else
                {
                    ChoiceTimeSlotHistogramSeriesCollection[0].Values[i] = 0;
                    ChoiceTimeSlotHistogramSeriesCollection[1].Values[i] = 0;
                }
            }
            ChoiceTimeSlotPieSeriesCollection[0].Values[0] = allOKCount;
            ChoiceTimeSlotPieSeriesCollection[1].Values[0] = allNGCount;
        }

        public void OpenFolder()
        {
            System.Diagnostics.Process.Start(_stepStatus.OutputPerHour.BasePath);
        }


        BrushConverter brushConverter = new BrushConverter();
        /// <summary>
        /// 初始化Bumper抛料统计界面
        /// </summary>
        private void InitialBumperTossingChart()
        {
            //bumper Tossing Rate数据
            DayPieTossingSeriesCollection = new SeriesCollection
            {
                new PieSeries()
                {
                    Title = "OK产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    FontSize = 24,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                },
                new PieSeries()
                {
                    Title = "NG产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    FontSize = 24,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                }
            };
            //bumper UPH Tossing By Hour数据
            DayHistogramUPHSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            DayHistogramTossingSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "良率",
                    Values = new ChartValues<double> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    PointGeometry = DefaultGeometries.None,//None表示不显示点
                    DataLabels = true,//启用数据标签
                    LabelPoint = point => (point.Y / 100.0).ToString("0.##%"),//标签内容
                    FontSize = 14,//标签字体大小
                    Foreground = new SolidColorBrush(Colors.White),//标签字体颜色
                    //LineSmoothness = 0.5,//折线弯曲程度（0最直-1最弯曲，默认值1）
                    Stroke = new SolidColorBrush(Colors.LightGreen),//折线颜色绿
                    Fill = new SolidColorBrush(Colors.Transparent),//折线下面填充颜色透明
                }
            };
            //bumper Tossing By Day数据
            WeekHistogramTossingSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //bumper Tossing By Nozzle数据
            NozzleHistogramTossingSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //bumper Tossing NG Summary数据
            TossingReasonHistogramSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };

            LabelsWeekBumper = new string[7];
            for (int i = 0; i < 7; i++)
            {
                LabelsWeekBumper[i] = DateTime.Now.AddDays(-6).AddDays(i).ToString("MMdd");
            }

            LabelsNozzleBumper = new[] { "Bumper左吸嘴", "Bumper右吸嘴" };
            LabelsTossReasonBumper = new[] { "吸取失败", "抓边失败" };
        }

        /// <summary>
        /// 初始化GND抛料统计界面
        /// </summary>
        private void InitialGNDTossingChart()
        {
            //GND Tossing Rate数据
            DayPieTossingSeriesCollection1 = new SeriesCollection
            {
                new PieSeries()
                {
                    Title = "OK产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    FontSize = 24,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                },
                new PieSeries()
                {
                    Title = "NG产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    FontSize = 24,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                }
            };
            //GND UPH Tossing By Hour数据
            DayHistogramUPHSeriesCollection1 = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            DayHistogramTossingSeriesCollection1 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "良率",
                    Values = new ChartValues<double> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    PointGeometry = DefaultGeometries.None,//None表示不显示点
                    DataLabels = true,//启用数据标签
                    LabelPoint = point => (point.Y / 100.0).ToString("0.##%"),//标签内容
                    FontSize = 14,//标签字体大小
                    Foreground = new SolidColorBrush(Colors.White),//标签字体颜色
                    //LineSmoothness = 0.5,//折线弯曲程度（0最直-1最弯曲，默认值1）
                    Stroke = new SolidColorBrush(Colors.LightGreen),//折线颜色绿
                    Fill = new SolidColorBrush(Colors.Transparent),//折线下面填充颜色透明
                }
            };
            //GND Tossing By Day数据
            WeekHistogramTossingSeriesCollection1 = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //GND Tossing By Nozzle数据
            NozzleHistogramTossingSeriesCollection1 = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0,0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //GND Tossing NG Summary数据
            TossingReasonHistogramSeriesCollection1 = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };

            LabelsWeekGND = new string[7];
            for (int i = 0; i < 7; i++)
            {
                LabelsWeekGND[i] = DateTime.Now.AddDays(-6).AddDays(i).ToString("MMdd");
            }

            LabelsNozzleGND = new[] { "GND左吸嘴", "GND右吸嘴" };
            LabelsTossReasonGND = new[] { "吸取失败", "抓边失败" };
        }

        /// <summary>
        /// 初始化复检NG统计界面
        /// </summary>
        private void InitialRecheckNGChart()
        {
            //RecheckNG By Hour数据
            DayHistogramUPHSeriesCollection2 = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    DataLabels = true,
                    FontSize = 14,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                    //ScalesYAt=0,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    DataLabels = true,
                    FontSize = 14,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                    //ScalesYAt=0,
                }
            };
            DayHistogramTossingSeriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "良率",
                    Values = new ChartValues<double> {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                    PointGeometry = DefaultGeometries.None,//None表示不显示点
                    DataLabels = true,//启用数据标签
                    LabelPoint = point => (point.Y / 100.0).ToString("0.##%"),//标签内容
                    FontSize = 14,//标签字体大小
                    Foreground = new SolidColorBrush(Colors.White),//标签字体颜色
                    //LineSmoothness = 0.5,//折线弯曲程度（0最直-1最弯曲，默认值1）
                    Stroke = new SolidColorBrush(Colors.LightGreen),//折线颜色绿
                    Fill = new SolidColorBrush(Colors.Transparent),//折线下面填充颜色透明
                }
            };
            //Recheck Rate数据
            DayPieReckeckSeriesCollection = new SeriesCollection
            {
                new PieSeries()
                {
                    Title = "OK产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 24,
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                },
                new PieSeries()
                {
                    Title = "NG产量",
                    Values = new ChartValues<int>() { 0 },
                    DataLabels = true,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 24,
                    //LabelPoint = new Func<ChartPoint, string>((chartPoint) =>
                    //{
                    //    return string.Format("{0}{1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);
                    //})
                }
            };
            //Recheck NG By Day数据
            WeekReckeckNGHistogramSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="OK产量",
                    Values = new ChartValues<int> {},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                },
                new StackedColumnSeries
                {
                    Title="NG产量",
                    Values = new ChartValues<int> {},
                    StackMode = StackMode.Values,
                    // Fill = (Brush)brushConverter.ConvertFromString("#6c81b4"),
                    Fill = Brushes.Red,
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //Recheck NG Summary数据
            ReckeckNGReasonHistogramSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //Recheck By Nozzle数据
            NozzleHistogramReckeckSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0,0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };
            //Recheck By Nozzle数据
            CavityHistogramReckeckSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title="NG数量",
                    Values = new ChartValues<int> {0,0,0,0,0,0,0,0,0,0,0,0},
                    StackMode = StackMode.Values,
                    Fill =(Brush)brushConverter.ConvertFromString("#77c7ff"),
                    FontSize = 14,
                    DataLabels = true,
                    LabelsPosition = BarLabelPosition.Perpendicular,
                }
            };

            LabelsWeekRecheck = new string[7];
            for (int i = 0; i < 7; i++)
            {
                LabelsWeekRecheck[i] = DateTime.Now.AddDays(-6).AddDays(i).ToString("MMdd");
            }
            LabelsCavity = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            LabelsNozzle = new[] { "Bumper左吸嘴", "Bumper右吸嘴", "GND左吸嘴", "GND右吸嘴" };
        }

        /// <summary>
        /// 查询Bumper抛料数据
        /// </summary>
        public void QueryBumperTossingCount()
        {
            try
            {
                //显示一天的抛料统计
                DayPieTossingSeriesCollection[0].Values[0] = 0;
                DayPieTossingSeriesCollection[1].Values[0] = 0;
                DayHistogramUPHSeriesCollection[0].Values.Clear();
                DayHistogramUPHSeriesCollection[1].Values.Clear();
                DayHistogramTossingSeriesCollection[0].Values.Clear();
                int allOKCount = 0;
                int allNGCount = 0;
                if (bumperTossingPerHour.ReadOutputPerHour(TossingEndDataBumper, 0, CurrentShowMes))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection[0].Values[i] = bumperTossingPerHour.DayTossingQuantitys[i].OkCount;
                        DayHistogramUPHSeriesCollection[1].Values[i] = bumperTossingPerHour.DayTossingQuantitys[i].NgCount;
                        int ok = bumperTossingPerHour.DayTossingQuantitys[i].OkCount;
                        int ng = bumperTossingPerHour.DayTossingQuantitys[i].NgCount;
                        if (ok + ng == 0)
                        {
                            DayHistogramTossingSeriesCollection[0].Values[i] = 0.0;
                        }
                        else
                        {
                            DayHistogramTossingSeriesCollection[0].Values[i] = (double)ok / (ng + ok) * 100.0;
                        }
                        allOKCount += bumperTossingPerHour.DayTossingQuantitys[i].OkCount;
                        allNGCount += bumperTossingPerHour.DayTossingQuantitys[i].NgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection[0].Values[i] = 0;
                        DayHistogramUPHSeriesCollection[1].Values[i] = 0;
                        DayHistogramTossingSeriesCollection[0].Values[i] = 0.0;
                    }
                }
                DayPieTossingSeriesCollection[0].Values[0] = allOKCount;
                DayPieTossingSeriesCollection[1].Values[0] = allNGCount;
                //Bumper/GND使用抛料率，复检使用良率
                double ngRate = allOKCount + allNGCount == 0 ? 0.0 : ((double)allNGCount / (allOKCount + allNGCount));
                DayPieTossingTB = $"OK: {allOKCount}    NG: {allNGCount}    抛料率: {ngRate.ToString("0.##%")}";

                //如果查询日期大于7天，即查询结束日期往前7天
                if ((TossingEndDataBumper - TossingStartDataBumper).TotalDays >= 7)
                {
                    TossingStartDataBumper = TossingEndDataBumper.AddDays(-6);
                }
                //显示抛料统计
                WeekHistogramTossingSeriesCollection[0].Values.Clear();
                WeekHistogramTossingSeriesCollection[1].Values.Clear();
                TimeSpan timeSpan = TossingEndDataBumper - TossingStartDataBumper;
                if (timeSpan.TotalMilliseconds < 0)
                {
                    return;
                }
                LabelsWeekBumper = new string[timeSpan.Days + 1];
                for (int i = 0; i < timeSpan.Days + 1; i++)
                {
                    LabelsWeekBumper[i] = null;
                    LabelsWeekBumper[i] = TossingStartDataBumper.AddDays(i).ToString("MMdd");
                    WeekHistogramTossingSeriesCollection[0].Values.Add(0);
                    WeekHistogramTossingSeriesCollection[1].Values.Add(0);
                    if (bumperTossingPerHour.ReadOutputPerHour(TossingStartDataBumper.AddDays(i), 0, CurrentShowMes))
                    {
                        WeekHistogramTossingSeriesCollection[0].Values[i] = bumperTossingPerHour.OkCount;
                        WeekHistogramTossingSeriesCollection[1].Values[i] = bumperTossingPerHour.NgCount;
                    }
                    else
                    {
                        WeekHistogramTossingSeriesCollection[0].Values[i] = 0;
                        WeekHistogramTossingSeriesCollection[1].Values[i] = 0;
                    }
                }

                //显示吸嘴一天的抛料统计
                NozzleHistogramTossingSeriesCollection[0].Values.Clear();
                if (bumperTossingPerHour.ReadTossingPerHour(TossingEndDataBumper, 0, CurrentShowMes))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NozzleHistogramTossingSeriesCollection[0].Values.Add(0);
                        NozzleHistogramTossingSeriesCollection[0].Values[i] = bumperTossingPerHour.NozzleTossingQuantitys[i * 2].NozzleNgCount + bumperTossingPerHour.NozzleTossingQuantitys[i * 2 + 1].NozzleNgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NozzleHistogramTossingSeriesCollection[0].Values.Add(0);
                        NozzleHistogramTossingSeriesCollection[0].Values[i] = 0;
                    }
                }

                //显示一天抛料NG的原因
                TossingReasonHistogramSeriesCollection[0].Values.Clear();
                for (int i = 0; i < 2; i++)
                {
                    TossingReasonHistogramSeriesCollection[0].Values.Add(0);
                    TossingReasonHistogramSeriesCollection[0].Values[i] = bumperTossingPerHour.ReasonTossingQuantitys[i].ReasonNgCount;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 查询GND抛料数据
        /// </summary>
        public void QueryGNDTossingCount()
        {
            try
            {
                //显示一天的抛料统计
                DayPieTossingSeriesCollection1[0].Values[0] = 0;
                DayPieTossingSeriesCollection1[1].Values[0] = 0;
                DayHistogramUPHSeriesCollection1[0].Values.Clear();
                DayHistogramUPHSeriesCollection1[1].Values.Clear();
                DayHistogramTossingSeriesCollection1[0].Values.Clear();
                int allOKCount = 0;
                int allNGCount = 0;
                if (GNDTossingPerHour.ReadOutputPerHour(TossingEndDataGND, 1, CurrentShowMes))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection1[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection1[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection1[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection1[0].Values[i] = GNDTossingPerHour.DayTossingQuantitys[i].OkCount;
                        DayHistogramUPHSeriesCollection1[1].Values[i] = GNDTossingPerHour.DayTossingQuantitys[i].NgCount;
                        int ok = GNDTossingPerHour.DayTossingQuantitys[i].OkCount;
                        int ng = GNDTossingPerHour.DayTossingQuantitys[i].NgCount;
                        if (ok + ng == 0)
                        {
                            DayHistogramTossingSeriesCollection1[0].Values[i] = 0.0;
                        }
                        else
                        {
                            DayHistogramTossingSeriesCollection1[0].Values[i] = (double)ok / (ng + ok) * 100.0;
                        }
                        allOKCount += GNDTossingPerHour.DayTossingQuantitys[i].OkCount;
                        allNGCount += GNDTossingPerHour.DayTossingQuantitys[i].NgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection1[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection1[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection1[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection1[0].Values[i] = 0;
                        DayHistogramUPHSeriesCollection1[1].Values[i] = 0;
                        DayHistogramTossingSeriesCollection1[0].Values[i] = 0.0;
                    }
                }
                DayPieTossingSeriesCollection1[0].Values[0] = allOKCount;
                DayPieTossingSeriesCollection1[1].Values[0] = allNGCount;
                //Bumper/GND使用抛料率，复检使用良率
                double ngRate = allOKCount + allNGCount == 0 ? 0.0 : ((double)allNGCount / (allOKCount + allNGCount));
                DayPieTossingTB1 = $"OK: {allOKCount}    NG: {allNGCount}    抛料率: {ngRate.ToString("0.##%")}";

                //如果查询日期大于7天，即查询结束日期往前7天
                if ((TossingEndDataGND - TossingStartDataGND).TotalDays >= 7)
                {
                    TossingStartDataGND = TossingEndDataGND.AddDays(-6);
                }
                //显示抛料统计
                WeekHistogramTossingSeriesCollection1[0].Values.Clear();
                WeekHistogramTossingSeriesCollection1[1].Values.Clear();
                TimeSpan timeSpan1 = TossingEndDataGND - TossingStartDataGND;
                if (timeSpan1.TotalMilliseconds < 0)
                {
                    return;
                }
                LabelsWeekGND = new string[timeSpan1.Days + 1];
                for (int i = 0; i < timeSpan1.Days + 1; i++)
                {
                    LabelsWeekGND[i] = null;
                    LabelsWeekGND[i] = TossingStartDataGND.AddDays(i).ToString("MMdd");
                    WeekHistogramTossingSeriesCollection1[0].Values.Add(0);
                    WeekHistogramTossingSeriesCollection1[1].Values.Add(0);
                    if (GNDTossingPerHour.ReadOutputPerHour(TossingStartDataGND.AddDays(i), 1, CurrentShowMes))
                    {
                        WeekHistogramTossingSeriesCollection1[0].Values[i] = GNDTossingPerHour.OkCount;
                        WeekHistogramTossingSeriesCollection1[1].Values[i] = GNDTossingPerHour.NgCount;
                    }
                    else
                    {
                        WeekHistogramTossingSeriesCollection1[0].Values[i] = 0;
                        WeekHistogramTossingSeriesCollection1[1].Values[i] = 0;
                    }
                }

                //显示吸嘴一天的抛料统计
                NozzleHistogramTossingSeriesCollection1[0].Values.Clear();
                if (GNDTossingPerHour.ReadTossingPerHour(TossingEndDataGND, 1, CurrentShowMes))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NozzleHistogramTossingSeriesCollection1[0].Values.Add(0);
                        NozzleHistogramTossingSeriesCollection1[0].Values[i] = GNDTossingPerHour.NozzleTossingQuantitys[i * 2].NozzleNgCount + GNDTossingPerHour.NozzleTossingQuantitys[i * 2 + 1].NozzleNgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NozzleHistogramTossingSeriesCollection1[0].Values.Add(0);
                        NozzleHistogramTossingSeriesCollection1[0].Values[i] = 0;
                    }
                }
                //显示一天抛料NG的原因
                TossingReasonHistogramSeriesCollection1[0].Values.Clear();
                for (int i = 0; i < 2; i++)
                {
                    TossingReasonHistogramSeriesCollection1[0].Values.Add(0);
                    TossingReasonHistogramSeriesCollection1[0].Values[i] = GNDTossingPerHour.ReasonTossingQuantitys[0].ReasonNgCount;
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 查询复检NG数据
        /// </summary>
        public void QueryRecheckNGCount()
        {
            try
            {
                //显示一天的抛料统计
                DayPieReckeckSeriesCollection[0].Values[0] = 0;
                DayPieReckeckSeriesCollection[1].Values[0] = 0;
                DayHistogramUPHSeriesCollection2[0].Values.Clear();
                DayHistogramUPHSeriesCollection2[1].Values.Clear();
                DayHistogramTossingSeriesCollection2[0].Values.Clear();
                int allOKCount = 0;
                int allNGCount = 0;
                if (recheckNGPerHour.ReadOutputPerHour(RecheckNGEndData, CurrentShowMes))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection2[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection2[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection2[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection2[0].Values[i] = recheckNGPerHour.DayRecheckNGQuantitys[i].OkCount;
                        DayHistogramUPHSeriesCollection2[1].Values[i] = recheckNGPerHour.DayRecheckNGQuantitys[i].NgCount;
                        int ok = recheckNGPerHour.DayRecheckNGQuantitys[i].OkCount;
                        int ng = recheckNGPerHour.DayRecheckNGQuantitys[i].NgCount;
                        if (ok + ng == 0)
                        {
                            DayHistogramTossingSeriesCollection2[0].Values[i] = 0.0;
                        }
                        else
                        {
                            DayHistogramTossingSeriesCollection2[0].Values[i] = (double)ok / (ng + ok) * 100.0;
                        }
                        allOKCount += recheckNGPerHour.DayRecheckNGQuantitys[i].OkCount;
                        allNGCount += recheckNGPerHour.DayRecheckNGQuantitys[i].NgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DayHistogramUPHSeriesCollection2[0].Values.Add(0);
                        DayHistogramUPHSeriesCollection2[1].Values.Add(0);
                        DayHistogramTossingSeriesCollection2[0].Values.Add(0.0);
                        DayHistogramUPHSeriesCollection2[0].Values[i] = 0;
                        DayHistogramUPHSeriesCollection2[1].Values[i] = 0;
                        DayHistogramTossingSeriesCollection2[0].Values[i] = 0.0;
                    }
                }
                DayPieReckeckSeriesCollection[0].Values[0] = allOKCount;
                DayPieReckeckSeriesCollection[1].Values[0] = allNGCount;
                //Bumper/GND使用抛料率，复检使用良率
                double okRate = allOKCount + allNGCount == 0 ? 0.0 : ((double)allOKCount / (allOKCount + allNGCount));
                DayPieReckeckTB = $"OK: {allOKCount}    NG: {allNGCount}    良率: {okRate.ToString("0.##%")}";

                //如果查询日期大于7天，即查询结束日期往前7天
                if ((RecheckNGEndData - RecheckNGStartData).TotalDays >= 7)
                {
                    RecheckNGStartData = RecheckNGEndData.AddDays(-6);
                }
                //显示抛料统计
                WeekReckeckNGHistogramSeriesCollection[0].Values.Clear();
                WeekReckeckNGHistogramSeriesCollection[1].Values.Clear();
                TimeSpan timeSpan = RecheckNGEndData - RecheckNGStartData;
                if (timeSpan.TotalMilliseconds < 0)
                {
                    return;
                }
                LabelsWeekRecheck = new string[timeSpan.Days + 1];
                for (int i = 0; i < timeSpan.Days + 1; i++)
                {
                    LabelsWeekRecheck[i] = null;
                    LabelsWeekRecheck[i] = RecheckNGStartData.AddDays(i).ToString("MMdd");
                    WeekReckeckNGHistogramSeriesCollection[0].Values.Add(0);
                    WeekReckeckNGHistogramSeriesCollection[1].Values.Add(0);
                    if (recheckNGPerHour.ReadOutputPerHour(RecheckNGStartData.AddDays(i), CurrentShowMes))
                    {
                        WeekReckeckNGHistogramSeriesCollection[0].Values[i] = recheckNGPerHour.OkCount;
                        WeekReckeckNGHistogramSeriesCollection[1].Values[i] = recheckNGPerHour.NgCount;
                    }
                    else
                    {
                        WeekReckeckNGHistogramSeriesCollection[0].Values[i] = 0;
                        WeekReckeckNGHistogramSeriesCollection[1].Values[i] = 0;
                    }
                }

                //显示一天中的不同吸嘴抛料数据
                NozzleHistogramReckeckSeriesCollection[0].Values.Clear();
                if (recheckNGPerHour.ReadRecheckNGPerHour(RecheckNGEndData, CurrentShowMes))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        NozzleHistogramReckeckSeriesCollection[0].Values.Add(0);
                        NozzleHistogramReckeckSeriesCollection[0].Values[i] = recheckNGPerHour.NozzleRecheckNGQuantitys[i].NozzleNgCount;
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        NozzleHistogramReckeckSeriesCollection[0].Values.Add(0);
                        NozzleHistogramReckeckSeriesCollection[0].Values[i] = 0;
                    }
                }

                //显示一天的不同原因复检NG数据
                ReckeckNGReasonHistogramSeriesCollection[0].Values.Clear();
                for (int i = 0; i < Enum.GetNames(typeof(EN_Recheck)).Length; i++)
                {
                    ReckeckNGReasonHistogramSeriesCollection[0].Values.Add(0);
                    ReckeckNGReasonHistogramSeriesCollection[0].Values[i] = recheckNGPerHour.ReasonRecheckNGQuantitys[i].ReasonNgCount;
                }

                //显示一天中不同穴位复检NG数据
                CavityHistogramReckeckSeriesCollection[0].Values.Clear();
                for (int i = 0; i < 12; i++)
                {
                    CavityHistogramReckeckSeriesCollection[0].Values.Add(0);
                    CavityHistogramReckeckSeriesCollection[0].Values[i] = recheckNGPerHour.CavityRecheckNGQuantitys[i].CavityNgCount;
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 打开数据表格
        /// </summary>
        public void OpenRecheckData()
        {
            try
            {
                string dir = @"D:\QKProject\RunInfo\DayRechechPerHour";
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
            //CarrierStatus carrierStatus = new CarrierStatus();

            //for (int i = 0; i < 12; i++)
            //{
            //    carrierStatus.Alert_Bumper_SN[i] = "FWV4276946-34415-0A004-R069C4_" + i.ToString();
            //    carrierStatus.Flex_GND_Tape_SN[i] = "FPY4282946-33504-0A001-R5524D_" + i.ToString();
            //    carrierStatus.carrierSN = "PC-S-LG-LA5-12-31-20240313-0044";
            //    carrierStatus.sipSN[i] = "FN6H80009FX0000DVM_" + i.ToString();
            //    carrierStatus.Alert_Bumper_Nozzle[i] = new Random().Next(1, 4);
            //    carrierStatus.Alert_Bumper_PastePress[i] = (float)(new Random().NextDouble() * 0.1 + 0.45);
            //    carrierStatus.Alert_Bumper_Indenter[i] = new Random().Next(1, 4);
            //    carrierStatus.Alert_Bumper_Isthrow[i] = 1;
            //    carrierStatus.Alert_Bumper_ThrowCount[i] = 1;
            //    if (i < 6)
            //    {
            //        carrierStatus.Alert_Bumper_ThrowReason[i] = (int)EN_TossingCode.MaterialDeflect;
            //    }
            //    else
            //    {
            //        carrierStatus.Alert_Bumper_ThrowReason[i] = (int)EN_TossingCode.BreakVacuum;
            //    }

            //    carrierStatus.Flex_GND_Tape_Nozzle[i] = new Random().Next(1, 4);
            //    carrierStatus.Flex_GND_Tape_PastePress[i] = (float)(new Random().NextDouble() * 0.1 + 0.45);
            //    carrierStatus.Flex_GND_Tape_Indenter[i] = new Random().Next(1, 4);
            //    carrierStatus.Flex_GND_Tape_Isthrow[i] = 1;
            //    carrierStatus.Flex_GND_Tape_ThrowCount[i] = 1;
            //    if (i < 6)
            //    {
            //        carrierStatus.Flex_GND_Tape_ThrowReason[i] = (int)EN_TossingCode.MaterialDeflect;
            //    }
            //    else
            //    {
            //        carrierStatus.Flex_GND_Tape_ThrowReason[i] = (int)EN_TossingCode.BreakVacuum;
            //    }


            //    carrierStatus.cameraResults[i].x1 = new Random().NextDouble() * 0.1 + 2.0;
            //    carrierStatus.cameraResults[i].y1 = new Random().NextDouble() * 0.1 + 2.4;
            //    carrierStatus.cameraResults[i].r1 = new Random().NextDouble() * 0.1 + 2.8;

            //    carrierStatus.cameraResults[i].X1 = new Random().NextDouble() * 0.1 + 5.0;
            //    carrierStatus.cameraResults[i].Y1 = new Random().NextDouble() * 0.1 + 5.4;
            //    carrierStatus.cameraResults[i].R1 = new Random().NextDouble() * 0.1 + 5.8;

            //    if (i < 6)
            //    {
            //        carrierStatus.cameraResults[i].camResult = EN_CamResult.XYOverRange1;
            //    }
            //    else
            //    {
            //        carrierStatus.cameraResults[i].camResult = EN_CamResult.XYOverRange2;
            //    }
            //}

            //for (int i = 0; i < 12; i++)
            //{
            //    _stepStatus.ReacheckNGPerHour.AddOne(carrierStatus, new Random().Next(1, 12));
            //}
        }

        public void ChangeUPHTossing()
        {
            CurrentShowUPH = !CurrentShowUPH;
        }

        public void ChangeUseMes()
        {
            CurrentShowMes = !CurrentShowMes;
        }
        #endregion
    }
}
