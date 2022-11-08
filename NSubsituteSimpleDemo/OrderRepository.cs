using NSubsituteSimpleDemo.Interfaces;
using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo
{
    internal class OrderRepository : IOrderRepository
    {
        public Task GetOrderDetailsAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task InsertOrderAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<int> QuanityAvaliableOfItem(int itemNumber)
        {
            throw new NotImplementedException();
        }
    }
}
