namespace Relay.Application.Tests
{
    using System.Net;
    using System.Threading.Tasks;

    public class FakeRemoteService : IRemoteService
    {
        public int ReceiveMsgCallsCount;

        public Task<HttpStatusCode> ReceiveMsg(Message msg)
        {
            if (ReceiveMsgCallsCount <= 0)
            {
                ReceiveMsgCallsCount++;
                return Task.FromResult(HttpStatusCode.BadRequest);
            }

            ReceiveMsgCallsCount++;

            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}