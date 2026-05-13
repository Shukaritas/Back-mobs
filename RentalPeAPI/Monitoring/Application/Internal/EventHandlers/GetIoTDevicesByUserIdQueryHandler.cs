using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para obtener todos los dispositivos IoT creados por un usuario específico.
/// Simula telemetría continua para cada dispositivo encendido.
/// </summary>
public class GetIoTDevicesByUserIdQueryHandler
    : IRequestHandler<GetIoTDevicesByUserIdQuery, IEnumerable<IoTDevice>>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetIoTDevicesByUserIdQueryHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<IoTDevice>> Handle(
        GetIoTDevicesByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        // Obtener todos los dispositivos creados por el usuario
        var devices = await _deviceRepository.ListByCreatedByUserIdAsync(query.UserId);

        // Simular telemetría: invocar GenerateRandomValue() para cada dispositivo encendido
        foreach (var device in devices)
        {
            device.GenerateRandomValue();
        }

        // Persistir los cambios en la base de datos
        await _unitOfWork.CompleteAsync();

        return devices;
    }
}

