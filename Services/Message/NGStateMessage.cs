using QA.Business.Define;

namespace QA.Business.Message
{
    public struct NGStateMessage
    {
        public EN_TrayStatus[,] Status { get; set; }
        public string[] CarrierSns { get; set; }
    }
}
