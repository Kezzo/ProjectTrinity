using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct UnitStateMessage
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

        public UnitStateMessage(byte unitId, int xPosition, int yPosition, byte rotation, byte frame)
        {
            UnitId = unitId;
            XPosition = xPosition;
            YPosition = yPosition;
            Rotation = rotation;
            Frame = frame;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];
            bytes[0] = MessageId.UNIT_STATE;
            bytes[1] = UnitId;

            Array.Copy(BitConverter.GetBytes(XPosition), 0, bytes, 2, 4);
            Array.Copy(BitConverter.GetBytes(YPosition), 0, bytes, 6, 4);

            bytes[10] = Rotation;
            bytes[11] = Frame;

            return bytes;
        }
    }
}

