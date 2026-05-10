using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record IoTDeviceResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("projectId")] long ProjectId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("installedAt")] DateTime InstalledAt
);