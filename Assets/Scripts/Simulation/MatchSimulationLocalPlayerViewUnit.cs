using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationLocalPlayerViewUnit : MatchSimulationViewUnit
{
    private bool receivedLocalAimingUpdate;

    public override void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, byte frame)
    {
        Vector3 targetPosition = updatedUnitState.GetUnityPosition();

        animator.SetBool("Running", Vector3.Distance(transform.position, targetPosition) > 0.01f);

        transform.position = targetPosition;
        modelRoot.transform.rotation = updatedUnitState.GetUnityRotation();
        return;
    }

    public override void OnLocalAimingUpdate(float rotation)
    {
        telegraphRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        receivedLocalAimingUpdate = true;
        telegraphRoot.gameObject.SetActive(true);
    }

    public override void UpdateToNextState(byte currentFrame)
    {
        if (receivedLocalAimingUpdate)
        {
            receivedLocalAimingUpdate = false;
        }
        else 
        {
            telegraphRoot.gameObject.SetActive(false);
        }
    }
}
