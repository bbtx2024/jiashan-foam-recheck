using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using QA.Business.CacheParam;
using QA.Business.Define;
using QA.Business.Manager;

namespace QA.Pages.OtherViews.ViewModels
{
    [Export("StatisticViewModel")]
    public class StatisticViewModel : Screen
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private CacheParamManager _cacheParamManager;
        private ResourceDictionary _languageDic;
        #endregion

        #region Property
        public HomeUiParam_Statistic Statistic { get; private set; }

        public string[] TranslatedStrs { get; set; }
        #endregion

        public StatisticViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _cacheParamManager = IoC.Get<CacheParamManager>();
            Statistic = _cacheParamManager.HomeUiParam.Statistic;
            _languageDic = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(t => t.Source != null && t.Source.ToString().Contains("Language"));
            string tape1Name = "";
            TranslatedStrs = new[]{
                TransLate((EN_TrayStatus.MES上传NG).ToString()),
                TransLate((EN_TrayStatus.Tape缺失).ToString()),
                TransLate((EN_TrayStatus.Tape大离型纸未撕).ToString()),
                TransLate((EN_TrayStatus.Tape贴装偏移).ToString()),
                TransLate((EN_TrayStatus.Mark定位失败).ToString()),
                TransLate((EN_TrayStatus.Tape抓边失败).ToString()),
                TransLate((EN_TrayStatus.排线SN扫码失败).ToString()),
            };
        }

        private string TransLate(string s)
        {
            if (!_languageDic.Contains(s))
            {
                return s;
            }
            string ret = (string)_languageDic[s];
            if (string.IsNullOrEmpty(ret))
            {
                return s;
            }
            return ret;
        }
    }
}
