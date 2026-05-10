namespace RentalPeAPI.Property.Domain.Aggregates.Enums;

/// <summary>
/// Estados de un espacio en el ciclo de vida de una remodelación.
/// </summary>
public enum SpaceStatus
{
    /// <summary>Espacio publicado y disponible para aceptar ofertas</summary>
    Published = 0,

    /// <summary>Oferta aceptada por un remodeler</summary>
    Accepted = 1,

    /// <summary>Remodelación en progreso</summary>
    InProgress = 2,

    /// <summary>Remodelación finalizada</summary>
    Finished = 3,

    /// <summary>Remodelación cancelada</summary>
    Cancelled = 4
}

