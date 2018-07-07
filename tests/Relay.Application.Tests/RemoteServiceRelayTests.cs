namespace Relay.Application.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class RemoteServiceRelayTests
    {
        private RemoteServiceRelay _remoteServiceRelay;
        private SpyRemoteService _spyRemoteService;
        private Message _message;

        [SetUp]
        public void Setup()
        {
            _message = new Message("someMessage");
            _spyRemoteService = new SpyRemoteService();
            _remoteServiceRelay = new RemoteServiceRelay();
        }

        [Test]
        public void AddRemoteService_GivenNull_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => _remoteServiceRelay.AddRemoteService(null));

        [Test]
        public async Task Broadcast_GivenMessage_RelaysMessageToRemoteService()
        {
            _remoteServiceRelay.AddRemoteService(_spyRemoteService);
            _remoteServiceRelay.Broadcast(_message);
            await _remoteServiceRelay.Complete();

            var receivedMessage = _spyRemoteService.ReceivedMessages.Single();

            Assert.That(receivedMessage.Body, Is.EqualTo(_message.Body));
        }

        [Test]
        public async Task Broadcast_GivenThreeMessages_RemoteServiceReceivesMessagesInOrderTheyWereSend()
        {
            var messages = Enumerable.Range(0, 10).Select(x => new Message(x.ToString())).ToList();

            _remoteServiceRelay.AddRemoteService(_spyRemoteService);
            messages.ForEach(m => _remoteServiceRelay.Broadcast(m));
            await _remoteServiceRelay.Complete();

            Assert.That(_spyRemoteService.ReceivedMessages, Is.EquivalentTo(messages));
        }

        [Test]
        public async Task Broadcast_GivenThreeRemoteServices_EachOfThemeReceiveTheSamMessage()
        {
            var spyRemoteServices = Enumerable.Range(0, 10).Select(x => new SpyRemoteService()).ToList();

            spyRemoteServices.ForEach(s => _remoteServiceRelay.AddRemoteService(s));
            _remoteServiceRelay.Broadcast(_message);
            await _remoteServiceRelay.Complete();

            var receivedMessages = spyRemoteServices.Select(s => s.ReceivedMessages.Single());

            Assert.That(receivedMessages.All(rm => rm.Id == _message.Id));
        }

        [Test]
        public async Task Broadcast_WhenRemoteServiceFailsToProcessMessage_RelatRetriesToSendMessage()
        {
            var fakeRemoteService = new FakeRemoteService();
            _remoteServiceRelay.AddRemoteService(fakeRemoteService);
            _remoteServiceRelay.Broadcast(_message);
            await _remoteServiceRelay.Complete();

            Assert.That(fakeRemoteService.ReceiveMsgCallsCount, Is.EqualTo(2));
        }
    }
}