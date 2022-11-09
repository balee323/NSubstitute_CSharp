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
