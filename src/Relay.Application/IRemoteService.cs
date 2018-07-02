namespace Relay.Application
{
    using System.Net;
    using System.Threading.Tasks;

    public interface IRemoteService
    {
        Task<HttpStatusCode> ReceiveMsg(Message msg);
    }
}