namespace ProjectTrinity.Networking.Messages
{
    public struct MatchStartAckMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;

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

