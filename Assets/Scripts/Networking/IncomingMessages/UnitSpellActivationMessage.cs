using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct UnitSpellActivationMessage : IIncomingMessage
    {
        public readonly byte UnitId;
        public readonly byte SpellId;
        public readonly byte Rotation;
        public readonly byte StartFrame;
        public readonly byte ActivationFrame;

        public UnitSpellActivationMessage(byte[] buffer)
        {
            UnitId = buffer[1];
            SpellId = buffer[2];
            Rotation = buffer[3];
            StartFrame = buffer[4];
            ActivationFrame = buffer[5];
        }
    }
}

