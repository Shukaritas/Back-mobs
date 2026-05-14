// Monitoring/Interfaces/REST/Controllers/ReadingsController.cs
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador refactorizado para gestionar lecturas de telemetría de dispositivos IoT.
/// Ya no usa tabla de Readings. Consulta directamente los IoTDevices y simula telemetría.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Tags("Readings")]
[Authorize] // ← CRÍTICO: Requiere autenticación JWT
public class ReadingsController : ControllerBase
{
    private readonly IIoTDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReadingsController(IIoTDeviceRepository deviceRepository, IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Método privado helper para extraer propiedades privadas del agregado IoTDevice.
    /// Utiliza Reflection para acceder a MinThreshold, MaxThreshold e IsInAlertState
    /// que son gestionadas internamente por el dominio.
    /// </summary>
    private static (decimal MinThreshold, decimal MaxThreshold, bool IsInAlertState) ExtractPrivateProperties(object device)
    {
        var minThresholdProp = device.GetType().GetProperty("MinThreshold", BindingFlags.NonPublic | BindingFlags.Instance);
        var maxThresholdProp = device.GetType().GetProperty("MaxThreshold", BindingFlags.NonPublic | BindingFlags.Instance);
        var isInAlertStateProp = device.GetType().GetProperty("IsInAlertState", BindingFlags.NonPublic | BindingFlags.Instance);

        var minThreshold = minThresholdProp?.GetValue(device) as decimal? ?? 0m;
        var maxThreshold = maxThresholdProp?.GetValue(device) as decimal? ?? 100m;
        var isInAlertState = isInAlertStateProp?.GetValue(device) as bool? ?? false;

        return (minThreshold, maxThreshold, isInAlertState);
    }

    /// <summary>
    /// GET: /api/v1/monitoring/readings/device/{deviceId}
    /// Devuelve el detalle completo de un dispositivo específico con telemetría, umbrales y estado de alerta.
    /// Antes de retornar, invoca GenerateRandomValue() y persiste los cambios.
    /// </summary>
    [HttpGet("device/{deviceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDeviceDetail(long deviceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            // Recuperar el dispositivo
            var device = await _deviceRepository.FindByIdAsync(deviceId);
            if (device == null)
                return NotFound(new { error = $"Dispositivo con ID {deviceId} no encontrado." });

            // Invocar simulación de telemetría
            device.GenerateRandomValue();

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Extraer propiedades privadas usando Reflection
            var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(device);

            // Mapear a DTO extendido con toda la información
            var resource = new IoTDeviceDetailExtendedResource(
                device.Id,
                device.SpaceId,
                device.Type,
                device.Name,
                device.SerialNumber,
                device.MetricName,
                device.Unit,
                device.Value,
                device.Timestamp,
                device.IsOn,
                minThreshold,
                maxThreshold,
                isInAlertState
            );

            return Ok(resource);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener detalle del dispositivo.", details = ex.Message });
        }
    }


    /// <summary>
    /// GET: /api/v1/monitoring/readings/space/{spaceId}
    /// Devuelve la lista de detalles de todos los dispositivos de un espacio con telemetría, umbrales y estado de alerta.
    /// Antes de retornar, invoca GenerateRandomValue() para cada dispositivo y persiste los cambios.
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSpaceReadings(long spaceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            // Recuperar dispositivos del espacio
            var devices = await _deviceRepository.ListBySpaceIdAsync(spaceId);
            if (!devices.Any())
                return NotFound(new { error = $"No se encontraron dispositivos para el espacio {spaceId}." });

            // Invocar simulación de telemetría para cada dispositivo
            foreach (var device in devices)
            {
                device.GenerateRandomValue();
            }

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Mapear a DTOs extendidos con toda la información
            var resources = devices.Select(d =>
            {
                var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(d);
                return new IoTDeviceDetailExtendedResource(
                    d.Id,
                    d.SpaceId,
                    d.Type,
                    d.Name,
                    d.SerialNumber,
                    d.MetricName,
                    d.Unit,
                    d.Value,
                    d.Timestamp,
                    d.IsOn,
                    minThreshold,
                    maxThreshold,
                    isInAlertState
                );
            }).ToList();

            return Ok(resources);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener lecturas del espacio.", details = ex.Message });
        }
    }

    /// <summary>
    /// GET: /api/v1/monitoring/readings/user
    /// Extrae el ID del usuario del token JWT y devuelve los detalles de todos sus dispositivos con telemetría, umbrales y estado de alerta.
    /// Antes de retornar, invoca GenerateRandomValue() para cada dispositivo y persiste los cambios.
    /// </summary>
    [HttpGet("user")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserReadings()
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier válido." });

        try
        {
            // Recuperar dispositivos del usuario
            var devices = await _deviceRepository.ListByCreatedByUserIdAsync(userId);
            if (!devices.Any())
                return NotFound(new { error = "No se encontraron dispositivos para el usuario autenticado." });

            // Invocar simulación de telemetría para cada dispositivo
            foreach (var device in devices)
            {
                device.GenerateRandomValue();
            }

            // Persistir cambios
            await _unitOfWork.CompleteAsync();

            // Mapear a DTOs extendidos con toda la información
            var resources = devices.Select(d =>
            {
                var (minThreshold, maxThreshold, isInAlertState) = ExtractPrivateProperties(d);
                return new IoTDeviceDetailExtendedResource(
                    d.Id,
                    d.SpaceId,
                    d.Type,
                    d.Name,
                    d.SerialNumber,
                    d.MetricName,
                    d.Unit,
                    d.Value,
                    d.Timestamp,
                    d.IsOn,
                    minThreshold,
                    maxThreshold,
                    isInAlertState
                );
            }).ToList();

            return Ok(resources);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al obtener lecturas del usuario.", details = ex.Message });
        }
    }
}