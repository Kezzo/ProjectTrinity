using System;
using ProjectTrinity.Root;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchStateMachine
    {
        private IMatchState currentMatchState;
        public byte LocalPlayerId { get; set; }
        public Int64 MatchStartTimestamp { get; set; }
        public MatchInputProvider MatchInputProvider { get; private set; }
        public MatchEventProvider MatchEventProvider { get; private set; }

        public MatchStateMachine()
        {
            MatchInputProvider = new MatchInputProvider();
            MatchEventProvider = new MatchEventProvider();
            ChangeMatchState(new TimeSyncMatchState());
        }

        public void ChangeMatchState(IMatchState matchState)
        {
            DIContainer.Logger.Debug(string.Format("Switching to {0}", matchState.GetType()));

            if(currentMatchState != null)
            {
                currentMatchState.OnDeactivate();
            }

            currentMatchState = matchState;
            currentMatchState.OnActivate(this);
        }

        public void OnFixedUpdateTick() 
        {
            currentMatchState.OnFixedUpdateTick();
        }
    }
}