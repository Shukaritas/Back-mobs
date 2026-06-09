// Property/Application/Internal/EventHandlers/UpdateSpaceTotalPricingCommandHandler.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Property.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando UpdateSpaceTotalPricingCommand.
/// Evalúa si el costo total de las tareas de un espacio excede el presupuesto estimado.
/// Si hay sobrecosto y no ha sido notificado al propietario, despacha una alerta.
/// 
/// MOTOR DE ALERTA DE SOBRECOSTO:
/// 1. Actualiza el costo total acumulado (EndingPricing)
/// 2. Compara con el presupuesto estimado (EstimatedBudget)
/// 3. Si (EndingPricing > EstimatedBudget) y no notificado: marca como notificado y envía alerta
/// 4. Persite cambios en la base de datos
/// </summary>
public class UpdateSpaceTotalPricingCommandHandler : IRequestHandler<UpdateSpaceTotalPricingCommand, bool>
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IMonitoringContextFacade _monitoringFacade;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSpaceTotalPricingCommandHandler(
        ISpaceRepository spaceRepository,
        IMonitoringContextFacade monitoringFacade,
        IUnitOfWork unitOfWork)
    {
        _spaceRepository = spaceRepository;
        _monitoringFacade = monitoringFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSpaceTotalPricingCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el espacio
        var space = await _spaceRepository.FindByIdAsync(request.SpaceId) 
            ?? throw new KeyNotFoundException($"Space con ID {request.SpaceId} no encontrado.");

        // 2. Actualizar el costo total de las tareas del espacio
        space.UpdateEndingPricing(request.TotalPricing);

        // 3. Evaluar si hay sobrecosto
        bool isOverBudget = space.EndingPricing > space.EstimatedBudget;

        // 4. Si hay sobrecosto y aún no se ha notificado, despachar alerta
        if (isOverBudget && !space.IsOverBudgetNotified)
        {
            // Marcar como notificado para evitar múltiples alertas
            space.MarkAsOverBudgetNotified();

            // Construir el mensaje de alerta
            var overBudgetAmount = space.EndingPricing - space.EstimatedBudget;
            string alertMessage = 
                $"El costo total de las tareas ({space.Currency} {space.EndingPricing:F2}) " +
                $"ha superado tu presupuesto estimado inicial ({space.Currency} {space.EstimatedBudget:F2}). " +
                $"Contacta al remodelador si tienes inquietudes sobre los costos.";

            // Despachar notificación al propietario via Monitoring BC
            await _monitoringFacade.DispatchNotificationAsync(
                space.HomeownerId,
                space.Id,
                "Presupuesto Excedido",
                alertMessage
            );
        }

        // 5. Persisti cambios (incluyendo IsOverBudgetNotified si fue marcado)
        await _unitOfWork.CompleteAsync();

        return true;
    }
}

