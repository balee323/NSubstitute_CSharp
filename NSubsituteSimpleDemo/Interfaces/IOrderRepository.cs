using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo.Interfaces
{
    public interface IOrderRepository
    {
        Task<Guid> InsertOrderAsync(Order order);

        Task GetOrderDetailsAsync(Guid orderId);

        Task<int> QuanityAvaliableOfItem(int itemNumber);

    }
}
