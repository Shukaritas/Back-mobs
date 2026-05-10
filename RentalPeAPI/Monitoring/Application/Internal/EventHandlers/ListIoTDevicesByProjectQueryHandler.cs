using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

public class ListIoTDevicesByProjectQueryHandler
    : IRequestHandler<ListIoTDevicesByProjectQuery, IEnumerable<IoTDevice>>
{
    private readonly IIoTDeviceRepository _deviceRepository;

    public ListIoTDevicesByProjectQueryHandler(IIoTDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<IEnumerable<IoTDevice>> Handle(
        ListIoTDevicesByProjectQuery query,
        CancellationToken cancellationToken)
    {
        return await _deviceRepository.ListBySpaceIdAsync(query.ProjectId);
    }
}