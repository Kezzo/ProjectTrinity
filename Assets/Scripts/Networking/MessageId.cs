namespace ProjectTrinity.Networking
{
    public static class MessageId
    {
        public static readonly byte PING_REQ = 0;
        public static readonly byte PING_RESP = 1;

        public static readonly byte TIME_REQ = 2;
        public static readonly byte TIME_RESP = 3;

        public static readonly byte INPUT = 4;
        public static readonly byte STATE = 5;
    }
}