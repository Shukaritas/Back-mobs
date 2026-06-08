namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Anti-Corruption Layer para acceder al Bounded Context de Property.
/// Permite validar espacios (Spaces) sin exponer detalles internos del contexto.
/// </summary>
public interface IPropertyContextFacade
{
    /// <summary>
    /// Valida que un espacio exista y tenga habilitada la tecnología IoT.
    /// </summary>
    /// <param name="spaceId">ID del espacio a validar</param>
    /// <returns>true si el espacio existe y HasIot es true; false en caso contrario</returns>
    Task<bool> ValidateSpaceHasIoTEnabledAsync(long spaceId);

    /// <summary>
    /// Obtiene el estado actual de un espacio.
    /// </summary>
    /// <param name="spaceId">ID del espacio a consultar</param>
    /// <returns>El estado del espacio como string (ej: "Published", "Accepted", "Finished", "Cancelled"), o null si no existe</returns>
    Task<string?> GetSpaceStatusAsync(long spaceId);
}



