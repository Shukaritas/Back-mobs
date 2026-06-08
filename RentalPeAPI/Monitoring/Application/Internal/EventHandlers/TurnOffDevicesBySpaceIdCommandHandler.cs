using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para apagar automáticamente todos los dispositivos IoT vinculados a un espacio.
/// Se ejecuta cuando un proyecto se cancela para optimizar el uso de recursos.
/// </summary>
public class TurnOffDevicesBySpaceIdCommandHandler : IRequestHandler<TurnOffDevicesBySpaceIdCommand, Unit>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TurnOffDevicesBySpaceIdCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(TurnOffDevicesBySpaceIdCommand command, CancellationToken cancellationToken)
    {
        // Buscar todos los dispositivos asociados al espacio
        var devices = await _deviceRepository.ListBySpaceIdAsync(command.SpaceId);

        // Iterar sobre los dispositivos y apagar cada uno
        foreach (var device in devices)
        {
            device.TurnOff();
        }

        // Persistir todos los cambios
        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}

