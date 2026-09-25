using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;
using QA.Business.Define;
using QA.Business.Interfaces;

namespace QA.Business.Steps
{
    public class StepFactory
    {
        private IDapperHelper _dataHelper;
        private Dictionary<EN_RunStep, IStepStation1> _stepHandlerDictionary1 = new Dictionary<EN_RunStep, IStepStation1>();
        private Dictionary<EN_RunStep, IStepStation2> _stepHandlerDictionary2 = new Dictionary<EN_RunStep, IStepStation2>();
        private Dictionary<EN_RunStep, IStepStation3> _stepHandlerDictionary3 = new Dictionary<EN_RunStep, IStepStation3>();

        public StepFactory()
        {
            _dataHelper = IoC.Get<IDapperHelper>();

            var name = Assembly.GetExecutingAssembly().ToString();
            var stepTypeList1 = _dataHelper?.GenerateManager<IStepStation1>(name);
            var stepTypeList2 = _dataHelper?.GenerateManager<IStepStation2>(name);
            var stepTypeList3 = _dataHelper?.GenerateManager<IStepStation3>(name);
            //如果下面循环堆栈越界，可能是循环调用BaseBiz
            foreach (var stepType in stepTypeList1)
            {
                if (Activator.CreateInstance(stepType) is IStepStation1 instance)
                    _stepHandlerDictionary1.Add(instance.RunStep, instance);
            }
            foreach (var stepType in stepTypeList2)
            {
                if (Activator.CreateInstance(stepType) is IStepStation2 instance)
                    _stepHandlerDictionary2.Add(instance.RunStep, instance);
            }
            foreach (var stepType in stepTypeList3)
            {
                if (Activator.CreateInstance(stepType) is IStepStation3 instance)
                    _stepHandlerDictionary3.Add(instance.RunStep, instance);
            }
        }

        public Dictionary<EN_RunStep, IStepStation1> GetStepHandlerDic1()
        {
            return _stepHandlerDictionary1;
        }
        public Dictionary<EN_RunStep, IStepStation2> GetStepHandlerDic2()
        {
            return _stepHandlerDictionary2;
        }
        public Dictionary<EN_RunStep, IStepStation3> GetStepHandlerDic3()
        {
            return _stepHandlerDictionary3;
        }
    }
}
