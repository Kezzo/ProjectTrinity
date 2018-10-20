using ProjectTrinity.Debugging;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Root;
using ProjectTrinity.UI;
using TMPro;
using UnityEngine;

public class Root : MonoBehaviour 
{
    [SerializeField]
    private MatchSimulationViewUnit player;

    [SerializeField]
    private MatchSimulationViewUnit player2;

    [SerializeField]
    private MatchSimulationViewUnit player3;

    [SerializeField]
    private TextMeshProUGUI rttText;

    [SerializeField]
    private GameObject lookingForMatchUI;

    [SerializeField]
    private GameObject matchEndedUI;

    [SerializeField]
    private VirtualJoystick movementJoyStick;

    [SerializeField]
    private VirtualJoystick aimingJoyStick;

    private MatchStateMachine matchStateMachine;

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

        if (movementJoyStick.JoystickActive && (Mathf.Abs(movementJoyStick.Horizontal) > 0.2f || Mathf.Abs(movementJoyStick.Vertical) > 0.2f))
        {
            matchStateMachine.MatchInputProvider.AddXTranslation(movementJoyStick.Horizontal);
            matchStateMachine.MatchInputProvider.AddYTranslation(movementJoyStick.Vertical);
        } 
        else if (UnitDebugAI.DebugAIEnabled)
        {
            if (Random.Range(0, 2) == 0)
            {
                matchStateMachine.MatchInputProvider.AddYTranslation(Random.Range(-1f, 1f));
            }
            else
            {
                matchStateMachine.MatchInputProvider.AddXTranslation(Random.Range(-1f, 1f));
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                matchStateMachine.MatchInputProvider.AddYTranslation(1f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                matchStateMachine.MatchInputProvider.AddYTranslation(-1f);
            }

            if (Input.GetKey(KeyCode.A))
            {
                matchStateMachine.MatchInputProvider.AddXTranslation(-1f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                matchStateMachine.MatchInputProvider.AddXTranslation(1f);
            }
        }

        if(Mathf.Abs(matchStateMachine.MatchInputProvider.XTranslation) > 0f || Mathf.Abs(matchStateMachine.MatchInputProvider.YTranslation) > 0f) {
            Quaternion rotation = Quaternion.LookRotation(
            new Vector3(matchStateMachine.MatchInputProvider.XTranslation, 0f,
                        matchStateMachine.MatchInputProvider.YTranslation), Vector3.up);

            matchStateMachine.MatchInputProvider.AddRotation(rotation.eulerAngles.y);
        }


    }
}
