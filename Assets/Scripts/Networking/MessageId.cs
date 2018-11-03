namespace ProjectTrinity.Networking
{
    public static class MessageId
    {
        public static readonly byte PING_REQ = 0;
        public static readonly byte PING_RESP = 1;

        public static readonly byte TIME_REQ = 2;
        public static readonly byte TIME_RESP = 3;

        public static readonly byte TIME_SYNC_DONE = 4;
        public static readonly byte TIME_SYNC_DONE_ACK = 5;

        public static readonly byte MATCH_START = 6;
        public static readonly byte MATCH_START_ACK = 7;

        public static readonly byte INPUT = 8;
        public static readonly byte UNIT_STATE = 9;

        public static readonly byte MATCH_END = 10;
        public static readonly byte MATCH_END_ACK = 11;

        public static readonly byte POSITION_CONFIRMATION = 12;

        public static readonly byte Ability_INPUT = 13;
        public static readonly byte UNIT_Ability_ACTIVATION = 14;

    }
}