using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Message;
using QA.Business.Steps;

namespace QA.Business.Status.GantryNo3Steps
{
    public class ReadNgState : IStepStation3
    {
        #region Field    
        private StepStatus _stepStatus;
        private protected PLC_Component _plc_Component;
        private IEventAggregator _eventAggregator;
        private GlobalVariable _globalVariable;
        #endregion

        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.ReadNgState;

        public ReadNgState()
        {
            _stepStatus = IoC.Get<StepStatus>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _globalVariable = IoC.Get<GlobalVariable>();
        }

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            while (!_plc_Component.IsConnected)
            {
                await Task.Delay(200);
            }
            while (true)
            {
                //获取ng仓载具SN，并显示在界面上
                if (!_plc_Component.ReadAllNGCarrierSns(out bool changedSn, out string[] carrierSns))
                {
                    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                }
                //获取ng仓状态，并显示在界面上
                if (!_plc_Component.ReadAllNGErrorCodes(out bool changed, out EN_TrayStatus[,] ngErrorCode))
                {
                    return (false, EN_RunRet.TaskOk, EN_RunStep.Err);
                }
                if (changed || changedSn)
                {
                    PublishNGStateMsg(ngErrorCode, carrierSns);
                }
                await Task.Delay(200);
            }
        }

        private void PublishNGStateMsg(EN_TrayStatus[,] status, string[] carrierSns)
        {
            _eventAggregator.Publish(new NGStateMessage()
            {
                Status = status,
                CarrierSns = carrierSns,
            }, action => { Task.Run(action); });
        }
    }
}
