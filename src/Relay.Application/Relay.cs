using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relay.Application
{
    public class Relay
    {
        private readonly List<ISubscriber> _subscribers;

        public Relay()
        {
            _subscribers = new List<ISubscriber>();
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
                await subscriber.ReceiveMsg(message);
            }
        }

        private static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}