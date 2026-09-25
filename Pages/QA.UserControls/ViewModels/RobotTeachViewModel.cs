/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-31
 * 说明：（示教界面运动控制逻辑）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Caliburn.Micro;
using QA.Business.Component.Motion.Googol;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.UserControls.Interfaces;

namespace QA.UserControls.ViewModels
{
    [Export("RobotTeachViewModel", typeof(IUserControl))]
    public class RobotTeachViewModel : Screen, IUserControl
    {
        public enum EnumSpeedType
        {
            Low,
            Middle,
            High,
        }
        private MotionGoogol_Component mGoogol;
        private ParamManager paramManager;
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private DispatcherTimer readPositionTimer = new DispatcherTimer();
        private float[] positionXYZ = new float[] { 0, 0, 0 };



        public List<EnumSpeedType> SpeedTypes { get; set; } = new List<EnumSpeedType>()
        {
            EnumSpeedType.Low,
            EnumSpeedType.Middle,
            EnumSpeedType.High,
        };

        private int _speedTypeSelectIndex = 0;
        public int SpeedTypeSelectIndex
        {
            get { return _speedTypeSelectIndex; }
            set { _speedTypeSelectIndex = value; NotifyOfPropertyChange(() => SpeedTypeSelectIndex); }
        }

        //所有工站
        private List<string> _allStations;
        public List<string> AllStations
        {
            get { return _allStations; }
            set { _allStations = value; NotifyOfPropertyChange(() => AllStations); }
        }

        //选择工站
        private int _curStation = 1;
        public int CurStation
        {
            get { return _curStation; }
            set { _curStation = value; NotifyOfPropertyChange(() => CurStation); }
        }

        //速度类型
        private EnumSpeedType _curSpeedType = EnumSpeedType.Low;
        public EnumSpeedType CurSpeedType
        {
            get { return _curSpeedType; }
            set { _curSpeedType = value; NotifyOfPropertyChange(() => CurSpeedType); }
        }

        //是否点动
        private bool _isDotMode = false;
        public bool IsDotMove
        {
            get { return _isDotMode; }
            set { _isDotMode = value; NotifyOfPropertyChange(() => IsDotMove); }
        }

