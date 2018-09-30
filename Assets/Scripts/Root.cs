using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Root;
using UnityEngine;

public class Root : MonoBehaviour 
{
    private MatchStateMachine matchStateMachine;

    private void Start()
    {
        matchStateMachine = new MatchStateMachine();

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
            Debug.Log("W");
            matchStateMachine.MatchInputProvider.AddXTranslation(1f);
        } 
        else if(Input.GetKey(KeyCode.S))
        {
            matchStateMachine.MatchInputProvider.AddXTranslation(-1f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            matchStateMachine.MatchInputProvider.AddYTranslation(1f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            matchStateMachine.MatchInputProvider.AddYTranslation(-1f);
        }
    }
}
