using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace ProjectTrinity.Networking
{
    public class UdpClient : IUdpClient
    {
        private readonly Socket socket;
        SocketAsyncEventArgs sendSocketEventArgs;
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
        }


        // will forward messages with the given message id to that listener
        public void RegisterListener(byte messageId, IUdpMessageListener listener)
        {
            listeners[messageId] = listener;
        }

        public void StartListening(Socket s)
        {
            Debug.Log("UdpClient: Start listening for messages");

            byte[] buffer = new byte[100];
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.SetBuffer(buffer, 0, 100);

            socketAsyncEventArgs.Completed += (o, eventArgs) =>
            {
                Debug.LogFormat("UdpClient: Received message of size: {0} with messageId: {1}", eventArgs.BytesTransferred, eventArgs.Buffer[0]);

                if (listeners.ContainsKey(eventArgs.Buffer[0]))
                {
                    listeners[eventArgs.Buffer[0]].OnMessageReceived(eventArgs.Buffer);
                }

                // listen for next packet
                s.ReceiveAsync(socketAsyncEventArgs);
            };

            // kick-off listening chain
            s.ReceiveAsync(socketAsyncEventArgs);
        }

        private void SendMessage(byte[] messageToSend)
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