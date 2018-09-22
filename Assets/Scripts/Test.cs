using System.Collections;
using System.Collections.Generic;
using ProjectTrinity.Networking;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UdpClient udpClient = new UdpClient("127.0.0.1", 2448, 1337);
		NetworkTimeService networkTimeService =  new NetworkTimeService();
		networkTimeService.Synch(udpClient, () => {
			Debug.Log("Time was synched!");
		});
	}
}
