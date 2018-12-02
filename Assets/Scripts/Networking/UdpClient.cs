using System;
using System.Collections.Generic;
using System.Net;
using ProjectTrinity.Root;
using UniRx;

namespace ProjectTrinity.Networking
{
    public class UdpClient : IUdpClient
    {
        private Subject<byte[]> OnMessageReceivedSubject;
        public IObservable<byte[]> OnMessageReceive {
            get
            {
                return OnMessageReceivedSubject;
            }
        }

        private Queue<byte[]> messageSendQueue;
        private bool isSendingMessage;

        System.Net.Sockets.UdpClient udpClient;

        private IPEndPoint ipEndpoint;

        public UdpClient(string host, int hostPort)
        {
            ipEndpoint = new IPEndPoint(IPAddress.Parse(host), hostPort);

            OnMessageReceivedSubject = new Subject<byte[]>();
            messageSendQueue = new Queue<byte[]>();

            try
            {
                udpClient = new System.Net.Sockets.UdpClient();
                ListenForNextMessage();
            }
            catch(Exception e)
            {
                DIContainer.Logger.Error(e.ToString());
            }
        }

        ~UdpClient()
        {
            OnMessageReceivedSubject.OnCompleted();
            udpClient.Dispose();
        }

        private void ListenForNextMessage()
        {
            udpClient.BeginReceive(OnMessageReceived, null);
        }

        private void OnMessageReceived(IAsyncResult result)
        {
            OnMessageReceivedSubject.OnNext(udpClient.EndReceive(result, ref ipEndpoint));
            ListenForNextMessage();
        }

        public void SendMessage(byte[] messageToSend)
        {
            if(isSendingMessage)
            {
                messageSendQueue.Enqueue(messageToSend);
                return;
            }

            isSendingMessage = true;
            udpClient.BeginSend(messageToSend, messageToSend.Length, ipEndpoint, OnMessageSent, null);
        }

        private void OnMessageSent(IAsyncResult result)
        {
            udpClient.EndSend(result);

            if (messageSendQueue.Count == 0)
            {
                isSendingMessage = false;
                return;
            }
            
            byte[] nextMessage = messageSendQueue.Dequeue();
            udpClient.BeginSend(nextMessage, nextMessage.Length, ipEndpoint, OnMessageSent, null);
        }
    }
}