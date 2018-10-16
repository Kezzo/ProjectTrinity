using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    private static readonly int PositionLerpTime = 150;

    private class InterpolationState
    {
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public long TargetTime;

        public Vector3 StartPosition;

        public InterpolationState(Vector3 targetPosition, Quaternion targetRotation, long targetTime)
        {
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;
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
            UpdatePosition(updatedUnitState.GetUnityPosition(), 1f);
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
            currentState.StartPosition = transform.position;

            lastUpdateTime = currentTime;
        }

        if (currentState != null)
        {
            float start = lastUpdateTime - startTime;
            float end = currentState.TargetTime - startTime;
            float value = (currentTime + 33) - startTime;
            float interpolant = Mathf.InverseLerp(start, end, value);

            UpdatePosition(Vector3.Lerp(currentState.StartPosition, currentState.TargetPosition, interpolant), interpolant);
            transform.rotation = currentState.TargetRotation;
        }
    }

    private void UpdatePosition(Vector3 position, float interpolant)
    {
        float positionDiff = Vector3.Distance(transform.position, position);
        bool running = positionDiff > 0 || interpolant < 1;
        animator.SetBool("Running", running);
        transform.position = position;
    }
}
