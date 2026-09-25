using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.Motion.Googol;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA_Infrastructure.NLogOut;

namespace QA.SpotCheckPages.Calibration.ViewModels
{
    [Export(typeof(ICalibrationViewModel))]
    public class JointCalibViewModel : Screen, INotifyPropertyChanged, ICalibrationViewModel
    {
        #region Field
        private Camera_Component _camera_Component;
        private ParamManager _paramManager;
        private MotionGoogol_Component _mGoogol_Component;
        private CacheParamManager _cacheParamManager;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "联合标定";

        public ushort OrderID { get; set; } = 1;

        private string _curFetchPlateJointCalibPosNozzleNo1;
        public string CurFetchPlateJointCalibPosNozzleNo1
        {
            get => _curFetchPlateJointCalibPosNozzleNo1;
            set
            {
                _curFetchPlateJointCalibPosNozzleNo1 = value;
                NotifyOfPropertyChange(() => CurFetchPlateJointCalibPosNozzleNo1);
            }
        }

        private string _curFetchPlateJointCalibPosNozzleNo2;
        public string CurFetchPlateJointCalibPosNozzleNo2
        {
            get => _curFetchPlateJointCalibPosNozzleNo2;
            set
            {
                _curFetchPlateJointCalibPosNozzleNo2 = value;
                NotifyOfPropertyChange(() => CurFetchPlateJointCalibPosNozzleNo2);
            }
        }

        private string _curFetchPlateJointCalibPosNozzleNo3;
        public string CurFetchPlateJointCalibPosNozzleNo3
        {
            get => _curFetchPlateJointCalibPosNozzleNo3;
            set
            {
                _curFetchPlateJointCalibPosNozzleNo3 = value;
                NotifyOfPropertyChange(() => CurFetchPlateJointCalibPosNozzleNo3);
            }
        }

        private string _curFetchPlateJointCalibPosNozzleNo4;
        public string CurFetchPlateJointCalibPosNozzleNo4
        {
            get => _curFetchPlateJointCalibPosNozzleNo4;
            set
            {
                _curFetchPlateJointCalibPosNozzleNo4 = value;
                NotifyOfPropertyChange(() => CurFetchPlateJointCalibPosNozzleNo4);
            }
        }


        private string _curCarrierJointCalibPosNo1;
        public string CurCarrierJointCalibPosNo1
        {
            get => _curCarrierJointCalibPosNo1;
            set
            {
                _curCarrierJointCalibPosNo1 = value;
                NotifyOfPropertyChange(() => CurCarrierJointCalibPosNo1);
            }
        }
        private string _curCarrierJointCalibPosNo2;
        public string CurCarrierJointCalibPosNo2
        {
            get => _curCarrierJointCalibPosNo2;
            set
            {
                _curCarrierJointCalibPosNo2 = value;
                NotifyOfPropertyChange(() => CurCarrierJointCalibPosNo2);
            }
        }

        private string _curCarrierJointCalibPosNo3;
        public string CurCarrierJointCalibPosNo3
        {
            get => _curCarrierJointCalibPosNo3;
            set
            {
                _curCarrierJointCalibPosNo3 = value;
                NotifyOfPropertyChange(() => CurCarrierJointCalibPosNo3);
            }
        }

        private string _curCarrierJointCalibPosNo4;
        public string CurCarrierJointCalibPosNo4
        {
            get => _curCarrierJointCalibPosNo4;
            set
            {
                _curCarrierJointCalibPosNo4 = value;
                NotifyOfPropertyChange(() => CurCarrierJointCalibPosNo4);
            }
        }


        private string _curNozzleNo1JointCalibPos;
        public string CurNozzleNo1JointCalibPos
        {
            get => _curNozzleNo1JointCalibPos;
            set
            {
                _curNozzleNo1JointCalibPos = value;
                NotifyOfPropertyChange(() => CurNozzleNo1JointCalibPos);
            }
        }


        private string _curNozzleNo1SucPiecePosNo1;
        public string CurNozzleNo1SucPiecePosNo1
        {
            get => _curNozzleNo1SucPiecePosNo1;
            set
            {
                _curNozzleNo1SucPiecePosNo1 = value;
                NotifyOfPropertyChange(() => CurNozzleNo1SucPiecePosNo1);
            }
        }

        private string _curNozzleNo1SucPiecePosNo2;
        public string CurNozzleNo1SucPiecePosNo2
        {
            get => _curNozzleNo1SucPiecePosNo2;
            set
            {
                _curNozzleNo1SucPiecePosNo2 = value;
                NotifyOfPropertyChange(() => CurNozzleNo1SucPiecePosNo2);
            }
        }

