namespace Relay.Benchmark
{
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using Application;

    public class TenSubscribersRelayBenchmark
    {
        private Relay _relay;
        private Message _message;

        [GlobalSetup]
        public void Setup()
        {
            _relay = new Relay();
            var stubSubscribers = Enumerable.Range(0, 9).Select(_ => new StubSubscriber());
            foreach (var stubSubscriber in stubSubscribers)
            {
                _relay.AddSubscriber(stubSubscriber);
            }
            _message = new Message("benchmark");
        }

        [Benchmark]
        public void BroadcastSingleMessage()
        {
            _relay.Broadcast(_message);
        }
    }
}