using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class JoinMatchHelper : MonoBehaviour
{
    public void GetRequest(){
		this.StartCoroutine(this.GetEndpointData());
	}
    // just for the purpose of testing a function call
    public string GetURL(){
        return "127.0.0.1";
    }
    private IEnumerator GetEndpointData() {
        int playerCount = 1;
        UnityWebRequest www = UnityWebRequest.Get("localhost:8080/joinmatch/" + playerCount);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("WebServer Response");
            Debug.Log(JsonUtility.ToJson(www));
            Debug.Log(www.downloadHandler.text); // rubbish
        }
    }
}