using System.Globalization;
using Ballright.Data;
using Ballright.Modules;
using Microsoft.EntityFrameworkCore;

class Program
{
    private const string ExitOption = "0";

    private static async Task Main()
    {
        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        await AddUser();
                        break;
                    case "2":
                        await AddProduct();
                        break;
                    case "3":
                        await MakeOrder();
                        break;
                    case "4":
                        await ShowUsers();
                        break;
                    case "5":
                        await ShowProducts();
                        break;
                    case "6":
                        await ShowOrders();
                        break;
                    case ExitOption:
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Please choose a valid menu option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }

            Pause();
        }
    }

    private static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Ballright Menu ===");
        Console.WriteLine("1. Add user");
        Console.WriteLine("2. Add product");
        Console.WriteLine("3. Make order");
        Console.WriteLine("4. Show users");
        Console.WriteLine("5. Show products");
        Console.WriteLine("6. Show orders");
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
    }

    private static async Task AddUser()
    {
        var userName = ReadRequiredText("User name: ");

        await using var context = new ApplicationDbContext();
        var user = new User { UserName = userName };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        Console.WriteLine($"Added user #{user.UserId}: {user.UserName}");
    }

    private static async Task AddProduct()
    {
        var productName = ReadRequiredText("Product name: ");

        await using var context = new ApplicationDbContext();
        var product = new Product { ProductName = productName };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        Console.WriteLine($"Added product #{product.ProductID}: {product.ProductName}");
    }

    private static async Task MakeOrder()
    {
        await using var context = new ApplicationDbContext();

        var users = await context.Users
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .ToListAsync();

        if (users.Count == 0)
        {
            Console.WriteLine("Add at least one user before making an order.");
            return;
        }

        var products = await context.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductID)
            .ToListAsync();

        if (products.Count == 0)
        {
            Console.WriteLine("Add at least one product before making an order.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"{user.UserId}. {user.UserName}");
        }

        var userId = ReadExistingUserId(users);

        Console.WriteLine();
        Console.WriteLine("Products:");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.ProductID}. {product.ProductName}");
        }

        var productIds = ReadExistingProductIds(products);
        var price = ReadDecimal("Order total price: ");

        var order = new Order
        {
            UserID = userId,
            Price = price,
            OrderProducts = productIds
                .Select(productId => new OrderProduct { ProductID = productId })
                .ToList()
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        Console.WriteLine($"Created order #{order.OrderID} with {productIds.Count} product(s).");
    }

    private static async Task ShowUsers()
    {
        await using var context = new ApplicationDbContext();
        var users = await context.Users
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .ToListAsync();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        Console.WriteLine("Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"{user.UserId}. {user.UserName}");
        }
    }

    private static async Task ShowProducts()
    {
        await using var context = new ApplicationDbContext();
        var products = await context.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductID)
            .ToListAsync();

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine("Products:");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.ProductID}. {product.ProductName}");
        }
    }

    private static async Task ShowOrders()
    {
        await using var context = new ApplicationDbContext();
        var orders = await context.Orders
            .AsNoTracking()
            .Include(order => order.User)
            .Include(order => order.OrderProducts)
            .ThenInclude(orderProduct => orderProduct.Product)
            .OrderBy(order => order.OrderID)
            .ToListAsync();

        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
            return;
        }

        Console.WriteLine("Orders:");
        foreach (var order in orders)
        {
            var productNames = order.OrderProducts
                .Select(orderProduct => orderProduct.Product.ProductName);

            Console.WriteLine(
                $"#{order.OrderID} | User: {order.User.UserName} | Total: {order.Price:0.00} | Products: {string.Join(", ", productNames)}");
        }
    }

    private static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Value cannot be empty.");
        }
    }

    private static int ReadExistingUserId(List<User> users)
    {
        var userIds = users.Select(user => user.UserId).ToHashSet();

        while (true)
        {
            var userId = ReadInt("User id: ");

            if (userIds.Contains(userId))
            {
                return userId;
            }

            Console.WriteLine("Choose a user id from the list.");
        }
    }

    private static List<int> ReadExistingProductIds(List<Product> products)
    {
        var productIds = products.Select(product => product.ProductID).ToHashSet();

        while (true)
        {
            Console.Write("Product ids separated by commas: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Choose at least one product.");
                continue;
            }

            var selectedIds = new List<int>();
            var values = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var allValuesAreValid = true;

            foreach (var value in values)
            {
                if (!int.TryParse(value.Trim(), out var productId) || !productIds.Contains(productId))
                {
                    allValuesAreValid = false;
                    break;
                }

                if (!selectedIds.Contains(productId))
                {
                    selectedIds.Add(productId);
                }
            }

            if (allValuesAreValid && selectedIds.Count > 0)
            {
                return selectedIds;
            }

            Console.WriteLine("Choose product ids from the list, for example: 1,2,3");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid number.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out var value) ||
                decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid price.");
        }
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}
