namespace Ballright.Modules
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<OrderProduct> OrderProducts { get; set; } = new();

    }
}
