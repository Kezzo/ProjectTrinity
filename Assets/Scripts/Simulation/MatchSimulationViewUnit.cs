using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    protected Animator animator;
    protected GameObject modelRoot;
    protected GameObject telegraphRoot;

    private static readonly byte PositionFrameDelay = 5; // 5 == ~165ms delay

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

    protected class SpellActivationData
    {
        public float Rotation { get; private set; }
        public byte StartFrame { get; private set; }
        public byte ActivationFrame { get; private set; }

        public SpellActivationData(float rotation, byte startFrame, byte activationFrame)
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
    protected SpellActivationData currentSpellActivation;

    private float lastMovementSpeedModifier = 0f;

    private void Awake()
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
                    break;
            }
        }
    }

    public virtual void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, byte frame)
    {
        InterpolationState stateToAdd = new InterpolationState(updatedUnitState.GetUnityPosition(), updatedUnitState.GetUnityRotation(),
                                                               (byte)MathHelper.Modulo(frame + PositionFrameDelay, byte.MaxValue));

        //DIContainer.Logger.Debug("OnUnitStateUpdate state: " + stateToAdd);

        interpolationQueue.Enqueue(stateToAdd);
    }

    public virtual void OnLocalAimingUpdate(float rotation) { }

    public virtual void OnSpellActivation(float rotation, byte startFrame, byte activationFrame) 
    {

    }

    public virtual void UpdateToNextState(byte currentFrame)
    {
        int speedPartToModify = 390 / 6;
        float queueCount = interpolationQueue.Count;
        float frameDelay = PositionFrameDelay;

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

            if (movementChangedCounter > 2)
            {
                animator.SetBool("Running", false);
            }
        }
    }

    private bool StartLerpingIfValid(byte currentFrame, ref float movementDistanceAvailable)
    {
        if (interpolationQueue.Count > 0 && ShouldLerpToState(interpolationQueue.Peek(), currentFrame))
        {
            CurrentStateToLerpTo = interpolationQueue.Dequeue();
            modelRoot.transform.rotation = CurrentStateToLerpTo.TargetRotation;
            AdvanceLerpToPositionState(CurrentStateToLerpTo, ref movementDistanceAvailable);
            animator.SetBool("Running", true);
            return true;
        }

        return false;
    }

    private void AdvanceLerpToPositionState(InterpolationState state, ref float movementDistanceAvailable)
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

        this.transform.position = position;

        if ((this.transform.position - state.TargetPosition).magnitude <= 0.0001f)
        {
            CurrentStateToLerpTo = null;
        }
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
}
