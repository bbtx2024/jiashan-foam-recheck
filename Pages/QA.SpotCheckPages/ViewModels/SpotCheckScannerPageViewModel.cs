using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using HandyControl.Controls;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Component.MES;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.PDCA;
using QA.Business.Component.PLC;
using QA.Business.Component.Scanner;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Station;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using IComponent = QA.Business.Interfaces.IComponent;

namespace QA.SpotCheckPages.ViewModels
{
    [Export("SpotCheckScannerPageViewModel", typeof(ISpotPageViewModel))]
    public class SpotCheckScannerPageViewModel : Screen, INotifyPropertyChanged, ISpotPageViewModel
    {
        #region Field
        private IEventAggregator _eventAggregator;
        private Scanner_TcpComponent _scanner_Component;
        private Scanner_TcpComponentRecheck _scanner_ComponentRecheck;
        private MES_Component _mes_Component;
        private MotionGoogol_Component _mGoogol_Component;
        private Hive_Component _hive_Component;
        private PDCA_Component _pdca_Component;
        private StepStatus _stepStatus;
        private ParamManager _paramManager;
        private DateTime lastReuploadPDCADate = DateTime.Now;
        private PLC_Component _plc_Component;
        private Camera_Component _camera_Component;
        #endregion

        #region Property

        public override string DisplayName { get; set; } = "扫码点检";

        public ushort OrderID { get; set; } = 2;

        private string _carrierSN = "";
        public string CarrierSN
        {
            get => _carrierSN;
            set
            {
                //DateTime time = DateTime.Now;
                //if ((time - inputTime).TotalMilliseconds <= 20 || value.Length <= 1)
                //{
                //    _carrierSN = value;
                //}
                //else if (value.Length - _carrierSN.Length == 1)
                //{
                //    //找到增加的位置
                //    char[] oldChars = _carrierSN.ToCharArray();
                //    char[] currChars = value.ToCharArray();
                //    bool find = false;
                //    for (int i = 0; i < oldChars.Length; i++)
                //    {
                //        if (oldChars[i] != currChars[i])
                //        {
                //            _carrierSN = new string(currChars[i], 1);
                //            find = true;
                //            break;
                //        }
                //    }
                //    if (!find)
                //    {
                //        _carrierSN = new string(currChars[currChars.Length - 1], 1);
                //    }
                //    //100ms后，如果长度小于5，自动重置，防止很快的按键盘
                //    _ = Task.Run(() =>
                //    {
                //        Thread.Sleep(100);
                //        if (_carrierSN.Length <= 5)
                //        {
                //            _carrierSN = "";
                //            NotifyOfPropertyChange(() => CarrierSN);
                //        }
                //    });
                //}
                //else
                //{
                //    _carrierSN = "!!!Error!!!";
                //}
                //inputTime = time;
                //NotifyOfPropertyChange(() => CarrierSN);
                _carrierSN = value;
                NotifyOfPropertyChange(() => CarrierSN);
            }
        }

        private string[] _lineSN = new string[12];
        public string[] LineSN
        {
            get => _lineSN;
            set
            {
                _lineSN = value;
                NotifyOfPropertyChange(() => LineSN);
            }
        }

        private string[] _sipSNs = new string[12];
        public string[] SipSNs
        {
            get => _sipSNs;
            set
            {
                _sipSNs = value;
                NotifyOfPropertyChange(() => SipSNs);
            }
        }

        private string _battery_Foam_Tape_SN = "";
        public string Battery_Foam_Tape_SN
        {
            get => _battery_Foam_Tape_SN;
            set
            {
                _battery_Foam_Tape_SN = value;
                NotifyOfPropertyChange(() => Battery_Foam_Tape_SN);
            }
        }

        private string[] _routings = new string[12];
        public string[] Routings
        {
            get => _routings;
            set
            {
                _routings = value;
                NotifyOfPropertyChange(() => Routings);
            }
        }

        private string[] _results = new string[12];
        public string[] Results
        {
            get => _results;
            set
            {
                _results = value;
                NotifyOfPropertyChange(() => Results);
            }
        }

        public bool IsPdcaReuploading { get; set; } = false;
        public bool EnableReupload { get => !IsPdcaReuploading; }
        public bool IsStopTest { get; set; } = false;

        #endregion

        public SpotCheckScannerPageViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _scanner_Component = (Scanner_TcpComponent)IoC.Get<IScanner>();
            _scanner_ComponentRecheck = (Scanner_TcpComponentRecheck)IoC.Get<IScannerRechck>();
            _mes_Component = (MES_Component)IoC.Get<IMES>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _stepStatus = IoC.Get<StepStatus>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            _pdca_Component = (PDCA_Component)IoC.Get<IPDCA>();
            _paramManager = IoC.Get<ParamManager>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
        }

