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

    [SerializeField]
    private bool enabledDebugAI;
    public static bool DebugAIEnabled { get; private set; }

    private void Awake()
    {
        staticEnvironment = environment;
        DebugAIEnabled = enabledDebugAI;
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
                    return "ec2-34-248-140-154.eu-west-1.compute.amazonaws.com";
                default:
                    return "";
            }
        }
    }
	
}
