using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Interfaces.Rest.Resources;

namespace RentalPeAPI.Property.Interfaces.Rest.Transform
{
    public static class SpaceCommandAssembler
    {
        public static CreateSpaceCommand ToCommand(CreateSpaceResource resource)
            => new(
                homeownerId: Guid.NewGuid(), // TODO: Obtener del contexto del usuario autenticado
                title: resource.Name,
                description: resource.Description,
                location: resource.Address,
                spaceType: resource.Type,
                dimensionsSquareMeters: resource.AreaM2,
                estimatedBudget: resource.PricePerHour, // Usar PricePerHour como EstimatedBudget
                currency: "PEN", // Valor por defecto
                hasIot: false, // Valor por defecto
                images: resource.Services ?? new List<string>() // Usar Services como images
            );

        public static UpdateSpaceCommand ToCommand(long id, UpdateSpaceResource resource)
            => new(
                id: id,
                title: resource.Name,
                description: resource.Description,
                location: resource.Address,
                dimensionsSquareMeters: resource.AreaM2,
                estimatedBudget: resource.PricePerHour, // Usar PricePerHour como EstimatedBudget
                hasIot: false, // Valor por defecto
                images: resource.Services ?? new List<string>() // Usar Services como images
            );
    }
}