using Microsoft.EntityFrameworkCore;
using TruckBot.Model.User;


namespace TruckBot.Data
{
    internal class DatabaseService
    {
        private readonly AppConfig _appConfig;
        private readonly ApplicationDbContext _dbContext;


        public DatabaseService(AppConfig appConfig, ApplicationDbContext dbContext)
        {
            _appConfig = appConfig;
            _dbContext = dbContext;
        }

        internal async Task MigrateAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                throw;
            }
            await AddFirstAdminAsync();
        }


        internal async Task<Dictionary<long, User>> LoadAdminListAsync()
        {
            return await _dbContext.Users
               .Where(admin => admin.IsAdmin == true)
               .ToDictionaryAsync(admin => admin.UserId, admin => admin);
        }

        internal async Task<Dictionary<long, User>> LoadUserListAsync()
        {
            return await _dbContext.Users
                .Where(user => user.IsAdmin == false)
                .ToDictionaryAsync(user => user.UserId, user => user);
        }

        private async Task AddFirstAdminAsync()
        {
            var firstAdmin = await _dbContext.Users.FindAsync(_appConfig.FirstAdminId);
            if (firstAdmin == null)
            {
                var newAdmin = new User
                {
                    UserId = _appConfig.FirstAdminId,
                    IsAdmin = true,
                    FirstName = "Administrator",
                };

                _dbContext.Users.Add(newAdmin);
                

                Console.WriteLine($"Пользователь с Id {newAdmin.UserId} был добавлен как Администратор.");

                await _dbContext.SaveChangesAsync();
            }
        }

        internal async Task AddUserAsync(User user)
        {
            var userInServer = await _dbContext.Users.FindAsync(user.UserId);
            if (userInServer == null)
            {
                await _dbContext.Users.AddAsync(user);
            }
            else
            {
                _dbContext.Entry(userInServer).CurrentValues.SetValues(user);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
