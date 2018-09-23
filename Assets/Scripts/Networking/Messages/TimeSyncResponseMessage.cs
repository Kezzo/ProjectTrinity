using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSynchResponseMessage
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

        public TimeSynchResponseMessage(Int64 transmissionTimestamp, Int64 serverReceptionTimestamp, Int64 serverTransmissionTimestamp, Int64 receptionTimestamp)
        {
            TransmissionTimestamp = transmissionTimestamp;
            ServerReceptionTimestamp = serverReceptionTimestamp;
            ServerTransmissionTimestamp = serverTransmissionTimestamp;
            ReceptionTimestamp = receptionTimestamp;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[33];
            bytes[0] = MessageId.TIME_RESP;

            Array.Copy(BitConverter.GetBytes(TransmissionTimestamp), 0, bytes, 1, 8);
            Array.Copy(BitConverter.GetBytes(ServerReceptionTimestamp), 0, bytes, 9, 8);
            Array.Copy(BitConverter.GetBytes(ServerTransmissionTimestamp), 0, bytes, 17, 8);
            Array.Copy(BitConverter.GetBytes(ReceptionTimestamp), 0, bytes, 25, 8);

            return bytes;
        }

        public TimeSpan GetServerTimeOffset() 
        {
            return TimeSpan.FromTicks(((ServerReceptionTimestamp - TransmissionTimestamp) + (ServerTransmissionTimestamp - ReceptionTimestamp)) / 2);
        }
    }
}

