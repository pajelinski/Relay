namespace Relay.Application.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class RelayTests
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
        public async Task Broadcast_GivenMessage_RelaysMessageToSubscribers()
        {
            _relay.AddSubscriber(_spySubscriber);
            _relay.Broadcast(_message);
            await _relay.Complete();

            var receivedMessage = _spySubscriber.ReceivedMessages.Single();

            Assert.That(receivedMessage.Body, Is.EqualTo(_message.Body));
        }

        [Test]
        public async Task Broadcast_GivenThreeMessages_SubscribersReceiveMessagesInOrderTheyWereSend()
        {
            var messages = Enumerable.Range(0, 10).Select(x => new Message(x.ToString())).ToList();

            _relay.AddSubscriber(_spySubscriber);
            messages.ForEach(m => _relay.Broadcast(m));
            await _relay.Complete();

            Assert.That(_spySubscriber.ReceivedMessages, Is.EquivalentTo(messages));
        }

        [Test]
        public async Task Broadcast_GivenThreeSubscribers_EachOfThemeReceiveTheSamMessage()
        {
            var spySubscribers = Enumerable.Range(0, 10).Select(x => new SpySubscriber()).ToList();

            spySubscribers.ForEach(s => _relay.AddSubscriber(s));
            _relay.Broadcast(_message);
            await _relay.Complete();

            var receivedMessages = spySubscribers.Select(s => s.ReceivedMessages.Single());

            Assert.That(receivedMessages.All(rm => rm.Id == _message.Id));
        }

        [Test]
        public async Task Broadcast_WhenSubscriberFailsToProcessMessage_SubscriberRetriesToProcessMessage()
        {
            var fakeSubscriber = new FakeSubscriber();
            _relay.AddSubscriber(fakeSubscriber);
            _relay.Broadcast(_message);
            await _relay.Complete();

            Assert.That(fakeSubscriber.ReceiveMsgCallsCount, Is.EqualTo(2));
        }
    }
}