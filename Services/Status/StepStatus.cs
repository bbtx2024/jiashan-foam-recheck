using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Caliburn.Micro;
using HandyControl.Controls;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Define;
using QA.Business.Manager;
using QA.Business.Model;
using QA.Business.Procedure;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Steps
{
    public class StepStatus
    {
        #region Field
        public ManualResetEvent ManualResetEvt_CtrlPlace = new ManualResetEvent(false);
        public AutoResetEvent AutoResetEvt_CtrlVisual = new AutoResetEvent(false);
        public ManualResetEvent AutoResetEvt_CtrlScanner = new ManualResetEvent(false);
        public AutoResetEvent AutoResetEvt_DownCam = new AutoResetEvent(false);
        private object obj = new object();
        private static bool createdInstance = false;
        public string lastCarrierSN = "";
        #endregion

        #region Property
        public CacheParamManager CacheParamManager { get; set; }
        public ParamManager ParamManager { get; set; }
        public GlobalVariable GlobalVariable { get; set; }
        public LaserSprayProcedure CurrentProcedure { get; set; }
        public OutputPerHour OutputPerHour { get; } = new OutputPerHour(DateTime.Now);
        public TossingPerHour BumperTossingPerHour { get; } = new TossingPerHour(DateTime.Now);
        public TossingPerHour GNDTossingPerHour { get; } = new TossingPerHour(DateTime.Now);
        public RecheckNGPerHour ReacheckNGPerHour { get; } = new RecheckNGPerHour(DateTime.Now);
        public HiveMachineStatusStatistic HiveMachineStatusStatistic { get; } = new HiveMachineStatusStatistic();
        public EN_RunStep NextStep1 { get; set; } = EN_RunStep.Idle;
        public EN_RunStep NextStep2 { get; set; } = EN_RunStep.Idle;
        public EN_RunStep NextStep3 { get; set; } = EN_RunStep.Idle;
        /// <summary>
        /// 指示Station2是否可启动。
        /// 0表示Station1正在判断能否启动，1表示Station1判断可启动，2表示Station1判断不可启动。
        /// </summary>
        public int AllowStation2Start { get; set; } = 0;
        #endregion

        public StepStatus()
        {
            if (createdInstance)
            {
                MessageBox.Error("已实例化StepStatus！");
                Environment.Exit(0);
            }
            createdInstance = true;
            AutoResetEvt_CtrlVisual.Set();
            AutoResetEvt_CtrlScanner.Set();
            ManualResetEvt_CtrlPlace.Reset();
            AutoResetEvt_DownCam.Reset();
            CacheParamManager = IoC.Get<CacheParamManager>();
            ParamManager = IoC.Get<ParamManager>();
            GlobalVariable = IoC.Get<GlobalVariable>();
        }

        #region 载具信息相关
        public List<CarrierStatus> Carriers = new List<CarrierStatus>();
        public void AddCarrier(CarrierStatus carrier)
        {
            lock (obj)
            {
                Carriers.Add(carrier);
            }
        }
        public CarrierStatus GetCurCarrier()
        {
            lock (obj)
            {
                return Carriers.Count > 0 ? Carriers[0] : null;
            }
        }

        public string GetCarrierListStr()
        {
            lock (obj)
            {
                if (Carriers.Count == 0)
                {
                    return "空";
                }
                StringBuilder sb = new StringBuilder(Carriers[0].carrierSN);
                for (int i = 1; i < Carriers.Count; i++)
                {
                    sb.Append(",").Append(Carriers[i].carrierSN);
                }
                return sb.ToString();
            }
        }

        public void RemoveCurCarrier()
        {
            lock (obj)
            {
                if (Carriers.Count > 0)
                {
                    Carriers.Remove(Carriers[0]);
                }
            }
        }
        public void ClearAllCarriers()
        {
            lock (obj)
            {
                Carriers.Clear();
            }
        }
        #endregion

        /// <summary>
        /// 获取Tray信息
        /// </summary>
        public bool GetTray(CarrierStatus carrierStatus)
        {
            try
            {
                string previousTrayFile = $@"\\{ParamManager.ScannerParam.TrayIP}\tray\current\{carrierStatus.carrierSN}.txt";
                if (!File.Exists(previousTrayFile))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"未找到{carrierStatus.carrierSN}的Tray文件(Không tìm thấy tập tin Tray)", En_Logout_Type.Alarm, true);
                    return false;
                }
                using (StreamReader sr = new StreamReader(previousTrayFile))
                {
                    string line;
                    string[] strArr;
                    //载具码
                    line = sr.ReadLine();
                    if (line != carrierStatus.carrierSN)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第1行载具码不一致(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    //errorCode
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第2行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.errorCode[i] = int.Parse(strArr[i]);
                        if (carrierStatus.errorCode[i] < (int)EN_TrayStatus.空穴 || carrierStatus.errorCode[i] > (int)EN_TrayStatus.拍照完成)
                        {
                            NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第2行穴位{i + 1}数据异常(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                            return false;
                        }
                    }
                    //sip码
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第3行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.sipSN[i] = strArr[i];
                    }
                    //Tape贴装吸嘴
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第4行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_Nozzle[i] = int.Parse(strArr[i]);
                    }
                    //Tape贴装压力
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Scan->{carrierStatus.carrierSN}.txt第5行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_PastePress[i] = float.Parse(strArr[i]);
                    }
                    //Tape保压头
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Scan->{carrierStatus.carrierSN}.txt第6行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_Indenter[i] = int.Parse(strArr[i]);
                    }
                    //Tape SN
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第7行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_SN[i] = strArr[i];
                    }
                    //Tape 是否抛料
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第8行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_Isthrow[i] = int.Parse(strArr[i]);
                    }
                    //Tape 抛料次数
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第9行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_ThrowCount[i] = int.Parse(strArr[i]);
                    }
                    //Tape 抛料类型
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第10行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.Tape_ThrowReason[i] = int.Parse(strArr[i]);
                    }
                    //穴位是否为空
                    line = sr.ReadLine();
                    strArr = line.Split(',');
                    if (strArr.Length != carrierStatus.errorCode.Length)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{carrierStatus.carrierSN}.txt第11行个数{strArr.Length}不等于{carrierStatus.errorCode.Length}(Tray phân tích thất bại)", En_Logout_Type.Alarm, true);
                        return false;
                    }
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        carrierStatus.isEmpty[i] = int.Parse(strArr[i]);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"未能获取{carrierStatus.carrierSN}的Tray信息(Tray phân tích thất bại)，{ex.Message}", En_Logout_Type.Alarm, true);
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, ex.ToString(), En_Logout_Type.Alarm);
                return false;
            }
        }

        /// <summary>
        /// 保存并备份Tray信息
        /// </summary>
        public bool BackupAndWriteTray(CarrierStatus carrierStatus)
        {
            try
            {
                DateTime nowTime = DateTime.Now;
                string backupDir = "D://tray//backup//" + nowTime.ToString("yyyy-MM-dd") + "//";
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                string backupTrayFile = backupDir + nowTime.ToString("HHmmss") + "_" + carrierStatus.carrierSN + ".txt";
                using (StreamWriter sw = new StreamWriter(backupTrayFile, false))
                {
                    //载具码
                    sw.WriteLine(carrierStatus.carrierSN);
                    //errorCode
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.errorCode[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //sip码
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.sipSN[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //贴装吸嘴
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_Nozzle[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //压力
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_PastePress[i].ToString("F2"));
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //保压头
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_Indenter[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //Tape SN
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_SN[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //Tape 是否抛料
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_Isthrow[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //Tape 抛料次数
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_ThrowCount[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                    //Tape 抛料类型
                    sb = new StringBuilder();
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        sb.Append(carrierStatus.Tape_ThrowReason[i]);
                        if (i != carrierStatus.errorCode.Length - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                }
                //写入Tray信息
                string currentTrayDir = $@"D:\tray\current";
                if (!Directory.Exists(currentTrayDir))
                {
                    Directory.CreateDirectory(currentTrayDir);
                }
                string currentTrayFile = $@"{currentTrayDir}\{carrierStatus.carrierSN}.txt";
                File.Copy(backupTrayFile, currentTrayFile, true);
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"未能备份并写入{carrierStatus.carrierSN}的Tray信息，{e.Message}", En_Logout_Type.Exception, true);
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString(), En_Logout_Type.Exception);
                return false;
            }
        }
    }

    public class CarrierStatus
    {
        #region FA11-004
        /// <summary>
        /// 载具SN
        /// </summary>
        public string carrierSN = "";
        /// <summary>
        /// 扫码时间
        /// </summary>
        public DateTime scanTime = DateTime.Now;
        /// <summary>
        /// 错误码，参考TrayStatus
        /// </summary>
        public int[] errorCode = new int[12];
        /// <summary>
        /// sip码
        /// </summary>
        public string[] sipSN = new string[12];
        /// <summary>
        /// 排线码
        /// </summary>
        public string[] lineSN = new string[12];
        /// <summary>
        /// sip类型
        /// </summary>
        public string[] sipType = new string[12];

        /// <summary>
        /// 贴装吸嘴，01~02
        /// </summary>
        public int[] Tape_Nozzle = new int[12] { 1, 1, 1, 2, 2, 2, 1, 1, 1, 2, 2, 2 };
        /// <summary>
        /// 保压头，01~04
        /// </summary>
        public int[] Tape_Indenter = new int[12] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4 };
        /// <summary>
        /// 贴装压力，单位kg，保留两位小数，0.40-0.60
        /// </summary>
        public float[] Tape_PastePress = new float[12];
        /// <summary>
        /// Tape SN
        /// </summary>
        public string[] Tape_SN = new string[12];
        /// <summary>
        /// Tape 是否抛料
        /// </summary>
        public int[] Tape_Isthrow = new int[12];
        /// <summary>
        /// Tape 抛料次数
        /// </summary>
        public int[] Tape_ThrowCount = new int[12];
        /// <summary>
        /// Tape 抛料类型
        /// </summary>
        public int[] Tape_ThrowReason = new int[12];
        /// <summary>
        /// 上视觉结果
        /// </summary>

        public string[] cavStateStr = new string[12]; //上视觉结果
        /// <summary>
        /// Tape下视觉定位数据
        /// </summary>

        public string[] tapeBaseDistance = new string[12];

        /// <summary>
        /// 上视觉定位数据
        /// </summary>
        public string[] markToPhotoCenterOffsets = new string[12];

        /// <summary>
        /// 是否为空
        /// </summary>
        public int[] isEmpty = new int[12];

        #endregion

        private static Random random = new Random();
        public CameraResult[] cameraResults = new CameraResult[12];//复检视觉结果
       // public List<HiveTossingInfo>[] hiveTossingInfolist = new List<HiveTossingInfo>[12];
        public EN_Routing_Result[] routingResults = new EN_Routing_Result[12];

        public CarrierStatus()
        {
            carrierSN = DateTime.Now.ToString("yyyyMMddHHmmss");
            for (int i = 0; i < 12; i++)
            {
                sipSN[i] = "";
                sipType[i] = "";
                Tape_PastePress[i] = (float)(random.NextDouble() * 0.1 + 0.45);
                Tape_SN[i] = "";

                Tape_Isthrow[i] = 0;
                Tape_ThrowCount[i] = 0;
                Tape_ThrowReason[i] = 0;
                isEmpty[i] = 0;

                cavStateStr[i] = "";
                markToPhotoCenterOffsets[i] = "0,0";
                tapeBaseDistance[i] = "0";

                cameraResults[i] = new CameraResult();
                //hiveTossingInfolist[i] = new List<HiveTossingInfo>();
            }
        }
    }
}
