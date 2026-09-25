using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using HandyControl.Controls;
using QA.Business.Component.Camera;
using QA.Business.Component.Motion.Googol;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Steps;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.SpotCheckPages.ViewModels
{
    [Export("SpotCheckCameraPageViewModel", typeof(ISpotPageViewModel))]
    class SpotCheckCameraPageViewModel : Screen, INotifyPropertyChanged, ISpotPageViewModel
    {
        #region Field
        private IEventAggregator _eventAggregator = null;
        private IWindowManager _windowManager = null;
        private MotionGoogol_Component _mGoogol_Component;
        private CacheParamManager _cacheParamManager;
        private Camera_Component _camera_Component;
        private PLC_Component _plc_Component;
        private StepStatus _stepStatus;

        private bool _stopTest = false;

        //private string _basePath = AppDomain.CurrentDomain.BaseDirectory + "CamTest";
        private string _basePath = @"D:\QKProject\Data\CamTest";
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "相机点检";

        public ushort OrderID { get; set; } = 5;

        private float _testCount = 0;
        public float TestCount
        {
            get => _testCount;
            set
            {
                _testCount = value;
                NotifyOfPropertyChange(() => TestCount);
            }
        }

        private float _axisWaitTime = 0;
        public float AxisWaitTime
        {
            get => _axisWaitTime;
            set
            {
                _axisWaitTime = value;
                NotifyOfPropertyChange(() => AxisWaitTime);
            }
        }

        private string _upCamStaticTestPos;
        public string UpCamStaticTestPos
        {
            get => _upCamStaticTestPos;
            set
            {
                _upCamStaticTestPos = value;
                NotifyOfPropertyChange(() => UpCamStaticTestPos);
            }
        }

        private string _downCamStaticTestPos;
        public string DownCamStaticTestPos
        {
            get => _downCamStaticTestPos;
            set
            {
                _downCamStaticTestPos = value;
                NotifyOfPropertyChange(() => DownCamStaticTestPos);
            }
        }

        private string _upCamDynamicTestStartPos;
        public string UpCamDynamicTestStartPos
        {
            get => _upCamDynamicTestStartPos;
            set
            {
                _upCamDynamicTestStartPos = value;
                NotifyOfPropertyChange(() => UpCamDynamicTestStartPos);
            }
        }

        private string _upCamDynamicTestEndPos;
        public string UpCamDynamicTestEndPos
        {
            get => _upCamDynamicTestEndPos;
            set
            {
                _upCamDynamicTestEndPos = value;
                NotifyOfPropertyChange(() => UpCamDynamicTestEndPos);
            }
        }

        private string _downCamDynamicTestStartPos;
        public string DownCamDynamicTestStartPos
        {
            get => _downCamDynamicTestStartPos;
            set
            {
                _downCamDynamicTestStartPos = value;
                NotifyOfPropertyChange(() => DownCamDynamicTestStartPos);
            }
        }

        private string _downCamDynamicTestEndPos;
        public string DownCamDynamicTestEndPos
        {
            get => _downCamDynamicTestEndPos;
            set
            {
                _downCamDynamicTestEndPos = value;
                NotifyOfPropertyChange(() => DownCamDynamicTestEndPos);
            }
        }
        private string _sendStr;
        public string SendStr
        {
            get => _sendStr;
            set
            {
                _sendStr = value;
                NotifyOfPropertyChange(() => SendStr);
            }
        }
        #endregion

        #region Constructor
        public SpotCheckCameraPageViewModel()
        {
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            _stepStatus = IoC.Get<StepStatus>();
            UpCamStaticTestPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCamStaticTestPos);
            DownCamStaticTestPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.DownCamStaticTestPos);
            UpCamDynamicTestStartPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCamDynamicTestStartPos);
            UpCamDynamicTestEndPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCamDynamicTestEndPos);
            DownCamDynamicTestStartPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.DownCamDynamicTestStartPos);
            DownCamDynamicTestEndPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.DownCamDynamicTestEndPos);

            TestCount = _cacheParamManager.manualPositionParam.TestCount;
            AxisWaitTime = _cacheParamManager.manualPositionParam.AxisWaitTime;

            //if (!Directory.Exists(_basePath))
            //{
            //    Directory.CreateDirectory(_basePath);
            //}
            //if (!Directory.Exists(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd")))
            //{
            //    Directory.CreateDirectory(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd"));
            //}
        }
        #endregion

        #region Method

        private float[] GetPos(bool isUpCam, bool isStatic, bool isStart)
        {
            return isStatic
                      ? isUpCam
                          ? _cacheParamManager.manualPositionParam.UpCamStaticTestPos
                          : _cacheParamManager.manualPositionParam.DownCamStaticTestPos
                      : isUpCam
                          ? isStart
                              ? _cacheParamManager.manualPositionParam.UpCamDynamicTestStartPos
                              : _cacheParamManager.manualPositionParam.UpCamDynamicTestEndPos
                          : isStart
                              ? _cacheParamManager.manualPositionParam.DownCamDynamicTestStartPos
                              : _cacheParamManager.manualPositionParam.DownCamDynamicTestEndPos;
        }

        private void SetPosStr(bool isUpCam, bool isStatic, bool isStart, float[] pos)
        {
            if (isStatic)
            {
                if (isUpCam)
                {
                    UpCamStaticTestPos = NLogTrace.GetFloatArrayString(pos);
                }
                else
                {
                    DownCamStaticTestPos = NLogTrace.GetFloatArrayString(pos);
                }
            }
            else
            {
                if (isUpCam)
                {
                    if (isStart)
                    {
                        UpCamDynamicTestStartPos = NLogTrace.GetFloatArrayString(pos);
                    }
                    else
                    {
                        UpCamDynamicTestEndPos = NLogTrace.GetFloatArrayString(pos);
                    }
                }
                else
                {
                    if (isStart)
                    {
                        DownCamDynamicTestStartPos = NLogTrace.GetFloatArrayString(pos);
                    }
                    else
                    {
                        DownCamDynamicTestEndPos = NLogTrace.GetFloatArrayString(pos);
                    }
                }
            }
        }

        private void SetTestPos(bool isUpCam, bool isStatic, bool isStart = true)
        {
            if (MessageBox.Show("确定设置坐标吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                //float[] pos = GetPos(isUpCam, isStatic, isStart);
                //if (isUpCam)
                //{
                //    pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
                //    pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
                //    pos[2] = 0;
                //    pos[3] = 0;
                //}
                //else
                //{
                //    pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                //    pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                //    pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                //    pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                //}
                //SetPosStr(isUpCam, isStatic, isStart, pos);
                //_cacheParamManager.SaveAllParam();
                //NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"设定坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
            }
        }

        private async void MoveTestPos(bool isUpCam, bool isStatic, bool isStart = true)
        {
            if (MessageBox.Show("确定空移吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //float[] pos = GetPos(isUpCam, isStatic, isStart);
                    ////false说明有轴在运动
                    //if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    //{
                    //    MessageBox.Warning("轴系在运动，等静止再操作！");
                    //    return;
                    //}
                    //if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
                    //_stepStatus.SetAllCylindersUp();
                    //if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
                    //if (isUpCam)
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { pos[0], pos[1] }, true, true)) return;
                    //}
                    //else
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { pos[0], pos[1] }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
                    //    _stepStatus.SetCylinderDown(1);
                    //}
                    //MessageBox.Success("移动完毕！");
                });
            }
        }

        public async void DoTest(bool isUpCam, bool isStatic)
        {
            if (MessageBox.Show("确定执行测试吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    //// 移动
                    ////false说明有轴在运动
                    //if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    //{
                    //    MessageBox.Warning("轴系在运动，等静止再操作！");
                    //    return;
                    //}
                    //if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
                    //_stepStatus.SetAllCylindersUp();
                    //if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
                    //// 拍照位
                    //float[] posStart = GetPos(isUpCam, isStatic, true);
                    //// 移动位
                    //float[] posEnd = GetPos(isUpCam, isStatic, false);
                    //if (isUpCam)
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { posStart[0], posStart[1] }, true, true)) return;
                    //}
                    //else
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { posStart[0], posStart[1] }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, posStart[2], true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, posStart[3], true, true)) return;
                    //    _stepStatus.SetCylinderDown(1);
                    //}
                    //if (isStatic)
                    //{
                    //    Thread.Sleep(500);
                    //}
                    //// 开始测试
                    //string sucMaterial = _stepStatus.ParamManager.CameraParam.SucMaterial;
                    //string targetMaterial = _stepStatus.ParamManager.CameraParam.TargetMaterial;
                    //List<float[]> testData = new List<float[]>();
                    //for (int i = 0; i < TestCount; i++)
                    //{
                    //    if (_stopTest == false)
                    //    {
                    //        if (!isStatic)
                    //        {
                    //            En_StationNo stationNo = isUpCam ? En_StationNo.StationNo1 : En_StationNo.StationNo2;
                    //            if (!_mGoogol_Component.MoveAbsoluteXY(stationNo, new float[2] { posEnd[0], posEnd[1] }, true, true)) return;
                    //            if (!_mGoogol_Component.MoveAbsoluteXY(stationNo, new float[2] { posStart[0], posStart[1] }, true, true)) return;
                    //            Thread.Sleep(200);
                    //        }
                    //        float[] outpos = new float[4];
                    //        if (isUpCam)
                    //        {
                    //            if (!_camera_Component.ProductProcess_TLT(1, "UpCamStatic", 1, sucMaterial, targetMaterial, 1, posStart[0], posStart[1], 0, outpos))
                    //            {
                    //                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"定位载具物料失败,Index:{i.ToString()}", En_Logout_Type.SpotCheck);
                    //                MessageBox.Warning($"定位载具物料失败,Index:{i.ToString()}");
                    //                return;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (!_camera_Component.ProductProcess_TLN(2, "DownCamStatic", 1, sucMaterial, targetMaterial, 1, posStart[0], posStart[1], 0, outpos))
                    //            {
                    //                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"静态定位吸嘴物料失败,Index:{i.ToString()}", En_Logout_Type.SpotCheck);
                    //                MessageBox.Warning($"定位吸嘴物料失败,Index:{i.ToString()}");
                    //                return;
                    //            }
                    //        }
                    //        testData.Add(new float[3] { outpos[0], outpos[1], outpos[2] });
                    //    }
                    //    else
                    //    {
                    //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"停止操作", En_Logout_Type.SpotCheck);
                    //        MessageBox.Warning("停止操作");
                    //    }
                    //}
                    //// 记录数据
                    //if (!Directory.Exists(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd")))
                    //{
                    //    Directory.CreateDirectory(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd"));
                    //}
                    //string s = isUpCam ? "_UpCam" : "_DownCam";
                    //s += isStatic ? "_Static" : "_Dynamic";
                    //if (!Directory.Exists(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + DateTime.Now.ToString("yyyyMMdd") + s))
                    //{
                    //    Directory.CreateDirectory(_basePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + DateTime.Now.ToString("yyyyMMdd") + s);
                    //}
                    //string resultPath = _basePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + DateTime.Now.ToString("yyyyMMdd") + s + "\\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".csv";
                    //StreamWriter file = new StreamWriter(resultPath, true);
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("编号");
                    //sb.Append(",");
                    //sb.Append("相机X");
                    //sb.Append(",");
                    //sb.Append("相机Y");
                    //sb.Append(",");
                    //sb.Append("相机角度");
                    //sb.Append("\r\n");
                    //for (int i = 0; i < testData.Count; i++)
                    //{
                    //    sb.Append((i + 1).ToString());
                    //    sb.Append(",");
                    //    sb.Append(testData[i][0].ToString("f3"));
                    //    sb.Append(",");
                    //    sb.Append(testData[i][1].ToString("f3"));
                    //    sb.Append(",");
                    //    sb.Append(testData[i][2].ToString("f3"));
                    //    sb.Append("\r\n");
                    //}
                    //file.Write(sb.ToString());
                    //file.Close();
                    //file.Dispose();
                    //MessageBox.Success("已写入文件！");

                    _stopTest = false;
                    string cpkState = isStatic ? "静态" : "动态";
                    _plc_Component.SetCPK(0, isStatic ? EN_PLC_CPK.Static : EN_PLC_CPK.Dynamic);
                    string dir = "D:\\CPK\\";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string file = dir + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + " " + cpkState + "CPK" + ".csv";
                    using (StreamWriter sw = new StreamWriter(file, true))
                    {
                        if (isStatic)
                        {
                            Thread.Sleep(200);
                        }
                        //执行101次，抛弃第一次数据，避免误差
                        for (int i = 0; i < (int)TestCount + 1; i++)
                        {
                            if (_stopTest)
                            {
                                _stopTest = false;
                                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"停止操作", En_Logout_Type.SpotCheck);
                                MessageBox.Warning("已停止操作！");
                                break;
                            }
                            //等待plc给相机到位信号
                            while (!_plc_Component.IsPlcCameraReady())
                            {
                                Thread.Sleep(50);
                            }
                            //相机已经到位，延时200ms再拍照防止抖动影响。静态只需要第一次加延时
                            Thread.Sleep(isStatic ? 50 : 200);
                            //拍照并视觉处理
                            bool isOk = true;
                            string x = "0";
                            string y = "0";
                            if (!_camera_Component.Cpk(out isOk, out x, out y))
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "与视觉软件通信异常！", En_Logout_Type.Run, true);
                                MessageBox.Warning("与视觉软件通信异常");
                                break;
                            }

                            if (!isOk)
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "CPK模板检测失败！", En_Logout_Type.Run, true);
                                MessageBox.Warning("CPK模板检测失败");
                                break;
                            }
                            //抛弃第一次数据，避免误差
                            if (i != 0)
                            {
                                sw.WriteLine(i + "," + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff") + "," + x + "," + y);
                            }
                            else
                            {
                                sw.WriteLine("测试次数,时间,x,y");
                            }
                            //如果是动态，需要给plc写视觉完成信号
                            if (!isStatic)
                            {
                                _plc_Component.SetCameraFinished(EN_Result.OK);
                                //给plc写信号后要等一下
                                Thread.Sleep(200);
                            }
                        }
                    }
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, cpkState + "CPK完成，数据已保存！", En_Logout_Type.Run, true);
                    MessageBox.Info(cpkState + "CPK完成，数据已保存！");
                    //给plc写cpk结束信号
                    _plc_Component.SetCPK(0, EN_PLC_CPK.None);
                });
            }
        }

        public async void DoAxisTest(bool isUpCam)
        {
            if (MessageBox.Show("确定执行测试吗？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await Task.Run(() =>
                {
                    //// 移动
                    ////false说明有轴在运动
                    //if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    //{
                    //    MessageBox.Warning("轴系在运动，等静止再操作！");
                    //    return;
                    //}
                    //if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.High)) return;
                    //_stepStatus.SetAllCylindersUp();
                    //if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
                    //// 拍照位/静止位
                    //float[] posStart = GetPos(isUpCam, false, true);
                    //// 移动位
                    //float[] posEnd = GetPos(isUpCam, false, false);
                    //if (isUpCam)
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { posStart[0], posStart[1] }, true, true)) return;
                    //}
                    //else
                    //{
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { posStart[0], posStart[1] }, true, true)) return;
                    //    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, posStart[2], true, true)) return;
                    //}
                    //int waitTime = (int)(AxisWaitTime * 1000);
                    //Thread.Sleep(waitTime);
                    //// 开始测试
                    //for (int i = 0; i < TestCount; i++)
                    //{
                    //    if (_stopTest == false)
                    //    {
                    //        En_StationNo stationNo = isUpCam ? En_StationNo.StationNo1 : En_StationNo.StationNo2;
                    //        if (!_mGoogol_Component.MoveAbsoluteX2Y2Z2(stationNo, new float[] { posEnd[0], posEnd[1], posEnd[2] }, true, true)) return;
                    //        if (!_mGoogol_Component.MoveAbsoluteX2Y2Z2(stationNo, new float[] { posStart[0], posStart[1], posStart[2] }, true, true)) return;
                    //        Thread.Sleep(waitTime);
                    //    }
                    //    else
                    //    {
                    //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"停止操作", En_Logout_Type.SpotCheck);
                    //        MessageBox.Warning("停止操作");
                    //    }
                    //}
                    //MessageBox.Success("移动完毕！");

                    _stopTest = false;
                    bool isStatic = false;
                    _plc_Component.SetCPK(0, isStatic ? EN_PLC_CPK.Static : EN_PLC_CPK.Dynamic);
                    //执行101次，抛弃第一次数据，避免误差
                    for (int i = 0; i < (int)TestCount; i++)
                    {
                        if (_stopTest)
                        {
                            _stopTest = false;
                            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"停止操作", En_Logout_Type.SpotCheck);
                            MessageBox.Warning("已停止操作！");
                            break;
                        }
                        //等待plc给相机到位信号
                        while (!_plc_Component.IsPlcCameraReady())
                        {
                            Thread.Sleep(50);
                        }
                        Thread.Sleep((int)(AxisWaitTime * 1000));
                        //需要给plc写视觉完成信号
                        _plc_Component.SetCameraFinished(EN_Result.OK);
                        //给plc写信号后要等一下
                        Thread.Sleep(200);
                    }
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "移动完毕！", En_Logout_Type.Run, true);
                    MessageBox.Info("移动完毕！");
                    //给plc写cpk结束信号
                    _plc_Component.SetCPK(0, EN_PLC_CPK.None);
                });
            }
        }

        public void SetUpCamStaticTestPos()
        {
            SetTestPos(true, true);
        }

        public void MoveUpCamStaticTestPos()
        {
            MoveTestPos(true, true);
        }

        public void DoUpCamStaticTest()
        {
            DoTest(true, true);
        }

        public void SetDownCamStaticTestPos()
        {
            SetTestPos(false, true);
        }

        public void MoveDownCamStaticTestPos()
        {
            MoveTestPos(false, true);
        }

        public void DoDownCamStaticTest()
        {
            DoTest(false, true);
        }

        public void SetUpCamDynamicTestStartPos()
        {
            SetTestPos(true, false, true);
        }

        public void MoveUpCamDynamicTestStartPos()
        {
            MoveTestPos(true, false, true);
        }

        public void SetUpCamDynamicTestEndPos()
        {
            SetTestPos(true, false, false);
        }

        public void MoveUpCamDynamicTestEndPos()
        {
            MoveTestPos(true, false, false);
        }

        public void DoUpCamDynamicTest()
        {
            DoTest(true, false);
        }

        public void DoUpCamAxisTest()
        {
            DoAxisTest(true);
        }

        public void SetDownCamDynamicTestStartPos()
        {
            SetTestPos(false, false, true);
        }

        public void MoveDownCamDynamicTestStartPos()
        {
            MoveTestPos(false, false, true);
        }

        public void SetDownCamDynamicTestEndPos()
        {
            SetTestPos(false, false, false);
        }

        public void MoveDownCamDynamicTestEndPos()
        {
            MoveTestPos(false, false, false);
        }

        public void DoDownCamDynamicTest()
        {
            DoTest(false, false);
        }

        public void DoDownCamAxisTest()
        {
            DoAxisTest(false);
        }

        public void StopTest()
        {
            _stopTest = true;
        }

        public void ReadData()
        {
            string dir = "D:\\CPK\\";
            if (Directory.Exists(dir))
            {
                System.Diagnostics.Process.Start(dir);
            }
            else
            {
                MessageBox.Warning("未检测到数据！");
            }
        }

        public void NumUDTestCountChanged(object obj)
        {
            NumericUpDown val = (NumericUpDown)obj;

            _cacheParamManager.manualPositionParam.TestCount = (int)val.Value;

            _cacheParamManager.SaveManualPositionParam();
        }

        public void NumUDTestCavityChanged(object obj)
        {
            NumericUpDown val = (NumericUpDown)obj;

            _cacheParamManager.manualPositionParam.AxisWaitTime = (float)val.Value;

            _cacheParamManager.SaveManualPositionParam();
        }

        public void SendCameraTest()
        {
            string resstr = string.Empty;
            if (!_camera_Component.SendData(SendStr, ref resstr))
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"视觉通信失败，指令:{SendStr}", En_Logout_Type.Alarm, true);
            }
            else
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"视觉通信返回:{resstr}", En_Logout_Type.Run, true);
            }
        }

        #endregion
    }
}