        #region Handle
        public void Handle(List<IComponent> message)
        {
            //每天中午12点、晚上8点，如果当前机台内没有载具，自动补传PDCA
            if (_hive_Component.HiveStatus != EN_HiveStatus.Running)
            {
                DateTime currTime = DateTime.Now;
                DateTime hour12 = currTime.Date.AddHours(12);
                DateTime hour20 = currTime.Date.AddHours(20);
                if ((lastReuploadPDCADate < hour12 && currTime >= hour12)
                    || (lastReuploadPDCADate < hour20 && currTime >= hour20))
                {
                    lastReuploadPDCADate = currTime;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "开始自动补传PDCA", En_Logout_Type.Pdca, true);
                    ReuploadPDCA();
                }
            }
        }
        #endregion

        #region Method

        public void GetSSSNs()
        {
            _scanner_Component.DataManManualTriger(out string carrierSN);
            CarrierSN = carrierSN;
        }

        public void GetRecheckSNs()
        {
            _scanner_ComponentRecheck.DataManManualTriger(out string carrierSN);
            CarrierSN = carrierSN;
        }

        public void GetLineSNs()
        {
            IsStopTest = false;
            LineSN = new string[12];
            string[] lineSN = new string[12];
            CarrierStatus carrierStatus = new CarrierStatus();
            _plc_Component.SetScanRecheckResult(EN_Result.OK);
            //给PLC处理时间
            Thread.Sleep(200);
            int[] idxArr = new int[] { 0, 3, 6, 9, 10, 7, 4, 1, 2, 5, 8, 11 };
            Task.Factory.StartNew(async () =>
            {
                //复检，拍照后立刻给plc返回ok
                for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                {

                    int idx = idxArr[i];

                    //等待相机到位信号
                    while (true)
                    {
                        if (IsStopTest == true)
                        {
                            return;
                        }
                        if (_plc_Component.IsPlcCameraReady() && _plc_Component.GetPlcIdxNow() == idx)
                        {
                            break;
                        }
                    }

                    //视觉分析
                    //拍照前延时，防止轴抖动造成误差
                    Thread.Sleep(_stepStatus.ParamManager.CameraParam.DelayPhtotTime);
                    //第二次失败再弹窗
                    if (!_camera_Component.GetCameraResultS1(carrierStatus, idx))
                    {
                        while (!_camera_Component.GetCameraResultS1(carrierStatus, idx))
                        {
                            NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1->{idx + 1}穴视觉软件失连", En_Logout_Type.Alarm, true);
                            if (MessageBox.Show("视觉软件失连，确定重试吗？", "操作提示", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.OK)
                            {
                                continue;
                            }
                        }
                    }
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->{idx + 1}穴复检拍照完成", En_Logout_Type.Run, true);
                    _plc_Component.SetCameraFinished(EN_Result.OK);

                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->开始获取所有视觉结果", En_Logout_Type.Run, true);
                //等待PLC要视觉结果
                //while (!_plc_Component.IsPlcCameraReadyResult())
                //{
                //    Thread.Sleep(5);
                //}
            S1A:
                //需要在3s内获取所有视觉的结果
                DateTime time = DateTime.Now;
                bool finished = false;
                while ((DateTime.Now - time).TotalMilliseconds < 3000)
                {
                    _camera_Component.GetCameraResultS1A(carrierStatus, out finished, out lineSN);
                    if (finished)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                if (!finished)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1A->未能获取所有穴位复检结果", En_Logout_Type.Alarm, true);
                    if (MessageBox.Show("未能获取所有穴位复检结果，确定重试吗？", "操作提示", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.OK)
                    {
                        goto S1A;
                    }
                    else
                    {
                        return;
                    }
                }
                //给PLC处理时间
                Thread.Sleep(200);
                for (int i = 0; i < lineSN.Length; i++)
                {
                    if (lineSN[i] != "7")
                    {
                        LineSN[i] = lineSN[i];
                    }
                }
                NotifyOfPropertyChange(() => LineSN);
                await Task.Delay(50);

            });

        }

        public void StopTest()
        {
            IsStopTest = true;
        }
        public bool GetSipSNs()
        {
            SipSNs = new string[12];
            Routings = new string[12];
            Results = new string[12];
            if (!_mes_Component.GetSipSNs(LineSN, CarrierSN, out string[] sipsns))
            {
                _sipSNs[0] = "获取所有Sip码通信异常";
                NotifyOfPropertyChange(() => SipSNs);
                return false;
            }
            for (int i = 0; i < sipsns.Length; i++)
            {
                SipSNs[i] = sipsns[i];
            }
            NotifyOfPropertyChange(() => SipSNs);
            return true;
        }

        public bool GetRoutings()
        {
            Routings = new string[12];
            Results = new string[12];
            //向Routings和cavArr写入数据
            int[] cavArr = new int[12];
            for (int i = 0; i < 12; i++)
            {
                if (string.IsNullOrEmpty(SipSNs[i]))
                {
                    continue;
                }
                if (!_mes_Component.GetRouting(SipSNs[i], CarrierSN, out EN_Routing_Result routing))
                {
                    Routings[i] = "获取路由及穴位通信异常";
                    NotifyOfPropertyChange(() => Routings);
                    return false;
                }
                if (routing == EN_Routing_Result.OK)
                {
                    Routings[i] = "路由检查OK";
                }
                else if (routing == EN_Routing_Result.HasUploaded)
                {
                    Routings[i] = "SIP板已经绑定，无需再绑";
                }
                else if (routing == EN_Routing_Result.NotThisStation)
                {
                    Routings[i] = "SIP板当前工序不是本站";
                }
                else
                {
                    Routings[i] = "其他错误，请查看日志";
                }
            }
            NotifyOfPropertyChange(() => Routings);
            //调整次序
            string[] tempSip = new string[12];
            string[] tempRouting = new string[12];
            bool cavErr = false;
            for (int i = 0; i < 12; i++)
            {
                if (string.IsNullOrEmpty(SipSNs[i]))
                {
                    continue;
                }
                int realCav = cavArr[i];
                if (realCav <= 0 || realCav > 12)
                {
                    cavErr = true;
                    Routings[i] += "；无穴位信息";
                    continue;
                }
                tempSip[realCav - 1] = SipSNs[i];
                tempRouting[realCav - 1] = Routings[i];
            }
            if (cavErr)
            {
                return false;
            }
            SipSNs = tempSip;
            Routings = tempRouting;
            return true;
        }

        public void ManualBindAll()
        {
            if (string.IsNullOrEmpty(Battery_Foam_Tape_SN))
            {
                Results[0] = $"未输入Battery_Foam_Tape_SN";
                NotifyOfPropertyChange(() => Results);
                return;
            }
            Results = new string[12];
            for (int i = 0; i < 12; i++)
            {
                if (string.IsNullOrEmpty(SipSNs[i]) || string.IsNullOrEmpty(Routings[i]) || !Routings[i].Contains("OK"))
                {
                    continue;
                }
                else if (!_mes_Component.BindOne(CarrierSN, i + 1, SipSNs[i], Battery_Foam_Tape_SN))
                {
                    Results[i] = "绑定失败";
                }
                else
                {
                    Results[i] = "绑定成功";
                }
            }
            NotifyOfPropertyChange(() => Results);
        }

        public void OnceClickQueryBind()
        {
            if (string.IsNullOrEmpty(CarrierSN))
            {
                CarrierSN = "需先获取载具SN";
                return;
            }
            if (GetSipSNs() && GetRoutings())
            {
                ManualBindAll();
            }
        }

        public void OnceCleanExcel()
        {
            SipSNs = new string[12];
            Routings = new string[12];
            Results = new string[12];
            LineSN = new string[12];
        }

        /// <summary>
        /// 上传所有失败的pdca
        /// </summary>
        public void ReuploadPDCA()
        {
            if (IsPdcaReuploading)
            {
                return;
            }
            IsPdcaReuploading = true;//防止多次进入的标记
            _ = Task.Run(() =>
            {
                string rootDir = @"D:\tray\pdca";
                if (!Directory.Exists(rootDir))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"没有需要补传的数据！", En_Logout_Type.Pdca, true);
                    return;
                }
                string[] dirs = Directory.GetDirectories(rootDir);
                if (dirs.Length == 0)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"没有需要补传的数据！", En_Logout_Type.Pdca, true);
                    return;
                }
                foreach (var dir in dirs)
                {
                    string sn = dir.Substring(dir.LastIndexOf('\\') + 1);
                    if (sn.Length != 18)
                    {
                        continue;
                    }
                    string localInfo = $@"{dir}\{sn}.txt";
                    if (!File.Exists(localInfo))
                    {
                        //NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"未找到{localInfo}，补传跳过该文件夹", En_Logout_Type.Pdca, true);
                        continue;
                    }
                    //如果文件读写时间在7天之上，删除文件夹
                    if ((DateTime.Now - new FileInfo(localInfo).LastWriteTime).TotalDays > _paramManager.PDCAParam.KeepReUploadInfoDays)
                    {
                        DeleteQuietly(dir);
                        continue;
                    }
                    string carrierSN;
                    int cavity;
                    CameraResult camResult = new CameraResult();
                    try
                    {
                        using (StreamReader sr = new StreamReader(localInfo))
                        {
                            string line;
                            string[] data;
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 2)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第1行不存在或项数不等于2，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            carrierSN = data[0];
                            cavity = int.Parse(data[1]);

                            //本机数据
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 3)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第2行不存在或项数不等于3，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            camResult.x1 = double.Parse(data[0]);
                            camResult.x1_min = double.Parse(data[1]);
                            camResult.x1_max = double.Parse(data[2]);
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 3)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第3行不存在或项数不等于3，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            camResult.y1 = double.Parse(data[0]);
                            camResult.y1_min = double.Parse(data[1]);
                            camResult.y1_max = double.Parse(data[2]);
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 3)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第4行不存在或项数不等于3，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            camResult.r1 = double.Parse(data[0]);
                            camResult.r1_min = double.Parse(data[1]);
                            camResult.r1_max = double.Parse(data[2]);
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 1)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第5行不存在或项数不等于1，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            camResult.imgOrigin = data[0];
                            if ((line = sr.ReadLine()) == null || (data = line.Split(',')).Length != 1)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}第6行不存在或项数不等于1，跳过补传", En_Logout_Type.Pdca, true);
                                continue;
                            }
                            camResult.imgProcess = data[0];

                        }
                    }
                    catch (Exception ex)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}解析异常，补传跳过该文件夹", En_Logout_Type.Pdca, true);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{localInfo}解析异常，补传跳过该文件夹\n{ex.ToString()}", En_Logout_Type.Pdca);
                        continue;
                    }
                    //        //测试用数据
                    //        string sn = "FN6HGY000WQ0000VL1", carrierSN = "PC-S-LG-LE4-01-31-20250422-0009";
                    //int cavity = 1; CameraResult camResult = new CameraResult();
                    //camResult.camResult = EN_CamResult.OK;
                    //camResult.x1_max = 99999;
                    //camResult.x1_min = -99999;
                    //camResult.y1_max = 99999;
                    //camResult.y1_min = -99999;
                    //camResult.r1_max = 99999;
                    //camResult.r1_min = -99999;

                    //camResult.imgOrigin = @"D:\Images\SourceImage\20250926\Cam1\OK\PC-S-LG-LE4-01-31-20250422-0009\1_1_103208.jpg";
                    //camResult.imgProcess = @"D:\Images\RecordImage\20250926\OK\PC-S-LG-LE4-01-31-20250422-0009\1_1_132804.jpg";
                    if (_pdca_Component.UploadPdcaOne(sn, carrierSN, cavity, camResult))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{sn}补传成功", En_Logout_Type.Pdca, true);
                    }
                    else
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{sn}补传失败", En_Logout_Type.Pdca, true);
                    }

                    if (_pdca_Component.UploadPdcaOne(sn, carrierSN, cavity, camResult))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{sn}补传成功", En_Logout_Type.Pdca, true);
                    }
                    else
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{sn}补传失败", En_Logout_Type.Pdca, true);
                    }
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"已全部补传完毕！", En_Logout_Type.Pdca, true);
                IsPdcaReuploading = false;
            });
        }

                /// <summary>
                /// 智能删除本地文件/文件夹
                /// </summary>
                /// <param name="path"></param>
                private void DeleteQuietly(string path)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        foreach (var file in Directory.GetFiles(path))
                        {
                            DeleteQuietly(file);
                        }
                        foreach (var dir in Directory.GetDirectories(path))
                        {
                            DeleteQuietly(dir);
                        }
                        Directory.Delete(path);
                    }
                }

                /// <summary>
                /// 打开pdca文件夹
                /// </summary>
                public void OpenPDCALogDir()
                {
                    try
                    {
                        //string dir = AppDomain.CurrentDomain.BaseDirectory + "logs/pdcalogs";
                        string dir = @"D:\QKProject\Log\pdcalogs";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        System.Diagnostics.Process.Start(dir);
                    }
                    catch (Exception e)
                    {
                        //ignored
                    }
                }

                public void HivecTest()
                {
                    _hive_Component.SendMachineData("1234567890", true, DateTime.Now.AddMinutes(-1), DateTime.Now);
                    _hive_Component.SendTossingInfo(1, EN_TossingCode.BreakVacuum, 1);
                }

                #endregion
            }
}
