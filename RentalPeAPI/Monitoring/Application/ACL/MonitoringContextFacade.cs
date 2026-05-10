using System;
using System.Threading.Tasks;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.ACL;

using CreateIoTDeviceCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateIoTDeviceCommand;
using IngestReadingCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.IngestReadingCommand;
using CreateWorkItemCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateWorkItemCommand;

/// <summary>
/// Facade Anti-Corruption Layer para Monitoring BC.
/// Proporciona métodos de acceso para otros BCs sin exponer detalles internos.
/// Alineado con SpaceId como llave foránea unificada.
/// </summary>
public class MonitoringContextFacade : IMonitoringContextFacade
{
    private readonly IMediator _mediator;

    public MonitoringContextFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo IoTDevice en un espacio (Space).
    /// </summary>
    public async Task<int> RegisterIoTDeviceAsync(
        long spaceId,
        string name,
        string serialNumber,
        string type)
    {
        var command = new CreateIoTDeviceCommand(
            SpaceId:      spaceId,
            Type:         type,
            Name:         name,
            SerialNumber: serialNumber
        );

        var device = await _mediator.Send(command);

        // El Id del device es long, devolvemos como int
        return (int)device.Id;
    }

    /// <summary>
    /// Envía una lectura de telemetría para un espacio específico.
    /// </summary>
    public async Task IngestTelemetryReadingAsync(
        long spaceId,
        long iotDeviceId,
        string metricName,
        decimal value,
        string unit,
        DateTime timestamp)
    {
        var command = new IngestReadingCommand(
            SpaceId:      spaceId,
            IoTDeviceId:  iotDeviceId,
            MetricName:   metricName,
            Value:        value,
            Unit:         unit,
            Timestamp:    timestamp
        );

        await _mediator.Send(command);
    }

    /// <summary>
    /// Crea una WorkItem asociada a un espacio (Space).
    /// </summary>
    public async Task<int> CreateWorkItemForSpaceAsync(
        long spaceId,
        Guid assignedToRemodelerId,
        string description)
    {
        var command = new CreateWorkItemCommand(
            SpaceId: spaceId,
            AssignedToRemodelerId: assignedToRemodelerId,
            Description: description
        );

        var workItemId = await _mediator.Send(command);
        return workItemId;
    }
}