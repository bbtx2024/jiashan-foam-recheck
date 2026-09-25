/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-24
 * 说明：（固高运动控制）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CS_QMotion;
using QA.Business.Component.PLC;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA.Business.Message;
using QA.Business.Model.Alarm;
using QA_Infrastructure;
using QA_Infrastructure.NLogOut;

namespace QA.Business.Component.Motion.Googol
{
    public enum En_SpeedType
    {
        Low,
        Mid,
        High,
        Work
    }
    public enum En_RobotWorkCtrl
    {
        Pause = 0x01,
        Resume = 0x02,
        Stop = 0x03,
        SlowPause = 0x28,
    }
    public enum En_AxisNum
    {
        X1 = 0,
        Y1,
        Z1,
        X2,
        Y2,
        Z2,
        R1,
        R2,
        R3,
        R4,
    }
    public enum En_AxisStatus
    {
        Axis_Alarm = 0x02,//伺服报警
        Axis_MoveErr = 0x10,//运动出错
        Axis_PosLimit = 0x20,//触发正限位
        Axis_NegLimit = 0x40,//触发负限位
        Axis_EStop = 0x100,//急停报警
        Axis_Power = 0x200,//电机使能
    }
    public enum En_StationNo
    {
        StationNo1,
        StationNo2,
        StationNo3,
    }

    public enum En_GetAxisClrSts
    {
        //Bit0 保留
        Bit0_Reserved = 0x01,
        //Bit1 驱动器报警标志 控制轴连接的驱动器报警时置 1
        Bit1_DriverAlarm = 0x02,
        //Bit2 保留
        Bit2_Reserved = 0x04,
        //Bit3 保留
        Bit3_Reserved = 0x08,
        //Bit4 跟随误差越限标志 控制轴规划位置和实际位置的误差大于设定极限时置 1 
        Bit4_FollowOverLimited = 0x10,
        //Bit5 正限位触发标志 正限位开关电平状态为限位触发电平时置 1规划位置大于正向软限位时置 1
        Bit5_PositiveLimitTriggered = 0x20,
        //Bit6 负限位触发标志 负限位开关电平状态为限位触发电平时置 1规划位置小于负向软限位时置 1
        Bit6_NegtiveLimitTriggered = 0x40,
        //Bit7 IO 平滑停止触发标志 如果轴设置了平滑停止 IO，当其输入为触发电平时置 1，并自动平滑停止该轴
        Bit7_IOSmoothStopTriggered = 0x80,
        //Bit8 IO 急停触发标志 如果轴设置了急停 IO，当其输入为触发电平时置 1，并自动急停该轴
        Bit8_EmergencyStopTriggered = 0x100,
        //Bit9 电机使能标志 电机使能时置 1
        Bit9_MotorEnabled = 0x200,
        //Bit10 规划运动标志 规划器运动时置 1
        Bit10_MoveEnabled = 0x400,
        //Bit11 电机到位标志 规划器静止，规划位置和实际位置的误差小于设定误差带，并且在误差带内保持设定时间后，置起到位标志
        Bit11_MotorInPlaceReady = 0x800,
    }

    public class MotionGoogol_Component : IMGoogol, IMotion
    {
        #region Field   
        private readonly int _axisNum = 9;//该机台总共使用的轴数为9个轴
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        private QMotion qMotion = new QMotion();
        private MotionGoogolParam _motionGoogolParam;
        private RobotRealStateInfo _curRobotStatusInfo;
        //private PLC_Component _plc_Component;
        //private IEventAggregator _eventAggregator;
        private ParamManager _paramManager = null;
        private int[] _ctrlSts = new int[9];//每个轴状态     

        private readonly float _axisX1MaxStroke = 355f;//核心参数不能乱改，容易撞
        private readonly float _axisY2MaxStroke = 480f;//核心参数不能乱改，容易撞
        private readonly float _axisY1Y2SafeMaxDis = 250 + 140;
        #endregion

        #region property     
        public IParam Param { get; set; } = null;
        public string ComponentName { get; set; } = "MotionGoogol";
        public bool IsConnected { get; set; }

        public bool IsResetCompleted { get; set; }

        private float[] Pulseequ = new float[10];
        public float[] CurPos { get; internal set; } = new float[Enum.GetValues(typeof(En_AxisNum)).Length];
        public ushort EInput { get; set; }
        public ushort EOutput { get; set; }
        public int[] AxisSts { get; set; } = new int[SysConfig.AxisCount];
        public short[] AInput { get; set; } = new short[6];     //模拟输入

        #endregion

        #region Event
        public event Action<RobotRealStateInfo> RobotValueRefresh;
        #endregion

        #region Constructor
        public MotionGoogol_Component()
        {
            Param = new MotionGoogolParam();  //固高运动板卡的参数放在组件类中
            _motionGoogolParam = IoC.Get<MotionGoogolParam>();
            _curRobotStatusInfo = new RobotRealStateInfo();
            //_eventAggregator = IoC.Get<IEventAggregator>();
        }
        #endregion

