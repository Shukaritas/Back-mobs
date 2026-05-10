using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Handler para la query de dispositivos IoT en un espacio.
/// </summary>
public class ListIoTDevicesBySpaceQueryHandler
    : IRequestHandler<ListIoTDevicesBySpaceQuery, IEnumerable<IoTDevice>>
{
    private readonly IIoTDeviceRepository _deviceRepository;

    public ListIoTDevicesBySpaceQueryHandler(IIoTDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<IEnumerable<IoTDevice>> Handle(
        ListIoTDevicesBySpaceQuery query,
        CancellationToken cancellationToken)
    {
        return await _deviceRepository.ListBySpaceIdAsync(query.SpaceId);
    }
}