using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    private static readonly int PositionLerpTime = 200;

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

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        startTime = UtcTimestampHelper.GetCurrentUtcMsTimestamp();
    }

    public void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, bool lerpToState)
    {
        this.lerpToState = lerpToState;
        if (!lerpToState)
        {
            UpdatePosition(updatedUnitState.GetUnityPosition());
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

            UpdatePosition(Vector3.Lerp(transform.position, currentState.Position, interpolant));
            transform.rotation = Quaternion.Lerp(transform.rotation, currentState.Rotation, interpolant);
        }
    }

    private void UpdatePosition(Vector3 position)
    {
        float positionDiff = Vector3.Distance(transform.position, position);
        animator.SetBool("Running", positionDiff > 0);
        transform.position = position;
    }
}
