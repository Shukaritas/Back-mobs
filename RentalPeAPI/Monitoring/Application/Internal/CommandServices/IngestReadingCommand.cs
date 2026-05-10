
// Monitoring/Application/Internal/CommandServices/IngestReadingCommand.cs
using System;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public record IngestReadingCommand(
    int ProjectId,
    int IoTDeviceId,
    string MetricName,
    decimal Value,
    string Unit,
    DateTime Timestamp
) : IRequest<bool>;
