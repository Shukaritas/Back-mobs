using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para crear un dispositivo IoT vinculado a un espacio (Space).
/// </summary>
public record CreateIoTDeviceResource(
    [property: Required(ErrorMessage = "SpaceId es requerido")]
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: Required(ErrorMessage = "Type es requerido")]
    [property: JsonPropertyName("type")] string Type,
    // opcionales: los puedes omitir desde el front
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("serialNumber")] string? SerialNumber
);