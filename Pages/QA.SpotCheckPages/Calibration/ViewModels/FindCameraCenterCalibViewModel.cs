using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using QA.Business.Component.Camera;
using QA.Business.Component.Motion.Googol;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QA.SpotCheckPages.Calibration.ViewModels
{
    [Export(typeof(ICalibrationViewModel))]
    public class FindCameraCenterCalibViewModel : Screen, INotifyPropertyChanged, ICalibrationViewModel
    {
        #region Field
        private Camera_Component _camera_Component;
        private ParamManager _paramManager;
        private MotionGoogol_Component _mGoogol_Component;
        private CacheParamManager _cacheParamManager;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "示教点位标定";

        public ushort OrderID { get; set; } = 0;

        private string _curAxisFeederPickPos;
        public string CurAxisFeederPickPos
        {
            get => _curAxisFeederPickPos;
            set
            {
                _curAxisFeederPickPos = value;
                NotifyOfPropertyChange(() => CurAxisFeederPickPos);
            }
        }

        private string _curAxisFeederPickR1Pos;
        public string CurAxisFeederPickR1Pos
        {
            get => _curAxisFeederPickR1Pos;
            set
            {
                _curAxisFeederPickR1Pos = value;
                NotifyOfPropertyChange(() => CurAxisFeederPickR1Pos);
            }
        }

        private string _curAxisFeederPickR2Pos;
        public string CurAxisFeederPickR2Pos
        {
            get => _curAxisFeederPickR2Pos;
            set
            {
                _curAxisFeederPickR2Pos = value;
                NotifyOfPropertyChange(() => CurAxisFeederPickR2Pos);
            }
        }

        private string _curAxisFeederPickR3Pos;
        public string CurAxisFeederPickR3Pos
        {
            get => _curAxisFeederPickR3Pos;
            set
            {
                _curAxisFeederPickR3Pos = value;
                NotifyOfPropertyChange(() => CurAxisFeederPickR3Pos);
            }
        }

        private string _curAxisFeederPickR4Pos;
        public string CurAxisFeederPickR4Pos
        {
            get => _curAxisFeederPickR4Pos;
            set
            {
                _curAxisFeederPickR4Pos = value;
                NotifyOfPropertyChange(() => CurAxisFeederPickR4Pos);
            }
        }


        private string _curCamNo2NozzleNo1Pos;
        public string CurCamNo2NozzleNo1Pos
        {
            get => _curCamNo2NozzleNo1Pos;
            set
            {
                _curCamNo2NozzleNo1Pos = value;
                NotifyOfPropertyChange(() => CurCamNo2NozzleNo1Pos);
            }
        }

        private string _curCamNo2NozzleNo2Pos;
        public string CurCamNo2NozzleNo2Pos
        {
            get => _curCamNo2NozzleNo2Pos;
            set
            {
                _curCamNo2NozzleNo2Pos = value;
                NotifyOfPropertyChange(() => CurCamNo2NozzleNo2Pos);
            }
        }

        private string _curCamNo2NozzleNo3Pos;
        public string CurCamNo2NozzleNo3Pos
        {
            get => _curCamNo2NozzleNo3Pos;
            set
            {
                _curCamNo2NozzleNo3Pos = value;
                NotifyOfPropertyChange(() => CurCamNo2NozzleNo3Pos);
            }
        }

        private string _curCamNo2NozzleNo4Pos;
        public string CurCamNo2NozzleNo4Pos
        {
            get => _curCamNo2NozzleNo4Pos;
            set
            {
                _curCamNo2NozzleNo4Pos = value;
                NotifyOfPropertyChange(() => CurCamNo2NozzleNo4Pos);
            }
        }

        private string _curNgMaterialPos;
        public string CurNgMaterialPos
        {
            get => _curNgMaterialPos;
            set
            {
                _curNgMaterialPos = value;
                NotifyOfPropertyChange(() => CurNgMaterialPos);
            }
        }

        private string _curCarrierScannerPos;
        public string CurCarrierScannerPos
        {
            get => _curCarrierScannerPos;
            set
            {
                _curCarrierScannerPos = value;
                NotifyOfPropertyChange(() => CurCarrierScannerPos);
            }
        }

        #endregion

        #region Constructor
        public FindCameraCenterCalibViewModel()
        {
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _paramManager = IoC.Get<ParamManager>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();

            CurAxisFeederPickPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickPos);

            CurCamNo2NozzleNo1Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos);
            CurCamNo2NozzleNo2Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos);
            CurCamNo2NozzleNo3Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos);
            CurCamNo2NozzleNo4Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos);
            CurNgMaterialPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNgSiloPos);
            CurCarrierScannerPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos);

            CurAxisFeederPickR1Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos) + "\n" +
                "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos);
            CurAxisFeederPickR2Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos) + "\n" +
                "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos);
            CurAxisFeederPickR3Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos) + "\n" +
                "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos);
            CurAxisFeederPickR4Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos) + "\n" +
                "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos);
        }
        #endregion

        #region Override

        #endregion

        #region Method

        public async void MoveSpacePosToFeederPickMaterial()
        {
            MessageBoxResult mbr = MessageBox.Show("确定移动飞达取料坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }

                    if (!_mGoogol_Component.CleanAlarm())
                        return;
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0f, true, false))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, _cacheParamManager.manualPositionParam.AxisFeederPickPos, true, true))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, _cacheParamManager.manualPositionParam.AxisFeederPickPos[2], true, false))
                        return;
                    var pos = _cacheParamManager.manualPositionParam.AxisFeederPickPos;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToFeederPickMaterial() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                    MessageBox.Show("移动飞达取料坐标成功！");
                    return;
                });
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToFeederPickMaterial() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public void SetFeederPickMaterialPos()
        {
            MessageBoxResult mbr = MessageBox.Show("确定设置飞达取料坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                _cacheParamManager.manualPositionParam.AxisFeederPickPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                _cacheParamManager.manualPositionParam.AxisFeederPickPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                _cacheParamManager.manualPositionParam.AxisFeederPickPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                _cacheParamManager.manualPositionParam.AxisFeederPickPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                _cacheParamManager.SaveAllParam();

                CurAxisFeederPickPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickPos);

                var pos = _cacheParamManager.manualPositionParam.AxisFeederPickPos;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                return;
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }

        public void SetFeederSucAngle(object obj)
        {
            int nozzleNo = Convert.ToInt32(obj);

            MessageBoxResult result = MessageBox.Show($"确定设置{nozzleNo.ToString()}号吸嘴取料坐标吗？\n是表示设置左取料，否表示设置右取料", "提示信息", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            float[] pos = new float[4];
            if (nozzleNo == 1)
            {
                if (result == MessageBoxResult.Yes)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos;
                }
                else if (result == MessageBoxResult.No)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos;
                }
                CurAxisFeederPickR1Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos) + "\n" +
                    "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos);
            }
            else if (nozzleNo == 2)
            {
                if (result == MessageBoxResult.Yes)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos;
                }
                else if (result == MessageBoxResult.No)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos;
                }
                CurAxisFeederPickR2Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos) + "\n" +
                    "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos);
            }
            else if (nozzleNo == 3)
            {
                if (result == MessageBoxResult.Yes)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R3];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos;
                }
                else if (result == MessageBoxResult.No)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R3];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos;
                }
                CurAxisFeederPickR3Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos) + "\n" +
                    "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos);
            }
            else if (nozzleNo == 4)
            {
                if (result == MessageBoxResult.Yes)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R4];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos;
                }
                else if (result == MessageBoxResult.No)
                {
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R4];
                    pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos;
                }
                CurAxisFeederPickR4Pos = "左：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos) + "\n" +
                    "右：" + NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos);
            }
            _cacheParamManager.SaveAllParam();
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederSucAngle({nozzleNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        }

        public async void MoveSpaceFeederSucAngle(object obj)
        {
            int nozzleNo = Convert.ToInt32(obj);

            MessageBoxResult result = MessageBox.Show($"确定空移{nozzleNo.ToString()}号吸嘴取料坐标吗？\n是表示空移左取料，否表示空移右取料", "提示信息", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            float[] pos = new float[4];
            await Task.Run(() =>
            {
                //false说明有轴在运动
                if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                {
                    MessageBox.Show("轴系在运动，等静止再操作！");
                    return;
                }
                if (_mGoogol_Component.CurPos[(byte)En_AxisNum.X1] > 50 || _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1] > 1)
                {
                    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 50, 0 }, true, true))
                        return;
                }

                if (nozzleNo == 1)
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1LeftPos;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle1RightPos;
                    }
                }
                else if (nozzleNo == 2)
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2LeftPos;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle2RightPos;
                    }
                }
                else if (nozzleNo == 3)
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3LeftPos;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle3RightPos;
                    }
                }
                else if (nozzleNo == 4)
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4LeftPos;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        pos = _cacheParamManager.manualPositionParam.AxisFeederPickNozzle4RightPos;
                    }
                }

                if (!_mGoogol_Component.CleanAlarm())
                    return;
                if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                    return;
                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0f, true, false))
                    return;

                if (!_mGoogol_Component.MoveAbsoluteSingleAxis((En_AxisNum)(nozzleNo + 4), pos[3], true, false))
                    return;

                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, pos, true, true))
                    return;
                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, false))
                    return;

                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceFeederSucAngle({nozzleNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                //MessageBox.Show("移动飞达取料坐标成功！");
                return;
            });
        }

        public void SetPosToDownCamera_Nozzle(object nozzleNo)
        {
            int _nlzzleNo = Convert.ToInt32(nozzleNo);

            MessageBoxResult mbr = MessageBox.Show($"确定设置{_nlzzleNo.ToString()}#吸嘴下相机坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                float[] pos = new float[4];
                if (_nlzzleNo == 1)
                {
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
                    CurCamNo2NozzleNo1Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos);
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos;
                }
                else if (_nlzzleNo == 2)
                {
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
                    CurCamNo2NozzleNo2Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos);
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos;
                }
                else if (_nlzzleNo == 3)
                {
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R3];
                    CurCamNo2NozzleNo3Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos);
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos;
                }
                else if (_nlzzleNo == 4)
                {
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                    _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R4];
                    CurCamNo2NozzleNo4Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos);
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos;
                }
                _cacheParamManager.SaveAllParam();

                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                return;
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public async void MoveSpacePosToDownCamera_Nozzle(object nozzleNo)
        {
            int _nlzzleNo = Convert.ToInt32(nozzleNo);
            MessageBoxResult mbr = MessageBox.Show($"确定空移{_nlzzleNo.ToString()}#吸嘴下相机坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                float[] pos = new float[4];
                if (_nlzzleNo == 1)
                {
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No1Pos;
                    //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
                }
                else if (_nlzzleNo == 2)
                {
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No2Pos;
                    //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo2, false);
                }
                else if (_nlzzleNo == 3)
                {
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No3Pos;
                    //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo3, false);
                }
                else if (_nlzzleNo == 4)
                {
                    pos = _cacheParamManager.manualPositionParam.AxisDownCamera_No4Pos;
                    //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
                }
                //Move 指令
                //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    if (!_mGoogol_Component.CleanAlarm())
                        return;
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true))
                        return;
                    //if (!_mGoogol_Component.MoveAbsoluteToPointSingleAxis((En_AxisNum)(_nlzzleNo + 4), pos[3], true, true))
                    //    return;
                    if (!_mGoogol_Component.MoveAbsoluteX2Y2R(En_StationNo.StationNo2, (En_AxisNum)(_nlzzleNo + 4), new float[3] { pos[0], pos[1], pos[3] }, true, true))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true))
                        return;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 坐标为：{pos[0].ToString("f2")},{pos[1].ToString("f2")},{pos[2].ToString("f2")},{pos[3].ToString("f2")}", En_Logout_Type.SpotCheck);
                    MessageBox.Show($"空移{_nlzzleNo.ToString()}#吸嘴下相机坐标成功！");
                    return;
                });

                //if (_nlzzleNo == 1)
                //{
                //    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
                //}
                //else if (_nlzzleNo == 2)
                //{
                //    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo2, true);
                //}
                //else if (_nlzzleNo == 3)
                //{
                //    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo3, true);
                //}
                //else
                //{
                //    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
                //}

            }
            else
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public async void MoveSpacePosToNgMaterial()
        {
            MessageBoxResult mbr = MessageBox.Show("确定移动Ng料仓坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    if (!_mGoogol_Component.CleanAlarm())
                        return;
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0f, true, true))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, _cacheParamManager.manualPositionParam.AxisNgSiloPos, true, true))
                        return;
                    if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, _cacheParamManager.manualPositionParam.AxisNgSiloPos[2], true, true))
                        return;
                    var pos = _cacheParamManager.manualPositionParam.AxisNgSiloPos;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToNgMaterial() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                    MessageBox.Show("移动Ng料仓坐标成功！");
                });
            }
            else
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToNgMaterial() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public void SetNgMaterialPos()
        {
            MessageBoxResult mbr = MessageBox.Show("确定设置Ng料仓坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                _cacheParamManager.manualPositionParam.AxisNgSiloPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
                _cacheParamManager.manualPositionParam.AxisNgSiloPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
                _cacheParamManager.manualPositionParam.AxisNgSiloPos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
                _cacheParamManager.manualPositionParam.AxisNgSiloPos[3] = 0;
                _cacheParamManager.SaveAllParam();

                CurNgMaterialPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisNgSiloPos);

                var pos = _cacheParamManager.manualPositionParam.AxisNgSiloPos;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                return;
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public async void MoveSpacePosToCarrierCode()
        {
            MessageBoxResult mbr = MessageBox.Show("确定移动到载具扫码坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                await Task.Run(() =>
                {
                    //false说明有轴在运动
                    if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                    {
                        MessageBox.Show("轴系在运动，等静止再操作！");
                        return;
                    }
                    if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid))
                        return;
                    if (!_mGoogol_Component.CleanAlarm())
                        return;
                    if (_mGoogol_Component.StationNo1InSafeRange())
                    {
                        if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos, true, true))
                            return;
                    }
                    else
                    {
                        MessageBox.Show("当前操作不安全！请先移动吸嘴轴到安全位置再操作");
                        return;
                    }

                    var pos = _cacheParamManager.manualPositionParam.AxisNgSiloPos;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToNgMaterial() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                    MessageBox.Show("移动到载具扫码坐标成功！");
                    return;
                });
            }
            else
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpacePosToNgMaterial() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        public void SetCarrierCodePos()
        {
            MessageBoxResult mbr = MessageBox.Show("确定设置载具扫码坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.OK)
            {
                _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
                _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
                _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos[2] = 0;
                _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos[3] = 0;
                _cacheParamManager.SaveAllParam();

                CurCarrierScannerPos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos);

                var pos = _cacheParamManager.manualPositionParam.AxisScannerCarrierBarcodePos;
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
                return;
            }
            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetFeederPickMaterialPos() 弹窗取消操作", En_Logout_Type.SpotCheck);
        }
        #endregion
    }
}
