namespace ProjectTrinity.MatchStateMachine
{
    public interface IMatchState
    {
        void OnSimulationFrame();
        void HandleMessage(byte[] message);
    }
}