using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

public record ListIoTDevicesByProjectQuery(long ProjectId)
    : IRequest<IEnumerable<IoTDevice>>;