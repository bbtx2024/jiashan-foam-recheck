/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-24
 * 说明：（相机参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.Camera
{
    [SaveParam(FileType.JSON)]
    [Serializable]
    [Description("相机设置")]
    public class CameraParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        //[ReadOnly(true)]
        //public override bool BUse { get => true; }
        public override bool BUse { get; set; }

        [Category("0.启用"), DisplayName("存全图")]
        public bool GetAllPhoto { get; set; } = false;

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("IP")]
        public string IP { get; set; } = "127.0.0.1";

        [XmlIgnore]
        [Category("1.通讯参数")]
        [DisplayName("端口")]
        public int Port { get; set; } = 8150;

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

        private string visionPath = @"E:\机台源码勿动\DCCK Project\DCCK-FA06-005D\DCCK-FA06-005D.vps";
        [Category("2.视觉软件参数配置")]
        [DisplayName("德创视觉vps文件路径")]
        public string VisionPath
        {
            get
            {
                return visionPath;
            }
            set
            {
                if (!visionPath.EndsWith(".vps"))
                {
                    visionPath = "";
                }
                else
                {
                    visionPath = value;
                }
            }
        }

        private string imgPath = @"D:\ImgSave";
        [Category("2.视觉软件参数配置")]
        [DisplayName("图片存储路径")]
        public string ImgPath
        {
            get
            {
                return imgPath;
            }
            set
            {
                imgPath = value.TrimEnd('\\');
            }
        }

        [Category("3.FTP图片上传")]
        [DisplayName("启用图片打包上传至服务器")]
        public bool FTP_AutoUploadImg { get; set; } = false;

        [Category("3.FTP图片上传")]
        [DisplayName("服务器IP")]
        public string FTP_ServerIP { get; set; } = "10.55.60.30";

        [Category("3.FTP图片上传")]
        [DisplayName("服务器端口")]
        public int FTP_ServerPort { get; set; } = 21;

        [Category("3.FTP图片上传")]
        [DisplayName("登录用户名")]
        public string FTP_UserName { get; set; } = "LA3_Line1";

        [Category("3.FTP图片上传")]
        [DisplayName("登陆密码")]
        public string FTP_Password { get; set; } = "sunny123";

        [Category("3.FTP图片上传")]
        [DisplayName("线别")]
        public string FTP_Line { get; set; } = "LA3_Line1";

        [Category("3.FTP图片上传")]
        [DisplayName("工站")]
        public string FTP_Station { get; set; } = "Step ANT-复检";

        [Category("4.复检拍照延时(ms)")]
        [DisplayName("工站")]
        public int DelayPhtotTime { get; set; } = 200;
    }
}
