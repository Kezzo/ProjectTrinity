namespace ProjectTrinity.Helper
{
    public interface ISerializer
    {
        string Serialize<T>(T objectToSerialize) where T : class;
        T Deserialize<T>(string stringToDeserialize) where T : class;
    }
}