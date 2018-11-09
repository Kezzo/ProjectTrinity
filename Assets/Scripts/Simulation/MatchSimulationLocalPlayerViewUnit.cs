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

        if (currentAbilityActivation == null)
        {
            modelRoot.transform.rotation = updatedUnitState.GetUnityRotation();
        }

        return;
    }

    public override void OnLocalAimingUpdate(float rotation)
    {
        if (currentAbilityActivation != null)
        {
            return;
        }

        telegraphRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        receivedLocalAimingUpdate = true;
        telegraphRoot.gameObject.SetActive(true);
    }

    public override void OnAbilityActivation(float rotation, byte startFrame, byte activationFrame)
    {
        if (currentAbilityActivation != null)
        {
            return;
        }

        currentAbilityActivation = new AbilityActivationData(rotation, startFrame, activationFrame);

        telegraphRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        modelRoot.transform.rotation = Quaternion.Euler(0f, rotation, 0f);

        telegraphRoot.gameObject.SetActive(true);
        telegraphFillRoot.gameObject.SetActive(true);
        animator.SetTrigger("Attack");
    }

    public override void UpdateToNextState(byte currentFrame)
    {
        if (currentAbilityActivation != null)
        {
            telegraphRoot.transform.rotation = Quaternion.Euler(0f, currentAbilityActivation.Rotation, 0f);
            UpdateTelegraphFillState(currentFrame);
            if (MatchSimulationUnit.IsFrameInFuture(currentFrame, currentAbilityActivation.ActivationFrame) || currentFrame == currentAbilityActivation.ActivationFrame)
            {
                //TODO: Show telegraph 'timer'
                telegraphRoot.gameObject.SetActive(false);
                telegraphFillRoot.gameObject.SetActive(false);
                currentAbilityActivation = null;
            }

            return;
        }

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
