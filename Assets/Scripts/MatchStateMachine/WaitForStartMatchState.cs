using System;
using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

namespace ProjectTrinity.MatchStateMachine
{
    public class WaitForStartMatchState : IMatchState, IUdpMessageListener
    {
        private Int64 receivedMatchStartTimestamp;
        private byte ackMessagesSent;

        private byte[] ackMessageToSend;

        private MatchStateMachine matchStateMachine;

        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            this.matchStateMachine.UDPClient.RegisterListener(MessageId.MATCH_START, this);
        }

        public void OnDeactivate()
        {
            matchStateMachine.UDPClient.DeregisterListener(MessageId.MATCH_START, this);
        }

        public void OnFixedUpdateTick()
        {
            if (receivedMatchStartTimestamp == 0)
            {
                return;
            }

            // send three times to increase chance of delivery, if not deliver with three tries network is too bad to play anyway.
            if(ackMessagesSent < 3) 
            {
                matchStateMachine.UDPClient.SendMessage(ackMessageToSend);
                ackMessagesSent++;
            }

            // match starts
            if(this.matchStateMachine.NetworkTimeService.NetworkTimestampMs >= receivedMatchStartTimestamp) 
            {
                DIContainer.Logger.Debug("Match start wait time is over. Switching to RunningMatchState");
                matchStateMachine.ChangeMatchState(new RunningMatchState());
            }
        }

        public void OnMessageReceived(byte[] message)
        {
            if(message[0] != MessageId.MATCH_START) 
            {
                return;
            }

            MatchStartMessage receivedMessage = new MatchStartMessage(message);
            receivedMatchStartTimestamp = receivedMessage.MatchStartTimestamp;

            matchStateMachine.MatchStartTimestamp = receivedMessage.MatchStartTimestamp;

            ackMessageToSend = new MatchStartAckMessage(this.matchStateMachine.LocalPlayerId).GetBytes();
            DIContainer.Logger.Debug(string.Format("MatchStartMessage received. MatchStartTimestamp is: {0}", receivedMatchStartTimestamp));
        }
    }
}