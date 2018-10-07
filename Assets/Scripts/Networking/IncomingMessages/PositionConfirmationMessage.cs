using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct PositionConfirmationMessage : IIncomingMessage
    {
        public readonly byte UnitId;
        public readonly int XPosition;
        public readonly int YPosition;
        public readonly byte Frame;

        public PositionConfirmationMessage(byte[] buffer) 
        {
            UnitId = buffer[1];
            XPosition = BitConverter.ToInt32(buffer, 2);
            YPosition = BitConverter.ToInt32(buffer, 6);
            Frame = buffer[10];
        }
    }
}

