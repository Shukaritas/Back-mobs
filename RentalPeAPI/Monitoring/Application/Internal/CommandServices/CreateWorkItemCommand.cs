using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public record CreateWorkItemCommand(
    long SpaceId,
    Guid AssignedToRemodelerId,
    string Description
) : IRequest<int>; 