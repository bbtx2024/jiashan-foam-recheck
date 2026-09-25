//20250716 刷卡
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using QA.Business.Manager;
using QA_Infrastructure;
using EN_UserType = QA.Business.Manager.EN_UserType;
using MessageBox = HandyControl.Controls.MessageBox;
using UserManager = QA.Business.Manager.UserManager;

namespace QA.UserControls.ViewModels
{
    public class LoginWindowViewModel : Screen, IHandle<BC750CardInfo>
    {
        #region Field

        private IEventAggregator _eventAggregator;
        private UserManager _userManager;
        private ParamManager _paramManager;
        private bool usePreviousUserAfterLogin;
        private EN_UserType needUserType = 0;

        #endregion

        #region Property

        public override string DisplayName { get; set; } = "用户登录";

        private List<User> _users = new List<User>();
        public List<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
                UserIndex = 0;
            }
        }

        private int _userIndex = 0;
        public int UserIndex
        {
            get { return _userIndex; }
            set
            {
                _userIndex = value;
                NotifyOfPropertyChange(() => UserIndex);
            }
        }

        private string _userName = "";
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public bool CommonLoginVisiable { get => _paramManager.OtherSettingParam.EnableCommonLogin; }
        public bool BC750LoginVisiable { get => !_paramManager.OtherSettingParam.EnableCommonLogin; }
        public bool UserManagerVaisiable { get => _userManager.CurrUserType >= EN_UserType.Manager; }

        #endregion

        public LoginWindowViewModel(EN_UserType userType, bool usePreviousUserAfterLogin)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _userManager = IoC.Get<UserManager>();
            _paramManager = IoC.Get<ParamManager>();
            this.usePreviousUserAfterLogin = usePreviousUserAfterLogin;
            Users = _userManager.Users.FindAll(t => string.IsNullOrEmpty(t.CardID) && t.UserType >= userType);
            needUserType = userType;
            Start();
        }

        public async void Start()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (needUserType > 0)
                    {
                        User user0 = _userManager.Users.Find(user => user.CardID == currCardID);
                        if (user0 != null && user0.UserType >= needUserType)
                        {
                            if (!usePreviousUserAfterLogin)
                            {
                                _userManager.Login(currCardID);
                            }
                            TryClose(true);
                            return;
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
            var previousUser = _userManager.CurrUser;
            if (!_userManager.Login(UserName, Password))
            {
                MessageBox.Error("登录失败");
            }
            else
            {
                if (usePreviousUserAfterLogin)
                {
                    _userManager.Login(previousUser.UserName, previousUser.Password);
                }
                TryClose(true);
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoLogin();
            }
        }

        private string currCardID = "";

        public void Handle(BC750CardInfo message)
        {
            currCardID = message.CardID;
        }
    }
}
