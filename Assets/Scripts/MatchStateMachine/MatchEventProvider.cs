using System.Collections.Generic;
using ProjectTrinity.Simulation;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchEventProvider
    {
        private Dictionary<byte, GameObject> viewUnitGameobjects = new Dictionary<byte, GameObject>();
        private Dictionary<byte, MatchSimulationViewUnit> viewUnits = new Dictionary<byte, MatchSimulationViewUnit>();

        public void AddUnitStateUpdateListener(byte unitID, GameObject matchViewUnitGameobject)
        {
            viewUnitGameobjects[unitID] = matchViewUnitGameobject;
        }

        public void OnUnitSpawn(byte unitId, bool isLocalPlayer = false)
        {
            GameObject unitGameobject;

            if (viewUnitGameobjects.TryGetValue(unitId, out unitGameobject))
            {
                MatchSimulationViewUnit matchSimulationViewUnit;

                if (isLocalPlayer)
                {
                    matchSimulationViewUnit = unitGameobject.AddComponent<MatchSimulationLocalPlayerViewUnit>();
                }
                else
                {
                    matchSimulationViewUnit = unitGameobject.AddComponent<MatchSimulationViewUnit>();
                }

                unitGameobject.SetActive(true);

                viewUnits[unitId] = matchSimulationViewUnit;
            }
        }

        public void OnUnitStateUpdate(MatchSimulationUnit unitState, byte frame)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitState.UnitId, out unit))
            {
                unit.OnUnitStateUpdate(unitState, frame);
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

        public void OnSpellActivation(byte unitId, float rotation, byte startFrame, byte activationFrame)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitId, out unit))
            {
                unit.OnSpellActivation(rotation, startFrame, activationFrame);
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