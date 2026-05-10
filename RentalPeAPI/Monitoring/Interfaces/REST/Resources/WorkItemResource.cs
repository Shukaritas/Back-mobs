// Monitoring/Interfaces/REST/Resources/WorkItemResource.cs
using System;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record WorkItemResource(
    int Id,
    int ProjectId,
    int? IncidentId,
    int AssignedToUserId,
    string Description,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt
);