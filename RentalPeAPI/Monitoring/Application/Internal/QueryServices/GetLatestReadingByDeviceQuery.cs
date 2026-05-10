// Monitoring/Application/Internal/QueryServices/GetLatestReadingByDeviceQuery.cs
using MediatR;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para obtener la última lectura registrada para un dispositivo IoT.
/// </summary>
public record GetLatestReadingByDeviceQuery(long IoTDeviceId)
    : IRequest<ReadingResource?>;