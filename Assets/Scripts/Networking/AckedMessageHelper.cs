using System;
using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Networking.Messages;

namespace ProjectTrinity.Networking
{
    public class AckedMessageHelper : IUdpMessageListener 
    {
        private struct AckMessageAwaitData
        {
            public byte AckMessageId { get; private set; }
            public IOutgoingMessage MessageToSend { get; private set; }
            public Action<byte[]> OnAckMessageReceivedCalback { get; private set; }
            public Int64 LastMessageToAckSendTimestamp { get; private set; }

            public AckMessageAwaitData(byte ackMessageId, IOutgoingMessage messageToSend, Action<byte[]> onAckMessageReceivedCalback)
            {
                this.AckMessageId = ackMessageId;
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
            pendingAckMessages.Add(new AckMessageAwaitData(messageAckIdToWaitFor, messageToSend, onAckMessageReceivedCallback));
            udpClient.RegisterListener(messageAckIdToWaitFor, this);
            udpClient.SendMessage(messageToSend.GetBytes());
        }

        public void OnMessageReceived(byte[] message)
        {
            for (int i = pendingAckMessages.Count - 1; i >= 0; i--)
            {
                if (pendingAckMessages[i].AckMessageId == message[0])
                {
                    AckMessageAwaitData ackedData = pendingAckMessages[i];

                    udpClient.DeregisterListener(pendingAckMessages[i].AckMessageId, this);
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