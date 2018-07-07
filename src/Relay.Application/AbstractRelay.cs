namespace Relay.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public abstract class AbstractRelay<T>
    {
        protected readonly List<T> Receivers;
        private readonly BufferBlock<Message> _bufferBlock;
        private readonly ActionBlock<Message> _actionBlock;

        protected AbstractRelay()
        {
            Receivers = new List<T>();
            _bufferBlock = new BufferBlock<Message>();
            _actionBlock = new ActionBlock<Message>(BroadcastMessage);
            _bufferBlock.LinkTo(_actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public void Broadcast(Message message)
        {
            _bufferBlock.Post(message);
        }

        //Added for testing purpose
        public async Task Complete()
        {
            _bufferBlock.Complete();
            await _actionBlock.Completion;
        }

        private async Task BroadcastMessage(Message message)
        {
            await Task.WhenAll(Receivers.Select(s => SendMessage(s, message)).ToArray());
        }

        protected abstract Task SendMessage(T subscriber, Message message);

        protected static void CanNotBeNull(object item, string name)
        {
            if (item == null) throw new ArgumentNullException(name);
        }
    }
}