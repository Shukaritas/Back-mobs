// Monitoring/Infrastructure/Persistence/EFC/Configuration/WorkItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad WorkItem.
/// Define el mapeo a la tabla 'tasks' en la base de datos.
/// </summary>
public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        // Nombre de la tabla en BD
        builder.ToTable("tasks");

        // PK - Autogenerado
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired();
        
        // FKs / Columnas de Relación
        builder.Property(t => t.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();

        builder.Property(t => t.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        // Propiedades principales
        builder.Property(t => t.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.PlannedStartDate)
            .HasColumnName("planned_start_date")
            .IsRequired();

        builder.Property(t => t.PlannedEndDate)
            .HasColumnName("planned_end_date")
            .IsRequired();

        // Estado y auditoría
        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.CompletedAt)
            .HasColumnName("completed_at")
            .IsRequired(false);

        // Índices para optimizar queries
        builder.HasIndex(t => t.SpaceId);
        builder.HasIndex(t => t.CreatedByUserId);
    }
}