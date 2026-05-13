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
}

