using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Security.Claims;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using HandyControl.Data;
using QA.Business.Component.HIVE;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Station;
using QA.Business.Steps;
using QA_Infrastructure.NLogOut;
using QA_Infrastructure;
using UserManager = QA.Business.Manager.UserManager;//20250716 刷卡
using EN_UserType = QA.Business.Manager.EN_UserType;//20250716 刷卡

namespace QA.SpotCheckPages.ViewModels
{
    [Export("SpotCheckMesPageViewModel", typeof(ISpotPageViewModel))]
    public class SpotCheckMesPageViewModel : Screen, INotifyPropertyChanged, ISpotPageViewModel, IHandle<LoginSuccessMessage>
    {
        #region Field
        private readonly IEventAggregator _eventAggregator;
        private QA.Business.Steps.StepStatus _stepStatus;
        private UserManager _userManager;//20250716 刷卡

        //20250718
        private Hive_Component _hive_Component;
        #endregion

        #region Property

        public ushort OrderID { get; set; } = 0;
        public override string DisplayName { get; set; } = "MES设定";

        private bool _change1 = false;
        public bool Change1
        {
            get => _change1;
            set
            {
                _change1 = value;
                NotifyOfPropertyChange(() => Change1);
            }
        }
        private bool _allow1 = false;
        public bool Allow1
        {
            get => _allow1;
            set
            {
                _allow1 = value;
                NotifyOfPropertyChange(() => Allow1);
            }
        }
        private bool _save1 = false;
        public bool Save1
        {
            get => _save1;
            set
            {
                _save1 = value;
                NotifyOfPropertyChange(() => Save1);
            }
        }


        private string _lineName;
        public string LineName
        {
            get => _lineName;
            set
            {
                _lineName = value;
                NotifyOfPropertyChange(() => LineName);
            }
        }
        private string _stationName;
        public string StationName
        {
            get => _stationName;
            set
            {
                _stationName = value;
                NotifyOfPropertyChange(() => StationName);
            }
        }
        private string _fixid;
        public string Fixid
        {
            get => _fixid;
            set
            {
                _fixid = value;
                NotifyOfPropertyChange(() => Fixid);
            }
        }

        //20250718
        private string _softWareHashValue;
        public string SoftWareHashValue
        {
            get => _softWareHashValue;
            set
            {
                _softWareHashValue = value;
                NotifyOfPropertyChange(() => SoftWareHashValue);
            }
        }

        private string _parameterHashValue;
        public string ParameterHashValue
        {
            get => _parameterHashValue;
            set
            {
                _parameterHashValue = value;
                NotifyOfPropertyChange(() => ParameterHashValue);
            }
        }

        private string _visualHashValue;
        public string VisualHashValue
        {
            get => _visualHashValue;
            set
            {
                _visualHashValue = value;
                NotifyOfPropertyChange(() => VisualHashValue);
            }
        }

        private string _administratorID;
        public string AdministratorID
        {
            get => _administratorID;
            set
            {
                _administratorID = value;
                NotifyOfPropertyChange(() => AdministratorID);
            }
        }

        #endregion

        public SpotCheckMesPageViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _stepStatus = IoC.Get<QA.Business.Steps.StepStatus>();
            _userManager = IoC.Get<UserManager>();//20250716 刷卡

            //20250718
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            ReadParam();
        }

        #region Method

        public void ReadParam()
        {
            LineName = _stepStatus.ParamManager.MESParam.Line;
            StationName = _stepStatus.ParamManager.MESParam.Station;
            Fixid = _stepStatus.ParamManager.MESParam.Fixid;

            //20250718
            SoftWareHashValue = _hive_Component.MainSoftwareSHA1;
            VisualHashValue = _hive_Component.VisionSoftwareSHA1;
            ParameterHashValue = _hive_Component.ConfigFileSHA1;

        }

        public void AllowModifyLeft()
        {
            ReadParam();
            Change1 = true;
            Allow1 = false;
            Save1 = true;
        }

        public void SaveLeftParam()
        {
            _stepStatus.ParamManager.MESParam.Line = LineName;
            //_stepStatus.ParamManager.MESParam.Station = StationName;
            _stepStatus.ParamManager.MESParam.Fixid = Fixid;
            _stepStatus.ParamManager.SaveParam(_stepStatus.ParamManager.MESParam);
            Change1 = false;
            Allow1 = true;
            Save1 = false;
        }

        //20250718
        public void unlock()
        {
            if (LoadWhitelistData(AdministratorID) && _hive_Component._needSendUpdataHash)//属于白名单
            {
                MessageBox.Show("版本已更新！", "操作提示");
                _hive_Component._unlock = true;
                _hive_Component._administratorID = AdministratorID;
            }
            else
            {
                MessageBox.Show($"非白名单人员或当前无版本变更！", "操作提示");
                _hive_Component._unlock = false;
            }
        }

        public void TestUsing()
        {
            string oldOriginalPath = @"D:\Images\RecordImage\202511101\OK\PC-S-LG-LE3-03-31-20241112-0018\1.txt";
            string oldProcessedPath = @"D:\Images\RecordImage\202511101\OK\PC-S-LG-LE3-03-31-20241112-0018\2.jpg";
            string newOriginalPath = @"D:\Images\RecordImage\202511101\OK\PC-S-LG-LE3-03-31-20241112-0018\111.txt";
            string newProcessedPath = @"D:\Images\RecordImage\202511101\OK\PC-S-LG-LE3-03-31-20241112-0018\222.jpg";
            File.Move(oldOriginalPath, newOriginalPath);
            File.Move(oldProcessedPath, newProcessedPath);
            File.Delete(oldOriginalPath);
            File.Delete(oldProcessedPath);
        }

        public bool LoadWhitelistData(string adminID)
        {
            string filePath = @"D:\QKProject\Data\Hive\WhitelistData.csv";

            if (!File.Exists(filePath))
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("白名单人员卡号"); // 写入表头
                }
            }
            var data = new List<string[]>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    data.Add(reader.ReadLine().Split(','));
                }
            }
            foreach (var record in data)
            {
                if (record.Length > 0 && record[0].Equals(adminID))
                {
                    return true;
                }
            }
            return false;
        }

        public void Handle(LoginSuccessMessage message)
        {
            ReadParam();
            Change1 = false;
            Allow1 = _userManager.CurrUserType >= EN_UserType.Manager;//20250716 刷卡
            Save1 = false;
        }

        #endregion
    }
}
