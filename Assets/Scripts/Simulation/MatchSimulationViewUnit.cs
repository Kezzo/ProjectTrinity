using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    private static readonly int PositionLerpTime = 100;

    private class InterpolationState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public long TargetTime;

        public InterpolationState(Vector3 position, Quaternion rotation, long targetTime)
        {
            Position = position;
            Rotation = rotation;
            TargetTime = targetTime;
        }
    }

    private Queue<InterpolationState> interpolationQueue = new Queue<InterpolationState>();
    private InterpolationState currentState;
    private long lastUpdateTime;
    private bool lerpToState;
    private long startTime;

    private void Start()
    {
        startTime = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
    }

    public void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, bool lerpToState)
    {
        this.lerpToState = lerpToState;
        if (!lerpToState)
        {
            transform.position = updatedUnitState.GetUnityPosition();
            transform.rotation = updatedUnitState.GetUnityRotation();
            return;
        }

        interpolationQueue.Enqueue(new InterpolationState(updatedUnitState.GetUnityPosition(),
                                                          updatedUnitState.GetUnityRotation(),
                                                          UtcTimestampHelper.GetCurrentUtcMsTimestamp() + PositionLerpTime));
    }

    private void Update()
    {
        if (!this.lerpToState)
        {
            return;
        }

        long currentTime = UtcTimestampHelper.GetCurrentUtcMsTimestamp();

        // next state target is 100ms or less in the future, so start interpolating to that state.
        if (interpolationQueue.Count > 0 && (currentTime + PositionLerpTime) >= interpolationQueue.Peek().TargetTime)
        {
            currentState = interpolationQueue.Dequeue();
            lastUpdateTime = currentTime;
        }

        if (currentState != null)
        {
            float start = lastUpdateTime - startTime;
            float end = currentState.TargetTime - startTime;
            float value = (currentTime + 33) - startTime;
            float interpolant = Mathf.InverseLerp(start, end, value);
            transform.position = Vector3.Lerp(transform.position, currentState.Position, interpolant);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentState.Rotation, interpolant);
        }
    }
}
