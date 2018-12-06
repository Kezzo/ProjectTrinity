using System;
using ProjectTrinity.Input;
using ProjectTrinity.Networking;
using ProjectTrinity.Root;
using UniRx;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchStateMachine
    {
        public ReactiveProperty<IMatchState> CurrentMatchState { get; private set; }
        public byte LocalPlayerId { get; set; }
        public Int64 MatchStartTimestamp { get; set; }
        public MatchInputProvider MatchInputProvider { get; private set; }
        public MatchSpawnService MatchEventProvider { get; private set; }
        public AckedMessageHelper AckedMessageHelper { get; private set; }
        public RoundTripTimeService RoundTripTimeService { get; private set; }
        public NetworkTimeService NetworkTimeService { get; private set; }

        public IUdpClient UDPClient { get; private set; }

        public MatchStateMachine()
        {
            MatchInputProvider = new MatchInputProvider();
            MatchEventProvider = new MatchSpawnService();
            CurrentMatchState = new ReactiveProperty<IMatchState>();

            ChangeMatchState(new IdleMatchState());
        }

        public void ChangeMatchState(IMatchState matchState)
        {
            DIContainer.Logger.Debug(string.Format("Switching to {0}", matchState.GetType()));

            if(CurrentMatchState.Value != null)
            {
                CurrentMatchState.Value.OnDeactivate();
            }

            CurrentMatchState.SetValueAndForceNotify(matchState);
            CurrentMatchState.Value.OnActivate(this);
        }

        public void OnFixedUpdateTick() 
        {
            CurrentMatchState.Value.OnFixedUpdateTick();

            if (AckedMessageHelper != null)
            { 
                AckedMessageHelper.OnFixedUpdateTick();
            }

            if (RoundTripTimeService != null)
            { 
                RoundTripTimeService.OnFixedUpdateTick();
            }
        }

        public void InitializeUdpClient(string ip, int port)
        {
            UDPClient = new UdpClient(ip, port);
            AckedMessageHelper = new AckedMessageHelper(UDPClient);
            NetworkTimeService = new NetworkTimeService();
        }

        public void StartRoundTripTimeService()
        {
            RoundTripTimeService = new RoundTripTimeService(UDPClient, NetworkTimeService);
        }
    }
}