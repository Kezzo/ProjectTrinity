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

        private int bufferCursor;
        private List<UnitStateMessage>[] unitStateMessageBuffer = new List<UnitStateMessage>[2];
        private List<PositionConfirmationMessage>[] PCMBuffer = new List<PositionConfirmationMessage>[2];

        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;

            // this is done to change the list we add new message to while we process a list
            // to one list will always be the one receiving message, while the other will be the one processed in that frame
            // we switch to the other list at the beginning of the frame
            // This also avoids having to do list copy etc. which can lead to memory fragmentation
            unitStateMessageBuffer[0] = new List<UnitStateMessage>();
            unitStateMessageBuffer[1] = new List<UnitStateMessage>();

            PCMBuffer[0] = new List<PositionConfirmationMessage>();
            PCMBuffer[1] = new List<PositionConfirmationMessage>();

            DIContainer.UDPClient.RegisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.RegisterListener(MessageId.UNIT_STATE, this);
            DIContainer.UDPClient.RegisterListener(MessageId.POSITION_CONFIRMATION, this);

            //TODO: Add other players/units
            matchSimulation = new MatchSimulation(matchStateMachine.LocalPlayerId, new byte[0], 
                                                  matchStateMachine.MatchStartTimestamp, matchStateMachine.MatchInputProvider, 
                                                  matchStateMachine.MatchEventProvider);
        }

        public void OnDeactivate()
        {
            DIContainer.UDPClient.DeregisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.DeregisterListener(MessageId.UNIT_STATE, this);
            DIContainer.UDPClient.DeregisterListener(MessageId.POSITION_CONFIRMATION, this);
        }

        public void OnFixedUpdateTick()
        {
            int currentBufferCursor = bufferCursor;
            bufferCursor = (bufferCursor + 1) % 2; // 0 -> 1 -> 0 -> 1

            matchSimulation.OnSimulationFrame(unitStateMessageBuffer[currentBufferCursor], PCMBuffer[currentBufferCursor]);
            unitStateMessageBuffer[currentBufferCursor].Clear();
            PCMBuffer[currentBufferCursor].Clear();
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
                /*DIContainer.Logger.Debug(string.Format(
                    "Received unit state message = UnitId: '{0}' XPosition: '{1}' YPosition: '{2}' Rotation: '{3}' Frame: '{4}'",
                    unitStateMessage.UnitId, unitStateMessage.XPosition, unitStateMessage.YPosition, unitStateMessage.Rotation, unitStateMessage.Frame));*/

                unitStateMessageBuffer[bufferCursor].Add(unitStateMessage);
            }

            if(message[0] == MessageId.POSITION_CONFIRMATION)
            {
                PositionConfirmationMessage positionConfirmationMessage = new PositionConfirmationMessage(message);
                PCMBuffer[bufferCursor].Add(positionConfirmationMessage);
            }
        }
    }
}