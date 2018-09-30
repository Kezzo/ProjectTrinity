using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;

namespace ProjectTrinity.MatchStateMachine
{
    public class RunningMatchState : IMatchState, IUdpMessageListener
    {
        private MatchStateMachine matchStateMachine;
        private MatchSimulation matchSimulation;

        public void Initialize(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            DIContainer.UDPClient.RegisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.RegisterListener(MessageId.UNIT_STATE, this);

            matchSimulation = new MatchSimulation();
        }

        public void OnFixedUpdateTick()
        {
            matchSimulation.OnSimulationFrame();
        }

        public void OnMessageReceived(byte[] message)
        {
            if(message[0] == MessageId.MATCH_END) 
            {
                DIContainer.Logger.Debug("Match end message received, switching to MatchEndState");
                this.matchStateMachine.ChangeMatchState(new EndMatchState());
                return;
            }

            if(message[0] == MessageId.UNIT_STATE)
            {
                //TODO: put all message into FIFO buffer and feed into simulation on next tick.
                UnitStateMessage unitStateMessage = new UnitStateMessage(message);
                DIContainer.Logger.Debug(string.Format(
                    "Received unit state message = UnitId: '{0}' XPosition: '{1}' YPosition: '{2}' Rotation: '{3}' Frame: '{4}'",
                    unitStateMessage.UnitId, unitStateMessage.XPosition, unitStateMessage.YPosition, unitStateMessage.Rotation, unitStateMessage.Frame));

            }
        }
    }
}