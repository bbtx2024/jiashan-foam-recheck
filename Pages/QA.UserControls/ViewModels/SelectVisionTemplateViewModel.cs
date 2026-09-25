using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace QA.UserControls.ViewModels
{
    public class SelectVisionTemplateViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        public List<VisionTemplateSelectInfo> TemplateInfos { get; set; } = new List<VisionTemplateSelectInfo>();

        public VisionTemplateSelectInfo CurrTemplateInfo { get; set; } = null;

        public SelectVisionTemplateViewModel(List<string> templateNames)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();

            foreach (string name in templateNames)
            {
                TemplateInfos.Add(new VisionTemplateSelectInfo() { TemplateName = name });
            }
        }

        public void MouseLeaveAction()
        {
            this.TryClose();
        }

        public void CurTemplateInfoChanged()
        {
            _eventAggregator.Publish(CurrTemplateInfo, action => { Task.Run(action); });
            TryClose();
        }
    }

    public class VisionTemplateSelectInfo
    {
        public string TemplateName { get; set; } = null;
    }
}
