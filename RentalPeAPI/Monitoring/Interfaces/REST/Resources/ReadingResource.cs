// Monitoring/Interfaces/REST/Resources/ReadingResource.cs
using System;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public record ReadingResource(
    long Id,
    long IoTDeviceId,
    long SpaceId,
    string MetricName,
    decimal Value,
    string Unit,
    DateTime Timestamp
);