using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSubsituteSimpleDemo.Models
{
    public class OrderDetail
    {
        public string ItemDescription { get; set; }
        public int ItemNumber { get; set; }
        public int Quanity { get; set; }
        public bool InStock { get; set; } = false;
    }
}
