namespace ProjectTrinity.Networking
{
    public interface IUdpMessageListener
    {
        void OnMessageReceived(byte[] message);
    }
}
