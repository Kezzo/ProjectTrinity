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

        private static readonly int playerMaxFrameSpeed = 400;

        private Int64 matchStartTimestamp;

        public MatchSimulation(byte localPlayerUnitID, byte[] matchUnitIDs, Int64 matchStartTimestamp, MatchInputProvider matchInputProvider, MatchEventProvider matchEventProvider)
        {
            this.matchStartTimestamp = matchStartTimestamp;
            inputProvider = matchInputProvider;
            eventProvider = matchEventProvider;

            localPlayer = new MatchSimulationLocalPlayer(localPlayerUnitID, 0, 0, 0, 0);
            eventProvider.OnUnitSpawn(localPlayerUnitID, true);

            foreach (byte matchUnitID in matchUnitIDs)
            {
                if(matchUnitID == localPlayerUnitID)
                {
                    continue;
                }

                simulationUnits.Add(matchUnitID, new MatchSimulationUnit(matchUnitID, 0, 0, 0, 0));
                eventProvider.OnUnitSpawn(matchUnitID);
            }
        }

        public void OnSimulationFrame(List<UnitStateMessage> receivedUnitStateMessagesSinceLastFrame, List<PositionConfirmationMessage> receivedPositionConfirmationMessagesSinceLastFrame)
        {
            byte currentTimebasedFrame = (byte)MathHelper.Modulo((DIContainer.NetworkTimeService.NetworkTimestampMs - matchStartTimestamp) / 33, byte.MaxValue);
            //DIContainer.Logger.Debug("Frame: " + currentTimebasedFrame);

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
                    eventProvider.OnUnitSpawn(unitStateMessage.UnitId);
                }

                bool positionChanged = unitToUpdate.SetConfirmedState(unitStateMessage.XPosition, unitStateMessage.YPosition, 
                                               unitStateMessage.Rotation, unitStateMessage.Frame);

                if (positionChanged)
                {
                    eventProvider.OnUnitStateUpdate(unitToUpdate, unitStateMessage.Frame);
                }

                //DIContainer.Logger.Debug("Received usm with frame: " + unitStateMessage.Frame);
            }

            // frame didn't change yet, should never happen in practise.
            if (currentSimulationFrame == currentTimebasedFrame)
            {
                return;
            }

            // so combined translation is max 1, so diagonal movement isn't faster.
            float[] cappedTranslations = MathHelper.GetCappedTranslations(inputProvider.XTranslation, inputProvider.YTranslation);

            byte inputFrame = (byte)MathHelper.Modulo(currentSimulationFrame + 1, byte.MaxValue);

            localPlayer.SetLocalFrameInput((int) Math.Round(playerMaxFrameSpeed * cappedTranslations[0]),
                                           (int) Math.Round(playerMaxFrameSpeed * cappedTranslations[1]),
                                           inputProvider.GetSimulationRotation(), inputFrame);

            byte frameToProcess = inputFrame;
            // this means we skipped a frame, we need to create buffer entries for all frames though
            while (frameToProcess != currentTimebasedFrame)
            {
                frameToProcess = (byte)MathHelper.Modulo(frameToProcess + 1, byte.MaxValue);
                localPlayer.SetLocalFrameInput(0, 0, inputProvider.GetSimulationRotation(), frameToProcess);
            }

            // sort by oldest frame to newest frame
            receivedPositionConfirmationMessagesSinceLastFrame.Sort((message1, message2) =>
            {
                return message1.Frame == message2.Frame ? 0 : MatchSimulationUnit.IsFrameInFuture(message1.Frame, message2.Frame) ? 1 : -1;
            });

            for (int i = 0; i < receivedPositionConfirmationMessagesSinceLastFrame.Count; i++)
            {
                PositionConfirmationMessage positionConfirmationMessage = receivedPositionConfirmationMessagesSinceLastFrame[i];

                if (positionConfirmationMessage.UnitId == localPlayer.UnitId)
                {
                    localPlayer.SetConfirmedState(positionConfirmationMessage.XPosition, positionConfirmationMessage.YPosition,
                                              0, positionConfirmationMessage.Frame);
                }
            }

            eventProvider.OnUnitStateUpdate(localPlayer, currentTimebasedFrame);

            if (inputProvider.InputReceived)
            {
                InputMessage inputMessage = new InputMessage(localPlayer.UnitId, inputProvider.GetSimulationXTranslation(), 
                                                             inputProvider.GetSimulationYTranslation(), localPlayer.Rotation, inputFrame);

                inputProvider.Reset();

                DIContainer.UDPClient.SendMessage(inputMessage.GetBytes());
            }

            eventProvider.OnSimulationFrame(currentTimebasedFrame);

            currentSimulationFrame = currentTimebasedFrame;
        }
    }
}