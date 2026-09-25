using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.CacheParam;
using QA.Business.Component.Motion.Googol;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.SpotCheckPages.Models;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.SpotCheckPages.ViewModels
{
    [Export("SpotCheckPressurePageViewModel", typeof(ISpotPageViewModel))]
    public class SpotCheckPressurePageViewModel : Screen, INotifyPropertyChanged, ISpotPageViewModel
    {
        //0V------>0
        //5V------>16383 (0x3FFF)
        //分辨率为：0.00030525

        #region Field       
        private MotionGoogol_Component _mGoogol_Component;
        private static readonly object LockObj = new object();
        private ComponentManager _componentManager;
        private CacheParamManager _cacheParamManager;

        private Task _curTask = null;       //当前正在执行的耗时操作 

        public ManualPressModel Model { get; set; } = new ManualPressModel();

        //public MotionGoogol_Component Motion { get { return _componentManager.MotionGoogol; } }
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "压力点检";

        public ushort OrderID { get; set; } = 3;

        private float _calibValue { get; set; }
        public float CalibValue
        {
            get => _calibValue;
            set
            {
                _calibValue = value;
                NotifyOfPropertyChange(() => CalibValue);
            }
        }

        private string _curPressPosVlaue { get; set; }
        public string CurPressPosVlaue
        {
            get => _curPressPosVlaue;
            set
            {
                _curPressPosVlaue = value;
                NotifyOfPropertyChange(() => CurPressPosVlaue);
            }
        }

        private string _setDestPressValue { get; set; }
        public string SetDestPressValue
        {
            get => _setDestPressValue;
            set
            {
                _setDestPressValue = value;
                NotifyOfPropertyChange(() => SetDestPressValue);
            }
        }

        private string _setDestPressMaxValue { get; set; }
        public string SetDestPressMaxValue
        {
            get => _setDestPressValue;
            set
            {
                _setDestPressMaxValue = value;
                NotifyOfPropertyChange(() => SetDestPressMaxValue);
            }
        }
        private string _setBufferPressDis { get; set; }
        public string SetBufferPressDis
        {
            get => _setBufferPressDis;
            set
            {
                _setBufferPressDis = value;
                NotifyOfPropertyChange(() => SetBufferPressDis);
            }
        }
        private string _setBufferPressSpeed { get; set; }
        public string SetBufferPressSpeed
        {
            get => _setBufferPressSpeed;
            set
            {
                _setBufferPressSpeed = value;
                NotifyOfPropertyChange(() => SetBufferPressSpeed);
            }
        }

        private string _setPosPressOffset { get; set; }
        public string SetPosPressOffset
        {
            get => _setPosPressOffset;
            set
            {
                _setPosPressOffset = value;
                NotifyOfPropertyChange(() => SetPosPressOffset);
            }
        }

        private string _setPressCompressTime { get; set; }
        public string SetPressCompressTime
        {
            get => _setPressCompressTime;
            set
            {
                _setPressCompressTime = value;
                NotifyOfPropertyChange(() => SetPressCompressTime);
            }
        }
        #endregion

        #region Constructor
        public SpotCheckPressurePageViewModel()
        {
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            //_eventAggregator = IoC.Get<IEventAggregator>();

            Model.DoPressTargetPos = _cacheParamManager.PressParam.TargetHeight;
            Model.DoPressTargetPress = _cacheParamManager.PressParam.TargetPress;
            Model.DoPressBufferDis = _cacheParamManager.PressParam.BufferHeight;
            Model.DoPressSpeed = _cacheParamManager.PressParam.BufferSpeed;
            Model.DoPressPosRange = _cacheParamManager.PressParam.PositionRange;
            Model.DoPressLiftUp = _cacheParamManager.PressParam.LiftUp;
            Model.DoPressTimeout = _cacheParamManager.PressParam.PressTimeout;
            UpdateUI();
            CurPressPosVlaue = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.CalibPressSensorPos);
        }
        #endregion

        #region Method     

        //public void SelectItemChangedCommand(object p)
        //{
        //    ListView lv = p as ListView;
        //    Friend friend = lv.SelectedItem as Friend;
        //    Head = friend.Head;
        //    Nickname = friend.Nickname;
        //}

        //public void NumUDDestPressChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetDestPressValue = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();

        //}

        //public void NumUDDestMaxPressChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetDestPressMaxValue = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();
        //}

        //public void NumUDBufferPressDisChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetBufferPressDis = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();
        //}
        //public void NumUDBufferPressSpeedChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetBufferPressSpeed = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();
        //}
        //public void NumUDPressPosOffsetChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetPressPosOffset = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();
        //}
        //public void NumUDPressCompressTimeChanged(object p)
        //{
        //    NumericUpDown numUd = p as NumericUpDown;
        //    _cacheParamManager.manualPositionParam.SetPressCompressTime = (float)numUd.Value;
        //    _cacheParamManager.SaveManualPositionParam();
        //}

        //public void LowerPressCalib()
        //{
        //    //记录读取压力，和设置压力机压力
        //    _cacheParamManager.calibParam.LowerCalibPressValue = CalibValue;

        //    _cacheParamManager.SaveCalibParam();
        //}

        //public void HigherPressCalib()
        //{
        //    _cacheParamManager.calibParam.HigherCalibPressValue = CalibValue;

        //    _cacheParamManager.SaveCalibParam();
        //}

        public void UpdateUI()
        {
            int nozzleIdx = Model.SltNozzleIdx;
            short pressGet = _mGoogol_Component.AInput[nozzleIdx];
            Model.CurPress = _cacheParamManager.PressParam.SglParam[nozzleIdx].GetCalibedPress(pressGet).ToString("F2");
        }

        public void GetCurPress(object selectIndex)
        {
            int nozzleIdx = Model.SltNozzleIdx;
            short pressGet = _mGoogol_Component.AInput[nozzleIdx];
            Model.CurPress = _cacheParamManager.PressParam.SglParam[nozzleIdx].GetCalibedPress(pressGet).ToString("F2");

            //UpdateUI();
        }

        /// <summary>
        /// 执行低压校准
        /// </summary>
        public void DoLowPressCalib()
        {
            MessageBoxResult rs = MessageBox.Ask("确认是否执行低压校准", "ASK");
            if (rs == MessageBoxResult.Yes || rs == MessageBoxResult.OK)
            {
                int nozzleIdx = Model.SltNozzleIdx;
                short lowA = _mGoogol_Component.AInput[nozzleIdx];
                float lowD = Model.LowPressCalibValue;

                //LogHelper.Info(LogType.Manual, $"执行低压校准 head:{headidx} lowA:{lowA} lowD:{lowD}");
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"执行低压校准 nozzleIdx:{nozzleIdx} lowA:{lowA} lowD:{lowD}", En_Logout_Type.SpotCheck);
                if (!_cacheParamManager.PressParam.SglParam[nozzleIdx].DoLowCalib(lowA, lowD))
                {
                    MessageBox.Error("低压校准失败", "ERROR");
                    return;
                }
                _cacheParamManager.SaveSpecifiedParam(typeof(PressParam));
            }
        }

        /// <summary>
        /// 执行高压校准
        /// </summary>
        public void DoHighPressCalib()
        {
            MessageBoxResult rs = MessageBox.Ask("确认是否执行高压校准", "ASK");
            if (rs == MessageBoxResult.Yes || rs == MessageBoxResult.OK)
            {
                int nozzleIdx = Model.SltNozzleIdx;
                short highA = _mGoogol_Component.AInput[nozzleIdx];
                float highD = Model.HighPressCalibValue;

                //LogHelper.Info(LogType.Manual, $"执行低压校准 head:{headidx} highA:{highA} highD:{highD}");
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"执行高压校准 nozzleIdx:{nozzleIdx} highA:{highA} highD:{highD}", En_Logout_Type.SpotCheck);

                if (!_cacheParamManager.PressParam.SglParam[nozzleIdx].DoHighCalib(highA, highD))
                {
                    MessageBox.Error("高压校准失败", "ERROR");
                    return;
                }
                _cacheParamManager.SaveSpecifiedParam(typeof(PressParam));
            }
        }

        /// <summary>
        /// 执行一次空压
        /// </summary>
        public async void DoEmptyPress()
        {
            MessageBoxResult rs = MessageBox.Ask("确认是否执行空压", "ASK");
            if (rs == MessageBoxResult.Yes || rs == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    int nozzleIdx = Model.SltNozzleIdx;
                    if (_curTask != null && !_curTask.IsCompleted)
                    {
                        MessageBox.Warning("当前还有未完成的任务，请稍后重试", "WARNING");
                        return;
                    }
                    _curTask = new TaskFactory().StartNew(() =>
                    {
                        En_AxisNum axis = Model.Heads[Model.SltNozzleIdx].Axis;

                        //NLogTrace.Info(LogType.Manual, $"执行一次空压 head:{headidx} axis:{axis}");
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"执行一次空压 nozzleIdx:{nozzleIdx} axis:{axis}");
                        _mGoogol_Component.SetMoveSpeed(axis, Model.DoPressSpeed);
                        _mGoogol_Component.MoveSingleAxis(axis, Model.DoPressTargetPos, false);
                        Thread.Sleep(500);

                        DateTime startTime = DateTime.Now;
                        bool isPressOk = false;
                        while (true)
                        {
                            if ((DateTime.Now - startTime).TotalMilliseconds >= Model.DoPressTimeout)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"[Spot Press Check]执行一次空压 到达超时时间 {Model.DoPressTimeout}");
                                break;
                            }
                            if (!isPressOk && !_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"[Spot Press Check]执行一次空压 点位运动结束 {axis}");
                                return;
                            }
                            short pressA = _mGoogol_Component.AInput[nozzleIdx];
                            float pressD = _cacheParamManager.PressParam.SglParam[nozzleIdx].GetCalibedPress(pressA);
                            Console.WriteLine($"{pressA},   {pressD}");
                            if (pressD >= Model.DoPressTargetPress * Model.BeforeRate)   //提前量
                            {
                                if (!isPressOk)
                                {
                                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"[Spot Press Check]执行一次空压 到达目标压力 {pressD}");
                                    isPressOk = true;
                                    _mGoogol_Component.StopAxisMove(axis);
                                }
                            }
                            Thread.Sleep(10);
                        }

                        float curPos = _mGoogol_Component.GetAxisCurPos(axis);
                        float liftUpPos = curPos - Model.DoPressLiftUp > 0 ? curPos - Model.DoPressLiftUp : 0;
                        if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                            return;
                        if (!_mGoogol_Component.MoveRelativeSingleAxis(axis, liftUpPos, true, true))
                        {
                            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"[Spot Press Check]执行一次空压 上抬错误");
                            return;
                        }
                    });
                });
            }
        }

        public void SetPressPos()
        {
            MessageBoxResult mbr = MessageBox.Show("确定设置压力标定坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                _cacheParamManager.manualPositionParam.CalibPressSensorPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                _cacheParamManager.manualPositionParam.CalibPressSensorPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                _cacheParamManager.manualPositionParam.CalibPressSensorPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                _cacheParamManager.manualPositionParam.CalibPressSensorPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                _cacheParamManager.SaveManualPositionParam();

                var pos = _cacheParamManager.manualPositionParam.CalibPressSensorPos;
                CurPressPosVlaue = NLogTrace.GetFloatArrayString(pos);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetPressPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                return;
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetPressPos() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }

        public async void MoveSpacePressPosXY()
        {
            MessageBoxResult mbr = MessageBox.Show("确定移动压力标定坐标XY吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
                    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, _cacheParamManager.manualPositionParam.CalibPressSensorPos, true, true)) return;

                    var pos = _cacheParamManager.manualPositionParam.CalibPressSensorPos;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePressPosXY() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                    MessageBox.Show("移动压力标定坐标XY成功！");
                    return;
                });
            }
            else
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePressPosXY() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }

        public async void MoveSpacePressPosZ()
        {
            MessageBoxResult mbr = MessageBox.Show("确定移动压力标定坐标Z吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
                    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, _cacheParamManager.manualPositionParam.CalibPressSensorPos, true, true)) return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, _cacheParamManager.manualPositionParam.CalibPressSensorPos[2], true, true)) return;

                    var pos = _cacheParamManager.manualPositionParam.CalibPressSensorPos;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePressPosXY() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                    MessageBox.Show("移动压力标定坐标Z 成功！");
                    return;
                });
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePressPosXY() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }

        public void SaveEmptyPressParam()
        {
            MessageBoxResult rs = MessageBox.Ask("确认保存空压参数", "ASK");
            if (rs == MessageBoxResult.Yes || rs == MessageBoxResult.OK)
            {
                _cacheParamManager.PressParam.TargetHeight = Model.DoPressTargetPos;
                _cacheParamManager.PressParam.TargetPress = Model.DoPressTargetPress;
                _cacheParamManager.PressParam.BufferHeight = Model.DoPressBufferDis;
                _cacheParamManager.PressParam.BufferSpeed = Model.DoPressSpeed;
                _cacheParamManager.PressParam.PositionRange = Model.DoPressPosRange;
                _cacheParamManager.PressParam.LiftUp = Model.DoPressLiftUp;
                _cacheParamManager.PressParam.PressTimeout = Model.DoPressTimeout;
                _cacheParamManager.SaveSpecifiedParam(typeof(PressParam));
            }
        }

        /// <summary>
        /// 停止空压
        /// </summary>
        public void StopEmptyPress()
        {
            int nozzleIdx = Model.SltNozzleIdx;
            En_AxisNum axis = Model.Heads[Model.SltNozzleIdx].Axis;
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"停止空压 nozzleIdx:{nozzleIdx} axis:{axis}");

            _mGoogol_Component.StopAxisMove(axis);
        }


        #endregion
    }
}
