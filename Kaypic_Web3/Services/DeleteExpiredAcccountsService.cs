using Kaypic_Web3.Data;
using System;

namespace Kaypic_Web3.Services
{
    public class DeleteExpiredAccountsService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public DeleteExpiredAccountsService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

                    await DeleteExpiredAccounts(db);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cleanup error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task DeleteExpiredAccounts(MainDbContext db)
        {
            var now = DateTime.UtcNow;

            var expiredUsers = db.Utilisateurs
                .Where(u => u.DeletionScheduledAt != null &&
                           u.DeletionScheduledAt <= now &&
                           !u.IsDeleted)
                .ToList();

            foreach (var user in expiredUsers)
            {
                user.IsDeleted = true;
            }

            await db.SaveChangesAsync();
        }
    }
}