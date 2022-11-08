using NSubsituteSimpleDemo.Interfaces;
using NSubsituteSimpleDemo.Models;
using System.Text;

namespace NSubsituteSimpleDemo
{
    public class ConsoleWriter : IReceiptWriter
    {
        public void GenerateReceipt(Order order)
        {

            var sb = new StringBuilder();
            sb.AppendLine("**********************************************************");
            sb.AppendLine($"Customer: {order.CustomerName}");
            sb.AppendLine($"Order Id: {order.OrderId}");
            sb.AppendLine($"Order Date: {order.OrderDate}");
            sb.AppendLine($"Order Description: {order.OrderDescription}");
            sb.AppendLine($"Order: {order.OrderJson}");
            sb.AppendLine("**********************************************************");      

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(sb.ToString());
            Console.ResetColor();
        }
    }
}
