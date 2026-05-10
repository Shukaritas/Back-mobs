using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

public class CreateIoTDeviceCommandHandler
    : IRequestHandler<CreateIoTDeviceCommand, IoTDevice>
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIoTDeviceCommandHandler(
        IIoTDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IoTDevice> Handle(
        CreateIoTDeviceCommand command,
        CancellationToken cancellationToken)
    {
        var device = new IoTDevice(
            command.ProjectId,
            command.Type,
            command.Name,
            command.SerialNumber
        );

        await _deviceRepository.AddAsync(device);
        await _unitOfWork.CompleteAsync();

        return device;
    }
}