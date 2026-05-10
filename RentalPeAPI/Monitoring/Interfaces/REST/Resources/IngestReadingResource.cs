// Monitoring/Interfaces/REST/Resources/IngestReadingResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la ingesta de lecturas de telemetría de dispositivos IoT en un espacio.
/// </summary>
public class IngestReadingResource
{
    [Required(ErrorMessage = "SpaceId es requerido")]
    [JsonPropertyName("spaceId")]
    public long SpaceId { get; init; }

    [Required(ErrorMessage = "IoTDeviceId es requerido")]
    [JsonPropertyName("iotDeviceId")]
    public long IoTDeviceId { get; init; }

    [Required(ErrorMessage = "MetricName es requerido")]
    [JsonPropertyName("metricName")]
    public string MetricName { get; init; } = default!;

    [Required(ErrorMessage = "Value es requerido")]
    [JsonPropertyName("value")]
    public decimal Value { get; init; }

    [Required(ErrorMessage = "Unit es requerido")]
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = default!;

    [Required(ErrorMessage = "Timestamp es requerido")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    public IngestReadingResource() { }
}