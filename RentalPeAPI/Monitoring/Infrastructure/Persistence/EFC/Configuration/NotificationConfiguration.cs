using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

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

        // FK a User (del lado dbjson) -> "userId"
        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // FK a Project -> "projectId"
        builder.Property(n => n.ProjectId)
            .HasColumnName("project_id")
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

        // "message" en el dbjson
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

        // "createdAt" en el dbjson
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(n => n.SentAt)
            .HasColumnName("sent_at")
            .IsRequired(false);

        // Índices
        builder.HasIndex(n => n.ProjectId);
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IncidentId);
    }
}
