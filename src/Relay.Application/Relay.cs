namespace Relay.Application
{
    using System;
    using System.Collections.Generic;

    public class Relay
    {
        private readonly List<MessagePump> _messagePumps;

        public Relay()
        {
            _messagePumps = new List<MessagePump>();
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            _messagePumps.Add(new MessagePump(subscriber));
        }

        public void Broadcast(Message message)
        {
            CanNotBeNull(message, nameof(message));
            foreach (var messagePump in _messagePumps)
            {
                messagePump.Send(message);
            }
        }

        private static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}