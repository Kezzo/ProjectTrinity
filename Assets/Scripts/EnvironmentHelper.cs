using UnityEngine;

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
    private static Environment staticEnvironment;

    private void Awake()
    {
        staticEnvironment = environment;
    }

    public static string ServerUrl 
    {
        get
        {
            switch(staticEnvironment)
            {
                case Environment.LOCAL:
                    return "127.0.0.1";
                case Environment.DEV:
                    return "ec2-34-242-151-135.eu-west-1.compute.amazonaws.com";
                default:
                    return "";
            }
        }
    }
	
}
