using Microsoft.EntityFrameworkCore;
using TruckBot.Model.Auto;
using TruckBot.Model.User;


namespace TruckBot.Data
{
    internal class ApplicationDbContext : DbContext
    {
        #region Properties
        
        public DbSet<User> Users { get; set; } 
        public DbSet<Auto> AutoList { get; set; } 
        
        #endregion


        #region ClassLifeCycles

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion
    }
}