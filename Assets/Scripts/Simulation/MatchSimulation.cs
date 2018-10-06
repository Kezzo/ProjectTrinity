using System;
using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

namespace ProjectTrinity.Simulation 
{
    public class MatchSimulation
    {
        private Dictionary<byte, MatchSimulationUnit> simulationUnits = new Dictionary<byte, MatchSimulationUnit>();
        private MatchSimulationLocalPlayer localPlayer;

        private MatchInputProvider inputProvider;
        private MatchEventProvider eventProvider;
        private byte currentSimulationFrame;

        private static readonly int playerMaxFrameSpeed = 100;

        private Int64 matchStartTimestamp;

        public MatchSimulation(byte localPlayerUnitID, byte[] matchUnitIDs, Int64 matchStartTimestamp, MatchInputProvider matchInputProvider, MatchEventProvider matchEventProvider)
        {
            localPlayer = new MatchSimulationLocalPlayer(localPlayerUnitID, 0, 0, 0, 0);
            this.matchStartTimestamp = matchStartTimestamp;
            inputProvider = matchInputProvider;
            eventProvider = matchEventProvider;

            foreach (byte matchUnitID in matchUnitIDs)
            {
                if(matchUnitID == localPlayerUnitID)
                {
                    continue;
                }

                simulationUnits.Add(matchUnitID, new MatchSimulationUnit(matchUnitID, 0, 0, 0, 0));
            }
        }

        public void OnSimulationFrame(List<UnitStateMessage> receivedUnitStateMessagesSinceLastFrame, List<PositionConfirmationMessage> receivedPositionConfirmationMessagesSinceLastFrame)
        {
            currentSimulationFrame = (byte)MathHelper.Modulo((DIContainer.NetworkTimeService.NetworkTimestampMs - matchStartTimestamp) / 33, byte.MaxValue);

            // sort by oldest frame to newest frame
            receivedUnitStateMessagesSinceLastFrame.Sort((message1, message2) =>
            {
                return message1.Frame == message2.Frame ? 0 : MatchSimulationUnit.IsFrameInFuture(message1.Frame, message2.Frame) ? 1 : -1;
            });

            for (int i = 0; i < receivedUnitStateMessagesSinceLastFrame.Count; i++)
            {
                UnitStateMessage unitStateMessage = receivedUnitStateMessagesSinceLastFrame[i];

                MatchSimulationUnit unitToUpdate;
                if (!simulationUnits.TryGetValue(unitStateMessage.UnitId, out unitToUpdate))
                {
                    unitToUpdate = new MatchSimulationUnit(unitStateMessage.UnitId, unitStateMessage.XPosition, unitStateMessage.YPosition, unitStateMessage.Rotation, unitStateMessage.Frame);
                    simulationUnits.Add(unitStateMessage.UnitId, unitToUpdate);
                }

                unitToUpdate.SetConfirmedState(unitStateMessage.XPosition, unitStateMessage.YPosition, 
                                               unitStateMessage.Rotation, unitStateMessage.Frame);

                eventProvider.OnUnitStateUpdate(unitToUpdate);
            }

            localPlayer.SetLocalFrameInput((int)(playerMaxFrameSpeed * inputProvider.XTranslation),
                                           (int)(playerMaxFrameSpeed * inputProvider.YTranslation),
                                           inputProvider.GetSimulationRotation(), currentSimulationFrame);

            // sort by oldest frame to newest frame
            receivedPositionConfirmationMessagesSinceLastFrame.Sort((message1, message2) =>
            {
                return message1.Frame == message2.Frame ? 0 : MatchSimulationUnit.IsFrameInFuture(message1.Frame, message2.Frame) ? 1 : -1;
            });

            for (int i = 0; i < receivedPositionConfirmationMessagesSinceLastFrame.Count; i++)
            {
                PositionConfirmationMessage positionConfirmationMessage = receivedPositionConfirmationMessagesSinceLastFrame[i];

                localPlayer.SetConfirmedState(positionConfirmationMessage.XPosition, positionConfirmationMessage.YPosition, 
                                              0, positionConfirmationMessage.Frame);
            }

            eventProvider.OnUnitStateUpdate(localPlayer);

            if (inputProvider.InputReceived)
            {
                //DIContainer.Logger.Debug(string.Format("XTranslation: {0} YTranslation: {1}", inputProvider.XTranslation, inputProvider.YTranslation));

                InputMessage inputMessage = new InputMessage(localPlayer.UnitId, inputProvider.GetSimulationXTranslation(), 
                                                             inputProvider.GetSimulationYTranslation(), inputProvider.GetSimulationRotation(), currentSimulationFrame);

                inputProvider.Reset();

                DIContainer.UDPClient.SendMessage(inputMessage.GetBytes());
            }
        }
    }
}