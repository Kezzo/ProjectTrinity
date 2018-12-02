using System;
using ProjectTrinity.Debugging;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.UI;
using UniRx;
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
        private bool releaseTriggersSkill;
        private float lastAimingRotation;

        public void Initialize(MatchStateMachine.MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;

            IObservable<long> inputObservable = Observable.EveryUpdate()
                .Where(_ => matchStateMachine != null && matchStateMachine.CurrentMatchState.Value is RunningMatchState);

            IObservable<long> joyStickInput = inputObservable
                .Where(_ => movementJoyStick.JoystickActive && (Mathf.Abs(movementJoyStick.Horizontal) > 0.2f || Mathf.Abs(movementJoyStick.Vertical) > 0.2f))
                .Do(_ =>
                {
                    matchStateMachine.MatchInputProvider.AddXTranslation(movementJoyStick.Horizontal);
                    matchStateMachine.MatchInputProvider.AddYTranslation(movementJoyStick.Vertical);
                });

            IObservable<long> debugAiInput = inputObservable
                .Where(_ => UnitDebugAI.DebugAIEnabled)
                .Do(_ =>
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        matchStateMachine.MatchInputProvider.AddYTranslation(UnityEngine.Random.Range(-1f, 1f));
                    }
                    else
                    {
                        matchStateMachine.MatchInputProvider.AddXTranslation(UnityEngine.Random.Range(-1f, 1f));
                    }
                });

            IObservable<long> keyboardInput = inputObservable
                .Where(_ => !movementJoyStick.JoystickActive && !UnitDebugAI.DebugAIEnabled &&
                    (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.S) ||
                    UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.D)))
                .Do(_ =>
                {
                    if (UnityEngine.Input.GetKey(KeyCode.W))
                    {
                        matchStateMachine.MatchInputProvider.AddYTranslation(-1f);
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S))
                    {
                        matchStateMachine.MatchInputProvider.AddYTranslation(1f);
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A))
                    {
                        matchStateMachine.MatchInputProvider.AddXTranslation(1f);
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D))
                    {
                        matchStateMachine.MatchInputProvider.AddXTranslation(-1f);
                    }
                });

            // apply rotation if any input was generated
            joyStickInput.Merge(debugAiInput, keyboardInput)
                .Subscribe(_ =>
                {
                    Quaternion rotation = Quaternion.LookRotation(
                        new Vector3(matchStateMachine.MatchInputProvider.XTranslation, 0f,
                            matchStateMachine.MatchInputProvider.YTranslation), Vector3.up);

                    matchStateMachine.MatchInputProvider.AddRotation(rotation.eulerAngles.y);
                })
                .AddTo(this);

            inputObservable
                .Where(_ => aimingJoyStick.JoystickActive)
                .Subscribe(_ =>
                {
                    if (Mathf.Abs(aimingJoyStick.Horizontal) > 0.5f || Mathf.Abs(aimingJoyStick.Vertical) > 0.5f)
                    {
                        Quaternion rotation = Quaternion.LookRotation(new Vector3(aimingJoyStick.Horizontal, 0f, aimingJoyStick.Vertical), Vector3.up);
                        lastAimingRotation = rotation.eulerAngles.y;
                        matchStateMachine.MatchInputProvider.AddAimingRotation(rotation.eulerAngles.y);
                        releaseTriggersSkill = true;
                    }
                    else
                    {
                        releaseTriggersSkill = false;
                    }
                })
                .AddTo(this);

            inputObservable
                .Where(_ => !aimingJoyStick.JoystickActive && releaseTriggersSkill)
                .Subscribe(_ =>
                {
                    matchStateMachine.MatchInputProvider.AddAbilityInput(lastAimingRotation);
                    releaseTriggersSkill = false;
                })
                .AddTo(this);
        }
    }
}