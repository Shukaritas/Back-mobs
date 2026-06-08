using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;

namespace RentalPeAPI.Property.Application.Internal.EventHandlers;

/// <summary>
/// Handler para el comando CancelSpaceCommand.
/// Ejecuta la cancelación de un espacio y automáticamente apaga todos los dispositivos IoT asociados.
/// </summary>
public class CancelSpaceCommandHandler
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMonitoringContextFacade _monitoringFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSpaceCommandHandler(
        ISpaceRepository spaceRepository,
        IMonitoringContextFacade monitoringFacade,
        IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _monitoringFacade = monitoringFacade;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Procesa la cancelación del espacio y dispara automáticamente el apagado de dispositivos.
    /// </summary>
    public async Task<bool> HandleAsync(CancelSpaceCommand request)
    {
        // Obtener el espacio
        var space = await _spaceRepository.FindByIdAsync(request.SpaceId);
        if (space == null)
            return false;

        // Ejecutar la lógica de dominio de cancelación
        space.CancelProject(request.RequestingUserId);
        
        // Persistir cambios en Property
        await _unitOfWork.CompleteAsync();

        // 🔌 Apagar automáticamente todos los dispositivos IoT del espacio
        // Esto se ejecuta DESPUÉS de confirmar la cancelación del espacio
        await _monitoringFacade.DisableAllDevicesForSpaceAsync(request.SpaceId);

        return true;
    }
}

