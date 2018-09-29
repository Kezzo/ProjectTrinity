using ProjectTrinity.Networking;

namespace ProjectTrinity.Root
{
    public static class DIContainer
    {
        private static IUdpClient udpClient;
        public static IUdpClient UDPClient
        {
            get
            {
                return udpClient != null ? udpClient : (udpClient = new UdpClient("34.253.150.89", 2448, 1337));
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
    }
}

