using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class EndMatchState : IMatchState
    {
        private byte sendMatchEndAckMessages;
        private byte[] matchEndAckMessageToSend;

        public void Initialize(MatchStateMachine matchStateMachine)
        {
            matchEndAckMessageToSend = new MatchEndAckMessage(matchStateMachine.LocalPlayerId).GetBytes();
        }

        public void OnFixedUpdateTick()
        {
            if(sendMatchEndAckMessages < 3)
            {
                DIContainer.UDPClient.SendMessage(matchEndAckMessageToSend);
                sendMatchEndAckMessages++;
                return;
            }

            DIContainer.Logger.Debug("3 MatchEndAckMessages have been sent. Match is done!");
            // TODO: match ended
        }
    }
}