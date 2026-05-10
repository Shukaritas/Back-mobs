using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.User.Domain; 
using RentalPeAPI.User.Infrastructure.Persistence.EFC.Configuration; 
using RentalPeAPI.Property.Domain.Aggregates; 
using RentalPeAPI.Property.Domain.Aggregates.Entities; 
using RentalPeAPI.Property.Infrastructure.Persistence.EFC.Configuration; 
using RentalPeAPI.Payments.Infrastructure.Persistence.EFC.configuration.extensions;
using EFCore.NamingConventions; // NECESARIO para UseSnakeCaseNamingConvention

using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

namespace RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 

/// <summary>
/// Represents the application's database context using Entity Framework Core.
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // --- DBSETS DEL BOUNDED CONTEXT USER ---
    public DbSet<User.Domain.User> Users { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = default!;

    // --- DBSETS DEL BOUNDED CONTEXT PROPERTY/SPACES ---
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Service> Services { get; set; }

    // --- DBSETS DEL BOUNDED CONTEXT MONITORING ---
    public DbSet<IoTDevice> IoTDevices { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Reading> Readings { get; set; }
    public DbSet<WorkItem> Tasks { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // --- DBSETS DEL BOUNDED CONTEXT PAYMENTS ---
    public DbSet<Payments.Domain.Model.Aggregates.Payment> Payments { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Add the created and updated interceptor
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- APLICACIÓN DE CONFIGURACIONES (Bounded Contexts Activos) ---
        
        // 1. User BC Configuration
        builder.ApplyConfiguration(new UserConfiguration()); 
        builder.ApplyConfiguration(new PaymentMethodConfiguration());
        
        // 2. Payments BC Configuration
        builder.ApplyPaymentsConfiguration();
        
        // 3. Space/Property BC Configuration
        builder.ApplyConfiguration(new SpaceConfiguration());
        builder.ApplyConfiguration(new ServiceConfiguration());
        
        // 4. Monitoring BC Configuration
        builder.ApplyConfiguration(new ProjectConfiguration()); 
        builder.ApplyConfiguration(new IoTDeviceConfiguration());
        builder.ApplyConfiguration(new ReadingConfiguration());
        builder.ApplyConfiguration(new WorkItemConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());


        // Configuración compartida
        builder.UseSnakeCaseNamingConvention();
    }
}