        private string _curNozzleNo1SucPiecePosNo3;
        public string CurNozzleNo1SucPiecePosNo3
        {
            get => _curNozzleNo1SucPiecePosNo3;
            set
            {
                _curNozzleNo1SucPiecePosNo3 = value;
                NotifyOfPropertyChange(() => CurNozzleNo1SucPiecePosNo3);
            }
        }

        private string _curNozzleNo1SucPiecePosNo4;
        public string CurNozzleNo1SucPiecePosNo4
        {
            get => _curNozzleNo1SucPiecePosNo4;
            set
            {
                _curNozzleNo1SucPiecePosNo4 = value;
                NotifyOfPropertyChange(() => CurNozzleNo1SucPiecePosNo4);
            }
        }

        #endregion

        #region Constructor
        public JointCalibViewModel()
        {
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _paramManager = IoC.Get<ParamManager>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            CurNozzleNo1JointCalibPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos);

            //相机拍照位置
            CurCarrierJointCalibPosNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1);
            CurCarrierJointCalibPosNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2);
            CurCarrierJointCalibPosNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3);
            CurCarrierJointCalibPosNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4);
            //吸嘴吸取
            CurNozzleNo1SucPiecePosNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1);
            CurNozzleNo1SucPiecePosNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2);
            CurNozzleNo1SucPiecePosNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3);
            CurNozzleNo1SucPiecePosNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4);
            //吸嘴从固定点吸取标定片位置
            CurFetchPlateJointCalibPosNozzleNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1);
            CurFetchPlateJointCalibPosNozzleNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2);
            CurFetchPlateJointCalibPosNozzleNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3);
            CurFetchPlateJointCalibPosNozzleNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4);

        }
        #endregion

        #region Method

        //public void SetFovPlateInCarrierPosToUpCamera(object obj)
        //{
        //    int selNo = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置上相机载具标定片{selNo.ToString()}坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        if (selNo == 1)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1[2] = 0;
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1[3] = 0;
        //            CurCarrierJointCalibPosNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1);
        //        }
        //        else if (selNo == 2)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2[2] = 0;
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2[3] = 0;
        //            CurCarrierJointCalibPosNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2);
        //        }
        //        else if (selNo == 3)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3[2] = 0;
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3[3] = 0;
        //            CurCarrierJointCalibPosNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3);
        //        }
        //        else if (selNo == 4)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4[2] = 0;
        //            _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4[3] = 0;
        //            CurCarrierJointCalibPosNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4);
        //        }

        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        if (selNo == 1)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1;
        //        }
        //        else if (selNo == 2)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2;
        //        }
        //        else if (selNo == 3)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3;
        //        }
        //        else if (selNo == 4)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4;
        //        }

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFovPlateInCarrierPosToUpCamera({selNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFovPlateInCarrierPosToUpCamera({selNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //}

        ///// <summary>
        ///// 空移上相机到载具标定片
        ///// </summary>
        ///// <param name="obj"></param>
        //public async void MoveSpaceFovPlateInCarrierPosToUpCamera(object obj)
        //{
        //    int selNo = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移载具标定片{selNo.ToString()}号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        if (selNo == 1)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1;
        //        }
        //        else if (selNo == 2)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2;
        //        }
        //        else if (selNo == 3)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3;
        //        }
        //        else if (selNo == 4)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4;
        //        }
        //        //Move 指令
        //        //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { pos[0], pos[1] }, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceFovPlateInCarrierPosToUpCamera({selNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //            MessageBox.Success($"空移载具标定片{selNo.ToString()}号坐标成功！");
        //            return;
        //        });
        //        //}
        //        //else
        //        //{
        //        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceFovPlateInCarrierPosToUpCamera({selNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //        //}
        //    }
        //    else
        //    {
        //        MessageBox.Show($"吸嘴轴不在安全位置,请先将吸嘴轴运动到安全位置！");
        //    }
        //}

        ///// <summary>
        ///// 空移吸嘴到安全位置
        ///// </summary>
        ///// <param name="obj"></param>
        //public async void Restsafepos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移吸嘴到安全位置?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        await Task.Run(() =>
        //        {
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true))
        //            {
        //                return;
        //            }
        //            MessageBox.Show("空移吸嘴到安全位置OK");
        //        });
        //    }
        //}

        ///// <summary>
        ///// 执行Carrier标定板标定
        ///// </summary>
        //public async void CalibFovPlateInCarrierToUpCamera()
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定开始标定上相机载具标定片吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        await Task.Run(() =>
        //        {
        //            //1 Received: SC,1,1,2,1,3,1,4,1,5,11
        //            //1 Send: SC,1
        //            //2 Received: C1,Xn1,Yn1,An1
        //            //2 Send: C1,1
        //            //3 Received: SET,1,6,Xc1,Yc1,Ac1
        //            //3 Send: SET,1
        //            //4 Received: C2,Xn2,Yn2,An2
        //            //4 Send: C2,1
        //            //5 Received: SET,2,6,Xc2,Yc2,Ac2
        //            //5 Send: SET,1
        //            //6 Received: C3,Xn3,Yn3,An3
        //            //6 Send: C3,1
        //            //7 Received: SET,3,6,Xc3,Yc3,Ac3
        //            //7 Send: SET,1
        //            //8 Received: C4,Xn4,Yn4,An4
        //            //8 Send: C4,1
        //            //9 Received: SET,4,6,Xc4,Yc4,Ac4
        //            //9 Send: SET,1

        //            //Send:SC,1,1,2,1,3,1,4,1,5,11
        //            //Receive:SC,1

        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CalibFovPlateInCarrierToUpCamera->开始标定上相机载具标定片", En_Logout_Type.SpotCheck);
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;

        //            if (!_camera_Component.SendJointScCalib())
        //            {
        //                MessageBox.Show("Send:SC,1,1,2,1,3,1,4,1,5,11\r\n 接收失败 ");
        //                return;
        //            }
        //            var nozzlesucpos1 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1;
        //            var camerapos1 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1;

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.X1, 50, true, true);
        //            //运动到1号相机点
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos1[0], camerapos1[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C1,Xn1,Yn1,An1
        //            //Receive:C1,1
        //            if (!_camera_Component.CalibProcess(1, nozzlesucpos1[0], nozzlesucpos1[1], nozzlesucpos1[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,1,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(1, 6, camerapos1[0], camerapos1[1], camerapos1[3]))
        //            {
        //                return;
        //            }

        //            //开始第二个点
        //            //需要规避X1Y1，将轴运动到0；
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 50, 0 }, true, true))
        //            {
        //                return;
        //            }
        //            //Z回零
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            //吸嘴轴运动到1号吸嘴吸取标定片
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos1[0], nozzlesucpos1[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos1[2], true, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, true);
        //            Task.Delay(1000);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            var nozzlesucpos2 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], nozzlesucpos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos2[2], true, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, true);
        //            Thread.Sleep(50);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }

        //            var camerapos2 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos2[0], camerapos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C2,Xn1,Yn1,An1
        //            //Receive:C2,1
        //            if (!_camera_Component.CalibProcess(2, nozzlesucpos2[0], nozzlesucpos2[1], nozzlesucpos2[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,2,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(2, 6, camerapos2[0], camerapos2[1], camerapos2[3]))
        //            {
        //                return;
        //            }

        //            //开始第三个点


        //            //需要规避X1Y1，将轴运动到0；
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 50, 0 }, true, true))
        //            {
        //                return;
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], nozzlesucpos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos2[2], true, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
        //            Task.Delay(500);
        //            var nozzlesucpos3 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], nozzlesucpos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos3[2], true, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, true);
        //            Thread.Sleep(50);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.CleanAlarm();
        //            var camerapos3 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3;

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.X1, 50, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos3[0], camerapos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C3,Xn1,Yn1,An1
        //            //Receive:C3,1
        //            if (!_camera_Component.CalibProcess(3, nozzlesucpos3[0], nozzlesucpos3[1], nozzlesucpos3[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,3,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(3, 6, camerapos3[0], camerapos3[1], camerapos3[3]))
        //            {
        //                return;
        //            }

        //            //开始第四个点
        //            //需要规避X1Y1，将轴运动到0；
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 50, 0 }, true, true))
        //            {
        //                return;
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], nozzlesucpos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos3[2], true, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
        //            Task.Delay(500);
        //            var nozzlesucpos4 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos4[2], true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, false);
        //            Task.Delay(500);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, true);

        //            Thread.Sleep(50);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo4, false);
        //            Task.Delay(500);

        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }

        //            var camerapos4 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4;
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.X1, 50, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos4[0], camerapos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C4,Xn1,Yn1,An1
        //            //Receive:C4,1
        //            if (!_camera_Component.CalibProcess(4, nozzlesucpos4[0], nozzlesucpos4[1], nozzlesucpos4[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,4,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(4, 6, camerapos4[0], camerapos4[1], camerapos4[3]))
        //            {
        //                return;
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 50, 0 }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibFovPlateInCarrierToUpCamera->相机轴回到安全位置（X1，Y1）：50，0", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("相机轴回到安全位置失败");
        //            }

        //            //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.Z2, 0, true, true)) return;

        //            //var orgpnt = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;

        //            //if (!_mGoogol_Component.MoveAbsoluteToPointXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true))
        //            //{
        //            //    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibFovPlateInCarrierToUpCamera->运动1号点：{orgpnt[0].ToString("f2")},{orgpnt[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //            //    MessageBox.Show("运动到联合标定11点起点失败");
        //            //}                 

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            Task.Delay(500);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos4[2], true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo4, true);
        //            Task.Delay(500);

        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }

        //            var orgpnt = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;


        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动1号点：{orgpnt[0].ToString("f2")},{orgpnt[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到一号点失败");
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, orgpnt[2], true, true);


        //            MessageBox.Show("标定成功");
        //            return;
        //        });
        //    }
        //}

        //public void SetNozzleNo1CalibPos()
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置吸嘴起始标定坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //        _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //        _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //        _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
        //        CurNozzleNo1JointCalibPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos);
        //        _cacheParamManager.SaveAllParam();
        //        var pos = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetNozzleNo1CalibPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetNozzleNo1CalibPos() 弹窗取消操作", En_Logout_Type.SpotCheck);

        //}


        //public async void MoveSpaceNozzleCalibPos()
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移吸嘴起始标定坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        var pos = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;
        //        //Move 指令
        //        //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            if (!_mGoogol_Component.CleanAlarm()) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, false)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, pos[3], true, false)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { pos[0], pos[1] }, true, false)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, false)) return;

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceNozzleCalibPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //            MessageBox.Success("空移吸嘴起始标定坐标成功！");
        //            return;
        //        });
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceNozzleCalibPos() 弹窗取消操作", En_Logout_Type.SpotCheck);
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show($"上相机不在安全位置，吸嘴移动失败，请先将上相机机复位或移动到安全位置", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    //}
        //}

        ///// <summary>
        ///// 空移上相机到安全位置
        ///// </summary>
        ///// <param name="obj"></param>
        //public async void MoveToSafePos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移上相机到安全位置?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Warning("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true))
        //            {
        //                return;
        //            }
        //            MessageBox.Show("空移上相机到安全位置OK");
        //        });
        //    }
        //}

        ///// <summary>
        ///// 4号吸嘴11点标定
        ///// </summary>
        //public async void ElevenCalib()
        //{
        //    //10 Received: C5,X1,Y1,A1
        //    //10 Send: C5,1
        //    //11 Received: C5,X2,Y2,A2
        //    //11 Send: C5,1
        //    //12 Received: C5,X3,Y3,A3
        //    //12 Send: C5,1
        //    //13 Received: C5,X4,Y4,A4
        //    //13 Send: C5,1
        //    //14 Received: C5,X5,Y5,A5
        //    //14 Send: C5,1
        //    //15 Received: C5,X6,Y6,A6
        //    //15 Send: C5,1
        //    //16 Received: C5,X7,Y7,A7
        //    //16 Send: C5,1
        //    //17 Received: C5,X8,Y8,A8
        //    //17 Send: C5,1
        //    //18 Received: C5,X9,Y9,A9
        //    //18 Send: C5,1
        //    //19 Received: C5,X10,Y10,A10
        //    //19 Send: C5,1
        //    //20 Received: C5,X11,Y11,A11
        //    //20 Send: C5,1
        //    //21 Received: EC
        //    //21 Send: EC,1
        //    MessageBoxResult mbr = MessageBox.Show("确定执行11点标定吗？", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"ElevenCalib->开始标定", En_Logout_Type.SpotCheck);
        //            var movdis = 5;
        //            var newPos = new float[4];
        //            var orgpnt = new float[4];

        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 50, 0 }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->相机轴回到安全位置：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("相机轴回到安全位置失败");
        //            }


        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;

        //            orgpnt = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;

        //            newPos[0] = orgpnt[0] - movdis;
        //            newPos[1] = orgpnt[1] + movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动1号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到一号点失败");
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, orgpnt[2], true, true);
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第一个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第一个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0];
        //            newPos[1] = orgpnt[1] + movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动2号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到二号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第二个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第二个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0] + movdis;
        //            newPos[1] = orgpnt[1] + movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动3号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到三号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第三个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第三个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0] + movdis;
        //            newPos[1] = orgpnt[1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动4号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到四号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第四个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第四个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0];
        //            newPos[1] = orgpnt[1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动5号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到五号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第五个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第五个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0] - movdis;
        //            newPos[1] = orgpnt[1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动6号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到六号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第六个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第六个点标定失败");
        //                return;
        //            }


        //            newPos[0] = orgpnt[0] - movdis;
        //            newPos[1] = orgpnt[1] - movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动7号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到七号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第七个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0];
        //            newPos[1] = orgpnt[1] - movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动8号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到八号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第八个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0] + movdis;
        //            newPos[1] = orgpnt[1] - movdis;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动9号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到九号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第九个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第九个点标定失败");
        //                return;
        //            }

        //            newPos[0] = orgpnt[0];
        //            newPos[1] = orgpnt[1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动10号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到10号点XY失败");
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R4, -5, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动10号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到10号点R失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, orgpnt[0], orgpnt[1], -5))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第十个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第10个点标定失败");
        //                return;
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R4, 5, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动11号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到11号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, orgpnt[0], orgpnt[1], 5))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第11个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第11个点标定失败");
        //                return;
        //            }

        //            _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R4, 0, true, true);

        //            _camera_Component.SendEcCalib();

        //            MessageBox.Show("标定成功");
        //            return;
        //        });
        //    }
        //}
        //public void SetNozzleNo1SucPieceCalibPos(object obj)
        //{
        //    int selNo = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show($"确定{selNo.ToString()}号吸嘴吸取标定片坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        if (selNo == 1)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
        //            CurNozzleNo1SucPiecePosNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1);
        //        }
        //        else if (selNo == 2)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
        //            CurNozzleNo1SucPiecePosNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2);
        //        }
        //        else if (selNo == 3)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R3];
        //            CurNozzleNo1SucPiecePosNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3);
        //        }
        //        else if (selNo == 4)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R4];
        //            CurNozzleNo1SucPiecePosNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4);
        //        }

        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        if (selNo == 1)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1;
        //        }
        //        else if (selNo == 2)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2;
        //        }
        //        else if (selNo == 3)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3;
        //        }
        //        else if (selNo == 4)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4;
        //        }
        //        MessageBox.Success($"设定{selNo.ToString()}号吸嘴吸取标定片坐标成功！");
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetNozzleNo1SucPieceCalibPos({selNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetNozzleNo1SucPieceCalibPos({selNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //}

        ///// <summary>
        ///// 空移?嘴吸取标定片?坐标
        ///// </summary>
        //public async void MoveSpaceNozzleNo1SucPieceCalibPos(object obj)
        //{
        //    int selNo = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移4号吸嘴吸取标定片{selNo.ToString()}号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        if (selNo == 1)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1;
        //        }
        //        else if (selNo == 2)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2;
        //        }
        //        else if (selNo == 3)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3;
        //        }
        //        else if (selNo == 4)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4;
        //        }

        //        //Move 指令
        //        //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true))
        //            {
        //                return;
        //            }

        //            //先让吸嘴y移回到100
        //            if (!_mGoogol_Component.GetCurPos())
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { _mGoogol_Component.CurPos[2], 100 }, true, true))
        //            {
        //                return;
        //            }

        //            //相机回去
        //            if (pos[0] > 220)
        //            {
        //                //大于220则相机去左边
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 390, 0 }, true, true))
        //                {
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true))
        //                {
        //                    return;
        //                }
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;

        //            //先移X
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { pos[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { pos[0], pos[1] }, true, true))
        //            {
        //                return;
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true))
        //            {
        //                return;
        //            }

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceNozzleNo1SucPieceCalibPos({selNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //            MessageBox.Success($"空移1号吸嘴吸取标定片{selNo.ToString()}号坐标成功！");
        //            return;
        //        });
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceNozzleNo1SucPieceCalibPos({selNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show($"上相机不在安全位置，吸嘴移动失败，请先将上相机机复位或移动到安全位置", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    //}
        //}

        ////public void FovFeederCalibration()
        ////{
        ////MessageBoxResult mbr = MessageBox.Show($"确定Feeder标定吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        ////if (mbr == MessageBoxResult.OK)
        ////{
        ////var pos = _cacheParamManager.manualPositionParam.AxisFeederCalibPos;
        ////_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.Z2, 0, false, false);
        ////_mGoogol_Component.MoveAbsoluteToPointXY(En_StationNo.StationNo2, new float[2] { pos[0], pos[1] }, true, false);

        ////Send:C3,Xn3,Yn3,An3
        ////Receive:C3,1
        ////_camera_Component.CalibProcess(3, pos[0], pos[1], pos[3]);

        ////Send:       SET,3,5,Xc1,Yc1,Ac1
        ////Receive:    SET,3Calib
        ////_camera_Component.CalibProcessSendSet(3, 5, pos[0], pos[1], pos[3]);

        ////_camera_Component.SendEcCalib();
        ////}
        ////NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"FovFeederCalibration() 弹窗取消操作", En_Logout_Type.SpotCheck);
        ////}

        //#region 标定片点检标定
        ///// <summary>
        ///// 取标定片
        ///// </summary>
        ///// <param name="obj"></param>
        //public void SetFetchPlateNo1Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置取标定片1号吸嘴坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1;
        //        CurFetchPlateJointCalibPosNozzleNo1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1);
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"设置取标定片1号吸嘴坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //}
        ///// <summary>
        ///// 空移标定片
        ///// </summary>
        ///// <param name="obj"></param>
        //public void MoveSpaceFetchPlateNo1Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移取标定片1号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo1;

        //        if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //        {
        //            MessageBox.Show("轴系在运动，等静止再操作！");
        //            return;
        //        }
        //        if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //        //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteX2Y2R(En_StationNo.StationNo2, En_AxisNum.R1, new float[3] { pos[0], pos[1], pos[3] }, true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"空移取标定片1号坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        MessageBox.Success($"空移取标定片1号坐标成功！");
        //        return;
        //    }
        //}

        //public void SetFetchPlateNo2Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置取标定片2号吸嘴坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2;
        //        CurFetchPlateJointCalibPosNozzleNo2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2);
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"设置取标定片2号吸嘴坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //}

        //public void MoveSpaceFetchPlateNo2Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移取标定片2号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo2;

        //        if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //        {
        //            MessageBox.Show("轴系在运动，等静止再操作！");
        //            return;
        //        }
        //        if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //        //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteX2Y2R(En_StationNo.StationNo2, En_AxisNum.R2, new float[3] { pos[0], pos[1], pos[3] }, true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"空移取标定片2号坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        MessageBox.Success($"空移取标定片2号坐标成功！");
        //        return;
        //    }
        //}

        //public void SetFetchPlateNo3Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置取标定片3号吸嘴坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R3];
        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3;
        //        CurFetchPlateJointCalibPosNozzleNo3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3);
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"设置取标定片3号吸嘴坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //}

        //public void MoveSpaceFetchPlateNo3Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移取标定片1号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo3;

        //        if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //        {
        //            MessageBox.Show("轴系在运动，等静止再操作！");
        //            return;
        //        }
        //        if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //        //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteX2Y2R(En_StationNo.StationNo2, En_AxisNum.R3, new float[3] { pos[0], pos[1], pos[3] }, true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"空移取标定片3号坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        MessageBox.Success($"空移取标定片3号坐标成功！");
        //        return;
        //    }
        //}

        //public void SetFetchPlateNo4Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定设置取标定片4号吸嘴坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //        _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R4];
        //        _cacheParamManager.SaveAllParam();
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4;
        //        CurFetchPlateJointCalibPosNozzleNo4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4);
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"设置取标定片4号吸嘴坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //}

        //public void MoveSpaceFetchPlateNo4Pos(object obj)
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移取标定片4号坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        pos = _cacheParamManager.manualPositionParam.AxisFetchPlateJointCalibPosNozzleNo4;

        //        if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //        {
        //            MessageBox.Show("轴系在运动，等静止再操作！");
        //            return;
        //        }
        //        if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //        //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.R1, pos[3], true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteX2Y2R(En_StationNo.StationNo2, En_AxisNum.R4, new float[3] { pos[0], pos[1], pos[3] }, true, true)) return;
        //        if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"空移取标定片4号坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        MessageBox.Success($"空移取标定片4号坐标成功！");
        //        return;
        //    }
        //}

        //#endregion
        ///// <summary>
        ///// 自动联合标定
        ///// </summary>
        //public async void SetAutoJointCalib()
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定开始联合自动标定吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        //1 Received: SC,1,1,2,1,3,1,4,1,5,11
        //        //1 Send: SC,1
        //        //2 Received: C1,Xn1,Yn1,An1
        //        //2 Send: C1,1
        //        //3 Received: SET,1,6,Xc1,Yc1,Ac1
        //        //3 Send: SET,1
        //        //4 Received: C2,Xn2,Yn2,An2
        //        //4 Send: C2,1
        //        //5 Received: SET,2,6,Xc2,Yc2,Ac2
        //        //5 Send: SET,1
        //        //6 Received: C3,Xn3,Yn3,An3
        //        //6 Send: C3,1
        //        //7 Received: SET,3,6,Xc3,Yc3,Ac3
        //        //7 Send: SET,1
        //        //8 Received: C4,Xn4,Yn4,An4
        //        //8 Send: C4,1
        //        //9 Received: SET,4,6,Xc4,Yc4,Ac4
        //        //9 Send: SET,1

        //        //Send:SC,1,1,2,1,3,1,4,1,5,11
        //        //Receive:SC,1

        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetAutoJointCalib->开始标定上相机载具标定片", En_Logout_Type.SpotCheck);

        //            if (!_mGoogol_Component.CleanAlarm()) return;
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;

        //            //先运动到吸标定片位置，1号吸嘴


        //            if (!_camera_Component.SendJointScCalib())
        //            {
        //                MessageBox.Show("Send:SC,1,1,2,1,3,1,4,1,5,11\r\n 接收失败 ");
        //                return;
        //            }
        //            var nozzlesucpos1 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo1;
        //            var camerapos1 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo1;
        //            //运动到1号相机点
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos1[0], camerapos1[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C1,Xn1,Yn1,An1
        //            //Receive:C1,1
        //            if (!_camera_Component.CalibProcess(1, nozzlesucpos1[0], nozzlesucpos1[1], nozzlesucpos1[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,1,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(1, 6, camerapos1[0], camerapos1[1], camerapos1[3]))
        //            {
        //                return;
        //            }

        //            //开始第二个点
        //            //需要规避X1Y1，将轴运动到0
        //            //相机回右上，后续应该以点位确定去左上还是右上
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true))
        //            {
        //                return;
        //            }
        //            //Z回零
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            //吸嘴轴运动到1号吸嘴吸取标定片
        //            //先动X
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos1[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos1[0], nozzlesucpos1[1] }, true, true))
        //            {
        //                return;
        //            }

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos1[2], true, true);
        //            Thread.Sleep(300);
        //            //吸
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, true);
        //            Thread.Sleep(500);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            //Z上
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            Thread.Sleep(500);

        //            var nozzlesucpos2 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo2;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos1[0], nozzlesucpos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], nozzlesucpos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(500);

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos2[2], true, true);
        //            Thread.Sleep(300);
        //            //吸关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, false);
        //            //吹
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, true);
        //            Thread.Sleep(500);
        //            //z上抬一小段
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -2, true, true);
        //            //上抬到位立刻吹关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, false);
        //            //继续上抬
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            Thread.Sleep(500);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }

        //            var camerapos2 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo2;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos2[0], camerapos2[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C2,Xn1,Yn1,An1
        //            //Receive:C2,1
        //            if (!_camera_Component.CalibProcess(2, nozzlesucpos2[0], nozzlesucpos2[1], nozzlesucpos2[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,2,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(2, 6, camerapos2[0], camerapos2[1], camerapos2[3]))
        //            {
        //                return;
        //            }

        //            //开始第三个点


        //            //需要规避X1Y1，将轴运动到0；
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true))
        //            {
        //                return;
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], nozzlesucpos2[1] }, true, true))
        //            {
        //                return;
        //            }

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos2[2], true, true);
        //            Thread.Sleep(300);
        //            //吸
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, true);
        //            Thread.Sleep(500);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            //Z上
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            Thread.Sleep(500);

        //            var nozzlesucpos3 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo3;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos2[0], nozzlesucpos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], nozzlesucpos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(500);

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos3[2], true, true);
        //            Thread.Sleep(300);
        //            //吸关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, false);
        //            //吹
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, true);
        //            Thread.Sleep(500);
        //            //z上抬一小段
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -2, true, true);
        //            //上抬到位立刻吹关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, false);
        //            //继续上抬
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            Thread.Sleep(500);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }
        //            _mGoogol_Component.CleanAlarm();
        //            var camerapos3 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo3;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos3[0], camerapos3[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C3,Xn1,Yn1,An1
        //            //Receive:C3,1
        //            if (!_camera_Component.CalibProcess(3, nozzlesucpos3[0], nozzlesucpos3[1], nozzlesucpos3[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,3,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(3, 6, camerapos3[0], camerapos3[1], camerapos3[3]))
        //            {
        //                return;
        //            }

        //            //开始第四个点
        //            //需要规避X1Y1，将轴运动到0；
        //            //轴避让到左上角
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 390, 0 }, true, true))
        //            {
        //                return;
        //            }

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], nozzlesucpos3[1] }, true, true))
        //            {
        //                return;
        //            }

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos3[2], true, true);
        //            Thread.Sleep(300);
        //            //吸
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, true);
        //            Thread.Sleep(500);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            //Z上
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            Thread.Sleep(500);

        //            var nozzlesucpos4 = _cacheParamManager.manualPositionParam.AxisNozzleNo1SucPiecePosNo4;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos3[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(500);

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos4[2], true, true);
        //            Thread.Sleep(300);
        //            //吸关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, false);
        //            //吹
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, true);
        //            Thread.Sleep(500);
        //            //z上抬一小段
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -2, true, true);
        //            //上抬到位立刻吹关
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialVacBreakNo1, false);
        //            //继续上抬
        //            _mGoogol_Component.MoveRelativeSingleAxis(En_AxisNum.Z2, -10, true, true);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            Thread.Sleep(500);

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 100, 100 }, true, true))
        //            {
        //                return;
        //            }

        //            var camerapos4 = _cacheParamManager.manualPositionParam.AxisCarrierJointCalibPosNo4;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { camerapos4[0], camerapos4[1] }, true, true))
        //            {
        //                return;
        //            }
        //            Thread.Sleep(300);
        //            //Send:C4,Xn1,Yn1,An1
        //            //Receive:C4,1
        //            if (!_camera_Component.CalibProcess(4, nozzlesucpos4[0], nozzlesucpos4[1], nozzlesucpos4[3]))
        //            {
        //                return;
        //            }
        //            //Send:       SET,4,6,Xc1,Yc1,Ac1
        //            //Receive:    SET,1
        //            if (!_camera_Component.CalibProcessSendSet(4, 6, camerapos4[0], camerapos4[1], camerapos4[3]))
        //            {
        //                return;
        //            }

        //            //轴避让到左上角
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 390, 0 }, true, true))
        //                return;

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], 100 }, true, true))
        //            {
        //                return;
        //            }
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], nozzlesucpos4[1] }, true, true))
        //            {
        //                return;
        //            }

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, nozzlesucpos4[2], true, true);
        //            Thread.Sleep(300);
        //            //吸
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutNozzleMaterialSucNo1, true);
        //            Thread.Sleep(500);
        //            //气缸上
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            //Z上
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);
        //            Thread.Sleep(500);

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { nozzlesucpos4[0], 100 }, true, true))
        //            {
        //                return;
        //            }

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetAutoJointCalib->开始标定", En_Logout_Type.SpotCheck);

        //            float movdis = 5.0f;//十一点标定xy偏移5mm
        //            float angle = 10.0f;//十一点标定角度偏移10度

        //            float[] orgpnt = _cacheParamManager.manualPositionParam.AxisCalibJoint_NozzleNo1Pos;//中心位置
        //            //calib11Points第一索引(0不用)
        //            //1 2 3
        //            //4 5 6
        //            //7 8 9
        //            //calib11Points第二索引
        //            //0x 1y
        //            float[,] calib11Points = new float[10, 2];
        //            calib11Points[1, 0] = orgpnt[0] - movdis;
        //            calib11Points[1, 1] = orgpnt[1] + movdis;
        //            calib11Points[2, 0] = orgpnt[0];
        //            calib11Points[2, 1] = orgpnt[1] + movdis;
        //            calib11Points[3, 0] = orgpnt[0] + movdis;
        //            calib11Points[3, 1] = orgpnt[1] + movdis;
        //            calib11Points[4, 0] = orgpnt[0] - movdis;
        //            calib11Points[4, 1] = orgpnt[1];
        //            calib11Points[5, 0] = orgpnt[0];
        //            calib11Points[5, 1] = orgpnt[1];
        //            calib11Points[6, 0] = orgpnt[0] + movdis;
        //            calib11Points[6, 1] = orgpnt[1];
        //            calib11Points[7, 0] = orgpnt[0] - movdis;
        //            calib11Points[7, 1] = orgpnt[1] - movdis;
        //            calib11Points[8, 0] = orgpnt[0];
        //            calib11Points[8, 1] = orgpnt[1] - movdis;
        //            calib11Points[9, 0] = orgpnt[0] + movdis;
        //            calib11Points[9, 1] = orgpnt[1] - movdis;

        //            float[] newPos = new float[4];

        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true);

        //            newPos[0] = calib11Points[5, 0];
        //            newPos[1] = calib11Points[5, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动1号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到一号点失败");
        //            }
        //            Thread.Sleep(500);

        //            //气缸下
        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            //Z下
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, orgpnt[2], true, true);
        //            Thread.Sleep(300);
        //            //拍照延时
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第一个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第一个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[2, 0];
        //            newPos[1] = calib11Points[2, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动2号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到二号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第二个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第二个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[1, 0];
        //            newPos[1] = calib11Points[1, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动3号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到三号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第三个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第三个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[4, 0];
        //            newPos[1] = calib11Points[4, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动4号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到四号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第四个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第四个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[7, 0];
        //            newPos[1] = calib11Points[7, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动5号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到五号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第五个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第五个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[8, 0];
        //            newPos[1] = calib11Points[8, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动6号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到六号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第六个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第六个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[9, 0];
        //            newPos[1] = calib11Points[9, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动7号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到七号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第七个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[6, 0];
        //            newPos[1] = calib11Points[6, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动8号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到八号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第八个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第八个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[3, 0];
        //            newPos[1] = calib11Points[3, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动9号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到九号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, newPos[0], newPos[1], newPos[3]))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第九个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第九个点标定失败");
        //                return;
        //            }

        //            newPos[0] = calib11Points[5, 0];
        //            newPos[1] = calib11Points[5, 1];
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动10号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到十号点失败");
        //                return;
        //            }
        //            Thread.Sleep(500);
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, -angle, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动10号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到10号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, orgpnt[0], orgpnt[1], -angle))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第十个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第10个点标定失败");
        //                return;
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, angle, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->运动11号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到11号点失败");
        //            }
        //            Thread.Sleep(500);
        //            if (!_camera_Component.CalibProcess(5, orgpnt[0], orgpnt[1], angle))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"ElevenCalib->第11个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第11个点标定失败");
        //                return;
        //            }

        //            _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //            _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.R1, 0, true, true);

        //            _camera_Component.SendEcCalib();

        //            MessageBox.Show("联合标定成功");
        //            return;
        //        });
        //    }
        //}
        #endregion
    }
}
