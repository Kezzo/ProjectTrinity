using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Root;
using UnityEngine;

public class Root : MonoBehaviour 
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject player2;

    [SerializeField]
    private GameObject player3;

    private MatchStateMachine matchStateMachine;

    private void Start()
    {
        matchStateMachine = new MatchStateMachine();
        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(0, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player.transform.position = position;
        });

        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(1, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player2.transform.position = position;
        });

        matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(2, (unitState) =>
        {
            Vector3 position = unitState.GetUnityPosition();
            player3.transform.position = position;
        });
    }

    private void FixedUpdate()
    {
        matchStateMachine.OnFixedUpdateTick();
        DIContainer.AckedMessageHelper.OnFixedUpdateTick();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
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
    }
}
