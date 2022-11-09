using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo.Interfaces
{
    public interface IReceiptWriter
    {
        OrderReceipt GenerateReceipt(Order order, Guid orderId);
    }
}
