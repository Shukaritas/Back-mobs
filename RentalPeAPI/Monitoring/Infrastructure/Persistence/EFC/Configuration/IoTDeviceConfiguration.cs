using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public class IoTDeviceConfiguration : IEntityTypeConfiguration<IoTDevice>
{
    public void Configure(EntityTypeBuilder<IoTDevice> builder)
    {
        builder.ToTable("iot_devices");

        // Primary Key
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .IsRequired();

        // SpaceId
        builder.Property(d => d.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();
        builder.HasIndex(d => d.SpaceId);

        // CreatedByUserId
        builder.Property(d => d.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();
        builder.HasIndex(d => d.CreatedByUserId);

        // Type (HUMIDITY, TEMPERATURE, VOLTAGE, LOAD, AIR_QUALITY, OTHER, OTHERS)
        builder.Property(d => d.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired();

        // Name
        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // SerialNumber
        builder.Property(d => d.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(100)
            .IsRequired(false);

        // MetricName - Nombre de la métrica (ej. "Humedad Relativa", "Temperatura")
        builder.Property(d => d.MetricName)
            .HasColumnName("metric_name")
            .HasMaxLength(100)
            .IsRequired();

        // Unit - Unidad de medida (ej. "%", "C°", "V", "kg", "AQI")
        builder.Property(d => d.Unit)
            .HasColumnName("unit")
            .HasMaxLength(50)
            .IsRequired();

        // Value - Valor numérico de telemetría
        builder.Property(d => d.Value)
            .HasColumnName("value")
            .IsRequired();

        // Timestamp - Marca de tiempo del último valor generado
        builder.Property(d => d.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

        // IsOn - Estado de encendido/apagado
        builder.Property(d => d.IsOn)
            .HasColumnName("is_on")
            .IsRequired();
        
        // MinThreshold - Umbral mínimo de telemetría (propiedad privada)
        builder.Property<decimal>("MinThreshold")
            .HasColumnName("min_threshold")
            .IsRequired();
        
        // MaxThreshold - Umbral máximo de telemetría (propiedad privada)
        builder.Property<decimal>("MaxThreshold")
            .HasColumnName("max_threshold")
            .IsRequired();
        
        // IsInAlertState - Indica si el valor ha cruzado los umbrales (propiedad privada)
        builder.Property<bool>("IsInAlertState")
            .HasColumnName("is_in_alert_state")
            .IsRequired()
            .HasDefaultValue(false);
    }
}