using RentalPeAPI.Property.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Implementación de la Anti-Corruption Layer para Property BC.
/// Proporciona acceso a funcionalidades del contexto de Property sin exponer detalles internos.
/// </summary>
public class PropertyContextFacade : IPropertyContextFacade
{
    private readonly ISpaceRepository _spaceRepository;

    public PropertyContextFacade(ISpaceRepository spaceRepository)
    {
        _spaceRepository = spaceRepository;
    }

    /// <summary>
    /// Valida que un espacio exista y tenga habilitada la tecnología IoT.
    /// </summary>
    public async Task<bool> ValidateSpaceHasIoTEnabledAsync(long spaceId)
    {
        var space = await _spaceRepository.FindByIdAsync(spaceId);
        return space != null && space.HasIot;
    }
}

