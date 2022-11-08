using NSubsituteSimpleDemo.Models;

namespace NSubsituteSimpleDemo.Interfaces
{
    public interface IReceiptWriter
    {
        void GenerateReceipt(Order order);
    }
}
