namespace Relay.Application.Tests
{
    using System.Threading.Tasks;

    public class StubSubscriber : ISubscriber
    {
        public Task<bool> ReceiveMsg(Message msg) => Task.FromResult(true);
    }
}