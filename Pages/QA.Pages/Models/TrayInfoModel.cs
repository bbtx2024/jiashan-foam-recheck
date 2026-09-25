using System;
using System.ComponentModel;
using System.Linq.Expressions;
using QA.Business.Converts;
using QA.Business.Define;

namespace QA.Pages.Models
{
    public class TrayInfoModel : INotifyPropertyChanged
    {
        #region Property Notify

        public event PropertyChangedEventHandler PropertyChanged;
        public void ChangeProperty(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public void ChangeProperty<T>(Expression<Func<T>> expression)
        {
            MemberExpression member = (MemberExpression)expression.Body;
            string propName = member.Member.Name;
            ChangeProperty(propName);
        }

        #endregion


        private bool _isUsed;
        public bool IsUsed
        {
            get { return _isUsed; }
            set
            {
                _isUsed = value;
                ChangeProperty(() => IsUsed);
                ChangeProperty(() => CavUseInfo);
                Status = _isUsed ? EN_TrayStatus.待料 : EN_TrayStatus.禁用;
            }
        }
        private int _cavityNum;
        public int CavityNum
        {
            get { return _cavityNum; }
            set
            {
                _cavityNum = value;
                ChangeProperty(() => CavityNum);
                ChangeProperty(() => CavUseInfo);
            }
        }
        public string CavStr
        {
            get
            {
                return CavityNum + "穴";
            }
        }

        public string CavUseInfo
        {
            get { return CavityNum + "穴" + (IsUsed ? "启用" : "禁用"); }
        }

        private EN_TrayStatus _status;
        public EN_TrayStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                ChangeProperty(() => Status);
                ChangeProperty(() => CavStateInfo);
            }
        }

        private static readonly TrayStatusToStringConvert trayStatusToStringConvert = new TrayStatusToStringConvert();
        public string CavStateInfo
        {
            get => (string)trayStatusToStringConvert.Convert(Status, null, null, null);
        }

        private string _str1;
        public string Str1
        {
            get { return _str1; }
            set
            {
                _str1 = value;
                ChangeProperty(() => Str1);
            }
        }

        private string _str2;
        public string Str2
        {
            get { return _str2; }
            set
            {
                _str2 = value;
                ChangeProperty(() => Str2);
            }
        }
    }
}
