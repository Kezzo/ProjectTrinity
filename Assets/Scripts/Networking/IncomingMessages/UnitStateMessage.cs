using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct UnitStateMessage : IIncomingMessage
    {
        public readonly byte UnitId;
        public readonly int XPosition;
        public readonly int YPosition;
        public readonly byte Rotation;
        public readonly byte Frame;

        public UnitStateMessage(byte[] buffer) 
        {
            UnitId = buffer[1];
            XPosition = BitConverter.ToInt32(buffer, 2);
            YPosition = BitConverter.ToInt32(buffer, 6);
            Rotation = buffer[10];
            Frame = buffer[11];
        }
    }
}

