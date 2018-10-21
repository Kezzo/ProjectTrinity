using ProjectTrinity.Debugging;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.UI;
using UnityEngine;

namespace ProjectTrinity.Input
{
    public class ViewMatchInputRegistry : MonoBehaviour
    {
        [SerializeField]
        private VirtualJoystick movementJoyStick;

        [SerializeField]
        private VirtualJoystick aimingJoyStick;

        private MatchStateMachine.MatchStateMachine matchStateMachine;

        public void Initialize(MatchStateMachine.MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
        }

        private void Update()
        {
            if (matchStateMachine == null || !(matchStateMachine.CurrentMatchState is RunningMatchState))
            {
                return;
            }


            if (movementJoyStick.JoystickActive && (Mathf.Abs(movementJoyStick.Horizontal) > 0.2f || Mathf.Abs(movementJoyStick.Vertical) > 0.2f))
            {
                matchStateMachine.MatchInputProvider.AddXTranslation(movementJoyStick.Horizontal);
                matchStateMachine.MatchInputProvider.AddYTranslation(movementJoyStick.Vertical);
            }
            else if (UnitDebugAI.DebugAIEnabled)
            {
                if (Random.Range(0, 2) == 0)
                {
                    matchStateMachine.MatchInputProvider.AddYTranslation(Random.Range(-1f, 1f));
                }
                else
                {
                    matchStateMachine.MatchInputProvider.AddXTranslation(Random.Range(-1f, 1f));
                }
            }
            else
            {
                if (UnityEngine.Input.GetKey(KeyCode.W))
                {
                    matchStateMachine.MatchInputProvider.AddYTranslation(1f);
                }
                else if (UnityEngine.Input.GetKey(KeyCode.S))
                {
                    matchStateMachine.MatchInputProvider.AddYTranslation(-1f);
                }

                if (UnityEngine.Input.GetKey(KeyCode.A))
                {
                    matchStateMachine.MatchInputProvider.AddXTranslation(-1f);
                }
                else if (UnityEngine.Input.GetKey(KeyCode.D))
                {
                    matchStateMachine.MatchInputProvider.AddXTranslation(1f);
                }
            }

            if (Mathf.Abs(matchStateMachine.MatchInputProvider.XTranslation) > 0f || Mathf.Abs(matchStateMachine.MatchInputProvider.YTranslation) > 0f)
            {
                Quaternion rotation = Quaternion.LookRotation(
                new Vector3(matchStateMachine.MatchInputProvider.XTranslation, 0f,
                            matchStateMachine.MatchInputProvider.YTranslation), Vector3.up);

                matchStateMachine.MatchInputProvider.AddRotation(rotation.eulerAngles.y);
            }

            if (aimingJoyStick.JoystickActive && (Mathf.Abs(aimingJoyStick.Horizontal) > 0.5f || Mathf.Abs(aimingJoyStick.Vertical) > 0.5f))
            {
                Quaternion rotation = Quaternion.LookRotation(new Vector3(aimingJoyStick.Horizontal, 0f, aimingJoyStick.Vertical), Vector3.up);
                matchStateMachine.MatchInputProvider.AddAimingRotation(rotation.eulerAngles.y);
            }
        }
    }
}