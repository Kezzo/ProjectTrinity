namespace ProjectTrinity.Helper
{
    public class UnityLogger : ILogger
    {
        public void Debug(string logMessage)
        {
            UnityEngine.Debug.Log(logMessage);
        }

        public void Warn(string logMessage)
        {
            UnityEngine.Debug.LogWarning(logMessage);
        }

        public void Error(string logMessage)
        {
            UnityEngine.Debug.LogError(logMessage);
        }
    }
}