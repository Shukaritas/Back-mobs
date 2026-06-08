using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para crear un dispositivo IoT con validación de ACL y autocompletado de métricas.
/// Implementa regla estricta: Solo se puede crear dispositivo si el espacio existe y HasIot = true.
/// 
/// REGLA DE CONGELAMIENTO DIFERENCIADO PARA IOTDEVICE:
/// - Si el estado del espacio es nulo o "Cancelled", lanza excepción.
/// - Los sensores NO se congelan si está en "Finished" (COMPLETED).
/// - Los propietarios pueden seguir monitoreando sus casas finalizadas.
/// </summary>
public class CreateIoTDeviceCommandHandler
    : IRequestHandler<CreateIoTDeviceCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<IoTDevice> Handle(
        CreateIoTDeviceCommand command,
        CancellationToken cancellationToken)
    {
        // Validar que el espacio existe y tiene tecnología IoT habilitada
        var spaceHasIot = await _propertyFacade.ValidateSpaceHasIoTEnabledAsync(command.SpaceId);
        if (!spaceHasIot)
        {
            throw new InvalidOperationException(
                $"El espacio con ID {command.SpaceId} no existe o no tiene tecnología IoT habilitada.");
        }

        // ===== VALIDACIÓN DE CONGELAMIENTO PARA SENSORES IoT =====
        // Obtener el estado del espacio desde la fachada ACL
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(command.SpaceId);
        
        // REGLA ESTRICTA: Si el estado es nulo o "Cancelled", denegar acción
        if (string.IsNullOrEmpty(spaceStatus) || spaceStatus == "Cancelled")
        {
            throw new InvalidOperationException(
                "Acción denegada: El espacio ha sido cancelado.");
        }

        // Crear el dispositivo con la lógica de autocompletado de métricas
        var device = new IoTDevice(
            command.SpaceId,
            command.CreatedByUserId,
            command.Type,
            command.Name,
            command.SerialNumber ?? string.Empty,
            command.CustomMetricName,
            command.CustomUnit,
            command.CustomMinThreshold,
            command.CustomMaxThreshold
        );

        await _deviceRepository.AddAsync(device);
        await _unitOfWork.CompleteAsync();

        return device;
    }
}