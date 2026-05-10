
// Monitoring/Application/Internal/CommandServices/IngestReadingCommand.cs
using System;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para ingestar una lectura de telemetría desde un dispositivo IoT en un espacio.
/// </summary>
public record IngestReadingCommand(
    long SpaceId,
    long IoTDeviceId,
    string MetricName,
    decimal Value,
    string Unit,
    DateTime Timestamp
) : IRequest<bool>;
