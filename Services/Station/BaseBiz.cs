using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Component.LaserHeightSensor;
using QA.Business.Component.MES;
using QA.Business.Component.Monitor;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.PDCA;
using QA.Business.Component.PLC;
using QA.Business.Component.Scanner;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Model.Alarm;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using IComponent = QA.Business.Interfaces.IComponent;
using StepStatus = QA.Business.Steps.StepStatus;

namespace QA.Business.Station
{
    public struct RunStepInfo
    {
        public En_StationNo Station { get; set; }
        public EN_RunStep RunStep { get; set; }
    }

    public class BaseBiz : PropertyChangedBase, IBaseBiz, IHandle<PlcTrigInfo>
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private StepFactory _stepFactory;
        protected StepStatus _stepStatus;
        protected ParamManager _paramManager;
        protected CacheParamManager _cacheParamManager;

        protected IPLC _plc;
        protected ICamera _camera;
        protected IPDCA _pdca;
        protected IMES _mes;
        protected IScanner _scanner;
        protected IScannerRechck _scannerRechck;
        protected ILaserHeightSensor _laserHeightSensor;
        protected IMonitor _monitor;
        protected IMGoogol _mGoogol;
        protected IHive _hive;

        protected List<IComponent> _components;//存储所有部件
        protected ObservableCollection<AlarmInfoModel> _alarmInfoModels = new ObservableCollection<AlarmInfoModel>();
        protected ObservableCollection<AlarmInfoModel> _runAlarmInfos = new ObservableCollection<AlarmInfoModel>();
        #endregion

        #region Property
        private RunStepInfo RunStep1 = new RunStepInfo()
        {
            Station = En_StationNo.StationNo1,
            RunStep = EN_RunStep.Idle
        };
        public EN_RunStep CurrStep1 => RunStep1.RunStep;

        private RunStepInfo RunStep2 = new RunStepInfo()
        {
            Station = En_StationNo.StationNo2,
            RunStep = EN_RunStep.Idle
        };
        public EN_RunStep CurrStep2 => RunStep2.RunStep;

        private RunStepInfo RunStep3 = new RunStepInfo()
        {
            Station = En_StationNo.StationNo3,
            RunStep = EN_RunStep.ReadNgState
        };
        public EN_RunStep CurrStep3 => RunStep3.RunStep;

        public bool AutoRun { get; set; } = false;

        public bool IsResetOk { get; set; } = false;
        #endregion

        #region Constructor
        public BaseBiz()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _stepFactory = IoC.Get<StepFactory>();
            _stepStatus = IoC.Get<StepStatus>();
            _paramManager = IoC.Get<ParamManager>();
            _cacheParamManager = IoC.Get<CacheParamManager>();

            _plc = (PLC_Component)IoC.Get<IPLC>();
            _camera = (Camera_Component)IoC.Get<ICamera>();
            _pdca = (PDCA_Component)IoC.Get<IPDCA>();
            _mes = (MES_Component)IoC.Get<IMES>();
            _scanner = (Scanner_TcpComponent)IoC.Get<IScanner>();
            _scannerRechck = (Scanner_TcpComponentRecheck)IoC.Get<IScannerRechck>();
            _laserHeightSensor = (LaserHeightSensor_Component)IoC.Get<ILaserHeightSensor>();
            _monitor = (Monitor_SerialComponent)IoC.Get<IMonitor>();
            _mGoogol = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _hive = (Hive_Component)IoC.Get<IHive>();

