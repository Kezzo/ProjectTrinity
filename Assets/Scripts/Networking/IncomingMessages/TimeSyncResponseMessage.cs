using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSynchResponseMessage : IIncomingMessage
    {
        private static readonly DateTime UtcStartDateTime = new DateTime(1970, 1, 1);

        public readonly Int64 TransmissionTimestamp;
        public readonly Int64 ServerReceptionTimestamp;
        public readonly Int64 ServerTransmissionTimestamp;
        public readonly Int64 ReceptionTimestamp;

        public TimeSynchResponseMessage(byte[] buffer)
        {
            ReceptionTimestamp = (DateTime.UtcNow - UtcStartDateTime).Ticks;

            TransmissionTimestamp = BitConverter.ToInt64(buffer, 1);

            ServerReceptionTimestamp = BitConverter.ToInt64(buffer, 9);
            ServerTransmissionTimestamp = BitConverter.ToInt64(buffer, 17);
        }

        public TimeSpan GetServerTimeOffset() 
        {
            return TimeSpan.FromTicks(((ServerReceptionTimestamp - TransmissionTimestamp) + (ServerTransmissionTimestamp - ReceptionTimestamp)) / 2);
        }
    }
}

