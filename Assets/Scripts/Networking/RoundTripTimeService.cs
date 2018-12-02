using System;
using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using UniRx;

public class RoundTripTimeService
{
    private Int64 lastPingTimestamp;
    public Int64 LastPing { get; private set; }

    private IUdpClient udpClient;
    private NetworkTimeService networkTimeService;

    public RoundTripTimeService(IUdpClient udpClient, NetworkTimeService networkTimeService)
    {
        this.udpClient = udpClient;
        this.networkTimeService = networkTimeService;
        this.udpClient.OnMessageReceive
            .Where(message => message[0] == MessageId.PING_RESP)
            .Subscribe(OnMessageReceived);
    }

    public void OnFixedUpdateTick()
    {
        Int64 currentTimestamp = networkTimeService.NetworkTimestampMs;

        if ((currentTimestamp - lastPingTimestamp) > 1000)
        {
            udpClient.SendMessage(new PingMessage(currentTimestamp).GetBytes());
            lastPingTimestamp = currentTimestamp;

            //DIContainer.Logger.Debug(string.Format("Send ping message. current time: {0}", currentTimestamp));
        }
    }

    private void OnMessageReceived(byte[] message)
    {
        PongMessage pongMessage = new PongMessage(message, networkTimeService.NetworkTimestampMs);
        LastPing = pongMessage.RoundTripTime;

        //DIContainer.Logger.Debug(string.Format("Received pong message. RTT: {0}", LastPing));
    }
}
