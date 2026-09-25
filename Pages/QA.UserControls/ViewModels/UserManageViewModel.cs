//20250716 刷卡
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Manager;
using MessageBox = HandyControl.Controls.MessageBox;
using UserManager = QA.Business.Manager.UserManager;

namespace QA.UserControls.ViewModels
{
    public class UserManageViewModel : Screen, IHandle<BC750CardInfo>
    {
        private IEventAggregator _eventAggregator;
        private UserManager _userManager;
        public override string DisplayName { get; set; } = "用户管理";

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        private User _sltUser;
        public User SltUser
        {
            get { return _sltUser; }
            set
            {
                if (value != null)
                {
                    _sltUser = value;
                    NotifyOfPropertyChange(() => SltUser);
                }
            }
        }

        public List<EN_UserType> UserTypes => _userManager.UserTypes;

        private EN_UserType _currUserType = EN_UserType.None;
        public EN_UserType CurrUserType
        {
            get { return _currUserType; }
            set
            {
                _currUserType = value;
                NotifyOfPropertyChange(() => CurrUserType);
            }
        }

        private string _currUserName = "";
        public string CurrUserName
        {
            get { return _currUserName; }
            set
            {
                _currUserName = value;
                NotifyOfPropertyChange(() => CurrUserName);
            }
        }

        private string _currPassword = "";
        public string CurrPassword
        {
            get { return _currPassword; }
            set
            {
                _currPassword = value;
                NotifyOfPropertyChange(() => CurrPassword);
            }
        }

        private string _currCardID = "";
        public string CurrCardID
        {
            get { return _currCardID; }
            set
            {
                _currCardID = value;
                NotifyOfPropertyChange(() => CurrCardID);
            }
        }

        [ImportingConstructor]
        public UserManageViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _userManager = IoC.Get<UserManager>();
            UpdateUserInfos();
        }

        /// <summary>
        /// 刷新所有用户信息
        /// </summary>
        public void UpdateUserInfos()
        {
            Users.Clear();
            foreach (User user in _userManager.Users)
            {
                Users.Add(user);
            }
            SltUser = Users[0];
        }

        public void SelectUserChange()
        {
            CurrUserType = SltUser.UserType;
            CurrUserName = SltUser.UserName;
            CurrPassword = SltUser.Password;
            CurrCardID = SltUser.CardID;
        }

        /// <summary>
        /// 添加账号
        /// </summary>
        public void AddUser()
        {
            if (CurrUserName == "")
            {
                MessageBox.Warning("用户名不能为空！");
                return;
            }
            if (CurrPassword == "" && CurrCardID == "")
            {
                MessageBox.Warning("密码或卡号必须输入一个！");
                return;
            }
            if (CurrPassword != "" && CurrCardID != "")
            {
                MessageBox.Warning("密码或卡号只能存在一个！");
                return;
            }
            _userManager.AddUser(CurrUserType, CurrUserName, CurrPassword, CurrCardID, true);
            UpdateUserInfos();
        }

        /// <summary>
        /// 修改账号
        /// </summary>
        public void ModifyUser()
        {
            if (CurrUserName == "")
            {
                MessageBox.Warning("用户名不能为空！");
                return;
            }
            if (SltUser == null)
            {
                MessageBox.Warning("需要先选中要修改的用户！");
                return;
            }
            if (CurrPassword == "" && CurrCardID == "")
            {
                MessageBox.Warning("密码或卡号必须输入一个！");
                return;
            }
            if (CurrPassword != "" && CurrCardID != "")
            {
                MessageBox.Warning("密码或卡号只能存在一个！");
                return;
            }
            _userManager.ModifyUser(SltUser, CurrUserType, CurrUserName, CurrPassword, CurrCardID);
            UpdateUserInfos();
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        public void DeleteUser()
        {
            if (MessageBox.Ask($"确定删除{SltUser.UserName}吗？") == MessageBoxResult.OK)
            {
                _userManager.DeleteUser(SltUser.UserName);
                UpdateUserInfos();
            }
        }

        /// <summary>
        /// 重置所有用户
        /// </summary>
        public void ResetAll()
        {
            _userManager.ResetAll();
            UpdateUserInfos();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            this.TryClose();
        }

        public void Handle(BC750CardInfo message)
        {
            CurrCardID = message.CardID;
        }
    }
}
