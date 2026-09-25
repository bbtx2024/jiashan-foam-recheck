using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;

namespace QA.Business.Model
{
    public class TossingPerHour
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
        /// 抛料查询路径
        /// </summary>
        private string _tossingPath = $"D:\\QKProject\\RunInfo\\DayRechechPerHour";
        public string TossingPath
        {
            get
            {
                if (!Directory.Exists(_tossingPath))
                {
                    Directory.CreateDirectory(_tossingPath);
                }
                return _tossingPath;
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

        /// <summary>
        /// 抛料吸嘴
        /// </summary>
        private int _nozzleNgCount;
        public int NozzleNgCount
        {
            get { return _nozzleNgCount; }
            set { _nozzleNgCount = value; }
        }

        /// <summary>
        /// 抛料原因
        /// </summary>
        private int _reasonNgCount;
        public int ReasonNgCount
        {
            get { return _reasonNgCount; }
            set { _reasonNgCount = value; }
        }

        public void Clear()
        {
            OkCount = 0;
            NgCount = 0;
            for (int i = 0; i < 24; i++)
            {
                DayTossingQuantitys[i].Clear();
            }
            NotifyOfPropertyChange(() => TotalCount);
            NotifyOfPropertyChange(() => OkRate);
        }

        //一天抛料数据
        private TossingQuantity[] _dayTossingQuantitys = new TossingQuantity[24];
        public TossingQuantity[] DayTossingQuantitys
        {
            get { return _dayTossingQuantitys; }
            set { _dayTossingQuantitys = value; NotifyOfPropertyChange(() => DayTossingQuantitys); }
        }

        //一周抛料数据
        private TossingQuantity[] _weekTossingQuantitys = new TossingQuantity[7];
        public TossingQuantity[] WeekTossingQuantitys
        {
            get { return _weekTossingQuantitys; }
            set { _weekTossingQuantitys = value; NotifyOfPropertyChange(() => WeekTossingQuantitys); }
        }

        //吸嘴抛料数据
        private TossingQuantity[] _nozzleTossingQuantitys = new TossingQuantity[4];
        public TossingQuantity[] NozzleTossingQuantitys
        {
            get { return _nozzleTossingQuantitys; }
            set { _nozzleTossingQuantitys = value; NotifyOfPropertyChange(() => NozzleTossingQuantitys); }
        }

        //抛料原因数据
        private TossingQuantity[] _reasonTossingQuantitys = new TossingQuantity[2];
        public TossingQuantity[] ReasonTossingQuantitys
        {
            get { return _reasonTossingQuantitys; }
            set { _reasonTossingQuantitys = value; NotifyOfPropertyChange(() => ReasonTossingQuantitys); }
        }

        public TossingPerHour(DateTime dateTime)
        {
            for (int i = 0; i < 24; i++)
            {
                _dayTossingQuantitys[i] = new TossingQuantity();
            }
            for (int i = 0; i < 7; i++)
            {
                _weekTossingQuantitys[i] = new TossingQuantity();
            }
            for (int i = 0; i < 4; i++)
            {
                _nozzleTossingQuantitys[i] = new TossingQuantity();
            }
            for (int i = 0; i < 2; i++)
            {
                _reasonTossingQuantitys[i] = new TossingQuantity();
            }

            //ReadOutputPerHour(dateTime, 1);
            //ReadTossingPerHour(dateTime, 1);
        }


        public bool ReadOutputPerHour(DateTime dateTime, int machineType,bool IsMES)
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
            string resultPath = _tossingPath + "\\" + _recheckNGModePath + "\\" + dateTime.ToString("yyyyMM") + "\\" + dateTime.ToString("yyyy-MM-dd") + ".csv";
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
                        DayTossingQuantitys[i].OkCount = 0;
                        DayTossingQuantitys[i].NgCount = 0;
                    }
                    if (machineType == 0)
                    {
                        while (!file.EndOfStream)
                        {
                            str = file.ReadLine();
                            string[] strs = str.Split(',');
                            DateTime strDataTime = DateTime.Parse(strs[1], CultureInfo.CurrentCulture);

                            for (int i = 0; i < 24; i++)
                            {
                                if (strDataTime.Hour == i)
                                {
                                    if (strs[12] == "0")
                                    {
                                        DayTossingQuantitys[i].OkCount++;
                                    }
                                    else
                                    {
                                        DayTossingQuantitys[i].NgCount++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        while (!file.EndOfStream)
                        {
                            str = file.ReadLine();
                            string[] strs = str.Split(',');
                            DateTime strDataTime = DateTime.Parse(strs[1], CultureInfo.CurrentCulture);

                            for (int i = 0; i < 24; i++)
                            {
                                if (strDataTime.Hour == i)
                                {
                                    if (strs[18] == "0")
                                    {
                                        DayTossingQuantitys[i].OkCount++;
                                    }
                                    else
                                    {
                                        DayTossingQuantitys[i].NgCount++;
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 0; i < 24; i++)
                    {
                        OkCount += DayTossingQuantitys[i].OkCount;
                        NgCount += DayTossingQuantitys[i].NgCount;
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

        public bool ReadTossingPerHour(DateTime dateTime, int machineType,bool IsMES)
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
            string resultPath = _tossingPath + "\\" + _recheckNGModePath + "\\" + dateTime.ToString("yyyyMM") + "\\" + dateTime.ToString("yyyy-MM-dd") + ".csv";
            if (!File.Exists(resultPath))
            {
                return false;
            }
            try
            {
                using (StreamReader file = new StreamReader(resultPath))
                {
                    string str = file.ReadLine();
                    for (int i = 0; i < 4; i++)
                    {
                        NozzleTossingQuantitys[i].NozzleNgCount = 0;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        ReasonTossingQuantitys[i].ReasonNgCount = 0;
                    }

                    if (machineType == 0)
                    {
                        while (!file.EndOfStream)
                        {
                            str = file.ReadLine();
                            string[] strs = str.Split(',');
                            //筛选不同吸嘴的NG数据
                            if (Convert.ToInt32(strs[7]) == 1 && Convert.ToInt32(strs[10]) != 0)
                            {
                                NozzleTossingQuantitys[0].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[7]) == 2 && Convert.ToInt32(strs[10]) != 0)
                            {
                                NozzleTossingQuantitys[1].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[7]) == 3 && Convert.ToInt32(strs[10]) != 0)
                            {
                                NozzleTossingQuantitys[2].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[7]) == 4 && Convert.ToInt32(strs[10]) != 0)
                            {
                                NozzleTossingQuantitys[3].NozzleNgCount++;
                            }
                            //筛选不同抛料原因的数据
                            if (strs[12] != "0")
                            {
                                if (strs[12] == "5")
                                {
                                    ReasonTossingQuantitys[0].ReasonNgCount++;
                                }
                                else //if (strs[12] == "14")//注释掉，防止数目对不上
                                {
                                    ReasonTossingQuantitys[1].ReasonNgCount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        while (!file.EndOfStream)
                        {
                            str = file.ReadLine();
                            string[] strs = str.Split(',');
                            //筛选不同吸嘴的NG数据
                            if (Convert.ToInt32(strs[13]) == 1 && Convert.ToInt32(strs[16]) != 0)
                            {
                                NozzleTossingQuantitys[0].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[13]) == 2 && Convert.ToInt32(strs[16]) != 0)
                            {
                                NozzleTossingQuantitys[1].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[13]) == 3 && Convert.ToInt32(strs[16]) != 0)
                            {
                                NozzleTossingQuantitys[2].NozzleNgCount++;
                            }
                            else if (Convert.ToInt32(strs[7]) == 4 && Convert.ToInt32(strs[16]) != 0)
                            {
                                NozzleTossingQuantitys[3].NozzleNgCount++;
                            }
                            if (strs[18] != "0")
                            {
                                if (strs[18] == "5")
                                {
                                    ReasonTossingQuantitys[0].ReasonNgCount++;
                                }
                                else //if (strs[18] == "14")//注释掉，防止数目对不上
                                {
                                    ReasonTossingQuantitys[1].ReasonNgCount++;
                                }
                            }
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



    }

    public class TossingQuantity
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

        private int _nozzleNgCount;
        public int NozzleNgCount
        {
            get { return _nozzleNgCount; }
            set { _nozzleNgCount = value; }
        }

        private int _reasonNgCount;
        public int ReasonNgCount
        {
            get { return _reasonNgCount; }
            set { _reasonNgCount = value; }
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
