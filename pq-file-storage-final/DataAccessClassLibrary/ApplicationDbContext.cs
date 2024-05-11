using DataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessClassLibrary
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor to support migrations
        public ApplicationDbContext()
        {
        }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        // Ensuring database has been created
        Database.Migrate();
    }

        // Overriding OnConfiguring method to configure the database
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            // Get connection string from environment variable
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

            // Configuring the database
            options.UseNpgsql(connectionString);
        }

    public DbSet<User> Users { get; set; }
    }
}
