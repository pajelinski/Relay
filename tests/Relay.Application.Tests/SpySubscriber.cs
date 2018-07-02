namespace Relay.Application.Tests
{
    using System.Threading.Tasks;

    public class SpySubscriber : ISubscriber
    {
        public Message ReceivedMessage { get; private set; }

        public Task<bool> ReceiveMsg(Message msg)
        {
            ReceivedMessage = msg;
            return Task.FromResult(true);
        }
    }
}