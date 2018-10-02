namespace ProjectTrinity.MatchStateMachine
{
    public interface IMatchState
    {
        void OnActivate(MatchStateMachine matchStateMachine);
        void OnDeactivate();
        void OnFixedUpdateTick();
    }
}