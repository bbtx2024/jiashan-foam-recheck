using QA.Business.Define;

namespace QA.Business.Message
{
    public struct CarrierInfoPanelMessage
    {
        public int Cavity { get; set; }

        public EN_TrayStatus Status { get; set; }

        public string XStr { get; set; }

        public string YStr { get; set; }

        public string RStr { get; set; }
    };
}
