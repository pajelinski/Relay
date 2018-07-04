using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace Relay.Application
{
    public class Relay
    {
        private readonly List<ISubscriber> _subscribers;
        private readonly RetryPolicy<bool> _retryPolicy;

        public Relay()
        {
            _subscribers = new List<ISubscriber>();
            _retryPolicy = Policy.HandleResult(false).WaitAndRetryForeverAsync(ra => TimeSpan.FromSeconds(ra));
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            _subscribers.Add(subscriber);
        }

        public async Task Broadcast(Message message)
        {
            CanNotBeNull(message, nameof(message));

            foreach (var subscriber in _subscribers)
            {
                await _retryPolicy.ExecuteAsync(() => subscriber.ReceiveMsg(message));
            }
        }

        private static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}