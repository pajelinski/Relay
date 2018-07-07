namespace Relay.Application.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public class SpyRemoteService : IRemoteService
    {
        public List<Message> ReceivedMessages { get; }

        public SpyRemoteService()
        {
            ReceivedMessages = new List<Message>();
        }

        public Task<HttpStatusCode> ReceiveMsg(Message msg)
        {
            ReceivedMessages.Add(msg);
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}