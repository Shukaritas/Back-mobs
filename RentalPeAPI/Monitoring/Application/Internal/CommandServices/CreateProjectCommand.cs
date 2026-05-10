using System;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public record CreateProjectCommand(
    Guid UserId,          // primero el usuario que crea el proyecto
    string Name,
    string Description,
    DateTime StartDate,
    
    int? PropertyId,      // ahora puede ser null (como en el dbjson)
    DateTime? EndDate     // también puede ser null
) : IRequest<int>;        // devuelve el Id del nuevo proyecto