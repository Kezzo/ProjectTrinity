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

        if (currentSpellActivation == null)
        {
            modelRoot.transform.rotation = updatedUnitState.GetUnityRotation();
        }

        return;
    }

    public override void OnLocalAimingUpdate(float rotation)
    {
        if (currentSpellActivation != null)
        {
            return;
        }

        telegraphRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        receivedLocalAimingUpdate = true;
        telegraphRoot.gameObject.SetActive(true);
    }

    public override void OnSpellActivation(float rotation, byte startFrame, byte activationFrame)
    {
        if (currentSpellActivation != null)
        {
            return;
        }

        currentSpellActivation = new SpellActivationData(rotation, startFrame, activationFrame);

        telegraphRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        modelRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);

        telegraphRoot.gameObject.SetActive(true);
        animator.SetTrigger("Attack");
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

        if (currentSpellActivation != null)
        {
            telegraphRoot.transform.rotation = Quaternion.Euler(0f, currentSpellActivation.Rotation, 0f);
            if (MatchSimulationUnit.IsFrameInFuture(currentFrame, currentSpellActivation.ActivationFrame) || currentFrame == currentSpellActivation.ActivationFrame)
            {
                //TODO: Show telegraph 'timer'
                currentSpellActivation = null;
            }
        }
    }
}
