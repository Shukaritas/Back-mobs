using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public record CreateIoTDeviceCommand(
    long ProjectId,
    string Type,
    string? Name,
    string? SerialNumber
) : IRequest<IoTDevice>;