using ProjectTrinity.Helper;
using ProjectTrinity.Networking;

namespace ProjectTrinity.Root
{
    public static class DIContainer
    {
        private static IUdpClient udpClient;
        public static IUdpClient UDPClient
        {
            get//"127.0.0.1
            {
                return udpClient != null ? udpClient : (udpClient = new UdpClient("ec2-34-248-140-154.eu-west-1.compute.amazonaws.com", 2448, 1337));
            }
        }

        private static NetworkTimeService networkTimeService;
        public static NetworkTimeService NetworkTimeService
        {
            get
            {
                return networkTimeService != null ? networkTimeService : (networkTimeService = new NetworkTimeService());
            }
        }

        private static AckedMessageHelper ackedMessageHelper;
        public static AckedMessageHelper AckedMessageHelper
        {
            get
            {
                return ackedMessageHelper != null ? ackedMessageHelper : (ackedMessageHelper = new AckedMessageHelper());
            }
        }

        private static ILogger logger;
        public static ILogger Logger
        {
            get
            {
                return logger != null ? logger : (logger = new UnityLogger());
            }
        }
    }
}

