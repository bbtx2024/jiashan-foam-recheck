using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Component.HIVE;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Station;

namespace QA.Business.Steps
{
    public class Reset_StepNo1 : IStepStation1
    {
        #region Field
        private StepStatus _stepStatus;
        private ParamManager _paramManager;
        private PLC_Component _plc_Component;
        private Hive_Component _hive_Component;
        private IEventAggregator _eventAggregator;
        #endregion

        #region Property
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.Reset;
        #endregion

        #region Constructor
        public Reset_StepNo1()
        {
            _stepStatus = IoC.Get<StepStatus>();
            _paramManager = IoC.Get<ParamManager>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        #endregion

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            var _baseBiz = (BaseBiz)IoC.Get<IBaseBiz>();
            try
            {
                _baseBiz.AutoRun = true;
                //清除临时报警
                _baseBiz.ClearRunAlarm();
                //加载系统参数
                _paramManager.LoadParams();
                //重新加载所有组件
                _baseBiz.InitialReset();
                //给plc复位中信号
                _plc_Component.SetPcMachineStatus(PcToPlcMachineStatus.复位中);
                //重置标记
                _stepStatus.AllowStation2Start = 0;
                _stepStatus.ClearAllCarriers();
                await Task.Delay(100);
                //重置界面
                for (int cav = 1; cav <= 12; cav++)
                {
                    PublishCavityStateMsg(cav, EN_TrayStatus.待料);
                }
                //重置xyr显示区
                PublishCavityStateMsg(0, EN_TrayStatus.待料);
                _plc_Component.SetPcMachineStatus(PcToPlcMachineStatus.复位完成);
                await Task.Delay(100);
                _plc_Component.SetPcMachineStatus(PcToPlcMachineStatus.空闲中);
            }
            finally
            {
                //复位结束后，无论是否完成复位，都将autorun置为false
                _baseBiz.AutoRun = false;
            }
            return (false, EN_RunRet.ResetErr, EN_RunStep.Idle);
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

    public class Reset_Step : IStepStation2
    {
        #region Property
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.Reset;
        #endregion

        #region Constructor
        public Reset_Step()
        {
        }
        #endregion

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            await Task.Delay(100);
            return (false, EN_RunRet.ResetErr, EN_RunStep.Idle);
        }
    }
}
