using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Property.Domain.Aggregates;
using RentalPeAPI.Property.Domain.Aggregates.Entities;

namespace RentalPeAPI.Property.Infrastructure.Persistence
{
    public class PropertyDbContext : DbContext
    {
        public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
            : base(options)
        {
           
        }

        public DbSet<Space> Spaces { get; set; }
        public DbSet<Service> Services { get; set; } // 👈 IMPORTANTE

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropertyDbContext).Assembly);
        }
    }
}