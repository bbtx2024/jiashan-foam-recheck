using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.Motion.Robot9075;
using QA.Business.Component.PLC;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Station;
using QA.Business.Steps;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.Pages.OtherViews.ViewModels
{
    [Export("NormalToolsViewModel")]
    public class NormalToolsViewModel : Screen, IHandle<List<IComponent>>
    {
        #region Field

        private readonly IEventAggregator _eventAggregator;
        private readonly ParamManager _paramManager;
        private Motion9075_Component[] _robot9075s;
        private protected PLC_Component _plc_Component;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private StepStatus _stepStatus;
        private readonly MotionGoogol_Component _mGoogol_Component;
        private IBaseBiz _baseBiz;

        #endregion

        #region Property

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

        public NormalToolsViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _paramManager = IoC.Get<ParamManager>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _stepStatus = IoC.Get<StepStatus>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _baseBiz = IoC.Get<IBaseBiz>();
            Start();
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var delayTime = 1000;
                    try
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        //PlcReady = _plc_Component.GetReadySignal() == true ? "1" : "0";
                    }
                    catch (Exception e)
                    {
                        //NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
                    }
                    await Task.Delay(delayTime, _cancellationToken);
                }
            }, _cancellationToken);
            return true;
        }

        #endregion

        #region Handle

        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
        }

        #endregion

        #region Override

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        #endregion

        #region Method

        /// <summary>
        /// 保存根据复检数值自动生成的制程
        /// </summary>
        public void CreateNewTask()
        {
        }

        public async void ClearTapes()
        {
            if (MessageBox.Show("确定清除所有吸嘴吸取的物料吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                //await Task.Run(() =>
                //{
                //    if (!_mGoogol_Component.GetCurPos() || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    if (Math.Abs(_mGoogol_Component.CurPos[(byte)En_AxisNum.X1]) > 1
                //        || Math.Abs(_mGoogol_Component.CurPos[(byte)En_AxisNum.Y1]) > 1
                //        || Math.Abs(_mGoogol_Component.CurPos[(byte)En_AxisNum.X2]) > 1
                //        || Math.Abs(_mGoogol_Component.CurPos[(byte)En_AxisNum.Y2]) > 1
                //        || Math.Abs(_mGoogol_Component.CurPos[(byte)En_AxisNum.Z2]) > 1)
                //    {
                //        MessageBox.Show("当前位置不是原点位置！请复位后再清料", "操作提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                //        return;
                //    }
                //    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid) || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    var posNgMaterial = _stepStatus.CacheParamManager.manualPositionParam.AxisNgSiloPos;
                //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { posNgMaterial[0], posNgMaterial[1] }, true) || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    if (!_stepStatus.SetAllCylindersDown() || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, posNgMaterial[2], true, true))
                //    {
                //        return;
                //    }
                //    for (int nozzleIdx = 0; nozzleIdx < 4; nozzleIdx++)
                //    {
                //        if (!_stepStatus.SetNozzleCloseInhaleOpenBreak(nozzleIdx + 1) || _mGoogol_Component.Exit())
                //        {
                //            return;
                //        }
                //    }
                //    Thread.Sleep(200);
                //    for (int nozzleIdx = 0; nozzleIdx < 4; nozzleIdx++)
                //    {
                //        if (!_stepStatus.SetNozzleCloseBreak(nozzleIdx + 1) || _mGoogol_Component.Exit())
                //        {
                //            return;
                //        }
                //    }
                //    if (!_stepStatus.SetAllCylindersUp() || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true) || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[] { 0, 0 }, true) || _mGoogol_Component.Exit())
                //    {
                //        return;
                //    }
                //    MessageBox.Show("已全部清料！", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //});
            }
        }

        #endregion
    }
}
