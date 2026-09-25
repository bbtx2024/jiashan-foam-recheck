namespace QA.Business.Define
{
    /// <summary>
    /// 视觉返回结果类型
    /// </summary>
    public enum EN_CamResult
    {
        空穴,
        OK,
        Mark定位失败,
        Tape缺失,
        Tape大离型纸未撕,
        Tape抓边失败,
        Tape贴装偏移,
        排线SN扫码失败,
    }
}
