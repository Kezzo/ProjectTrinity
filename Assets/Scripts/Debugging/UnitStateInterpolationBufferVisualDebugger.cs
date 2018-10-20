using UnityEngine;

namespace ProjectTrinity.Debugging
{
    public class UnitStateInterpolationBufferVisualDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        private MatchSimulationViewUnit[] viewUnits;

        private void Update()
        {
            viewUnits = FindObjectsOfType<MatchSimulationViewUnit>();
        }

        private void OnDrawGizmos()
        {
            if (viewUnits == null || viewUnits.Length == 0)
            {
                return;
            }

            for (int i = 0; i < viewUnits.Length; i++)
            {
                if (viewUnits[i].CurrentStateToLerpTo != null)
                {
                    Gizmos.color = Color.green;

                    Gizmos.DrawSphere(viewUnits[i].CurrentStateToLerpTo.TargetPosition, 0.25f);
                }

                Gizmos.color = Color.red;

                MatchSimulationViewUnit.InterpolationState[] currentBuffer = viewUnits[i].CurrentInterpolationBuffer;

                for (int j = 0; j < currentBuffer.Length; j++)
                {
                    Gizmos.DrawSphere(currentBuffer[j].TargetPosition, 0.25f);
                }
            }
        }
#endif
    }
}