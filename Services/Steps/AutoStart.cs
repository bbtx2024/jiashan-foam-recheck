using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Component.HIVE;
using QA.Business.Component.MES;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Model.Alarm;
using QA.Business.Station;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using IComponents = QA.Business.Interfaces.IComponent;//注意在MEF中也有IComponent
using MessageBox = HandyControl.Controls.MessageBox;
using UserManager = QA.Business.Manager.UserManager;//20250716 刷卡 版本更新报警停机
using EN_UserType = QA.Business.Manager.EN_UserType;//20250716 刷卡 版本更新报警停机

namespace QA.Business.Steps
{
    public class AutoStart_Station1 : IStepStation1, IHandle<ObservableCollection<AlarmInfoModel>>
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private StepStatus _stepStatus;
        private GlobalVariable _globalVariable;
        private Hive_Component _hive_Component;
        private MES_Component _mes_Component;
        private ParamManager _paramManager;

        //20250718 版本更新报警停机
        private CacheParamManager _cacheParamManager;
        //20250716 刷卡
        private UserManager _userManager;
        #endregion

        #region Property
        public ObservableCollection<AlarmInfoModel> CurrentAlarmInfoModels { get; set; } = new ObservableCollection<AlarmInfoModel>();
        public EN_RunStep RunStep { get; } = EN_RunStep.AutoStart;
        #endregion

        public AutoStart_Station1()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _stepStatus = IoC.Get<StepStatus>();
            _globalVariable = IoC.Get<GlobalVariable>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            _mes_Component = (MES_Component)IoC.Get<IMES>();
            _paramManager = IoC.Get<ParamManager>();

            //20250718 版本更新报警停机
            _cacheParamManager = IoC.Get<CacheParamManager>();
            //20250716 刷卡
            _userManager = IoC.Get<UserManager>();
        }

        #region Caliburn.Micro
        public void Handle(ObservableCollection<AlarmInfoModel> alarmInfo)
        {
            CurrentAlarmInfoModels = alarmInfo;
        }
        #endregion

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponents> components)
        {
            try
            {
                await Task.Delay(0);
                var plc = components.FirstOrDefault(s => s is PLC_Component) as PLC_Component;
                //判断HIVE模式
                if (_hive_Component.HiveStatus == EN_HiveStatus.Engineering)
                {
                    if (MessageBox.Show("当前是工程师模式，确定启动吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        _stepStatus.AllowStation2Start = 2;
                        return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                    }
                }
                //else if (_hive_Component.HiveStatus == EN_HiveStatus.PlannedDowntime)
                //{
                //    ShowErrTip("当前是手动计划停机模式");
                //    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                //}
                //else if (_hive_Component.HiveStatus == EN_HiveStatus.Downtime)
                //{
                //    if (_hive_Component.ErrDetail== "Daily Maintenance")
                //    {
                //        if (MessageBox.Show("当前是工程师模式，确定启动吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                //        {
                //            _stepStatus.AllowStation2Start = 2;
                //            return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                //        }
                //    }
                //    else
                //    {
                //        ShowErrTip("当前是宕机模式");
                //        return (false, EN_RunRet.TaskOk, EN_RunStep.Err);

                //    }
                    
                //}
                //检查安全门
                if (plc.IsSafeDoorAlarm())
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "AutoStart->检测到安全门报警", En_Logout_Type.Alarm, true);
                    _stepStatus.AllowStation2Start = 2;
                    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                }
                //检查是否有报警
                if (CurrentAlarmInfoModels.Count > 0)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "AutoStart->检测到系统有报警", En_Logout_Type.Alarm, true);
                    _stepStatus.AllowStation2Start = 2;
                    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                }
                //启动的时候检测Hive管控的软件版本号和实际版本号是否一致
                //CheckHiveSoftware();
                //20250718 启动时检查哈希值是否增加 增加则报警停机等待确认
                //if (_cacheParamManager.HomeUiParam.Statistic.CheckHash(_hive_Component.MainSoftwareSHA1))//是否新增
                //{
                //    _hive_Component._needSendUpdataHash = true;//是否需要上传hash用户相关信息
                //    ShowErrTip("软件版本更新未确认");
                //    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                //}

                //允许Station2进入其他步骤
                _stepStatus.AllowStation2Start = 1;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "AutoStart->异常" + ex.ToString() + ex.StackTrace, En_Logout_Type.Alarm, true);
                _stepStatus.AllowStation2Start = 2;
                return (true, EN_RunRet.MotionOk, EN_RunStep.Err);
            }
            return (true, EN_RunRet.MotionOk, EN_RunStep.AutoRunIdle);
        }

        public void ShowErrTip(string info)
        {
            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"AutoStart->{info}", En_Logout_Type.Alarm, true);
            var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.Robot, info, 100);
            MessageBox.Warning($"{info}，无法启动！", "警告");
            _baseBiz.RemoveRunAlarm(alarm);
            _stepStatus.AllowStation2Start = 2;
        }


        public void CheckHiveSoftware()
        {
            try
            {
                //检查Hive管控软件版本号和实际运行程序的版本号是否一致
                if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _paramManager.MESParam.BUse)
                {
                    if (_paramManager.MESParam.IsCheckVersion)
                    {
                        var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
                        string hiveVersion = null;
                        if (!_mes_Component.GetVersion(_paramManager.OtherSettingParam, out hiveVersion))
                        {
                        ShowBox:
                            var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.PDCA, $"软件版本异常", 0);
                            if (MessageBox.Show("当前软件版本和Hive版本不一致是否停机检查？", "操作提示", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.OK)
                            {
                                _baseBiz.RemoveRunAlarm(alarm);
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Hive管控软件版本为{hiveVersion} 和本地版本不一致", En_Logout_Type.Alarm, true);
                                goto ShowBox;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class AutoStart_Station2 : IStepStation2
    {
        #region Field
        StepStatus _stepStatus;
        #endregion

        #region Property
        public EN_RunStep RunStep { get; } = EN_RunStep.AutoStart;
        #endregion

        public AutoStart_Station2()
        {
            _stepStatus = IoC.Get<StepStatus>();
        }

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponents> components)
        {
            try
            {
                await Task.Delay(0);
                //检测是否允许启动
                while (_stepStatus.AllowStation2Start == 0)
                {
                    if (_stepStatus.NextStep2 != RunStep)
                    {
                        return (false, EN_RunRet.TaskOk, _stepStatus.NextStep2);
                    }
                    await Task.Delay(100);
                }
                if (_stepStatus.AllowStation2Start == 2)
                {
                    return (true, EN_RunRet.MotionErr, EN_RunStep.Err);
                }
                //只有AllowStation2Start值为1才能走到这里，继续运行
                _stepStatus.AllowStation2Start = 0;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "AutoStart2->" + ex.ToString(), En_Logout_Type.Alarm, true);
                return (true, EN_RunRet.MotionErr, EN_RunStep.Err);
            }
            return (true, EN_RunRet.MotionOk, EN_RunStep.AutoRunIdle);
        }
    }
}
