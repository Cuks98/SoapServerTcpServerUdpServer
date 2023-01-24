using ConsoleApp1;
using Microsoft.EntityFrameworkCore;

namespace SoapServer
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        //public DataContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        public DbSet<Trainer> Trainers { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<User>().Property(u => u.LeftOnRegistration).HasComputedColumnSql("DATEDIFF(DAY, GETDATE(), RegisteredTo)");
        //    base.OnModelCreating(builder);
        //}

    }
}
