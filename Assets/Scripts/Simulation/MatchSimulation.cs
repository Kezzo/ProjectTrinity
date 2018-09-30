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

        public MatchSimulation(byte localPlayerUnitID, byte[] matchUnitIDs, MatchInputProvider matchInputProvider, MatchEventProvider matchEventProvider)
        {
            localPlayer = new MatchSimulationLocalPlayer(localPlayerUnitID, 0, 0, 0, 0);
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
            foreach (UnitStateMessage unitStateMessage in receivedUnitStateMessagesSinceLastFrame)
            {
                MatchSimulationUnit unitToUpdate;
                if (!simulationUnits.TryGetValue(unitStateMessage.UnitId, out unitToUpdate))
                {
                    DIContainer.Logger.Warn(string.Format("Received UnitStateMessage for unit that doesn't exist in the simulation. UnitID: {0}", unitStateMessage.UnitId));
                    continue;
                }

                unitToUpdate.SetConfirmedState(unitStateMessage.XPosition, unitStateMessage.YPosition, 
                                               unitStateMessage.Rotation, unitStateMessage.Frame);

                eventProvider.OnUnitStateUpdate(unitToUpdate);
            }

            foreach (var positionConfirmationMessage in receivedPositionConfirmationMessagesSinceLastFrame)
            {
                localPlayer.SetConfirmedState(positionConfirmationMessage.XPosition, positionConfirmationMessage.YPosition, 
                                              positionConfirmationMessage.Rotation, positionConfirmationMessage.Frame);
            }

            DIContainer.Logger.Debug(string.Format("XTranslation: {0} YTranslation: {1}", inputProvider.XTranslation, inputProvider.YTranslation));
            localPlayer.SetLocalFrameInput((int) (playerMaxFrameSpeed * inputProvider.XTranslation),
                                           (int) (playerMaxFrameSpeed * inputProvider.YTranslation),
                                           inputProvider.GetSimulationRotation(), currentSimulationFrame);

            eventProvider.OnUnitStateUpdate(localPlayer);

            if (inputProvider.InputReceived)
            {
                InputMessage inputMessage = new InputMessage(localPlayer.UnitId, inputProvider.GetSimulationXTranslation(), 
                                                             inputProvider.GetSimulationYTranslation(), inputProvider.GetSimulationRotation(), currentSimulationFrame);

                inputProvider.Reset();

                DIContainer.UDPClient.SendMessage(inputMessage.GetBytes());
            }

            currentSimulationFrame = (byte) MathHelper.Modulo((currentSimulationFrame + 1), byte.MaxValue);
        }
    }
}