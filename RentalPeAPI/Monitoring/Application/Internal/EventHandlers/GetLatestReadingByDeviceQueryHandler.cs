// Monitoring/Application/Internal/QueryServices/GetLatestReadingByDeviceQueryHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

public class GetLatestReadingByDeviceQueryHandler 
    : IRequestHandler<GetLatestReadingByDeviceQuery, ReadingResource?>
{
    private readonly IReadingRepository _readingRepository;

    public GetLatestReadingByDeviceQueryHandler(IReadingRepository readingRepository)
    {
        _readingRepository = readingRepository;
    }

    public async Task<ReadingResource?> Handle(GetLatestReadingByDeviceQuery query, CancellationToken cancellationToken)
    {
        var reading = await _readingRepository.FindLatestByDeviceIdAsync(query.IoTDeviceId);

        if (reading is null) return null;

        return new ReadingResource(
            reading.Id,
            reading.IoTDeviceId,
            reading.SpaceId,
            reading.MetricName,
            reading.Value,
            reading.Unit,
            reading.Timestamp
        );
    }
}