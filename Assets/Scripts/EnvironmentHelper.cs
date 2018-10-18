using UnityEngine;
using ProjectTrinity.Helper;
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

    private static JoinMatchHelper joinMatchHelper;

    private void Awake()
    {
        staticEnvironment = environment;
        DebugAIEnabled = enabledDebugAI;
        joinMatchHelper = new JoinMatchHelper();
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
                    return joinMatchHelper.GetMatchServerAddress();
                    // return "34.254.60.219";
                default:
                    return "";
            }
        }
    }
	
}
