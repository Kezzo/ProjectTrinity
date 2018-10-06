namespace ProjectTrinity.Helper
{
    public interface ILogger
    {
        void Debug(string logMessage);
        void Warn(string logMessage);
        void Error(string logMessage);
    }
}