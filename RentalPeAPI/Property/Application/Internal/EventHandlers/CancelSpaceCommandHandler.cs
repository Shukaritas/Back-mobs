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
    /// Procesa la cancelación del espacio, apaga dispositivos IoT y despacha notificaciones bifurcadas.
    /// Notifica tanto al Homeowner como al Remodelador (si está asignado).
    /// </summary>
    public async Task<bool> HandleAsync(CancelSpaceCommand request)
    {
        // Obtener el espacio
        var space = await _spaceRepository.FindByIdAsync(request.SpaceId);
        if (space == null)
            return false;

        // Guardar el RemodelerId antes de ejecutar la cancelación (por si se limpia en la lógica de dominio)
        var remodelerId = space.RemodelerId;

        // Ejecutar la lógica de dominio de cancelación
        space.CancelProject(request.RequestingUserId);
        
        // Persistir cambios en Property
        await _unitOfWork.CompleteAsync();

        // 📢 NOTIFICACIÓN AL HOMEOWNER (PROPIETARIO)
        // El propietario cancela exitosamente la solicitud y se desactivan los sensores
        await _monitoringFacade.DispatchNotificationAsync(
            space.HomeownerId,
            request.SpaceId,
            "Solicitud Cancelada",
            "Has cancelado exitosamente la solicitud para este espacio. Los sensores han sido desactivados."
        );

        // 📢 NOTIFICACIÓN AL REMODELADOR (SI APLICA)
        // Si había un remodelador asignado, notificarle que el proyecto fue cancelado
        if (remodelerId.HasValue)
        {
            await _monitoringFacade.DispatchNotificationAsync(
                remodelerId.Value,
                request.SpaceId,
                "Proyecto Cancelado",
                "El propietario ha cancelado el proyecto en el que estabas asignado."
            );
        }

        // 🔌 Apagar automáticamente todos los dispositivos IoT del espacio
        // Esto se ejecuta DESPUÉS de confirmar la cancelación del espacio y despachar notificaciones
        await _monitoringFacade.DisableAllDevicesForSpaceAsync(request.SpaceId);

        return true;
    }
}

