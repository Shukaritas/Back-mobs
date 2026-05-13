using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para alternar el estado de encendido/apagado de un dispositivo IoT.
/// Valida que el usuario solicitante sea el creador del dispositivo.
/// </summary>
public class ToggleIoTDevicePowerCommandHandler
    : IRequestHandler<ToggleIoTDevicePowerCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleIoTDevicePowerCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
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

        // Alternar el estado
        device.TogglePower();

        // Guardar cambios
        await _unitOfWork.CompleteAsync();

        return device;
    }
}

