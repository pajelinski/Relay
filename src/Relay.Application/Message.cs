namespace Relay.Application
{
    using System;

    public struct Message
    {
        public Guid Id { get; set; }
        public string Body { get; }

        public Message(string body)
        {
            Id = Guid.NewGuid();
            Body = body;
        }
    }
}