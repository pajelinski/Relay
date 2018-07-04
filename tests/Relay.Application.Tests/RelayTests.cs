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
        private Message _message;

        [SetUp]
        public void Setup()
        {
            _message = new Message("someMessage");
            _spySubscriber = new SpySubscriber();
            _relay = new Relay();

        }

        [Test]
        public void AddSubscriber_GivenNull_ThrowsArgumentNullException() => 
            Assert.Throws<ArgumentNullException>(() => _relay.AddSubscriber(null));

        [Test]
        public void Broadcast_GivenNull_ThrowsArgumentNullException() => 
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _relay.Broadcast(null));

        [Test]
        public async Task Broadcast_GivenMessage_RelaysMessageToSubscribers()
        {
            _relay.AddSubscriber(_spySubscriber);
            await _relay.Broadcast(_message);

            var receivedMessage = _spySubscriber.ReceivedMessages.Single();
            Assert.That(receivedMessage.Body, Is.EqualTo(_message.Body));
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
            var spySubscribers = new List<SpySubscriber>
            {
                new SpySubscriber(),
                new SpySubscriber(),
                new SpySubscriber(),
            };

            spySubscribers.ForEach(s => _relay.AddSubscriber(s));
            await _relay.Broadcast(_message);

            var receivedMessages = spySubscribers.Select(s => s.ReceivedMessages.Single());

            Assert.That(receivedMessages.All(rm => rm.Id == _message.Id));
        }

        [Test]
        public async Task Broadcast_WhenSubscriberFailsToProcessMessage_SubscriberRetriesToProcessMessage()
        {
            var fakeSubscriber = new FakeSubscriber();
            _relay.AddSubscriber(fakeSubscriber);
            await _relay.Broadcast(_message);

            Assert.That(fakeSubscriber.ReceiveMsgCallsCount, Is.EqualTo(2));
        }

        public class FakeSubscriber : ISubscriber
        {
            public int ReceiveMsgCallsCount;

            public Task<bool> ReceiveMsg(Message msg)
            {
                if (ReceiveMsgCallsCount <= 0)
                {
                    ReceiveMsgCallsCount++;
                    return Task.FromResult(false);
                }

                ReceiveMsgCallsCount++;

                return Task.FromResult(true);
            }
        }
    }
}