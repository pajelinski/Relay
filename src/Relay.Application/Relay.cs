using System.Threading.Tasks;

namespace Relay.Application
{
    using System;
    using Polly;
    using Polly.Retry;

    public class Relay : AbstractRelay<ISubscriber>
    {
        private readonly RetryPolicy<bool> _retryPolicy;

        public Relay()
        {
            _retryPolicy = Policy.HandleResult(false).WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount));
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            Receivers.Add(subscriber);
        }

        protected override Task SendMessage(ISubscriber subscriber, Message message)
        {
            return _retryPolicy.ExecuteAsync(() => subscriber.ReceiveMsg(message));
        }
    }
}