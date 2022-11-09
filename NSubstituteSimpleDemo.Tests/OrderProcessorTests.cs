using NSubsituteSimpleDemo;
using NSubsituteSimpleDemo.Interfaces;
using NSubsituteSimpleDemo.Models;
using NSubstitute;

namespace NSubstituteSimpleDemo.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class OrderProcessorTests
    {

        // 1. Shows basic use of NSub stubbing
        [TestMethod]
        public async Task OrderProcessor_HappyPath_ExpectNoException()
        {

            // Arrange (these dependencies are being stubbed out, so no behavior has been added)
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            OrderReceipt stubbedOutReceipt = new OrderReceipt { OrderStatus = "Shipping" };

            // you can set the stubbed out return value for any args.
            receiptWriter.GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>()).ReturnsForAnyArgs(stubbedOutReceipt);

            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = "Brian Lee",
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act
           var orderReceipt = await orderProcessor.ProcessOrderAsync(orderRequest);

            // Assert

            // Received just 1 call
            receiptWriter.Received(1).GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // We made 2 calls to check if the games were avaliable
            await orderRepository.ReceivedWithAnyArgs(2).QuanityAvaliableOfItem(Arg.Any<int>());
            
            // We inserted the order into the Repository
            await orderRepository.ReceivedWithAnyArgs(1).InsertOrderAsync(Arg.Any<Order>());

            // let's make sure ErrorQueue.SendToQueue did not get called
            await errorQueue.DidNotReceive().SendToQueue(Arg.Any<OrderRequest>());

            Assert.IsNotNull(orderReceipt);
            Assert.IsFalse(orderReceipt.OrderStatus.ToLower().Contains("error"));
        }


        // 2. Exception (No calls made)
        [TestMethod]
        public async Task OrderProcessor_NullCustomerName_ExpectException()
        {

            // Arrange (these dependencies are being stubbed out, so no behavior has been added)
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = null,
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act & Assert
            OrderReceipt? orderReceipt = null;

            // Expecting due to exception thrown outside try/catch
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => orderReceipt = await orderProcessor.ProcessOrderAsync(orderRequest));

            // Received no calls
            receiptWriter.DidNotReceive().GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // Received no calls
            await orderRepository.DidNotReceive().QuanityAvaliableOfItem(Arg.Any<int>());

            // Received no calls
            await orderRepository.DidNotReceive().InsertOrderAsync(Arg.Any<Order>());

            // Received no calls
            await errorQueue.DidNotReceive().SendToQueue(Arg.Any<OrderRequest>());

            // orderReceipt should be null
            Assert.IsNull(orderReceipt);
        }



        // 3. Shows basic use of NSub mocking (injected behavior => throw exception)
        [TestMethod]
        public async Task OrderProcessor_RepositoryExpection_ExpectException()
        {

            // Arrange (these dependencies are being stubbed out, so no behavior has been added)
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            orderRepository.InsertOrderAsync(Arg.Any<Order>()).Returns<Guid>(x => { throw new Exception("Some kind of bad juju."); });

            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = "Brian Lee",
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act
            var orderReceipt = await orderProcessor.ProcessOrderAsync(orderRequest);

            // Assert

            // We made 2 calls to check if the games were avaliable
            await orderRepository.ReceivedWithAnyArgs(2).QuanityAvaliableOfItem(Arg.Any<int>());

            // We inserted the order into the Repository
            await orderRepository.ReceivedWithAnyArgs(1).InsertOrderAsync(Arg.Any<Order>());

            // expect no calls received
            receiptWriter.DidNotReceive().GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // expect 1 call here
            await errorQueue.Received(1).SendToQueue(Arg.Any<OrderRequest>());

            Assert.IsNotNull(orderReceipt);
            Assert.IsTrue(orderReceipt.OrderStatus.ToLower().Contains("error"));
        }


        // 4. Ignoring Arguments, stub out return
        [TestMethod]
        public async Task OrderProcessor_VerifyOrderId_ExpectNoExceptions()
        {

            // Arrange (these dependencies are being stubbed out, so no behavior has been added)
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            orderRepository.InsertOrderAsync(Arg.Any<Order>()).Returns<Guid>(x => Guid.Parse("9fcc76b3-6c7f-4ea8-a0cf-f883f013875b"));
         
            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = "Brian Lee",
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act 
            OrderReceipt? orderReceipt = await orderProcessor.ProcessOrderAsync(orderRequest);

            // Assert 

            // Received just 1 call
            receiptWriter.Received(1).GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // We made 2 calls to check if the games were avaliable
            await orderRepository.ReceivedWithAnyArgs(2).QuanityAvaliableOfItem(Arg.Any<int>());

            // We inserted the order into the Repository
            await orderRepository.ReceivedWithAnyArgs(1).InsertOrderAsync(Arg.Any<Order>());

            // let's make sure ErrorQueue.SendToQueue did not get called
            await errorQueue.DidNotReceive().SendToQueue(Arg.Any<OrderRequest>());

            // now we can check if the correct orderId was received
            receiptWriter.Received().GenerateReceipt(Arg.Any<Order>(), Arg.Is<Guid>(x => x == Guid.Parse("9fcc76b3-6c7f-4ea8-a0cf-f883f013875b")));

        }


        // 5. Argument Matching
        [TestMethod]
        public async Task OrderProcessor_VerifyCustomerName_ExpectNoExceptions()
        {

            // Arrange
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            orderRepository.InsertOrderAsync(Arg.Is<Order>(x => x.CustomerName == "Brian Lee")).Returns<Guid>(x => Guid.Parse("9fcc76b3-6c7f-4ea8-a0cf-f883f013875b"));
            orderRepository.InsertOrderAsync(Arg.Is<Order>(x => x.CustomerName == "Dwayne Stammer")).Returns<Guid>(x => Guid.Parse("be128a7e-37c6-4dca-9216-47f01a1ffaf9"));


            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = "Brian Lee",
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act 
            OrderReceipt? orderReceipt = await orderProcessor.ProcessOrderAsync(orderRequest);

            // Assert 

            // Received just 1 call
            receiptWriter.Received(1).GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // We made 2 calls to check if the games were avaliable
            await orderRepository.ReceivedWithAnyArgs(2).QuanityAvaliableOfItem(Arg.Any<int>());

            // We inserted the order into the Repository
            await orderRepository.ReceivedWithAnyArgs(1).InsertOrderAsync(Arg.Any<Order>());

            // let's make sure ErrorQueue.SendToQueue did not get called
            await errorQueue.DidNotReceive().SendToQueue(Arg.Any<OrderRequest>());

            // now we can check if the correct orderId was assigned as expected
            receiptWriter.Received().GenerateReceipt(Arg.Is<Order>(x => x.CustomerName == "Brian Lee"), Arg.Is<Guid>(x => x == Guid.Parse("9fcc76b3-6c7f-4ea8-a0cf-f883f013875b")));

            // Sanity check
            receiptWriter.DidNotReceive().GenerateReceipt(Arg.Is<Order>(x => x.CustomerName == "Dwayne Stammer"), Arg.Is<Guid>(x => x == Guid.Parse("be128a7e-37c6-4dca-9216-47f01a1ffaf9")));
            // simplier sanity check
            receiptWriter.DidNotReceive().GenerateReceipt(Arg.Is<Order>(x => x.CustomerName == "Dwayne Stammer"), Arg.Any<Guid>());
        }


        // 6. Multiple return values
        [TestMethod]
        public async Task OrderProcessor_VerifyInventory_ExpectNoExceptions()
        {

            // Arrange
            IReceiptWriter receiptWriter = Substitute.For<IReceiptWriter>();
            IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
            IErrorQueue errorQueue = Substitute.For<IErrorQueue>();

            // The first call will return 2 quantity and the 2nd call will return 0 quantity.
            orderRepository.QuanityAvaliableOfItem(Arg.Any<int>()).Returns(2, 0);

            OrderProcessor orderProcessor = new OrderProcessor(receiptWriter, orderRepository, errorQueue);

            var orderRequest = new OrderRequest
            {
                CustomerName = "Brian Lee",
                OrderDescription = "Vintage NES games for Stammer.",
                OrderJson = "[{\"ItemDescription\":\"Stadium Events\",\"ItemNumber\":16575,\"Quanity\":1,\"InStock\":false}," +
                 "{\"ItemDescription\":\"Little Samson\",\"ItemNumber\":58654,\"Quanity\":1,\"InStock\":false}]"
            };

            // Act 
            await orderProcessor.ProcessOrderAsync(orderRequest);

            // Assert 

            // Received just 1 call
            receiptWriter.Received(1).GenerateReceipt(Arg.Any<Order>(), Arg.Any<Guid>());

            // We made 2 calls to check if the games were avaliable
            await orderRepository.ReceivedWithAnyArgs(2).QuanityAvaliableOfItem(Arg.Any<int>());

            // We inserted the order into the Repository
            await orderRepository.ReceivedWithAnyArgs(1).InsertOrderAsync(Arg.Any<Order>());

            // let's make sure ErrorQueue.SendToQueue did not get called
            await errorQueue.DidNotReceive().SendToQueue(Arg.Any<OrderRequest>());

            // We see that both calls to check quantity returned in stock for first game and not in stock for second game.
            receiptWriter.Received(1).GenerateReceipt(Arg.Is<Order>(x => x.OrderDetails[0].InStock == true), Arg.Any<Guid>());
            receiptWriter.Received(1).GenerateReceipt(Arg.Is<Order>(x => x.OrderDetails[1].InStock == false), Arg.Any<Guid>());

        }

    }
}