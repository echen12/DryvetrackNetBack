using DryvetrackTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace DryvetrackTest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }  // Add this line

        public DbSet<Insurance> Insurance { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Seed data for the Cars table
        //    modelBuilder.Entity<Car>().HasData(
        //        new Car
        //        {
        //            VIN = "1HGCM82633A004352",
        //            Make = "Honda",
        //            Model = "Accord"
        //        },
        //        new Car
        //        {
        //            VIN = "1J4FA39S36P712345",
        //            Make = "Jeep",
        //            Model = "Wrangler"
        //        },
        //        new Car
        //        {
        //            VIN = "3C4PDCAB2FT123456",
        //            Make = "Chrysler",
        //            Model = "Pacifica"
        //        }
        //    );
        //}
    }
}
