using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Notification.
/// Mapea correctamente las columnas de base de datos según el modelo DDD refactorizado.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        // PK: long en lugar de int
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .IsRequired();

        // FK a User (destinatario de la alerta)
        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // FK a Space (obra asociada)
        builder.Property(n => n.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();

        // Título descriptivo de la alerta
        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        // Mensaje detallado de la alerta
        builder.Property(n => n.Message)
            .HasColumnName("message")
            .HasMaxLength(1000)
            .IsRequired();

        // Estado de lectura (bool en lugar de string "unread"/"read")
        builder.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();

        // Fecha UTC de creación
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Índices para optimización de consultas
        // Índice compuesto: búsqueda rápida de notificaciones por usuario
        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("idx_notifications_user_id");

        // Índice para búsquedas por espacio
        builder.HasIndex(n => n.SpaceId)
            .HasDatabaseName("idx_notifications_space_id");

        // Índice compuesto para casos de uso comunes: notificaciones no leídas de un usuario
        builder.HasIndex(n => new { n.UserId, n.IsRead })
            .HasDatabaseName("idx_notifications_user_unread");
    }
}
