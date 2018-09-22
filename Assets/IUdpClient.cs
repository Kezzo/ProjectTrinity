namespace ProjectTrinity.Networking
{
    public interface IUdpClient
    {

        void RegisterListener(byte messageId, IUdpMessageListener listener);
    }
}

