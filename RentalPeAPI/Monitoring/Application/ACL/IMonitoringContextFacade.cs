

// Monitoring/Interfaces/ACL/IMonitoringContextFacade.cs
using System;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Anti-Corruption Layer para el Bounded Context de Monitoring.
/// Define el contrato de acceso para otros BCs.
/// </summary>
public interface IMonitoringContextFacade
{
    /// <summary>
    /// Registra un dispositivo IoT en un espacio específico.
    /// </summary>
    Task<int> RegisterIoTDeviceAsync(
        long spaceId,
        string name,
        string serialNumber,
        string type);

    /// <summary>
    /// Ingesta una lectura de telemetría para un espacio y dispositivo.
    /// </summary>
    Task IngestTelemetryReadingAsync(
        long spaceId,
        long iotDeviceId,
        string metricName,
        decimal value,
        string unit,
        DateTime timestamp);

    /// <summary>
    /// Crea una tarea de trabajo vinculada a un espacio.
    /// </summary>
    Task<int> CreateWorkItemForSpaceAsync(
        long spaceId,
        Guid createdByUserId,
        string title,
        string description,
        DateTime plannedStartDate,
        DateTime plannedEndDate);
}