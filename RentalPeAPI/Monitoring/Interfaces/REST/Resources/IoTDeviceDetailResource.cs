using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para presentar el detalle completo de un dispositivo IoT con telemetría,
/// umbrales e información de estado de alerta. Incluye toda la metadata para monitoreo.
/// Campos: id, spaceId, type, name, serialNumber, metricName, unit, value, timestamp,
/// isOn, minThreshold, maxThreshold, isInAlertState
/// </summary>
public record IoTDeviceDetailResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("serialNumber")] string SerialNumber,
    [property: JsonPropertyName("metricName")] string MetricName,
    [property: JsonPropertyName("unit")] string Unit,
    [property: JsonPropertyName("value")] double Value,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("isOn")] bool IsOn,
    [property: JsonPropertyName("minThreshold")] decimal MinThreshold,
    [property: JsonPropertyName("maxThreshold")] decimal MaxThreshold,
    [property: JsonPropertyName("isInAlertState")] bool IsInAlertState
);
