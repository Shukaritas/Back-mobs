using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Application.Internal.Dtos;
using RentalPeAPI.Property.Application.Internal.QueryServices;
using RentalPeAPI.Property.Domain.Aggregates;
using RentalPeAPI.Property.Domain.Aggregates.Enums;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using MediatR;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.ACL;

namespace RentalPeAPI.Property.Application.Services;

public class SpaceAppService
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IMonitoringContextFacade _monitoringFacade;

    public SpaceAppService(
        ISpaceRepository spaceRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IMonitoringContextFacade monitoringFacade)
    {
        _spaceRepository = spaceRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _monitoringFacade = monitoringFacade;
    }

    public async Task<SpaceDto> CreateSpaceAsync(CreateSpaceCommand command)
    {
        var space = new Space(
            command.HomeownerId,
            command.Title,
            command.Description,
            command.Location,
            Enum.TryParse<SpaceType>(command.SpaceType, true, out var spaceType) ? spaceType : SpaceType.Other,
            command.DimensionsSquareMeters,
            command.EstimatedBudget,
            command.Currency,
            command.HasIot,
            command.Images
        );

        await _spaceRepository.AddAsync(space);
        await _unitOfWork.CompleteAsync();

        return ToDto(space);
    }

    public async Task<SpaceDto?> UpdateSpaceAsync(UpdateSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.Id);
        if (space == null) return null;

        space.UpdateDetails(
            command.Title,
            command.Description,
            command.Location,
            command.DimensionsSquareMeters,
            command.EstimatedBudget,
            command.HasIot
        );

        if (command.Images != null && command.Images.Any())
        {
            space.UpdateImages(command.Images);
        }

        await _unitOfWork.CompleteAsync();

        return ToDto(space);
    }

    public async Task<SpaceDto?> AcceptProjectAsync(AcceptSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.SpaceId);
        if (space == null) return null;

        space.AcceptProject(command.RemodelerId);
        await _unitOfWork.CompleteAsync();

        // PASO 5A: Despacho automático de notificación al homeowner (creador del espacio)
        // Alerta reactiva: "Proyecto Aceptado"
        var notificationCommand = new CreateNotificationCommand(
            space.HomeownerId,
            space.Id,
            "Proyecto Aceptado",
            "El remodelador ha aceptado tu solicitud para el espacio. La obra está lista para iniciar."
        );
        await _mediator.Send(notificationCommand);

        return ToDto(space);
    }

    /// <summary>
    /// Método heredado para compatibilidad. Usa AcceptProjectAsync internamente.
    /// </summary>
    [Obsolete("Use AcceptProjectAsync instead.")]
    public async Task<SpaceDto?> AcceptOfferAsync(long spaceId, Guid remodelerId)
    {
        var command = new AcceptSpaceCommand(spaceId, remodelerId);
        return await AcceptProjectAsync(command);
    }

    public async Task<bool> DeleteSpaceAsync(DeleteSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.Id);
        if (space == null) return false;

        _spaceRepository.Remove(space);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<SpaceDto?> GetSpaceByIdAsync(GetSpaceByIdQuery query)
    {
        var space = await _spaceRepository.FindByIdAsync(query.Id);
        return space != null ? ToDto(space) : null;
    }

    public async Task<IEnumerable<SpaceDto>> ListSpacesAsync(ListSpacesQuery query)
    {
        var spaces = await _spaceRepository.ListAsync();
        
        // Filtrar por OwnerId si se proporciona
        if (query.OwnerId.HasValue)
        {
            spaces = spaces.Where(s => s.HomeownerId == query.OwnerId.Value).ToList();
        }
        
        // Filtrar por Status si se proporciona
        if (!string.IsNullOrEmpty(query.Status))
        {
            if (Enum.TryParse<SpaceStatus>(query.Status, true, out var statusEnum))
            {
                spaces = spaces.Where(s => s.Status == statusEnum).ToList();
            }
        }
        
        return spaces.Select(ToDto).ToList();
    }

    public async Task<SpaceDto?> CompleteProjectAsync(CompleteSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.SpaceId);
        if (space == null) return null;

        space.CompleteProject(command.RequestingUserId);
        await _unitOfWork.CompleteAsync();

        // Despacho automático de notificación
        var notificationCommand = new CreateNotificationCommand(
            space.HomeownerId,
            space.Id,
            "Proyecto Completado",
            "Tu proyecto ha sido marcado como completado. ¡Gracias por usar RentalPe!"
        );
        await _mediator.Send(notificationCommand);

        return ToDto(space);
    }

    public async Task<SpaceDto?> CancelProjectAsync(CancelSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.SpaceId);
        if (space == null) return null;

        space.CancelProject(command.RequestingUserId);
        await _unitOfWork.CompleteAsync();

        // Despacho automático de notificación
        var notificationCommand = new CreateNotificationCommand(
            space.HomeownerId,
            space.Id,
            "Proyecto Cancelado",
            "Tu proyecto ha sido cancelado según lo solicitado."
        );
        await _mediator.Send(notificationCommand);

        // 🔌 Apagar automáticamente todos los dispositivos IoT del espacio cancelado
        // Esto optimiza recursos al desactivar sensores cuando el proyecto ya no está activo
        await _monitoringFacade.DisableAllDevicesForSpaceAsync(command.SpaceId);

        return ToDto(space);
    }

    private static SpaceDto ToDto(Space space)
    {
        return new SpaceDto
        {
            Id = space.Id,
            Title = space.Title,
            Description = space.Description,
            Location = space.Location,
            HomeownerId = space.HomeownerId,
            RemodelerId = space.RemodelerId,
            SpaceType = space.SpaceType.ToString(),
            DimensionsSquareMeters = space.DimensionsSquareMeters,
            EstimatedBudget = space.EstimatedBudget,
            Currency = space.Currency,
            Status = space.Status.ToString(),
            HasIot = space.HasIot,
            Images = space.Images,
            PublishedAt = space.PublishedAt,
            AcceptedAt = space.AcceptedAt
        };
    }
}