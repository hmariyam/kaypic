using Microsoft.AspNetCore.Identity;

namespace Kaypic_Web3.Data.Script
{
    public class UpdatePasswords
    {
        public static async Task RunAsync(MainDbContext context)
        {
            foreach (var user in context.Utilisateurs)
            {
                if (user.Mdp == "Pass123")
                {
                    user.Mdp = BCrypt.Net.BCrypt.HashPassword("Pass123");
                }
            }

            await context.SaveChangesAsync();

            Console.WriteLine("`MDP CHANGÉ");
        }
    }
}