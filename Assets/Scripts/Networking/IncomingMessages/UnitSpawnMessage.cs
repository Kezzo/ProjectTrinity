using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct UnitSpawnMessage : IIncomingMessage
    {
        public readonly byte UnitId;
        public readonly byte TeamId;
        public readonly byte UnitType;
        public readonly int XPosition;
        public readonly int YPosition;
        public readonly byte Rotation;
        public readonly byte HealthPercent;
        public readonly byte Frame;

        public UnitSpawnMessage(byte[] buffer) 
        {
            UnitId = buffer[1];
            TeamId = buffer[2];
            UnitType = buffer[3];
            XPosition = BitConverter.ToInt32(buffer, 4);
            YPosition = BitConverter.ToInt32(buffer, 8);
            Rotation = buffer[12];
            HealthPercent = buffer[13];
            Frame = buffer[14];
        }
    }
}

