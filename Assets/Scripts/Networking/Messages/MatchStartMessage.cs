using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct MatchStartMessage
    {
        public readonly Int64 MatchStartTimestamp;

        public MatchStartMessage(byte[] buffer)
        {
            MatchStartTimestamp = BitConverter.ToInt64(buffer, 1);
        }

        public MatchStartMessage(Int64 matchStartTimestamp)
        {
            MatchStartTimestamp = matchStartTimestamp;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[9];
            bytes[0] = MessageId.MATCH_START;
            Array.Copy(BitConverter.GetBytes(MatchStartTimestamp), 0, bytes, 1, 8);

            return bytes;
        }
    }
}

