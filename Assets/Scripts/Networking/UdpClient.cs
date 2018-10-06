using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProjectTrinity.Root;

namespace ProjectTrinity.Networking
{
    public class UdpClient : IUdpClient
    {
        private readonly Queue<byte[]> messageSendQueue;
        private readonly Dictionary<byte, IUdpMessageListener> listeners;
        System.Net.Sockets.UdpClient udpClient;

        public UdpClient(string host, int hostPort)
        {
            messageSendQueue = new Queue<byte[]>();
            listeners = new Dictionary<byte, IUdpMessageListener>();

            try
            {
                udpClient = new System.Net.Sockets.UdpClient();
                StartListening();
                Task.Run(async () =>
                {
                    while(true)
                    {
                        while(messageSendQueue.Count == 0)
                        {

                        }

                        byte[] messageToSend = messageSendQueue.Dequeue();

                        //DIContainer.Logger.Debug(string.Format("UdpClient: Sending message of size: {0} with messageId: {1}", messageToSend.Length, messageToSend[0]));
                        await udpClient.SendAsync(messageToSend, messageToSend.Length, host, hostPort);
                    }
                });
            }
            catch(Exception e)
            {
                DIContainer.Logger.Error(e.ToString());
            }
        }

        ~UdpClient()
        {
            udpClient.Dispose();
        }

        // will forward messages with the given message id to that listener
        public void RegisterListener(byte messageId, IUdpMessageListener listener)
        {
            listeners[messageId] = listener;
        }

        public void DeregisterListener(byte messageId, IUdpMessageListener listener)
        {
            // only remove when the given listener is the current
            IUdpMessageListener currentListener;
            if(listeners.TryGetValue(messageId, out currentListener))
            {
                if(currentListener.Equals(listener))
                {
                    listeners[messageId] = null;
                }
            }
        }

        private void StartListening()
        {
            DIContainer.Logger.Debug("UdpClient: Start listening for messages");

            Task.Run(async () =>
            {
                while (true)
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();

                    //DIContainer.Logger.Debug(string.Format("UdpClient: Received message of size: {0} with messageId: {1}", result.Buffer.Length, result.Buffer[0]));

                    if (listeners.ContainsKey(result.Buffer[0]) && listeners[result.Buffer[0]] != null)
                    {
                        listeners[result.Buffer[0]].OnMessageReceived(result.Buffer);
                    }
                } 
            });
        }

        public void SendMessage(byte[] messageToSend, Action onMessageSent = null)
        {
            messageSendQueue.Enqueue(messageToSend);
        }
    }
}