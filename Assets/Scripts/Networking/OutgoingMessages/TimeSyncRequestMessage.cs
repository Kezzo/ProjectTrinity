﻿using System;

namespace ProjectTrinity.Networking.Messages
{
    public struct TimeSynchRequestMessage : IOutgoingMessage
    {
        public readonly Int64 TransmissionTimestamp;

        public TimeSynchRequestMessage(Int64 transmissionTimestamp)
        {
            TransmissionTimestamp = transmissionTimestamp;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[9];
            bytes[0] = MessageId.TIME_REQ;
            Array.Copy(BitConverter.GetBytes(TransmissionTimestamp), 0, bytes, 1, 8);

            return bytes;
        }
    }
}

