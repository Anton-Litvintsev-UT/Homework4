using Homework3.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Data
{
    public class ZooDbContext : DbContext
    {
        public DbSet<AbstractAnimal> Animals { get; set; }

        public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AbstractAnimal>().HasKey(a => a.Id);

            modelBuilder.Entity<AbstractAnimal>()
                .HasDiscriminator<string>("AnimalType")
                .HasValue<Jellyfish>(nameof(Jellyfish))
                .HasValue<Penguin>(nameof(Penguin))
                .HasValue<Whale>(nameof(Whale));

            base.OnModelCreating(modelBuilder);
        }
    }
}