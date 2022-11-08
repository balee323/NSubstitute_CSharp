using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo.Interfaces
{
    public interface IErrorQueue
    {
        Task SendToQueue(OrderRequest request);
    }
}
