using RentalPeAPI.Property.Domain.Aggregates.Entities;
using RentalPeAPI.Property.Domain.Aggregates.Enums;

namespace RentalPeAPI.Property.Domain.Aggregates;

/// <summary>
/// Agregado raíz que representa un espacio a remodelar en el Marketplace.
/// Un Homeowner publica un Space y un Remodeler lo acepta.
/// </summary>
public class Space
{
    public long Id { get; private set; }
    
    // Propietario que publica
    public Guid HomeownerId { get; private set; }
    
    // Remodeler que acepta la oferta (nullable hasta que se acepte)
    public Guid? RemodelerId { get; private set; }
    
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public SpaceType SpaceType { get; private set; }
    public decimal DimensionsSquareMeters { get; private set; }
    public decimal EstimatedBudget { get; private set; }
    public string Currency { get; private set; } = "PEN";
    public SpaceStatus Status { get; private set; } = SpaceStatus.Published;
    public bool HasIot { get; private set; }
    
    // Imágenes como URLs (colección de strings)
    public List<string> Images { get; private set; } = new();
    
    // Servicios asociados al espacio
    public List<Service> Services { get; private set; } = new();
    
    public DateTimeOffset PublishedAt { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }

    protected Space() { }

    public Space(
        Guid homeownerId,
        string title,
        string description,
        string location,
        SpaceType spaceType,
        decimal dimensionsSquareMeters,
        decimal estimatedBudget,
        string currency = "PEN",
        bool hasIot = false,
        IEnumerable<string>? images = null
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título no puede estar vacío.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción no puede estar vacía.", nameof(description));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("La ubicación no puede estar vacía.", nameof(location));
        if (dimensionsSquareMeters <= 0)
            throw new ArgumentException("Las dimensiones deben ser mayores a 0.", nameof(dimensionsSquareMeters));
        if (estimatedBudget < 0)
            throw new ArgumentException("El presupuesto no puede ser negativo.", nameof(estimatedBudget));

        HomeownerId = homeownerId;
        Title = title;
        Description = description;
        Location = location;
        SpaceType = spaceType;
        DimensionsSquareMeters = dimensionsSquareMeters;
        EstimatedBudget = estimatedBudget;
        Currency = currency;
        HasIot = hasIot;
        Status = SpaceStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;
        Images = images?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Acepta el proyecto por parte de un remodelador.
    /// Asigna el RemodelerId, establece AcceptedAt y cambia el Status a Accepted.
    /// Solo se puede invocar desde estado Published.
    /// </summary>
    public void AcceptProject(Guid remodelerId)
    {
        if (Status != SpaceStatus.Published)
            throw new InvalidOperationException("Solo se pueden aceptar proyectos de espacios publicados.");
        if (remodelerId == Guid.Empty)
            throw new ArgumentException("El ID del remodelador no puede estar vacío.", nameof(remodelerId));

        RemodelerId = remodelerId;
        Status = SpaceStatus.Accepted;
        AcceptedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Método heredado para compatibilidad. Usa AcceptProject() internamente.
    /// </summary>
    [Obsolete("Use AcceptProject() instead.")]
    public void AcceptOffer(Guid remodelerId)
    {
        AcceptProject(remodelerId);
    }

    /// <summary>
    /// Marca el espacio como en progreso.
    /// </summary>
    public void StartWork()
    {
        if (Status != SpaceStatus.Accepted)
            throw new InvalidOperationException("Solo se puede empezar trabajo en espacios aceptados.");
        
        Status = SpaceStatus.InProgress;
    }

    /// <summary>
    /// Marca el espacio como finalizado.
    /// </summary>
    public void FinishWork()
    {
        if (Status != SpaceStatus.InProgress)
            throw new InvalidOperationException("Solo se puede finalizar trabajo en espacios en progreso.");
        
        Status = SpaceStatus.Finished;
    }

    /// <summary>
    /// Cancela la remodelación.
    /// </summary>
    public void Cancel()
    {
        if (Status == SpaceStatus.Finished)
            throw new InvalidOperationException("No se pueden cancelar espacios finalizados.");
        
        Status = SpaceStatus.Cancelled;
        RemodelerId = null;
        AcceptedAt = null;
    }

    /// <summary>
    /// Actualiza las imágenes del espacio.
    /// </summary>
    public void UpdateImages(IEnumerable<string> newImages)
    {
        Images.Clear();
        Images.AddRange(newImages ?? new List<string>());
    }

    /// <summary>
    /// Actualiza los detalles básicos del espacio (solo si está publicado).
    /// </summary>
    public void UpdateDetails(
        string title,
        string description,
        string location,
        decimal dimensionsSquareMeters,
        decimal estimatedBudget,
        bool hasIot = false
    )
    {
        if (Status != SpaceStatus.Published)
            throw new InvalidOperationException("Solo se pueden actualizar detalles de espacios publicados.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título no puede estar vacío.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción no puede estar vacía.", nameof(description));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("La ubicación no puede estar vacía.", nameof(location));
        if (dimensionsSquareMeters <= 0)
            throw new ArgumentException("Las dimensiones deben ser mayores a 0.", nameof(dimensionsSquareMeters));
        if (estimatedBudget < 0)
            throw new ArgumentException("El presupuesto no puede ser negativo.", nameof(estimatedBudget));

        Title = title;
        Description = description;
        Location = location;
        DimensionsSquareMeters = dimensionsSquareMeters;
        EstimatedBudget = estimatedBudget;
        HasIot = hasIot;
    }
}
