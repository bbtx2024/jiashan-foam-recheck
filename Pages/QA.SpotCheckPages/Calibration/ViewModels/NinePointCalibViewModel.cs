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
    public class NinePointCalibViewModel : Screen, INotifyPropertyChanged, ICalibrationViewModel
    {
        #region Field
        private Camera_Component _camera_Component;
        private ParamManager _paramManager;
        private MotionGoogol_Component _mGoogol_Component;
        private CacheParamManager _cacheParamManager;
        #endregion

        #region Property
        public override string DisplayName { get; set; } = "9点标定";

        public ushort OrderID { get; set; } = 2;

        private float _calibMoveDis;
        public float CalibMoveDis
        {
            get => _calibMoveDis;
            set
            {
                _calibMoveDis = value;
                NotifyOfPropertyChange(() => CalibMoveDis);
            }
        }

        private string _curCamNo1NineCalibPos_1;
        public string CurCamNo1NineCalibPos_1
        {
            get => _curCamNo1NineCalibPos_1;
            set
            {
                _curCamNo1NineCalibPos_1 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_1);
            }
        }

        private string _curCamNo1NineCalibPos_2;
        public string CurCamNo1NineCalibPos_2
        {
            get => _curCamNo1NineCalibPos_2;
            set
            {
                _curCamNo1NineCalibPos_2 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_2);
            }
        }

        private string _curCamNo1NineCalibPos_3;
        public string CurCamNo1NineCalibPos_3
        {
            get => _curCamNo1NineCalibPos_3;
            set
            {
                _curCamNo1NineCalibPos_3 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_3);
            }
        }

        private string _curCamNo1NineCalibPos_4;
        public string CurCamNo1NineCalibPos_4
        {
            get => _curCamNo1NineCalibPos_4;
            set
            {
                _curCamNo1NineCalibPos_4 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_4);
            }
        }

        private string _curCamNo1NineCalibPos_5;
        public string CurCamNo1NineCalibPos_5
        {
            get => _curCamNo1NineCalibPos_5;
            set
            {
                _curCamNo1NineCalibPos_5 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_5);
            }
        }

        private string _curCamNo1NineCalibPos_6;
        public string CurCamNo1NineCalibPos_6
        {
            get => _curCamNo1NineCalibPos_6;
            set
            {
                _curCamNo1NineCalibPos_6 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_6);
            }
        }

        private string _curCamNo1NineCalibPos_7;
        public string CurCamNo1NineCalibPos_7
        {
            get => _curCamNo1NineCalibPos_7;
            set
            {
                _curCamNo1NineCalibPos_7 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_7);
            }
        }

        private string _curCamNo1NineCalibPos_8;
        public string CurCamNo1NineCalibPos_8
        {
            get => _curCamNo1NineCalibPos_8;
            set
            {
                _curCamNo1NineCalibPos_8 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_8);
            }
        }

        private string _curCamNo1NineCalibPos_9;
        public string CurCamNo1NineCalibPos_9
        {
            get => _curCamNo1NineCalibPos_9;
            set
            {
                _curCamNo1NineCalibPos_9 = value;
                NotifyOfPropertyChange(() => CurCamNo1NineCalibPos_9);
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

        #endregion

        #region Constructor
        public NinePointCalibViewModel()
        {
            _camera_Component = (Camera_Component)IoC.Get<ICamera>();
            _paramManager = IoC.Get<ParamManager>();
            _mGoogol_Component = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            _cacheParamManager = IoC.Get<CacheParamManager>();
            CurCamNo2NozzleNo1Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos);
            CurCamNo2NozzleNo2Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos);
            CurCamNo2NozzleNo3Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos);
            CurCamNo2NozzleNo4Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos);
            CurCamNo1NineCalibPos_1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1);
            CurCamNo1NineCalibPos_2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2);
            CurCamNo1NineCalibPos_3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3);
            CurCamNo1NineCalibPos_4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4);
            CurCamNo1NineCalibPos_5 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5);
            CurCamNo1NineCalibPos_6 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6);
            CurCamNo1NineCalibPos_7 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7);
            CurCamNo1NineCalibPos_8 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8);
            CurCamNo1NineCalibPos_9 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9);

            CalibMoveDis = _cacheParamManager.manualPositionParam.MoveDisNineCalib;
        }
        #endregion

        #region Override

        #endregion

        #region Method

        //public void SetCalibNinePointPosToUpCameraNo1(object obj)
        //{
        //    int index = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show("确定设置1#相机9点标定起始坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        if (index == 1)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1[3] = 0;
        //            CurCamNo1NineCalibPos_1 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1;
        //        }
        //        else if (index == 2)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2[3] = 0;
        //            CurCamNo1NineCalibPos_2 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2;
        //        }
        //        else if (index == 3)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3[3] = 0;
        //            CurCamNo1NineCalibPos_3 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3;
        //        }
        //        else if (index == 4)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4[3] = 0;
        //            CurCamNo1NineCalibPos_4 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4;
        //        }
        //        else if (index == 5)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5[3] = 0;
        //            CurCamNo1NineCalibPos_5 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5;
        //        }
        //        else if (index == 6)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6[3] = 0;
        //            CurCamNo1NineCalibPos_6 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6;
        //        }
        //        else if (index == 7)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7[3] = 0;
        //            CurCamNo1NineCalibPos_7 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7;
        //        }
        //        else if (index == 8)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8[3] = 0;
        //            CurCamNo1NineCalibPos_8 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8;
        //        }
        //        else if (index == 9)
        //        {
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9[2] = 0;
        //            _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9[3] = 0;
        //            CurCamNo1NineCalibPos_9 = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9);
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9;
        //        }

        //        _cacheParamManager.SaveAllParam();

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibNinePointPosToUpCameraNo1({index.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibNinePointPosToUpCameraNo1({index.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //}

        ///// <summary>
        ///// 上相机9点标定
        ///// </summary>
        //public async void MoveSpaceCalibNinePointPosToUpCameraNo1(object obj)
        //{
        //    int index = Convert.ToInt32(obj);
        //    MessageBoxResult mbr = MessageBox.Show($"确定移动到相机9点标定起始坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];

        //        if (index == 1)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1;
        //        }
        //        else if (index == 2)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2;
        //        }
        //        else if (index == 3)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3;
        //        }
        //        else if (index == 4)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4;
        //        }
        //        else if (index == 5)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5;
        //        }
        //        else if (index == 6)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6;
        //        }
        //        else if (index == 7)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7;
        //        }
        //        else if (index == 8)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8;
        //        }
        //        else if (index == 9)
        //        {
        //            pos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9;
        //        }
        //        //Move 指令
        //        //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降     
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Warning("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { pos[0], pos[1] }, true, true)) return;

        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibNinePointPosToUpCameraNo1({index.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //            MessageBox.Success("移动到相机9点标定起始坐标成功！");
        //            return;
        //        });
        //    }
        //    else
        //    {
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibNinePointPosToUpCameraNo1({index.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //    }
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show($"吸嘴不在安全位置,请先将吸嘴轴到安全位置！");
        //    //}
        //}
        ////public void SetCalibNinePointPosToUpCameraNo3()
        ////{
        ////    MessageBoxResult mbr = MessageBox.Show("确定设置3#相机9点标定起始坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        ////    if (mbr == MessageBoxResult.OK)
        ////    {

        ////        _cacheParamManager.manualPositionParam.UpCameraNo3NineCalibPos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X1];
        ////        _cacheParamManager.manualPositionParam.UpCameraNo3NineCalibPos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y1];

        ////        _cacheParamManager.SaveAllParam();
        ////        var pos = _mGoogol_Component.CurPos;
        ////        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibNinePointPosToUpCameraNo3() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        ////        return;
        ////    }
        ////    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibNinePointPosToUpCameraNo3() 弹窗取消操作", En_Logout_Type.SpotCheck);
        ////}
        ////public void MoveSpaceCalibNinePointPosToUpCameraNo3()
        ////{
        ////    MessageBoxResult mbr = MessageBox.Show($"确定3#相机9点标定起始坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        ////    if (mbr == MessageBoxResult.OK)
        ////    {
        ////        float[] pos = new float[4];
        ////        pos = _cacheParamManager.manualPositionParam.UpCameraNo3NineCalibPos;

        ////        //Move 指令
        ////        //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
        ////        _mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.Z2, 0, false, false);
        ////        _mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.R3, pos[3], false, false);
        ////        _mGoogol_Component.MoveAbsoluteToPointXY(En_StationNo.StationNo1, new float[2] { pos[0], pos[1] }, false, false);
        ////        _mGoogol_Component.MoveAbsoluteToPointSingleAxis(En_AxisNum.Z2, pos[2], true, false);

        ////        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibNinePointPosToUpCameraNo3() 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        ////        return;
        ////    }
        ////    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibNinePointPosToUpCameraNo3() 弹窗取消操作", En_Logout_Type.SpotCheck);
        ////}

        ///// <summary>
        ///// 空移上相机到安全位置
        ///// </summary>
        ///// <param name="obj"></param>
        //public void EndCalibration()
        //{
        //    MessageBoxResult mbr = MessageBox.Show($"确定结束标定吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        _camera_Component.EndCalibUN();
        //        MessageBox.Info("等待视觉软件标定结束后即可");
        //    }
        //}

        ///// <summary>
        ///// 空移吸嘴到安全位置
        ///// </summary>
        ///// <param name="obj"></param>
        ////public async void Restsafepos(object obj)
        ////{
        ////    MessageBoxResult mbr = MessageBox.Show($"确定空移吸嘴到安全位置?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        ////    if (mbr == MessageBoxResult.OK)
        ////    {
        ////        await Task.Run(() =>
        ////        {
        ////            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        ////            if (!_mGoogol_Component.MoveAbsoluteToPointXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true))
        ////            {
        ////                return;
        ////            }
        ////            MessageBox.Show("空移吸嘴到安全位置OK");
        ////        });
        ////    }
        ////}

        //public void SetCalibElevenPointPosToDownCamera_Nozzle(object nozzleNo)
        //{
        //    int _nlzzleNo = Convert.ToInt32(nozzleNo);

        //    MessageBoxResult mbr = MessageBox.Show($"确定设置{_nlzzleNo.ToString()}#吸嘴起始标定坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        float[] pos = new float[4];
        //        if (_nlzzleNo == 1)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            //cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R1];
        //            CurCamNo2NozzleNo1Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos);
        //            pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos;
        //        }
        //        else if (_nlzzleNo == 2)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            //_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
        //            CurCamNo2NozzleNo2Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos);
        //            pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos;
        //        }
        //        else if (_nlzzleNo == 3)
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            //_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
        //            CurCamNo2NozzleNo3Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos);
        //            pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos;
        //        }
        //        else
        //        {
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos[0] = _mGoogol_Component.CurPos[(byte)En_AxisNum.X2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos[1] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Y2];
        //            _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos[2] = _mGoogol_Component.CurPos[(byte)En_AxisNum.Z2];
        //            //_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos[3] = _mGoogol_Component.CurPos[(byte)En_AxisNum.R2];
        //            CurCamNo2NozzleNo4Pos = NLogTrace.GetFloatArrayString(_cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos);
        //            pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos;
        //        }
        //        _cacheParamManager.SaveAllParam();

        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibElevenPointPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 坐标为：{NLogTrace.GetFloatArrayString(pos)}", En_Logout_Type.SpotCheck);
        //        return;
        //    }
        //    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"SetCalibElevenPointPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //}

        ///// <summary>
        ///// 空移吸嘴
        ///// </summary>
        ///// <param name="nozzleNo"></param>
        //public async void MoveSpaceCalibElevenPointPosToDownCamera_Nozzle(object nozzleNo)
        //{
        //    int _nlzzleNo = Convert.ToInt32(nozzleNo);
        //    MessageBoxResult mbr = MessageBox.Show($"确定空移{_nlzzleNo.ToString()}#吸嘴起始标定坐标吗?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        //    if (mbr == MessageBoxResult.OK)
        //    {

        //        //{
        //        await Task.Run(() =>
        //        {
        //            //false说明有轴在运动
        //            if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //            {
        //                MessageBox.Show("轴系在运动，等静止再操作！");
        //                return;
        //            }
        //            float[] pos = new float[4];
        //            if (_nlzzleNo == 1)
        //            {
        //                pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos;
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, false);
        //            }
        //            else if (_nlzzleNo == 2)
        //            {
        //                pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos;
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo2, false);
        //            }
        //            else if (_nlzzleNo == 3)
        //            {
        //                pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos;
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo3, false);
        //            }
        //            else
        //            {
        //                pos = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos;
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, false);
        //            }
        //            //Move 指令
        //            //先Z上抬到0，然后 R轴旋转 最后XY ，Z下降
        //            if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis((En_AxisNum)(_nlzzleNo + 4), pos[3], true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { pos[0], pos[1] }, true, true)) return;
        //            if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, pos[2], true, true)) return;
        //            if (_nlzzleNo == 1)
        //            {
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //            }
        //            else if (_nlzzleNo == 2)
        //            {
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo2, true);
        //            }
        //            else if (_nlzzleNo == 3)
        //            {
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo3, true);
        //            }
        //            else
        //            {
        //                //_mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //            }
        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibElevenPointPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 坐标为：{pos[0].ToString("f2")},{pos[1].ToString("f2")},{pos[2].ToString("f2")},{pos[3].ToString("f2")}", En_Logout_Type.SpotCheck);
        //            MessageBox.Show("空移{ _nlzzleNo.ToString()}#吸嘴起始标定坐标成功！");
        //            return;
        //        });
        //        //}
        //        //else
        //        //{
        //        //    MessageBox.Show($"上相机不在安全位置,请先将上相机运动到安全位置！");
        //        //}
        //    }
        //    else
        //    {
        //        NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"MoveSpaceCalibElevenPointPosToDownCamera_Nozzle({_nlzzleNo.ToString()}) 弹窗取消操作", En_Logout_Type.SpotCheck);
        //    }
        //}
        //public async void CalibElevenPoint_Nozzle(object nozzleNo)
        //{
        //    int _nlzzleNo = Convert.ToInt32(nozzleNo);

        //    int _camNo = 1;

        //    MessageBoxResult mbr = MessageBox.Show($"确定执行{_nlzzleNo.ToString()}号吸嘴11点标定吗 ?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

        //    //{
        //    if (mbr == MessageBoxResult.OK)
        //    {
        //        await Task.Run(() =>
        //            {
        //                //false说明有轴在运动
        //                if (!_mGoogol_Component.GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
        //                {
        //                    MessageBox.Show("轴系在运动，等静止再操作！");
        //                    return;
        //                }
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CalibNinePointToUpCamera({nozzleNo.ToString()})->开始标定", En_Logout_Type.SpotCheck);

        //                int cmdSerialNumber = 0;
        //                if (_nlzzleNo == 1)
        //                {
        //                    cmdSerialNumber = 7;
        //                }
        //                else if (_nlzzleNo == 2)
        //                {
        //                    cmdSerialNumber = 7;
        //                }
        //                else if (_nlzzleNo == 3)
        //                {
        //                    cmdSerialNumber = 8;
        //                }
        //                else if (_nlzzleNo == 4)
        //                {
        //                    cmdSerialNumber = 9;
        //                }

        //                if (!_mGoogol_Component.SetSpeedAll(En_SpeedType.Mid)) return;
        //                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //                if (!_camera_Component.SendScCalib(cmdSerialNumber, 11))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->CalibrationStart() 标定启动失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("标定启动失败");
        //                    return;
        //                }

        //                var movdis = CalibMoveDis;
        //                var newPos = new float[4];
        //                var orgpnt = new float[4];

        //                if (_nlzzleNo == 1)
        //                {
        //                    orgpnt = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo1Pos;//_cacheParamManager.manualPositionParam.UpCameraCalibPos;
        //                    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo1, true);
        //                }
        //                else if (_nlzzleNo == 2)
        //                {

        //                    orgpnt = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo2Pos;
        //                    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo2, true);
        //                }
        //                else if (_nlzzleNo == 3)
        //                {
        //                    orgpnt = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo3Pos;
        //                    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo3, true);
        //                }
        //                else
        //                {
        //                    orgpnt = _cacheParamManager.manualPositionParam.AxisCalibNineAddTwo_NozzleNo4Pos;
        //                    _mGoogol_Component.WriteOutPort((byte)(_mGoogol_Component.Param as MotionGoogolParam).OutCylinderDownNo4, true);
        //                }


        //                //newPos[0] = orgpnt[0] - movdis;
        //                //newPos[1] = orgpnt[1] + movdis;
        //                newPos[0] = orgpnt[0];
        //                newPos[1] = orgpnt[1];
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动1号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到一号点失败");
        //                    return;
        //                }
        //                _mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, orgpnt[2], true, true);

        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第一个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第一个点标定失败");
        //                    return;
        //                }

        //                newPos[0] = orgpnt[0];
        //                newPos[1] = orgpnt[1] + movdis;
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动2号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到二号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第二个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第二个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0] + movdis;
        //                //newPos[1] = orgpnt[1] + movdis;
        //                newPos[0] = orgpnt[0] - movdis;
        //                newPos[1] = orgpnt[1] + movdis;
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动3号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到三号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第三个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第三个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0] + movdis;
        //                //newPos[1] = orgpnt[1];
        //                newPos[0] = orgpnt[0] - movdis;
        //                newPos[1] = orgpnt[1];

        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动4号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到四号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第四个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第四个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0];
        //                //newPos[1] = orgpnt[1];
        //                newPos[0] = orgpnt[0] - movdis;
        //                newPos[1] = orgpnt[1] - movdis;
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动5号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到五号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第五个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第五个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0] - movdis;
        //                //newPos[1] = orgpnt[1];
        //                newPos[0] = orgpnt[0];
        //                newPos[1] = orgpnt[1] - movdis;

        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动6号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到六号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(_nlzzleNo > 2 ? En_StationNo.StationNo2 : En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第六个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第六个点标定失败");
        //                    return;
        //                }


        //                //newPos[0] = orgpnt[0] - movdis;
        //                //newPos[1] = orgpnt[1] - movdis;

        //                newPos[0] = orgpnt[0] + movdis;
        //                newPos[1] = orgpnt[1] - movdis;

        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动7号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到七号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第七个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0];
        //                //newPos[1] = orgpnt[1] - movdis;
        //                newPos[0] = orgpnt[0] + movdis;
        //                newPos[1] = orgpnt[1];
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动8号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到八号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第八个点标定失败");
        //                    return;
        //                }

        //                //newPos[0] = orgpnt[0] + movdis;
        //                //newPos[1] = orgpnt[1] - movdis;
        //                newPos[0] = orgpnt[0] + movdis;
        //                newPos[1] = orgpnt[1] + movdis;

        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动9号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到九号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);

        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], newPos[2]))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第九个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第九个点标定失败");
        //                    return;
        //                }


        //                newPos[0] = orgpnt[0];
        //                newPos[1] = orgpnt[1];
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true))
        //                {
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动10号点：{NLogTrace.GetFloatArrayString(newPos)}", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("运动到十号点失败");
        //                    return;
        //                }
        //                Thread.Sleep(200);

        //                var selR = En_AxisNum.R1;
        //                if (_nlzzleNo == 1)
        //                    selR = En_AxisNum.R1;
        //                else if (_nlzzleNo == 2)
        //                    selR = En_AxisNum.R2;
        //                else if (_nlzzleNo == 3)
        //                    selR = En_AxisNum.R3;
        //                else if (_nlzzleNo == 4)
        //                    selR = En_AxisNum.R4;

        //                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(selR, -5, true, true))
        //                {
        //                    MessageBox.Show("运动到十号点失败");
        //                    return;
        //                }

        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], -5))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { newPos[0], newPos[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第十个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第十个点标定失败");
        //                    return;
        //                }

        //                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(selR, 5, true, true))
        //                {
        //                    MessageBox.Show("运动到11号点失败");
        //                    return;
        //                }

        //                Thread.Sleep(200);
        //                if (!_camera_Component.CalibProcess(cmdSerialNumber, newPos[0], newPos[1], 5))
        //                {
        //                    _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第11个点标定失败", En_Logout_Type.SpotCheck);
        //                    MessageBox.Show("第11个点标定失败");
        //                    return;
        //                }

        //                _camera_Component.SendEcCalib();


        //                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(selR, 0, true, true)) return;
        //                if (!_mGoogol_Component.MoveAbsoluteSingleAxis(En_AxisNum.Z2, 0, true, true)) return;
        //                if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo2, new float[2] { 0, 0 }, true, true)) return;

        //                if (_nlzzleNo == 4)
        //                {
        //                    if (MessageBox.Show("确定2、3号吸嘴11点标定完毕了吗？结束标定UN", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //                    {
        //                        _camera_Component.EndCalibUN();
        //                    }
        //                }

        //                MessageBox.Info("标定成功,等待视觉软件标定结果");
        //                //MessageBox.Show("标定成功");
        //                return;
        //            });
        //    }
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show($"上相机不在安全位置,请先将上相机运动到安全位置！");
        //    //}
        //}
        //public async void CalibNinePoint(object camNo)
        //{
        //    int _camNo = Convert.ToInt32(camNo);
        //    MessageBoxResult mbr = MessageBox.Show($"确定执行{_camNo.ToString()}号相机9点标定吗 ?", "操作提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
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
        //            NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"CalibNinePointToUpCamera({camNo.ToString()})->开始标定", En_Logout_Type.SpotCheck);

        //            int cmdEncode = 0;
        //            if (_camNo == 1)
        //            {
        //                cmdEncode = 6;
        //            }
        //            //else if (_camNo == 3)
        //            //{
        //            //    cmdEncode = 5;
        //            //}

        //            if (!_camera_Component.SendScCalib(cmdEncode, 9))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->CalibrationStart() 标定启动失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("标定启动失败");
        //                return;
        //            }
        //            var movdis = CalibMoveDis;
        //            var newPos = new float[4];
        //            var orgpnt = new float[4];
        //            //if (_camNo == 1)
        //            //{
        //            //    orgpnt = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos;
        //            //}
        //            //else if (_camNo == 3)
        //            //{
        //            //    orgpnt = _cacheParamManager.manualPositionParam.UpCameraNo3NineCalibPos;
        //            //}
        //            _mGoogol_Component.SetSpeedAll(En_SpeedType.Mid);

        //            //newPos[0] = orgpnt[0] - movdis;
        //            //newPos[1] = orgpnt[1] + movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_1;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动1号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到一号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第一个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第一个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0];
        //            //newPos[1] = orgpnt[1] + movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_2;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动2号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到二号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第二个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第二个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0] + movdis;
        //            //newPos[1] = orgpnt[1] + movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_3;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动3号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到三号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第三个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第三个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0] + movdis;
        //            //newPos[1] = orgpnt[1];
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_4;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动4号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到四号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第四个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第四个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0];
        //            //newPos[1] = orgpnt[1];
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_5;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动5号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到五号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第五个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第五个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0] - movdis;
        //            //newPos[1] = orgpnt[1];
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_6;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动6号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到六号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第六个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第六个点标定失败");
        //                return;
        //            }


        //            //newPos[0] = orgpnt[0] - movdis;
        //            //newPos[1] = orgpnt[1] - movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_7;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动7号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到七号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第七个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0];
        //            //newPos[1] = orgpnt[1] - movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_8;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动8号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到八号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第八个点标定失败");
        //                return;
        //            }

        //            //newPos[0] = orgpnt[0] + movdis;
        //            //newPos[1] = orgpnt[1] - movdis;
        //            newPos = _cacheParamManager.manualPositionParam.UpCameraNo1NineCalibPos_9;
        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { newPos[0], newPos[1] }, true, true))
        //            {
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->运动9号点：{newPos[0].ToString("f2")},{newPos[1].ToString("f2")}", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("运动到九号点失败");
        //            }
        //            Thread.Sleep(200);
        //            if (!_camera_Component.CalibProcess(cmdEncode, newPos[0], newPos[1], 0))
        //            {
        //                _mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { orgpnt[0], orgpnt[1] }, true, true);
        //                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"CalibNinePointToUpCamera->第七个点标定失败", En_Logout_Type.SpotCheck);
        //                MessageBox.Show("第九个点标定失败");
        //                return;
        //            }

        //            if (!_mGoogol_Component.MoveAbsoluteXY(En_StationNo.StationNo1, new float[2] { 0, 0 }, true, true)) return;

        //            _camera_Component.SendEcCalib();

        //            MessageBox.Info("标定指令结束，需要等待视觉软件计算标定结果，可能需要一定时间，耐心等待！");
        //            return;
        //        });
        //    }
        //}

        //public void NumUDMoveDisChanged(object obj)
        //{
        //    NumericUpDown val = (NumericUpDown)obj;

        //    _cacheParamManager.manualPositionParam.MoveDisNineCalib = (float)val.Value;

        //    _cacheParamManager.SaveManualPositionParam();

        //}
        #endregion
    }
}
