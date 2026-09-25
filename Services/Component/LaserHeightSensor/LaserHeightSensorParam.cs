/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-12
 * 说明：（激光测高仪参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.LaserHeightSensor
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("激光测高设置")]
    public class LaserHeightSensorParam : IParam
    {

        public void SetAllReadOnly(bool value)
        {
            var props = typeof(LaserHeightSensorParam).GetProperties();
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
        [Category("0.启用")]
        [DisplayName("启用")]
        public bool BUse { get; set; } = true;

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("端口号")]
        public string COMPort { get; set; } = "COM7";

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("波特率")]
        public int Baudrate { get; set; } = 9600;

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("写入超时")]
        [Description("ms")]
        public int SendTimeout { get; set; } = 3000;

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("读取超时")]
        [Description("ms")]
        public int ReceiveTimeout { get; set; } = 3000;

        [XmlIgnore]
        [Category("2.参数设置")]
        [DisplayName("激光测高延时")]
        [Description("ms")]
        public int MeasureHeightDelay { get; set; } = 100;

        [Category("2.参数设置"), DisplayName("测高范围下限")]
        public float AllowMinDistance { get; set; } = 0.800f;

        [Category("2.参数设置"), DisplayName("测高范围上限")]
        public float AllowMaxDistance { get; set; } = 0.940f;
    }
}
