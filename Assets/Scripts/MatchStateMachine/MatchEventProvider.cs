using System;
using System.Collections.Generic;
using ProjectTrinity.Simulation;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchEventProvider
    {
        private Dictionary<byte, Action<MatchSimulationUnit, bool>> listener = new Dictionary<byte, Action<MatchSimulationUnit, bool>>();

        public void AddUnitStateUpdateListener(byte unitID, Action<MatchSimulationUnit, bool> callback)
        {
            listener[unitID] = callback;
        }

        public void OnUnitStateUpdate(MatchSimulationUnit unitState, bool lerpToState = true)
        {
            Action<MatchSimulationUnit, bool> callback;

            if (listener.TryGetValue(unitState.UnitId, out callback))
            {
                callback(unitState, lerpToState);
            }
        }
    }
}