using System.Collections.Generic;
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

        private List<UnitStateMessage> unitStateMessageBuffer = new List<UnitStateMessage>();
        private List<PositionConfirmationMessage> positionConfirmationMessageBuffer = new List<PositionConfirmationMessage>();

        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            DIContainer.UDPClient.RegisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.RegisterListener(MessageId.UNIT_STATE, this);
            DIContainer.UDPClient.RegisterListener(MessageId.POSITION_CONFIRMATION, this);

            //TODO: Add other players/units
            matchSimulation = new MatchSimulation(matchStateMachine.LocalPlayerId, new byte[0], matchStateMachine.MatchInputProvider, matchStateMachine.MatchEventProvider);
        }

        public void OnDeactivate()
        {
            DIContainer.UDPClient.DeregisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.DeregisterListener(MessageId.UNIT_STATE, this);
            DIContainer.UDPClient.DeregisterListener(MessageId.POSITION_CONFIRMATION, this);
        }

        public void OnFixedUpdateTick()
        {
            matchSimulation.OnSimulationFrame(unitStateMessageBuffer, positionConfirmationMessageBuffer);
            unitStateMessageBuffer.Clear();
            positionConfirmationMessageBuffer.Clear();
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
                UnitStateMessage unitStateMessage = new UnitStateMessage(message);
                DIContainer.Logger.Debug(string.Format(
                    "Received unit state message = UnitId: '{0}' XPosition: '{1}' YPosition: '{2}' Rotation: '{3}' Frame: '{4}'",
                    unitStateMessage.UnitId, unitStateMessage.XPosition, unitStateMessage.YPosition, unitStateMessage.Rotation, unitStateMessage.Frame));

                unitStateMessageBuffer.Add(unitStateMessage);
            }

            if(message[0] == MessageId.POSITION_CONFIRMATION)
            {
                PositionConfirmationMessage positionConfirmationMessage = new PositionConfirmationMessage(message);

                if(positionConfirmationMessageBuffer.Count == 0)
                {
                    positionConfirmationMessageBuffer.Add(positionConfirmationMessage);
                }
                else if(positionConfirmationMessageBuffer.Count > 0 && positionConfirmationMessage.Frame > positionConfirmationMessageBuffer[0].Frame)
                {
                    positionConfirmationMessageBuffer[0] = positionConfirmationMessage;
                }
            }
        }
    }
}