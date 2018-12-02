using System;
using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Networking.Messages;
using UniRx;

namespace ProjectTrinity.Networking
{
    public class AckedMessageHelper 
    {
        private struct AckMessageAwaitData
        {
            public byte AckMessageId { get; private set; }
            public IDisposable MessageReceiveDisposable { get; private set; }
            public IOutgoingMessage MessageToSend { get; private set; }
            public Action<byte[]> OnAckMessageReceivedCalback { get; private set; }
            public Int64 LastMessageToAckSendTimestamp { get; private set; }

            public AckMessageAwaitData(byte ackMessageId, IDisposable messageReceiveDisposable, IOutgoingMessage messageToSend, Action<byte[]> onAckMessageReceivedCalback)
            {
                this.AckMessageId = ackMessageId;
                this.MessageReceiveDisposable = messageReceiveDisposable;
                this.MessageToSend = messageToSend;
                this.OnAckMessageReceivedCalback = onAckMessageReceivedCalback;
                this.LastMessageToAckSendTimestamp = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
            }

            public void RefreshSendTime()
            {
                this.LastMessageToAckSendTimestamp = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
            }
        }

        private List<AckMessageAwaitData> pendingAckMessages = new List<AckMessageAwaitData>();
        private IUdpClient udpClient;

        public AckedMessageHelper(IUdpClient udpClient)
        {
            this.udpClient = udpClient;
        }

        // An acked messages expects an ack to be sent from the server, if the ack message is not received in time the message will be sent again.
        // optimize wait time by only sending message again when waiting time > rtt
        public void SendAckedMessage(IOutgoingMessage messageToSend, byte messageAckIdToWaitFor, Action<byte[]> onAckMessageReceivedCallback)
        {
            IDisposable messageReceiveDisposable = udpClient.OnMessageReceive
                .Where(message => message[0] == messageAckIdToWaitFor)
                .Subscribe(OnMessageReceived);

            pendingAckMessages.Add(new AckMessageAwaitData(messageAckIdToWaitFor, messageReceiveDisposable, messageToSend, onAckMessageReceivedCallback));

            udpClient.SendMessage(messageToSend.GetBytes());
        }

        private void OnMessageReceived(byte[] message)
        {
            for (int i = pendingAckMessages.Count - 1; i >= 0; i--)
            {
                if (pendingAckMessages[i].AckMessageId == message[0])
                {
                    AckMessageAwaitData ackedData = pendingAckMessages[i];

                    pendingAckMessages[i].MessageReceiveDisposable.Dispose();
                    pendingAckMessages.Remove(ackedData);

                    if (ackedData.OnAckMessageReceivedCalback != null)
                    {
                        ackedData.OnAckMessageReceivedCalback(message);
                    }

                    break;
                }
            }
        }

        public void OnFixedUpdateTick()
        {
            for (int i = 0; i < pendingAckMessages.Count; i++)
            {
                if((UtcTimestampHelper.GetCurrentUtcMsTimestamp() - pendingAckMessages[i].LastMessageToAckSendTimestamp) >= 100)
                {
                    udpClient.SendMessage(pendingAckMessages[i].MessageToSend.GetBytes());
                    pendingAckMessages[i].RefreshSendTime();
                }
            }
        }
    }
}