namespace ProjectTrinity.Networking.Messages
{
    public struct MatchEndAckMessage
    {
        public readonly byte PlayerId;

        public MatchEndAckMessage(byte[] buffer)
        {
            PlayerId = buffer[1];
        }

        public MatchEndAckMessage(byte playerId)
        {
            PlayerId = playerId;
        }

        public byte[] GetBytes()
        {
            return new byte[] { MessageId.MATCH_END_ACK, PlayerId };
        }
    }
}

