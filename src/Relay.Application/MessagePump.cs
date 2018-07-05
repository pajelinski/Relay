namespace Relay.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Polly;
    using Polly.Retry;

    public class MessagePump
    {
        private readonly ISubscriber _subscriber;
        private readonly RetryPolicy<bool> _retryPolicy;
        private readonly ConcurrentQueue<Message> _messageQueue;
        private readonly Task _messagePumpTask;

        public MessagePump(ISubscriber subscriber)
        {
            _subscriber = subscriber;
            _retryPolicy = Policy.HandleResult(false).WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount));
            _messageQueue = new ConcurrentQueue<Message>();
            _messagePumpTask = CreateMessagePumpTask();
            _messagePumpTask.Start();
        }

        public void Send(Message msg) => _messageQueue.Enqueue(msg);

        private Task CreateMessagePumpTask()
        {
            return new Task(async () =>
            {
                while (true)
                {
                    _messageQueue.TryDequeue(out var message);
                    if (message == null) continue;

                    await _retryPolicy.ExecuteAsync(() => _subscriber.ReceiveMsg(message));
                }
            });
        }
    }
}