using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

public class WorkItem
{
    public int Id { get; set; }

    // Referencia al espacio (Space)
    public long SpaceId { get; set; }
    
    // Remodeler asignado
    public Guid AssignedToRemodelerId { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public WorkItem() { }

    public WorkItem(long spaceId, Guid assignedToRemodelerId, string description)
    {
        if (spaceId <= 0)
            throw new ArgumentException("Space ID is required.", nameof(spaceId));
        if (assignedToRemodelerId == Guid.Empty)
            throw new ArgumentException("Remodeler ID is required.", nameof(assignedToRemodelerId));

        SpaceId = spaceId;
        AssignedToRemodelerId = assignedToRemodelerId;
        Description = description;
        Status = "pending";
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    public void MarkCompleted()
    {
        if (Status != "completed")
        {
            Status = "completed";
            CompletedAt = DateTime.UtcNow;
        }
    }
}