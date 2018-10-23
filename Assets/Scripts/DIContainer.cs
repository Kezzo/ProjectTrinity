using ProjectTrinity.Helper;

namespace ProjectTrinity.Root
{
    public static class DIContainer
    {
        private static ILogger logger;
        public static ILogger Logger
        {
            get
            {
                return logger != null ? logger : (logger = new UnityLogger());
            }
        }

        private static ISerializer serializer;
        public static ISerializer Serializer
        {
            get
            {
                return serializer != null ? serializer : (serializer = new JsonSerializer());
            }
        }
    }
}