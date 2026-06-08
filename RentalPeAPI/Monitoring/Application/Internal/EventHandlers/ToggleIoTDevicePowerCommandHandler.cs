using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para alternar el estado de encendido/apagado de un dispositivo IoT.
/// Valida que el usuario solicitante sea el creador del dispositivo.
/// 
/// REGLA DE CONGELAMIENTO DIFERENCIADO PARA IOTDEVICE:
/// - Si el estado del espacio es nulo o "Cancelled", lanza excepción.
/// - Los sensores NO se congelan si está en "Finished" (COMPLETED).
/// </summary>
public class ToggleIoTDevicePowerCommandHandler
    : IRequestHandler<ToggleIoTDevicePowerCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleIoTDevicePowerCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<IoTDevice> Handle(
        ToggleIoTDevicePowerCommand command,
        CancellationToken cancellationToken)
    {
        // Buscar el dispositivo
        var device = await _deviceRepository.FindByIdAsync(command.DeviceId);
        if (device == null)
        {
            throw new KeyNotFoundException(
                $"Dispositivo IoT con ID {command.DeviceId} no encontrado.");
        }

        // Validar que el usuario solicitante sea el creador del dispositivo
        if (device.CreatedByUserId != command.RequestingUserId)
        {
            throw new UnauthorizedAccessException(
                $"El usuario {command.RequestingUserId} no tiene autorización para alternar el estado del dispositivo {command.DeviceId}.");
        }

        // ===== VALIDACIÓN DE CONGELAMIENTO PARA SENSORES IoT =====
        // Obtener el estado del espacio desde la fachada ACL
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(device.SpaceId);
        
        // REGLA ESTRICTA: Si el estado es nulo o "Cancelled", denegar acción
        if (string.IsNullOrEmpty(spaceStatus) || spaceStatus == "Cancelled")
        {
            throw new InvalidOperationException(
                "Acción denegada: El espacio ha sido cancelado.");
        }

        // Alternar el estado
        device.TogglePower();

        // Guardar cambios
        await _unitOfWork.CompleteAsync();

        return device;
    }
}

