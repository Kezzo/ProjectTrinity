namespace ProjectTrinity.Networking.Messages
{
    public struct AbilityInputMessage : IOutgoingMessage
    {
        public readonly byte PlayerId;
        public readonly byte AbilityId;
        public readonly byte Rotation;
        public readonly byte StartFrame;

        public AbilityInputMessage(byte unitId, byte abilityId, byte rotation, byte startFrame)
        {
            PlayerId = unitId;
            AbilityId = abilityId;
            Rotation = rotation;
            StartFrame = startFrame;
        }

        public byte[] GetBytes()
        {
            return new byte[] { MessageId.ABILITY_INPUT, PlayerId, AbilityId, Rotation, StartFrame };
        }
    }
}

