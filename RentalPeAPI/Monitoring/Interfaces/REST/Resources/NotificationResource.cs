// Monitoring/Interfaces/REST/Resources/NotificationResource.cs
using System;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record NotificationResource(
    int Id,
    int UserId,
    int ProjectId,
    string Message,
    DateTime CreatedAt
);