namespace QA.Business.Define
{
    /// <summary>
    /// 载具上每个穴位状态
    /// </summary>
    public enum EN_TrayStatus
    {
        空穴,
        OK,
        MES上传NG,
        Tape缺失,
        Tape大离型纸未撕,
        Tape贴装偏移 = 6,
        Mark定位失败,
        Tape抓边失败,
        排线SN扫码失败 = 13,

        禁用,
        待料,
        等待处理,
        视觉失连,
        拍照完成
    }
}
