//20250716 刷卡
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using QA.Business;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Pages.Interfaces;
using QA.UserControls.ViewModels;
using QA_Infrastructure;
using EN_UserType = QA.Business.Manager.EN_UserType;
using MessageBox = HandyControl.Controls.MessageBox;
using UserManager = QA.Business.Manager.UserManager;

namespace QA.Pages.ViewModels
{
    [Export(typeof(IPageViewModel))]
    public class LoginPageViewModel : Screen, IPageViewModel, IHandle<NeedLoginMessage>, IHandle<LoginSuccessMessage>, IHandle<BC750CardInfo>
    {
        #region Field
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private UserManager _userManager;
        private ParamManager _paramManager;
        private CacheParamManager _cacheParamManager;
        private GlobalVariable _globalVariable;
        private EN_UserType needUserType = 0;
        #endregion

        #region Property
        public ushort OrderID { get; set; } = 5;

        private string _loginUserName = "Operator";
        public string LoginUserName
        {
            get { return _loginUserName; }
            set { _loginUserName = value; NotifyOfPropertyChange(() => LoginUserName); }
        }

        private EN_UserType _loginUserType = EN_UserType.Operator;
        public EN_UserType LoginUserType
        {
            get { return _loginUserType; }
            set { _loginUserType = value; NotifyOfPropertyChange(() => LoginUserType); }
        }

        private List<User> _users = new List<User>();
        public List<User> Users
        {
            get { return _users; }
            set { _users = value; NotifyOfPropertyChange(() => Users); UserIndex = 0; }
        }

        private int _userIndex = 0;
        public int UserIndex
        {
            get { return _userIndex; }
            set { _userIndex = value; NotifyOfPropertyChange(() => UserIndex); }
        }

        private string _userName = "";
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; NotifyOfPropertyChange(() => UserName); }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyOfPropertyChange(() => Password); }
        }

        public bool CommonLoginVisiable { get => _paramManager.OtherSettingParam.EnableCommonLogin; }
        public bool BC750LoginVisiable { get => !_paramManager.OtherSettingParam.EnableCommonLogin; }
        public bool UserManagerVaisiable { get => _userManager.CurrUserType >= EN_UserType.Manager; }

        public bool SelectedLanguage0 { get; set; }
        public bool SelectedLanguage1 { get; set; }
        public bool SelectedLanguage2 { get; set; }
        #endregion

        public LoginPageViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _userManager = IoC.Get<UserManager>();
            _paramManager = IoC.Get<ParamManager>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _globalVariable = IoC.Get<GlobalVariable>();
            Users = _userManager.Users.FindAll(t => string.IsNullOrEmpty(t.CardID));
            //自动切换语言
            SwitchLanguage(_paramManager.OtherSettingParam.LanguageIndex + "");
            Start();
        }

        public async void Start()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (!_userManager.IsLoginDialogShowing)
                    {
                        User user0 = _userManager.Users.Find(user => user.CardID == currCardID);
                        if (user0 != null && user0.UserType >= needUserType)
                        {
                            _userManager.Login(currCardID);
                        }
                        currCardID = null;
                    }
                    NotifyOfPropertyChange(() => CommonLoginVisiable);
                    NotifyOfPropertyChange(() => BC750LoginVisiable);
                    NotifyOfPropertyChange(() => UserManagerVaisiable);
                    await Task.Delay(50);
                }
            });
        }

        public void DoLogin()
        {
            if (!_userManager.Login(UserName, Password))
            {
                MessageBox.Error("账号登录失败");
                return;
            }
            Password = "";
        }

        public void OpenUserManager()
        {
            _windowManager.ShowDialog(new UserManageViewModel());
        }

        public void Handle(NeedLoginMessage message)
        {
            needUserType = message.TargetUserType;
            Users = _userManager.Users.FindAll(t => string.IsNullOrEmpty(t.CardID) && t.UserType >= message.TargetUserType);
            Password = "";
        }

        public void Handle(LoginSuccessMessage message)
        {
            needUserType = 0;
            LoginUserName = _userManager.CurrUserName;
            LoginUserType = _userManager.CurrUserType;
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoLogin();
            }
        }

        public void SwitchLanguage(string languageIndex)
        {
            Collection<ResourceDictionary> appDictionaries = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary selectDic;
            if (languageIndex == "0")
            {
                foreach (ResourceDictionary dict in appDictionaries)
                {
                    if (dict.Source != null && dict.Source.ToString().Contains("Language"))
                    {
                        appDictionaries.Remove(dict);
                        break;
                    }
                }
                selectDic = new ResourceDictionary()
                {
                    Source = new Uri("/QA.IntelligentEquipment;component/Language/zh-cn.xaml", UriKind.RelativeOrAbsolute),
                };
            }
            else if (languageIndex == "1")
            {
                foreach (ResourceDictionary dict in appDictionaries)
                {
                    if (dict.Source != null && dict.Source.ToString().Contains("Language"))
                    {
                        appDictionaries.Remove(dict);
                        break;
                    }
                }
                selectDic = new ResourceDictionary()
                {
                    Source = new Uri("/QA.IntelligentEquipment;component/Language/en.xaml", UriKind.RelativeOrAbsolute),
                };
            }
            else if (languageIndex == "2")
            {
                foreach (ResourceDictionary dict in appDictionaries)
                {
                    if (dict.Source != null && dict.Source.ToString().Contains("Language"))
                    {
                        appDictionaries.Remove(dict);
                        break;
                    }
                }
                selectDic = new ResourceDictionary()
                {
                    Source = new Uri("/QA.IntelligentEquipment;component/Language/vi.xaml", UriKind.RelativeOrAbsolute),
                };
            }
            else
            {
                return;
            }
            appDictionaries.Add(selectDic);
            SelectedLanguage0 = languageIndex == "0";
            SelectedLanguage1 = languageIndex == "1";
            SelectedLanguage2 = languageIndex == "2";
            _cacheParamManager.HomeUiParam.Enable.NotifyAll();
            _paramManager.OtherSettingParam.LanguageIndex = int.Parse(languageIndex);
            _paramManager.SaveParam(_paramManager.OtherSettingParam);
        }

        private string currCardID = "";

        public void Handle(BC750CardInfo message)
        {
            currCardID = message.CardID;
        }
    }
}
