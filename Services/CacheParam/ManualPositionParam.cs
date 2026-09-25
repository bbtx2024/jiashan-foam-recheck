/*******************************************************
 * 快克智能装备股份有限公司
 * 作者：胡勇
 * 创建日期：2022-02-23
 * 说明：（定位位置数据）
 * 版本号：1.0.0 
 * 修改记录：日期 + 修改内容
 * 
*******************************************************/
using System;
using System.ComponentModel;
using QA_Infrastructure;
namespace QA.Business.CacheParam
{
    [SaveParam(FileType.JSON)]
    [Serializable]
    [Description("点检位置设置")]
    public class ManualPositionParam
    {

        [DisplayName("9点标定移动距离")]
        public float MoveDisNineCalib { get; set; } = 1;

        [DisplayName("1号上相机中心坐标")]
        public float[] UpCameraNo1CenterPos { get; set; } = new float[4];

        [DisplayName("3号上相机中心坐标")]
        public float[] UpCameraNo3CenterPos { get; set; } = new float[4];

        [DisplayName("1号吸嘴中心坐标")]
        public float[] AxisNozzleNo1CenterPos { get; set; } = new float[4];

        [DisplayName("3号吸嘴中心坐标")]
        public float[] AxisNozzleNo3CenterPos { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_1 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_2 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_3 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_4 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_5 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_6 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_7 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_8 { get; set; } = new float[4];

        [DisplayName("1号上相机9点标定起始点坐标")]
        public float[] UpCameraNo1NineCalibPos_9 { get; set; } = new float[4];


        [DisplayName("3号上相机9点标定起始点坐标")]
        public float[] UpCameraNo3NineCalibPos { get; set; } = new float[4];

        [DisplayName("Feeder Pick坐标")]
        public float[] AxisFeederPickPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴1左取料位")]
        public float[] AxisFeederPickNozzle1LeftPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴1右取料位")]
        public float[] AxisFeederPickNozzle1RightPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴2左取料位")]
        public float[] AxisFeederPickNozzle2LeftPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴2右取料位")]
        public float[] AxisFeederPickNozzle2RightPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴3左取料位")]
        public float[] AxisFeederPickNozzle3LeftPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴3右取料位")]
        public float[] AxisFeederPickNozzle3RightPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴4左取料位")]
        public float[] AxisFeederPickNozzle4LeftPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Feeder 吸嘴4右取料位")]
        public float[] AxisFeederPickNozzle4RightPos
        {
            get; set;
        } = new float[4];

        //[DisplayName("Feeder Pick R轴角度,R1->R4")]
        //public float[] AxisFeederPickRPos
        //{
        //    get; set;
        //} = new float[4];

        [DisplayName("Feeder标定坐标")]
        public float[] AxisFeederCalibPos
        {
            get; set;
        } = new float[4];

        [DisplayName("1号嘴吸取标定片1号坐标")]
        public float[] AxisNozzleNo1SucPiecePosNo1
        {
            get; set;
        } = new float[4];
        [DisplayName("1号嘴吸取标定片2号坐标")]
        public float[] AxisNozzleNo1SucPiecePosNo2
        {
            get; set;
        } = new float[4];
        [DisplayName("1号嘴吸取标定片3号坐标")]
        public float[] AxisNozzleNo1SucPiecePosNo3
        {
            get; set;
        } = new float[4];
        [DisplayName("1号嘴吸取标定片4号坐标")]
        public float[] AxisNozzleNo1SucPiecePosNo4
        {
            get; set;
        } = new float[4];

        [DisplayName("载具Fov标定1号坐标")]
        public float[] AxisCarrierJointCalibPosNo1
        {
            get; set;
        } = new float[4];
        [DisplayName("载具Fov标定2号坐标")]
        public float[] AxisCarrierJointCalibPosNo2
        {
            get; set;
        } = new float[4];
        [DisplayName("载具Fov标定3号坐标")]
        public float[] AxisCarrierJointCalibPosNo3
        {
            get; set;
        } = new float[4];
        [DisplayName("载具Fov标定4号坐标")]
        public float[] AxisCarrierJointCalibPosNo4
        {
            get; set;
        } = new float[4];

        [DisplayName("固定点取标定片1号吸嘴坐标")]
        public float[] AxisFetchPlateJointCalibPosNozzleNo1
        {
            get; set;
        } = new float[4];

        [DisplayName("固定点取标定片2号吸嘴坐标")]
        public float[] AxisFetchPlateJointCalibPosNozzleNo2
        {
            get; set;
        } = new float[4];

        [DisplayName("固定点取标定片3号吸嘴坐标")]
        public float[] AxisFetchPlateJointCalibPosNozzleNo3
        {
            get; set;
        } = new float[4];

        [DisplayName("固定点取标定片4号吸嘴坐标")]
        public float[] AxisFetchPlateJointCalibPosNozzleNo4
        {
            get; set;
        } = new float[4];

        [DisplayName("Ng料仓坐标")]
        public float[] AxisNgSiloPos
        {
            get; set;
        } = new float[4];

        [DisplayName("Ng料仓最大压力")]
        public float flingMaterialsMaxForce
        {
            get; set;
        } = 2.0f;

        [DisplayName("扫载具码坐标")]
        public float[] AxisScannerCarrierBarcodePos
        {
            get; set;
        } = new float[4];

        [DisplayName("测试次数")]
        public int TestCount
        {
            get; set;
        }

        [DisplayName("轴精度停止时间")]
        public float AxisWaitTime
        {
            get; set;
        }

        [DisplayName("上相机静态测试坐标")]
        public float[] UpCamStaticTestPos
        {
            get; set;
        } = new float[4];

        [DisplayName("下相机静态测试坐标")]
        public float[] DownCamStaticTestPos
        {
            get; set;
        } = new float[4];

        [DisplayName("上相机动态测试起点")]
        public float[] UpCamDynamicTestStartPos
        {
            get; set;
        } = new float[4];

        [DisplayName("上相机动态测试终点")]
        public float[] UpCamDynamicTestEndPos
        {
            get; set;
        } = new float[4];

        [DisplayName("下相机动态测试起点")]
        public float[] DownCamDynamicTestStartPos
        {
            get; set;
        } = new float[4];

        [DisplayName("下相机动态测试终点")]
        public float[] DownCamDynamicTestEndPos
        {
            get; set;
        } = new float[4];


        [DisplayName("1#吸嘴联合标定坐标")]
        public float[] AxisCalibJoint_NozzleNo1Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("1#吸嘴标定起始坐标")]
        public float[] AxisCalibNineAddTwo_NozzleNo1Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("2#吸嘴标定起始坐标")]
        public float[] AxisCalibNineAddTwo_NozzleNo2Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("3#吸嘴标定起始坐标")]
        public float[] AxisCalibNineAddTwo_NozzleNo3Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("4#吸嘴标定起始坐标")]
        public float[] AxisCalibNineAddTwo_NozzleNo4Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("下视觉拍照喷嘴物料1号坐标")]
        public float[] AxisDownCamera_No1Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("下视觉拍照喷嘴物料2号坐标")]
        public float[] AxisDownCamera_No2Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("下视觉拍照喷嘴物料3号坐标")]
        public float[] AxisDownCamera_No3Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("下视觉拍照喷嘴物料4号坐标")]
        public float[] AxisDownCamera_No4Pos
        {
            get; set;
        } = new float[4];

        [DisplayName("压力标定坐标")]
        public float[] CalibPressSensorPos
        {
            get; set;
        } = new float[4];

        [DisplayName("点检设置目标压力值")]
        public float SetDestPressValue
        {
            get; set;
        } = 5;

        [DisplayName("点检设置目标压力最大值")]
        public float SetDestPressMaxValue
        {
            get; set;
        } = 10;

        [DisplayName("点检设置压力缓冲距离")]
        public float SetBufferPressDis
        {
            get; set;
        } = 10;

        [DisplayName("点检设置压力缓冲速度")]
        public float SetBufferPressSpeed
        {
            get; set;
        } = 10;

        [DisplayName("点检设置压力位置偏差")]
        public float SetPressPosOffset
        {
            get; set;
        } = 10;

        [DisplayName("点检设置压力停留时间")]
        public float SetPressCompressTime
        {
            get; set;
        } = 10;


    }
}
