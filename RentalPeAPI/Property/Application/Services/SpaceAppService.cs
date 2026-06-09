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
        
        // El propietario es notificado que su solicitud ha sido aceptada
        var notificationCommandHomeowner = new CreateNotificationCommand(
            space.HomeownerId,
            space.Id,
            " Proyecto Aceptado",
            "El remodelador ha aceptado tu solicitud para el espacio. La obra está lista para iniciar."
        );
        await _mediator.Send(notificationCommandHomeowner);
        
        // El remodelador es notificado que ha sido asignado a este proyecto
        if (space.RemodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                space.RemodelerId.Value,
                space.Id,
                " Oferta Aceptada",
                "Has sido asignado a este proyecto. Ya puedes empezar a gestionar tus tareas."
            );
        }

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

    /// <summary>
    /// Obtiene todos los espacios asociados a un usuario.
    /// Retorna espacios donde el usuario es propietario (HomeownerId) o remodelador asignado (RemodelerId).
    /// </summary>
    public async Task<IEnumerable<SpaceDto>> GetSpacesByUserIdAsync(GetSpacesByUserIdQuery query)
    {
        var spaces = await _spaceRepository.ListAsync();
        
        // Filtrar espacios donde el usuario es owner o remodeler
        var userSpaces = spaces.Where(s => 
            s.HomeownerId == query.UserId || s.RemodelerId == query.UserId
        ).ToList();
        
        return userSpaces.Select(ToDto).ToList();
    }

    public async Task<SpaceDto?> CompleteProjectAsync(CompleteSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.SpaceId);
        if (space == null) return null;

        space.CompleteProject(command.RequestingUserId);
        await _unitOfWork.CompleteAsync();
        
        // El propietario confirma que la obra ha sido completada exitosamente
        await _monitoringFacade.DispatchNotificationAsync(
            space.HomeownerId,
            space.Id,
            $"Proyecto {space.Title} Finalizado",
            "Has marcado la obra como finalizada con éxito."
        );
        
        // Si existe un remodelador asignado, notificarle que el proyecto fue completado
        if (space.RemodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                space.RemodelerId.Value,
                space.Id,
                $"Proyecto {space.Title} Finalizado",
                $"El propietario  ha dado la conformidad final de la obra. ¡Excelente trabajo!"
            );
        }

        return ToDto(space);
    }

    public async Task<SpaceDto?> CancelProjectAsync(CancelSpaceCommand command)
    {
        var space = await _spaceRepository.FindByIdAsync(command.SpaceId);
        if (space == null) return null;

        // Guardar el RemodelerId antes de ejecutar la cancelación (por si se limpia en la lógica de dominio)
        var remodelerId = space.RemodelerId;

        space.CancelProject(command.RequestingUserId);
        await _unitOfWork.CompleteAsync();
        
        // El propietario cancela exitosamente la solicitud y se desactivan los sensores
        await _monitoringFacade.DispatchNotificationAsync(
            space.HomeownerId,
            space.Id,
            "Solicitud Cancelada",
            "Has cancelado exitosamente la solicitud para este espacio. Los sensores han sido desactivados."
        );
        
        // Si había un remodelador asignado, notificarle que el proyecto fue cancelado
        if (remodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                remodelerId.Value,
                space.Id,
                "Proyecto Cancelado",
                $"El propietario  ha cancelado el proyecto en el que estabas asignado."
            );
        }
        
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