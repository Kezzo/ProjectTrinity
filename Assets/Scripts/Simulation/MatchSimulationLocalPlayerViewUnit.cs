using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationLocalPlayerViewUnit : MatchSimulationViewUnit
{
    private bool receivedLocalAimingUpdate;
    private int continousPositionChangeFrames = 0;

    protected override void OnPositionRotationUpdate(MatchSimulationUnit.MovementProperties movementProperties)
    {
        Vector3 targetPosition = movementProperties.GetUnityPosition();

        float distance = Vector3.Distance(transform.position, targetPosition);
        continousPositionChangeFrames = Mathf.Clamp((distance > 0f ? 5 : continousPositionChangeFrames - 1), 0, 3);

        animator.SetBool("Running", continousPositionChangeFrames > 0);
        transform.position = targetPosition;

        if (currentAbilityActivation == null)
        {
            modelRoot.transform.rotation = movementProperties.GetUnityRotation();
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
