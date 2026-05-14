namespace Ballright.Modules
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
