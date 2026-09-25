using System;
using System.ComponentModel;
using QA.Business.Interfaces;
using QA_Infrastructure;

namespace QA.Business.Component.HIVE
{
    [SaveParam(FileType.JSON)]
    [Serializable]
    [Description("Hive系统设置")]
    public class HiveParam : IParam
    {
        [Category("0.启用"), DisplayName("模块启用")]
        public override bool BUse { get; set; } = true;

        [Category("1.Hive参数"), DisplayName("IP地址")]
        public string Hive_IP { get; set; } = "10.0.0.2";

        [Category("1.Hive参数"), DisplayName("端口号")]
        public string Hive_Port { get; set; } = "5008";

        [Category("2.其他"), DisplayName("Site")]
        public string Site { get; set; } = "ITJS";

        [Category("2.其他"), DisplayName("Vendor")]
        public string Vendor { get; set; } = "Quick";

        [Category("2.其他"), DisplayName("Line_type")]
        public string Line_type { get; set; } = "";

        [Category("2.其他"), DisplayName("Rfid")]
        public string Rfid { get; set; } = "";

        [Category("2.其他"), DisplayName("Station_type")]
        public string Station_type { get; set; } = "LA6";

        [Category("2.其他"), DisplayName("SF_line_ID")]
        public string SF_line_ID { get; set; } = "";

        [Category("2.其他"), DisplayName("软件更新时间")]
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;

        [Category("2.其他"), DisplayName("软件版本维持红色天数")]
        public int UpdateRedDays { get; set; } = 7;

        //添加Hive点位信息""
        [Category("3.HIVE上传参数列表"), DisplayName("Assy X1_upper limite")] //组装上限
        public string Assy_X1_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy X1_lower limite")] //组装下限
        public string Assy_X1_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy Y1_upper limite")] 
        public string Assy_Y1_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy Y1_lower limite")]
        public string Assy_Y1_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy R1_upper limite")] 
        public string Assy_R1_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy R1_lower limite")]
        public string Assy_R1_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy X2_upper limite")] //组装上限
        public string Assy_X2_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy X2_lower limite")] //组装下限
        public string Assy_X2_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy Y2_upper limite")]
        public string Assy_Y2_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy Y2_lower limite")]
        public string Assy_Y2_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy R2_upper limite")]
        public string Assy_R2_Upperlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Assy R2_lower limite")]
        public string Assy_R2_Lowerlimite { get; set; } = "0";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_X1")]
        public string Safty_position_X1 { get; set; } = "320";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_Y1")]
        public string Safty_position_Y1 { get; set; } = "260";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_Z1")]
        public string Safty_position_Z1 { get; set; } = "40";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_X2")]
        public string Safty_position_X2 { get; set; } = "330";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_Y2")]
        public string Safty_position_Y2 { get; set; } = "460";

        [Category("3.HIVE上传参数列表"), DisplayName("Safty position_Z2")]
        public string Safty_position_Z2 { get; set; } = "40";

        //拍照
        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X1")]
        public string Vision_position_X1 { get; set; } = "247.673";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y1")]
        public string Vision_position_Y1 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z1")]
        public string Vision_position_Z1 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X4")]
        public string Vision_position_X4 { get; set; } = "247.673";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y4")]
        public string Vision_position_Y4 { get; set; } = "193.067";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z4")]
        public string Vision_position_Z4 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X7")]
        public string Vision_position_X7 { get; set; } = "247.673";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y7")]
        public string Vision_position_Y7 { get; set; } = "148.155";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z7")]
        public string Vision_position_Z7 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X10")]
        public string Vision_position_X10 { get; set; } = "247.673";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y10")]
        public string Vision_position_Y10 { get; set; } = "102.672";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z10")]
        public string Vision_position_Z10 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X2")]
        public string Vision_position_X2 { get; set; } = "157.701";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y2")]
        public string Vision_position_Y2 { get; set; } = "237.721";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z2")]
        public string Vision_position_Z2 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X5")]
        public string Vision_position_X5 { get; set; } = "157.701";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y5")]
        public string Vision_position_Y5 { get; set; } = "192.914";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z5")]
        public string Vision_position_Z5 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X8")]
        public string Vision_position_X8 { get; set; } = "157.701";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y8")]
        public string Vision_position_Y8 { get; set; } = "148.038";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z8")]
        public string Vision_position_Z8 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X11")]
        public string Vision_position_X11 { get; set; } = "157.701";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y11")]
        public string Vision_position_Y11 { get; set; } = "102.898";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z11")]
        public string Vision_position_Z11 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X3")]
        public string Vision_position_X3 { get; set; } = "67.746";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y3")]
        public string Vision_position_Y3 { get; set; } = "238.221";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z3")]
        public string Vision_position_Z3 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X6")]
        public string Vision_position_X6 { get; set; } = "67.746";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y6")]
        public string Vision_position_Y6 { get; set; } = "193.323";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z6")]
        public string Vision_position_Z6 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X9")]
        public string Vision_position_X9 { get; set; } = "67.746";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y9")]
        public string Vision_position_Y9 { get; set; } = "148.31";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z9")]
        public string Vision_position_Z9 { get; set; } = "238.43";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_X12")]
        public string Vision_position_X12 { get; set; } = "67.746";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Y12")]
        public string Vision_position_Y12 { get; set; } = "103.302";

        [Category("3.HIVE上传参数列表"), DisplayName("Vision position_Z12")]
        public string Vision_position_Z12 { get; set; } = "238.43";
    }
}
