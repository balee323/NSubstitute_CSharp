using Ardalis.GuardClauses;
using NSubsituteSimpleDemo.Interfaces;
using NSubsituteSimpleDemo.Models;
using System.Text;

namespace NSubsituteSimpleDemo
{
    public class OrderProcessor
    {
        private IReceiptWriter _receiptWriter;
        private IOrderRepository _orderRepository;
        private IErrorQueue _errorQueue;

        public OrderProcessor(IReceiptWriter receiptWriter, IOrderRepository orderRepository, IErrorQueue errorQueue)
        {
            _receiptWriter = receiptWriter;
            _orderRepository = orderRepository;
            _errorQueue = errorQueue;
        }


        public async Task<OrderReceipt> ProcessOrderAsync(OrderRequest request)
        {
            Guard.Against.Null(request.CustomerName);

            try
            {
           
                List<OrderDetail> orderDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderDetail>>(request.OrderJson) 
                    ?? new List<OrderDetail>();

                await CheckItemsInStock(orderDetails);

                Order order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerName = request.CustomerName,
                    OrderDate = DateTime.UtcNow,
                    OrderDescription = request.OrderDescription,
                    OrderJson = request.OrderJson
                };


                await _orderRepository.InsertOrderAsync(order);
                _receiptWriter.GenerateReceipt(order);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
               await _errorQueue.SendToQueue(request);
               return new OrderReceipt { OrderStatus = "Error placing order." };
            }

            return new OrderReceipt{ OrderStatus = "Order being prepared for shipment." };
        }

        private async Task CheckItemsInStock(List<OrderDetail> orderDetails)
        {
            StringBuilder itemStatus = new StringBuilder();

            foreach(var orderDetail in orderDetails)
            {
                orderDetail.InStock = await CheckIfItemIsAvaliable(orderDetail.ItemNumber);                
            }
        }


        private async Task<bool> CheckIfItemIsAvaliable(int itemNumber)
        {
           return await _orderRepository.QuanityAvaliableOfItem(itemNumber) > 0;

        }
     

    }
}
