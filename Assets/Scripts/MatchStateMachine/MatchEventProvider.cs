using System;
using System.Collections.Generic;
using ProjectTrinity.Simulation;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchEventProvider
    {
        private Dictionary<byte, Action<MatchSimulationUnit>> listener = new Dictionary<byte, Action<MatchSimulationUnit>>();

        public void AddUnitStateUpdateListener(byte unitID, Action<MatchSimulationUnit> callback)
        {
            listener[unitID] = callback;
        }

        public void OnUnitStateUpdate(MatchSimulationUnit unitState)
        {
            Action<MatchSimulationUnit> callback;

            if (listener.TryGetValue(unitState.UnitId, out callback))
            {
                callback(unitState);
            }
        }
    }
}