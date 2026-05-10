


// Monitoring/Interfaces/ACL/IMonitoringContextFacade.cs
using System;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.ACL;

public interface IMonitoringContextFacade
{
    Task<int> CreateProjectAsync(
        long propertyId,
        Guid userId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate);

    Task<int> RegisterIoTDeviceAsync(
        int projectId,
        string name,
        string serialNumber,
        string type);

    Task IngestTelemetryReadingAsync(
        int projectId,
        int iotDeviceId,
        string metricName,
        decimal value,
        string unit,
        DateTime timestamp);

    Task<int> CreateWorkItemForIncidentAsync(
        int projectId,
        int? incidentId,
        int assignedToUserId,
        string description);
}