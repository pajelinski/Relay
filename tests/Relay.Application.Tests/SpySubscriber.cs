using System.Collections.Generic;

namespace Relay.Application.Tests
{
    using System.Threading.Tasks;

    public class SpySubscriber : ISubscriber
    {
        public List<Message> ReceivedMessages { get;}

        public SpySubscriber()
        {
            ReceivedMessages = new List<Message>();
        }

        public Task<bool> ReceiveMsg(Message msg)
        {
            ReceivedMessages.Add(msg);
            return Task.FromResult(true);
        }
    }
}