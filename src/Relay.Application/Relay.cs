using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentQueue<Message> _messageQueue;

        public Relay()
        {
            _subscribers = new List<ISubscriber>();
            _retryPolicy = Policy.HandleResult(false).WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount));
            _messageQueue = new ConcurrentQueue<Message>();
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            _subscribers.Add(subscriber);
        }

        public void Broadcast(Message message)
        {
            CanNotBeNull(message, nameof(message));
            _messageQueue.Enqueue(message);
            BroadcastToSubscribers();
        }

        private void BroadcastToSubscribers()
        {
            Task.Run(() =>
            {
                while (!_messageQueue.IsEmpty)
                {
                    _messageQueue.TryDequeue(out var message);

                    if (message == null) break;

                    foreach (var subscriber in _subscribers)
                    {
                        _retryPolicy.ExecuteAsync(() => subscriber.ReceiveMsg(message));
                    }
                }
            });
        }

        private static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}