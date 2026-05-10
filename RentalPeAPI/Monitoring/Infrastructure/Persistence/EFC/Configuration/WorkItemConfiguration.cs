// Monitoring/Infrastructure/Persistence/EFC/Configuration/WorkItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        // Nombre de la tabla en BD
        builder.ToTable("tasks");

        // PK
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired();
        
        // FKs / Relacionales
        builder.Property(t => t.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();

        builder.Property(t => t.AssignedToRemodelerId)
            .HasColumnName("assigned_to_remodeler_id")
            .IsRequired();

        // Propiedades
        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(t => t.CompletedAt)
            .HasColumnName("completed_at")
            .IsRequired(false);

        // Índices
        builder.HasIndex(t => t.SpaceId);
        builder.HasIndex(t => t.AssignedToRemodelerId);
    }
}