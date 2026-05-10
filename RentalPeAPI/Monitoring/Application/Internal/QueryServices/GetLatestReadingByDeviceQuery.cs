// Monitoring/Application/Internal/QueryServices/GetLatestReadingByDeviceQuery.cs
using MediatR;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

public record GetLatestReadingByDeviceQuery(int IoTDeviceId)
    : IRequest<ReadingResource?>;