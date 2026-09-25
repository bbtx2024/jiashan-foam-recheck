/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-14
 * 说明：（PLC通讯逻辑）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CSSIModbus;
using QA.Business.Component.Camera;
using QA.Business.Converts;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA.Business.Model.Alarm;
using QA_Infrastructure;
using QA_Infrastructure.BaseCtrls;
using QA_Infrastructure.NLogOut;


namespace QA.Business.Component.PLC
{
    public class PlcTrigInfo
    {
        public bool IsStart { get; set; } = false;
        public bool IsReset { get; set; } = false;
        public bool IsEStop { get; set; } = false;
        public bool IsStop { get; set; } = false;
    }

    public class PLC_Component : IPLC
    {
        #region Field
        public const int EVERY_ADDR_OFFSET = 1000;
        private PLCParam _plcParam = new PLCParam();
        private volatile CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private IEventAggregator _eventAggregator;
        private int _clientId;
        private ushort[] _readValue = new ushort[256];
        private ushort[] _writeValue = new ushort[256];
        private Camera_Component _camera_Component;
        #endregion

        #region Property
        public bool IsCurReset { get; private set; }
        public bool IsCurStart { get; private set; }
        public bool IsCurEStop { get; private set; }
        public bool IsCurStop { get; private set; }


        public bool IsCurSimulateRun { get; private set; }

        public string ComponentName { get; set; }
        public bool IsConnected { get; set; }
        public IParam Param { get; set; }

        public ushort[] ReadUshorts
        {
            get { return _readValue; }
            set { _readValue = value; }
        }

        public ushort[] WriteUshorts
        {
            get { return _writeValue; }
            set { _writeValue = value; }
        }
        public event Action<ushort[]> ReadValueRefresh;
        //public event Action<ushort[]> WriteValueRefresh;      
        #endregion

        #region Constructor
        public PLC_Component()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _plcParam = IoC.Get<PLCParam>();
            _camera_Component = (Camera_Component)IoC.Get<Interfaces.ICamera>();
        }
        #endregion

        #region Method

        #region base

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Initial(IParam param)
        {
            Param = _plcParam = param as PLCParam;
            _readValue = new ushort[_plcParam.FromPLC_AddrNum];//单次读取20ms左右
            _writeValue = new ushort[_plcParam.ToPLC_AddrNum];
            return true;
        }

