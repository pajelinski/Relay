using System;
using System.Threading.Tasks;
using Relay.Application;

namespace Relay
{
    public class Subscriber : ISubscriber
    {
        private readonly int _id;

        public Subscriber(int id)
        {
            _id = id;
        }

        public Task<bool> ReceiveMsg(Message msg)
        {
            Console.WriteLine($"Subscriber: {_id} receivedMessage: {msg.Body}");
            return Task.FromResult(true);
        }
    }
}