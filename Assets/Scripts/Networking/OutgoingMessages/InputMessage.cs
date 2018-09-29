namespace ProjectTrinity.Networking.Messages
{
    public struct InputMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;
        public readonly byte Translation;
        public readonly byte Rotation;
        public readonly byte Frame;

        public InputMessage(byte unitId, byte translation, byte rotation, byte frame)
        {
            PlayerId = unitId;
            Translation = translation;
            Rotation = rotation;
            Frame = frame;
        }

        public byte[] GetBytes() {
            return new byte[] { MessageId.INPUT, PlayerId, Translation, Rotation, Frame };
        }
    }
}

