﻿using ProjectTrinity.Helper;
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
                return udpClient != null ? udpClient : (udpClient = new UdpClient(EnvironmentHelper.ServerUrl, 2448));
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

        private static RoundTripTimeService roundTripTimeService;
        public static RoundTripTimeService RoundTripTimeService
        {
            get
            {
                return roundTripTimeService != null ? roundTripTimeService : (roundTripTimeService = new RoundTripTimeService());
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