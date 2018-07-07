namespace Relay.Application
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Polly;
    using Polly.Retry;

    public class RemoteServiceRelay : AbstractRelay<IRemoteService>
    {
        private readonly RetryPolicy<HttpStatusCode> _retryPolicy;

        public RemoteServiceRelay()
        {
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpStatusCode>(result => result != HttpStatusCode.OK)
                .WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount));
        }

        public void AddRemoteService(IRemoteService subscriber)
        {
            CanNotBeNull(subscriber, nameof(subscriber));

            Receivers.Add(subscriber);
        }

        protected override Task SendMessage(IRemoteService subscriber, Message message)
        {
            return _retryPolicy.ExecuteAsync(() => subscriber.ReceiveMsg(message));
        }
    }
}