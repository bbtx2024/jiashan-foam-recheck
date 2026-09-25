using System;
using System.ComponentModel;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.MES
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("MES设置")]
    public class MESParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        public override bool BUse { get; set; } = true;

        [Category("0.启用")]
        [DisplayName("启动时是否MES检查软件版本")]
        public bool IsCheckVersion { get; set; } = false;
        [Category("1.Tape相关")]
        [DisplayName("Tape物料名称")]
        public string TapeName { get; set; } = "BATTERY FOAM";

        [Category("2.MES参数设定")]
        [DisplayName("Line")]
        public string Line { get; set; } = "BU22-J6311";//NPI: BU22-J6102

        [Category("2.MES参数设定")]
        [DisplayName("Station")]
        public string Station { get; set; } = "Flex GND Tape";

        [Category("2.MES参数设定")]
        [DisplayName("Fixid")]
        public string Fixid { get; set; } = "A1-1F-L01L-Step Flex Bend and Assy";//NPI: 001

        [Category("2.MES参数设定")]
        [DisplayName("MesIP")]
        public string MesIP { get; set; } = "";

        [Category("2.MES参数设定")]
        [DisplayName("操作人员编号")]
        public string Opuserid { get; set; } = "Quick";

        //[Category("2.MES参数设定")]
        //[DisplayName("启用MES查询软件版本")]
        //public bool IsCheckVersion { get; set; } = true;

        [Category("3.MES参数设定")]
        [DisplayName("MES上传保压压力")]
        public string BumperPressure { get; set; } = "1.0";

        [Category("3.MES参数设定")]
        [DisplayName("MES上传保压时间")]
        public string BumperPressureTime { get; set; } = "05";
    }
}
