using System;
using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;

public class RoundTripTimeService : IUdpMessageListener
{
    private Int64 lastPingTimestamp;
    public Int64 LastPing { get; private set; }

    private IUdpClient udpClient;
    private NetworkTimeService networkTimeService;

    public RoundTripTimeService(IUdpClient udpClient, NetworkTimeService networkTimeService)
    {
        this.udpClient = udpClient;
        this.networkTimeService = networkTimeService;
        this.udpClient.RegisterListener(MessageId.PING_RESP, this);
    }

    public void OnFixedUpdateTick()
    {
        Int64 currentTimestamp = networkTimeService.NetworkTimestampMs;

        if ((currentTimestamp - lastPingTimestamp) > 1000)
        {
            udpClient.SendMessage(new PingMessage(currentTimestamp).GetBytes());
            lastPingTimestamp = currentTimestamp;
        }
    }

    public void OnMessageReceived(byte[] message)
    {
        if (message[0] == MessageId.PING_RESP)
        {
            PongMessage pongMessage = new PongMessage(message, networkTimeService.NetworkTimestampMs);
            LastPing = pongMessage.RoundTripTime;

            //DIContainer.Logger.Debug(string.Format("Received pong message. RTT: {0}", LastPing));
        }
    }
}
