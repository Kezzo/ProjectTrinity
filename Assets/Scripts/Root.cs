using ProjectTrinity.Input;
using ProjectTrinity.MatchStateMachine;
using TMPro;
using UnityEngine;

namespace ProjectTrinity.Root
{
    public class Root : MonoBehaviour
    {
        [SerializeField]
        private GameObject player;

        [SerializeField]
        private GameObject player2;

        [SerializeField]
        private GameObject player3;

        [SerializeField]
        private TextMeshProUGUI rttText;

        [SerializeField]
        private GameObject selectMatchTypeRoot;

        [SerializeField]
        private GameObject lookingForMatchUI;

        [SerializeField]
        private GameObject matchEndedUI;

        [SerializeField]
        private GameObject cameraRoot;

        [SerializeField]
        private ViewMatchInputRegistry viewMatchInputRegistry;

        private MatchStateMachine.MatchStateMachine matchStateMachine;
        private bool parentedPlayerCamera;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;
            matchStateMachine = new MatchStateMachine.MatchStateMachine();
        }

        private void FixedUpdate()
        {
            matchStateMachine.OnFixedUpdateTick();
        }

        private void Update()
        {
            rttText.text = string.Format("RTT: {0}ms", matchStateMachine.RoundTripTimeService != null ? matchStateMachine.RoundTripTimeService.LastPing : 0);

            selectMatchTypeRoot.SetActive(matchStateMachine.CurrentMatchState is IdleMatchState);

            lookingForMatchUI.SetActive(matchStateMachine.CurrentMatchState is JoinMatchState ||
                                        matchStateMachine.CurrentMatchState is TimeSyncMatchState ||
                                        matchStateMachine.CurrentMatchState is WaitForStartMatchState);

            matchEndedUI.SetActive(matchStateMachine.CurrentMatchState is EndMatchState);

            if (!parentedPlayerCamera && matchStateMachine.CurrentMatchState is WaitForStartMatchState)
            {
                parentedPlayerCamera = true;
                switch (matchStateMachine.LocalPlayerId)
                {
                    case 0:
                        cameraRoot.transform.SetParent(player.transform);
                        break;
                    case 1:
                        cameraRoot.transform.SetParent(player2.transform);
                        break;
                    case 2:
                        cameraRoot.transform.SetParent(player3.transform);
                        break;
                }
            }
        }

        public void FindMatch(int playerCount)
        {
            if (!(matchStateMachine.CurrentMatchState is IdleMatchState))
            {
                return;
            }

            matchStateMachine.ChangeMatchState(new JoinMatchState(playerCount));
            matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(0, player);
            matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(1, player2);
            matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(2, player3);

            viewMatchInputRegistry.Initialize(matchStateMachine);
        }
    }
}