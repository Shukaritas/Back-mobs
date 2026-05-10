using System;

namespace RentalPeAPI.Monitoring.Domain.Model.Aggregates;

public class IoTDevice
{
    public long Id { get; private set; }
    public long SpaceId { get; private set; }
    
    // Alias para SpaceId (usado en algunos contextos como ProjectId)
    public long ProjectId => SpaceId;

    public string Name { get; private set; } = string.Empty;      // columna NOT NULL en MySQL
    public string SerialNumber { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;      // "Temperature Sensor"
    public string Status { get; private set; } = "active";        // igual que dbjson
    public DateTime InstalledAt { get; private set; } = DateTime.UtcNow;

    // EF
    private IoTDevice() { }

    public IoTDevice(long spaceId, string type, string? name, string? serialNumber)
    {
        if (spaceId <= 0) throw new ArgumentException("SpaceId debe ser > 0", nameof(spaceId));
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Type es obligatorio", nameof(type));

        SpaceId = spaceId;
        Type = type;
        Name = string.IsNullOrWhiteSpace(name) ? type : name;   // si no mandas name, usamos type
        SerialNumber = serialNumber ?? string.Empty;
        Status = "active";
        InstalledAt = DateTime.UtcNow;
    }
}