// Monitoring/Interfaces/REST/Resources/ProjectResource.cs
using System;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record ProjectResource(
    int Id,
    int? PropertyId,
    Guid UserId,
    string Name,
    string Description,
    string Status,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime CreatedAt
);