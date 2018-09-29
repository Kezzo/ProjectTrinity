namespace ProjectTrinity.MatchStateMachine
{
    public interface IMatchState
    {
        void Initialize(MatchStateMachine matchStateMachine);
        void OnFixedUpdateTick();
    }
}