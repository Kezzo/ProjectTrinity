namespace ProjectTrinity.Networking.Messages
{
    public struct SpellInputMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;
        public readonly byte SpellId;
        public readonly byte Rotation;
        public readonly byte StartFrame;

        public SpellInputMessage(byte unitId, byte spellId, byte rotation, byte startFrame)
        {
            PlayerId = unitId;
            SpellId = spellId;
            Rotation = rotation;
            StartFrame = startFrame;
        }

        public byte[] GetBytes()
        {
            return new byte[] { MessageId.SPELL_INPUT, PlayerId, SpellId, Rotation, StartFrame };
        }
    }
}

