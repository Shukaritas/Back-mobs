using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para crear un dispositivo IoT vinculado a un espacio (Space).
/// </summary>
public record CreateIoTDeviceCommand(
    long SpaceId,
    string Type,
    string? Name,
    string? SerialNumber
) : IRequest<IoTDevice>;