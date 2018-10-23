using UnityEngine;

namespace ProjectTrinity.Helper
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string stringToDeserialize) where T : class
        {
            return JsonUtility.FromJson<T>(stringToDeserialize);
        }

        public string Serialize<T>(T objectToSerialize) where T : class
        {
            return JsonUtility.ToJson(objectToSerialize);
        }
    }
}