using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.HIVE;
using QA.Business.Component.MES;
using QA.Business.Component.PDCA;
using QA.Business.Component.PLC;
using QA.Business.Component.Scanner;
using QA.Business.Converts;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Message;
using QA.Business.Station;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.Business.Status.GantryNo2Steps
{
    public class CameraStep : IStepStation2
    {
        #region Field
        private StepStatus _stepStatus;
        private PLC_Component _plc_Component;
        private Camera_Component _camera_Component;
        private Hive_Component _hive_Component;
        private PDCA_Component _pdca_Component;
        private MES_Component _mes_Component;
        private Scanner_TcpComponentRecheck _scanner_Component;
        private IEventAggregator _eventAggregator;
        private GlobalVariable _globalVariable;
        private CamResultToStringConvert camResultToStringConvert = new CamResultToStringConvert();
        #endregion

        public EN_RunStep RunStep
        {
            get;
        } = EN_RunStep.CameraStep;

        public CameraStep()
        {
            _stepStatus = IoC.Get<StepStatus>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _hive_Component = (Hive_Component)IoC.Get<IHive>();
            _pdca_Component = (PDCA_Component)IoC.Get<IPDCA>();
            _mes_Component = (MES_Component)IoC.Get<IMES>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _globalVariable = IoC.Get<GlobalVariable>();
            _scanner_Component = (Scanner_TcpComponentRecheck)IoC.Get<IScannerRechck>();
        }

        public async Task<(bool, EN_RunRet, EN_RunStep)> Handle<T>(T t, List<IComponent> components)
        {

            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            //进入条件：_plc_Component.IsPlcCameraReady() == true
            await Task.Delay(10);
            _globalVariable.CT.SetStartTime();
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, "板间CT：" + _globalVariable.CT.CycleCT.ToString("F2") + " s", En_Logout_Type.Run, true);
            //刷新穴位状态
            for (int cav = 1; cav <= 12; cav++)
            {
                PublishCavityStateMsg(cav, EN_TrayStatus.等待处理);
            }
            PublishCavityStateMsg(0, EN_TrayStatus.待料);

            CarrierStatus carrierStatus = _stepStatus.GetCurCarrier();
            if (carrierStatus == null)
            {
                if (_plc_Component.IsSimulateRun())
                {
                    carrierStatus = new CarrierStatus();
                    _stepStatus.AddCarrier(carrierStatus);
                }
                else
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->未获得载具信息但是收到复检扫码到位信号", En_Logout_Type.Alarm, true);
                    return (false, EN_RunRet.VisionErr, EN_RunStep.Err);
                }
            }
            //保存载具队列状态到日志，并保存列表中首个载具信息到 D:/tray/process
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->开始载具队列：{_stepStatus.GetCarrierListStr()}", En_Logout_Type.Run);



            #region 复检扫码
            if (_plc_Component.IsSimulateRun())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->空跑模式，不扫载具码", En_Logout_Type.Run, true);
            }
            else if (!_scanner_Component.Param.BUse && _hive_Component.HiveStatus == EN_HiveStatus.Engineering)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->扫码未启用", En_Logout_Type.Run, true);
            }
            else
            {
                //string carrierSN = carrierStatus.carrierSN;
                ////获取tray信息
                //if (!_stepStatus.GetTray(carrierStatus))
                //{
                //    var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, "未获取到前站Tray信息", 109);
                //    if (MessageBox.Show($"未找到{carrierSN}的Tray信息！\n是否使用自动生成的Tray信息？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                //    {
                //        _baseBiz.RemoveRunAlarm(alarm);
                //        NLogTrace.LogOut(EN_WARN_LEVEL.Warn, $"Camera->自动生成{carrierSN}的Tray信息", En_Logout_Type.Run, true);
                //    }
                //    else
                //    {
                //        _baseBiz.RemoveRunAlarm(alarm);
                //        return (false, EN_RunRet.MotionErr, EN_RunStep.Err);
                //    }
                //}
                //else
                //{
                //    int dwellStatus = 0;
                //    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                //    {
                //        if (carrierStatus.isEmpty[i] == 1)
                //        {
                //            dwellStatus |= (1 << (i));
                //        }
                //    }
                //    _plc_Component.SetTaryIsEmpty(dwellStatus);
                //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->已获取{carrierSN}的Tray信息", En_Logout_Type.Run, true);
                //}
            }
            #endregion


            #region 复检视觉
            //蛇形
            int[] idxArr = new int[] { 0, 3, 6, 9, 10, 7, 4, 1, 2, 5, 8, 11 };
            //复检，拍照后立刻给plc返回ok
            for (int i = 0; i < carrierStatus.errorCode.Length; i++)
            {
                int idx = idxArr[i];
                //根据tray中传入的空穴信息判断是否需要扫码,1为空穴，0为非空
                if (carrierStatus.isEmpty[idx] == 1)
                {
                    Thread.Sleep(200);
                    carrierStatus.lineSN[idx] = "";
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"扫码枪->{idx + 1}穴为空穴，排线码跳过扫码", En_Logout_Type.Alarm, true);
                }
                else
                {
                    //等待plc扫码到位信号
                    while (!_plc_Component.IsScanReadyRecheck())
                    {
                        if (_stepStatus.NextStep2 != RunStep)
                        {
                            return (false, EN_RunRet.HeightErr, _stepStatus.NextStep2);
                        }
                        Thread.Sleep(5);
                    }
                    if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _scanner_Component.Param.BUse)
                    {
                        _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddScannerCount(1);
                        //先扫排线码
                        if (!_scanner_Component.DataManManualTriger(out carrierStatus.lineSN[idx]))
                        {
                            carrierStatus.lineSN[idx] = "";
                            NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"扫码枪->{idx + 1}穴排线码扫码失败", En_Logout_Type.Alarm, true);
                        }
                        else
                        {
                            _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddScannerCount(0);
                            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"扫码枪->扫码完成，{idx + 1}穴排线码：{carrierStatus.lineSN[idx]}", En_Logout_Type.Run, true);
                        }
                    }
                    else
                    {
                        Thread.Sleep(200);
                        carrierStatus.lineSN[idx] = "";
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"扫码枪->{idx + 1}穴排线码跳过扫码", En_Logout_Type.Alarm, true);
                    }
                    _plc_Component.SetScanRecheckResult(EN_Result.OK);

                }
                if (_camera_Component.IsGetAllPhoto())
                {
                    TakePhotoS2(carrierStatus, idx, 1);
                }
                var ret1 = TakePhotoS1(carrierStatus, idx);
                if (_camera_Component.IsGetAllPhoto())
                {
                    TakePhotoS2(carrierStatus, idx, 3);
                }
                if (!ret1.Item1)
                {
                    return ret1;
                }
            }
            //获取所有穴位复检结果，并给plc写各穴位errorcode
            var ret = GetResultsS1(carrierStatus);
            if (!ret.Item1)
            {
                return ret;
            }
            //上传MES,先获取排线SN,再检查路由状态,最后进行MES上传
            if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _stepStatus.ParamManager.MESParam.BUse)
            {
                for (int i = 0; i < 12; i++)
                {
                    double x1pianyi = (carrierStatus.cameraResults[i].x1_max - carrierStatus.cameraResults[i].x1_min) / 2 - carrierStatus.cameraResults[i].x1 + carrierStatus.cameraResults[i].x1_min;
                    double y1pianyi = -((carrierStatus.cameraResults[i].y1_max - carrierStatus.cameraResults[i].y1_min) / 2 - carrierStatus.cameraResults[i].y1 + carrierStatus.cameraResults[i].y1_min);
                    string str1 = carrierStatus.cameraResults[i].x1.ToString("F3") +"(" + carrierStatus.cameraResults[i].x1_min.ToString("F2") + "," + carrierStatus.cameraResults[i].x1_max.ToString("F2") + ")(" + x1pianyi.ToString("F3") + ")";
                    string str2 = carrierStatus.cameraResults[i].y1.ToString("F3") +"(" + carrierStatus.cameraResults[i].y1_min.ToString("F2") + "," + carrierStatus.cameraResults[i].y1_max.ToString("F2") + ")(" + y1pianyi.ToString("F3") + ")";

                    int cav = i + 1;
                    if (carrierStatus.lineSN[i] == "" || carrierStatus.sipSN[i] == "DEFAULT" || carrierStatus.errorCode[i] != (int)EN_TrayStatus.OK)
                    {
                        continue;
                    }
                    if (!_mes_Component.GetSipSN(carrierStatus.lineSN[i],carrierStatus.carrierSN, out carrierStatus.sipSN[i]))
                    {
                        carrierStatus.routingResults[i] = EN_Routing_Result.Other;
                        carrierStatus.errorCode[i] = (int)EN_TrayStatus.MES上传NG;
                        PublishCavityStateMsg(cav, EN_TrayStatus.MES上传NG, str1, str2);
                        _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount(carrierStatus.errorCode[i]);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{cav}穴MES查询sip码失败", En_Logout_Type.Run, true);
                    }
                    if (!_mes_Component.GetRouting(carrierStatus.sipSN[i], carrierStatus.carrierSN, out carrierStatus.routingResults[i]))
                    {
                        if (carrierStatus.routingResults[i] == EN_Routing_Result.HasUploaded)
                        {
                            continue;
                        }
                        carrierStatus.errorCode[i] = (int)EN_TrayStatus.MES上传NG;
                        PublishCavityStateMsg(cav, EN_TrayStatus.MES上传NG, str1, str2);
                        _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount(carrierStatus.errorCode[i]);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{cav}穴MES查询路由信息失败", En_Logout_Type.Run, true);
                    }
                    if (carrierStatus.routingResults[i] == EN_Routing_Result.OK)
                    {
                        if (!_mes_Component.BindOne(carrierStatus, cav))
                        {
                            carrierStatus.errorCode[i] = (int)EN_TrayStatus.MES上传NG;
                            PublishCavityStateMsg(cav, EN_TrayStatus.MES上传NG,str1,str2);
                            _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount(carrierStatus.errorCode[i]);
                            NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{cav}穴MES绑定失败", En_Logout_Type.Run, true);
                        }
                        else
                        {
                            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->{cav}穴MES绑定成功", En_Logout_Type.Run, true);
                        }
                    }
                    else if (carrierStatus.routingResults[i] == EN_Routing_Result.HasUploaded)
                    {
                        carrierStatus.errorCode[i] = (int)EN_TrayStatus.OK;
                        PublishCavityStateMsg(cav, EN_TrayStatus.OK, str1, str2);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{cav}穴MES已经过站", En_Logout_Type.Run, true);
                    }
                    else
                    {
                        carrierStatus.errorCode[i] = (int)EN_TrayStatus.MES上传NG;
                        PublishCavityStateMsg(cav, EN_TrayStatus.MES上传NG, str1, str2);
                        _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount(carrierStatus.errorCode[i]);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{cav}穴路由NG，跳过MES", En_Logout_Type.Run, true);
                    }
                }
            }
            //根据sipSN重新命名图片，并将carrierStatus.camresult中的路径修改为更换后的路径
            for (int i = 0; i < carrierStatus.cameraResults.Length; i++)
            {
                if (carrierStatus.lineSN[i] == "7" || string.IsNullOrEmpty(carrierStatus.lineSN[i]))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"sip长度不是18位，跳过移图", En_Logout_Type.Run, true);
                    continue;
                }
                if (carrierStatus.errorCode[i] != (int)EN_TrayStatus.空穴)
                {
                    try
                    {
                        string oldOriginalPath = carrierStatus.cameraResults[i].imgOrigin;
                        string oldProcessedPath = carrierStatus.cameraResults[i].imgProcess;
                        var data = oldOriginalPath.Split('_');
                        string newOriginalPath = data[0] + "_" + carrierStatus.lineSN[i] + "_" + data[2];
                        data = oldProcessedPath.Split('_');
                        string newProcessedPath = data[0] + "_" + carrierStatus.lineSN[i] + "_" + data[2];
                        File.Move(oldOriginalPath, newOriginalPath);
                        File.Move(oldProcessedPath, newProcessedPath);
                        carrierStatus.cameraResults[i].imgOrigin = newOriginalPath;
                        carrierStatus.cameraResults[i].imgProcess = newProcessedPath;
                        File.Delete(oldOriginalPath);
                        File.Delete(oldProcessedPath);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"获取sip失败，跳过移图", En_Logout_Type.Run, true);
                    continue;
                }
            }

            //给PLC写最终复检结果
            if (!_plc_Component.SetCarrierErrorcode(carrierStatus.errorCode))
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1A->未能写入复检信息", En_Logout_Type.Run, true);
                return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
            }
            //给PLC处理时间
            Thread.Sleep(200);
            //写tray
            if (!_plc_Component.IsSimulateRun())
            {
                if (!_stepStatus.BackupAndWriteTray(carrierStatus))
                {
                    return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
                }
            }
            _globalVariable.CT.SetEndTime();
            //给HIVE发送machine data
            _ = Task.Run(() =>
            {
                DateTime inputTime = carrierStatus.scanTime;
                DateTime outputTime = DateTime.Now;
                for (int i = 0; i < 12; i++)
                {
                    string sipSN = carrierStatus.sipSN[i];
                    if (carrierStatus.errorCode[i] != (int)EN_TrayStatus.OK//只传ok的
                        || carrierStatus.sipSN[i] == ""
                        || carrierStatus.sipSN[i] == "DEFAULT")
                    {
                        continue;
                    }
                    _hive_Component.SendMachineData(sipSN, true, inputTime, outputTime);
                }
            });
            //上传pdca
            if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _pdca_Component.Param.BUse)
            {
                _ = Task.Run(() =>
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->{carrierStatus.carrierSN}开始上传PDCA", En_Logout_Type.Run, true);
                    if (_pdca_Component.UploadPdcaAll(carrierStatus, out string info))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->{carrierStatus.carrierSN} PDCA上传成功", En_Logout_Type.Run, true);
                    }
                    else
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"Camera->{carrierStatus.carrierSN} PDCA上传失败，{info}", En_Logout_Type.Run, true);
                    }
                });
            }
            #endregion

            //给PLC处理时间
            await Task.Delay(200);
            _stepStatus.RemoveCurCarrier();
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->结束载具队列：{_stepStatus.GetCarrierListStr()}", En_Logout_Type.Run);
            if (_stepStatus.Carriers.Count > 1)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->当前载具数目大于1", En_Logout_Type.Run, true);
                return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
            }
            return (false, EN_RunRet.TaskOk, EN_RunStep.AutoProcess);
        }

        /// <summary>
        /// 复检拍照
        /// </summary>
        /// <param name="carrierStatus"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private (bool, EN_RunRet, EN_RunStep) TakePhotoS1(CarrierStatus carrierStatus, int idx)
        {
            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            //等待相机到位信号
            while (true)
            {
                if (_stepStatus.NextStep2 != RunStep)
                {
                    return (false, EN_RunRet.HeightErr, _stepStatus.NextStep2);
                }
                if (_plc_Component.IsPlcCameraReady() && _plc_Component.GetPlcIdxNow() == idx)
                {
                    break;
                }
            }
            //空跑处理
            if (_plc_Component.IsSimulateRun())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->空跑模式，跳过{idx + 1}穴复检", En_Logout_Type.Run, true);
                _plc_Component.SetCameraFinished(EN_Result.OK);
                // 给PLC处理时间
                Thread.Sleep(200);
                _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount((int)EN_TrayStatus.OK);
                _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddUph();
                _stepStatus.OutputPerHour.AddOne(true);
                PublishCavityStateMsg(idx + 1, EN_TrayStatus.OK, "空跑中", "空跑中", "空跑中");
                return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
            //禁用处理
            if (!_stepStatus.CurrentProcedure.GetVisionPointByCavityNum(idx + 1).IsUsed)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->{idx + 1}穴禁用，跳过{idx + 1}穴复检", En_Logout_Type.Run, true);
                _plc_Component.SetCameraFinished(EN_Result.OK);
                // 给PLC处理时间
                Thread.Sleep(200);
                return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
            //视觉分析
            //拍照前延时，防止轴抖动造成误差
            Thread.Sleep(_stepStatus.ParamManager.CameraParam.DelayPhtotTime);
            //第二次失败再弹窗
            if (!_camera_Component.GetCameraResultS1(carrierStatus, idx))
            {
                while (!_camera_Component.GetCameraResultS1(carrierStatus, idx))
                {
                    if (_stepStatus.NextStep2 != RunStep) return (false, EN_RunRet.MotionErr, _stepStatus.NextStep2);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1->{idx + 1}穴视觉软件失连", En_Logout_Type.Alarm, true);
                    var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, $"视觉软件失连", 103);
                    if (MessageBox.Show("视觉软件失连，确定重试吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _baseBiz.RemoveRunAlarm(alarm);
                        continue;
                    }
                    else
                    {
                        _baseBiz.RemoveRunAlarm(alarm);
                        return (false, EN_RunRet.VisionErr, EN_RunStep.Err);
                    }
                }
            }
            PublishCavityStateMsg(idx + 1, EN_TrayStatus.拍照完成);
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->{idx + 1}穴复检拍照完成", En_Logout_Type.Run, true);
            if (_camera_Component.IsGetAllPhoto())
            {
                if (!_camera_Component.SaveAllPhoto(carrierStatus, idx, 2))
                {
                    while (!_camera_Component.SaveAllPhoto(carrierStatus, idx, 2))
                    {
                        if (_stepStatus.NextStep2 != RunStep) return (false, EN_RunRet.MotionErr, _stepStatus.NextStep2);
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1->{idx + 1}穴视觉软件失连", En_Logout_Type.Alarm, true);
                        var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, $"视觉软件失连", 103);
                        if (MessageBox.Show("视觉软件失连，确定重试吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            _baseBiz.RemoveRunAlarm(alarm);
                            continue;
                        }
                        else
                        {
                            _baseBiz.RemoveRunAlarm(alarm);
                            return (false, EN_RunRet.VisionErr, EN_RunStep.Err);
                        }
                    }
                }
            }
            _plc_Component.SetCameraFinished(EN_Result.OK);
            return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
        }
        /// <summary>
        /// 存全图拍照
        /// </summary>
        /// <param name="carrierStatus"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private (bool, EN_RunRet, EN_RunStep) TakePhotoS2(CarrierStatus carrierStatus, int idx,int times)
        {
            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            //等待相机到位信号
            while (true)
            {
                if (_stepStatus.NextStep2 != RunStep)
                {
                    return (false, EN_RunRet.HeightErr, _stepStatus.NextStep2);
                }
                if (_plc_Component.IsPlcCameraReady() && _plc_Component.GetPlcIdxNow() == idx)
                {
                    break;
                }
            }
            //空跑处理
            if (_plc_Component.IsSimulateRun())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->空跑模式，跳过{idx + 1}穴复检", En_Logout_Type.Run, true);
                _plc_Component.SetCameraFinished(EN_Result.OK);
                // 给PLC处理时间
                Thread.Sleep(200);
                _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount((int)EN_TrayStatus.OK);
                _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddUph();
                _stepStatus.OutputPerHour.AddOne(true);
                PublishCavityStateMsg(idx + 1, EN_TrayStatus.OK, "空跑中", "空跑中", "空跑中");
                return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
            //禁用处理
            if (!_stepStatus.CurrentProcedure.GetVisionPointByCavityNum(idx + 1).IsUsed)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->{idx + 1}穴禁用，跳过{idx + 1}穴复检", En_Logout_Type.Run, true);
                _plc_Component.SetCameraFinished(EN_Result.OK);
                // 给PLC处理时间
                Thread.Sleep(200);
                return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
            //视觉分析
            //拍照前延时，防止轴抖动造成误差
            Thread.Sleep(_stepStatus.ParamManager.CameraParam.DelayPhtotTime);
            //第二次失败再弹窗
            if (!_camera_Component.SaveAllPhoto(carrierStatus, idx,times))
            {
                while (!_camera_Component.SaveAllPhoto(carrierStatus, idx, times))
                {
                    if (_stepStatus.NextStep2 != RunStep) return (false, EN_RunRet.MotionErr, _stepStatus.NextStep2);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1->{idx + 1}穴视觉软件失连", En_Logout_Type.Alarm, true);
                    var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, $"视觉软件失连", 103);
                    if (MessageBox.Show("视觉软件失连，确定重试吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        _baseBiz.RemoveRunAlarm(alarm);
                        continue;
                    }
                    else
                    {
                        _baseBiz.RemoveRunAlarm(alarm);
                        return (false, EN_RunRet.VisionErr, EN_RunStep.Err);
                    }
                }
            }
            PublishCavityStateMsg(idx + 1, EN_TrayStatus.拍照完成);
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1->{idx + 1}穴复检拍照完成", En_Logout_Type.Run, true);
            _plc_Component.SetCameraFinished(EN_Result.OK);
            return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
        }

        /// <summary>
        /// 获取AB板拍照所有xyr
        /// </summary>
        /// <param name="carrierStatus"></param>
        /// <returns></returns>
        private (bool, EN_RunRet, EN_RunStep) GetResultsS1(CarrierStatus carrierStatus)
        {
            var _baseBiz = IoC.Get<IBaseBiz>() as BaseBiz;
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"Camera->等待PLC要视觉结果", En_Logout_Type.Run, true);
            //等待PLC要视觉结果
            while (!_plc_Component.IsPlcCameraReadyResult())
            {
                if (_stepStatus.NextStep2 != RunStep)
                {
                    return (false, EN_RunRet.HeightErr, _stepStatus.NextStep2);
                }
                Thread.Sleep(5);
            }

            //空跑处理
            if (_plc_Component.IsSimulateRun())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1A->空跑模式，跳过获取所有穴位复检结果,随机出现NG项次", En_Logout_Type.Run, true);
                Random random = new Random();
                for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                {
                    carrierStatus.errorCode[i] = (int)EN_TrayStatus.OK;
                }
                if (random.Next(0, 10) > 2)
                {
                    for (int i = 0; i < carrierStatus.errorCode.Length; i++)
                    {
                        carrierStatus.errorCode[i] = random.Next((int)EN_TrayStatus.空穴, (int)EN_TrayStatus.禁用);
                        Thread.Sleep(10);
                    }
                }
                if (!_plc_Component.SetCarrierErrorcode(carrierStatus.errorCode))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1A->未能写入复检信息", En_Logout_Type.Run, true);
                    return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
                }
                // 给PLC处理时间
                Thread.Sleep(200);
                return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
            }
        S1A:
            //需要在3s内获取所有视觉的结果
            DateTime time = DateTime.Now;
            bool finished = false;
            while ((DateTime.Now - time).TotalMilliseconds < 3000)
            {
                Thread.Sleep(2000);
                _camera_Component.GetCameraResultS1A(carrierStatus, out finished,out string[] lineSN);
                if (finished)
                {
                    break;
                }
            }
            if (!finished)
            {
                if (_stepStatus.NextStep2 != RunStep) return (false, EN_RunRet.MotionErr, _stepStatus.NextStep2);
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1A->未能获取所有穴位复检结果", En_Logout_Type.Alarm, true);
                var alarm = _baseBiz.AddRunAlarm(EN_WarnModules.DataMan, $"未能获取所有穴位复检结果", 104);
                if (MessageBox.Show("未能获取所有穴位复检结果，确定重试吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    _baseBiz.RemoveRunAlarm(alarm);
                    goto S1A;
                }
                else
                {
                    _baseBiz.RemoveRunAlarm(alarm);
                    return (false, EN_RunRet.VisionErr, EN_RunStep.Err);
                }
            }
            for (int idx = 0; idx < 12; idx++)
            {
                WriteDataToCsv(carrierStatus, idx);
                //显示测量值XY超限多少
                double x1pianyi = (carrierStatus.cameraResults[idx].x1_max - carrierStatus.cameraResults[idx].x1_min) / 2 - carrierStatus.cameraResults[idx].x1 + carrierStatus.cameraResults[idx].x1_min;
                double y1pianyi = -((carrierStatus.cameraResults[idx].y1_max - carrierStatus.cameraResults[idx].y1_min) / 2 - carrierStatus.cameraResults[idx].y1 + carrierStatus.cameraResults[idx].y1_min);
                string str1 = carrierStatus.cameraResults[idx].x1.ToString("F3") +
                    "(" + carrierStatus.cameraResults[idx].x1_min.ToString("F2") + "," + carrierStatus.cameraResults[idx].x1_max.ToString("F2") + ")(" + x1pianyi.ToString("F3") + ")";
                string str2 = carrierStatus.cameraResults[idx].y1.ToString("F3") +
                    "(" + carrierStatus.cameraResults[idx].y1_min.ToString("F2") + "," + carrierStatus.cameraResults[idx].y1_max.ToString("F2") + ")(" + y1pianyi.ToString("F3") + ")";
                string str3 = "";
                //根据结果修改errorcode，并在主界面显示
                if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.空穴)
                {
                    PublishCavityStateMsg(idx + 1, EN_TrayStatus.空穴);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1A->{idx + 1}穴空穴", En_Logout_Type.Run, true);
                    carrierStatus.errorCode[idx] = (int)EN_TrayStatus.空穴;
                }
                else if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.OK)
                {
                    PublishCavityStateMsg(idx + 1, EN_TrayStatus.OK, str1, str2, str3);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1A->{idx + 1}穴视觉OK, {str1}, {str2}", En_Logout_Type.Run, true);
                    carrierStatus.errorCode[idx] = (int)EN_TrayStatus.OK;
                    _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddUph();
                    _stepStatus.OutputPerHour.AddOne(true);
                }
                else
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CameraS1A->{idx + 1}穴视觉NG, {str1}, {str2}", En_Logout_Type.Run, true);
                    if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.Mark定位失败)
                    {
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.Mark定位失败;
                    }
                    else if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.Tape缺失)
                    {
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.Tape缺失;
                    }
                    else if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.Tape大离型纸未撕)
                    {
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.Tape大离型纸未撕;
                    }
                    else if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.Tape抓边失败)
                    {
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.Tape抓边失败;
                    }
                    else if (carrierStatus.cameraResults[idx].camResult == EN_CamResult.排线SN扫码失败)
                    {
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.排线SN扫码失败;
                    }
                    else
                    {
                        //其余NG情况都按贴装偏移算
                        carrierStatus.errorCode[idx] = (int)EN_TrayStatus.Tape贴装偏移;
                    }
                    PublishCavityStateMsg(idx + 1, (EN_TrayStatus)carrierStatus.errorCode[idx], str1, str2, str3);
                    _stepStatus.OutputPerHour.AddOne(false);
                }

                if ((_hive_Component.HiveStatus == EN_HiveStatus.Running || _hive_Component.HiveStatus == EN_HiveStatus.Idle) || _stepStatus.ParamManager.MESParam.BUse)
                {
                    _stepStatus.ReacheckNGPerHour.AddOne(carrierStatus, idx, true);
                }
                {
                    _stepStatus.ReacheckNGPerHour.AddOne(carrierStatus, idx, false);
                }
                _stepStatus.CacheParamManager.HomeUiParam.Statistic.AddErrorCodeCount(carrierStatus.errorCode[idx]);
            }
            //if (!_plc_Component.SetCarrierErrorcode(carrierStatus.errorCode))
            //{
            //    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CameraS1A->未能写入复检信息", En_Logout_Type.Run, true);
            //    return (false, EN_RunRet.DefaultErr, EN_RunStep.Err);
            //}
            ////给PLC处理时间
            //Thread.Sleep(200);
            return (true, EN_RunRet.TaskOk, EN_RunStep.CameraStep);
        }

        private void PublishCavityStateMsg(int cavity, EN_TrayStatus status, string xStr = "", string yStr = "", string rStr = "")
        {
            _eventAggregator.Publish(new CarrierInfoPanelMessage()
            {
                Cavity = cavity,
                Status = status,
                XStr = xStr,
                YStr = yStr,
                RStr = rStr,
            }, action => { Task.Run(action); });
        }

        /// <summary>
        /// 将某个指定穴位的数据记录到表格中
        /// </summary>
        private void WriteDataToCsv(CarrierStatus carrierStatus, int idx)
        {
            //string dir0 = $@"D:\CsvData\产品信息";
            string dir0 = $@"D:\QKProject\Data\Product";
            if (!Directory.Exists(dir0))
            {
                Directory.CreateDirectory(dir0);
            }
            string file = $@"{dir0}\{DateTime.Now.ToString("yyyy-MM-dd")}.csv";
            bool existCsv = File.Exists(file);
            using (StreamWriter sw = new StreamWriter(file, true))
            {
                if (!existCsv)
                {
                    sw.WriteLine($"拍照时间,载具码,SIP码,穴位号,吸嘴号,复检结果,X1,Y1,R1");
                }
                sw.WriteLine(
                    carrierStatus.cameraResults[idx].time.ToString("yyyy-MM-dd HH:mm:ss:fff") + "," +
                    carrierStatus.carrierSN + "," +
                    carrierStatus.sipSN[idx] + "," +
                    (idx + 1) + "," +
                    carrierStatus.Tape_Nozzle[idx] + "," +
                    (string)camResultToStringConvert.Convert(carrierStatus.cameraResults[idx].camResult, null, null, null) + "," +
                    carrierStatus.cameraResults[idx].x1.ToString("F3") + "," +
                    carrierStatus.cameraResults[idx].y1.ToString("F3") + "," +
                    carrierStatus.cameraResults[idx].r1.ToString("F2")
                    );
            }
        }

    }
}
