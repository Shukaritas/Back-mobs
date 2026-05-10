using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Notification.
/// Alineada con SpaceId en lugar del obsoleto ProjectId.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        // PK
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .IsRequired();

        // FK a User (del lado DDD) -> "user_id"
        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // FK a Space (reemplaza el obsoleto project_id) -> "space_id"
        builder.Property(n => n.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();

        // IncidentId puede ser null (no todas las notificaciones vienen de un incidente)
        builder.Property(n => n.IncidentId)
            .HasColumnName("incident_id")
            .IsRequired(false);

        // Recipient es más interno, lo dejamos opcional
        builder.Property(n => n.Recipient)
            .HasColumnName("recipient")
            .HasMaxLength(150)
            .IsRequired(false);

        // "message"
        builder.Property(n => n.Message)
            .HasColumnName("message")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        // "createdAt"
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(n => n.SentAt)
            .HasColumnName("sent_at")
            .IsRequired(false);

        // Índices
        builder.HasIndex(n => n.SpaceId);
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IncidentId);
    }
}
