/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-10
 * 说明：（PDCA参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using QA.Business.Define;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.PDCA
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("PDCA设置")]
    public class PDCAParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        public override bool BUse { get; set; } = true;

        [Category("0.启用")]
        [DisplayName("启用获取前站TP数据并上传")]
        public bool UploadTP { get; set; } = true;

        [Category("0.启用")]
        [DisplayName("启用上传NG产品")]
        public bool UploadNgProduct { get; set; } = false;

        [Category("1.通讯")]
        [DisplayName("IP")]
        public string IP { get; set; } = "169.254.1.10";

        [XmlIgnore]
        [Category("1.通讯")]
        [DisplayName("端口")]
        public int Port { get; set; } = 1111;

        [XmlIgnore]
        [Category("1.通讯")]
        [DisplayName("缓冲区大小")]
        [Description("kb")]
        public int BufferSize { get; set; } = 8192;

        [XmlIgnore]
        [Category("1.通讯")]
        [DisplayName("超时")]
        [Description("ms")]
        public int Timeout { get; set; } = 10000;

        [Category("2.参数设置")]
        [DisplayName("AE_Vendor")]
        public string AE_Vendor { get; set; } = "Quick";

        [Category("2.参数设置")]
        [DisplayName("machine_ID")]
        public string machine_ID { get; set; } = "Flex GND Tape";

        [Category("2.参数设置")]
        [DisplayName("Operator_ID")]
        public int Operator_ID { get; set; } = 1;

        [Category("2.参数设置")]
        [DisplayName("Mode")]
        public En_PdcaMode Mode { get; set; } = En_PdcaMode.PROD;

        [Category("2.参数设置")]
        [DisplayName("TestSeriesID")]
        public int TestSeriesID { get; set; } = 0;

        [Category("2.参数设置")]
        [DisplayName("Priority")]
        public int Priority { get; set; } = 1;

        [Category("2.参数设置")]
        [DisplayName("online")]
        public int online { get; set; } = 1;

        [Category("2.参数设置")]
        [DisplayName("本机IP")]
        public string CurrIP { get; set; } = "169.254.1.100";

        [Category("2.参数设置")]
        [DisplayName("UserName")]
        public string UserName { get; set; } = "admin";

        [Category("2.参数设置")]
        [DisplayName("Password")]
        public string Password { get; set; } = "06005";

        [Category("2.参数设置")]
        [DisplayName("前站TP机MES IP")]
        public string TP_IP { get; set; } = "10.55.72.18";

        [Category("3.PDCA自动补传")]
        [DisplayName("上传失败信息留存天数")]
        public int KeepReUploadInfoDays { get; set; } = 7;
    }
}
