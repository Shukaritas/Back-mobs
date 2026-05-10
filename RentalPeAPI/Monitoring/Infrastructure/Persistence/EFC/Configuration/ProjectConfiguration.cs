// Monitoring/Infrastructure/Persistence/EFC/Configuration/ProjectConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        // PK
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();

        // FK a Property (puede ser null, como en el dbjson)
        builder.Property(p => p.PropertyId)
            .HasColumnName("property_id")
            .IsRequired(false);

        // FK a User (Guid, obligatorio)
        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Propiedades principales
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(p => p.EndDate)
            .HasColumnName("end_date")
            .IsRequired(false); // puede ser null

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Índices (sin Unique en property_id: una propiedad puede tener varios proyectos)
        builder.HasIndex(p => p.PropertyId);
        builder.HasIndex(p => p.UserId);
    }
}