using NSubsituteSimpleDemo.Interfaces;
using NSubsituteSimpleDemo.Models;
using System.Text;

namespace NSubsituteSimpleDemo
{
    public class ConsoleWriter : IReceiptWriter
    {
        public OrderReceipt GenerateReceipt(Order order, Guid orderId)
        {

            var sb = new StringBuilder();
            sb.AppendLine("**********************************************************");
            sb.AppendLine($"Customer: {order.CustomerName}");
            sb.AppendLine($"Order Id: {orderId}");
            sb.AppendLine($"Order Date: {order.OrderDate}");
            sb.AppendLine($"Order Description: {order.OrderDescription}");
            sb.AppendLine($"Order: {order.OrderJson}");
            sb.AppendLine("**********************************************************");

            string receipt = sb.ToString();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(receipt);
            Console.ResetColor();

            return new OrderReceipt
            {
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderId = orderId,
                OrderDescription = order.OrderDescription,
                OrderStatus = "Preparing for shipment",
                OrderDeliveryEstimate = DateTime.Now.AddDays(-7)
            };
        }
    }
}
