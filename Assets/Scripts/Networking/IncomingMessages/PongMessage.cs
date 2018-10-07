using System;
using ProjectTrinity.Root;

namespace ProjectTrinity.Networking.Messages
{
    public class PongMessage : IIncomingMessage
    {
        public readonly Int64 PingTransmissionTimestamp;
        public readonly Int64 RoundTripTime;

        public PongMessage(byte[] buffer)
        {
            PingTransmissionTimestamp = BitConverter.ToInt64(buffer, 1);
            RoundTripTime = DIContainer.NetworkTimeService.NetworkTimestampMs - PingTransmissionTimestamp;
        }
    }
}