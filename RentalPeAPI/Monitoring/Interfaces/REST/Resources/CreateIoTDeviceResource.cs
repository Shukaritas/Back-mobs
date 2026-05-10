using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record CreateIoTDeviceResource(
    [property: JsonPropertyName("projectId")] long ProjectId,
    [property: JsonPropertyName("type")] string Type,
    // opcionales: los puedes omitir desde el front
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("serialNumber")] string? SerialNumber
);