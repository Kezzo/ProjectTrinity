using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public static class JoinMatchHelper
{
    public static string url;
    public static string port;

    public static IEnumerator GetEndpointData() {
        int playerCount = 2;
        UnityWebRequest www = UnityWebRequest.Get("localhost:8080/joinmatch/" + playerCount);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("WebServer Response");
            Debug.Log(www.downloadHandler.text);

        }
    }
}