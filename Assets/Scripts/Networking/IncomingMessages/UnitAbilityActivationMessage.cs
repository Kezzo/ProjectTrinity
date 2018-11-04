using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct UnitAbilityActivationMessage : IIncomingMessage
    {
        public readonly byte UnitId;
        public readonly byte AbilityId;
        public readonly byte Rotation;
        public readonly byte StartFrame;
        public readonly byte ActivationFrame;
        public readonly byte EndFrame;

        public UnitAbilityActivationMessage(byte[] buffer)
        {
            UnitId = buffer[1];
            AbilityId = buffer[2];
            Rotation = buffer[3];
            StartFrame = buffer[4];
            ActivationFrame = buffer[5];
            EndFrame = buffer[6];
        }
    }
}

