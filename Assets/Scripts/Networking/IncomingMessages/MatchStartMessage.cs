using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct MatchStartMessage : IIncomingMessage
    {
        public readonly Int64 MatchStartTimestamp;

        public MatchStartMessage(byte[] buffer)
        {
            MatchStartTimestamp = BitConverter.ToInt64(buffer, 1);
        }
    }
}

