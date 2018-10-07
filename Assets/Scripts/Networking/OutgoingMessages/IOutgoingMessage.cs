namespace ProjectTrinity.Networking.Messages
{
    public interface IOutgoingMessage
    {
        byte[] GetBytes();
    }
}