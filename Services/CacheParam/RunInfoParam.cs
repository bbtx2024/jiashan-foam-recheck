using System;
using System.ComponentModel;
using QA_Infrastructure;

namespace QA.Business.CacheParam
{
    [SaveParam(FileType.JSON)]
    [Serializable]
    public class RunInfoParam : INotifyPropertyChanged
    {
        #region Property Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void ChangeProperty(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private int _totalSolderCount = 0;
        public int TotalSolderCount
        {
            get { return _totalSolderCount; }
            set { _totalSolderCount = value; ChangeProperty("TotalSolderCount"); }
        }

        //public TimeSpan LastTimeSpan { get; set; }

        public string CurTaskName { get; set; }

        public bool SwitchLanguage { get; set; }

        public string LastAllTimeSpan { get; set; } = string.Empty;

    }
}
