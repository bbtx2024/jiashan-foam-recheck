using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.Monitor
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("Monitor设置")]
    public class MonitorParam : IParam
    {
        public void SetAllReadOnly(bool value)
        {
            var props = typeof(MonitorParam).GetProperties();
            foreach (var prop in props)
            {
                if (prop.IsDefined(typeof(ReadOnlyAttribute)))
                {
                    //CommonFunc.SetAttributes<ReadOnlyAttribute>(this, prop.Name, value);
                }
            }
        }

        public bool SetReadOnly(string propertyName, bool value)
        {
            //return CommonFunc.SetAttributes<ReadOnlyAttribute>(this, propertyName, value);
            return false;
        }

        [Browsable(true)]
        [Category("0.启用")]
        [DisplayName("启用")]
        public bool BUse { get; set; } = true;
        [TypeConverter(typeof(SerialPortsConverter))]
        [Browsable(true)]
        [Category("1.连接设置"), DisplayName("串口号")]
        public string PortName { get; set; }
        [Browsable(true), ReadOnly(false)]
        [Category("1.连接设置"), DisplayName("波特率")]
        public int Baudrate { get; set; } = 115200;

        [Browsable(true), ReadOnly(false)]
        [Category("1.连接设置"), DisplayName("数据位")]
        public int DataBit { get; set; } = 8;

        [Browsable(true), ReadOnly(false)]
        [Category("1.连接设置"), DisplayName("停止位")]
        public StopBits StopBits { get; set; } = StopBits.One;

        [Browsable(true), ReadOnly(false)]
        [Category("1.连接设置"), DisplayName("校验位")]
        public Parity Parity { get; set; } = Parity.None;

        [Browsable(true), ReadOnly(false)]
        [Category("1.连接设置"), DisplayName("超时时长")]
        public int TimeOut { get; set; } = 500;

        [Browsable(true), ReadOnly(false)]
        [Category("2.读取设置"), DisplayName("起始位")]
        public int StartAddress { get; set; } = 18;

        [Browsable(true), ReadOnly(false)]
        [Category("2.读取设置"), DisplayName("长度")]
        public int ReadLength { get; set; } = 12;
    }
}
