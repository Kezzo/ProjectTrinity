using System;

namespace ProjectTrinity.Networking
{
    public interface IUdpClient
    {
        IObservable<byte[]> OnMessageReceive { get; }
        void SendMessage(byte[] messageToSend);
    }
}

