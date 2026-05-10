// Monitoring/Interfaces/REST/Resources/IngestReadingResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public class IngestReadingResource
{
    [Required]
    [JsonPropertyName("projectId")]
    public int ProjectId { get; init; }

    [Required]
    [JsonPropertyName("iotDeviceId")]
    public int IoTDeviceId { get; init; }

    [Required]
    [JsonPropertyName("metricName")]
    public string MetricName { get; init; } = default!;

    [Required]
    [JsonPropertyName("value")]
    public decimal Value { get; init; }

    [Required]
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = default!;

    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    public IngestReadingResource() { }
}