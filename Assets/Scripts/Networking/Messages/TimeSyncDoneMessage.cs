namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSyncDoneMessage
    {
        public readonly byte PlayerId;

        public TimeSyncDoneMessage(byte[] buffer)
        {
            PlayerId = buffer[1];
        }

        public TimeSyncDoneMessage(byte playerId)
        {
            PlayerId = playerId;
        }

        public byte[] GetBytes()
        {
            return new byte[] { MessageId.TIME_SYNC_DONE, PlayerId };
        }
    }
}

