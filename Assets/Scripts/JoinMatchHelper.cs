using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
public class JoinMatchHelper
{
    public string endpoint;
    public string port;

    private void Start()
    {
        GetEndpointData();
    }

    async void GetEndpointData() {
        await Task.Run(async () =>
          {
              int playerCount = 1;
              UnityWebRequest www = UnityWebRequest.Get(
                  "localhost:8080/joinmatch/" + playerCount);
              var result = www.SendWebRequest();
              if (www.isNetworkError || www.isHttpError)
              {
                  Debug.Log(www.error);
              }
              else
              {
                  Debug.Log("WebServer Response");
                  Debug.Log(JsonUtility.ToJson(www));
                  Debug.Log(www.downloadHandler.text);
                  endpoint = "123";
                  port = "123";
              }
          });
    }
    public string GetMatchServerAddress()
    {
        return endpoint + port;
    }
}