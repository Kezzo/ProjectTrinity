namespace ProjectTrinity.Networking.Messages
{
    public struct InputMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;
        public readonly byte XTranslation;
        public readonly byte YTranslation;
        public readonly byte Rotation;
        public readonly byte Frame;

        public InputMessage(byte unitId, byte xtranslation, byte ytranslation, byte rotation, byte frame)
        {
            PlayerId = unitId;
            XTranslation = xtranslation;
            YTranslation = ytranslation;
            Rotation = rotation;
            Frame = frame;
        }

        public byte[] GetBytes() {
            return new byte[] { MessageId.INPUT, PlayerId, XTranslation, YTranslation, Rotation, Frame };
        }
    }
}

