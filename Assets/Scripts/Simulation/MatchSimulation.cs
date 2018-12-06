using System;
using System.Collections.Generic;
using ProjectTrinity.Helper;
using ProjectTrinity.Input;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulation
    {
        private Dictionary<byte, MatchSimulationUnit> simulationUnits = new Dictionary<byte, MatchSimulationUnit>();
        private MatchSimulationLocalPlayer localPlayer;

        private MatchInputProvider inputProvider;
        private MatchEventProvider eventProvider;
        private IUdpClient udpClient;
        private NetworkTimeService networkTimeService;

        private byte currentSimulationFrame;

        private static readonly int playerMaxFrameSpeed = 250;

        private Int64 matchStartTimestamp;
        private byte localPlayerUnitId;

        public MatchSimulation(byte localPlayerUnitId, Int64 matchStartTimestamp, MatchInputProvider matchInputProvider, 
                               MatchEventProvider matchEventProvider, IUdpClient udpClient, NetworkTimeService networkTimeService)
        {
            this.localPlayerUnitId = localPlayerUnitId;
            this.matchStartTimestamp = matchStartTimestamp;
            inputProvider = matchInputProvider;
            eventProvider = matchEventProvider;
            this.udpClient = udpClient;
            this.networkTimeService = networkTimeService;
        }

        public void OnSimulationFrame(List<UnitStateMessage> receivedUnitStateMessagesSinceLastFrame, 
                                      List<PositionConfirmationMessage> receivedPositionConfirmationMessagesSinceLastFrame,
                                      List<UnitAbilityActivationMessage> receivedUnitAbilityMessagesSinceLastFrame,
                                      List<UnitSpawnMessage> receivedUnitSpawnMessagesSinceLastFrame)
        {
            byte currentTimebasedFrame = (byte)MathHelper.Modulo((networkTimeService.NetworkTimestampMs - matchStartTimestamp) / 33, byte.MaxValue);
            //DIContainer.Logger.Debug("Frame: " + currentTimebasedFrame);

            SpawnUnits(receivedUnitSpawnMessagesSinceLastFrame);
            UpdateUnitStates(receivedUnitStateMessagesSinceLastFrame);
            UpdateUnitAbilityActivations(receivedUnitAbilityMessagesSinceLastFrame);

            // frame didn't change yet, should never happen in practise.
            if (currentSimulationFrame == currentTimebasedFrame)
            {
                return;
            }

            byte inputFrame = UpdateLocalPlayerState(receivedPositionConfirmationMessagesSinceLastFrame, currentTimebasedFrame);
            SendInputMessages(inputFrame);

            eventProvider.OnSimulationFrame(currentTimebasedFrame);
            currentSimulationFrame = currentTimebasedFrame;
        }

        private void SpawnUnits(List<UnitSpawnMessage> receivedUnitSpawnMessagesSinceLastFrame)
        {
            foreach (UnitSpawnMessage unitSpawnMessage in receivedUnitSpawnMessagesSinceLastFrame)
            {
                if (simulationUnits.ContainsKey(unitSpawnMessage.UnitId) || (localPlayer != null && localPlayer.UnitId == unitSpawnMessage.UnitId))
                {
                    return;
                }

                if(unitSpawnMessage.UnitId == localPlayerUnitId)
                {
                    MatchSimulationLocalPlayer simulationLocalPlayer = new MatchSimulationLocalPlayer(unitSpawnMessage.UnitId, unitSpawnMessage.XPosition,
                                                                    unitSpawnMessage.YPosition, unitSpawnMessage.Rotation,
                                                                    unitSpawnMessage.HealthPercent, unitSpawnMessage.Frame);

                    localPlayer = simulationLocalPlayer;
                    eventProvider.OnUnitSpawn(unitSpawnMessage.UnitId, unitSpawnMessage.UnitType, simulationLocalPlayer, unitSpawnMessage.UnitId == localPlayerUnitId);
                }
                else
                {
                    MatchSimulationUnit simulationUnit = new MatchSimulationUnit(unitSpawnMessage.UnitId, unitSpawnMessage.XPosition,
                                                                    unitSpawnMessage.YPosition, unitSpawnMessage.Rotation,
                                                                    unitSpawnMessage.HealthPercent, unitSpawnMessage.Frame);

                    simulationUnits.Add(unitSpawnMessage.UnitId, simulationUnit);
                    eventProvider.OnUnitSpawn(unitSpawnMessage.UnitId, unitSpawnMessage.UnitType, simulationUnit, unitSpawnMessage.UnitId == localPlayerUnitId);
                }
            }
        }

        private void UpdateUnitStates(List<UnitStateMessage> receivedUnitStateMessagesSinceLastFrame)
        {
            // sort by oldest frame to newest frame
            receivedUnitStateMessagesSinceLastFrame.Sort((message1, message2) =>
            {
                return message1.Frame == message2.Frame ? 0 : MatchSimulationUnit.IsFrameInFuture(message1.Frame, message2.Frame) ? 1 : -1;
            });

            for (int i = 0; i < receivedUnitStateMessagesSinceLastFrame.Count; i++)
            {
                UnitStateMessage unitStateMessage = receivedUnitStateMessagesSinceLastFrame[i];

                MatchSimulationUnit unitToUpdate;

                if(localPlayer != null && localPlayer.UnitId == unitStateMessage.UnitId) 
                {
                    // only update the health here and ignore position and rotation for the local player,
                    // since that will be confirmed via the PositionConfirmationMessage
                    localPlayer.HealthPercent.Value = unitStateMessage.HealthPercent;
                    return;
                }
                else if (!simulationUnits.TryGetValue(unitStateMessage.UnitId, out unitToUpdate))
                {
                    return;
                }

                unitToUpdate.SetConfirmedState(unitStateMessage.XPosition, unitStateMessage.YPosition,
                                               unitStateMessage.Rotation, unitStateMessage.HealthPercent, unitStateMessage.Frame);

                //DIContainer.Logger.Debug("Received usm with frame: " + unitStateMessage.Frame);
            }
        }

        private void UpdateUnitAbilityActivations(List<UnitAbilityActivationMessage> receivedUnitAbilityMessagesSinceLastFrame)
        {
            // sort by oldest frame to newest frame
            receivedUnitAbilityMessagesSinceLastFrame.Sort((message1, message2) =>
            {
                return message1.StartFrame == message2.StartFrame ? 0 : MatchSimulationUnit.IsFrameInFuture(message1.StartFrame, message2.StartFrame) ? 1 : -1;
            });

            for (int i = 0; i < receivedUnitAbilityMessagesSinceLastFrame.Count; i++)
            {
                UnitAbilityActivationMessage unitAbilityActivationMessage = receivedUnitAbilityMessagesSinceLastFrame[i];

                MatchSimulationUnit unitToUpdate;
                if (simulationUnits.TryGetValue(unitAbilityActivationMessage.UnitId, out unitToUpdate))
                {
                    eventProvider.OnAbilityActivation(unitToUpdate.UnitId, UnitValueConverter.ToUnityRotation(unitAbilityActivationMessage.Rotation), 
                                                      unitAbilityActivationMessage.StartFrame, unitAbilityActivationMessage.ActivationFrame);
                }
            }
        }

        private byte UpdateLocalPlayerState(List<PositionConfirmationMessage> receivedPositionConfirmationMessagesSinceLastFrame, byte currentTimebasedFrame)
        {
            if(localPlayer == null)
            {
                return currentTimebasedFrame;
            }

            // so combined translation is max 1, so diagonal movement isn't faster.
            float[] cappedTranslations = MathHelper.GetCappedTranslations(inputProvider.XTranslation, inputProvider.YTranslation);

            byte inputFrame = (byte)MathHelper.Modulo(currentSimulationFrame + 1, byte.MaxValue);

            byte rotation = inputProvider.AbilityInputReceived ? inputProvider.GetSimulationAimingRotation() : inputProvider.GetSimulationRotation();

            localPlayer.SetLocalFrameInput((int)Math.Round(playerMaxFrameSpeed * cappedTranslations[0]),
                                       (int)Math.Round(playerMaxFrameSpeed * cappedTranslations[1]),
                                       rotation, inputFrame);

            byte frameToProcess = inputFrame;
            // this means we skipped a frame, we need to create buffer entries for all frames though
            while (frameToProcess != currentTimebasedFrame)
            {
                frameToProcess = (byte)MathHelper.Modulo(frameToProcess + 1, byte.MaxValue);
                localPlayer.SetLocalFrameInput(0, 0, rotation, frameToProcess);
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
                                              0, 0, positionConfirmationMessage.Frame);
                }
            }
            
            if (inputProvider.AbilityInputReceived)
            {
                byte activationFrame = (byte)MathHelper.Modulo(inputFrame + 10, byte.MaxValue);
                eventProvider.OnAbilityActivation(localPlayer.UnitId, inputProvider.AimingRotation, inputFrame, activationFrame);
            } 
            else if (inputProvider.AimingInputReceived)
            {
                eventProvider.OnLocalAimingUpdate(localPlayer.UnitId, inputProvider.AimingRotation);
            }

            return inputFrame;
        }

        private void SendInputMessages(byte inputFrame)
        {
            if(localPlayer == null)
            {
                return;
            }

            if (inputProvider.InputReceived)
            {
                InputMessage inputMessage = new InputMessage(localPlayer.UnitId, inputProvider.GetSimulationXTranslation(),
                                                             inputProvider.GetSimulationYTranslation(), localPlayer.MovementState.Value.Rotation, inputFrame);
                udpClient.SendMessage(inputMessage.GetBytes());
            }

            if (inputProvider.AbilityInputReceived)
            {
                AbilityInputMessage AbilityInputMessage = new AbilityInputMessage(localPlayer.UnitId, 0, localPlayer.MovementState.Value.Rotation, inputFrame);
                udpClient.SendMessage(AbilityInputMessage.GetBytes());
            }

            inputProvider.Reset();
        }
    }
}