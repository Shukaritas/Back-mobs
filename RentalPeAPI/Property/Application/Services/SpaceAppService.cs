using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Application.Internal.Dtos;
using RentalPeAPI.Property.Application.Internal.QueryServices;
using RentalPeAPI.Property.Domain.Aggregates;
using RentalPeAPI.Property.Domain.Aggregates.Enums;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Property.Application.Services;

public class SpaceAppService
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SpaceAppService(ISpaceRepository spaceRepository, IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _unitOfWork = unitOfWork;
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
        return spaces.Select(ToDto).ToList();
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