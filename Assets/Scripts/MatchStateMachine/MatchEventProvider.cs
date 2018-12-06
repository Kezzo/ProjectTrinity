using System.Collections.Generic;
using ProjectTrinity.Simulation;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchEventProvider
    {
        private Dictionary<byte, GameObject> viewUnitPrefabs = new Dictionary<byte, GameObject>();
        private Dictionary<byte, MatchSimulationViewUnit> viewUnits = new Dictionary<byte, MatchSimulationViewUnit>();
        public Transform CameraRoot { get; set; }

        public void AddUnitPrefab(byte unitType, GameObject matchViewUnitGameobject)
        {
            viewUnitPrefabs[unitType] = matchViewUnitGameobject;
        }

        public void OnUnitSpawn(byte unitId, byte unitType, MatchSimulationUnit unitState, bool isLocalPlayer = false)
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
                matchSimulationViewUnit.OnSpawn(unitState);

                viewUnits[unitId] = matchSimulationViewUnit;
            }
        }

        public void OnUnitStateUpdate(MatchSimulationUnit unitState, byte frame)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitState.UnitId, out unit))
            {
                unit.OnPositionRotationUpdate(unitState, frame);
            }
        }

        public void OnLocalAimingUpdate(byte unitId, float rotation)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitId, out unit))
            {
                unit.OnLocalAimingUpdate(rotation);
            }
        }

        public void OnAbilityActivation(byte unitId, float rotation, byte startFrame, byte activationFrame)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitId, out unit))
            {
                unit.OnAbilityActivation(rotation, startFrame, activationFrame);
            }
        }

        public void OnSimulationFrame(byte frame)
        { 
            foreach (var viewUnit in viewUnits)
            {
                viewUnit.Value.UpdateToNextState(frame);
            }
        }
    }
}