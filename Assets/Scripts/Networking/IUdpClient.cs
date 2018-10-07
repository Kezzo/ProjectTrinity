using System;

namespace ProjectTrinity.Networking
{
    public interface IUdpClient
    {
        void RegisterListener(byte messageId, IUdpMessageListener listener);
        void DeregisterListener(byte messageId, IUdpMessageListener listener);
        void SendMessage(byte[] messageToSend, Action onMessageSent = null);
    }
}

