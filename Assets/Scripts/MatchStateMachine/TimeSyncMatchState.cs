using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

namespace ProjectTrinity.MatchStateMachine
{
    public class TimeSyncMatchState : IMatchState
    {
        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            matchStateMachine.NetworkTimeService.Synch(matchStateMachine.UDPClient, () =>
            {
                matchStateMachine.AckedMessageHelper.SendAckedMessage(new TimeSyncDoneMessage(), MessageId.TIME_SYNC_DONE_ACK, ackMessage =>
                {
                    DIContainer.Logger.Debug("Time synch done, switching to WaitForStartMatchState");
                    TimeSyncDoneAckMessage receivedMessage = new TimeSyncDoneAckMessage(ackMessage);
                    matchStateMachine.LocalPlayerId = receivedMessage.PlayerId;
                    matchStateMachine.StartRoundTripTimeService();
                    matchStateMachine.ChangeMatchState(new WaitForStartMatchState());
                });
            });
        }

        public void OnDeactivate()
        {

        }

        public void OnFixedUpdateTick()
        {

        }
    }
}