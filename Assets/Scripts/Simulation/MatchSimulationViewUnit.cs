using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using ProjectTrinity.UI;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    protected Animator animator;
    protected GameObject modelRoot;
    protected GameObject telegraphRoot;
    protected GameObject telegraphFillRoot;

    private static readonly byte FrameDelay = 5; // 5 == ~165ms delay

    public class InterpolationState
    {
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public byte TargetFrame;
        
        public InterpolationState(Vector3 targetPosition, Quaternion targetRotation, byte targetFrame)
        {
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;
            TargetFrame = targetFrame;
        }

        public override string ToString()
        {
            return string.Format("TargetPosition: {0} TargetRotation: {1} TargetFrame: {2}",
                                 TargetPosition, TargetRotation.eulerAngles, TargetFrame);
        }
    }

    protected class AbilityActivationData
    {
        public float Rotation { get; private set; }
        public byte StartFrame { get; private set; }
        public byte ActivationFrame { get; private set; }
        public bool Started { get; set; }

        public AbilityActivationData(float rotation, byte startFrame, byte activationFrame)
        {
            Rotation = rotation;
            StartFrame = startFrame;
            ActivationFrame = activationFrame;
        }
    }

    private Queue<InterpolationState> interpolationQueue = new Queue<InterpolationState>();

    public InterpolationState[] CurrentInterpolationBuffer
    {
        get 
        {
            return interpolationQueue.ToArray();
        }
    }

    private int movementChangedCounter = 0;
    public InterpolationState CurrentStateToLerpTo { get; private set; }
    protected AbilityActivationData currentAbilityActivation;

    private float lastMovementSpeedModifier = 0f;

    private Healthbar healthbar;

    private void InitializeChildComponents()
    {
        animator = GetComponent<Animator>();

        foreach (Transform child in gameObject.transform)
        {
            switch (child.tag) { 
                case "ModelRoot":
                    modelRoot = child.gameObject;
                    break;
                case "TelegraphRoot":
                    telegraphRoot = child.gameObject;
                    telegraphRoot.SetActive(false);

                    foreach (Transform telegraphChild in telegraphRoot.transform)
                    {
                        if (telegraphChild.tag == "TelegraphFillRoot") 
                        {
                            telegraphFillRoot = telegraphChild.gameObject;
                            // invisble/unfilled telegraph
                            telegraphFillRoot.transform.localScale = new Vector3(0f, 1f, 0f);
                            telegraphFillRoot.SetActive(false);
                        }
                    }

                    break;
                case "Healthbar":
                    healthbar = child.gameObject.GetComponent<Healthbar>();
                    break;
            }
        }
    }

    public virtual void OnPositionRotationUpdate(MatchSimulationUnit updatedUnitState, byte frame)
    {
        InterpolationState stateToAdd = new InterpolationState(updatedUnitState.GetUnityPosition(), updatedUnitState.GetUnityRotation(),
                                                               (byte)MathHelper.Modulo(frame + FrameDelay, byte.MaxValue));

        //DIContainer.Logger.Debug("OnUnitStateUpdate state: " + stateToAdd);

        interpolationQueue.Enqueue(stateToAdd);
    }

    public void OnSpawn(MatchSimulationUnit unitState)
    {
        InitializeChildComponents();
        transform.position = unitState.GetUnityPosition();
        transform.rotation = unitState.GetUnityRotation();
        healthbar.Initialize(unitState);
    }

    public virtual void OnLocalAimingUpdate(float rotation) { }

    public virtual void OnAbilityActivation(float rotation, byte startFrame, byte activationFrame) 
    {
        if (currentAbilityActivation != null)
        {
            DIContainer.Logger.Warn("Received Ability activation while already having an ability active");
            return;
        }

        currentAbilityActivation = new AbilityActivationData(rotation, 
                                                             (byte)MathHelper.Modulo(startFrame + FrameDelay, byte.MaxValue),
                                                             (byte)MathHelper.Modulo(activationFrame + FrameDelay, byte.MaxValue));
    }

    public virtual void UpdateToNextState(byte currentFrame)
    {
        int speedPartToModify = 390 / 6;
        float queueCount = interpolationQueue.Count;
        float frameDelay = FrameDelay;

        // quick decrease, slow increase
        float maxSpeedThisFrame = Mathf.Min(1f, lastMovementSpeedModifier + 0.05f);
        float minSpeedThisFrame = Mathf.Max(0f, lastMovementSpeedModifier - 0.3f);

        float movementSpeedModifier = Mathf.Clamp01(Mathf.Clamp(queueCount / frameDelay, minSpeedThisFrame, maxSpeedThisFrame));
        int movementSpeedForFrame = (int)(390 - speedPartToModify + (speedPartToModify * movementSpeedModifier));
        float movevementDistanceAvailable = UnitValueConverter.ToUnityPosition(movementSpeedForFrame);

        lastMovementSpeedModifier = movementSpeedModifier;

        /*DIContainer.Logger.Debug(string.Format("Movement speed for frame: {0} Modifier: {1} QueueCount: {2} Delay: {3}",
                                               movementSpeedForFrame, movementSpeedModifier, interpolationQueue.Count, PositionFrameDelay));*/

        if (CurrentStateToLerpTo != null)
        {
            AdvanceLerpToPositionState(CurrentStateToLerpTo, ref movevementDistanceAvailable);

            if (CurrentStateToLerpTo == null)
            {
                // to ensure next lerp is started so wait frame that results in a stutter is generated.
                StartLerpingIfValid(currentFrame, ref movevementDistanceAvailable);
            }
        }
        else if (!StartLerpingIfValid(currentFrame, ref movevementDistanceAvailable))
        {
            /*StringBuilder stringBuilder = new StringBuilder();
            InterpolationState[] stateArray = interpolationQueue.ToArray();
            for (int i = 0; i < stateArray.Length; i++)
            {
                stringBuilder.Append(stateArray[i].TargetFrame);
                stringBuilder.Append(" ");
            }

            DIContainer.Logger.Warn(string.Format("No state to update to at frame: {0} existing frames: {1}", currentFrame, stringBuilder));*/

            movementChangedCounter++;

            if (movementChangedCounter > 2 && animator != null)
            {
                animator.SetBool("Running", false);
            }
        }

        if (currentAbilityActivation != null && 
            (MatchSimulationUnit.IsFrameInFuture(currentFrame, currentAbilityActivation.StartFrame) || 
             currentFrame == currentAbilityActivation.StartFrame))
        {
            modelRoot.transform.rotation = Quaternion.Euler(0f, currentAbilityActivation.Rotation, 0f);
            telegraphRoot.transform.rotation = Quaternion.Euler(0f, currentAbilityActivation.Rotation, 0f);

            if (!currentAbilityActivation.Started)
            {
                telegraphRoot.gameObject.SetActive(true);
                telegraphFillRoot.gameObject.SetActive(true);

                if(animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                currentAbilityActivation.Started = true;
            }

            if (MatchSimulationUnit.IsFrameInFuture(currentFrame, currentAbilityActivation.ActivationFrame) || currentFrame == currentAbilityActivation.ActivationFrame)
            {
                currentAbilityActivation = null;
                telegraphRoot.gameObject.SetActive(false);
                telegraphFillRoot.gameObject.SetActive(true);
                telegraphFillRoot.transform.localScale = new Vector3(0f, 1f, 0f);
            }
            else
            {
                UpdateTelegraphFillState(currentFrame);
            }
        }
    }

    private bool StartLerpingIfValid(byte currentFrame, ref float movementDistanceAvailable)
    {
        if (interpolationQueue.Count > 0 && ShouldLerpToState(interpolationQueue.Peek(), currentFrame))
        {
            CurrentStateToLerpTo = interpolationQueue.Dequeue();
            modelRoot.transform.rotation = CurrentStateToLerpTo.TargetRotation;
            float positionChanged = AdvanceLerpToPositionState(CurrentStateToLerpTo, ref movementDistanceAvailable);

            if(positionChanged > 0.1f && animator != null)
            {
                animator.SetBool("Running", true);
            }

            return true;
        }

        return false;
    }

    private float AdvanceLerpToPositionState(InterpolationState state, ref float movementDistanceAvailable)
    {
        movementChangedCounter = 0;

        Vector3 positionDiff =  state.TargetPosition - this.transform.position;

        float clampedXPositionDiff = Mathf.Clamp(Mathf.Abs(positionDiff.x), 0, movementDistanceAvailable); 
        float cappedXPositionDiff = positionDiff.x > 0 ? clampedXPositionDiff : -clampedXPositionDiff;

        float clampedZPositionDiff = Mathf.Clamp(Mathf.Abs(positionDiff.z), 0, movementDistanceAvailable);
        float cappedZPositionDiff = positionDiff.z > 0 ? clampedZPositionDiff : -clampedZPositionDiff;

        Vector3 position = new Vector3(this.transform.position.x + cappedXPositionDiff, 0f, this.transform.position.z + cappedZPositionDiff);

        movementDistanceAvailable -= clampedXPositionDiff > clampedZPositionDiff ? clampedXPositionDiff : clampedZPositionDiff;

        /*DIContainer.Logger.Debug(string.Format("AdvanceLerpToPositionState: frameDiffStartTarget: {0} frameDiffStartCurrent: {1} interpolant: {2} position: {3} CurrentFrame: {4} TargetFrame: {5}",
                                               frameDiffStartTarget, frameDiffStartCurrent, interpolant, position, currentFrame, state.TargetFrame));*/

        float positionChange = (this.transform.position - position).magnitude;
        this.transform.position = position;

        if ((this.transform.position - state.TargetPosition).magnitude <= 0.0001f)
        {
            CurrentStateToLerpTo = null;
        }

        return positionChange;
    }

    private bool ShouldLerpToState(InterpolationState state, byte currentFrame)
    {
        // is state in past or current frame? Should never happen.
        if (MatchSimulationUnit.IsFrameInFuture(currentFrame, state.TargetFrame) || currentFrame == state.TargetFrame)
        {
            return true;
        }

        Vector3 positionDiff = state.TargetPosition - this.transform.position;
        byte frameDiff = MathHelper.GetFrameDiff(currentFrame, state.TargetFrame);
        byte maxFramesForPositionChange = MathHelper.GetRoundedMaxFramesForPositionChange(positionDiff);
        bool shouldLerp = (frameDiff - maxFramesForPositionChange) <= 1 || frameDiff == 0;

        /*DIContainer.Logger.Debug(string.Format("ShouldLerp: {0} PositionDiff: {1} FrameDiff: {2} FramesForPositionChange: {3} CurrentFrame: {4} TargetFrame: {5}", 
                                               shouldLerp, positionDiff, frameDiff, maxFramesForPositionChange, currentFrame, state.TargetFrame));*/

        return shouldLerp;
    }

    protected void UpdateTelegraphFillState(byte currentFrame)
    {
        // so fill telegraph is full shortly before ability is done
        currentFrame = (byte)MathHelper.Modulo(currentFrame + 1, byte.MaxValue);

        byte skillDurationInFrames = MathHelper.GetFrameDiff(currentAbilityActivation.StartFrame, currentAbilityActivation.ActivationFrame);
        byte framesTillActivation = MathHelper.GetFrameDiff(currentFrame, currentAbilityActivation.ActivationFrame);
        float interpolant = Mathf.InverseLerp(skillDurationInFrames, 0, framesTillActivation);

        telegraphFillRoot.transform.localScale = new Vector3(interpolant, 1f, interpolant);
    }
}
