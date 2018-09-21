using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkClient : MonoBehaviour {
  // Use this for initialization
  void Start () {

    var networkTimeService = new NetworkTimeService();
    networkTimeService.Synch(IPAddress.Parse ("127.0.0.1"), 2448, 1337, () => {
      
    });

    try {
      Socket s = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      IPAddress broadcast = IPAddress.Parse ("127.0.0.1");
      IPEndPoint srcIpEndPoint = new IPEndPoint (broadcast, 1337);
      s.Bind (srcIpEndPoint);
      IPEndPoint ep = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 2448);
      s.SendTo (Encoding.ASCII.GetBytes ("Start"), ep);

      StartCoroutine (SendMessages (s, broadcast));

    } catch (Exception e) {
      Debug.Log (e.ToString ());
    }
  }

  private void ListenForMessages (Socket s) {
    Debug.Log ("Listening for messages");

    byte[] buffer = new byte[100];
    SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
    socketAsyncEventArgs.SetBuffer(buffer, 0, 100);

    socketAsyncEventArgs.Completed += (o, eventArgs) => {
      string szReceived = Encoding.ASCII.GetString (eventArgs.Buffer, 0, eventArgs.BytesTransferred);
      Debug.LogFormat (szReceived);

      // listen for next packet
      s.ReceiveAsync(socketAsyncEventArgs);
    };
    
    // kick-off listening chain
    s.ReceiveAsync(socketAsyncEventArgs);
  }

  private IEnumerator SendMessages (Socket s, IPAddress ip) {
    IPEndPoint ep = new IPEndPoint (ip, 2448); // endpoint where server is listening
    //client.Connect(ep);
    byte[] sendbuf = Encoding.ASCII.GetBytes ("HALLO");
    SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
    socketAsyncEventArgs.RemoteEndPoint = ep;
    socketAsyncEventArgs.SetBuffer(sendbuf, 0, sendbuf.Length);

    socketAsyncEventArgs.Completed += (o, eventArgs) => {
        Debug.LogFormat ("Message sent to the broadcast address");
      };

      for (int i = 0; i < 5; i++)
      {
        s.SendToAsync (socketAsyncEventArgs);
        yield return new WaitForSeconds (1f);
      }

      ListenForMessages(s);
  }
}