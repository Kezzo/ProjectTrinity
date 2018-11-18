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
        private List<UnitAbilityActivationMessage>[] unitAbilityMessageBuffer = new List<UnitAbilityActivationMessage>[2];
        private List<UnitSpawnMessage>[] unitSpawnMessageBuffer = new List<UnitSpawnMessage>[2];

        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            this.matchStateMachine.MatchInputProvider.Reset();

            // this is done to change the list we add new messages to while we process a list
            // so one list will always be the one receiving messages, while the other will be processed in that frame
            // we switch to the other list at the beginning of the frame
            // This also avoids having to do list copy etc. which can lead to memory fragmentation
            unitStateMessageBuffer[0] = new List<UnitStateMessage>();
            unitStateMessageBuffer[1] = new List<UnitStateMessage>();

            PCMBuffer[0] = new List<PositionConfirmationMessage>();
            PCMBuffer[1] = new List<PositionConfirmationMessage>();

            unitAbilityMessageBuffer[0] = new List<UnitAbilityActivationMessage>();
            unitAbilityMessageBuffer[1] = new List<UnitAbilityActivationMessage>();

            unitSpawnMessageBuffer[0] = new List<UnitSpawnMessage>();
            unitSpawnMessageBuffer[1] = new List<UnitSpawnMessage>();

            this.matchStateMachine.UDPClient.RegisterListener(MessageId.MATCH_END, this);
            this.matchStateMachine.UDPClient.RegisterListener(MessageId.UNIT_STATE, this);
            this.matchStateMachine.UDPClient.RegisterListener(MessageId.POSITION_CONFIRMATION, this);
            this.matchStateMachine.UDPClient.RegisterListener(MessageId.UNIT_ABILITY_ACTIVATION, this);
            this.matchStateMachine.UDPClient.RegisterListener(MessageId.UNIT_SPAWN, this);

            matchSimulation = new MatchSimulation(matchStateMachine.LocalPlayerId, matchStateMachine.MatchStartTimestamp, matchStateMachine.MatchInputProvider, 
                                                  matchStateMachine.MatchEventProvider, matchStateMachine.UDPClient, matchStateMachine.NetworkTimeService);
        }

        public void OnDeactivate()
        {
            this.matchStateMachine.UDPClient.DeregisterListener(MessageId.MATCH_END, this);
            this.matchStateMachine.UDPClient.DeregisterListener(MessageId.UNIT_STATE, this);
            this.matchStateMachine.UDPClient.DeregisterListener(MessageId.POSITION_CONFIRMATION, this);
            this.matchStateMachine.UDPClient.DeregisterListener(MessageId.UNIT_ABILITY_ACTIVATION, this);
            this.matchStateMachine.UDPClient.DeregisterListener(MessageId.UNIT_SPAWN, this);
        }

        public void OnFixedUpdateTick()
        {
            int currentBufferCursor = bufferCursor;
            bufferCursor = (bufferCursor + 1) % 2; // 0 -> 1 -> 0 -> 1

            matchSimulation.OnSimulationFrame(unitStateMessageBuffer[currentBufferCursor], 
                                              PCMBuffer[currentBufferCursor], 
                                              unitAbilityMessageBuffer[currentBufferCursor],
                                              unitSpawnMessageBuffer[currentBufferCursor]);

            unitStateMessageBuffer[currentBufferCursor].Clear();
            PCMBuffer[currentBufferCursor].Clear();
            unitAbilityMessageBuffer[currentBufferCursor].Clear();
            unitSpawnMessageBuffer[currentBufferCursor].Clear();
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

            if (message[0] == MessageId.UNIT_ABILITY_ACTIVATION)
            {
                UnitAbilityActivationMessage unitAbilityActivationMessage = new UnitAbilityActivationMessage(message);
                unitAbilityMessageBuffer[bufferCursor].Add(unitAbilityActivationMessage);
            }

            if (message[0] == MessageId.UNIT_SPAWN)
            {
                UnitSpawnMessage unitSpawnMessage = new UnitSpawnMessage(message);
                unitSpawnMessageBuffer[bufferCursor].Add(unitSpawnMessage);
            }
        }
    }
}