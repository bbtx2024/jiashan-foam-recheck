using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using QA.Business.Steps;

namespace QA.Business.Model
{
    public enum EN_Recheck
    {
        MesNG,

        TapeNotExist1,
        TapeNotExist2,
        TapeLxzNotExist,
        LxzNotRemoved1,
        LxzNotRemoved2,
        XYOverRange1,
        XYOverRange2,
        TPTapeNotExist
    }

    public class RecheckNGPerHour
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

        /// <summary>
        /// 复检NG查询路径
        /// </summary>
        private string _recheckNGPath = $"D:\\QKProject\\RunInfo\\DayRechechPerHour";
        public string RecheckNGPath
        {
            get
            {
                if (!Directory.Exists(_recheckNGPath))
                {
                    Directory.CreateDirectory(_recheckNGPath);
                }
                return _recheckNGPath;
            }
        }

        private int _okCount;
        public int OkCount
        {
            get { return _okCount; }
            set { _okCount = value; NotifyOfPropertyChange(() => OkCount); }
        }

        private int _ngCount;
        public int NgCount
        {
            get { return _ngCount; }
            set { _ngCount = value; NotifyOfPropertyChange(() => NgCount); }
        }

        public int TotalCount
        {
            get { return OkCount + NgCount; }
        }
        public float OkRate
        {
            get { return TotalCount == 0 ? 0.0f : OkCount * 1.0f / TotalCount; }
        }
        public void Clear()
        {
            OkCount = 0;
            NgCount = 0;
            for (int i = 0; i < 24; i++)
            {
                DayRecheckNGQuantitys[i].Clear();
            }
            NotifyOfPropertyChange(() => TotalCount);
            NotifyOfPropertyChange(() => OkRate);
        }

        private int _reasonNgCount;
        public int ReasonNgCount
        {
            get { return _reasonNgCount; }
            set { _reasonNgCount = value; }
        }

        private int _nozzelNgCount;
        public int NozzleNgCount
        {
            get { return _nozzelNgCount; }
            set { _nozzelNgCount = value; }
        }

        private int _cavityNgCount;
        public int CavityNgCount
        {
            get { return _cavityNgCount; }
            set { _cavityNgCount = value; }
        }


        //复检NG一天数据
        private RecheckNGQuantity[] _dayRecheckNGQuantitys = new RecheckNGQuantity[24];
        public RecheckNGQuantity[] DayRecheckNGQuantitys
        {
            get { return _dayRecheckNGQuantitys; }
            set { _dayRecheckNGQuantitys = value; NotifyOfPropertyChange(() => DayRecheckNGQuantitys); }
        }

        //复检NG一周数据
        private RecheckNGQuantity[] _weekRecheckNGQuantitys = new RecheckNGQuantity[7];
        public RecheckNGQuantity[] WeekRecheckNGQuantitys
        {
            get { return _weekRecheckNGQuantitys; }
            set { _weekRecheckNGQuantitys = value; NotifyOfPropertyChange(() => WeekRecheckNGQuantitys); }
        }

        //复检NG原因数据
        private RecheckNGQuantity[] _reasonRecheckNGQuantitys = new RecheckNGQuantity[Enum.GetNames(typeof(EN_Recheck)).Length];
        public RecheckNGQuantity[] ReasonRecheckNGQuantitys
        {
            get { return _reasonRecheckNGQuantitys; }
            set { _reasonRecheckNGQuantitys = value; NotifyOfPropertyChange(() => ReasonRecheckNGQuantitys); }
        }

        //复检NG吸嘴数据
        private RecheckNGQuantity[] _nozzelRecheckNGQuantitys = new RecheckNGQuantity[4];
        public RecheckNGQuantity[] NozzleRecheckNGQuantitys
        {
            get { return _nozzelRecheckNGQuantitys; }
            set { _nozzelRecheckNGQuantitys = value; NotifyOfPropertyChange(() => NozzleRecheckNGQuantitys); }
        }

        //复检NG穴位数据
        private RecheckNGQuantity[] _cavityRecheckNGQuantitys = new RecheckNGQuantity[12];
        public RecheckNGQuantity[] CavityRecheckNGQuantitys
        {
            get { return _cavityRecheckNGQuantitys; }
            set { _cavityRecheckNGQuantitys = value; NotifyOfPropertyChange(() => CavityRecheckNGQuantitys); }
        }

