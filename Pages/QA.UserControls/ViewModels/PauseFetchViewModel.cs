using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.Define;
using QA.Business.Steps;
using QA.UserControls.Interfaces;

namespace QA.UserControls.ViewModels
{
    [Export("PauseFetchViewModel", typeof(IUserControl))]
    public class PauseFetchViewModel : Screen, IUserControl
    {
        private StepStatus _stepStatus;
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        public PauseFetchViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
        }

        public void ContinueRun()
        {
            _stepStatus.NextStep2 = EN_RunStep.EmptyStep;
            _stepStatus.AutoResetEvt_DownCam.Set();
        }

        public void Stop()
        {
            _stepStatus.NextStep2 = EN_RunStep.Stop;
            _stepStatus.AutoResetEvt_DownCam.Set();
        }

        protected override void OnActivate()
        {

            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            _stepStatus.NextStep2 = EN_RunStep.Stop;
            _stepStatus.AutoResetEvt_DownCam.Set();
            this.TryClose();
            base.OnDeactivate(close);

        }
    }
}
