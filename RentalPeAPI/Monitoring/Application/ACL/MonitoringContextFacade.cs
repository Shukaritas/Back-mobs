using System;
using System.Threading.Tasks;
using MediatR;



namespace RentalPeAPI.Monitoring.Application.ACL;


using CreateProjectCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateProjectCommand;
using CreateIoTDeviceCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateIoTDeviceCommand;
using IngestReadingCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.IngestReadingCommand;
using CreateWorkItemCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateWorkItemCommand;



public class MonitoringContextFacade : IMonitoringContextFacade
{
    private readonly IMediator _mediator;

    public MonitoringContextFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un proyecto en el BC Monitoring.
    /// </summary>
    public async Task<int> CreateProjectAsync(
        long propertyId,
        Guid userId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate)
    {
        // 👇 Convertimos el long a int porque el CreateProjectCommand
        // está esperando int/int?.
        var command = new CreateProjectCommand(
            PropertyId:  (int)propertyId,
            UserId:      userId,
            Name:        name,
            Description: description,
            StartDate:   startDate,
            EndDate:     endDate
        );

        var projectId = await _mediator.Send(command);
        return projectId;
    }

    /// <summary>
    /// Registra un nuevo IoTDevice en un proyecto.
    /// </summary>
    public async Task<int> RegisterIoTDeviceAsync(
        int projectId,
        string name,
        string serialNumber,
        string type)
    {
        var command = new CreateIoTDeviceCommand(
            ProjectId:    projectId,
            Name:         name,
            SerialNumber: serialNumber,
            Type:         type
        );

        var device = await _mediator.Send(command);

        // 👇 El Id del device es long, pero el método devuelve int.
        // Hacemos cast explícito para que compile.
        return (int)device.Id;
    }

    /// <summary>
    /// Envía una lectura de telemetría al BC Monitoring.
    /// </summary>
    public async Task IngestTelemetryReadingAsync(
        int projectId,
        int iotDeviceId,
        string metricName,
        decimal value,
        string unit,
        DateTime timestamp)
    {
        var command = new IngestReadingCommand(
            ProjectId:   projectId,
            IoTDeviceId: iotDeviceId,
            MetricName:  metricName,
            Value:       value,
            Unit:        unit,
            Timestamp:   timestamp
        );

        await _mediator.Send(command);
    }

    /// <summary>
    /// Crea una WorkItem asociada a un proyecto (y opcionalmente a un incidente).
    /// </summary>
    public async Task<int> CreateWorkItemForIncidentAsync(
        int projectId,
        int? incidentId,
        int assignedToUserId,
        string description)
    {
        // Convertir int a Guid (generar un GUID basado en el ID del usuario)
        var assignedToGuid = new Guid(assignedToUserId, 0, 0, new byte[8]);

        var command = new CreateWorkItemCommand(
            SpaceId: projectId,
            AssignedToRemodelerId: assignedToGuid,
            Description: description
        );

        var workItemId = await _mediator.Send(command);
        return workItemId;
    }
}