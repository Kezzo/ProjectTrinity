namespace ProjectTrinity.Networking.Messages
{
    public struct MatchStartAckMessage
    {
        public readonly byte PlayerId;

        public MatchStartAckMessage(byte[] buffer)
        {
            PlayerId = buffer[1];
        }

        public MatchStartAckMessage(byte playerId)
        {
            PlayerId = playerId;
        }

        public byte[] GetBytes()
        {
            return new byte[] { MessageId.MATCH_START_ACK, PlayerId };
        }
    }
}