        #region Method
        public bool Initial(IParam param)
        {
            try
            {
                Param = _motionGoogolParam = (param as MotionGoogolParam).DeepCopy();

                //Pulseequ = new float[10] {
                //    _motionGoogolParam.PluseEquivalentX1,
                //    _motionGoogolParam.PluseEquivalentY1,
                //    _motionGoogolParam.PluseEquivalentX2,
                //    _motionGoogolParam.PluseEquivalentY2,
                //    _motionGoogolParam.PluseEquivalentZ2,
                //    _motionGoogolParam.PluseEquivalentR1,
                //    _motionGoogolParam.PluseEquivalentR2,
                //    _motionGoogolParam.PluseEquivalentR3,
                //    _motionGoogolParam.PluseEquivalentR4,
                //};

                Pulseequ = new float[10] {
                    1280,
                    1280,
                    1280,
                    1280,
                    2560,
                    138.89f,
                    138.89f,
                    138.89f,
                    138.89f,
                    138.89f
                };
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, e + e.StackTrace);
                return false;
            }
            return true;
        }
        public bool SetSoftLimit()
        {
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.X1, true, 1, 355 * Pulseequ[(byte)En_AxisNum.X1], -100f))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.Y1, true, 1, 250 * Pulseequ[(byte)En_AxisNum.Y1], -100f))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.X2, true, 1, 305 * Pulseequ[(byte)En_AxisNum.X2], -100f))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.Y2, true, 1, 480 * Pulseequ[(byte)En_AxisNum.Y2], -100f))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.Z2, true, 1, 50 * Pulseequ[(byte)En_AxisNum.Z2], -100f))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.R1, true, 1, 250 * Pulseequ[(byte)En_AxisNum.R1], -250f * Pulseequ[(byte)En_AxisNum.R1]))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.R2, true, 1, 250 * Pulseequ[(byte)En_AxisNum.R2], -250f * Pulseequ[(byte)En_AxisNum.R2]))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.R3, true, 1, 250 * Pulseequ[(byte)En_AxisNum.R3], -250f * Pulseequ[(byte)En_AxisNum.R3]))
                return false;
            if (!qMotion.AxisSetSoftLimit((byte)En_AxisNum.R4, true, 1, 250 * Pulseequ[(byte)En_AxisNum.R4], -250f * Pulseequ[(byte)En_AxisNum.R4]))
                return false;
            return true;
        }
        public bool SetServeOnOff(bool isOpen, short axisId = -1)
        {
            //axisId = -1 默认所有轴开关伺服
            //axisId = 0,代表X1轴关闭，后面依次类推
            return qMotion.SetAxisOnOff(isOpen, axisId);
        }
        public bool Connect()
        {
            short errcode = 0;
            short getAxisNum = 0;
            //传入轴的数目、脉冲当量、输出出错代码
            bool ret = qMotion.Init(_axisNum, Pulseequ, ref getAxisNum, ref errcode);
            if (errcode == 1)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "打开运动控制器失败", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 2)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "EtherCAT初始化，通讯未完全建立，启动总线通讯失败", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 3)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "复位运动控制器失败", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 4)
            {
                //下载配置信息到运动控制器，调用该指令后需再调用 GTN_ClrSts才能使该指令生效
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "下载配置信息到运动控制器失败", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 5)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "读取EtherCAT总线的在线从站数目失败或数目不对", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 6)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"读取EtherCAT总线的在线从站数目不对,当前数目：{getAxisNum.ToString()}", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 7)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "清除控制器报警失败", En_Logout_Type.Run, true);
                return false;
            }
            else if (errcode == 8)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "扩展模块初始化失败", En_Logout_Type.Run, true);
                return false;
            }
            if (!SetServeOnOff(true))
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "控制器上伺服使能失败", En_Logout_Type.Run, true);
                return false;
            }

            DateTime starttime = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - starttime).TotalSeconds > 3)
                {
                    break;
                }
                Thread.Sleep(200);
                if (GetServoEnabledStatus())
                {
                    //double dt = DateTime.Now.Subtract(starttime).TotalMilliseconds;
                    NLogTrace.LogOut(EN_WARN_LEVEL.Sucess, "控制器伺服使能成功！", En_Logout_Type.Run, true);
                    return true;
                }
            }

            if (!GetServoEnabledStatus())
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, "控制器获取伺服使能状态使能有问题", En_Logout_Type.Run, true);
                return false;
            }
            return true;
        }
        public bool Start()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                int _einput = 0;
                int[] sts0 = new int[9];
                int[] sts1 = new int[1];
                Task.Factory.StartNew(async () =>
                {
                    IsConnected = Connect();

                    while (true)
                    {
                        var delayTime = _motionGoogolParam.BUse ? 10 : 1000;
                        await Task.Delay(delayTime, _cancellationToken);
                        try
                        {
                            if (_cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            GetCurPos();
                            //CurRobotStatusInfoEvent();占用资源太大不要启用
                            if (!GetInput(ref _einput))
                            {
                                IsConnected = false;
                            }
                            EInput = (ushort)_einput;

                            short[] ain = new short[6] { 0, 0, 0, 0, 0, 0 };
                            qMotion.GetAIn(1, 0, 6, ref ain);
                            AInput = ain;

                            if (!qMotion.AxisGetSts(0, ref sts0, 8) || !qMotion.AxisGetSts(8, ref sts1, 1))
                            {
                                IsConnected = false;
                            }
                            sts0[8] = sts1[0];
                            _ctrlSts = sts0;

                        }
                        catch (Exception e)
                        {
                            NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
                        }

                    }
                }, _cancellationToken);
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.CriticalError, ex.Message + ex.StackTrace, En_Logout_Type.Exception);
            }
            return true;
        }
        public bool Stop()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
            }
            return true;
        }
        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
            if (Param.BUse)
            {
                if (!IsConnected)
                {
                    var count0 = alarmInfos.Count;
                    alarmInfos.Add(new AlarmInfoModel()
                    {
                        AlarmLevel = EN_WARN_LEVEL.Error,
                        AlarmModule = EN_WarnModules.Robot,
                        AlarmMsg = "无连接",
                        Datetime = DateTime.Now,
                        ErrorCode = 0,
                        Index = count0
                    });

                    for (int i = 0; i < _ctrlSts.Length; i++)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if ((_ctrlSts[i] & 0x01 << j) > 0x00)
                            {
                                if (0x01 << j == (int)En_GetAxisClrSts.Bit1_DriverAlarm)
                                {
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴驱动器报警",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                                else if (0x01 << j == (int)En_GetAxisClrSts.Bit4_FollowOverLimited)
                                {
                                    //跟随误差越限标记
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴跟随误差越限",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                                else if (0x01 << j == (int)En_GetAxisClrSts.Bit5_PositiveLimitTriggered)
                                {
                                    //正限位触发标志
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴正限位触发",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                                else if (0x01 << j == (int)En_GetAxisClrSts.Bit6_NegtiveLimitTriggered)
                                {
                                    //负限位触发标志
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴负限位触发",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                                else if (0x01 << j == (int)En_GetAxisClrSts.Bit7_IOSmoothStopTriggered)
                                {
                                    //IO 平滑停止触发标志
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴IO 平滑停止触发",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                                else if (0x01 << j == (int)En_GetAxisClrSts.Bit8_EmergencyStopTriggered)
                                {
                                    //急停触发标志
                                    var count = alarmInfos.Count;
                                    alarmInfos.Add(new AlarmInfoModel()
                                    {
                                        AlarmLevel = EN_WARN_LEVEL.Error,
                                        AlarmModule = EN_WarnModules.Robot,
                                        AlarmMsg = $"第{i.ToString()}号轴急停触发",
                                        Datetime = DateTime.Now,
                                        ErrorCode = 0,
                                        Index = count
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 获取轴状态

        public bool GetServoEnabledStatus()
        {
            int[] sts0 = new int[9];
            int[] sts1 = new int[1];
            if (!qMotion.AxisGetSts(0, ref sts0, 8))
                return false;
            if (!qMotion.AxisGetSts(8, ref sts1, 1))
                return false;

            sts0[8] = sts1[0];
            for (int i = 0; i < sts0.Length; i++)
            {
                if ((sts0[i] & (int)En_GetAxisClrSts.Bit9_MotorEnabled) <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        //Bit10_MoveEnabled ：注意false 说明在运动 ，true代表静止
        public bool GetAxisClrSts(En_GetAxisClrSts clrSts)
        {
            int[] sts0 = new int[9];
            int[] sts1 = new int[1];
            if (!qMotion.AxisGetSts(0, ref sts0, 8))
                return false;
            if (!qMotion.AxisGetSts(8, ref sts1, 1))
                return false;

            sts0[8] = sts1[0];
            for (int i = 0; i < sts0.Length; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if ((sts0[i] & 0x01 << j) > 0x00)
                    {
                        if (0x01 << j == (int)clrSts)
                        {
                            //报警
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool ExistAlarmAxisClrSts()
        {
            int[] sts0 = new int[9];
            int[] sts1 = new int[1];
            if (!qMotion.AxisGetSts(0, ref sts0, 8))
                return false;
            if (!qMotion.AxisGetSts(8, ref sts1, 1))
                return false;

            sts0[8] = sts1[0];

            for (int i = 0; i < sts0.Length; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if ((sts0[i] & 0x01 << j) > 0x00)
                    {
                        if (0x01 << j == (int)En_GetAxisClrSts.Bit1_DriverAlarm)
                        {
                            //驱动器报警
                            return false;
                        }
                        else if (0x01 << j == (int)En_GetAxisClrSts.Bit4_FollowOverLimited)
                        {
                            //跟随误差越限标志
                            return false;
                        }
                        else if (0x01 << j == (int)En_GetAxisClrSts.Bit5_PositiveLimitTriggered)
                        {
                            //正限位触发标志
                            return false;
                        }
                        else if (0x01 << j == (int)En_GetAxisClrSts.Bit6_NegtiveLimitTriggered)
                        {
                            //负限位触发标志
                            return false;
                        }
                        else if (0x01 << j == (int)En_GetAxisClrSts.Bit7_IOSmoothStopTriggered)
                        {
                            //IO 平滑停止触发标志
                            return false;
                        }
                        else if (0x01 << j == (int)En_GetAxisClrSts.Bit8_EmergencyStopTriggered)
                        {
                            //急停触发标志
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return true;
                }
            }
            return true;
        }

        /// <summary>
        /// 轴速度转换成脉冲数
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private float[] ConvertToPluse(En_AxisNum axis, float[] values)
        {
            float[] pluses = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                pluses[i] = values[i] * _motionGoogolParam.PluseEquivalents[(int)axis];
            }
            return pluses;
        }

        #endregion
        /// <summary>
        /// 轴坐标转换成脉冲数
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private float ConvertToPluse(En_AxisNum axis, float value)
        {
            return value * _motionGoogolParam.PluseEquivalents[(int)axis];
        }

        #region 碰撞保护
        public bool StationNo1InSafeRange()
        {
            //115 核心参数，不能乱改，后果自负
            //运动相轴系，需要判断吸嘴轴系的Y是否在安全位置
            if (!GetCurPos())
            {
                return false;
            }
            var pos = CurPos;

            if (pos[(byte)En_AxisNum.Y2] < 120) return true;
            else return false;
        }
        public bool StartCollisionCheck()
        {
            bool ret = false;
            //bool lastStatus_Y1_NoCross = false;//Station1相机位置与Station2 轴系不接触
            //bool nowStatus_Y1_NoCross = false;

            bool lastStatus_Y2_NoCross = false;//Station1相机位置与Station2 轴系不接触
            bool nowStatus_Y2_NoCross = false;

            bool xLimit = false;

            bool lastStatus_Y2_Cross = false;//Station1相机位置与Station2 轴系 交叉接触
            bool nowStatus_Y2_Cross = false;

            bool lastStatus_X2_Pos = false;//Station1 X1位置与Station2 X2 之间保持125距离，正方向运动，超过会撞
            bool nowStatus_X2_Pos = false;

            bool lastStatus_X2_Neg = false;//Station1 X1位置与Station2 X2 之间保持180距离，负方向运动，超过会撞
            bool nowStatus_X2_Neg = false;

            bool lastStatus_X1_Pos = false;//Station1 X1位置与Station2 X2 之间保持125距离，正方向运动，超过会撞
            bool nowStatus_X1_Pos = false;

            bool lastStatus_X1_Neg = false;//Station1 X1位置与Station2 X2 之间保持180距离，负方向运动，超过会撞
            bool nowStatus_X1_Neg = false;

            try
            {
                float xLimitVal = 0;
                if ((_axisX1MaxStroke - CurPos[(byte)En_AxisNum.X1]) - CurPos[(byte)En_AxisNum.X2] < 0)
                {
                    //相机在吸嘴左边
                    xLimitVal = 180;
                }
                else
                { //相机在吸嘴右边
                    xLimitVal = 145;
                }
                // //这个地方是否判断相机在吸嘴左右测，然后再判断限制范围
                if (Math.Abs((_axisX1MaxStroke - CurPos[(byte)En_AxisNum.X1]) - CurPos[(byte)En_AxisNum.X2]) < xLimitVal)
                {
                    xLimit = true;
                }
                else
                    xLimit = false;

                //360 = 250 + 140 
                //2个Y处于交叉状态
                if ((390 - CurPos[(byte)En_AxisNum.Y1] < CurPos[(byte)En_AxisNum.Y2]) &&
                    (482 - CurPos[(byte)En_AxisNum.Y1] > CurPos[(byte)En_AxisNum.Y2]))
                {
                    if (Math.Abs((355 - CurPos[(byte)En_AxisNum.X1]) - CurPos[(byte)En_AxisNum.X2]) < xLimitVal)
                    {
                        if ((355 - CurPos[(byte)En_AxisNum.X1]) - CurPos[(byte)En_AxisNum.X2] > 0)
                        {
                            //相机在吸嘴右边
                            lastStatus_X2_Pos = true;
                            lastStatus_X1_Pos = true;

                            lastStatus_X2_Neg = false;
                            lastStatus_X1_Neg = false;
                        }
                        else
                        {
                            lastStatus_X2_Neg = true;
                            lastStatus_X1_Neg = true;
                            //相机在吸嘴右边
                            lastStatus_X2_Pos = false;
                            lastStatus_X1_Pos = false;
                        }
                    }
                    else
                    {
                        lastStatus_X2_Neg = false;
                        lastStatus_X1_Neg = false;

                        lastStatus_X2_Pos = false;
                        lastStatus_X1_Pos = false;
                    }
                }
                else
                {
                    //2个轴理论处于非交叉状态
                    //如果坐标过冲，超过475怎么办？？？
                    if ((CurPos[(byte)En_AxisNum.Y1] + CurPos[(byte)En_AxisNum.Y2]) > 484)
                    {
                        JogAxis(En_AxisNum.Y1, 1, false);
                        JogAxis(En_AxisNum.Y2, 1, false);
                    }
                    lastStatus_X2_Pos = false;
                    lastStatus_X2_Neg = false;
                    lastStatus_X1_Neg = false;
                    lastStatus_X1_Pos = false;
                }

                if (nowStatus_X2_Pos == false && lastStatus_X2_Pos == true)
                {
                    JogAxis(En_AxisNum.X2, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Warn, "Run->Stop_X2_Pos", En_Logout_Type.Other, true);
                    // JogAxis(En_AxisNum.X1, 1, false);
                }
                nowStatus_X2_Pos = lastStatus_X2_Pos;

                if (nowStatus_X2_Neg == false && lastStatus_X2_Neg == true)
                {
                    JogAxis(En_AxisNum.X2, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Warn, "Run->Stop_X2_Neg", En_Logout_Type.Other, true);
                    // JogAxis(En_AxisNum.X1, 1, false);
                }
                nowStatus_X2_Neg = lastStatus_X2_Neg;

                if (nowStatus_X1_Pos == false && lastStatus_X1_Pos == true)
                {
                    JogAxis(En_AxisNum.X1, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Run->Stop_X1_Pos", En_Logout_Type.Other, true);
                }
                nowStatus_X1_Pos = lastStatus_X1_Pos;

                if (nowStatus_X1_Neg == false && lastStatus_X1_Neg == true)
                {
                    JogAxis(En_AxisNum.X1, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Run->Stop_X1_Neg", En_Logout_Type.Other, true);
                }
                nowStatus_X1_Neg = lastStatus_X1_Neg;


                if ((390 - CurPos[(byte)En_AxisNum.Y1] < CurPos[(byte)En_AxisNum.Y2]))
                //||   CurPos[(byte)En_AxisNum.Y2] >= 475)
                {
                    lastStatus_Y2_NoCross = true;
                }
                else
                {
                    lastStatus_Y2_NoCross = false;
                }

                if ((481 - CurPos[(byte)En_AxisNum.Y1] < CurPos[(byte)En_AxisNum.Y2]) /*||*/
                 /* CurPos[(byte)En_AxisNum.Y2] >= 471*/)
                {
                    lastStatus_Y2_Cross = true;
                }
                else
                {
                    lastStatus_Y2_Cross = false;
                }

                if (nowStatus_Y2_NoCross == false && lastStatus_Y2_NoCross == true && xLimit == true)
                {
                    JogAxis(En_AxisNum.Y2, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Run->Stop_Y2", En_Logout_Type.Other, true);
                    JogAxis(En_AxisNum.Y1, 1, false);
                }
                if (nowStatus_Y2_Cross == false && lastStatus_Y2_Cross == true && xLimit == false)
                {
                    JogAxis(En_AxisNum.Y2, 1, false);
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "Run->Stop_Y2", En_Logout_Type.Other, true);
                    JogAxis(En_AxisNum.Y1, 1, false);
                }

                nowStatus_Y2_Cross = lastStatus_Y2_Cross;
                nowStatus_Y2_NoCross = lastStatus_Y2_NoCross;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace, En_Logout_Type.Exception);
            }
            return true;
        }
        #endregion

        #region 报警处理
        public bool CleanAlarm()
        {
            return qMotion.CleanAlarm();
        }
        #endregion

        #region StopAxis 停止轴系运动
        public bool StopAxisMove(En_AxisNum axisId)
        {
            bool ret = false;
            qMotion.Exit(true);
            ret = qMotion.StopAxisMove((byte)axisId);
            qMotion.Exit(false);
            return ret;
        }
        public bool StopAllAxisMove()
        {
            qMotion.Exit(true);
            if (!qMotion.StopMultiAxisMove(new float[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }))
            {
                qMotion.Exit(false);
                return false;
            }
            qMotion.Exit(false);
            return true;
        }
        public void ExitAxisMove(bool isExit = true)
        {
            qMotion.Exit(isExit);
        }
        public bool GetExitStatus()
        {
            return qMotion.GetExitStatus();
        }

        public bool Exit()
        {
            return qMotion.GetExitStatus();
        }

        public void SetExitSts(bool sts)
        {
            qMotion.Exit(sts);
        }

        #endregion

        #region Reset
        public bool SetResetSpeed(En_StationNo station)
        {
            //dSpeed[0] = fZeroSpeed[axisId - 1][0] * dEqut[axisId - 1];//搜索开关速度，单位：驱动器设置的用户速度单位
            //dSpeed[1] = fZeroSpeed[axisId - 1][1] * dEqut[axisId - 1];//搜索 index 标识速度，单位：驱动器设置的用户速度单位
            //dSpeed[2] = fZeroSpeed[axisId - 1][2] * dEqut[axisId - 1];//搜索加速度，单位：驱动器设置的用户加速度单位 
            float[,] spd = new float[9, 3] {
                    { 50f,15f,300f},
                    { 50f,15f,300f},
                    { 50f,15f,300f},
                    { 50f,15f,300f},
                    { 50f,15f,300f},
                    { 50f,30f,1000f},
                    { 50f,30f,1000f},
                    { 50f,30f,1000f},
                    { 50f,30f,1000f},
                };
            if (station == En_StationNo.StationNo1)
            {
                qMotion.SetZeroSpeed((short)En_AxisNum.X1, new float[3] { spd[(byte)En_AxisNum.X1, 0] * Pulseequ[(byte)En_AxisNum.X1],
                                                                          spd[(byte)En_AxisNum.X1, 1] * Pulseequ[(byte)En_AxisNum.X1],
                                                                          spd[(byte)En_AxisNum.X1, 2] * Pulseequ[(byte)En_AxisNum.X1] });
                qMotion.SetZeroSpeed((short)En_AxisNum.Y1, new float[3] { spd[(byte)En_AxisNum.Y1, 0] * Pulseequ[(byte)En_AxisNum.Y1],
                                                                          spd[(byte)En_AxisNum.Y1, 1] * Pulseequ[(byte)En_AxisNum.Y1],
                                                                          spd[(byte)En_AxisNum.Y1, 2] * Pulseequ[(byte)En_AxisNum.Y1] });
            }
            else
            {
                qMotion.SetZeroSpeed((short)En_AxisNum.X2, new float[3] { spd[(byte)En_AxisNum.X2, 0] * Pulseequ[(byte)En_AxisNum.X2],
                                                                          spd[(byte)En_AxisNum.X2, 1] * Pulseequ[(byte)En_AxisNum.X2],
                                                                          spd[(byte)En_AxisNum.X2, 2] * Pulseequ[(byte)En_AxisNum.X2] });
                qMotion.SetZeroSpeed((short)En_AxisNum.Y2, new float[3] { spd[(byte)En_AxisNum.Y2, 0] * Pulseequ[(byte)En_AxisNum.Y2],
                                                                          spd[(byte)En_AxisNum.Y2, 1] * Pulseequ[(byte)En_AxisNum.Y2],
                                                                          spd[(byte)En_AxisNum.Y2, 2] * Pulseequ[(byte)En_AxisNum.Y2] });
                qMotion.SetZeroSpeed((short)En_AxisNum.Z2, new float[3] { spd[(byte)En_AxisNum.Z2, 0] * Pulseequ[(byte)En_AxisNum.Z2],
                                                                          spd[(byte)En_AxisNum.Z2, 1] * Pulseequ[(byte)En_AxisNum.Z2],
                                                                          spd[(byte)En_AxisNum.Z2, 2] * Pulseequ[(byte)En_AxisNum.Z2] });
                qMotion.SetZeroSpeed((short)En_AxisNum.R1, new float[3] { spd[(byte)En_AxisNum.R1, 0] * Pulseequ[(byte)En_AxisNum.R1],
                                                                          spd[(byte)En_AxisNum.R1, 1] * Pulseequ[(byte)En_AxisNum.R1],
                                                                          spd[(byte)En_AxisNum.R1, 2] * Pulseequ[(byte)En_AxisNum.R1] });
                qMotion.SetZeroSpeed((short)En_AxisNum.R2, new float[3] { spd[(byte)En_AxisNum.R2, 0] * Pulseequ[(byte)En_AxisNum.R2],
                                                                          spd[(byte)En_AxisNum.R2, 1] * Pulseequ[(byte)En_AxisNum.R2],
                                                                          spd[(byte)En_AxisNum.R2, 2] * Pulseequ[(byte)En_AxisNum.R2] });
                qMotion.SetZeroSpeed((short)En_AxisNum.R3, new float[3] { spd[(byte)En_AxisNum.R3, 0] * Pulseequ[(byte)En_AxisNum.R3],
                                                                          spd[(byte)En_AxisNum.R3, 1] * Pulseequ[(byte)En_AxisNum.R3],
                                                                          spd[(byte)En_AxisNum.R3, 2] * Pulseequ[(byte)En_AxisNum.R3] });
                qMotion.SetZeroSpeed((short)En_AxisNum.R4, new float[3] { spd[(byte)En_AxisNum.R4, 0] * Pulseequ[(byte)En_AxisNum.R4],
                                                                          spd[(byte)En_AxisNum.R4, 1] * Pulseequ[(byte)En_AxisNum.R4],
                                                                          spd[(byte)En_AxisNum.R4, 2] * Pulseequ[(byte)En_AxisNum.R4] });

            }
            return true;
        }
        public bool ResetAxis(En_StationNo stationNo)
        {
            if (stationNo == En_StationNo.StationNo1)
            {
                //复位顺序必须是Y->X
                qMotion.ResetAxis((byte)En_AxisNum.Y1, false, false);

                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Y1))
                {
                    return false;
                }

                qMotion.ResetAxis((byte)En_AxisNum.X1, false, false);
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.X1))
                {
                    return false;
                }
            }
            else if (stationNo == En_StationNo.StationNo2)
            {
                //复位顺序必须是Z->Y->X
                qMotion.ResetAxis((byte)En_AxisNum.Z2, false, false);
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Z2))
                {
                    return false;
                }

                qMotion.ResetAxis((byte)En_AxisNum.Y2, false, false);
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Y2))
                {
                    return false;
                }

                qMotion.ResetAxis((byte)En_AxisNum.X2, false, false);
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.X2))
                {
                    return false;
                }

                qMotion.ResetAxis((byte)En_AxisNum.R1, false, false);
                qMotion.ResetAxis((byte)En_AxisNum.R2, false, false);
                qMotion.ResetAxis((byte)En_AxisNum.R3, false, false);
                qMotion.ResetAxis((byte)En_AxisNum.R4, false, false);

                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R4))
                {
                    return false;
                }
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R3))
                {
                    return false;
                }
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R2))
                {
                    return false;
                }
                if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R1))
                {
                    return false;
                }
            }
            return true;
        }
        public bool ResetAllAxis()
        {
            IsResetCompleted = false;
            qMotion.CleanAlarm();

            if (!WaitAxisMoveEnd() || Exit())
                return false;


            SetResetSpeed(En_StationNo.StationNo1);
            SetResetSpeed(En_StationNo.StationNo2);
            //注意这里复位是有轴顺序的，不能乱调整
            //复位顺序必须是Z->Y->X
            qMotion.ResetAxis((byte)En_AxisNum.Z2, false, false);
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Z2))
            {
                return false;
            }

            qMotion.ResetAxis((byte)En_AxisNum.Y1, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.Y2, false, false);

            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Y1))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.Y2))
            {
                return false;
            }

            qMotion.ResetAxis((byte)En_AxisNum.X1, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.X2, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.R1, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.R2, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.R3, false, false);
            qMotion.ResetAxis((byte)En_AxisNum.R4, false, false);

            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.X1))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.X2))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R4))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R3))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R2))
            {
                return false;
            }
            if (!qMotion.AxisWaitResetZero((byte)En_AxisNum.R1))
            {
                return false;
            }
            IsResetCompleted = true;
            return true;
        }
        public bool ResetAxis(En_AxisNum axisId, bool waitforend = true, bool buseflag = true)
        {
            return qMotion.ResetAxis((short)axisId, waitforend, buseflag);
        }
        #endregion

        #region IO设置
        /// <summary>
        /// 读输入状态
        /// </summary>
        /// <param name="iSlave">IO模块索引（0开始）</param>
        /// <param name="iStatus">IO状态</param>
        /// <returns></returns>
        public bool GetInput(ref int iStatus)
        {
            return qMotion.GetDIn(0, ref iStatus);
        }

        public bool GetOutput(ref int loData)
        {
            return qMotion.GetDOut(0, ref loData);
        }

        public bool GetSingleInput(EN_GoogolExtendInput input)
        {
            bool ret = false;
            if (Convert.ToInt16(input) < 16)
            {
                ret = ((EInput & (1 << Convert.ToInt16(input))) != 0) ? true : false;
            }
            return ret;
        }
        /// <summary>
        /// 读输出状态
        /// </summary>
        /// <param name="iSlave">IO模块索引（0开始）</param>g
        /// <param name="iStatus">IO状态</param>
        /// <returns></returns>
        public bool ReadOutPort()
        {
            int getstatus = 0;
            bool ret = qMotion.GetDOut(0, ref getstatus);
            EOutput = (ushort)getstatus;
            return ret;
        }

        /// <summary>
        /// 设置单个输出状态
        /// </summary>
        /// <param name="iSlave">IO模块索引（0开始）</param>
        /// <param name="iIndex">序号</param>
        /// <param name="IsOn">开关</param>
        /// <returns></returns>
        public bool WriteOutPort(byte iIndex, bool IsOn)
        {
            return qMotion.SetSingleDOut(0, iIndex, IsOn);
        }

        public bool WriteMultiOutPort(int lData)
        {
            return qMotion.SetDOut(0, lData);
        }

        public bool WriteMultiOutPort(int lData, bool status)
        {
            int curStatus = 0;
            if (!GetOutput(ref curStatus))
                return false;

            bool[] bitStatus = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                bitStatus[i] = (lData & ((ushort)(1 << i))) > 0 ? true : false;

                if (bitStatus[i])
                {
                    if (status)
                    {
                        if ((curStatus & (lData & (ushort)(1 << i))) <= 0)
                        {
                            curStatus = curStatus + (lData & (ushort)(1 << i));
                        }
                    }
                    else
                    {
                        if ((curStatus & (lData & (ushort)(1 << i))) > 0)
                        {
                            curStatus = curStatus - (lData & (ushort)(1 << i));
                        }
                    }
                }
            }
            return qMotion.SetDOut(0, curStatus);
        }

        public bool GetMultiAnalogInput(int iSlave, int startIndex, int count, ref short[] val)
        {
            return qMotion.GetAIn(iSlave, startIndex, count, ref val);
        }

        #endregion

        #region Speed
        public bool SetSpeed(En_AxisNum axis, float[] spd)
        {
            bool rs = false;
            try
            {
                float[] speedpluse = new float[3];
                for (int i = 0; i < spd.Length; i++)
                {
                    speedpluse[i] = spd[i] * Pulseequ[(byte)axis];
                }

                rs = qMotion.SetMoveSpeed((int)axis, speedpluse);
                NLogTrace.LogOut(rs ? EN_WARN_LEVEL.Info : EN_WARN_LEVEL.Error, $" Axis {axis.ToString()} Speed {NLogTrace.GetFloatArrayString(spd)} rs {rs.ToString()}");
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"SetCustomSingleSpeed  {ex.ToString()},{ex.StackTrace}");
            }
            return true;
        }

        public bool SetMoveSpeed(En_AxisNum axis, float speed)
        {
            try
            {
                bool rs = true;
                float[] speedParam = _motionGoogolParam.GetSpeed(axis, En_SpeedType.Work);
                speedParam[1] = speed;
                rs &= qMotion.SetMoveSpeed((int)axis, ConvertToPluse(axis, speedParam));
                return rs;
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, ex.Message + ex.StackTrace);
            }
            return false;
        }



        public bool SetSpeed(En_AxisNum axis, En_SpeedType speedtype)
        {
            bool rs = false;
            try
            {
                float[] speed = _motionGoogolParam.GetSpeed(axis, speedtype);
                float[] speedpluse = new float[3];
                for (int i = 0; i < speed.Length; i++)
                {
                    speedpluse[i] = speed[i] * Pulseequ[(byte)axis];
                }
                rs = qMotion.SetMoveSpeed((int)axis, speedpluse);
                NLogTrace.LogOut(rs ? EN_WARN_LEVEL.Info : EN_WARN_LEVEL.Error, $" Axis {axis.ToString()} Speed {NLogTrace.GetFloatArrayString(speed)} rs {rs.ToString()}");
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"SetSpeed {ex.ToString()},{ex.StackTrace}");
            }
            return rs;

        }
        public bool SetSpeedAll(En_SpeedType speedtype)
        {
            bool ret = true;
            try
            {
                var axisNum = Enum.GetNames(typeof(En_AxisNum));
                for (int i = 0; i < axisNum.Length; i++)
                {
                    if (!SetSpeed((En_AxisNum)Enum.Parse(typeof(En_AxisNum), axisNum[i]), speedtype))
                        ret = false;
                }
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"SetSpeedAll  {ex.ToString()},{ex.StackTrace}");
            }
            return ret;
        }

        #endregion

        #region 当前坐标
        public bool GetCurPos()
        {
            bool rs = true;
            double[] xyzrrpulse1 = new double[2];
            double[] xyzrrpulse2 = new double[7];
            if (!qMotion.GetEncPos((byte)En_AxisNum.X1, 2, ref xyzrrpulse1))
            {
                CurPos[0] = -999.999f;
                CurPos[1] = -999.999f;
                rs = false;
            }
            else
            {
                CurPos[(byte)En_AxisNum.X1] = (float)Math.Round(xyzrrpulse1[0] / Pulseequ[(byte)En_AxisNum.X1], 3);
                CurPos[(byte)En_AxisNum.Y1] = (float)Math.Round(xyzrrpulse1[1] / Pulseequ[(byte)En_AxisNum.Y1], 3);
            }

            if (!qMotion.GetEncPos((byte)En_AxisNum.X2, 7, ref xyzrrpulse2))
            {
                CurPos[2] = -999.999f;
                CurPos[3] = -999.999f;
                CurPos[4] = -999.999f;
                CurPos[5] = -999.999f;
                CurPos[6] = -999.999f;
                CurPos[7] = -999.999f;
                CurPos[8] = -999.999f;
                rs = false;
            }
            else
            {
                CurPos[(byte)En_AxisNum.X2] = (float)Math.Round(xyzrrpulse2[0] / Pulseequ[(byte)En_AxisNum.X2], 3);
                CurPos[(byte)En_AxisNum.Y2] = (float)Math.Round(xyzrrpulse2[1] / Pulseequ[(byte)En_AxisNum.Y2], 3);
                CurPos[(byte)En_AxisNum.Z2] = (float)Math.Round(xyzrrpulse2[2] / Pulseequ[(byte)En_AxisNum.Z2], 3);
                CurPos[(byte)En_AxisNum.R1] = (float)Math.Round(xyzrrpulse2[3] / Pulseequ[(byte)En_AxisNum.R1], 3);
                CurPos[(byte)En_AxisNum.R2] = (float)Math.Round(xyzrrpulse2[4] / Pulseequ[(byte)En_AxisNum.R2], 3);
                CurPos[(byte)En_AxisNum.R3] = (float)Math.Round(xyzrrpulse2[5] / Pulseequ[(byte)En_AxisNum.R3], 3);
                CurPos[(byte)En_AxisNum.R4] = (float)Math.Round(xyzrrpulse2[6] / Pulseequ[(byte)En_AxisNum.R4], 3);
            }
            IsConnected = rs;
            return rs;
        }
        public bool GetCurPos(En_StationNo stationNo)
        {
            bool rs = true;
            if (stationNo == En_StationNo.StationNo1)
            {
                double[] xyzrrpulse1 = new double[2];
                if (!qMotion.GetEncPos((byte)En_AxisNum.X1, 2, ref xyzrrpulse1))
                {
                    CurPos[(byte)En_AxisNum.X1] = -999.999f;
                    CurPos[(byte)En_AxisNum.Y1] = -999.999f;
                    rs = false;
                }
                else
                {
                    CurPos[(byte)En_AxisNum.X1] = (float)Math.Round(xyzrrpulse1[0] / Pulseequ[(byte)En_AxisNum.X1], 3);
                    CurPos[(byte)En_AxisNum.Y1] = (float)Math.Round(xyzrrpulse1[1] / Pulseequ[(byte)En_AxisNum.Y1], 3);
                }
            }
            else if (stationNo == En_StationNo.StationNo2)
            {
                double[] xyzrrpulse2 = new double[7];

                if (!qMotion.GetEncPos((byte)En_AxisNum.X2, 7, ref xyzrrpulse2))
                {
                    CurPos[(byte)En_AxisNum.X2] = -999.999f;
                    CurPos[(byte)En_AxisNum.Y2] = -999.999f;
                    CurPos[(byte)En_AxisNum.Z2] = -999.999f;
                    CurPos[(byte)En_AxisNum.R1] = -999.999f;
                    CurPos[(byte)En_AxisNum.R2] = -999.999f;
                    CurPos[(byte)En_AxisNum.R3] = -999.999f;
                    CurPos[(byte)En_AxisNum.R4] = -999.999f;
                    rs = false;
                }
                else
                {
                    CurPos[(byte)En_AxisNum.X2] = (float)Math.Round(xyzrrpulse2[0] / Pulseequ[(byte)En_AxisNum.X2]);
                    CurPos[(byte)En_AxisNum.Y2] = (float)Math.Round(xyzrrpulse2[1] / Pulseequ[(byte)En_AxisNum.Y2]);
                    CurPos[(byte)En_AxisNum.Z2] = (float)Math.Round(xyzrrpulse2[2] / Pulseequ[(byte)En_AxisNum.Z2]);
                    CurPos[(byte)En_AxisNum.R1] = (float)Math.Round(xyzrrpulse2[3] / Pulseequ[(byte)En_AxisNum.R1], 3);
                    CurPos[(byte)En_AxisNum.R2] = (float)Math.Round(xyzrrpulse2[4] / Pulseequ[(byte)En_AxisNum.R2], 3);
                    CurPos[(byte)En_AxisNum.R3] = (float)Math.Round(xyzrrpulse2[5] / Pulseequ[(byte)En_AxisNum.R3], 3);
                    CurPos[(byte)En_AxisNum.R4] = (float)Math.Round(xyzrrpulse2[6] / Pulseequ[(byte)En_AxisNum.R4], 3);
                }
            }
            return rs;
        }
        public float GetAxisCurPos(En_AxisNum axis)
        {
            double[] pluse = new double[1];
            qMotion.GetPrfPos((int)axis, 1, ref pluse);
            return (float)(pluse[0] / _motionGoogolParam.PluseEquivalents[(int)axis]);
        }
        #endregion

        #region Tape站

        /// <summary>
        /// 单轴移动到目标位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="pos"></param>
        /// <param name="waitforend"></param>
        /// <returns></returns>
        public bool MoveSingleAxis(En_AxisNum axis, float pos, bool waitforend)
        {
            try
            {
                float pluse = ConvertToPluse(axis, pos);
                bool rtn = qMotion.MoveAbsoluteToPointSingleAxis((short)axis, pluse, false, false);

                if (!waitforend)
                    return rtn;

                return WaitMoveAxisEnd(axis);
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Info, ex.Message + ex.StackTrace);
            }
            return false;
        }

        public bool AxisWaitResetZero(short asid, int timeout = 40000)
        {
            ushort val = 0;
            int _timeout = timeout;
            while (timeout > 0)
            {
                timeout--;
                qMotion.EcatGetHomingStatus(asid, ref val);
                Thread.Sleep(10);
                if (val == 3)
                    break;
            }
            qMotion.EcatSetHomingMode(asid, 8);

            if (val != 3)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 等待轴点位移动结束
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool WaitMoveAxisEnd(En_AxisNum axis)
        {
            return WaitMoveAxisEnd(new En_AxisNum[] { axis });
        }

        /// <summary>
        /// 等待轴点位移动结束
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool WaitMoveAxisEnd(En_AxisNum[] axis)
        {
            DateTime starttime = DateTime.Now;
            PLC_Component _plc_Component = (PLC_Component)IoC.Get<IPLC>();

            while (true)
            {
                if ((DateTime.Now - starttime).TotalSeconds > 60)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "等待轴移动结束超过60s");
                    break;
                }

                if (_plc_Component.IsReset() || _plc_Component.IsEStop())
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, "等待轴移动结束时机台Reset/EStop");
                    break;
                }

                /*PLC暂停 ????? */

                int[] sts = new int[axis.Length];
                for (int i = 0; i < axis.Length; i++)
                {
                    if (IsDriverAlarm(axis[i]) ||
                        IsAxisPosiLimitAlarm(axis[i]) ||
                        IsAxisNaviLimitAlarm(axis[i]) ||
                        !IsAxisEnable(axis[i]))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, "等待轴移动结束期间，轴异常");
                        return false;
                    }

                    if (!qMotion.AxisGetSts((short)axis[i], ref sts, 1))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, "轴移动间获取轴状态失败");
                        return false;
                    }
                }
                bool isEnd = true;
                for (int i = 0; i < axis.Length; i++)
                {
                    isEnd &= (sts[i] & 0x01 << 10) == 0x00;
                }

                if (isEnd)
                {
                    if (_plc_Component.IsStop()) //PLC暂停状态，本次移动结束后就停这不动
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    if (_plc_Component.IsReset() || _plc_Component.IsEStop())
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Info, "轴移动结束后机台Reset/EStop");
                        return false;
                    }
                    return true;
                }

                Thread.Sleep(10);
            }
            return false;
        }

        /// <summary>
        /// 判断是否驱动器报警
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsDriverAlarm(En_AxisNum axis)
        {
            return (AxisSts[(int)axis] & 0x01 << 1) != 0x00;
        }

        /// <summary>
        /// 判断轴是否正限位报警
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsAxisPosiLimitAlarm(En_AxisNum axis)
        {
            return (AxisSts[(int)axis] & 0x01 << 5) != 0x00;
        }
        /// <summary>
        /// 判断轴是否负限位报警
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsAxisNaviLimitAlarm(En_AxisNum axis)
        {
            return (AxisSts[(int)axis] & 0x01 << 6) != 0x00;
        }
        /// <summary>
        /// 判断轴是否上使能
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsAxisEnable(En_AxisNum axis)
        {
            return (AxisSts[(int)axis] & 0x01 << 9) != 0x00;
        }

        public bool WaitAxisMoveEnd()
        {
            DateTime starttime = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - starttime).TotalSeconds > 30)
                {
                    Console.WriteLine("运动超过20s");
                    break;
                }
                qMotion.CleanAlarm();
                if (GetAxisClrSts(En_GetAxisClrSts.Bit10_MoveEnabled))
                {// false 说明在运动 ，true代表静止
                    return true;
                }
                //int[] sts = new int[8];              
                //qMotion.AxisGetSts(0, ref sts, 8);

                //int[] singlests = new int[1];
                //qMotion.AxisGetSts(8, ref singlests, 1);

                //Console.WriteLine($"Axis Status:{sts[0].ToString()}");
                //if (((sts[0] & 0x01 << 10) == 0x00) &&
                //    ((sts[1] & 0x01 << 10) == 0x00) &&
                //    ((sts[2] & 0x01 << 10) == 0x00) &&
                //    ((sts[3] & 0x01 << 10) == 0x00) &&
                //    ((sts[4] & 0x01 << 10) == 0x00) &&
                //    ((sts[5] & 0x01 << 10) == 0x00) &&
                //    ((sts[6] & 0x01 << 10) == 0x00) &&
                //    ((sts[7] & 0x01 << 10) == 0x00) &&
                //    ((singlests[0] & 0x01 << 10) == 0x00))
                //{
                //    return true;
                //}
            }
            return false;
        }


        public bool MoveAbsoluteSingleAxis(En_AxisNum axis, float pos, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            return MoveToPointAxes(new En_AxisNum[] { axis }, new float[] { pos }, waitforend, buseflag, true, checkPLC);
        }
        public bool MoveAbsoluteXY(En_StationNo station, float[] xy, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo1)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X1, En_AxisNum.Y1 }, new float[] { xy[0], xy[1] }, waitforend, buseflag, true, checkPLC);
            }
            else if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2 }, new float[] { xy[0], xy[1] }, waitforend, buseflag, true, checkPLC);
            }
            else
            {
                return false;
            }
        }
        public bool MoveAbsoluteX2Y2R(En_StationNo station, En_AxisNum axisR, float[] xyr, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2, axisR }, new float[] { xyr[0], xyr[1], xyr[2] }, waitforend, buseflag, true, checkPLC);
            }
            else
            {
                return false;
            }
        }
        public bool MoveAbsoluteX2Y2Z2(En_StationNo station, float[] xyz, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo1)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X1, En_AxisNum.Y1 }, new float[] { xyz[0], xyz[1] }, waitforend, buseflag, true, checkPLC);
            }
            else if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2, En_AxisNum.Z2 }, new float[] { xyz[0], xyz[1], xyz[2] }, waitforend, buseflag, true, checkPLC);
            }
            else
            {
                return false;
            }
        }
        public bool MoveAbsoluteX2Y2Z2R(En_StationNo station, En_AxisNum axisR, float[] xyzr, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2, En_AxisNum.Z2, axisR }, new float[] { xyzr[0], xyzr[1], xyzr[2], xyzr[3] }, waitforend, buseflag, true, checkPLC);
            }
            else
            {
                return false;
            }
        }
        public bool MoveRelativeSingleAxis(En_AxisNum axis, float dis, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            return MoveToPointAxes(new En_AxisNum[] { axis }, new float[] { dis }, waitforend, buseflag, false, checkPLC);
        }
        public bool MoveRelativeXY(En_StationNo station, float[] xy, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo1)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X1, En_AxisNum.Y1 }, new float[] { xy[0], xy[1] }, waitforend, buseflag, false, checkPLC);
            }
            else if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2 }, new float[] { xy[0], xy[1] }, waitforend, buseflag, false, checkPLC);
            }
            else
            {
                return false;
            }
        }
        public bool MoveRelativeX2Y2Z2(En_StationNo station, float[] xyz, bool waitforend, bool buseflag = true, bool checkPLC = true)
        {
            if (station == En_StationNo.StationNo2)
            {
                return MoveToPointAxes(new En_AxisNum[] { En_AxisNum.X2, En_AxisNum.Y2, En_AxisNum.Z2 }, new float[] { xyz[0], xyz[1], xyz[2] }, waitforend, buseflag, false, checkPLC);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 控制轴移动。
        /// </summary>
        /// <param name="axes"></param>
        /// <param name="pos"></param>
        /// <param name="waitforend"></param>
        /// <param name="buseflag"></param>
        /// <param name="absolute"></param>
        /// <param name="checkPLC">轴运动完成后是否检测plc的暂停状态</param>
        /// <returns></returns>
        private bool MoveToPointAxes(En_AxisNum[] axes, float[] pos, bool waitforend, bool buseflag, bool absolute, bool checkPLC = true)
        {
            if (!qMotion.CleanAlarm())
            {
                return false;
            }
            if (axes.Length != pos.Length)
            {
                return false;
            }
            float[] pluse = new float[axes.Length];
            for (int i = 0; i < axes.Length; i++)
            {
                pluse[i] = pos[i] * Pulseequ[(byte)axes[i]];
                if (absolute)
                {
                    if (!qMotion.MoveAbsoluteToPointSingleAxis((short)axes[i], pluse[i], false, buseflag))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!qMotion.MoveRelativeToPointSingleAxis((short)axes[i], pluse[i], false, buseflag))
                    {
                        return false;
                    }
                }
            }
            if (waitforend == false)
            {
                return true;
            }
            PLC_Component _plc_Component = (PLC_Component)IoC.Get<IPLC>();
            for (int i = 0; i < axes.Length; i++)
            {
                if (absolute)
                {
                    if (!qMotion.MoveAbsoluteToPointSingleAxis((short)axes[i], pluse[i], true, buseflag))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!qMotion.MoveRelativeToPointSingleAxis((short)axes[i], pluse[i], true, buseflag))
                    {
                        return false;
                    }
                }
            }
            if (_plc_Component.IsEStop() || _plc_Component.IsReset())
            {
                return false;
            }
            if (checkPLC)
            {
                if (_plc_Component.IsStop())
                {
                    while (_plc_Component.IsStop())
                    {
                        Thread.Sleep(200);
                        if (_plc_Component.IsEStop() || _plc_Component.IsReset())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool SetJogSpeed(En_AxisNum axisId, float[] speed, bool dir = true)
        {
            float[] speedpluse = new float[3] { 0, 0, 0 };
            for (int i = 0; i < speed.Length; i++)
            {
                speedpluse[i] = speed[i] * Pulseequ[(byte)axisId];
            }
            if (dir == false) speedpluse[1] = -1 * speed[1] * Pulseequ[(byte)axisId];
            return qMotion.SetJogSpeed((int)axisId, speedpluse);
        }

        public bool SetJogSpeed(En_AxisNum axis, En_SpeedType speedtype, bool dir = true)
        {
            bool rs = false;
            try
            {
                float[] speedpluse = new float[3] { 0, 0, 0 };
                float[] speed = _motionGoogolParam.GetSpeed(axis, speedtype);

                for (int i = 0; i < speed.Length; i++)
                {
                    speedpluse[i] = speed[i] * Pulseequ[(byte)axis];
                }

                if (dir == false) speedpluse[1] = -1 * speed[1] * Pulseequ[(byte)axis];

                rs = qMotion.SetJogSpeed((int)axis, speedpluse);
                NLogTrace.LogOut(rs ? EN_WARN_LEVEL.Info : EN_WARN_LEVEL.Error, $" Axis {axis.ToString()} Speed {NLogTrace.GetFloatArrayString(speed)} rs {rs.ToString()}");
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"SetSpeed {ex.ToString()},{ex.StackTrace}");
            }
            return rs;

        }
        public bool SetJogSpeedAll(En_SpeedType speedtype)
        {
            bool ret = true;
            try
            {
                var axisNum = Enum.GetNames(typeof(En_AxisNum));
                for (int i = 0; i < axisNum.Length; i++)
                {
                    if (!SetJogSpeed((En_AxisNum)Enum.Parse(typeof(En_AxisNum), axisNum[i]), speedtype))
                        ret = false;
                }
            }
            catch (Exception ex)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"SetSpeedAll  {ex.ToString()},{ex.StackTrace}");
            }
            return ret;
        }
        public bool JogAxis(En_AxisNum axisId, sbyte dir, bool isRun)
        {
            return qMotion.JogAxis((int)axisId, dir, isRun);
        }

        public bool ZeroPos(En_AxisNum axisId, int count = 1)
        {
            return qMotion.ZeroPos((int)axisId, count);
        }

        public bool ZeroPos(En_StationNo station)
        {
            if (station == En_StationNo.StationNo1)
            {
                return ZeroPos(En_AxisNum.X1, 2);
            }
            else
            {
                return ZeroPos(En_AxisNum.X2, 7);
            }
        }

        public string GetCurInfo()
        {
            StringBuilder sbinfo = new StringBuilder();
            sbinfo.Append(" X1: ");
            sbinfo.Append(CurPos[0].ToString("f2"));
            sbinfo.Append(" mm Y1: ");
            sbinfo.Append(CurPos[1].ToString("f2"));
            sbinfo.Append(" mm X2: ");
            sbinfo.Append(CurPos[2].ToString("f1"));
            sbinfo.Append(" mm Y2: ");
            sbinfo.Append(CurPos[3].ToString("f1"));
            sbinfo.Append(" mm Z2: ");
            sbinfo.Append(CurPos[4].ToString("f1"));
            sbinfo.Append(" mm R1: ");
            sbinfo.Append(CurPos[5].ToString("f2"));
            sbinfo.Append(" mm R2: ");
            sbinfo.Append(CurPos[6].ToString("f2"));
            sbinfo.Append(" mm R3: ");
            sbinfo.Append(CurPos[7].ToString("f2"));
            sbinfo.Append(" mm R4: ");
            sbinfo.Append(CurPos[8].ToString("f2"));
            return sbinfo.ToString();
        }
        #endregion

        #region NoUse
        private void CurRobotStatusInfoEvent()
        {
            _curRobotStatusInfo.X1axis = CurPos[(byte)En_AxisNum.X1];
            _curRobotStatusInfo.Y1axis = CurPos[(byte)En_AxisNum.Y1];
            _curRobotStatusInfo.X2axis = CurPos[(byte)En_AxisNum.X2];
            _curRobotStatusInfo.Y2axis = CurPos[(byte)En_AxisNum.Y2];
            _curRobotStatusInfo.Z2axis = CurPos[(byte)En_AxisNum.Z2];
            _curRobotStatusInfo.R1axis = CurPos[(byte)En_AxisNum.R1];
            _curRobotStatusInfo.R2axis = CurPos[(byte)En_AxisNum.R2];
            _curRobotStatusInfo.R3axis = CurPos[(byte)En_AxisNum.R3];
            _curRobotStatusInfo.R4axis = CurPos[(byte)En_AxisNum.R4];
            _curRobotStatusInfo.Einput = EInput;
            _curRobotStatusInfo.EOutput = EOutput;
            RobotValueRefresh?.Invoke(_curRobotStatusInfo);
            //_eventAggregator.Publish(_curRobotStatusInfo, action => { Task.Run(action); });
        }
        #endregion
    }
}
