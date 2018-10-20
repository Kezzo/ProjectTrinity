using System.Collections.Generic;
using System.Text;
using ProjectTrinity.Helper;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private static readonly byte PositionFrameDelay = 10; // 4 == ~198ms

    private class InterpolationState
    {
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public byte TargetFrame;

        public byte StartFrame;

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

    private Queue<InterpolationState> interpolationQueue = new Queue<InterpolationState>();

    public bool LerpToState;

    private int movementChangedCounter = 0;
    private InterpolationState currentStateToLerpTo;

    public void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, byte frame)
    {
        if (!LerpToState)
        {
            Vector3 targetPosition = updatedUnitState.GetUnityPosition();

            animator.SetBool("Running", Vector3.Distance(transform.position, targetPosition) > 0.01f);

            transform.position = targetPosition;
            transform.rotation = updatedUnitState.GetUnityRotation();
            return;
        }


        InterpolationState stateToAdd = new InterpolationState(updatedUnitState.GetUnityPosition(), updatedUnitState.GetUnityRotation(),
                                                               (byte)MathHelper.Modulo(frame + PositionFrameDelay, byte.MaxValue));

        //DIContainer.Logger.Debug("OnUnitStateUpdate state: " + stateToAdd);

        interpolationQueue.Enqueue(stateToAdd);
    }

    public void UpdateToNextState(byte currentFrame)
    {
        if (!LerpToState)
        {
            return;
        }

        if (currentStateToLerpTo != null)
        {
            AdvanceLerpToPositionState(currentStateToLerpTo, currentFrame);

            if (currentStateToLerpTo == null)
            {
                // to ensure next lerp is started so wait frame that results in a stutter is generated.
                StartLerpingIfValid(currentFrame);
            }
        }
        else if (!StartLerpingIfValid(currentFrame))
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

            if (movementChangedCounter > 1)
            {
                animator.SetBool("Running", false);
            }
        }
    }

    private bool StartLerpingIfValid(byte currentFrame)
    {
        if (interpolationQueue.Count > 0 && ShouldLerpToState(interpolationQueue.Peek(), currentFrame))
        {
            currentStateToLerpTo = interpolationQueue.Dequeue();
            currentStateToLerpTo.StartFrame = currentFrame;
            this.transform.rotation = currentStateToLerpTo.TargetRotation;
            AdvanceLerpToPositionState(currentStateToLerpTo, currentFrame);
            animator.SetBool("Running", true);
            return true;
        }

        return false;
    }

    private void AdvanceLerpToPositionState(InterpolationState state, byte currentFrame)
    {
        movementChangedCounter = 0;

        // this is done to account for frame wrap-around
        byte frameDiffStartTarget = MathHelper.GetFrameDiff(state.StartFrame, state.TargetFrame);
        byte frameDiffStartCurrent = MathHelper.GetFrameDiff(state.StartFrame, currentFrame);

        float interpolant = Mathf.InverseLerp(0, frameDiffStartTarget, frameDiffStartCurrent);
        Vector3 position = Vector3.Lerp(this.transform.position, state.TargetPosition, interpolant);

        /*DIContainer.Logger.Debug(string.Format("AdvanceLerpToPositionState: frameDiffStartTarget: {0} frameDiffStartCurrent: {1} interpolant: {2} position: {3} CurrentFrame: {4} TargetFrame: {5}",
                                               frameDiffStartTarget, frameDiffStartCurrent, interpolant, position, currentFrame, state.TargetFrame));*/

        this.transform.position = position;

        if (MatchSimulationUnit.IsFrameInFuture(currentFrame, state.TargetFrame) || currentFrame == state.TargetFrame || interpolant >= 1f)
        {
            // just to be sure we're on the correct position even when interpolant isn't 1 yet.
            this.transform.position = state.TargetPosition;
            currentStateToLerpTo = null;
            animator.SetBool("Running", false);
        }
    }

    private bool ShouldLerpToState(InterpolationState state, byte currentFrame)
    {
        // is state in past or current frame? Should never happen.
        // TODO: Maybe always use for movement?
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
