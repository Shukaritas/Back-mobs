// Monitoring/Application/Internal/CommandServices/UpdateWorkItemStatusCommand.cs
using MediatR;
using System;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para actualizar el estado de una tarea (WorkItem).
/// Valida que solo el remodelador asignado al espacio puede hacer cambios.
/// </summary>
public record UpdateWorkItemStatusCommand(
    int TaskId,
    string Status,
    Guid RequestingUserId
) : IRequest<bool>;

