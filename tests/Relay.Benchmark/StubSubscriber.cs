namespace Relay.Benchmark
{
    using System.Threading.Tasks;
    using Application;

    public class StubSubscriber : ISubscriber
    {
        public Task<bool> ReceiveMsg(Message msg) => Task.FromResult(true);
    }
}