using System;

namespace RentalPeAPI.Monitoring.Domain.Model.Aggregates;

/// <summary>
/// Dispositivo IoT vinculado a un espacio (Space) con simulación automática de telemetría
/// y validaciones de seguridad por usuario creador.
/// </summary>
public class IoTDevice
{
    // Tipos de sensores permitidos
    private static readonly HashSet<string> AllowedTypes = new()
    {
        "HUMIDITY", "TEMPERATURE", "VOLTAGE", "LOAD", "AIR_QUALITY", "OTHER", "OTHERS"
    };

    // Mapa de autocompletado de métricas
    private static readonly Dictionary<string, (string MetricName, string Unit)> MetricMap = new()
    {
        { "HUMIDITY", ("Humedad Relativa", "%") },
        { "TEMPERATURE", ("Temperatura", "C°") },
        { "VOLTAGE", ("Voltaje", "V") },
        { "LOAD", ("Carga", "kg") },
        { "AIR_QUALITY", ("Calidad del Aire", "AQI") }
    };

    // Propiedades de solo lectura con encapsulamiento
    public long Id { get; private set; }
    public long SpaceId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public string MetricName { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public int Value { get; private set; }
    public DateTime Timestamp { get; private set; }
    public bool IsOn { get; private set; }

    // Constructor privado para EF Core
    private IoTDevice() { }

    /// <summary>
    /// Constructor principal para crear un nuevo dispositivo IoT con autocompletado de métricas.
    /// </summary>
    /// <param name="spaceId">ID del espacio propietario del dispositivo</param>
    /// <param name="createdByUserId">ID del usuario que crea el dispositivo</param>
    /// <param name="type">Tipo de sensor (HUMIDITY, TEMPERATURE, VOLTAGE, LOAD, AIR_QUALITY, OTHER, OTHERS)</param>
    /// <param name="name">Nombre del dispositivo</param>
    /// <param name="serialNumber">Número de serie del dispositivo</param>
    /// <param name="customMetricName">Nombre de métrica personalizada (requerida si type es OTHER/OTHERS)</param>
    /// <param name="customUnit">Unidad personalizada (requerida si type es OTHER/OTHERS)</param>
    public IoTDevice(
        long spaceId,
        Guid createdByUserId,
        string type,
        string name,
        string serialNumber,
        string? customMetricName = null,
        string? customUnit = null)
    {
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser > 0", nameof(spaceId));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId debe ser un GUID válido", nameof(createdByUserId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type es obligatorio", nameof(type));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name es obligatorio", nameof(name));

        // Convertir a mayúsculas y validar tipo
        Type = type.ToUpperInvariant();
        if (!AllowedTypes.Contains(Type))
            throw new ArgumentException(
                $"Type '{Type}' no permitido. Opciones: {string.Join(", ", AllowedTypes)}",
                nameof(type));

        // Autocompletar MetricName y Unit según el tipo
        if (MetricMap.TryGetValue(Type, out var metrics))
        {
            MetricName = metrics.MetricName;
            Unit = metrics.Unit;
        }
        else if (Type is "OTHER" or "OTHERS")
        {
            // Para tipos personalizados, validar que se proporcionen los valores
            if (string.IsNullOrWhiteSpace(customMetricName))
                throw new ArgumentException(
                    "customMetricName es obligatorio cuando Type es OTHER u OTHERS",
                    nameof(customMetricName));

            if (string.IsNullOrWhiteSpace(customUnit))
                throw new ArgumentException(
                    "customUnit es obligatorio cuando Type es OTHER u OTHERS",
                    nameof(customUnit));

            MetricName = customMetricName;
            Unit = customUnit;
        }

        SpaceId = spaceId;
        CreatedByUserId = createdByUserId;
        Name = name;
        SerialNumber = serialNumber ?? string.Empty;
        IsOn = true;
        Timestamp = DateTime.UtcNow;
        GenerateRandomValue(); // Genera el valor inicial
    }

    /// <summary>
    /// Actualiza el nombre y número de serie del dispositivo.
    /// </summary>
    public void UpdateDetails(string name, string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name es obligatorio", nameof(name));

        Name = name;
        SerialNumber = serialNumber ?? string.Empty;
    }

    /// <summary>
    /// Alterna el estado de encendido/apagado del dispositivo.
    /// Si se enciende, genera inmediatamente un nuevo valor de telemetría.
    /// </summary>
    public void TogglePower()
    {
        IsOn = !IsOn;

        // Si se enciende, generar un nuevo valor inmediatamente
        if (IsOn)
        {
            GenerateRandomValue();
        }
    }

    /// <summary>
    /// Genera un valor aleatorio de telemetría y actualiza el timestamp.
    /// Solo se ejecuta si el dispositivo está encendido (IsOn = true).
    /// </summary>
    public void GenerateRandomValue()
    {
        if (!IsOn)
            return;

        Value = Random.Shared.Next(10, 100);
        Timestamp = DateTime.UtcNow;
    }
}