using System;
using ProjectTrinity.Input;
using ProjectTrinity.Root;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchStateMachine
    {
        public IMatchState CurrentMatchState { get; private set; }
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

            if(CurrentMatchState != null)
            {
                CurrentMatchState.OnDeactivate();
            }

            CurrentMatchState = matchState;
            CurrentMatchState.OnActivate(this);
        }

        public void OnFixedUpdateTick() 
        {
            CurrentMatchState.OnFixedUpdateTick();
        }
    }
}