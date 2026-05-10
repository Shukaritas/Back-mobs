using MediatR;
using System.Linq; // Para Select sobre PaymentMethods
using System.Threading;
using System.Threading.Tasks;

using RentalPeAPI.User.Application.Internal.CommandServices; // UserDto, PaymentMethodDto
using RentalPeAPI.User.Domain.Repositories;

namespace RentalPeAPI.User.Application.Internal.QueryServices;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        // Traemos el usuario desde el repositorio
        var user = await _userRepository.GetByIdAsync(query.UserId);

        if (user is null)
        {
            return null;
        }

        // Mapeamos la lista de PaymentMethods del dominio al DTO
        var paymentMethodsDto = user.PaymentMethods
            .Select(pm => new PaymentMethodDto(
                pm.Id,
                pm.Type,
                pm.Number,
                pm.Expiry,
                pm.Cvv
            ))
            .ToList();

        // Devolvemos el UserDto completo, incluyendo paymentMethods
        return new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Phone,
            user.CreatedAt,
            user.Role,
            user.Photo,
            paymentMethodsDto
        );
    }
}