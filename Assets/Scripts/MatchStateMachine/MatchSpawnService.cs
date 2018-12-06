using System.Collections.Generic;
using ProjectTrinity.Simulation;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchSpawnService
    {
        private Dictionary<byte, GameObject> viewUnitPrefabs = new Dictionary<byte, GameObject>();
        public Transform CameraRoot { get; set; }

        public void AddUnitPrefab(byte unitType, GameObject matchViewUnitGameobject)
        {
            viewUnitPrefabs[unitType] = matchViewUnitGameobject;
        }

        public void OnUnitSpawn(byte unitId, byte unitType, MatchSimulationUnit unitState, MatchSimulation matchSimulation, bool isLocalPlayer = false)
        {
            GameObject unitGameobject;

            if (viewUnitPrefabs.TryGetValue(unitType, out unitGameobject))
            {
                // TODO: get from pool instead
                GameObject spawnedGameObject = MonoBehaviour.Instantiate(unitGameobject);
                MatchSimulationViewUnit matchSimulationViewUnit;

                if (isLocalPlayer)
                {
                    matchSimulationViewUnit = spawnedGameObject.AddComponent<MatchSimulationLocalPlayerViewUnit>();
                    CameraRoot.transform.SetParent(spawnedGameObject.transform);
                }
                else
                {
                    matchSimulationViewUnit = spawnedGameObject.AddComponent<MatchSimulationViewUnit>();
                }

                spawnedGameObject.SetActive(true);
                matchSimulationViewUnit.OnSpawn(unitState, matchSimulation);
            }
        }
    }
}