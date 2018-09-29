using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine 
{
    public class RunningMatchState : IMatchState, IUdpMessageListener
    {
        private MatchStateMachine matchStateMachine;

        public void Initialize(MatchStateMachine matchStateMachine)
        {
            this.matchStateMachine = matchStateMachine;
            DIContainer.UDPClient.RegisterListener(MessageId.MATCH_END, this);
            DIContainer.UDPClient.RegisterListener(MessageId.UNIT_STATE, this);
        }

        public void OnFixedUpdateTick()
        {
            // TODO: start game simulation
        }

        public void OnMessageReceived(byte[] message)
        {
            if(message[0] == MessageId.MATCH_END) 
            {
                Debug.Log("Match end message received, switching to MatchEndState");
                this.matchStateMachine.ChangeMatchState(new EndMatchState());
                return;
            }

            if(message[0] == MessageId.UNIT_STATE)
            {
                UnitStateMessage unitStateMessage = new UnitStateMessage(message);
                Debug.Log(string.Format(
                    "Received unit state message = UnitId: '{0}' XPosition: '{1}' YPosition: '{2}' Rotation: '{3}' Frame: '{4}'",
                    unitStateMessage.UnitId, unitStateMessage.XPosition, unitStateMessage.YPosition, unitStateMessage.Rotation, unitStateMessage.Frame));

            }
        }
    }
}