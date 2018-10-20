using UnityEngine;

namespace ProjectTrinity.Debugging
{
    public class UnitDebugAI : MonoBehaviour
    {
        [SerializeField]
        private bool enabledDebugAI;
        public static bool DebugAIEnabled { get; private set; }

        private void Awake()
        {
            DebugAIEnabled = enabledDebugAI;
        }
    }
}