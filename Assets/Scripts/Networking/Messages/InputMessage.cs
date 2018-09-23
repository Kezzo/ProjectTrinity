namespace ProjectTrinity.Networking.Messages
{
    public struct InputMessage
    {
        public readonly byte UnitId;
        public readonly byte Translation;
        public readonly byte Rotation;
        public readonly byte Frame;

        public InputMessage(byte[] buffer)
        {
            UnitId = buffer[1];
            Translation = buffer[2];
            Rotation = buffer[3];
            Frame = buffer[4];
        }

        public InputMessage(byte unitId, byte translation, byte rotation, byte frame)
        {
            UnitId = unitId;
            Translation = translation;
            Rotation = rotation;
            Frame = frame;
        }

        public byte[] GetBytes() {
            return new byte[] { MessageId.INPUT, UnitId, Translation, Rotation, Frame };
        }
    }
}

