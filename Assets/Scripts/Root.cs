using ProjectTrinity.Input;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Root;
using TMPro;
using UnityEngine;

public class Root : MonoBehaviour 
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject player2;

    [SerializeField]
    private GameObject player3;

    [SerializeField]
    private TextMeshProUGUI rttText;

    [SerializeField]
    private GameObject lookingForMatchUI;

    [SerializeField]
    private GameObject matchEndedUI;

    [SerializeField]
    private GameObject cameraRoot;

    [SerializeField]
    private ViewMatchInputRegistry viewMatchInputRegistry;

    private MatchStateMachine matchStateMachine;
    private bool parentedPlayerCamera;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        matchStateMachine = new MatchStateMachine();
        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(0, player);
        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(1, player2);
        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(2, player3);

        viewMatchInputRegistry.Initialize(matchStateMachine);
    }

    private void FixedUpdate()
    {
        matchStateMachine.OnFixedUpdateTick();
        DIContainer.AckedMessageHelper.OnFixedUpdateTick();
        DIContainer.RoundTripTimeService.OnFixedUpdateTick();
    }

    private void Update()
    {
        rttText.text = string.Format("RTT: {0}ms", DIContainer.RoundTripTimeService.LastPing);

        lookingForMatchUI.SetActive(matchStateMachine.CurrentMatchState is TimeSyncMatchState || matchStateMachine.CurrentMatchState is WaitForStartMatchState);
        matchEndedUI.SetActive(matchStateMachine.CurrentMatchState is EndMatchState);

        if (!parentedPlayerCamera && matchStateMachine.CurrentMatchState is WaitForStartMatchState)
        {
            parentedPlayerCamera = true;
            switch (matchStateMachine.LocalPlayerId)
            { 
                case 0:
                    cameraRoot.transform.SetParent(player.transform);
                    break;
                case 1:
                    cameraRoot.transform.SetParent(player2.transform);
                    break;
                case 2:
                    cameraRoot.transform.SetParent(player3.transform);
                    break;
            }
        }
    }
}
