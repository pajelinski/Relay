using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Relay.Application.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RelayTests : Relay
    {
        private Relay _relay;
        private SpySubscriber _spySubscriber;

        [SetUp]
        public void Setup()
        {
            _spySubscriber = new SpySubscriber();
            _relay = new Relay();

        }

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

            _relay.AddSubscriber(_spySubscriber);
            await _relay.Broadcast(message);

            var receivedMessage = _spySubscriber.ReceivedMessages.Single();
            Assert.That(receivedMessage.Body, Is.EqualTo(message.Body));
        }

        [Test]
        public void Broadcast_GivenThreeMessages_SubscribersReceiveMessagesInOrderTheyWereSend()
        {
            var messages = new List<Message>
            {
                new Message("1"),
                new Message("2"),
                new Message("3")
            };

            _relay.AddSubscriber(_spySubscriber);
            messages.ForEach(async m => await _relay.Broadcast(m));

            Assert.That(_spySubscriber.ReceivedMessages, Is.EquivalentTo(messages));
        }

        [Test]
        public async Task Broadcast_GivenThreeSubscribers_EachOfThemeReceiveTheSamMessage()
        {
            var message = new Message("someMessage");

            var spySubscribers = new List<SpySubscriber>
            {
                new SpySubscriber(),
                new SpySubscriber(),
                new SpySubscriber(),
            };

            spySubscribers.ForEach(s => _relay.AddSubscriber(s));
            await _relay.Broadcast(message);

            var receivedMessages = spySubscribers.Select(s => s.ReceivedMessages.Single());

            Assert.That(receivedMessages.All(rm => rm.Id == message.Id));
        }
    }
}