using System;
using System.Collections.Generic;
using ProjectTrinity.Simulation;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchEventProvider
    {
        private Dictionary<byte, MatchSimulationViewUnit> viewUnits = new Dictionary<byte, MatchSimulationViewUnit>();

        public void AddUnitStateUpdateListener(byte unitID, MatchSimulationViewUnit matchViewUnit)
        {
            viewUnits[unitID] = matchViewUnit;
        }


        public void OnUnitSpawn(byte unitId, bool isLocalPlayer = false)
        {
            MatchSimulationViewUnit unit;

            if (viewUnits.TryGetValue(unitId, out unit))
            {
                unit.gameObject.SetActive(true);
                unit.LerpToState = !isLocalPlayer;
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

        public void OnSimulationFrame(byte frame)
        { 
            foreach (var viewUnit in viewUnits)
            {
                viewUnit.Value.UpdateToNextState(frame);
            }
        }
    }
}