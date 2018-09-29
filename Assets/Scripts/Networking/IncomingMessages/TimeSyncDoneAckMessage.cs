namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSyncDoneAckMessage : IIncomingMessage
    {
        public readonly byte PlayerId;

        public TimeSyncDoneAckMessage(byte[] buffer)
        {
            PlayerId = buffer[1];
        }
    }
}

