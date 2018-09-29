using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchStateMachine
    {
        private IMatchState currentMatchState;
        public byte LocalPlayerId { get; set; }

        public MatchStateMachine()
        {
            ChangeMatchState(new TimeSyncMatchState());
        }

        public void ChangeMatchState(IMatchState matchState)
        {
            Debug.Log(string.Format("Switching to {0}", matchState.GetType()));

            currentMatchState = matchState;
            currentMatchState.Initialize(this);
        }

        public void OnFixedUpdateTick() 
        {
            currentMatchState.OnFixedUpdateTick();
        }
    }
}