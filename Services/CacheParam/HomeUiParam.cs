using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using QA.Business.Define;
using QA.Business.Manager;
using QA_Infrastructure;

namespace QA.Business.CacheParam
{
    [SaveParam(FileType.JSON)]
    [Serializable]
    public class HomeUiParam : INotifyPropertyChanged
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

        /// <summary>
        /// 产量统计
        /// </summary>
        public HomeUiParam_Statistic Statistic { get; set; } = new HomeUiParam_Statistic();

        /// <summary>
        /// 功能启用
        /// </summary>
        public HomeUiParam_Enable Enable { get; set; } = new HomeUiParam_Enable();

        /// <summary>
        /// 喷咀管理参数
        /// </summary>
        public NozzleManageParam NozeleManageParam { get; set; } = new NozzleManageParam();

        /// <summary>
        /// 锡球管理参数
        /// </summary>
        public TinBallManageParam TinBallManageParam { get; set; } = new TinBallManageParam();
    }

    public class HomeUiParam_Statistic : INotifyPropertyChanged
    {
        #region NotifyOfPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            MemberExpression member = (MemberExpression)property.Body;
            string propName = member.Member.Name;
            NotifyOfPropertyChange(propName);
        }
        #endregion

        private int _currentNum;
        public int CurrentNum
        {
            get { return _currentNum; }
            set { _currentNum = value; NotifyOfPropertyChange("CurrentNum"); }
        }

        private int _targetNum;
        public int TargetNum
        {
            get { return _targetNum; }
            set { _targetNum = value; NotifyOfPropertyChange("TargetNum"); }
        }

        private int _setTargetNum;
        public int SetTargetNum
        {
            get { return _setTargetNum; }
            set { _setTargetNum = value; NotifyOfPropertyChange("SetTargetNum"); }
        }

        private void ChangeAll()
        {
            string[] s1 = { "ErrorCodeCount", "OkCount", "NgCount", "ProductCount", "OkRate"};
            foreach (var ss1 in s1)
            {
                NotifyOfPropertyChange(ss1);
            }
        }
        private void ChangeScanner()
        {
            string[] s1 = {"ScannerOKRate", "ScannerAllCount", };
            foreach (var ss1 in s1)
            {
                NotifyOfPropertyChange(ss1);
            }
        }

        private int[] _errorCodeCount = new int[(int)EN_TrayStatus.禁用];
        public int[] ErrorCodeCount
        {
            get { return _errorCodeCount; }
            set
            {
                //_errorCodeCount = value;
                for (int i = 0; i < Math.Min(_errorCodeCount.Length, value.Length); i++)
                {
                    _errorCodeCount[i] = value[i];
                }
                ChangeAll();
            }
        }
        //ScannerCount[0]代表扫码枪扫码OK数，ScannerCount[1]代表扫码枪扫码总数
        private int[] _scannerCount = new int[10];
        public int[] ScannerCount
        {
            get { return _scannerCount; }
            set
            {
                //_errorCodeCount = value;
                for (int i = 0; i < Math.Min(_scannerCount.Length, value.Length); i++)
                {
                    _scannerCount[i] = value[i];
                }
                ChangeAll();
            }
        }

        public int OkCount
        {
            get { return ErrorCodeCount[(int)EN_TrayStatus.OK]; }
        }

        public int ScannerOkCount
        {
            get { return ScannerCount[0]; }
        }

        public int ScannerAllCount
        {
            get { return ScannerCount[1]; }
        }


        public int NgCount
        {
            get
            {
                int ret = 0;
                for (int i = (int)EN_TrayStatus.OK + 1; i < ErrorCodeCount.Length; i++)
                {
                    ret += ErrorCodeCount[i];
                }
                return ret;
            }
        }

        public int ProductCount
        {
            get { return OkCount + NgCount; }
        }

        public float OkRate
        {
            get => ProductCount == 0 ? 1 : (float)OkCount / ProductCount;
        }

        public float ScannerOKRate        {
            get => ScannerAllCount == 0 ? 1 : (float)ScannerOkCount / ScannerAllCount;
        }

        public void AddErrorCodeCount(int errorCode)
        {
            ErrorCodeCount[errorCode]++;
            ChangeAll();
        }
        /// <summary>
        /// type:0代表扫码枪扫码OK数，1代表扫码枪扫码总数
        /// </summary>
        /// <param name="type">0代表扫码枪扫码OK数，1代表扫码枪扫码总数</param>
        public void AddScannerCount(int type)
        {
            ScannerCount[type]++;
            ChangeScanner();
        }

        public void ResetCount()
        {
            for (int i = 0; i < ErrorCodeCount.Length; i++)
            {
                ErrorCodeCount[i] = 0;
            }
            for (int i = 0; i < ScannerCount.Length; i++)
            {
                ScannerCount[i] = 0;
            }
            ChangeAll();
            ChangeScanner();
        }

        private void ChangeAllUph()
        {
            for (int i = 0; i < 24; i++)
            {
                NotifyOfPropertyChange("Uph" + i);
            }
        }

        public int[] _uph = new int[24];
        /// <summary>
        /// 8H-9H对应的产能
        /// </summary>
        public int Uph0
        {
            get { return _uph[0]; }
            set { _uph[0] = value; ChangeAllUph(); }
        }
        public int Uph1
        {
            get { return _uph[1]; }
            set { _uph[1] = value; ChangeAllUph(); }
        }
        public int Uph2
        {
            get { return _uph[2]; }
            set { _uph[2] = value; ChangeAllUph(); }
        }
        public int Uph3
        {
            get { return _uph[3]; }
            set { _uph[3] = value; ChangeAllUph(); }
        }
        public int Uph4
        {
            get { return _uph[4]; }
            set { _uph[4] = value; ChangeAllUph(); }
        }
        public int Uph5
        {
            get { return _uph[5]; }
            set { _uph[5] = value; ChangeAllUph(); }
        }
        public int Uph6
        {
            get { return _uph[6]; }
            set { _uph[6] = value; ChangeAllUph(); }
        }
        public int Uph7
        {
            get { return _uph[7]; }
            set { _uph[7] = value; ChangeAllUph(); }
        }
        public int Uph8
        {
            get { return _uph[8]; }
            set { _uph[8] = value; ChangeAllUph(); }
        }
        public int Uph9
        {
            get { return _uph[9]; }
            set { _uph[9] = value; ChangeAllUph(); }
        }
        public int Uph10
        {
            get { return _uph[10]; }
            set { _uph[10] = value; ChangeAllUph(); }
        }
        public int Uph11
        {
            get { return _uph[11]; }
            set { _uph[11] = value; ChangeAllUph(); }
        }
        public int Uph12
        {
            get { return _uph[12]; }
            set { _uph[12] = value; ChangeAllUph(); }
        }
        public int Uph13
        {
            get { return _uph[13]; }
            set { _uph[13] = value; ChangeAllUph(); }
        }
        public int Uph14
        {
            get { return _uph[14]; }
            set { _uph[14] = value; ChangeAllUph(); }
        }
        public int Uph15
        {
            get { return _uph[15]; }
            set { _uph[15] = value; ChangeAllUph(); }
        }
        public int Uph16
        {
            get { return _uph[16]; }
            set { _uph[16] = value; ChangeAllUph(); }
        }
        public int Uph17
        {
            get { return _uph[17]; }
            set { _uph[17] = value; ChangeAllUph(); }
        }
        public int Uph18
        {
            get { return _uph[18]; }
            set { _uph[18] = value; ChangeAllUph(); }
        }
        public int Uph19
        {
            get { return _uph[19]; }
            set { _uph[19] = value; ChangeAllUph(); }
        }
        public int Uph20
        {
            get { return _uph[20]; }
            set { _uph[20] = value; ChangeAllUph(); }
        }
        public int Uph21
        {
            get { return _uph[21]; }
            set { _uph[21] = value; ChangeAllUph(); }
        }
        public int Uph22
        {
            get { return _uph[22]; }
            set { _uph[22] = value; ChangeAllUph(); }
        }
        public int Uph23
        {
            get { return _uph[23]; }
            set { _uph[23] = value; ChangeAllUph(); }
        }

        /// <summary>
        /// _uph数组对应的时间起点，即_uph[0]对应的某一天的8H
        /// </summary>
        public DateTime CurrUphStartTime { get; set; } = DateTime.MinValue;

        public void AddUph()
        {
            ResetUphAndSaveDataIfNeed();
            int hour = DateTime.Now.Hour;
            _uph[(hour + 16) % 24]++;
            ChangeAllUph();

            CurrentNum++;
        }

        private object uphLock = new object();

        /// <summary>
        /// 判断是否需要刷新uph数据。
        /// 如果是，保存当前uph数据，并清零uph数据，修改主界面显示。
        /// </summary>
        public void ResetUphAndSaveDataIfNeed()
        {
            lock (uphLock)
            {
                // 找到上一个8H
                DateTime currTime = DateTime.Now;
                DateTime previous8H = currTime.Date.AddHours(8);
                if (currTime < previous8H)
                {
                    previous8H = previous8H.AddDays(-1);
                }
                // 如果与CurrUphStartTime不匹配，说明需要更新
                if (previous8H == CurrUphStartTime)
                {
                    return;
                }
                try
                {
                    string dir0 = $@"D:\CsvData\产能数据";
                    if (!Directory.Exists(dir0))
                    {
                        Directory.CreateDirectory(dir0);
                    }
                    string file = $@"{dir0}\产能数据_{CurrUphStartTime.ToString("yyyyMMddHH")}.csv";
                    using (StreamWriter sw = new StreamWriter(file, false))
                    {
                        DateTime previous0H = CurrUphStartTime.Date;
                        sw.WriteLine("开始时间,结束时间,产能数据");
                        for (int i = 0; i < _uph.Length; i++)
                        {
                            DateTime timeStart = CurrUphStartTime.AddHours(i);
                            DateTime timeEnd = CurrUphStartTime.AddHours(i + 1);
                            sw.WriteLine(timeStart.ToString("yyyy-MM-dd HH:mm:ss") + ","
                                + timeEnd.ToString("yyyy-MM-dd HH:mm:ss") + ","
                                + _uph[i]);
                        }
                    }
                }
                catch (Exception e)
                {
                    //ignore
                }
                _uph = new int[24];
                CurrUphStartTime = previous8H;
                ChangeAllUph();
            }
        }

        public void ResetUph()
        {
            _uph = new int[24];
        }
        private int _runTimeMinute = 0;
        public int RunTimeMinute
        {
            get => _runTimeMinute;
            set
            {
                _runTimeMinute = value;
                NotifyOfPropertyChange(() => RunTimeMinute);
                NotifyOfPropertyChange(() => RunTimeStr);
            }
        }
        public string RunTimeStr { get => _runTimeMinute / 60 + " 时 " + _runTimeMinute % 60 + " 分"; }

        private int _idleTimeMinute = 0;
        public int IdleTimeMinute
        {
            get => _idleTimeMinute;
            set
            {
                _idleTimeMinute = value;
                NotifyOfPropertyChange(() => IdleTimeMinute);
                NotifyOfPropertyChange(() => IdleTimeStr);
            }
        }
        public string IdleTimeStr { get => _idleTimeMinute / 60 + " 时 " + _idleTimeMinute % 60 + " 分"; }

        private int _errTimeMinute = 0;
        public int ErrTimeMinute
        {
            get => _errTimeMinute;
            set
            {
                _errTimeMinute = value;
                NotifyOfPropertyChange(() => ErrTimeMinute);
                NotifyOfPropertyChange(() => ErrTimeStr);
            }
        }
        public string ErrTimeStr { get => _errTimeMinute / 60 + " 时 " + _errTimeMinute % 60 + " 分"; }

        private int _alarmCount = 0;
        public int AlarmCount
        {
            get => _alarmCount;
            set
            {
                _alarmCount = value;
                NotifyOfPropertyChange(() => AlarmCount);
                NotifyOfPropertyChange(() => AlarmCountStr);
            }
        }
        public string AlarmCountStr { get => _alarmCount + " 次"; }


        public List<string> SoftwareHashList { get; set; } = new List<string>();

        private int _currSoftwareVersion = 0;
        public int CurrSoftwareVersion
        {
            get => _currSoftwareVersion;
            set { _currSoftwareVersion = value; NotifyOfPropertyChange(() => CurrSoftwareVersion); }
        }

        ////20250717 一楼版本
        //public void AddSoftwareVersionIfNeed(string currHash, out bool needUpdateLastTime)
        //{
        //    if (!SoftwareHashList.Contains(currHash))
        //    {
        //        SoftwareHashList.Add(currHash);
        //        needUpdateLastTime = true;
        //    }
        //    else
        //    {
        //        needUpdateLastTime = false;
        //    }
        //    CurrSoftwareVersion = SoftwareHashList.IndexOf(currHash) + 1;
        //}

        //20250718 二楼版本
        public void AddSoftwareVersionIfNeed(string currHash)
        {
            if (!SoftwareHashList.Contains(currHash))
            {
                SoftwareHashList.Add(currHash);
            }
            CurrSoftwareVersion = SoftwareHashList.IndexOf(currHash) + 1;
        }

        //20250718 二楼版本 检查hash是否新增
        public bool CheckHash(string currHash)
        {
            if (!SoftwareHashList.Contains(currHash))
            {
                return true;//不包含，则说明是新的hash
            }
            return false;
        }
    }

    public class HomeUiParam_Enable : INotifyPropertyChanged
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

        public void NotifyAll()
        {
            UseMes = UseMes;
            PrepareInAdvance = PrepareInAdvance;
            UseNozzle1 = UseNozzle1;
            UseNozzle2 = UseNozzle2;
            UseNozzle3 = UseNozzle3;
            UseNozzle4 = UseNozzle4;
            UseVacSucCheck = UseVacSucCheck;
        }

        private bool _useMes = true;
        public bool UseMes
        {
            get { return _useMes; }
            set { _useMes = value; ChangeProperty("UseMes"); }
        }

        /// <summary>
        /// 提前取料
        /// </summary>
        private bool _prepareInAdvance = true;
        public bool PrepareInAdvance
        {
            get { return _prepareInAdvance; }
            set { _prepareInAdvance = value; ChangeProperty("PrepareInAdvance"); }
        }

        private bool _isUseNozzleNo1 = true;
        public bool UseNozzle1
        {
            get { return _isUseNozzleNo1; }
            set { _isUseNozzleNo1 = value; ChangeProperty("UseNozzle1"); }
        }

        private bool _isUseNozzleNo2 = true;
        public bool UseNozzle2
        {
            get { return _isUseNozzleNo2; }
            set { _isUseNozzleNo2 = value; ChangeProperty("UseNozzle2"); }
        }

        private bool _isUseNozzleNo3 = true;
        public bool UseNozzle3
        {
            get { return _isUseNozzleNo3; }
            set { _isUseNozzleNo3 = value; ChangeProperty("UseNozzle3"); }
        }

        private bool _isUseNozzleNo4 = true;
        public bool UseNozzle4
        {
            get { return _isUseNozzleNo4; }
            set { _isUseNozzleNo4 = value; ChangeProperty("UseNozzle4"); }
        }

        private bool _isUseVacSucCheck = true;
        public bool UseVacSucCheck
        {
            get { return _isUseVacSucCheck; }
            set { _isUseVacSucCheck = value; ChangeProperty("UseVacSucCheck"); }
        }
    }

    public class NozzleManageParam : INotifyPropertyChanged
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

        private string _nozzleSN = "";  //喷咀SN
        public string NozzleSN
        {
            get { return _nozzleSN; }
            set { _nozzleSN = value; ChangeProperty("NozzleSN"); }
        }

        private string _nozzleChangeTime = "";  //喷咀更换时间
        public string NozzleChangeTime
        {
            get { return _nozzleChangeTime; }
            set { _nozzleChangeTime = value; ChangeProperty("NozzleChangeTime"); }
        }

        private string _tinBallSN = "";  //锡球SN
        public string TinBallSN
        {
            get { return _tinBallSN; }
            set { _tinBallSN = value; ChangeProperty("TinBallSN"); }
        }

        private string _tinBallOpenTime = "";  //锡球打开时间
        public string TinBallOpenTime
        {
            get { return _tinBallOpenTime; }
            set { _tinBallOpenTime = value; ChangeProperty("TinBallOpenTime"); }
        }

        private string _tinBallChangeTime = "";  //锡球更换时间
        public string TinBallChangeTime
        {
            get { return _tinBallChangeTime; }
            set { _tinBallChangeTime = value; ChangeProperty("TinBallChangeTime"); }
        }

        private int _nozzleLifetime = 5000;  //允许使用寿命
        public int NozzleLifetime
        {
            get { return _nozzleLifetime; }
            set { _nozzleLifetime = value; ChangeProperty("NozzleLifetime"); }
        }

        private int _nozzleCleanInterval = 0;  //清洗间隔
        public int NozzleCleanInterval
        {
            get { return _nozzleCleanInterval; }
            set { _nozzleCleanInterval = value; ChangeProperty("NozzleCleanInterval"); }
        }

        private int _nozzleLifetimeAlarmPercent = 90;  //使用寿命预警阈值
        public int NozzleLifetimeAlarmPercent
        {
            get { return _nozzleLifetimeAlarmPercent; }
            set { _nozzleLifetimeAlarmPercent = value; ChangeProperty("NozzleLifetimeAlarmPercent"); }
        }

        private int _nozzleCleanAlarmPercent = 90;  //清洗间隔预警阈值
        public int NozzleCleanAlarmPercent
        {
            get { return _nozzleCleanAlarmPercent; }
            set { _nozzleCleanAlarmPercent = value; ChangeProperty("NozzleCleanAlarmPercent"); }
        }

        private int _nozzleCurrUseTotalCount = 0;  //当前使用总计数
        public int NozzleCurrUseTotalCount
        {
            get { return _nozzleCurrUseTotalCount; }
            set { _nozzleCurrUseTotalCount = value; ChangeProperty("NozzleCurrUseTotalCount"); }
        }

        private int _nozzleCurrUseCount = 0;  //当前使用次数
        public int NozzleCurrUseCount
        {
            get { return _nozzleCurrUseCount; }
            set { _nozzleCurrUseCount = value; ChangeProperty("NozzleCurrUseCount"); }
        }

        private int _nozzleCleanCount = 0;  //已清洗次数
        public int NozzleCleanCount
        {
            get { return _nozzleCleanCount; }
            set { _nozzleCleanCount = value; ChangeProperty("NozzleCleanCount"); }
        }

    }

    public class TinBallManageParam : INotifyPropertyChanged
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

        private string _tinBallSN = "";  //锡球编号
        public string TinBallSN
        {
            get { return _tinBallSN; }
            set { _tinBallSN = value; ChangeProperty("TinBallSN"); }
        }

        private string _addStartTime = "";  //加料起始时间
        public string AddStartTime
        {
            get { return _addStartTime; }
            set { _addStartTime = value; ChangeProperty("AddStartTime"); }
        }

        private string _addCount = "0";  //加料次数
        public string AddCount
        {
            get { return _addCount; }
            set { _addCount = value; ChangeProperty("AddCount"); }
        }
    }
}
