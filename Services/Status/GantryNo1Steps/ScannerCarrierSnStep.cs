using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Component.HIVE;
using QA.Business.Component.MES;
using QA.Business.Component.PLC;
using QA.Business.Component.Scanner;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Message;
using QA.Business.Station;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.Business.Status.GantryNo1Steps
{
    public class ScannerCarrierSnStep : IStepStation1
    {
        #region Field    
        private StepStatus _stepStatus;
        private Scanner_TcpComponent _scanner_Component;
        private PLC_Component _plc_Component;
        private Hive_Component _hive_Component;
        private MES_Component _mes_Component;
        private GlobalVariable _globalVariable;
        private IEventAggregator _eventAggregator;
        #endregion

        #region Property
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.ScannerCarrierSnStep;
        #endregion

        #region Constructor
        public ScannerCarrierSnStep()
        {
            _stepStatus = IoC.Get<StepStatus>();
            _scanner_Component = (Scanner_TcpComponent)IoC.Get<IScanner>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            _mes_Component = (MES_Component)IoC.Get<IMES>();
            _globalVariable = IoC.Get<GlobalVariable>();
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        #endregion

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            //进入条件：_plc_Component.GetScannerSignal() == true
            //如果当前无载具，则可以刷新穴位状态
            if (_stepStatus.GetCurCarrier() == null)
            {
                for (int cav = 1; cav <= 12; cav++)
                {
                    PublishCavityStateMsg(cav, EN_TrayStatus.等待处理);
                }
                PublishCavityStateMsg(0, EN_TrayStatus.待料);
            }
            //扫码
            CarrierStatus carrierStatus = new CarrierStatus();
            string carrierSN;
            if (_plc_Component.IsSimulateRun())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Scan->空跑模式，不扫载具码", En_Logout_Type.Run, true);
            }
            else if (!_scanner_Component.Param.BUse && _hive_Component.HiveStatus == EN_HiveStatus.Engineering)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Scan->扫码未启用", En_Logout_Type.Run, true);
            }
            else
            {
                if (!_scanner_Component.DataManManualTriger(out carrierSN))
                {
                    while (!_scanner_Component.DataManManualTriger(out carrierSN))
                    {
                        if (_stepStatus.NextStep1 != RunStep) return (false, EN_RunRet.MotionErr, _stepStatus.NextStep1);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Scan->撕膜扫码失败", En_Logout_Type.Alarm, true);
                        var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, $"扫码失败", 101);
                        if (MessageBox.Show("撕膜载具码扫码失败，是否重新扫码？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            _baseBiz.RemoveRunAlarm(alarm);
                            continue;
                        }
                        else
                        {
                            _baseBiz.RemoveRunAlarm(alarm);
                            return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
                        }
                    }
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Scan->扫码完成，载具码：{carrierSN}", En_Logout_Type.Run, true);
                carrierStatus.carrierSN = carrierSN;
            }
            carrierSN = carrierStatus.carrierSN;
            //获取tray信息
            if (!_stepStatus.GetTray(carrierStatus))
            {
                var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, "未获取到前站Tray信息", 109);
                if (MessageBox.Show($"未找到{carrierSN}的Tray信息！\n是否使用自动生成的Tray信息？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _baseBiz.RemoveRunAlarm(alarm);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"Camera->自动生成{carrierSN}的Tray信息", En_Logout_Type.Run, true);
                }
                else
                {
                    _baseBiz.RemoveRunAlarm(alarm);
                    return (false, EN_RunRet.MotionErr, EN_RunStep.Err);
                }
            }
            else
            {
                int dwellStatus = 0;
                for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                {
                    if (carrierStatus.isEmpty[i] == 1)
                    {
                        dwellStatus |= (1 << (i));
                    }
                }
                _plc_Component.SetTaryIsEmpty(dwellStatus);
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->已获取{carrierSN}的Tray信息", En_Logout_Type.Run, true);
            }
            _plc_Component.SetScanResult(EN_Result.OK);
            //给PLC处理时间
            await Task.Delay(200);
            //添加载具信息
            _stepStatus.AddCarrier(carrierStatus);
            //保存载具队列状态到日志，并保存当前读到的载具信息到 D:/tray/read
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Scan->当前载具队列：{_stepStatus.GetCarrierListStr()}", En_Logout_Type.Run);
            //判断载具列表是否个数异常
            //if (_stepStatus.Carriers.Count > 2)
            //{
            //    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Scan->当前载具数目大于2", En_Logout_Type.Run, true);
            //    //等待二站处理完再报警
            //    while (_baseBiz.CurrStep2 == EN_RunStep.CameraStep)
            //    {
            //        await Task.Delay(200);
            //    }
            //    return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
            //}
            return (false, EN_RunRet.TaskOk, EN_RunStep.AutoProcess);
        }

        private void PublishCavityStateMsg(int cavity, EN_TrayStatus status, string xStr = "", string yStr = "", string rStr = "")
        {
            _eventAggregator.Publish(new CarrierInfoPanelMessage()
            {
                Cavity = cavity,
                Status = status,
                XStr = xStr,
                YStr = yStr,
                RStr = rStr,
            }, action => { Task.Run(action); });
        }
    }
}
