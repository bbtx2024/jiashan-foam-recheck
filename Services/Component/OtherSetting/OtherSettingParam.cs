/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-03-16
 * 说明：（其他功能参数）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using QA.Business.Interfaces;
using QA.Business.Manager;
using QA_Infrastructure;

namespace QA.Business.Component.OtherSetting
{
    [SaveParam(FileType.Binary)]
    [Serializable]
    [Description("其他设置")]
    public class OtherSettingParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        [Browsable(false)]
        public override bool BUse { get; set; }

        [Category("语言"), DisplayName("语言")]
        [Browsable(false)]
        public int LanguageIndex { get; set; } = 0;

        [Category("时间"), DisplayName("自动切回主界面时间(Minutes)")]
        public double QuitTime { get; set; } = 1.0;

        [Category("1.其他设置"), DisplayName("允许使用用户名密码登录")]
        [ReadOnly(true)]
        public bool EnableCommonLogin { get; set; } = true;

        //20250716 是否使用手动设定版本号
        [Category("1.其他设置"), DisplayName("使用手动设定版本号")]
        public bool UseManualSoftwareVersion { get; set; } = true;

        [Category("1.其他设置"), DisplayName("手动设定软件版本号")]
        public string ManualSoftwareVersion { get; set; } = "";

        [Category("1.其他设置"), DisplayName("软件版本号")]
        public string SoftwareVersion
        {
            get
            {
                //if (!string.IsNullOrEmpty(ManualSoftwareVersion))
                //{
                //    return ManualSoftwareVersion;
                //}
                //20250716 版本号
                if (UseManualSoftwareVersion)
                {
                    if (!string.IsNullOrEmpty(ManualSoftwareVersion))
                    {
                        return ManualSoftwareVersion;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ManualSoftwareVersion))
                    {
                        return ManualSoftwareVersion;
                    }
                }
                return $"QK_{IoC.Get<CacheParamManager>().HomeUiParam.Statistic.CurrSoftwareVersion}.1.0.1_{new FileInfo(Process.GetCurrentProcess().MainModule.FileName).LastWriteTime.ToString("yyMMdd")}_POR";
            }
        }

        [Category("1.其他设置"), DisplayName("是否为大线")]
        public bool IsLargeLine { get; set; } = false;

        [Category("1.其他设置"), DisplayName("是否为量产")]
        public bool IsMP { get; set; } = true;
    }
}
