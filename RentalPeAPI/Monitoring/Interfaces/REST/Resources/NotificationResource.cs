// Monitoring/Interfaces/REST/Resources/NotificationResource.cs
using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para presentar los datos de una notificación en un espacio.
/// </summary>
public record NotificationResource(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("userId")] Guid UserId,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
);