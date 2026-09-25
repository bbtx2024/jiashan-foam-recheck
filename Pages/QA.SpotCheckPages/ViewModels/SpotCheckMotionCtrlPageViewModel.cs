using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.Motion.Robot9075;
using QA.Business.Interfaces;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.SpotCheckPages.ViewModels
{
    [Export("SpotCheckMotionCtrlPageViewModel", typeof(ISpotPageViewModel))]
    public class SpotCheckMotionCtrlPageViewModel : Screen, INotifyPropertyChanged, ISpotPageViewModel
    {
        #region field
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private Motion9075_Component _motion9075_Component;
        private MotionGoogol_Component _mGoogol_Component;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "主板点检";

        public ushort OrderID { get; set; } = 0;

        /// <summary>
        /// 扩展输入
        /// </summary>
        private ObservableCollection<bool> _isExtIn = new ObservableCollection<bool>();
        public ObservableCollection<bool> IsExtIn
        {
            get => _isExtIn;
            set
            {
                _isExtIn = value;
                NotifyOfPropertyChange(() => IsExtIn);
            }
        }
        /// <summary>
        /// 主输入，目前没用
        /// </summary>
        private ObservableCollection<bool> _isMainIn = new ObservableCollection<bool>();
        public ObservableCollection<bool> IsMainIn
        {
            get => _isMainIn;
            set
            {
                _isMainIn = value;
                NotifyOfPropertyChange(() => IsMainIn);
            }
        }
        /// <summary>
        /// 扩展输出
        /// </summary>
        private ObservableCollection<bool> _isExtOut = new ObservableCollection<bool>();
        public ObservableCollection<bool> IsExtOut
        {
            get => _isExtOut;
            set
            {
                _isExtOut = value;
                NotifyOfPropertyChange(() => IsExtOut);
            }
        }
        /// <summary>
        /// 主输出，目前没用
        /// </summary>
        private ObservableCollection<bool> _isMainOut = new ObservableCollection<bool>();
        public ObservableCollection<bool> IsMainOut
        {
            get => _isMainOut;
            set
            {
                _isMainOut = value;
                NotifyOfPropertyChange(() => IsMainOut);
            }
        }
        private bool _isMonitorInState { get; set; } = false;
        public bool IsMonitorInState
        {
            get => _isMonitorInState;
            set
            {
                _isMonitorInState = value;
                NotifyOfPropertyChange(() => IsMonitorInState);
            }
        }

        #endregion

        #region Constructor
        public SpotCheckMotionCtrlPageViewModel()
        {
            _motion9075_Component = (Motion9075_Component)IoC.Get<IM9075>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();

            for (int i = 0; i < 4; i++)
            {
                //主输入、主输出目前没用
                IsMainOut.Add(false);
                IsMainIn.Add(false);
            }
            for (int i = 0; i < 16; i++)
            {
                IsExtOut.Add(false);
                IsExtIn.Add(false);
            }
        }
        #endregion

        #region Method
        public void IsCheckBoxExtOut(object index)
        {
            byte _index = Convert.ToByte(index);
            bool val = IsExtOut[_index];
            _mGoogol_Component.WriteOutPort(_index, val);
            _mGoogol_Component.ReadOutPort();
            IsExtOut[_index] = (_mGoogol_Component.EOutput & (1 << _index)) != 0 ? true : false;
        }

        public void IsCheckBoxMainOut(object index)
        {
            //主输出没用
            byte _index = Convert.ToByte(index);
            bool val = IsMainOut[_index];

            //_motion9075_Component.WriteOutPort((byte)(_index + 16), val, true);
            //_motion9075_Component.ReadOutPort();
            //IsMainOut[_index] = (_motion9075_Component.MOutput & (1 << _index)) != 0 ? true : false;
        }

        public void IsCheckBoxMonitorInState()
        {
            if (IsMonitorInState)
            {
                //启动监控输出状态
                Start();
            }
            else
            {
                //停止监控输出状态
                Stop();
            }
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var delayTime = 100;
                    try
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            IsExtIn[i] = (_mGoogol_Component.EInput & (1 << i)) != 0 ? true : false;
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
                    }
                    await Task.Delay(delayTime, _cancellationToken);
                }
            }, _cancellationToken);
            return true;
        }
        public bool Stop()
        {
            try
            {
                //base.Close();
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
            }
            return true;
        }
        #endregion
    }
}
