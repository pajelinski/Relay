
namespace Relay.Application
{
    using System.Threading.Tasks;

    public interface ISubscriber
    {
        Task<bool> ReceiveMsg(Message msg);
    }
}