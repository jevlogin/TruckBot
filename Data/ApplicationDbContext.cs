using Microsoft.EntityFrameworkCore;
using TruckBot.Model.User;


namespace TruckBot.Data
{
    internal class ApplicationDbContext : DbContext
    {
        #region Properties
        
        public DbSet<User> Users { get; set; } 
        
        #endregion


        #region ClassLifeCycles

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Data/local.db");
            }
        }


    }
}