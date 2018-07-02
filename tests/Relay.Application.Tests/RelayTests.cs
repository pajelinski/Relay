using System;
using System.Threading.Tasks;

namespace Relay.Application.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RelayTests : Relay
    {
        [Test]
        public void AddSubscriber_GivenNull_ThrowsArgumentNullException() => 
            Assert.Throws<ArgumentNullException>(() => new Relay().AddSubscriber(null));

        [Test]
        public void Broadcast_GivenNull_ThrowsArgumentNullException() => 
            Assert.ThrowsAsync<ArgumentNullException>(async () => await new Relay().Broadcast(null));

        [Test]
        public async Task Broadcast_GivenMessage_RelaysMessageToSubscribers()
        {
            var message = new Message("someMessage");
            var spySubscriber = new SpySubscriber();
            var relay = new Relay();

            relay.AddSubscriber(spySubscriber);
            await relay.Broadcast(message);

            Assert.That(spySubscriber.ReceivedMessage, Is.Not.Null);
            Assert.That(spySubscriber.ReceivedMessage.Body, Is.EqualTo(message.Body));
        }
    }
}