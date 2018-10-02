using System;
using System.Collections.Generic;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

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
            var timeSynchRequest = new TimeSynchRequestMessage((DateTime.UtcNow - UtcStartDateTime).Ticks);
            udpClient.SendMessage(timeSynchRequest.GetBytes());
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

            receivedOffSets.Add(new TimeSynchResponseMessage(message).GetServerTimeOffset());
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
                udpClient.DeregisterListener(MessageId.TIME_RESP, this);
                DIContainer.Logger.Debug(string.Format("Server synched time is: {0} offset was: {1} ms", NetworkDateTime.ToString(), offset.TotalMilliseconds.ToString()));
                currentOnTimeSynchedCallback();
            } 
            else 
            {
                SendTimeSynchMessage();
            }
        }
    }
}