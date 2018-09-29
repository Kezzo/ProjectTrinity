using System;
using System.Collections.Generic;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

namespace ProjectTrinity.Networking
{
    public class AckedMessageHelper : IUdpMessageListener
    {
        private struct AckMessageAwaitData
        {
            public byte AckMessageId;
            public Action<byte[]> OnAckMessageReceivedCalback;
            private Int64 LastMessageToAckSendTimestamp;

            public AckMessageAwaitData (byte ackMessageId, Action<byte[]> onAckMessageReceivedCalback)
            {
                this.AckMessageId = ackMessageId;
                this.OnAckMessageReceivedCalback = onAckMessageReceivedCalback;
                this.LastMessageToAckSendTimestamp = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
            }

            public void RefreshSendTime()
            {
                this.LastMessageToAckSendTimestamp = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
            }
        }

        private List<AckMessageAwaitData> pendingAckMessages = new List<AckMessageAwaitData>();

        // An acked messages expects an ack to be sent from the server, if the ack message is not received in time the message will be sent again.
        // optimize wait time by only sending message again when waiting time > rtt
        public void SendAckedMessage(IOutgoingMessage messageToSend, byte messageAckIdToWaitFor, Action<byte[]> onAckMessageReceivedCallback) 
        {
            pendingAckMessages.Add(new AckMessageAwaitData(messageAckIdToWaitFor, onAckMessageReceivedCallback));
            DIContainer.UDPClient.RegisterListener(messageAckIdToWaitFor, this);
            DIContainer.UDPClient.SendMessage(messageToSend.GetBytes());
        }

        public void OnMessageReceived(byte[] message)
        {
            AckMessageAwaitData completedAck;

            foreach (var pendingAckMessage in pendingAckMessages)
            {
                if(pendingAckMessage.AckMessageId == message[0]) {
                    completedAck = pendingAckMessage;
                    break;
                }
            }

            for (int i = pendingAckMessages.Count - 1; i >= 0; i--)
            {
                if (pendingAckMessages[i].AckMessageId == message[0])
                {
                    AckMessageAwaitData ackedData = pendingAckMessages[i];
                    pendingAckMessages.Remove(ackedData);

                    if(ackedData.OnAckMessageReceivedCalback != null) 
                    {
                        ackedData.OnAckMessageReceivedCalback(message);
                    }

                    break;
                }
            }
        }

        public void OnFixedUpdateTick()
        {

        }
    }
}