        //点动步长
        private float _dotStepLenth = 0.01f;
        public float DotStepLenth
        {
            get { return _dotStepLenth; }
            set { _dotStepLenth = value; NotifyOfPropertyChange(() => DotStepLenth); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="windowManager"></param>
        [ImportingConstructor]
        public RobotTeachViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();//windowManager;
            _eventAggregator = IoC.Get<IEventAggregator>();//eventAggregator;
            mGoogol = (MotionGoogol_Component)IoC.Get<IMGoogol>();
            paramManager = IoC.Get<ParamManager>();
            DisplayName = "RobotTeach";

            _allStations = new List<string>() { "1", "2", "3", "4" };

            readPositionTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            readPositionTimer.Tick += new EventHandler(readPositionTimer_Action);
            readPositionTimer.IsEnabled = true;
            readPositionTimer.Start();
        }

        private void readPositionTimer_Action(object sender, EventArgs e)
        {
            //if (mGoogol.ReadStationPrfXYZR((short)CurStation, ref positionXYZ) == false) return;

            //Position_X = positionXYZ[0];
            //Position_Y = positionXYZ[1];
            //Position_Z = positionXYZ[2];
        }
        public void SwitchStation(string msg)
        {
            CurStation = int.Parse(msg);
        }

        /// <summary>
        /// ROBOT轴移动/停止
        /// </summary>
        /// <param name="dirmsg"></param>
        /// <param name="isMove"></param>
        public void RobotMove(string dirmsg, bool isMove)
        {
        }
        //public void RobotMove(string dirmsg, bool isMove)
        //{
        //    float[] speed = new float[5];
        //    //switch (SpeedTypeSelectIndex)
        //    //{
        //    //    case 0:
        //    //        speed = paramManager.MotionGoogolParam.teachLowSpeed;
        //    //        break;
        //    //    case 1:
        //    //        speed = paramManager.MotionGoogolParam.teachMidSpeed;
        //    //        break;
        //    //    case 2:
        //    //        speed = paramManager.MotionGoogolParam.teachHighSpeed;
        //    //        break;
        //    //    default:
        //    //        speed = paramManager.MotionGoogolParam.teachLowSpeed;
        //    //        break;
        //    //}
        //    //点动模式
        //    if (IsDotMove)
        //    {
        //        switch (dirmsg)
        //        {
        //            case "Forward":
        //                if (CurStation == 1 || CurStation == 3)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.LeftY, DotStepLenth, speed,true);
        //                }
        //                else if (CurStation == 2 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.RightY, DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTY, DotStepLenth, speed, true);
        //                }
        //                break;
        //            case "Backward":
        //                if (CurStation == 1 || CurStation == 3)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.LeftY, -DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 2 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.RightY, -DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTY, -DotStepLenth, speed, true);
        //                }
        //                break;
        //            case "Left":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.SolderX, -DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.VisionX, -DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTX, -DotStepLenth, speed, true);
        //                }
        //                break;
        //            case "Right":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.SolderX, DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.VisionX, DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTX, DotStepLenth, speed, true);
        //                }
        //                break;
        //            case "Up":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.SolderZ, -DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.VisionZ, -DotStepLenth, speed, true);
        //                }
        //                break;
        //            case "Down":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.SolderZ, DotStepLenth, speed, true);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.MoveRelativeToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.VisionZ, DotStepLenth, speed, true);
        //                }
        //                break;
        //        }

        //    }
        //    else//Jog模式
        //    {
        //        switch (dirmsg)
        //        {
        //            case "Forward":
        //                if (CurStation == 1 || CurStation == 3)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.LeftY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.LeftY, 1, isMove);
        //                }
        //                else if (CurStation == 2 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.RightY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.RightY, 1, isMove);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.AJUSTY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.AJUSTY, 1, isMove);
        //                }
        //                break;
        //            case "Backward":
        //                if (CurStation == 1 || CurStation == 3)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.LeftY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.LeftY, -1, isMove);
        //                }
        //                else if (CurStation == 2 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.RightY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.RightY, -1, isMove);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.AJUSTY, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.AJUSTY, -1, isMove);
        //                }
        //                break;
        //            case "Left":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.SolderX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.SolderX, -1, isMove);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.VisionX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.VisionX, -1, isMove);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.AJUSTX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.AJUSTX, -1, isMove);
        //                }
        //                break;
        //            case "Right":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.SolderX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.SolderX, 1, isMove);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.VisionX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.VisionX, 1, isMove);
        //                }
        //                else if (CurStation == 0)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.AJUSTX, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.AJUSTX, 1, isMove);
        //                }
        //                break;
        //            case "Up":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.SolderZ, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.SolderZ, -1, isMove);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.VisionZ, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.VisionZ, -1, isMove);
        //                }
        //                break;
        //            case "Down":
        //                if (CurStation == 1 || CurStation == 2)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.SolderZ, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.SolderZ, 1, isMove);
        //                }
        //                else if (CurStation == 3 || CurStation == 4)
        //                {
        //                    mGoogol.SetJogSpeed(MotionGoogol_Component.EN_AxisNum.VisionZ, speed);
        //                    mGoogol.JogAxis(MotionGoogol_Component.EN_AxisNum.VisionZ, 1, isMove);
        //                }
        //                break;
        //        }

        //    }
        //    Console.WriteLine($"RobotMove {dirmsg} {isMove}");

        //    //TODO ROBOT轴移动/停止
        //}

        /// <summary>
        /// 小滑台复位
        /// </summary>
        public void RobotReset()
        {
            //mGoogol.ResetAjustAxis(true);
            //mGoogol.MoveAbsoluteToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTX, paramManager.MotionGoogolParam.ResetPos_Small_X + paramManager.MotionGoogolParam.RedOffset_X, paramManager.MotionGoogolParam.WorkSpeedSmall_XY, false);
            //mGoogol.MoveAbsoluteToPointSingleAxis(MotionGoogol_Component.EN_AxisNum.AJUSTY, paramManager.MotionGoogolParam.ResetPos_Small_Y + paramManager.MotionGoogolParam.RedOffset_Y, paramManager.MotionGoogolParam.WorkSpeedSmall_XY, false);
        }


        /// <summary>
        /// ROBOT切换速度
        /// </summary>
        public void RobotChangSpeedType()
        {
            if (CurSpeedType == EnumSpeedType.High)
                CurSpeedType = EnumSpeedType.Low;
            else
                CurSpeedType++;
        }

        /// <summary>
        /// X坐标
        /// </summary>
        private float _position_X;
        public float Position_X
        {
            get { return _position_X; }
            set { _position_X = value; NotifyOfPropertyChange(() => Position_X); }
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        private float _position_Y;
        public float Position_Y
        {
            get { return _position_Y; }
            set { _position_Y = value; NotifyOfPropertyChange(() => Position_Y); }
        }

        /// <summary>
        /// Z坐标
        /// </summary>
        private float _position_Z;
        public float Position_Z
        {
            get { return _position_Z; }
            set { _position_Z = value; NotifyOfPropertyChange(() => Position_Z); }
        }
    }
}