        public bool Connect()
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }
                    return CSSIModbusRTUInterface.CSSIModbusRTU_InitSerialPort(_plcParam.ComPort);
                }
                _clientId = CSSIModbusTCPInterface.CSSIModbusTCP_InitTcpClient(_plcParam.IP, _plcParam.Port, 500/*_plcParam.Timeout*/);
                return _clientId != -1;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }

        public bool DisConnect()
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }
                    CSSIModbusRTUInterface.CSSIModbusRTU_Exit();
                    return true;
                }
                CSSIModbusTCPInterface.CSSIModbusTCP_Exit();
                return true;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            //避免偶发的只存在一瞬间急停的情况直接被判定为急停，必须连续两次循环结果都为急停才算急停
            bool estopFlag = false;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var delayTime = _plcParam.BUse ? 10 : 1000;
                    await Task.Delay(delayTime, _cancellationToken);
                    try
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        if (!_plcParam.BUse)
                        {
                            continue;
                        }
                        if (!IsConnected)
                        {
                            IsConnected = Connect();
                        }
                        if (!GetAddrMutiValue(_plcParam.StationID, _plcParam.FromPLC_StartAddr, _plcParam.FromPLC_AddrNum, ref _readValue))
                        {
                            IsConnected = false;
                        }
                        else
                        {
                            OnReadValueRefresh(_readValue);
                        }
                        #region 监控是否是空跑
                        ushort value = 0;
                        if (!GetAddrValue(_plcParam.StationID, _plcParam.FromPLC_SimulateRunSignal, ref value))
                        {
                            IsConnected = false;
                        }
                        else
                        {
                            if (value == 1)
                            {
                                IsCurSimulateRun = true;
                            }
                            else
                            {
                                if (!GetAddrValue(_plcParam.StationID, _plcParam.FromPLC_SimulateRunSignal2, ref value))
                                {
                                    IsConnected = false;
                                }
                                else
                                {
                                    if (value == 1)
                                    {
                                        IsCurSimulateRun = true;
                                    }
                                    else
                                    {
                                        IsCurSimulateRun = false;
                                    }
                                }
                            }
                        }
                        #endregion
                        //连接PLC后根据参数写拍全图模式
                        int model = _camera_Component.IsGetAllPhoto() == true ? 2 : 1;
                        SetSaveAllPhotoModel((ushort)model);
                        if (IsEStop())
                        {
                            if (!IsCurEStop)
                            {
                                if (!estopFlag)
                                {
                                    estopFlag = true;
                                    continue;
                                }
                                _eventAggregator.Publish(new PlcTrigInfo() { IsEStop = true }, action => { Task.Run(action); });
                            }
                            IsCurEStop = true;
                            IsCurReset = false;
                            IsCurStop = false;
                            IsCurStart = false;
                            continue;
                        }
                        else
                        {
                            IsCurEStop = false;
                            estopFlag = false;
                        }
                        if (IsReset())
                        {
                            if (!IsCurReset)
                            {
                                _eventAggregator.Publish(new PlcTrigInfo() { IsReset = true }, action => { Task.Run(action); });
                            }
                            IsCurReset = true;
                            IsCurStop = false;
                            IsCurStart = false;
                            continue;
                        }
                        else
                        {
                            IsCurReset = false;
                        }
                        if (IsStop())
                        {
                            if (!IsCurStop)
                            {
                                _eventAggregator.Publish(new PlcTrigInfo() { IsStop = true }, action => { Task.Run(action); });
                            }
                            IsCurStop = true;
                            IsCurStart = false;
                            continue;
                        }
                        else
                        {
                            IsCurStop = false;
                        }
                        if (IsStart())
                        {
                            if (!IsCurStart)
                            {
                                _eventAggregator.Publish(new PlcTrigInfo() { IsStart = true }, action => { Task.Run(action); });
                            }
                            IsCurStart = true;
                            continue;
                        }
                        else
                        {
                            IsCurStart = false;
                        }
                    }
                    catch (Exception e)
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, e.ToString(), En_Logout_Type.Exception);
                    }

                }
            }, _cancellationToken);
            return true;
        }

        protected virtual void OnReadValueRefresh(ushort[] obj)
        {
            ReadValueRefresh?.Invoke(obj);
        }

        public bool Stop()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                if (!_plcParam.BUseModbusTcp)
                {
                    CSSIModbus.CSSIModbusRTUInterface.CSSIModbusRTU_Exit();
                }
                else
                {
                    CSSIModbus.CSSIModbusTCPInterface.CSSIModbusTCP_Exit();
                }
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
            }
            return true;
        }

        private static readonly Dictionary<int, string[]> errorInfo = new Dictionary<int, string[]>()
        {
            { 4000, new [] {
                "撕膜阻挡气缸原点异常",
                "撕膜阻挡气缸动点异常",
                "撕膜顶升气缸原点异常",
                "撕膜顶升气缸动点异常",
                "撕膜伸缩气缸原点异常",
                "撕膜伸缩气缸动点异常",
                "撕膜上下气缸原点异常",
                "撕膜上下气缸动点异常",

            }},
            { 4001, new [] {
               "复检阻挡气缸原点异常  ",
               "复检阻挡气缸动点异常  ",
               "复检顶升1气缸原点异常 ",
               "复检顶升1气缸动点异常 ",
               "复检顶升2气缸原点异常 ",
               "复检顶升2气缸动点异常 ",
               "NG料仓顶升气缸原点异常",
               "NG料仓顶升气缸动点异常",

            }},
            { 4002, new [] {
              "撕膜1真空吹异常",
              "撕膜1真空吸异常",
              "撕膜2真空吹异常",
              "撕膜2真空吸异常",
              "撕膜3真空吹异常",
              "撕膜3真空吸异常",
              "备用           ",
              "备用           ",

            }},
            { 4003, new [] {
                "撕膜Z轴异常报警    ",
                "复检Z轴异常报警    ",
                "复检Y轴异常报警    ",
                "复检X轴异常报警    ",
                "撕膜Y轴异常报警    ",
                "撕膜X轴异常报警    ",
                "正流流线轴1异常报警",
                "正流流线轴2异常报警",

            }},
            { 4004, new [] {
                "复检顶升流线轴1异常报警",
                "复检顶升流线轴2异常报警",
                "NG流线轴1异常报警      ",
                "NG流线轴2异常报警      ",
                "回流流线轴异常报警     ",
                "流线调宽轴1异常报警    ",
                "流线调宽轴2异常报警    ",
                "备用                   ",

            }},
            { 4005, new [] {
                "设备急停中      ",
                "设备气压异常报警",
                "安全门异常报警  ",
                "备用            ",
                "备用            ",
                "备用            ",
                "备用            ",
                "备用            ",

            }},
            { 4006, new [] {
               "PC失联            ",
               "PC端报警          ",
               "撕膜位检测有料异常",
               "撕膜位检测连料异常",
               "撕膜位载具顶升异常",
               "撕膜气缸不在初始位",
               "撕膜吸嘴有料异常  ",
               "撕膜吸真空异常    ",

            }},
            { 4007, new [] {
                "撕膜位扫码回复超时",
                "复检位检测有料异常",
                "复检位载具顶升异常",
                "复检位扫码回复异常",
                "复检位拍照回复异常",
                "NG料仓检测有料异常",
                "复检位相机拍照结果反馈超时",
                "撕膜吸嘴1使用次数已到",

            }},
            { 4008, new [] {
                "撕膜吸嘴2使用次数已到",
                "撕膜吸嘴3使用次数已到",
                "备用                 ",
                "备用                 ",
                "备用                 ",
                "备用                 ",
                "备用                 ",
                "备用                 ",
            }},
        };
        public void GetCurAlarms(ref ObservableCollection<AlarmInfoModel> alarmInfos)
        {
            if (_plcParam.BUse)
            {
                if (!IsConnected)
                {
                    var count = alarmInfos.Count;
                    alarmInfos.Add(new AlarmInfoModel()
                    {
                        AlarmLevel = EN_WARN_LEVEL.Error,
                        AlarmModule = EN_WarnModules.Plc,
                        AlarmMsg = "无连接",
                        Datetime = DateTime.Now,
                        ErrorCode = 0,
                        Index = count + 1
                    });
                }
                ushort valueAlarm;
                foreach (var p in errorInfo)
                {
                    valueAlarm = 0;
                    if (GetAddrValue(0, (ushort)p.Key, ref valueAlarm))
                    {
                        for (byte bit = 0; bit < p.Value.Length; bit++)
                        {
                            if (GetUshortOneBitStatus(valueAlarm, bit) && !string.IsNullOrEmpty(p.Value[bit]))
                            {
                                var count = alarmInfos.Count;
                                alarmInfos.Add(new AlarmInfoModel()
                                {
                                    AlarmLevel = EN_WARN_LEVEL.Error,
                                    AlarmModule = EN_WarnModules.Plc,
                                    AlarmMsg = p.Value[bit],
                                    Datetime = DateTime.Now,
                                    ErrorCode = p.Key * 16 + bit,
                                    Index = count + 1
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断PLC是否启动
        /// </summary>
        /// <param name="plcidx"></param>
        /// <returns></returns>
        public bool IsStart()
        {
            return _readValue[_plcParam.FromPLC_MachineStatus - _plcParam.FromPLC_StartAddr] == 1;
            //return (_readValue[_plcParam.FromPLC_RunStatus - _plcParam.FromPLC_StartAddr] & (1 << 1)) > 0 ? true : false;
        }

        /// <summary>
        /// 判断PLC是否复位
        /// </summary>
        /// <param name="plcidx"></param>
        /// <returns></returns>
        public bool IsReset()
        {
            return _readValue[_plcParam.FromPLC_MachineStatus - _plcParam.FromPLC_StartAddr] == 3;
            // return (_readValue[_plcParam.FromPLC_RunStatus - _plcParam.FromPLC_StartAddr] & (1 << 2)) > 0 ? true : false;
        }

        /// <summary>
        /// 判断PLC是否停止（按停止按钮或飞达光栅触发）
        /// </summary>
        /// <returns></returns>
        public bool IsStop()
        {
            return _readValue[_plcParam.FromPLC_MachineStatus - _plcParam.FromPLC_StartAddr] == 4;
            //return (_readValue[_plcParam.FromPLC_RunStatus - _plcParam.FromPLC_StartAddr] & (1 << 3)) > 0 ? true : false;
        }

        /// <summary>
        /// 判断PLC是否急停
        /// </summary>
        /// <returns></returns>
        public bool IsEStop()
        {
            return _readValue[_plcParam.FromPLC_MachineStatus - _plcParam.FromPLC_StartAddr] == 2;
            // return (_readValue[_plcParam.FromPLC_RunStatus - _plcParam.FromPLC_StartAddr] & (1 << 4)) > 0 ? false : true;
        }

        #endregion

        #region FA06-005-D

        /// <summary>
        /// 检测安全门是否报警
        /// </summary>
        /// <returns></returns>
        public bool IsSafeDoorAlarm()
        {
            return false;
            //return _readValue[_plcParam.FromPLC_SafeDoorAlarmAddr - _plcParam.FromPLC_StartAddr] == 1;
        }

        /// <summary>
        /// 判断PLC是否为空跑模式
        /// </summary>
        /// <returns></returns>
        public bool IsSimulateRun()
        {
            return IsCurSimulateRun;
        }

        /// <summary>
        /// 判断PLC是否撕膜扫码到位
        /// </summary>
        /// <param name="stationindex"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool IsScanReady()
        {
            return _readValue[_plcParam.FromPLC_TriggerScanner - _plcParam.FromPLC_StartAddr] == 1;
        }
        /// <summary>
        /// 判断PLC是否复检扫码到位
        /// </summary>
        /// <param name="stationindex"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool IsScanReadyRecheck()
        {
            return _readValue[_plcParam.FromPLC_TriggerScannerRecheck - _plcParam.FromPLC_StartAddr] == 1;
        }
        /// <summary>
        /// 判断PLC是否要视觉结果
        /// </summary>
        /// <param name="plcidx"></param>
        /// <returns></returns>
        public bool IsPlcCameraReadyResult()
        {
            return _readValue[_plcParam.FromPLC_TriggerCameraResul - _plcParam.FromPLC_StartAddr] == 1;
        }

        /// <summary>
        /// 判断PLC是否相机到位
        /// </summary>
        /// <param name="plcidx"></param>
        /// <returns></returns>
        public bool IsPlcCameraReady()
        {
            ushort value = 0;
            if (!GetAddrValue(_plcParam.StationID, _plcParam.FromPLC_TriggerCameraTwice, ref value))
            {
                return false;
            }
            return value == 1;
        }

        /// <summary>
        /// 判断PLC当前处理穴位索引（穴位号减1）
        /// </summary>
        /// <param name="plcidx"></param>
        /// <returns></returns>
        public int GetPlcIdxNow()
        {
            ushort value = 999;
            GetAddrValue(_plcParam.StationID, _plcParam.FromPLC_IdxNow, ref value);
            return value;
        }







        /// <summary>
        /// 设置撕膜扫码结果
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        public bool SetScanResult(EN_Result value)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置撕膜扫码结果时连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_ScannerResultAddr, (ushort)value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置撕膜扫码结果失败 {value}"); }
            return rs;
        }
        /// <summary>
        /// 设置复检扫码结果
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        public bool SetScanRecheckResult(EN_Result value)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置复检扫码结果时连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_ScannerRecheckResultAddr, (ushort)value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置复检扫码结果失败 {value}"); }
            return rs;
        }
        /// <summary>
        /// 设置复检条码
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public bool SetScannerRecheckSn(string sn)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置复检条码结果时连接异常 {sn}");
                return false;
            }
            bool rs = SetAddrStringValue(0, _plcParam.ToPLC_ScannerRecheckSnAddr, 50, sn);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置复检条码结果失败 {sn}"); }
            return rs;
        }

        /// <summary>
        /// 设置视觉完成，通信成功写1，通信失败写2
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        public bool SetCameraFinished(EN_Result value)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置视觉完成时连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_CameraFinished, (ushort)value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置视觉完成失败 {value}"); }
            return rs;
        }

        /// <summary>
        /// 设置拍全图，正常拍照写1，拍全图写2
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        public bool SetSaveAllPhotoModel(ushort value)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_SaveAllPhotoModel, value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置视觉完成失败 {value}"); }
            return rs;
        }

        /// <summary>
        /// 设置拍照结果
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        public bool SetCameraScanLineSNResult(ushort value)
        {
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_CameraScanLineSNResult, value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置视觉扫排线码结果 {value}失败"); }
            return rs;
        }

        /// <summary>
        /// 设置errorcode
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCarrierErrorcode(int[] errorcode)
        {
            if (!IsConnected)
            {
                return false;
            }
            ushort[] value = new ushort[errorcode.Length];
            for (int i = 0; i < errorcode.Length; i++)
            {
                value[i] = (ushort)errorcode[i];
            }
            //先给5130-5141写errorcode，再给5121写1告诉plc写完了
            return SetAddrMultiValue(0, _plcParam.ToPLC_CarrierTrayInfoStart, value)
                && SetAddrValue(0, _plcParam.ToPLC_SetNeedDwellFinished, 1);
        }

        public bool SetTaryIsEmpty(int value)
        {
            // 5126 1动2静
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置前站空穴信息时连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(0, _plcParam.ToPLC_TrayisEmpty, (ushort)value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置前站空穴信息失败 {value}"); }
            return rs;
        }

        /// <summary>
        /// 设置CPK
        /// </summary>
        /// <param name="plcidx"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCPK(byte plcidx, EN_PLC_CPK value)
        {
            // 5126 1动2静
            if (!IsConnected)
            {
                Console.WriteLine($"[PLC]设置CPK时连接异常 {value}");
                return false;
            }
            bool rs = SetAddrValue(plcidx, 5126, (ushort)value);
            if (rs == false)
            { Console.WriteLine($"[PLC]设置CPK失败 {value}"); }
            return rs;
        }

        /// <summary>
        /// 读寿命
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetAlive(int index)
        {
            ushort addr = (ushort)(7800 + index * 2);
            ushort[] values = new ushort[2];
            if (!GetAddrMutiValue(0, addr, 2, ref values))
            {
                return -1;
            }
            return values[0] + values[1] * 0x10000;
        }
        private const int layerCount = 7;//NG仓层数加1
        private const int cavnum = 12;//穴位个数
        private const int cavStatuPlcNum = 30;//PLC每层状态地址位数
        private const int carrierSnPlcNum = 50;//PLC每层载具SN地址位数
        private int[,] ngErrorCode = new int[layerCount, cavnum];
        private string[] CarrierSns = new string[layerCount];
        TrayStatusToStringConvert trayStatusToStringConvert = new TrayStatusToStringConvert();


        /// <summary>
        /// 读取NG仓状态
        /// </summary>
        /// <returns></returns>
        public bool ReadAllNGErrorCodes(out bool changed, out EN_TrayStatus[,] ngStatus)
        {
            changed = false;
            ngStatus = new EN_TrayStatus[layerCount, cavnum];
            int totalWords = layerCount * cavStatuPlcNum - cavStatuPlcNum;
            ushort[] ret = new ushort[totalWords];

            if (totalWords > 0)
            {
                int maxBatchSize = 120;
                int bytesRead = 0;

                while (bytesRead < totalWords)
                {
                    int batchSize = Math.Min(maxBatchSize, totalWords - bytesRead);
                    ushort[] batchBuffer = new ushort[batchSize];
                    ushort startAddr = (ushort)(_plcParam.FromPLC_NGStateFirstAddr + bytesRead);

                    if (!GetAddrMutiValue(_plcParam.StationID, startAddr, (ushort)batchSize, ref batchBuffer))
                    {
                        NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"获取NG仓状态失败 (地址:{startAddr}, 数量:{batchSize})", En_Logout_Type.Run, true);
                        return false;
                    }

                    Array.Copy(batchBuffer, 0, ret, bytesRead, batchSize);
                    bytesRead += batchSize;
                }

                for (int i = 1; i < layerCount; i++)
                {
                    for (int j = 0; j < cavnum; j++)
                    {
                        int cav = j + 1;
                        int idx = cavStatuPlcNum * (i - 1) + j;

                        if (idx >= 0 && idx < ret.Length)
                        {
                            if (ngErrorCode[i, j] != ret[idx])
                            {
                                NLogTrace.LogOut(EN_WARN_LEVEL.Info,
                                    $"NG仓{i}层{cav}穴状态：" +
                                    $"{(string)trayStatusToStringConvert.Convert((EN_TrayStatus)ngErrorCode[i, j], null, null, null)}->" +
                                    $"{(string)trayStatusToStringConvert.Convert((EN_TrayStatus)ret[idx], null, null, null)}",
                                    En_Logout_Type.Run, true);
                                ngErrorCode[i, j] = ret[idx];
                                changed = true;
                            }
                            ngStatus[i, j] = (EN_TrayStatus)ret[idx];
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 读取NG仓载具SN
        /// </summary>
        /// <returns></returns>
        public bool ReadAllNGCarrierSns(out bool changed, out string[] carrierSns)
        {
            changed = false;
            carrierSns = new string[layerCount];
            string str = "";

            for (int i = 1; i < layerCount; i++)
            {
                if (!GetAddrStringValue(_plcParam.StationID, (ushort)(_plcParam.FromPLC_NGCarrierSNFirstAddr + ((i - 1) * carrierSnPlcNum)), carrierSnPlcNum, ref str))
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Error, $"获取NG仓载具SN失败", En_Logout_Type.Run, true);
                    return false;
                }
                if (CarrierSns[i] != str)
                {
                    NLogTrace.LogOut(EN_WARN_LEVEL.Info, $"NG仓{i}层载具变化：{CarrierSns[i]}=>{str}", En_Logout_Type.Run, true);
                    CarrierSns[i] = str;
                    changed = true;
                }
                carrierSns[i] = str;
            }
            return true;
        }

        #endregion

        #region 告诉PLC上位机的运行状态

        /// <summary>
        /// 设置心跳，大约2s一次
        /// </summary>
        /// <returns></returns>
        public bool SetHeartBeat()
        {
            return SetAddrValue(_plcParam.StationID, _plcParam.ToPLC_HeartBeat, (ushort)1);
        }

        PcToPlcMachineStatus pcToPlcMachineStatus = PcToPlcMachineStatus.空闲中;
        /// <summary>
        /// 设置PC设备状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetPcMachineStatus(PcToPlcMachineStatus status)
        {
            if (status == PcToPlcMachineStatus.报警警告清除)
            {
                status = pcToPlcMachineStatus;
            }
            if (!(status == PcToPlcMachineStatus.报警中 || status == PcToPlcMachineStatus.警告中))
            {
                pcToPlcMachineStatus = status;
            }
            return SetAddrValue(_plcParam.StationID, _plcParam.ToPLC_RunStausAddr, (ushort)status);
        }

        /// <summary>
        /// 等价于按下机台启动/暂停/复位按钮
        /// </summary>
        public bool TrigPlcButton(EN_Plc_TrigButton button)
        {
            return SetAddrValue(_plcParam.StationID, _plcParam.ToPLC_PlcButton, (ushort)button);
        }

        #endregion

        #region Modbus基础指令、地址读写、字符读写
        public bool GetUshortOneBitStatus(ushort val, byte index)
        {
            if (index > 15)
            {//Bit越界
                return false;
            }
            return (val & (1 << index)) > 0 ? true : false;
        }
        public bool GetAddrValue(byte id, ushort addr, ref ushort value)
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }

                    return CSSIModbusRTUInterface.CSSIModbusRTU_GetAddrValue(_plcParam.ComPort, id, addr, true, ref value);
                }

                return CSSIModbusTCPInterface.CSSIModbusTCP_GetAddrValue(_clientId, id, addr, true, ref value); ;
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }
        public bool GetAddrMutiValue(byte id, ushort addr, ushort num, ref ushort[] valueUshorts)
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }

                    return CSSIModbusRTUInterface.CSSIModbusRTU_GetAddrMutiValue(_plcParam.ComPort, id, addr, true, num, valueUshorts);
                }
                return CSSIModbusTCPInterface.CSSIModbusTCP_GetAddrMutiValue(_clientId, id, addr, true, num, valueUshorts);
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }
        public bool GetAddrStringValue(byte id, ushort addr, ushort num, ref string str)
        {
            bool ret = false;
            ushort[] value = new ushort[num];
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }
                    ret = CSSIModbus.CSSIModbusRTUInterface.CSSIModbusRTU_GetAddrMutiValue(_plcParam.ComPort, id, addr, true, num, value);
                }
                else
                {
                    ret = CSSIModbus.CSSIModbusTCPInterface.CSSIModbusTCP_GetAddrMutiValue(_clientId, id, addr, true, num, value);
                }
                byte[] bytes = new byte[value.Length * 2];
                System.Buffer.BlockCopy(value, 0, bytes, 0, value.Length);
                str = Encoding.ASCII.GetString(bytes, 0, bytes.Length).Trim('\0');
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
            }
            return ret;
        }
        /// <summary>
        /// 读取连续的字符串(不能超过125)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addr"></param>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <param name="strArray"></param>
        /// <returns></returns>
        public bool GetAddrStringValues(byte id, ushort addr, ushort num, ushort count, ref string[] strArray)
        {
            bool ret = false;
            // 计算需要读取的总寄存器数量
            ushort totalRegisters = (ushort)(num * count);
            ushort[] value = new ushort[totalRegisters];

            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }
                    ret = CSSIModbus.CSSIModbusRTUInterface.CSSIModbusRTU_GetAddrMutiValue(_plcParam.ComPort, id, addr, true, totalRegisters, value);
                }
                else
                {
                    ret = CSSIModbus.CSSIModbusTCPInterface.CSSIModbusTCP_GetAddrMutiValue(_clientId, id, addr, true, totalRegisters, value);
                }

                // 初始化字符串数组
                strArray = new string[count];

                for (int i = 0; i < count; i++)
                {
                    // 获取当前字符串对应的寄存器数据
                    ushort[] stringRegisters = new ushort[num];
                    Array.Copy(value, i * num, stringRegisters, 0, num);

                    // 将寄存器数据转换为字节数组
                    byte[] bytes = new byte[num * 2];
                    Buffer.BlockCopy(stringRegisters, 0, bytes, 0, num * 2);

                    // 将字节数组转换为字符串并去除空字符
                    strArray[i] = Encoding.ASCII.GetString(bytes).Trim('\0');
                }
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
            }

            return ret;
        }
        public bool SetAddrValue(byte id, ushort addr, ushort value)
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }

                    return CSSIModbusRTUInterface.CSSIModbusRTU_SetAddrValue(_plcParam.ComPort, id, addr, value);
                }

                return CSSIModbusTCPInterface.CSSIModbusTCP_SetAddrValue(_clientId, id, addr, value);
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }
        public bool SetAddrMultiValue(byte id, ushort addr, ushort[] value)
        {
            try
            {
                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }

                    return CSSIModbusRTUInterface.CSSIModbusRTU_SetAddrMutiValue(_plcParam.ComPort, id, addr, value);
                }

                return CSSIModbusTCPInterface.CSSIModbusTCP_SetAddrMutiValue(_clientId, id, addr, value);
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }
        public bool SetAddrStringValue(byte id, ushort addr, ushort num, string str)
        {
            try
            {
                byte[] sndata = Encoding.ASCII.GetBytes(str);
                ushort[] value = new ushort[num];
                System.Buffer.BlockCopy(sndata, 0, value, 0, sndata.Length);

                if (!_plcParam.BUseModbusTcp)
                {
                    if (string.IsNullOrEmpty(_plcParam.ComPort) || !SerialPortCtrl.GetPortNames().Contains(_plcParam.ComPort))
                    {
                        return false;
                    }
                    return CSSIModbus.CSSIModbusRTUInterface.CSSIModbusRTU_SetAddrMutiValue(_plcParam.ComPort, id, addr, value);
                }
                else
                {
                    return CSSIModbus.CSSIModbusTCPInterface.CSSIModbusTCP_SetAddrMutiValue(_clientId, id, addr, value);
                }
            }
            catch (Exception e)
            {
                NLogTrace.LogOut(EN_WARN_LEVEL.Error, e + e.StackTrace);
                return false;
            }
        }
        #endregion

        #endregion
    }
}
