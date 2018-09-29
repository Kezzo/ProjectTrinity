namespace ProjectTrinity.Networking.Messages
{
    public struct MatchEndAckMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;

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

