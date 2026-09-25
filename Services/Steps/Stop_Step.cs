using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Define;
using QA.Business.Interfaces;

namespace QA.Business.Steps
{
    public class Stop_Step : IStepStation2
    {
        private StepStatus _stepStatus;
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.Stop;

        public Stop_Step()
        {
            _stepStatus = IoC.Get<StepStatus>();
        }

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            await Task.Delay(0);
            return (false, EN_RunRet.TaskOk, EN_RunStep.Stop);
        }
    }
    public class Stop_StepNo1 : IStepStation1
    {
        private StepStatus _stepStatus;
        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.Stop;
        public Stop_StepNo1()
        {
            _stepStatus = IoC.Get<StepStatus>();
        }
        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {
            await Task.Delay(0);

            if (_stepStatus.NextStep1 != RunStep)
            {
                return (false, EN_RunRet.TaskOk, _stepStatus.NextStep1);
            }
            return (false, EN_RunRet.TaskOk, EN_RunStep.Stop);
        }
    }
}