            _mGoogol.RobotValueRefresh += ReadRobotValueRefresh;
        }
        #endregion

        #region 初始化
        public virtual bool Initial()
        {
            if (!InitialReset())
            {
                return false;
            }
            try
            {
                MasterRunNo1();
                MasterRunNo2();
                MasterRunNo3();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"软件初始化发生异常:{e.Message}{Environment.NewLine}{e.StackTrace}", En_Logout_Type.Run, true);
                return false;
            }
            return true;
        }

        public bool InitialReset()
        {
            try
            {
                List<IComponent> components = new List<IComponent>();

                _plc.Initial(_paramManager.PLCParam);
                _plc.ComponentName = "PLC";
                components.Add(_plc);

                _scanner.Initial(_paramManager.ScannerParam);
                _scanner.ComponentName = "撕膜位扫码枪";
                components.Add(_scanner);
                _scannerRechck.Initial(_paramManager.ScannerParamRecheck);
                _scannerRechck.ComponentName = "复检位扫码枪";
                components.Add(_scannerRechck);

                _camera.Initial(_paramManager.CameraParam);
                _camera.ComponentName = "相机";
                components.Add(_camera);

                _hive.Initial(_paramManager.HiveParam);
                _hive.ComponentName = "HIVE";
                components.Add(_hive);

                _pdca.Initial(_paramManager.PDCAParam);
                _pdca.ComponentName = "PDCA";
                components.Add(_pdca);

                _mes.Initial(_paramManager.MESParam);
                _mes.ComponentName = "MES";
                components.Add(_mes);

                _components = components;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"软件初始化发生异常:{e.Message}{Environment.NewLine}{e.StackTrace}", En_Logout_Type.Run, true);
                return false;
            }
            return true;
        }

        public virtual bool Start()
        {
            try
            {
                _components.ForEach(t => t.Start());
                RefreshComponentsAlarms();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"软件启动发生异常:{e.Message}{Environment.NewLine}{e.StackTrace}", En_Logout_Type.Exception, true);
                return false;
            }
            return true;
        }

        public virtual bool Stop()
        {
            try
            {
                _components.ForEach(t => t.Stop());
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"软件启动发生异常:{e.Message}{Environment.NewLine}{e.StackTrace}");
                return false;
            }
            return true;
        }

        protected async void MasterRunNo1()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        _eventAggregator.Publish(RunStep1, action => { Task.Run(action); });
                        bool getHandler = _stepFactory.GetStepHandlerDic1().TryGetValue(RunStep1.RunStep, out IStepStation1 stepHander);
                        if (getHandler)
                        {
                            var handleResult = await stepHander.Handle(RunStep1.RunStep, _components);
                            //根据_stepStatus.NextStep1有没有修改，决定下一步应该切换到哪个step
                            if (RunStep1.RunStep != _stepStatus.NextStep1)
                            {
                                RunStep1.RunStep = _stepStatus.NextStep1;
                            }
                            else
                            {
                                RunStep1.RunStep = _stepStatus.NextStep1 = handleResult.Item3;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo1->{e.Message}", En_Logout_Type.Exception, true);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo1->{e}", En_Logout_Type.Exception);
                        RunStep1.RunStep = _stepStatus.NextStep1 = EN_RunStep.Err;
                    }
                    await Task.Delay(10);
                }
            });
        }

        protected async void MasterRunNo2()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        _eventAggregator.Publish(RunStep2, action => { Task.Run(action); });
                        bool getHandler = _stepFactory.GetStepHandlerDic2().TryGetValue(RunStep2.RunStep, out IStepStation2 stepHander);
                        if (getHandler)
                        {
                            var handleResult = await stepHander.Handle(RunStep2.RunStep, _components);
                            //根据_stepStatus.NextStep2有没有修改，决定下一步应该切换到哪个step
                            if (RunStep2.RunStep != _stepStatus.NextStep2)
                            {
                                RunStep2.RunStep = _stepStatus.NextStep2;
                            }
                            else
                            {
                                RunStep2.RunStep = _stepStatus.NextStep2 = handleResult.Item3;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo2->{e.Message}", En_Logout_Type.Exception, true);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo2->{e}", En_Logout_Type.Exception);
                        RunStep2.RunStep = _stepStatus.NextStep2 = EN_RunStep.Err;
                    }
                    await Task.Delay(10);
                }
            });
        }

        protected async void MasterRunNo3()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        _eventAggregator.Publish(RunStep3, action => { Task.Run(action); });
                        bool getHandler = _stepFactory.GetStepHandlerDic3().TryGetValue(RunStep3.RunStep, out IStepStation3 stepHander);
                        if (getHandler)
                        {
                            var handleResult = await stepHander.Handle(RunStep3.RunStep, _components);
                            //根据_stepStatus.NextStep2有没有修改，决定下一步应该切换到哪个step
                            if (RunStep3.RunStep != _stepStatus.NextStep3)
                            {
                                RunStep3.RunStep = _stepStatus.NextStep3;
                            }
                            else
                            {
                                RunStep3.RunStep = _stepStatus.NextStep3 = handleResult.Item3;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo3->{e.Message}", En_Logout_Type.Exception, true);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"MasterRunNo3->{e}", En_Logout_Type.Exception);
                        RunStep3.RunStep = _stepStatus.NextStep3 = EN_RunStep.Err;
                    }
                    await Task.Delay(10);
                }
            });
        }

        #region Caliburn.Micro
        //PLC触发启动、复位
        public void Handle(PlcTrigInfo info)
        {
            if (info.IsEStop)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "Plc触发急停", En_Logout_Type.PlcAction, true);
                //(_mGoogol as MotionGoogol_Component).SetServeOnOff(false);
                //(_mGoogol as MotionGoogol_Component).SetExitSts(true);
            }
            else if (info.IsReset)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Plc触发复位", En_Logout_Type.PlcAction, true);
                //(_plc as PLC_Component).SetSwReady(false);
                //(_mGoogol as MotionGoogol_Component).StopAllAxisMove();
                _stepStatus.NextStep1 = EN_RunStep.Reset;
                _stepStatus.NextStep2 = EN_RunStep.Reset;
                _stepStatus.ManualResetEvt_CtrlPlace.Set();
                _stepStatus.AutoResetEvt_CtrlVisual.Set();
                _stepStatus.AutoResetEvt_CtrlScanner.Set();
                _stepStatus.AutoResetEvt_DownCam.Set();
            }
            else if (info.IsStop)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "Plc触发停止", En_Logout_Type.PlcAction, true);
            }
            else if (info.IsStart)
            {
                if (AutoRun == false)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Plc触发启动", En_Logout_Type.PlcAction, true);
                    _stepStatus.NextStep1 = EN_RunStep.AutoStart;
                    _stepStatus.NextStep2 = EN_RunStep.AutoStart;
                    AutoRun = true;
                }
            }
        }
        #endregion

        #endregion

        #region 界面交互
        private async void ReadRobotValueRefresh(RobotRealStateInfo _robotRealStateInfo)
        {
            await Task.Factory.StartNew(async () =>
            {
                _eventAggregator.Publish(_robotRealStateInfo, action => { Task.Run(action); });
                await Task.Delay(200);
            });
        }

        public AlarmInfoModel AddRunAlarm(EN_WarnModules model, string msg, int errorCode)
        {
            AlarmInfoModel alarm = new AlarmInfoModel()
            {
                AlarmLevel = EN_WARN_LEVEL.Warn,
                AlarmModule = model,
                AlarmMsg = msg,
                Datetime = DateTime.Now,
                ErrorCode = errorCode,
                //Index = 0
            };
            _runAlarmInfos.Add(alarm);
            return alarm;
        }

        public void RemoveRunAlarm(AlarmInfoModel alarm)
        {
            _runAlarmInfos.Remove(alarm);
        }

        public void ClearRunAlarm()
        {
            _runAlarmInfos.Clear();
        }

        private async void RefreshComponentsAlarms()
        {
            await Task.Factory.StartNew(async () =>
            {
                //记录上一次状态，0ok，1当前报警大于个数0，2一站ng，3二站ng
                int lastErrorReason = 0;
                while (true)
                {
                    try
                    {
                        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Application.Current.Dispatcher));
                        SynchronizationContext.Current.Post(Pl =>
                        {
                            _alarmInfoModels.Clear();
                            _components.ForEach(t =>
                            {
                                t.GetCurAlarms(ref _alarmInfoModels);
                            });
                            foreach (var alarm in _runAlarmInfos)
                            {
                                alarm.Index = _alarmInfoModels.Count;
                                _alarmInfoModels.Add(alarm);
                            }
                            //设置蜂鸣器
                            var _plc_Component = _plc as PLC_Component;
                            if (_plc_Component.IsConnected)
                            {
                                if (_alarmInfoModels.Count > 0)
                                {
                                    if (lastErrorReason != 1)
                                    {
                                        StringBuilder sb = new StringBuilder($"lastErrorReason{lastErrorReason}->1");
                                        for (int i = 0; i < _alarmInfoModels.Count; i++)
                                        {
                                            sb.Append(_alarmInfoModels[i].AlarmMsg);
                                        }
                                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, sb.ToString());
                                        lastErrorReason = 1;
                                    }
                                }
                                else if (RunStep1.RunStep == EN_RunStep.Err)
                                {
                                    if (lastErrorReason != 2)
                                    {
                                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"lastErrorReason{lastErrorReason}->2");
                                        lastErrorReason = 2;
                                    }
                                }
                                else if (RunStep2.RunStep == EN_RunStep.Err)
                                {
                                    if (lastErrorReason != 3)
                                    {
                                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"lastErrorReason{lastErrorReason}->3");
                                        lastErrorReason = 3;
                                    }
                                }
                                else
                                {
                                    if (lastErrorReason != 0)
                                    {
                                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"lastErrorReason{lastErrorReason}->0");
                                        lastErrorReason = 0;
                                    }
                                }
                                _plc_Component.SetPcMachineStatus(lastErrorReason == 0 ? PcToPlcMachineStatus.报警警告清除 : PcToPlcMachineStatus.报警中);
                            }
                            //发布报警信息传递给ViewModel——>传递给View界面
                            _eventAggregator.Publish(_alarmInfoModels, action => { Task.Run(action); });
                            _eventAggregator.Publish(_components, action => { Task.Run(action); });
                        }, null);
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"轮询获取报警异常，{e}");
                    }
                    await Task.Delay(200);
                }
            });
        }

        public ObservableCollection<AlarmInfoModel> GetAlarmInfo()
        {
            return _alarmInfoModels;
        }
        #endregion
    }
}
