using System;
using System.Collections.Generic;
using ProjectTrinity.Input;
using ProjectTrinity.MatchStateMachine;
using TMPro;
using UniRx;
using UnityEngine;

namespace ProjectTrinity.Root
{
    public class Root : MonoBehaviour
    {
        [Serializable]
        private class UnitTypePrefab
        {
            public byte UnitType;
            public GameObject Prefab;
        }

        [SerializeField]
        private List<UnitTypePrefab> unitTypePrefabs;

        [SerializeField]
        private TextMeshProUGUI rttText;

        [SerializeField]
        private GameObject selectMatchTypeRoot;

        [SerializeField]
        private GameObject lookingForMatchUI;

        [SerializeField]
        private GameObject matchEndedUI;

        [SerializeField]
        private Transform cameraRoot;

        [SerializeField]
        private ViewMatchInputRegistry viewMatchInputRegistry;

        private MatchStateMachine.MatchStateMachine matchStateMachine;
        private bool parentedPlayerCamera;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;
            matchStateMachine = new MatchStateMachine.MatchStateMachine();
            matchStateMachine.MatchEventProvider.CameraRoot = cameraRoot;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    rttText.text = string.Format("RTT: {0}ms", 
                        matchStateMachine.RoundTripTimeService != null ? matchStateMachine.RoundTripTimeService.LastPing : 0);
                });

            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    matchStateMachine.OnFixedUpdateTick();
                });

            matchStateMachine.CurrentMatchState.ObserveOnMainThread().Subscribe(matchState =>
            {
                selectMatchTypeRoot.SetActive(matchState is IdleMatchState);

                lookingForMatchUI.SetActive(matchState is JoinMatchState ||
                                            matchState is TimeSyncMatchState ||
                                            matchState is WaitForStartMatchState);

                matchEndedUI.SetActive(matchState is EndMatchState);
            });
        }

        public void FindMatch(int playerCount)
        {
            if (!(matchStateMachine.CurrentMatchState.Value is IdleMatchState))
            {
                return;
            }

            matchStateMachine.ChangeMatchState(new JoinMatchState(playerCount));

            foreach (UnitTypePrefab unitPrefab in unitTypePrefabs)
            {
                matchStateMachine.MatchEventProvider.AddUnitPrefab(unitPrefab.UnitType, unitPrefab.Prefab);
            }

            viewMatchInputRegistry.Initialize(matchStateMachine);
        }
    }
}