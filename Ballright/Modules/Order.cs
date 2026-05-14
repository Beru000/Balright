namespace Ballright.Modules
{
    public class Order
    {
        public int OrderID { get; set; }
        public decimal Price { get; set; }
        public int UserID { get; set; }
        public User User { get; set; } = null!;
        public List<OrderProduct> OrderProducts { get; set; } = new();
    }
}
