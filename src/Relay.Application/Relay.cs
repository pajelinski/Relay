
using System.Threading.Tasks;

namespace Relay.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using Polly;
    using Polly.Retry;

    public class Relay
    {
        private readonly List<ISubscriber> _subscribers;
        private readonly RetryPolicy<bool> _retryPolicy;
        private readonly BufferBlock<Message> _bufferBlock;

        public Relay()
        {
            _subscribers = new List<ISubscriber>();
            _retryPolicy = Policy.HandleResult(false).WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount));
            _bufferBlock = BuildRelayBlock();
        }

        private BufferBlock<Message> BuildRelayBlock()
        {
            var bufferBlock = new BufferBlock<Message>();
            var actionBlock = new ActionBlock<Message>(async m => await Task.WhenAll(_subscribers.Select(s => SendMessage(s, m)).ToArray()));
            bufferBlock.LinkTo(actionBlock);

            return bufferBlock;
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            _subscribers.Add(subscriber);
        }

        public void Broadcast(Message message)
        {
            CanNotBeNull(message, nameof(message));
            _bufferBlock.Post(message);
        }

        private Task SendMessage(ISubscriber subscriber, Message message)
        {
            return _retryPolicy.ExecuteAsync(() => subscriber.ReceiveMsg(message));
        }

        private static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}