using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ProjectTrinity.Networking
{
    public class UdpClient : IUdpClient
    {
        private readonly Socket socket;
        private SocketAsyncEventArgs sendSocketEventArgs;
        private SocketAsyncEventArgs receiveSocketEventArgs;
        private bool messageSendingInProgress;

        private readonly Queue<byte[]> messageSendQueue;

        private readonly Dictionary<byte, IUdpMessageListener> listeners;

        public UdpClient(string host, int hostPort, int listenPort)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress broadcast = IPAddress.Parse("127.0.0.1");
            IPEndPoint srcIpEndPoint = new IPEndPoint(broadcast, listenPort);
            socket.Bind(srcIpEndPoint);

            sendSocketEventArgs = new SocketAsyncEventArgs();
            sendSocketEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(host), hostPort);
            sendSocketEventArgs.Completed += (o, eventArgs) =>
            {
                messageSendingInProgress = false;
                if (messageSendQueue.Count > 0)
                {
                    SendMessage(messageSendQueue.Dequeue());
                }
            };

            messageSendQueue = new Queue<byte[]>();

            listeners = new Dictionary<byte, IUdpMessageListener>();

            StartListening();
        }

        ~UdpClient()
        {
            sendSocketEventArgs.Dispose();
            receiveSocketEventArgs.Dispose();

        }


        // will forward messages with the given message id to that listener
        public void RegisterListener(byte messageId, IUdpMessageListener listener)
        {
            listeners[messageId] = listener;
        }

        private void StartListening()
        {
            Debug.Log("UdpClient: Start listening for messages");

            byte[] buffer = new byte[100];
            receiveSocketEventArgs = new SocketAsyncEventArgs();
            receiveSocketEventArgs.SetBuffer(buffer, 0, buffer.Length);

            receiveSocketEventArgs.Completed += (o, eventArgs) =>
            {
                Debug.LogFormat("UdpClient: Received message of size: {0} with messageId: {1}", eventArgs.BytesTransferred, eventArgs.Buffer[0]);

                if (listeners.ContainsKey(eventArgs.Buffer[0]))
                {
                    listeners[eventArgs.Buffer[0]].OnMessageReceived(eventArgs.Buffer);
                }

                // listen for next packet
                socket.ReceiveAsync(receiveSocketEventArgs);
            };

            // kick-off listening chain
            socket.ReceiveAsync(receiveSocketEventArgs);
        }

        public void SendMessage(byte[] messageToSend, Action onMessageSent = null)
        {
            if (messageSendingInProgress)
            {
                messageSendQueue.Enqueue(messageToSend);
                return;
            }

            sendSocketEventArgs.SetBuffer(messageToSend, 0, messageToSend.Length);
            socket.SendToAsync(sendSocketEventArgs);
            messageSendingInProgress = true;
        }
    }
}