using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkClient : MonoBehaviour {
  // Use this for initialization
  void Start () {
    try {
      Socket s = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      IPAddress broadcast = IPAddress.Parse ("127.0.0.1");
      IPEndPoint srcIpEndPoint = new IPEndPoint (broadcast, 1337);
      s.Bind (srcIpEndPoint);
      IPEndPoint ep = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 2448);
      s.SendTo (Encoding.ASCII.GetBytes ("Start"), ep);

      StartCoroutine (ListenForMessages (s));
      StartCoroutine (SendMessages (s));

    } catch (Exception e) {
      Debug.Log (e.ToString ());
    }
  }

  private IEnumerator ListenForMessages (Socket s) {
    Debug.Log ("Waiting for broadcast");
    while (true) {
      byte[] b = new byte[100];
      int k = s.Receive (b);
      string szReceived = Encoding.ASCII.GetString (b, 0, k);
      Debug.LogFormat (szReceived);
      yield return new WaitForSeconds (1f);
    }
  }
  private IEnumerator SendMessages (Socket s) {
    IPAddress broadcast = IPAddress.Parse ("127.0.0.1");
    IPEndPoint ep = new IPEndPoint (broadcast, 2448); // endpoint where server is listening
    //client.Connect(ep);
    byte[] sendbuf = Encoding.ASCII.GetBytes ("HALLO");
    while (true) {
      s.SendTo (sendbuf, ep);
      Debug.LogFormat ("Message sent to the broadcast address");
      yield return new WaitForSeconds (1f);
    }
  }

  // Update is called once per frame
  void Update () {

  }
}