        public RecheckNGPerHour(DateTime dateTime)
        {
            for (int i = 0; i < 24; i++)
            {
                _dayRecheckNGQuantitys[i] = new RecheckNGQuantity();
            }
            for (int i = 0; i < 7; i++)
            {
                _weekRecheckNGQuantitys[i] = new RecheckNGQuantity();
            }
            for (int i = 0; i < Enum.GetNames(typeof(EN_Recheck)).Length; i++)
            {
                _reasonRecheckNGQuantitys[i] = new RecheckNGQuantity();
            }
            for (int i = 0; i < 4; i++)
            {
                _nozzelRecheckNGQuantitys[i] = new RecheckNGQuantity();
            }
            for (int i = 0; i < 12; i++)
            {
                _cavityRecheckNGQuantitys[i] = new RecheckNGQuantity();
            }

            //ReadOutputPerHour(dateTime);
            //ReadRecheckNGPerHour(dateTime);
        }

        //if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _stepStatus.ParamManager.MESParam.BUse)

        private bool SaveRecheckNGPerHour(CarrierStatus carrierStatus, int cavity, bool IsMes)
        {
            //需要修改
            string _recheckNGModePath;
            if (IsMes)
            {
                _recheckNGModePath = $"Running";
            }
            else
            {
                _recheckNGModePath = $"Debug";
            }
            if (!Directory.Exists(_recheckNGPath + "\\" + _recheckNGModePath + "\\" + DateTime.Now.ToString("yyyyMM")))
            {
                Directory.CreateDirectory(_recheckNGPath + "\\" + _recheckNGModePath + "\\" + DateTime.Now.ToString("yyyyMM"));
            }

            string resultPath = _recheckNGPath + "\\" + _recheckNGModePath + "\\" + DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            try
            {

                if (!File.Exists(resultPath))
                {
                    using (StreamWriter file = new StreamWriter(resultPath, false))
                    {
                        file.WriteLine("站别,时间,料号,CarrierSN,穴位号,SIPSN,吸嘴号,吸嘴压力,保压头,是否抛料,抛料次数,抛料类型,X,Y,A,复检结果,复检NG类型");
                    }
                }
                using (StreamWriter file = new StreamWriter(resultPath, true))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("RecheckBumperAndGND");//站别
                    sb.Append(",");
                    sb.Append(DateTime.Now.ToString());//时间
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_SN[cavity].ToString());//物料料号
                    sb.Append(",");
                    sb.Append(carrierStatus.carrierSN);//CarrierSN
                    sb.Append(",");
                    sb.Append((cavity + 1).ToString());//穴位号
                    sb.Append(",");
                    sb.Append(carrierStatus.sipSN[cavity].ToString());//SIPSN
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_Nozzle[cavity].ToString());//吸嘴号
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_PastePress[cavity].ToString());//吸嘴压力
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_Indenter[cavity].ToString());//保压头
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_Isthrow[cavity].ToString());//是否抛料
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_ThrowCount[cavity].ToString());//抛料次数
                    sb.Append(",");
                    sb.Append(carrierStatus.Tape_ThrowReason[cavity].ToString());//抛料类型
                    sb.Append(",");
                    sb.Append(carrierStatus.cameraResults[cavity].x1.ToString());
                    sb.Append(",");
                    sb.Append(carrierStatus.cameraResults[cavity].y1.ToString());
                    sb.Append(",");
                    sb.Append(carrierStatus.cameraResults[cavity].camResult.ToString());
                    sb.Append(",");
                    sb.Append(carrierStatus.cameraResults[cavity].camResult.ToString());
                    file.WriteLine(sb.ToString());
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //0611
        public bool ReadOutputPerHour(DateTime dateTime, bool IsMES)
        {
            string _recheckNGModePath;
            if (IsMES)
            {
                _recheckNGModePath = $"Running";
            }
            else
            {
                _recheckNGModePath = $"Debug";
            }
            string resultPath = _recheckNGPath + "\\" + _recheckNGModePath + "\\" + dateTime.ToString("yyyyMM") + "\\" + dateTime.ToString("yyyy-MM-dd") + ".csv";
            if (!File.Exists(resultPath))
            {
                return false;
            }
            try
            {
                using (StreamReader file = new StreamReader(resultPath))
                {
                    string str = file.ReadLine();
                    OkCount = 0;
                    NgCount = 0;
                    for (int i = 0; i < 24; i++)
                    {
                        DayRecheckNGQuantitys[i].OkCount = 0;
                        DayRecheckNGQuantitys[i].NgCount = 0;
                    }
                    while (!file.EndOfStream)
                    {
                        str = file.ReadLine();
                        string[] strs = str.Split(',');
                        DateTime strDataTime = DateTime.Parse(strs[1], CultureInfo.CurrentCulture);

                        for (int i = 0; i < 24; i++)
                        {
                            if (strDataTime.Hour == i)
                            {
                                if (strs[26] == "OK")
                                {
                                    DayRecheckNGQuantitys[i].OkCount++;
                                }
                                else
                                {
                                    DayRecheckNGQuantitys[i].NgCount++;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < 24; i++)
                    {
                        OkCount += DayRecheckNGQuantitys[i].OkCount;
                        NgCount += DayRecheckNGQuantitys[i].NgCount;
                    }
                    file.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ReadRecheckNGPerHour(DateTime dateTime, bool IsMES)
        {
            string _recheckNGModePath;
            if (IsMES)
            {
                _recheckNGModePath = $"Running";
            }
            else
            {
                _recheckNGModePath = $"Debug";
            }
            string resultPath = _recheckNGPath + "\\" + _recheckNGModePath + "\\" + dateTime.ToString("yyyyMM") + "\\" + dateTime.ToString("yyyy-MM-dd") + ".csv";
            if (!File.Exists(resultPath))
            {
                return false;
            }
            try
            {
                using (StreamReader file = new StreamReader(resultPath))
                {
                    string str = file.ReadLine();
                    //初始化上一次数据
                    for (int i = 0; i < 4; i++)
                    {
                        NozzleRecheckNGQuantitys[i].NozzleNgCount = 0;
                    }
                    for (int i = 0; i < Enum.GetNames(typeof(EN_Recheck)).Length; i++)
                    {
                        ReasonRecheckNGQuantitys[i].ReasonNgCount = 0;
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        CavityRecheckNGQuantitys[i].CavityNgCount = 0;
                    }


                    while (true)
                    {
                        str = file.ReadLine();
                        string[] strs = str.Split(',');
                        //筛选不同吸嘴的复检NG数据
                        //if (Convert.ToInt32(strs[13]) == 1 && strs[26] != "OK")
                        //20250715 逻辑更改
                        //if (Convert.ToInt32(strs[13]) == 1 && strs[26] != "OK")
                        //{
                        //    NozzleRecheckNGQuantitys[0].NozzleNgCount++;
                        //}
                        //else if (Convert.ToInt32(strs[13]) == 2 && strs[26] != "OK")
                        //{
                        //    NozzleRecheckNGQuantitys[1].NozzleNgCount++;
                        //}
                        //else if (Convert.ToInt32(strs[13]) == 3 && strs[26] != "OK")
                        //{
                        //    NozzleRecheckNGQuantitys[2].NozzleNgCount++;
                        //}
                        //else if (Convert.ToInt32(strs[13]) == 4 && strs[26] != "OK")
                        //{
                        //    NozzleRecheckNGQuantitys[3].NozzleNgCount++;
                        //}

                        //20250715
                        if (strs[26] == EN_Recheck.XYOverRange2.ToString()
                            || strs[26] == EN_Recheck.LxzNotRemoved2.ToString()
                            || strs[26] == EN_Recheck.TapeLxzNotExist.ToString()
                            || strs[26] == EN_Recheck.TapeNotExist2.ToString()
                            || strs[26] == "TapeSideNG2")
                        {
                            if ((Convert.ToInt32(strs[13]) == 1 || Convert.ToInt32(strs[13]) == 2) && strs[26] != "OK")
                            {
                                NozzleRecheckNGQuantitys[2].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[13]) == 3 && strs[26] != "OK")
                            {
                                NozzleRecheckNGQuantitys[3].NozzleNgCount++;
                            }
                        }
                        if (strs[26] == EN_Recheck.TapeNotExist1.ToString()
                            || strs[26] == EN_Recheck.LxzNotRemoved1.ToString()
                            || strs[26] == EN_Recheck.XYOverRange1.ToString()
                            || strs[26] == "TapeSideNG1")
                        {
                            if (Convert.ToInt32(strs[7]) == 1 && strs[26] != "OK")
                            {
                                NozzleRecheckNGQuantitys[0].NozzleNgCount++;
                            }
                            //else if (Convert.ToInt32(strs[13]) == 2 && strs[26] != "OK")
                            else if (Convert.ToInt32(strs[7]) == 3 && strs[26] != "OK")
                            {
                                NozzleRecheckNGQuantitys[1].NozzleNgCount++;
                            }
                        }

                        //筛选不同原因复检NG的数据
                        if (strs[26] == EN_Recheck.MesNG.ToString())
                        {
                            ReasonRecheckNGQuantitys[0].ReasonNgCount++;
                        }
                        //20250715
                        else if (strs[26] == "TapeSideNG1" || strs[26] == EN_Recheck.TapeNotExist1.ToString())
                        {
                            ReasonRecheckNGQuantitys[1].ReasonNgCount++;
                        }
                        else if (strs[26] == "TapeSideNG2" || strs[26] == EN_Recheck.TapeNotExist2.ToString())
                        {
                            ReasonRecheckNGQuantitys[2].ReasonNgCount++;
                        }
                        //else if (strs[26] == EN_Recheck.TapeNotExist1.ToString())
                        //{
                        //    ReasonRecheckNGQuantitys[1].ReasonNgCount++;
                        //}
                        //else if (strs[26] == EN_Recheck.TapeNotExist2.ToString())
                        //{
                        //    ReasonRecheckNGQuantitys[2].ReasonNgCount++;
                        //}
                        else if (strs[26] == EN_Recheck.TapeLxzNotExist.ToString())
                        {
                            ReasonRecheckNGQuantitys[3].ReasonNgCount++;
                        }
                        else if (strs[26] == EN_Recheck.LxzNotRemoved1.ToString())
                        {
                            ReasonRecheckNGQuantitys[4].ReasonNgCount++;
                        }
                        else if (strs[26] == EN_Recheck.LxzNotRemoved2.ToString())
                        {
                            ReasonRecheckNGQuantitys[5].ReasonNgCount++;
                        }
                        else if (strs[26] == EN_Recheck.XYOverRange1.ToString())
                        {
                            ReasonRecheckNGQuantitys[6].ReasonNgCount++;
                        }
                        else if (strs[26] == EN_Recheck.XYOverRange2.ToString())
                        {
                            ReasonRecheckNGQuantitys[7].ReasonNgCount++;
                        }
                        else if (strs[26] == EN_Recheck.TPTapeNotExist.ToString())
                        {
                            ReasonRecheckNGQuantitys[8].ReasonNgCount++;
                        }

                        //筛选不同穴位复检NG数据
                        if (Convert.ToInt32(strs[5]) == 1 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[0].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 2 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[1].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 3 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[2].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 4 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[3].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 5 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[4].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 6 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[5].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 7 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[6].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 8 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[7].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 9 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[8].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 10 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[9].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 11 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[10].CavityNgCount++;
                        }
                        else if (Convert.ToInt32(strs[5]) == 12 && strs[26] != "OK")
                        {
                            CavityRecheckNGQuantitys[11].CavityNgCount++;
                        }

                        if (file.Peek() == -1)
                        {
                            break;
                        }
                    }
                    file.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddOne(CarrierStatus carrierStatus, int cavity, bool IsMES)
        {
            SaveRecheckNGPerHour(carrierStatus, cavity, IsMES);//存储NG抛料信息
        }

    }

    public class RecheckNGQuantity
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

        private int _okCount;
        public int OkCount
        {
            get { return _okCount; }
            set { _okCount = value; NotifyOfPropertyChange(() => OkCount); }
        }

        private int _ngCount;
        public int NgCount
        {
            get { return _ngCount; }
            set { _ngCount = value; NotifyOfPropertyChange(() => NgCount); }
        }

        public int TotalCount
        {
            get { return OkCount + NgCount; }
        }

        public float OkRate
        {
            get { return TotalCount == 0 ? 0.0f : OkCount * 1.0f / TotalCount; }
        }

        private int _nozzelNgCount;
        public int NozzleNgCount
        {
            get { return _nozzelNgCount; }
            set { _nozzelNgCount = value; }
        }

        private int _reasonNgCount;
        public int ReasonNgCount
        {
            get { return _reasonNgCount; }
            set { _reasonNgCount = value; }
        }


        private int _cavityNgCount;
        public int CavityNgCount
        {
            get { return _cavityNgCount; }
            set { _cavityNgCount = value; }
        }

        public void AddOne(bool isOk)
        {
            if (isOk)
            { OkCount++; }
            else
            { NgCount++; }
            NotifyOfPropertyChange(() => TotalCount);
            NotifyOfPropertyChange(() => OkRate);
        }

        public void Clear()
        {
            OkCount = 0;
            NgCount = 0;
            NotifyOfPropertyChange(() => TotalCount);
            NotifyOfPropertyChange(() => OkRate);
        }




    }
}
