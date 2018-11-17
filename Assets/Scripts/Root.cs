﻿using System;
using System.Collections.Generic;
using ProjectTrinity.Input;
using ProjectTrinity.MatchStateMachine;
using TMPro;
using UnityEngine;

namespace ProjectTrinity.Root
{
    public class Root : MonoBehaviour
    {
        [Serializable]
        private class UnitGameobject
        {
            public byte UnitId;
            public GameObject Gameobject;
        }

        [SerializeField]
        private List<UnitGameobject> unitPrefabs;

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

                foreach (UnitGameobject unitPrefab in unitPrefabs)
                {
                    if(matchStateMachine.LocalPlayerId == unitPrefab.UnitId)
                    {
                        cameraRoot.transform.SetParent(unitPrefab.Gameobject.transform);
                        break;
                    }
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

            foreach (UnitGameobject unitPrefab in unitPrefabs)
            {
                matchStateMachine.MatchEventProvider.AddUnitStateUpdateListener(unitPrefab.UnitId, unitPrefab.Gameobject);
            }

            viewMatchInputRegistry.Initialize(matchStateMachine);
        }
    }
}