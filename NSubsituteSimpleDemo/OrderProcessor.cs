﻿using Ardalis.GuardClauses;
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
            //pass in a null
            Guard.Against.Null(request.CustomerName);

            try
            {
           
                //blow up the JSON
                List<OrderDetail> orderDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderDetail>>(request.OrderJson) 
                    ?? new List<OrderDetail>();

                await CheckItemsInStock(orderDetails);

                Order order = new Order
                {
                    CustomerName = request.CustomerName,
                    OrderDate = DateTime.UtcNow,
                    OrderDescription = request.OrderDescription,
                    OrderJson = request.OrderJson,
                    OrderDetails = orderDetails
                };

                var orderId = await _orderRepository.InsertOrderAsync(order);
                var receipt = _receiptWriter.GenerateReceipt(order, orderId);
                return receipt;
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
                //verify that request goes to _errorQueue.SendToQueue
               await _errorQueue.SendToQueue(request);
                //verify that order status has error
               return new OrderReceipt { OrderStatus = "Error placing order." };
            }

        }

        private async Task<List<OrderDetail>> CheckItemsInStock(List<OrderDetail> orderDetails)
        {
            StringBuilder itemStatus = new StringBuilder();

            foreach(var orderDetail in orderDetails)
            {
                orderDetail.InStock = await CheckIfItemIsAvaliable(orderDetail.ItemNumber);                
            }

            return orderDetails;
        }

        private async Task<bool> CheckIfItemIsAvaliable(int itemNumber)
        {
            //have repository send back 0 on a few items
           return await _orderRepository.QuanityAvaliableOfItem(itemNumber) > 0;

        }   

    }
}
