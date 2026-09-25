namespace QA.Business.Interfaces
{
    public interface IScannerRechck : IComponent
    {
        bool SendData(string data, ref string receive, bool needreceive = true);
        bool SendData(byte[] data, byte[] receive, bool needreceive = true);
    }
}
