// Monitoring/Application/Internal/CommandServices/IngestReadingCommandHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public class IngestReadingCommandHandler : IRequestHandler<IngestReadingCommand, bool>
{
    private readonly IReadingRepository _readingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IngestReadingCommandHandler(
        IReadingRepository readingRepository,
        IUnitOfWork unitOfWork)
    {
        _readingRepository = readingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(IngestReadingCommand command, CancellationToken cancellationToken)
    {
        // Crear la entidad Reading a partir del comando
        var reading = new Reading(
            command.IoTDeviceId,
            command.SpaceId,
            command.MetricName,
            command.Value,
            command.Unit,
            command.Timestamp
        );

        // Guardar la lectura
        await _readingRepository.AddAsync(reading);

        // Confirmar cambios en BD
        await _unitOfWork.CompleteAsync();


        // Si llegamos aquí, asumimos éxito
        return true;
    }
}