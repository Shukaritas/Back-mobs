using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Domain;
using SharedIUnitOfWork = RentalPeAPI.Shared.Domain.Repositories.IUnitOfWork;

namespace RentalPeAPI.User.Application.Internal.CommandServices;

public class AddPaymentMethodCommandHandler
    : IRequestHandler<AddPaymentMethodCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly SharedIUnitOfWork _unitOfWork;

    public AddPaymentMethodCommandHandler(
        IUserRepository userRepository,
        IPaymentMethodRepository paymentMethodRepository,
        SharedIUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(AddPaymentMethodCommand command, CancellationToken cancellationToken)
    {
        // 1. Obtenemos el usuario (solo para validar y para devolverlo luego)
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user is null)
            throw new Exception("Usuario no encontrado.");

        // 2. Creamos explícitamente un PaymentMethod como entidad independiente
        var paymentMethod = new PaymentMethod(
            Guid.NewGuid(),
            user.Id,
            command.Type,
            command.Number,
            command.Expiry,
            command.Cvv
        );

        await _paymentMethodRepository.AddAsync(paymentMethod);

        // 3. Guardamos cambios
        await _unitOfWork.CompleteAsync();

        // 4. Volvemos a cargar el usuario con sus métodos de pago actualizados
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        if (updatedUser is null)
            throw new Exception("Usuario no encontrado después de actualizar.");

        var paymentMethodsDto = updatedUser.PaymentMethods
            .Select(pm => new PaymentMethodDto(pm.Id, pm.Type, pm.Number, pm.Expiry, pm.Cvv))
            .ToList();

        return new UserDto(
            updatedUser.Id,
            updatedUser.FullName,
            updatedUser.Email,
            updatedUser.Phone,
            updatedUser.CreatedAt,
            updatedUser.Role,
            updatedUser.Photo,
            paymentMethodsDto
        );
    }
}
