using UnityEngine;

namespace ProjectTrinity.Helper
{
    public class EnvironmentHelper : MonoBehaviour
    {
        public enum Environment
        {
            LOCAL = 0,
            DEV = 1,
            LIVE = 2
        }

        [SerializeField]
        private Environment environment;
        public static Environment StaticEnvironment;

        private void Awake()
        {
            StaticEnvironment = environment;
        }

        public static string HTTPServerUrl
        {
            get
            {
                switch (StaticEnvironment)
                {
                    case Environment.LOCAL:
                        return "http://127.0.0.1:8080";
                    case Environment.DEV:
                        return "http://dev-trinity-web-server-alb-2023057386.eu-west-1.elb.amazonaws.com";
                    default:
                        return "";
                }
            }
        }
    }
}