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

    private MatchStateMachine matchStateMachine;

    private void Start()
    {
        matchStateMachine = new MatchStateMachine();
        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(0, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player.transform.position = position;

            Vector3 rotation = unitState.GetUnityRotation();
            player.transform.rotation = Quaternion.Euler(rotation);
        });

        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(1, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player2.transform.position = position;

            Vector3 rotation = unitState.GetUnityRotation();
            player2.transform.rotation = Quaternion.Euler(rotation);
        });

        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(2, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player3.transform.position = position;

            Vector3 rotation = unitState.GetUnityRotation();
            player3.transform.rotation = Quaternion.Euler(rotation);
        });
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

        if (Input.GetKey(KeyCode.W))
        {
            matchStateMachine.MatchInputProvider.AddYTranslation(1f);
        } 
        else if(Input.GetKey(KeyCode.S))
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

        if(Mathf.Abs(matchStateMachine.MatchInputProvider.XTranslation) > 0f || Mathf.Abs(matchStateMachine.MatchInputProvider.YTranslation) > 0f) {
            Quaternion rotation = Quaternion.LookRotation(
            new Vector3(matchStateMachine.MatchInputProvider.XTranslation, 0f,
                        matchStateMachine.MatchInputProvider.YTranslation), Vector3.up);

            matchStateMachine.MatchInputProvider.AddRotation(rotation.eulerAngles.y);
        }

        if(EnvironmentHelper.DebugAIEnabled)
        {
            if(Random.Range(0, 2) == 0)
            {
                matchStateMachine.MatchInputProvider.AddYTranslation(Random.Range(-1f, 1f));
            }
            else
            {
                matchStateMachine.MatchInputProvider.AddXTranslation(Random.Range(-1f, 1f));
            }
        }
    }
}
