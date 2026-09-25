//20250716 刷卡
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using HandyControl.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QA.Business.Message;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Manager
{
    public enum EN_UserType
    {
        None,
        Operator,
        Engineer,
        Manager,
    }

    public class User
    {
        public EN_UserType UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CardID { get; set; }
    }

    public class UserManager
    {
        #region 用户信息读取存储

        public string fileOld = "C:\\ProgramData\\Quick\\QuickSec";
        public string file = "C:\\ProgramData\\Quick\\UserInfo.json";
        public List<User> Users { get; set; } = new List<User>();

        public void LoadUsers()
        {
            Users.Clear();
            //旧用户信息和新的有一定冲突，尽量尝试读取
            if (!File.Exists(file) && File.Exists(fileOld))
            {
                File.Copy(fileOld, file);
            }
            if (File.Exists(file))
            {
                try
                {
                    using (StreamReader sr = File.OpenText(file))
                    {
                        JArray arr = (JArray)JToken.ReadFrom(new JsonTextReader(sr));
                        foreach (JObject obj in arr)
                        {
                            if (obj.ContainsKey("UserType")
                                && obj.ContainsKey("UserName")
                                && obj.ContainsKey("Password")
                                && obj.ContainsKey("CardID"))
                            {
                                Enum.TryParse(obj["UserType"].ToString(), out EN_UserType userType);
                                AddUser(userType, obj["UserName"].ToString(), obj["Password"].ToString(),
                                    obj["CardID"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
            //检测默认的三个用户是否存在，不存在则添加对应用户
            if (Users.Find(user => user.UserType == EN_UserType.Operator
                                   && user.UserName == EN_UserType.Operator.ToString()) == null)
            {
                AddUser(EN_UserType.Operator, "Operator", "1", "");
            }
            if (Users.Find(user => user.UserType == EN_UserType.Engineer
                                   && user.UserName == EN_UserType.Engineer.ToString()) == null)
            {
                AddUser(EN_UserType.Engineer, "Engineer", "1", "");
            }
            if (Users.Find(user => user.UserType == EN_UserType.Manager
                                   && user.UserName == EN_UserType.Manager.ToString()) == null)
            {
                AddUser(EN_UserType.Manager, "Manager", "1", "");
            }
            SaveUsers();
        }

        public void SaveUsers()
        {
            JArray arr = new JArray();
            foreach (var user in Users)
            {
                JObject obj = new JObject();
                obj.Add("UserType", user.UserType.ToString());
                obj.Add("UserName", user.UserName.ToString());
                obj.Add("Password", user.Password.ToString());
                obj.Add("CardID", user.CardID.ToString());
                arr.Add(obj);
            }
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(arr.ToString(Formatting.Indented));
            }
        }

        public void AddUser(EN_UserType userType, string userName, string password, string cardID, bool save = false)
        {
            Users.Add(new User()
            {
                UserType = userType,
                UserName = userName,
                Password = password,
                CardID = cardID,
            });
            if (save)
            {
                SaveUsers();
            }
        }

        public void ModifyUser(User user, EN_UserType userType, string userName, string password, string cardID)
        {
            user.UserType = userType;
            user.UserName = userName;
            user.Password = password;
            user.CardID = cardID;
            SaveUsers();
        }

        public void DeleteUser(string userName)
        {
            Users.Remove(Users.Find(user => user.UserName == userName));
            SaveUsers();
        }

        public void ResetAll()
        {
            Users.Clear();
            AddUser(EN_UserType.Operator, "Operator", "1", "");
            AddUser(EN_UserType.Engineer, "Engineer", "1", "");
            AddUser(EN_UserType.Manager, "Manager", "1", "");
            SaveUsers();
        }

        #endregion

        #region 用户登录

        public User CurrUser { get; set; }
        public string CurrUserName => CurrUser == null ? "未登录" : CurrUser.UserName;
        public EN_UserType CurrUserType => CurrUser == null ? EN_UserType.None : CurrUser.UserType;
        public List<EN_UserType> UserTypes { get; set; } = new List<EN_UserType>();

        public bool Login(string cardID)
        {
            User user0 = Users.Find(user => user.CardID == cardID);
            if (user0 == null)
            {
                return false;
            }
            CurrUser = user0;
            NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{CurrUser.UserName}已登录", En_Logout_Type.SystemParam, true);
            PublishLoginSuccessMessage();
            return true;
        }

        public bool Login(string userName, string password)
        {
            User user0 = Users.Find(user => user.UserName == userName && user.Password == password);
            if (user0 == null)
            {
                return false;
            }
            CurrUser = user0;
            NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{CurrUser.UserName}已登录", En_Logout_Type.SystemParam, true);
            PublishLoginSuccessMessage();
            return true;
        }

        public void UnLogin()
        {
            if (CurrUser != null)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{CurrUser.UserName}已注销", En_Logout_Type.SystemParam, true);
                CurrUser = null;
                PublishLoginSuccessMessage();
            }
        }

        public void PublishLoginSuccessMessage()
        {
            _eventAggregator.Publish(new LoginSuccessMessage() { CurrUser = CurrUser }, action => { Task.Run(action); });
        }

        #endregion

        private readonly IEventAggregator _eventAggregator;
        public bool IsLoginDialogShowing = false;

        public UserManager()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            foreach (var type in Enum.GetValues(typeof(EN_UserType)))
            {
                if ((EN_UserType)type != EN_UserType.None)
                {
                    UserTypes.Add((EN_UserType)type);
                }
            }
            LoadUsers();
        }
    }
}
