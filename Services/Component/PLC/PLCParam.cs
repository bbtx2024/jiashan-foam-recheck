/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-14
 * 说明：（PLC参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using QA.Business.Component.PLC.Attributes;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.PLC
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("PLC设置")]
    public class PLCParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        public override bool BUse { get; set; } = true;

        [Category("1.启用")]
        [DisplayName("启用ModbusTcp")]
        [ReadOnly(true)]
        public bool BUseModbusTcp { get; set; } = true;

        [TypeConverter(typeof(SerialPortsConverter))]
        [XmlIgnore]
        [Category("2.Modbus485通讯参数")]
        [DisplayName("串口号")]
        public string ComPort { get; set; }

        [XmlIgnore]
        [Category("2.Modbus485通讯参数")]
        [DisplayName("站号")]
        [ReadOnly(true)]
        public byte StationID { get; set; } = 1;

        [Category("3.ModbusTcp通讯")]
        [DisplayName("IP")]
        //[ReadOnly(true)]
        public string IP { get; set; } = "172.16.11.100";

        [XmlIgnore]
        [Category("3.ModbusTcp通讯")]
        [DisplayName("端口")]
        //[ReadOnly(true)]
        public int Port { get; set; } = 502;

        [XmlIgnore]
        [Category("3.ModbusTcp通讯")]
        [DisplayName("缓冲区大小")]
        [Description("kb")]
        [ReadOnly(true)]
        public int BufferSize { get; set; } = 1024;

        private int _timeout = 3000;
        [XmlIgnore]
        [Category("3.ModbusTcp通讯")]
        [DisplayName("超时")]
        [Description("ms")]
        [ReadOnly(true)]
        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 3000)
                {
                    _timeout = value;
                }
            }
        }

        #region 读通信地址
        private ushort _fromPLC_StartAddr = 2000;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-起始地址")]
        [ReadOnly(true)]
        public ushort FromPLC_StartAddr
        {
            get => _fromPLC_StartAddr;
            set => _fromPLC_StartAddr = value;
        }

        private ushort _fromPlc_AddrNum = 50;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-地址个数")]
        [ReadOnly(true)]
        public ushort FromPLC_AddrNum
        {
            get => _fromPlc_AddrNum;
            set
            {
                if (value < 1) value = 1;
                if (value > 100) value = 100;
                if (_fromPlc_AddrNum > 0)
                {
                    _fromPlc_AddrNum = value;
                }
            }
        }

        private ushort _fromPlcMachineStatus = 2000;

        [Category("4.读通讯地址")]
        [DisplayName("读PLC-机台状态地址")]
        [Description("机台启动中1、急停中2、复位中3、暂停中4")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_MachineStatus
        {
            get => _fromPlcMachineStatus;
            set
            {
                if (_fromPlcMachineStatus >= _fromPLC_StartAddr)
                {
                    _fromPlcMachineStatus = value;
                }
            }
        }

        private ushort _fromPlcRunStatus = 2001;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-控制上位机状态地址")]
        [Description("PLC控制上位机 bit0-上电 bit1—启动 bit2—复位 bit3—暂停 bit4—急停")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_RunStatus
        {
            get => _fromPlcRunStatus;
            set
            {
                if (_fromPlcRunStatus >= _fromPLC_StartAddr)
                {
                    _fromPlcRunStatus = value;
                }
            }
        }

        private ushort _fromPLC_TriggerScanner = 2003;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-触发撕膜扫码标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_TriggerScanner
        {
            get => _fromPLC_TriggerScanner;
            set
            {
                if (_fromPLC_TriggerScanner >= _fromPLC_StartAddr)
                {
                    _fromPLC_TriggerScanner = value;
                }
            }
        }
        private ushort _fromPLC_TriggerScannerRecheck = 2004;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-触发复检扫码标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_TriggerScannerRecheck
        {
            get => _fromPLC_TriggerScannerRecheck;
            set
            {
                if (_fromPLC_TriggerScannerRecheck >= _fromPLC_StartAddr)
                {
                    _fromPLC_TriggerScannerRecheck = value;
                }
            }
        }
        private ushort _fromPLC_TriggerCameraResul = 2005;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-触发视觉结果标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_TriggerCameraResul
        {
            get => _fromPLC_TriggerCameraResul;
            set
            {
                if (_fromPLC_TriggerCameraResul >= _fromPLC_StartAddr)
                {
                    _fromPLC_TriggerCameraResul = value;
                }
            }
        }


        //private ushort _fromPLC_SafeDoorAlarmAddr = 5004;
        //[Category("4.读通讯地址")]
        //[DisplayName("读PLC-安全门报警地址")]
        //[Description("0:正常 1:报警")]
        //[TriggerHandler(true)]
        //[ReadOnly(true)]
        //public ushort FromPLC_SafeDoorAlarmAddr
        //{
        //    get => _fromPLC_SafeDoorAlarmAddr;
        //    set
        //    {
        //        if (_fromPLC_SafeDoorAlarmAddr >= _fromPLC_StartAddr)
        //        {
        //            _fromPLC_SafeDoorAlarmAddr = value;
        //        }
        //    }
        //}

        private ushort _fromPLC_SimulateRunSignal = 5000;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-空跑模式标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_SimulateRunSignal
        {
            get => _fromPLC_SimulateRunSignal;
            set
            {
                _fromPLC_SimulateRunSignal = value;
            }
        }
        private ushort _fromPLC_SimulateRunSignal2 = 5001;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-带载具空跑模式标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_SimulateRunSignal2
        {
            get => _fromPLC_SimulateRunSignal2;
            set
            {
                _fromPLC_SimulateRunSignal2 = value;
            }
        }
        private ushort _fromPLC_TriggerCameraTwice = 5100;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-触发拍照标记")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_TriggerCameraTwice
        {
            get => _fromPLC_TriggerCameraTwice;
            set
            {
                _fromPLC_TriggerCameraTwice = value;
            }
        }

        private ushort _fromPLC_IdxNow = 5101;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-当前处理穴位")]
        [TriggerHandler(true)]
        [ReadOnly(true)]
        public ushort FromPLC_IdxNow
        {
            get => _fromPLC_IdxNow;
            set
            {
                _fromPLC_IdxNow = value;
            }
        }



        private ushort _fromPLC_NGStateFirstAddr = 6020;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-NG仓状态起始地址")]
        [TriggerHandler(true)]
        [ReadOnly(true)]

        public ushort FromPLC_NGStateFirstAddr
        {
            get => _fromPLC_NGStateFirstAddr;
            set => _fromPLC_NGStateFirstAddr = value;
        }
        private ushort _fromPLC_CarrierSNStateFirstAddr = 6600;
        [Category("4.读通讯地址")]
        [DisplayName("读PLC-NG仓载具SN起始地址")]
        [TriggerHandler(true)]
        [ReadOnly(true)]

        public ushort FromPLC_NGCarrierSNFirstAddr
        {
            get => _fromPLC_CarrierSNStateFirstAddr;
            set => _fromPLC_CarrierSNStateFirstAddr = value;
        }
        #endregion

        #region 写通信地址
        private ushort _toPLC_StartAddr = 3000;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-起始地址")]
        [ReadOnly(true)]
        public ushort ToPLC_StartAddr
        {
            get => _toPLC_StartAddr;
            set => _toPLC_StartAddr = value;
        }

        [Category("4.写通讯地址")]
        [DisplayName("写PLC-地址个数")]
        [ReadOnly(true)]
        public ushort ToPLC_AddrNum { get; set; } = 50;

        private ushort _toPLC_RunStausAddr = 3000;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-工位运行状态地址")]
        [Description("1.运行 2.急停 3.复位 4.暂停 5.复位完成 6.工作完成 9. 空闲中")]
        [ReadOnly(true)]
        public ushort ToPLC_RunStausAddr
        {
            get => _toPLC_RunStausAddr;
            set
            {
                if (_toPLC_RunStausAddr >= _toPLC_StartAddr)
                {
                    _toPLC_RunStausAddr = value;
                }
            }
        }


        private ushort _toPLC_PlcButton = 3001;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-控制PLC按钮")]
        [Description("1:控制PLC启动 2:控制PLC暂停 3:控制PLC复位")]
        [FloatAddress(true)]
        [ReadOnly(true)]
        public ushort ToPLC_PlcButton
        {
            get => _toPLC_PlcButton;
            set
            {
                if (_toPLC_PlcButton >= _toPLC_StartAddr)
                {
                    _toPLC_PlcButton = value;
                }
            }
        }


        private ushort _toPLC_HeartBeat = 3002;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-心跳信号")]
        [Description("1:心跳正常")]
        [FloatAddress(true)]
        [ReadOnly(true)]
        public ushort ToPLC_HeartBeat
        {
            get => _toPLC_HeartBeat;
            set
            {
                if (_toPLC_HeartBeat >= _toPLC_StartAddr)
                {
                    _toPLC_HeartBeat = value;
                }
            }
        }
        private ushort _toPLC_ScannerResultAddr = 3003;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-撕膜扫码结果")]
        [Description("1:OK 2:NG 0:直接过站")]
        [ReadOnly(true)]
        public ushort ToPLC_ScannerResultAddr
        {
            get => _toPLC_ScannerResultAddr;
            set
            {
                _toPLC_ScannerResultAddr = value;
            }
        }
        private ushort _toPLC_ScannerSnAddr = 3004;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-撕膜扫码条码")]
        [Description("写PLC-撕膜扫码条码")]
        [ReadOnly(true)]
        public ushort ToPLC_ScannerSnAddr
        {
            get => _toPLC_ScannerSnAddr;
            set
            {
                _toPLC_ScannerSnAddr = value;
            }
        }
        private ushort _toPLC_ScannerRecheckResultAddr = 3054;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-复检扫码结果")]
        [Description("1:OK 2:NG 0:直接过站")]
        [ReadOnly(true)]
        public ushort ToPLC_ScannerRecheckResultAddr
        {
            get => _toPLC_ScannerRecheckResultAddr;
            set
            {
                _toPLC_ScannerRecheckResultAddr = value;
            }
        }
        private ushort _toPLC_ScannerRecheckSnAddr = 3055;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-复检扫码条码")]
        [Description("写PLC-复检扫码条码")]
        [ReadOnly(true)]
        public ushort ToPLC_ScannerRecheckSnAddr
        {
            get => _toPLC_ScannerRecheckSnAddr;
            set
            {
                _toPLC_ScannerRecheckSnAddr = value;
            }
        }

        private ushort _toPLC_CameraFinished = 5500;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-拍照完成")]
        [Description("1:OK 2:NG 0:直接过站")]
        [ReadOnly(true)]
        public ushort ToPLC_CameraFinished
        {
            get => _toPLC_CameraFinished;
            set
            {
                _toPLC_CameraFinished = value;
            }
        }
        private ushort _toPLC_SetNeedDwellFinished = 3105;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-载具盘物料复检信息是否写完")]
        [Description("1:已写完")]
        [FloatAddress(true)]
        [ReadOnly(true)]
        public ushort ToPLC_SetNeedDwellFinished
        {
            get => _toPLC_SetNeedDwellFinished;
            set
            {
                _toPLC_SetNeedDwellFinished = value;
            }
        }

        private ushort _toPLC_TrayisEmpty = 3120;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-前站反馈是否是空穴")]
        [Description("int型1-12位分别代表对应穴位状态，1：空穴，0：非空")]
        [FloatAddress(true)]
        [ReadOnly(true)]
        public ushort ToPLC_TrayisEmpty
        {
            get => _toPLC_TrayisEmpty;
            set
            {
                _toPLC_TrayisEmpty = value;
            }
        }

        private ushort _toPLC_SaveAllPhotoModel = 5103;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-拍全图模式选择")]
        [Description("0.空穴 1:OK 2:NG")]
        [ReadOnly(true)]
        public ushort ToPLC_SaveAllPhotoModel
        {
            get => _toPLC_SaveAllPhotoModel;
            set
            {
                _toPLC_SaveAllPhotoModel = value;
            }
        }

        private ushort _toPLC_CarrierTrayInfoSSStart = 5530;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-载具盘物料撕膜信息")]
        [Description("0.空穴 1:OK 2:NG")]
        [ReadOnly(true)]
        public ushort ToPLC_CarrierTrayInfoSSStart
        {
            get => _toPLC_CarrierTrayInfoSSStart;
            set
            {
                _toPLC_CarrierTrayInfoSSStart = value;
            }
        }


        private ushort _toPLC_CarrierTrayInfoStart = 5530;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-载具盘物料复检信息")]
        [Description("0.空穴 1:OK 2:NG")]
        [ReadOnly(true)]
        public ushort ToPLC_CarrierTrayInfoStart
        {
            get => _toPLC_CarrierTrayInfoStart;
            set
            {
                _toPLC_CarrierTrayInfoStart = value;
            }
        }

        private ushort _toPLC_CameraScanLineSNResult = 3108;
        [Category("4.写通讯地址")]
        [DisplayName("写PLC-载具盘物料复检信息")]
        [Description("0.空穴 1:OK 2:NG")]
        [ReadOnly(true)]
        public ushort ToPLC_CameraScanLineSNResult
        {
            get => _toPLC_CameraScanLineSNResult;
            set
            {
                _toPLC_CameraScanLineSNResult = value;
            }
        }

        #endregion
    }
}
