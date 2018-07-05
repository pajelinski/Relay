namespace Relay.Application.Tests
{
    using System.Threading.Tasks;

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