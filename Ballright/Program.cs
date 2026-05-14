using Ballright.Data;
using Microsoft.EntityFrameworkCore;


class Program
{
       async static Task Main()
    {
        using (var context = new ApplicationDbContext())
        {
            var users = await context.Users.ToListAsync();
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.UserName}");
            }

        }

    }

}