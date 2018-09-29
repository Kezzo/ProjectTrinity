namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSyncDoneMessage : IOutgoingMessage
    {
        public byte[] GetBytes()
        {
            return new byte[] { MessageId.TIME_SYNC_DONE };
        }
    }
}

