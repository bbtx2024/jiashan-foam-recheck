using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.CacheParam;
using QA.Business.Manager;

namespace QA.UserControls.ViewModels
{
    public class NozzleManageViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly CacheParamManager _cacheParamManager;

        public override string DisplayName { get; set; } = "喷咀管理";

        public HomeUiParam HomeUiParam { get; set; }

        public NozzleManageParam NozzleManageParam { get; set; }

        [ImportingConstructor]
        public NozzleManageViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _cacheParamManager = IoC.Get<CacheParamManager>();

            HomeUiParam = _cacheParamManager.HomeUiParam;
            NozzleManageParam = _cacheParamManager.HomeUiParam.NozeleManageParam;
        }

        public void Confirm()
        {
            //Random rd = new Random();
            //NozzleManageParam.NozzleSN = rd.Next(1, 100).ToString();
            //NozzleManageParam.NozzleChangeTime = rd.Next(1, 100).ToString();
            //NozzleManageParam.TinBallSN = rd.Next(1, 100).ToString();
            //NozzleManageParam.TinBallOpenTime = rd.Next(1, 100).ToString();
            //NozzleManageParam.TinBallChangeTime = rd.Next(1, 100).ToString();
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }

    }
}
