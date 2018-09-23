﻿namespace ProjectTrinity.Networking
{
    public static class MessageId
    {
        public static readonly byte PING_REQ = 0;
        public static readonly byte PING_RESP = 1;

        public static readonly byte TIME_REQ = 2;
        public static readonly byte TIME_RESP = 3;

        public static readonly byte MATCH_START = 4;
        public static readonly byte MATCH_START_ACK = 5;

        public static readonly byte INPUT = 6;
        public static readonly byte UNIT_STATE = 7;

        public static readonly byte MATCH_END = 8;
        public static readonly byte MATCH_END_ACK = 9;
    }
}