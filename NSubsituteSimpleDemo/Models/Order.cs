namespace NSubsituteSimpleDemo.Models
{
    public class Order
    {
        public string OrderDescription { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid OrderId { get; set; }
        public string OrderJson { get; set; }
    }
}
