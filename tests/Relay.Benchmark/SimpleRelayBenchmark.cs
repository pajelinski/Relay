using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;

namespace Relay.Benchmark
{
    using BenchmarkDotNet.Attributes;
    using Application;

    [CoreJob]
    public class SimpleRelayBenchmark
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