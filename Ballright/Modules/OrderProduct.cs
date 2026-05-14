namespace Ballright.Modules
{
    public class OrderProduct
    {
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int ProductID { get; set; }

        public Product Product { get; set; }
    }
}
