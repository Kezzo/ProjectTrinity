using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

namespace ProjectTrinity.MatchStateMachine
{
    public class EndMatchState : IMatchState
    {
        private byte sendMatchEndAckMessages;
        private byte[] matchEndAckMessageToSend;

        private MatchStateMachine matchStateMachine;

        public void OnActivate(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            matchEndAckMessageToSend = new MatchEndAckMessage(matchStateMachine.LocalPlayerId).GetBytes();
        }

        public void OnDeactivate()
        {
           
        }

        public void OnFixedUpdateTick()
        {
            if(sendMatchEndAckMessages < 3)
            {
                matchStateMachine.UDPClient.SendMessage(matchEndAckMessageToSend);
                sendMatchEndAckMessages++;

                if (sendMatchEndAckMessages == 3)
                {
                    DIContainer.Logger.Debug("3 MatchEndAckMessages have been sent. Match is done!");
                }

                return;
            }

            // TODO: match ended
        }
    }
}