using Homework3.Models;
using Homework3.Models.Animals;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Data
{
    public class ZooDbContext : DbContext
    {
        public DbSet<AbstractAnimal> Animals { get; set; }
        public DbSet<Enclosure> Enclosures { get; set; }

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

            modelBuilder.Entity<AbstractAnimal>()
                .HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.EnclosureId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enclosure>().HasData(
                new Enclosure { Id = 1, Name = "Aquarium" },
                new Enclosure { Id = 2, Name = "Iceberg" },
                new Enclosure { Id = 3, Name = "Deep Ocean" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}