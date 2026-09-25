using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.CacheParam;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Station;

namespace QA.Pages.OtherViews.ViewModels
{
    [Export("FunctionIsEnabledViewModel")]
    public class FunctionIsEnabledViewModel : Screen, IHandle<List<IComponent>>
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private CacheParamManager _cacheParamManager;
        private IBaseBiz _baseBiz;
        #endregion

        #region Property
        public HomeUiParam_Enable Enable { get => _cacheParamManager.HomeUiParam.Enable; }

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
        public FunctionIsEnabledViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _baseBiz = IoC.Get<IBaseBiz>();
        }
        #endregion

        #region Handle
        public void Handle(List<IComponent> message)
        {
            EnableButtons = !(_baseBiz as BaseBiz).AutoRun;
        }
        #endregion

        #region Method
        public void SwitchUseNozzle1()
        {
            Enable.UseNozzle1 = !Enable.UseNozzle1;
            _cacheParamManager.SaveHomeUiParam();
        }

        public void SwitchUseNozzle2()
        {
            Enable.UseNozzle2 = !Enable.UseNozzle2;
            _cacheParamManager.SaveHomeUiParam();
        }

        public void SwitchUseNozzle3()
        {
            Enable.UseNozzle3 = !Enable.UseNozzle3;
            _cacheParamManager.SaveHomeUiParam();
        }

        public void SwitchUseNozzle4()
        {
            Enable.UseNozzle4 = !Enable.UseNozzle4;
            _cacheParamManager.SaveHomeUiParam();
        }

        public void SwitchUseVacSucCheck()
        {
            Enable.UseVacSucCheck = !Enable.UseVacSucCheck;
            _cacheParamManager.SaveHomeUiParam();
        }
        #endregion
    }
}
