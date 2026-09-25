/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-10
 * 说明：（PDCA数据上传）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Model.Alarm;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.BaseCtrls;
using QA_Infrastructure.BaseCtrls.ParamEnum;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Component.PDCA
{
    public class PDCA_Component : TcpCtrl, IPDCA
    {
        private PDCAParam _pdcaParam;
        private ParamManager paramManager;
        private SocketParam _socketParam = new SocketParam();
        private GlobalVariable _globalVariable;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private object obj = new object();

        public IParam Param { get; set; }
        public string ComponentName { get; set; } = "PDCA";
        public new bool IsConnected => base.IsConnected;

        public PDCA_Component()
        {
            Param = _pdcaParam = IoC.Get<PDCAParam>();
            paramManager = IoC.Get<ParamManager>();
            _globalVariable = IoC.Get<GlobalVariable>();
        }

        public bool Initial(IParam param)
        {
            try
            {
                Param = _pdcaParam = param as PDCAParam;
                _socketParam.Ip = _pdcaParam.IP;
                _socketParam.Port = _pdcaParam.Port;
                _socketParam.SendTimeout = _pdcaParam.Timeout;
                _socketParam.ReceiveTimeout = _pdcaParam.Timeout;
                _socketParam.SendBuffSize = _pdcaParam.BufferSize;
                _socketParam.ReceiveBuffSize = _pdcaParam.BufferSize;
                base.SetParam(_socketParam);
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, e + e.StackTrace);
                return false;
            }
            return true;
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            var _hive_Component = IoC.Get<IHive>() as Hive_Component;
            Task.Factory.StartNew(async () =>
            {
                DateTime time = DateTime.Now;
                while (true)
                {
                    var delayTime = _pdcaParam.BUse ? 100 : 1000;
                    try
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (Param.BUse /*|| (_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle)*/)
                        {
                            if (!IsConnected)
                            {
                                //base.Close();
                                base.Connect(_socketParam);
                            }
                            else
                            {
                                if ((DateTime.Now - time).TotalMilliseconds > 3000)
                                {
                                    time = DateTime.Now;
                                    if (!PingMini())
                                    {
                                        base.ReConnect();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
                    }
                    await Task.Delay(delayTime, _cancellationToken);
                }
            }, _cancellationToken);
            return true;
        }

        public bool Stop()
        {
            try
            {
                base.Close();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
            }
            return true;
        }

        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
            var _hive_Component = IoC.Get<IHive>() as Hive_Component;
            if (Param.BUse/* || (_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle)*/)
            {
                if (!IsConnected)
                {
                    var count = alarmInfos.Count;
                    alarmInfos.Add(new AlarmInfoModel()
                    {
                        AlarmLevel = EN_WARN_LEVEL.Error,
                        AlarmModule = EN_WarnModules.PDCA,
                        AlarmMsg = "无连接",
                        Datetime = DateTime.Now,
                        ErrorCode = 0,
                        Index = count
                    });
                }
            }
        }

        private bool SendData(string sendStr, out string retStr)
        {
            lock (obj)
            {
                retStr = "";
                return base.SendData(sendStr, ref retStr);
            }
        }

        public bool PingMini()
        {
            try
            {
                var _hive_Component = IoC.Get<IHive>() as Hive_Component;
                StringBuilder order = new StringBuilder("_{\n}\n");
                string retorder;
                if (!SendData(order.ToString(), out retorder))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "ping mini失败，发送: " + order.ToString(), En_Logout_Type.Pdca);
                    return false;
                }
                string returnstr = retorder.Trim('\0').Trim('\n').Trim('\r').Trim();
                if (!returnstr.StartsWith("ok@{success}@"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "ping mini接收格式异常，发送: " + order.ToString() + "接收: " + retorder, En_Logout_Type.Pdca);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString());
                return false;
            }
        }

        public bool UploadPdcaAll(CarrierStatus carrier, out string info)
        {
            info = "";
            //写入本机数据，写到第二行
            for (int i = 0; i < 12; i++)
            {
                if (carrier.errorCode[i] == (int)EN_TrayStatus.空穴
                    || carrier.sipSN[i] == "" || carrier.sipSN[i] == "DEFAULT")
                {
                    continue;
                }
                var camResult = carrier.cameraResults[i];
                #region 图片有xyr数据，必须匹配，不能虚拟数据
                ////如果复检NG，虚拟一个数据
                //if (carrier.errorCode[i] != (int)EN_TrayStatus.OK)
                //{
                //    Random random = new Random();
                //    double randMin = camResult.minX + (camResult.maxX - camResult.minX) / 4;
                //    double randMax = camResult.maxX - (camResult.maxX - camResult.minX) / 4;
                //    camResult.x = random.Next((int)(randMin * 1000), (int)(randMax * 1000)) / 1000.0;
                //    randMin = camResult.minY + (camResult.maxY - camResult.minY) / 4;
                //    randMax = camResult.maxY - (camResult.maxY - camResult.minY) / 4;
                //    camResult.y = random.Next((int)(randMin * 1000), (int)(randMax * 1000)) / 1000.0;
                //    randMin = -0.5;
                //    randMax = 0.5;
                //    camResult.r = random.Next((int)(randMin * 100), (int)(randMax * 100)) / 100.0;
                //}
                #endregion
                //附加信息
                string dir = $@"D:\\tray\pdca\{carrier.sipSN[i]}";
                try
                {
                    Directory.CreateDirectory(dir);
                    using (StreamWriter sw = new StreamWriter($@"{dir}\{carrier.sipSN[i]}.txt", true))
                    {
                        sw.WriteLine($"{carrier.carrierSN},{i + 1}");

                        //本机
                        sw.WriteLine($"{camResult.x1},{camResult.x1_min},{camResult.x1_max}");
                        sw.WriteLine($"{camResult.y1},{camResult.y1_min},{camResult.y1_max}");
                        sw.WriteLine($"{camResult.r1},{camResult.r1_min},{camResult.r1_max}");
                        sw.WriteLine($"{camResult.imgOrigin}");
                        sw.WriteLine($"{camResult.imgProcess}");
                    }
                }
                catch (Exception ex)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"尝试保存{carrier.sipSN[i]}的pdca数据到文件失败", En_Logout_Type.Pdca, true);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"尝试保存pdca数据到文件失败，载具{carrier.carrierSN}，穴位{i + 1}，SipSN {carrier.sipSN[i]}\n" + ex.ToString(), En_Logout_Type.Pdca);
                }
            }

            //下面的代码不能合并到上面，因为中途失败返回会导致后面的穴位缺少信息

            //一个sip板一次
            for (int i = 0; i < 12; i++)
            {
                if (carrier.errorCode[i] == (int)EN_TrayStatus.空穴
                    || carrier.sipSN[i] == "" || carrier.sipSN[i] == "DEFAULT")
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"{i + 1}穴空穴，跳过该穴PDCA上传", En_Logout_Type.Pdca, true);
                    continue;
                }
                if (!_pdcaParam.UploadNgProduct && carrier.errorCode[i] != (int)EN_TrayStatus.OK)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{i + 1}穴NG，跳过该穴PDCA上传", En_Logout_Type.Pdca, true);
                    continue;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                if (carrier.errorCode[i] == (int)EN_TrayStatus.空穴
                    || carrier.sipSN[i] == "" || carrier.sipSN[i] == "DEFAULT")
                {
                    continue;
                }
                if (!_pdcaParam.UploadNgProduct && carrier.errorCode[i] != (int)EN_TrayStatus.OK)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"{i + 1}穴{carrier.sipSN[i]} NG，当前禁用NG上传，跳过该穴PDCA上传", En_Logout_Type.Pdca, true);
                    continue;
                }
                if (!UploadPdcaOne(carrier.sipSN[i], carrier.carrierSN, i + 1, carrier.cameraResults[i]))
                {
                    info += $"{i + 1}穴上传失败 ";
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{i + 1}穴{carrier.sipSN[i]} PDCA上传失败", En_Logout_Type.Pdca, true);
                }
                else
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, $"{i + 1}穴{carrier.sipSN[i]} PDCA上传成功", En_Logout_Type.Pdca, true);
                }
            }
            return true;
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
        public bool UploadPdcaOne(string sipSN, string carrierSN, int cavity, CameraResult camResult)
        {
            if (sipSN == "" || sipSN == "DEFAULT")
            {
                return true;
            }
            //本机图片可能由于硬盘读写问题，出现仍未保存好的情况，后续自动补传会尝试上传，此处不需要处理
            string imgProcess_Local = camResult.imgProcess.Replace(@"/", @"\").Replace(@"d\", @"D:\");
            if (!File.Exists(imgProcess_Local))
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"未找到本机处理图，跳过上传PDCA", En_Logout_Type.Pdca, true);
                return true;
            }
            bool overRange = camResult.x1 < camResult.x1_min || camResult.x1 > camResult.x1_max
                 || camResult.y1 < camResult.y1_min || camResult.y1 > camResult.y1_max
                 || camResult.r1 < camResult.r1_min || camResult.r1 > camResult.r1_max;

            if (!_pdcaParam.UploadNgProduct && overRange)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"{sipSN}数据超限，跳过上传PDCA", En_Logout_Type.Pdca, true);
                return true;
            }
            try
            {
                imgProcess_Local = imgProcess_Local.Replace(@"\",@"/");
                imgProcess_Local = imgProcess_Local.Remove(0,2);
                StringBuilder order = new StringBuilder();
                order.Append("_{\n");
                order.Append($"{sipSN}@start\n");
                order.Append($"{sipSN}@attr@Carrier_SN@{carrierSN}\n")
                    .Append($"{sipSN}@attr@Cavity@{cavity}@1@12\n")
                    .Append($"{sipSN}@attr@AE_Vendor@{_pdcaParam.AE_Vendor}\n");
                //本机
                order.Append($"{sipSN}@pdata@{paramManager.MESParam.TapeName} Inspection_X@{camResult.x1.ToString("F3")}@{camResult.x1_min.ToString("F2")}@{camResult.x1_max.ToString("F2")}@mm\n")
                    .Append($"{sipSN}@pdata@{paramManager.MESParam.TapeName} Inspection_Y@{camResult.y1.ToString("F3")}@{camResult.y1_min.ToString("F2")}@{camResult.y1_max.ToString("F2")}@mm\n")
                    .Append($"{sipSN}@pdata@{paramManager.MESParam.TapeName} Inspection_A@{camResult.r1.ToString("F2")}@{camResult.r1_min.ToString("F2")}@{camResult.r1_max.ToString("F2")}@degree\n")
                    .Append($"{sipSN}@log_file@smb://{_pdcaParam.CurrIP}{imgProcess_Local}@{_pdcaParam.UserName}@{_pdcaParam.Password}\n");
                order.Append($"{sipSN}@submit@{IoC.Get<ParamManager>().OtherSettingParam.SoftwareVersion}\n");
                order.Append("}\n");
                if (!SendData(order.ToString(), out string retorder))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{sipSN} PDCA发送失败", En_Logout_Type.Pdca, true);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "PDCA发送失败，发送: " + order.ToString(), En_Logout_Type.Pdca);
                    return false;
                }
                string returnstr = retorder.Trim('\0').Trim('\n').Trim('\r').Trim();
                if (!returnstr.StartsWith("ok@{success}@"))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{sipSN} PDCA接收格式异常", En_Logout_Type.Pdca, true);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, "PDCA接收格式异常，发送: " + order.ToString() + "接收: " + retorder, En_Logout_Type.Pdca);
                    return false;
                }
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, "PDCA返回ok，发送: " + order.ToString() + "接收: " + retorder, En_Logout_Type.Pdca);
                //删除本地pdca信息
                DeleteQuietly($@"D:\\tray\pdca\{sipSN}");
                return true;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"{sipSN} PDCA上传过程出现异常", En_Logout_Type.Pdca, true);
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "PDCA上传过程出现异常，" + ex.ToString(), En_Logout_Type.Pdca);
                return false;
            }
        }
    }
}
