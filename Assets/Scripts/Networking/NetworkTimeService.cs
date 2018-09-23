using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectTrinity.Networking
{
    public class NetworkTimeService : IUdpMessageListener
    {
        private TimeSpan offset;
        private List<TimeSpan> receivedOffSets = new List<TimeSpan>();
        private readonly DateTime UtcStartDateTime = new DateTime(1970, 1, 1);

        private Action currentOnTimeSynchedCallback;
        private int responsesReceived;

        private IUdpClient udpClient;

        public DateTime NetworkDateTime
        {
            get
            {
                return DateTime.UtcNow + offset;
            }
        }

        public long NetworkTimestamp
        {
            get
            {
                return (long)NetworkDateTime.Subtract(UtcStartDateTime).TotalSeconds;
            }
        }

        public long NetworkTimestampMs
        {
            get
            {
                return (long)NetworkDateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }

        public void Synch(IUdpClient udpClient, Action onTimeSynched = null)
        {
            currentOnTimeSynchedCallback = onTimeSynched;
            responsesReceived = 0;

            this.udpClient = udpClient;
            udpClient.RegisterListener(MessageId.TIME_RESP, this);

            SendTimeSynchMessage();
        }

        private void SendTimeSynchMessage() 
        {
            byte[] timeSynchBuffer = new byte[25];
            timeSynchBuffer[0] = MessageId.TIME_REQ;

            Int64 currentTickTimestamp = (DateTime.UtcNow - UtcStartDateTime).Ticks;
            // 1 byte = message id
            // 8 bytes = transmission timestamp
            // 8 bytes = server reception timestamp
            // 8 bytes = server transmission timestamp
            byte[] currentTimestampInBytes = BitConverter.GetBytes(currentTickTimestamp);
            Array.Copy(currentTimestampInBytes, 0, timeSynchBuffer, 1, 8);

            udpClient.SendMessage(timeSynchBuffer);
        }

        public void OnMessageReceived(byte[] message)
        {
            if(receivedOffSets.Count >= responsesReceived+1) 
            {
                // already received all responses
                return;
            }

            if(responsesReceived == 0)
            {
                // first message resets old offset
                offset = new TimeSpan();
            }

            receivedOffSets.Add(GetOffsetFromMessage(message));
            responsesReceived++;
            
            for (int i = 0; i < receivedOffSets.Count; i++)
            {
                if(i == 0)
                {
                    offset = receivedOffSets[i];
                }
                else
                {
                    offset += receivedOffSets[i];
                }
            }

            offset = TimeSpan.FromMilliseconds(offset.TotalMilliseconds / responsesReceived);

            if(responsesReceived >= 3 && currentOnTimeSynchedCallback != null) 
            {
                Debug.LogFormat("Server synched time is: {0} offset was: {1} ms", NetworkDateTime.ToString(), offset.TotalMilliseconds.ToString());
                currentOnTimeSynchedCallback();
            } 
            else 
            {
                SendTimeSynchMessage();
            }
        }

        private TimeSpan GetOffsetFromMessage(byte[] message)
        {
            // 1 byte = message id
            // 8 bytes = transmission timestamp
            // 8 bytes = server reception timestamp
            // 8 bytes = server transmission timestamp
            long transmissionTimestamp = BitConverter.ToInt64(message, 1);

            long serverReceptionTimestamp = BitConverter.ToInt64(message, 9);
            long serverTransmissionTimestamp = BitConverter.ToInt64(message, 17);

            long receptionTimestamp = (DateTime.UtcNow - UtcStartDateTime).Ticks;

            return TimeSpan.FromTicks(((serverReceptionTimestamp - transmissionTimestamp) + (serverTransmissionTimestamp - receptionTimestamp)) / 2);
        }
    }
}