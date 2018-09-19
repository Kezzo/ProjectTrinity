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
      StartListen ();
      StartCoroutine (StartSend ());
    } catch (Exception e) {
      Debug.Log (e.ToString ());
    }
  }

  private void StartListen () {
    Debug.Log ("Waiting for broadcast");
    Task.Run (async () => {
      using (var udpClient = new UdpClient (1337)) {
        while (true) {
          //IPEndPoint object will allow us to read datagrams sent from any source.
          var receivedResults = await udpClient.ReceiveAsync ();
          var buffer = receivedResults.Buffer;
          Debug.LogFormat ("Received broadcast from : {0}\n",
            Encoding.ASCII.GetString (buffer, 0, buffer.Length));
        }
      }
    });
  }
  private IEnumerator StartSend () {

    Socket s = new Socket (AddressFamily.InterNetwork, SocketType.Dgram,
      ProtocolType.Udp);

    IPAddress broadcast = IPAddress.Parse ("127.0.0.1");

    byte[] sendbuf = Encoding.ASCII.GetBytes ("HALLO");
    IPEndPoint ep = new IPEndPoint (broadcast, 2448);
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