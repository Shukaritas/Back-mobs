using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para actualizar nombre y número de serie de un dispositivo IoT.
/// 
/// REGLA DE CONGELAMIENTO DIFERENCIADO PARA IOTDEVICE:
/// - Si el estado del espacio es nulo o "Cancelled", lanza excepción.
/// - Los sensores NO se congelan si está en "Finished" (COMPLETED).
/// </summary>
public class UpdateIoTDeviceCommandHandler
    : IRequestHandler<UpdateIoTDeviceCommand, Unit>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UpdateIoTDeviceCommand command,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.FindByIdAsync(command.DeviceId);
        if (device == null)
        {
            throw new KeyNotFoundException(
                $"Dispositivo IoT con ID {command.DeviceId} no encontrado.");
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

        device.UpdateDetails(command.Name, command.SerialNumber ?? string.Empty);
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

