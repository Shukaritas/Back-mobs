using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para actualizar nombre y número de serie de un dispositivo IoT.
/// </summary>
public class UpdateIoTDeviceCommandHandler
    : IRequestHandler<UpdateIoTDeviceCommand, Unit>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
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

        device.UpdateDetails(command.Name, command.SerialNumber ?? string.Empty);
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

