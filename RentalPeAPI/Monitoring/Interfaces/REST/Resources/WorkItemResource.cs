// Monitoring/Interfaces/REST/Resources/WorkItemResource.cs
using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para presentar los datos de una tarea (WorkItem).
/// </summary>
public record WorkItemResource(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("assignedToRemodelerId")] Guid AssignedToRemodelerId,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("completedAt")] DateTime? CompletedAt
);