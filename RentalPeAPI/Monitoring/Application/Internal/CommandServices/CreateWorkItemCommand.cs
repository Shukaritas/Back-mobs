using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para crear una nueva tarea (WorkItem) vinculada a un espacio.
/// Puede ser ejecutado por el dueño del espacio o por un remodelador.
/// </summary>
public record CreateWorkItemCommand(
    long SpaceId,
    Guid CreatedByUserId,
    string Title,
    string Description,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate
) : IRequest<int>; 