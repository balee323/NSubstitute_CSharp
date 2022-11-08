using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSubsituteSimpleDemo.Models
{
    public class OrderReceipt
    {

        public string OrderDescription { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid OrderId { get; set; }
        public DateTime OrderDeliveryEstimate { get; set; }
        public string OrderStatus { get; set; }
    }
}
