using System;
using ProjectTrinity.Networking;
using ProjectTrinity.Networking.Messages;
using ProjectTrinity.Root;

public class RoundTripTimeService : IUdpMessageListener
{
    private Int64 lastPingTimestamp;
    public Int64 LastPing { get; private set; }

    public RoundTripTimeService()
    {
        DIContainer.UDPClient.RegisterListener(MessageId.PING_RESP, this);
    }

    public void OnFixedUpdateTick()
    {
        Int64 currentTimestamp = DIContainer.NetworkTimeService.NetworkTimestampMs;

        if ((currentTimestamp - lastPingTimestamp) > 1000)
        {
            DIContainer.UDPClient.SendMessage(new PingMessage(currentTimestamp).GetBytes());
            lastPingTimestamp = currentTimestamp;
        }
    }

    public void OnMessageReceived(byte[] message)
    {
        if (message[0] == MessageId.PING_RESP)
        {
            PongMessage pongMessage = new PongMessage(message);
            LastPing = pongMessage.RoundTripTime;

            //DIContainer.Logger.Debug(string.Format("Received pong message. RTT: {0}", LastPing));
        }
    }
}
