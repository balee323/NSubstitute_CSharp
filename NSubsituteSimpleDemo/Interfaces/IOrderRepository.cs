using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo.Interfaces
{
    public interface IOrderRepository
    {
        Task InsertOrderAsync(Order order);

        Task GetOrderDetailsAsync(Guid orderId);

        Task<int> QuanityAvaliableOfItem(int itemNumber);

    }
}
