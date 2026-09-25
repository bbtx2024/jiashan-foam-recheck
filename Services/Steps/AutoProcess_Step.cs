using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;

namespace QA.Business.Steps
{
    public class AutoProcess_Step : IStepStation2
    {
        StepStatus _stepStatus;
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.AutoProcess;

        public AutoProcess_Step()
        {
            _stepStatus = IoC.Get<StepStatus>();
        }

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            await Task.Delay(100);
            var plc = components.FirstOrDefault(s => s is PLC_Component) as PLC_Component;
            if (plc.IsScanReadyRecheck() || plc.IsPlcCameraReady())
            {
                return (false, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
            return (false, EN_RunRet.TaskOk, _stepStatus.NextStep2);
        }
    }
    public class AutoProcess_StepNo1 : IStepStation1
    {
        StepStatus _stepStatus;
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.AutoProcess;
        public AutoProcess_StepNo1()
        {
            _stepStatus = IoC.Get<StepStatus>();
        }
        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            await Task.Delay(100);
            var plc = components.FirstOrDefault(s => s is PLC_Component) as PLC_Component;
            if (plc.IsScanReady())
            {
                return (false, EN_RunRet.TaskOk, EN_RunStep.ScannerCarrierSnStep);
            }
            return (false, EN_RunRet.TaskOk, _stepStatus.NextStep1);
        }
    }
}
