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
}
