using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubNubAPI;
using System;

public class Lobby : MonoBehaviour 
{
	[SerializeField]
	private GameObject m_playerPrefab;

	private PubNub m_pubNubClient;
	private Dictionary<string, GameObject> m_playerObjects = new Dictionary<string, GameObject>();
	private string m_localPlayerId;

	private bool m_localPlayerJoined;

	private Player m_localPlayer;

	[Serializable]
	public class PlayerPositionUpdates {
		public List<PlayerPositionUpdate> playerPositionUpdates;

		public PlayerPositionUpdates (List<PlayerPositionUpdate> playerPositionUpdates) {
			this.playerPositionUpdates = playerPositionUpdates;
		}
	}

    [Serializable]
	public class PlayerPositionUpdate {
		public string playerId;
		public string xPosition;

		public string zPosition;

		public int sequence;

		public PlayerPositionUpdate (string playerId, string xPosition, string zPosition) {
			this.playerId = playerId;
			this.xPosition = xPosition;
			this.zPosition = zPosition;
		}
	}

	// Use this for initialization
	private void Start () 
	{
		Application.targetFrameRate = 30;
		/*PlayerPositionUpdates test = new PlayerPositionUpdates(new List<PlayerPositionUpdate> {
			new PlayerPositionUpdate("43563456", "234", "234")
		});*/

		//Debug.Log(JsonUtility.ToJson(test));

		m_localPlayerId = UnityEngine.Random.Range(0, 9999999).ToString();

		Debug.Log("Joining as: " + m_localPlayerId);
		PNConfiguration pnConfiguration = new PNConfiguration();
		pnConfiguration.SubscribeKey = "sub-c-9f6202a4-b9c7-11e8-8fd2-4a2bdf4876be";
		pnConfiguration.PublishKey = "pub-c-1a007f83-30b3-453c-bb73-494a3ba26029";
		pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
		pnConfiguration.UUID = m_localPlayerId;

		m_pubNubClient = new PubNub(pnConfiguration);
		List<string> channels = new List<string>{ "world" };
		m_pubNubClient.SusbcribeCallback += OnMessageReceived;
		m_pubNubClient.Subscribe().Channels(channels).Execute();

		Command joinCommand = new Command(m_localPlayerId, "JOIN", false);
		m_pubNubClient.Publish().Channel("world").Message(joinCommand)
		.Async((result, status) => {
			Debug.Log("JOIN Command sent");
		});
	}

	private void OnMessageReceived (object sender, EventArgs eventArgs) 
	{
		SusbcribeEventEventArgs subscribeEvent = eventArgs as SusbcribeEventEventArgs;

		if(subscribeEvent == null)
		{
			return;
		}

		if(subscribeEvent.Status.Operation == PNOperationType.PNSubscribeOperation && 
			subscribeEvent.MessageResult == null && !m_localPlayerJoined) 
		{
			GameObject localPlayerObject = Instantiate(m_playerPrefab) as GameObject;
			m_playerObjects[m_localPlayerId] = localPlayerObject;

			m_localPlayer = localPlayerObject.GetComponent<Player>();

			if(m_localPlayer != null) 
			{
				m_localPlayer.SetCommandSentCallback((command) => {

					Command commandToSend = new Command(m_localPlayerId, command);
					m_pubNubClient.Publish().Channel("world").Message(JsonUtility.ToJson(commandToSend)).PublishAsIs(true)
					.Async((result, status) => {
							Debug.Log(command + " Command sent");
						});
					m_localPlayer.AddLocalPosition(commandToSend);
				});
			}

			m_localPlayerJoined = true;
			return;
		}

		if(subscribeEvent.MessageResult.IssuingClientId != "server") {
			return;
		}

		if(subscribeEvent.MessageResult != null) 
		{
			string stringPayload = (string)subscribeEvent.MessageResult.Payload;
			PlayerPositionUpdates playerPositionUpdates = JsonUtility.FromJson<PlayerPositionUpdates>(stringPayload);

			if(playerPositionUpdates != null) {
				foreach (var playerPositionUpdate in playerPositionUpdates.playerPositionUpdates)
				{
					GameObject playerObject;
					if(m_playerObjects.TryGetValue(playerPositionUpdate.playerId, out playerObject)) 
					{
						float xPosition = playerObject.transform.position.x;
						float.TryParse(playerPositionUpdate.xPosition, out xPosition);

						float zPosition = playerObject.transform.position.z;
						float.TryParse(playerPositionUpdate.zPosition, out zPosition);

						Player player = playerObject.GetComponent<Player>();
						if(player != null) {
							player.SetPosition(new Vector3(xPosition, 0.5f, zPosition), playerPositionUpdate.sequence);
						}
					} 
					else 
					{
						GameObject createdPlayerObject = Instantiate(m_playerPrefab) as GameObject;
						m_playerObjects[playerPositionUpdate.playerId] = createdPlayerObject;

						float xPosition = createdPlayerObject.transform.position.x;
						float.TryParse(playerPositionUpdate.xPosition, out xPosition);

						float zPosition = createdPlayerObject.transform.position.z;
						float.TryParse(playerPositionUpdate.zPosition, out zPosition);

						Player player = createdPlayerObject.GetComponent<Player>();
						if(player != null) {
							player.SetPosition(new Vector3(xPosition, 0.5f, zPosition), playerPositionUpdate.sequence);
						}
					}
				}
			}
			
		}

	}
}
