using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using UnityEngine;

public class MatchSimulationViewUnit : MonoBehaviour
{
    private static readonly int PositionLerpTime = 100;

    private Vector3 lastNetworkPosition;
    private Quaternion lastNetworkRotation;
    private long lastReceivedUpdate;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private long nextLerpTargetTime;

    public void OnUnitStateUpdate(MatchSimulationUnit updatedUnitState, bool lerpToState)
    {
        long currentTimestamp = DIContainer.NetworkTimeService.NetworkTimestampMs;
        lastNetworkPosition = networkPosition;
        lastNetworkRotation = networkRotation;
        lastReceivedUpdate = nextLerpTargetTime;

        nextLerpTargetTime = lerpToState ? currentTimestamp + PositionLerpTime : currentTimestamp;
        networkPosition = updatedUnitState.GetUnityPosition();
        networkRotation = updatedUnitState.GetUnityRotation();
    }

    private void Update()
    {
        long currentTimestamp = DIContainer.NetworkTimeService.NetworkTimestampMs;
        if (currentTimestamp >= nextLerpTargetTime)
        {
            transform.position = networkPosition;
            transform.rotation = networkRotation;
            return;
        }

        float interpolant = Mathf.InverseLerp(lastReceivedUpdate, nextLerpTargetTime, currentTimestamp);

        transform.position = Vector3.Lerp(lastNetworkPosition, networkPosition, interpolant);
        transform.rotation = Quaternion.Lerp(lastNetworkRotation, networkRotation, interpolant);

        DIContainer.Logger.Debug("Interpolant: " + interpolant);
    }
}
