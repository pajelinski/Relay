namespace Relay.Benchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Application;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RelayBenchmark>();
        }
    }

    public class StubSubscriber : ISubscriber
    {
        public Task<bool> ReceiveMsg(Message msg) => Task.FromResult(true);
    }

    public class RelayBenchmark
    {
        private Relay _relay;
        private Message _message;

        [GlobalSetup]
        public void Setup()
        {
            _relay = new Relay();
            _relay.AddSubscriber(new StubSubscriber());
            _message = new Message("benchmark");
        }

        [Benchmark]
        public void BroadcastSingleMessage()
        {
            _relay.Broadcast(_message);
        }
    }
}